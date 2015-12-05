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

        private readonly Queue<IBatchedRequest> _requestQueue;

        public SequentialRequestBatch()
        {
            _requestQueue = new Queue<IBatchedRequest>();
        }

        /// <summary>
        /// Enqueue new task.
        /// If this batch was completed its execution already, will throws exception.
        /// </summary>
        /// <param name="request">queued request</param>
        /// <param name="token">cancellation token</param>
        /// <exception cref="InvalidOperationException">execution of this batch was already completed.</exception>
        /// <returns>Task that will complete after request will be executed.</returns>
        [NotNull]
        public Task Enqueue(IRequest request, CancellationToken token = new CancellationToken())
        {
            var task = TryEnqueue(request, token);
            if (task == null)
            {
                throw new InvalidOperationException("The execution of this batch had been completed already.");
            }
            return task;
        }

        /// <summary>
        /// Enqueue new task.
        /// </summary>
        /// <param name="request">queued request</param>
        /// <param name="token">cancellation token</param>
        /// <returns>Task that will complete after request will be executed.</returns>
        [CanBeNull]
        public Task TryEnqueue(IRequest request, CancellationToken token = new CancellationToken())
        {
            BatchedRequest bt;
            lock (_requestQueue)
            {
                if (_executionCompleted)
                {
                    return null;
                }
                _requestQueue.Enqueue(bt = new BatchedRequest(request, token));
            }
            return bt.ResultTask;
        }

        /// <summary>
        /// Enqueue new task.
        /// If this batch was completed its execution already, will throws exception.
        /// </summary>
        /// <param name="request">queued request</param>
        /// <param name="timeout">timeout which started from starting process of this request</param>
        /// <exception cref="InvalidOperationException">execution of this batch was already completed.</exception>
        /// <returns>Task that will complete after request will be executed.</returns>
        [NotNull]
        public Task Enqueue(IRequest request, TimeSpan timeout)
        {
            var task = TryEnqueue(request, timeout);
            if (task == null)
            {
                throw new InvalidOperationException("The execution of this batch had been completed already.");
            }
            return task;
        }

        /// <summary>
        /// Enqueue new task.
        /// </summary>
        /// <param name="request">queued request</param>
        /// <param name="timeout">timeout which started from starting process of this request</param>
        /// <returns>Task that will complete after request will be executed.</returns>
        [CanBeNull]
        public Task TryEnqueue(IRequest request, TimeSpan timeout)
        {
            BatchedRequest bt;
            lock (_requestQueue)
            {
                if (_executionCompleted)
                {
                    return null;
                }
                _requestQueue.Enqueue(bt = new BatchedRequest(request, timeout));
            }
            return bt.ResultTask;
        }


        /// <summary>
        /// Enqueue new *typed* task.
        /// If this batch was completed its execution already, will throws exception.
        /// </summary>
        /// <param name="request">queued request</param>
        /// <param name="token">cancellation token</param>
        /// <exception cref="InvalidOperationException">execution of this batch was already completed.</exception>
        /// <returns>Task that will complete after request will be executed.</returns>
        [NotNull]
        public Task<T> Enqueue<T>(IRequest<T> request, CancellationToken token = new CancellationToken())
        {
            var task = TryEnqueue(request, token);
            if (task == null)
            {
                throw new InvalidOperationException("The execution of this batch had been completed already.");
            }
            return task;
        }

        /// <summary>
        /// Enqueue new *typed* task.
        /// </summary>
        /// <param name="request">queued request</param>
        /// <param name="token">cancellation token</param>
        /// <returns>Task that will complete after request will be executed.</returns>
        [CanBeNull]
        public Task<T> TryEnqueue<T>(IRequest<T> request, CancellationToken token = new CancellationToken())
        {
            BatchedRequest<T> bt;
            lock (_requestQueue)
            {
                if (_executionCompleted)
                {
                    return null;
                }
                _requestQueue.Enqueue(bt = new BatchedRequest<T>(request, token));
            }
            return bt.ResultTask;
        }

        /// <summary>
        /// Enqueue new *typed* task.
        /// If this batch was completed its execution already, will throws exception.
        /// </summary>
        /// <param name="request">queued request</param>
        /// <param name="timeout">timeout which started from starting process of this request</param>
        /// <exception cref="InvalidOperationException">execution of this batch was already completed.</exception>
        /// <returns>Task that will complete after request will be executed.</returns>
        [NotNull]
        public Task<T> Enqueue<T>(IRequest<T> request, TimeSpan timeout)
        {
            var task = TryEnqueue(request, timeout);
            if (task == null)
            {
                throw new InvalidOperationException("The execution of this batch had been completed already.");
            }
            return task;
        }

        /// <summary>
        /// Enqueue new *typed* task.
        /// </summary>
        /// <param name="request">queued request</param>
        /// <param name="timeout">timeout which started from starting process of this request</param>
        /// <returns>Task that will complete after request will be executed.</returns>
        [CanBeNull]
        public Task<T> TryEnqueue<T>(IRequest<T> request, TimeSpan timeout)
        {
            BatchedRequest<T> bt;
            lock (_requestQueue)
            {
                if (_executionCompleted)
                {
                    return null;
                }
                _requestQueue.Enqueue(bt = new BatchedRequest<T>(request, timeout));
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

        /// <summary>
        /// Execute all tasks.
        /// </summary>
        /// <param name="token">token for cancelling exceution.</param>
        /// <returns></returns>
        public async Task Send(CancellationToken token)
        {
            while (true)
            {
                token.ThrowIfCancellationRequested();
                IBatchedRequest request;
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

        /// <summary>
        /// Batched request.
        /// </summary>
        /// <remarks>
        /// This interface is equal to IRequest, but we should not merge them
        /// so not as to raise confusing.
        /// (We can distinguish pure request task and batched request task easily 
        ///  if the type tree is completely separated.)
        /// </remarks>
        private interface IBatchedRequest
        {
            [NotNull]
            Task Send(CancellationToken token);
        }

        /// <summary>
        /// The batched request.
        /// </summary>
        private sealed class BatchedRequest : IBatchedRequest
        {
            private readonly IRequest _request;
            private readonly TimeSpan _timeout;
            private readonly CancellationToken _token;
            private readonly TaskCompletionSource<object> _completionSource;

            public BatchedRequest(IRequest request, CancellationToken token)
                : this(request, token, Timeout.InfiniteTimeSpan)
            {
            }

            public BatchedRequest(IRequest request, TimeSpan timeout)
                : this(request, CancellationToken.None, timeout)
            {
            }

            private BatchedRequest(IRequest request, CancellationToken token, TimeSpan timeout)
            {
                _request = request;
                _timeout = timeout;
                _token = token;
                _completionSource = new TaskCompletionSource<object>();
            }

            [NotNull]
            public Task ResultTask => _completionSource.Task;

            public async Task Send(CancellationToken token)
            {
                var localTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token, _token);
                localTokenSource.CancelAfter(_timeout);
                var localToken = localTokenSource.Token;
                try
                {
                    await _request.Send(localToken);
                    _completionSource.SetResult(null);
                }
                catch (Exception ex)
                {
                    _completionSource.SetException(ex);
                }
            }
        }

        /// <summary>
        /// The batched request which supports typed result.
        /// </summary>
        /// <typeparam name="T">result type</typeparam>
        private sealed class BatchedRequest<T> : IBatchedRequest
        {
            private readonly IRequest<T> _request;
            private readonly TimeSpan _timeout;
            private readonly CancellationToken _token;
            private readonly TaskCompletionSource<T> _completionSource;

            public BatchedRequest(IRequest<T> request, CancellationToken token)
                : this(request, token, Timeout.InfiniteTimeSpan)
            {
            }

            public BatchedRequest(IRequest<T> request, TimeSpan timeout)
                : this(request, CancellationToken.None, timeout)
            {
            }

            private BatchedRequest(IRequest<T> request, CancellationToken token, TimeSpan timeout)
            {
                _request = request;
                _timeout = timeout;
                _token = token;
                _completionSource = new TaskCompletionSource<T>();
            }

            [NotNull]
            public Task<T> ResultTask => _completionSource.Task;

            public async Task Send(CancellationToken token)
            {
                var localTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token, _token);
                localTokenSource.CancelAfter(_timeout);
                var localToken = localTokenSource.Token;
                try
                {
                    var result = await _request.Send(localToken);
                    _completionSource.SetResult(result);
                }
                catch (Exception ex)
                {
                    _completionSource.SetException(ex);
                }
            }
        }
    }
}
