using System;
using System.Threading;
using System.Threading.Tasks;
using Cadena.Engine._Internals;

namespace Cadena.Engine
{
    public sealed class RequestEngine
    {
        private static TaskFactory _taskFactory;

        public int MaxThreadCount { get; }

        public RequestEngine(int maxThreadCount = Int32.MaxValue)
        {
            MaxThreadCount = maxThreadCount;
            // use TaskFactoryDistrict as simple limited-queue task scheduler.
            _taskFactory = new TaskFactoryDistrict(maxThreadCount).GetOrCreateFactory(0);
        }

        /// <summary>
        /// Send request on this engine.
        /// </summary>
        /// <param name="request">request</param>
        /// <param name="token">cancellation token for cancelling request.</param>
        /// <returns>task that complete after request will completed.</returns>
        public Task SendRequest(IRequest request, CancellationToken token = new CancellationToken())
        {
            return _taskFactory.StartNew(() => request.Send(token), token).Unwrap();
        }

        /// <summary>
        /// Send *typed* request on this engine.
        /// </summary>
        /// <param name="request">request</param>
        /// <param name="token">cancellation token for cancelling request.</param>
        /// <returns>task that complete after request will completed.</returns>
        public Task<T> SendRequest<T>(IRequest<T> request, CancellationToken token = new CancellationToken())
        {
            return _taskFactory.StartNew(() => request.Send(token), token).Unwrap();
        }

        /// <summary>
        /// Send request with timeout.
        /// </summary>
        /// <param name="request">sending request</param>
        /// <param name="timeout">timeout value for this request</param>
        /// <returns>task that complete after request will completed.</returns>
        public Task SendRequest(IRequest request, TimeSpan timeout)
        {
            return _taskFactory.StartNew(async () =>
            {
                using (var ts = new CancellationTokenSource(timeout))
                {
                    await SendRequest(request, ts.Token);
                }
            });
        }

        /// <summary>
        /// Send *typed* request on this engine with timeout.
        /// </summary>
        /// <param name="request">request</param>
        /// <param name="timeout">timeout value for this request</param>
        /// <returns>task that complete after request will completed.</returns>
        public Task<T> SendRequest<T>(IRequest<T> request, TimeSpan timeout)
        {
            return _taskFactory.StartNew(async () =>
            {
                using (var ts = new CancellationTokenSource(timeout))
                {
                    return await SendRequest(request, ts.Token);
                }
            }).Unwrap();
        }
    }
}
