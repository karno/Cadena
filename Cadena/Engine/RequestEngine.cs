using System;
using System.Threading;
using System.Threading.Tasks;
using Cadena.Engine._Internals;
using Cadena.Engine.Requests;

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

        public async Task SendRequest(IRequest request, TimeSpan timeout)
        {
            using (var ts = new CancellationTokenSource(timeout))
            {
                await SendRequest(request, ts.Token);
            }
        }

        public Task SendRequest(IRequest request, CancellationToken token = new CancellationToken())
        {
            return _taskFactory.StartNew(() => request.Send(token), token).Unwrap();
        }
    }
}
