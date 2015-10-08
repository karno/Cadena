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
    public class ListReceiver : CyclicReceiverBase
    {
        private readonly IApiAccess _access;
        private readonly Action<TwitterStatus> _handler;
        private readonly Action<Exception> _exceptionHandler;
        private readonly ListParameter _targetList;
        private readonly int _receiveCount;
        private readonly bool _includeRetweets;

        private long _lastSinceId = -1;

        public ListReceiver([NotNull] IApiAccess access, [NotNull] Action<TwitterStatus> handler,
            [NotNull] Action<Exception> exceptionHandler, [NotNull] ListParameter targetList,
            int receiveCount = 100, bool includeRetweets = false)
        {
            if (access == null) throw new ArgumentNullException(nameof(access));
            if (handler == null) throw new ArgumentNullException(nameof(handler));
            if (exceptionHandler == null) throw new ArgumentNullException(nameof(exceptionHandler));
            if (targetList == null) throw new ArgumentNullException(nameof(targetList));
            _access = access;
            _handler = handler;
            _exceptionHandler = exceptionHandler;
            _targetList = targetList;
            _receiveCount = receiveCount;
            _includeRetweets = includeRetweets;
        }

        protected override async Task<RateLimitDescription> Execute(CancellationToken token)
        {
            try
            {
                var result = await _access.GetListTimelineAsync(_targetList,
                    _lastSinceId, null, _receiveCount, _includeRetweets, token).ConfigureAwait(false);
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
