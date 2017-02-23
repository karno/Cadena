using System;
using System.Threading;
using System.Threading.Tasks;
using Cadena._Internals;
using Cadena.Api.Parameters;
using Cadena.Api.Rest;
using Cadena.Data;
using JetBrains.Annotations;

namespace Cadena.Engine.CyclicReceivers.Timelines
{
    public class UserTimelineReceiver : CyclicTimelineReceiverBase
    {
        private readonly IApiAccessor _accessor;
        private readonly UserParameter _target;
        private readonly int _receiveCount;
        private readonly bool _excludeReplies;
        private readonly bool _includeRetweets;

        public UserTimelineReceiver([NotNull] IApiAccessor accessor, [NotNull] Action<TwitterStatus> handler,
            [CanBeNull] Action<Exception> exceptionHandler, [NotNull] UserParameter target, int receiveCount = 100,
            bool excludeReplies = false, bool includeRetweets = true) : base(handler, exceptionHandler)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            if (target == null) throw new ArgumentNullException(nameof(target));
            _accessor = accessor;
            _target = target;
            _receiveCount = receiveCount;
            _excludeReplies = excludeReplies;
            _includeRetweets = includeRetweets;
        }

        protected override async Task<RateLimitDescription> Execute(CancellationToken token)
        {
            try
            {
                var result = await _accessor.GetUserTimelineAsync(_target, _receiveCount, LastSinceId, null,
                    _excludeReplies, _includeRetweets, token).ConfigureAwait(false);
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
