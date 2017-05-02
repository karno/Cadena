using System;
using System.Threading;
using System.Threading.Tasks;
using Cadena.Api.Rest;
using Cadena.Data;
using Cadena._Internals;
using JetBrains.Annotations;

namespace Cadena.Engine.CyclicReceivers.Timelines
{
    public class DirectMessagesReceiver : CyclicTimelineReceiverBase
    {
        private readonly IApiAccessor _accessor;
        private readonly int _receiveCount;

        public DirectMessagesReceiver([NotNull] IApiAccessor accessor, [NotNull] Action<TwitterStatus> handler,
            [CanBeNull] Action<Exception> exceptionHandler, int receiveCount = 100) : base(handler, exceptionHandler)
        {
            _accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
            _receiveCount = receiveCount;
        }

        protected override async Task<RateLimitDescription> Execute(CancellationToken token)
        {
            try
            {
                var result = await _accessor.GetDirectMessagesAsync(_receiveCount,
                    LastSinceId, null, token).ConfigureAwait(false);
                result.CallForEachItems(CallHandler);
                return result.RateLimit;
            }
            catch (Exception ex)
            {
                CallExceptionHandler(ex);
                throw;
            }
        }
    }
}