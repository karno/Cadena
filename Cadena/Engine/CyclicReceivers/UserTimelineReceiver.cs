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
    public class UserTimelineReceiver : CyclicReceiverBase
    {
        private readonly IApiAccess _access;
        private readonly Action<TwitterStatus> _handler;
        private readonly Action<Exception> _exceptionHandler;
        private readonly UserParameter _target;
        private readonly int _receiveCount;
        private readonly bool _excludeReplies;
        private readonly bool _includeRetweets;

        private long _lastSinceId = -1;

        public UserTimelineReceiver([NotNull] IApiAccess access, [NotNull] Action<TwitterStatus> handler,
            [NotNull] Action<Exception> exceptionHandler, [NotNull] UserParameter target, int receiveCount = 100,
            bool excludeReplies = false, bool includeRetweets = true)
        {
            if (access == null) throw new ArgumentNullException(nameof(access));
            if (handler == null) throw new ArgumentNullException(nameof(handler));
            if (exceptionHandler == null) throw new ArgumentNullException(nameof(exceptionHandler));
            if (target == null) throw new ArgumentNullException(nameof(target));
            _access = access;
            _handler = handler;
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
                var result = await _access.GetUserTimelineAsync(_target, _receiveCount, _lastSinceId, null,
                    _excludeReplies, _includeRetweets, token).ConfigureAwait(false);
                result.Result?.ForEach(i => _handler(i));
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
