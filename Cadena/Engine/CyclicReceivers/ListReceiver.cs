using System;
using System.Collections.Generic;
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
        #region Receive Moderation among ListReceivers


        #endregion

        /// <summary>
        /// Access interval (per list)
        /// </summary>
        private const int BaseAccessIntervalSec = 30;

        private readonly IApiAccess _access;
        private readonly Action<TwitterStatus> _handler;
        private readonly Action<Exception> _exceptionHandler;
        private readonly int _receiveCount;
        private readonly bool _includeRetweets;

        private int _accessIndex;

        private readonly List<ListParameter> _targetLists = new List<ListParameter>();

        // normally allows 180 access / 15 min => 5sec intv.
        protected override long MinimumIntervalTicks => TimeSpan.FromSeconds(5).Ticks;

        public ListReceiver([NotNull] IApiAccess access, [NotNull] Action<TwitterStatus> handler,
            [NotNull] Action<Exception> exceptionHandler, int receiveCount = 100, bool includeRetweets = false)
        {
            if (access == null) throw new ArgumentNullException(nameof(access));
            if (handler == null) throw new ArgumentNullException(nameof(handler));
            if (exceptionHandler == null) throw new ArgumentNullException(nameof(exceptionHandler));
            _access = access;
            _handler = handler;
            _exceptionHandler = exceptionHandler;
            _receiveCount = receiveCount;
            _includeRetweets = includeRetweets;
        }

        public bool AddList(ListParameter parameter)
        {
            lock (_targetLists)
            {
                var index = _targetLists.IndexOf(parameter);
                if (index >= 0) return false;
                _targetLists.Add(parameter);
                return true;
            }
        }

        public bool RemoveList(ListParameter parameter)
        {
            lock (_targetLists)
            {
                var index = _targetLists.IndexOf(parameter);
                if (index < 0) return false;
                if (index < _accessIndex)
                {
                    _accessIndex--;
                }
                _targetLists.RemoveAt(index);
                return true;
            }
        }

        protected override async Task<RateLimitDescription> Execute(CancellationToken token)
        {
            ListParameter target;
            lock (_targetLists)
            {
                if (_targetLists.Count == 0)
                {
                    return RateLimitDescription.Empty;
                }
                target = _targetLists[_accessIndex++ % _targetLists.Count];
                _accessIndex %= _targetLists.Count;
            }
            try
            {
                var result = await _access.GetListTimelineAsync(target,
                    null, null, _receiveCount, _includeRetweets, token).ConfigureAwait(false);
                result.Result?.ForEach(i => _handler(i));
                return result.RateLimit;
            }
            catch (Exception ex)
            {
                _exceptionHandler(ex);
                throw;
            }
        }

        protected override long CalculateIntervalTicks(TimeSpan remain, RateLimitDescription rld)
        {
            int count;
            lock (_targetLists)
            {
                count = _targetLists.Count;
            }
            if (count == 0)
            {
                // list receiver is empty -> next 5 second
                return TimeSpan.FromSeconds(5).Ticks;
            }
            // based on rate-limit description
            var rldintv = (long)(remain.Ticks / (rld.Remain * ApiConsumptionLimitRatio));
            // based on list count
            var lcintv = TimeSpan.FromSeconds(BaseAccessIntervalSec).Ticks / count;
            // choose larger interval
            return Math.Max(rldintv, lcintv);
        }
    }
}
