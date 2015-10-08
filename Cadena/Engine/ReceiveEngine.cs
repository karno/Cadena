﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cadena.Engine._Internals;
using JetBrains.Annotations;

namespace Cadena.Engine
{
    public sealed class ReceiveEngine : IDisposable
    {
        private readonly CancellationTokenSource _tokenSource;

        private readonly EngineTaskManager _manager;

        private readonly SortedList<DateTime, List<Tuple<IReceiver, RequestPriority>>> _receivers;

        private readonly ManualResetEvent _re;

        public ReceiveEngine()
        {
            _tokenSource = new CancellationTokenSource();
            _manager = new EngineTaskManager(4, _tokenSource.Token);
            _receivers = new SortedList<DateTime, List<Tuple<IReceiver, RequestPriority>>>();
            _re = new ManualResetEvent(false);
        }

        public void RegisterReceiver([NotNull] IReceiver receiver, RequestPriority priority = RequestPriority.Middle)
        {
            if (receiver == null) throw new ArgumentNullException(nameof(receiver));
            RegisterReceiver(DateTime.Now, receiver, priority);
        }

        private void RegisterReceiver(DateTime key, [NotNull] IReceiver receiver, RequestPriority priority)
        {
            if (receiver == null) throw new ArgumentNullException(nameof(receiver));
            if (_tokenSource.Token.IsCancellationRequested) return;
            bool firstChanged;
            lock (_receivers)
            {
                if (!_receivers.ContainsKey(key))
                {
                    _receivers[key] = new List<Tuple<IReceiver, RequestPriority>>();
                }
                _receivers[key].Add(Tuple.Create(receiver, priority));
                firstChanged = _receivers.Values[0] == _receivers[key];
            }
            if (firstChanged)
            {
                // registered item is needed to call before than previous first item.
                _re.Set();
            }
        }

        public bool UnregisterReceiver([NotNull] IReceiver receiver)
        {
            if (receiver == null) throw new ArgumentNullException(nameof(receiver));
            RequestPriority _;
            return UnregisterReceiver(receiver, out _);
        }

        private bool UnregisterReceiver([NotNull] IReceiver receiver, out RequestPriority priority)
        {
            if (receiver == null) throw new ArgumentNullException(nameof(receiver));
            lock (_receivers)
            {
                // iterate P<DT, List<T<IR, RP>>>
                foreach (var pair in _receivers)
                {
                    // iterate T<IR, RP>
                    foreach (var tuple in pair.Value)
                    {
                        if (tuple.Item1 == receiver)
                        {
                            pair.Value.Remove(tuple);
                            if (pair.Value.Count == 0)
                            {
                                // remove empty list
                                _receivers.Remove(pair.Key);
                            }
                            priority = tuple.Item2;
                            return true;
                        }
                    }
                }
            }
            // dummy value
            priority = RequestPriority.Low;
            return false;
        }

        public void Begin()
        {
            Task.Factory.StartNew(EngineCore, TaskCreationOptions.LongRunning);
        }

        private void EngineCore()
        {
            var token = _tokenSource.Token;
            while (!token.IsCancellationRequested)
            {
                var span = Timeout.InfiniteTimeSpan;
                // stage 1: calculate wait time
                lock (_receivers)
                {
                    if (_receivers.Count > 0)
                    {
                        var time = _receivers.Keys[0];
                        var now = DateTime.Now;
                        span = time <= now ? TimeSpan.Zero : _receivers.Keys[0] - now;
                    }
                }
                // stage 2: wait
                if (span == Timeout.InfiniteTimeSpan || span > TimeSpan.Zero)
                {
                    _re.WaitOne(span);
                }
                // stage 3: check time and dequeue item
                List<Tuple<IReceiver, RequestPriority>> list = null;
                lock (_receivers)
                {
                    if (_receivers.Count > 0)
                    {
                        var time = _receivers.Keys[0];
                        if (time <= DateTime.Now)
                        {
                            // execute tasks
                            list = _receivers.Values[0];
                            _receivers.RemoveAt(0);
                        }
                    }
                }
                // stage 4: execute receive task
                if (list != null)
                {
                    foreach (var tuple in list)
                    {
                        Receive(tuple.Item1, tuple.Item2);
                    }
                }
            }
        }

        public void Receive([NotNull] IReceiver receiver)
        {
            if (receiver == null) throw new ArgumentNullException(nameof(receiver));
            RequestPriority priority;
            if (!UnregisterReceiver(receiver, out priority))
            {
                // receiver not found
                throw new ArgumentException("Specified receiver is not contained in this receive engine.");
            }
            Receive(receiver, priority);
        }

        public void ReceiveAll()
        {
            List<Tuple<IReceiver, RequestPriority>>[] items;
            lock (_receivers)
            {
                items = _receivers.Values.ToArray();
                _receivers.Clear();
            }
            foreach (var item in items.SelectMany(i => i))
            {
                Receive(item.Item1, item.Item2);
            }
        }

        private void Receive([NotNull] IReceiver receiver, RequestPriority priority)
        {
            if (receiver == null) throw new ArgumentNullException(nameof(receiver));
            var func = new Func<CancellationToken, Task>(async token =>
            {
                var span = await receiver.ExecuteAsync(token).ConfigureAwait(false);
                var now = DateTime.Now;
                if (token.IsCancellationRequested) return;
                if (span.Ticks > (DateTime.MaxValue - now).Ticks)
                {
                    // register as MaxValue
                    RegisterReceiver(DateTime.MaxValue, receiver, priority);
                }
                else
                {
                    RegisterReceiver(now + span, receiver, priority);
                }
            });
            _manager.Request(func, priority);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ReceiveEngine()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _tokenSource.Cancel();
                _tokenSource.Dispose();
                _re.Set();
                _re.Dispose();
            }
        }
    }
}