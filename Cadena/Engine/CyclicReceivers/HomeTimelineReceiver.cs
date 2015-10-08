using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cadena.Api.Rest;
using Cadena.Data;

namespace Cadena.Engine.CyclicReceivers
{
    public sealed class HomeTimelineReceiver : CyclicReceiverBase
    {
        private readonly IApiAccess _access;
        private readonly Action<TwitterStatus> _handler;
        private readonly int _receiveCount;
        public override RequestPriority Priority => RequestPriority.Middle;

        private long _lastSinceId = -1;

        public HomeTimelineReceiver(IApiAccess access, Action<TwitterStatus> handler, int receiveCount = 100)
        {
            _access = access;
            _handler = handler;
            _receiveCount = receiveCount;
        }

        protected async override Task<RateLimitDescription> Execute(CancellationToken token)
        {
            var result = await _access.GetHomeTimelineAsync(_receiveCount,
                _lastSinceId, null, token).ConfigureAwait(false);
            result.Result?.ForEach(i => _handler(i));
            return result.RateLimit;
        }

    }
}
