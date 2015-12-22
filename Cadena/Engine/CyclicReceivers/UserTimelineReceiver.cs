using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cadena.Api.Parameters;
using Cadena.Api.Rest;
using Cadena.Data;
using JetBrains.Annotations;

namespace Cadena.Engine.CyclicReceivers
{
    public class UserTimelineReceiver : CyclicTimelineReceiverBase
    {
        private readonly ApiAccessor _accessor;
        private readonly Action<Exception> _exceptionHandler;
        private readonly UserParameter _target;
        private readonly int _receiveCount;
        private readonly bool _excludeReplies;
        private readonly bool _includeRetweets;

        public UserTimelineReceiver([NotNull] ApiAccessor accessor, [NotNull] Action<TwitterStatus> handler,
            [NotNull] Action<Exception> exceptionHandler, [NotNull] UserParameter target, int receiveCount = 100,
            bool excludeReplies = false, bool includeRetweets = true) : base(handler)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            if (exceptionHandler == null) throw new ArgumentNullException(nameof(exceptionHandler));
            if (target == null) throw new ArgumentNullException(nameof(target));
            _accessor = accessor;
            _exceptionHandler = exceptionHandler;
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
                result.Result?.ForEach(CallHandler);
                return result.RateLimit;
            }
            catch (Exception ex)
            {
                _exceptionHandler(ex);
                throw;
            }
        }
    }
}
