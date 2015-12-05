using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Cadena.Engine.Requests
{
    public sealed class SequentialRequestBatch : IRequest, IEnumerable<IRequest>
    {
        private bool _executionCompleted;

        private readonly Queue<IBatchedTask> _requestQueue;

        public SequentialRequestBatch()
        {
            _requestQueue = new Queue<IRequest>();
        }

        public SequentialRequestBatch(IEnumerable<IRequest> requests)
        {
            _requestQueue = new Queue<IRequest>(requests);
        }

        public SequentialRequestBatch(params IRequest[] requests)
            : this((IEnumerable<IRequest>)requests)
        {
        }

        [NotNull]
        public Task Enqueue(IRequest request)
        {
            var task = TryEnqueue(request);
            if (task == null)
            {
                throw new InvalidOperationException("The execution of this batch had been completed already.");
            }
            return task;
        }

        [CanBeNull]
        public Task TryEnqueue(IRequest request)
        {
            BatchedTask bt;
            lock (_requestQueue)
            {
                if (_executionCompleted)
                {
                    return null;
                }
                _requestQueue.Enqueue(bt = new BatchedTask(request));
            }
            return bt.ResultTask;
        }

        [NotNull]
        public Task<T> Enqueue<T>(IRequest<T> request)
        {
            var task = TryEnqueue(request);
            if (task == null)
            {
                throw new InvalidOperationException("The execution of this batch had been completed already.");
            }
            return task;
        }

        [CanBeNull]
        public Task<T> TryEnqueue<T>(IRequest<T> request)
        {
            BatchedTask<T> bt;
            lock (_requestQueue)
            {
                if (_executionCompleted)
                {
                    return null;
                }
                _requestQueue.Enqueue(bt = new BatchedTask<T>(request));
            }
            return bt.ResultTask;
        }

        public IEnumerator<IRequest> GetEnumerator()
        {
            lock (_requestQueue)
            {
                return (IEnumerator<IRequest>)_requestQueue.ToArray().GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public async Task Send(CancellationToken token)
        {
            while (true)
            {
                IBatchedTask request;
                lock (_requestQueue)
                {
                    if (_requestQueue.Count == 0)
                    {
                        _executionCompleted = true;
                        break;
                    }
                    request = _requestQueue.Dequeue();
                }
                await request.Send(token);
            }
        }

        private interface IBatchedTask
        {
            [NotNull]
            Task Send(CancellationToken token);
        }

        private sealed class BatchedTask : IBatchedTask
        {
            private readonly IRequest _request;
            private readonly TaskCompletionSource<object> _completionSource;

            public BatchedTask(IRequest request)
            {
                _request = request;
                _completionSource = new TaskCompletionSource<object>();
            }

            [NotNull]
            public Task ResultTask => _completionSource.Task;

            public async Task Send(CancellationToken token)
            {
                try
                {
                    await _request.Send(token);
                    _completionSource.SetResult(null);
                }
                catch (TaskCanceledException)
                {
                    _completionSource.SetCanceled();
                }
                catch (Exception ex)
                {
                    _completionSource.SetException(ex);
                }
            }
        }

        private sealed class BatchedTask<T> : IBatchedTask
        {
            private readonly IRequest<T> _request;
            private readonly TaskCompletionSource<T> _completionSource;

            public BatchedTask(IRequest<T> request)
            {
                _request = request;
                _completionSource = new TaskCompletionSource<T>();
            }

            [NotNull]
            public Task<T> ResultTask => _completionSource.Task;

            public async Task Send(CancellationToken token)
            {
                try
                {
                    var result = await _request.Send(token);
                    _completionSource.SetResult(result);
                }
                catch (TaskCanceledException)
                {
                    _completionSource.SetCanceled();
                }
                catch (Exception ex)
                {
                    _completionSource.SetException(ex);
                }
            }
        }
    }
}
