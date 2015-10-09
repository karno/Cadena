using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cadena.Engine._Internals
{
    /// <summary>
    /// Priority task manager
    /// </summary>
    internal sealed class EngineTaskManager
    {
        private readonly int _maxWorkingThreads;

        private readonly CancellationToken _token;

        private int _workingThreadCount;

        private int _unconsumedTaskCount;

        private readonly ConcurrentQueue<Func<CancellationToken, Task>>[] _queues;

        public EngineTaskManager(int maxWorkingThreads, CancellationToken token)
        {
            _maxWorkingThreads = maxWorkingThreads;
            _token = token;
            _queues = Enumerable.Range(0, 3).Select(_ => new ConcurrentQueue<Func<CancellationToken, Task>>())
                                .ToArray();
        }

        public void Request(Func<CancellationToken, Task> task, RequestPriority priority)
        {
            _queues[(int)priority].Enqueue(task);
            Run();
        }

        public void Request(Func<Task> task, RequestPriority priority)
        {
            _queues[(int)priority].Enqueue(_ => task());
            Run();
        }


        private void Run()
        {
            if (_token.IsCancellationRequested) return;
            Interlocked.Increment(ref _unconsumedTaskCount);
            if (Interlocked.Increment(ref _workingThreadCount) < _maxWorkingThreads)
            {
                Interlocked.Exchange(ref _unconsumedTaskCount, 0);
                Task.Run(() => RunSingle(), _token);
            }
            else
            {
                Interlocked.Decrement(ref _workingThreadCount);
            }
        }

        private Func<CancellationToken, Task> DequeueOne()
        {
            foreach (var queue in _queues)
            {
                Func<CancellationToken, Task> item;
                if (queue.TryDequeue(out item))
                {
                    return item;
                }
            }
            return null;
        }

        private async void RunSingle()
        {
            while (true)
            {
                Func<CancellationToken, Task> item;
                while ((item = DequeueOne()) != null && !_token.IsCancellationRequested)
                {
                    try
                    {
                        await item(_token).ConfigureAwait(false);
                    }
                    catch
                    {
                        // ignored
                    }
                    Interlocked.Exchange(ref _unconsumedTaskCount, 0);
                }

                // decrement thread counter
                var wtc = Interlocked.Decrement(ref _workingThreadCount);

                if (_token.IsCancellationRequested)
                {
                    return;
                }

                // quit this task when
                // * other tasks are still alive 
                // or
                // * no tasks has been registered that was not spawn any tasks
                if (wtc > 0 || Interlocked.Exchange(ref _unconsumedTaskCount, 0) == 0)
                {
                    return;
                }

                // reborn this thread
                Interlocked.Increment(ref _workingThreadCount);
            }
        }
    }
}
