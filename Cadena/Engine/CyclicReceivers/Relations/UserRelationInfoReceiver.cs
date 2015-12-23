using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cadena.Api.Parameters;
using Cadena.Api.Rest;
using Cadena.Data;
using Cadena.Util;

namespace Cadena.Engine.CyclicReceivers.Relations
{
    public class UserRelationInfoReceiver : CyclicReceiverBase
    {
        private readonly ApiAccessor _accessor;
        private readonly Action<RelationInfoResult> _handler;
        private readonly Action<Exception> _exceptionHandler;

        protected override long MinimumIntervalTicks => TimeSpan.FromHours(6).Ticks;

        public UserRelationInfoReceiver(ApiAccessor accessor, Action<RelationInfoResult> handler,
            Action<Exception> exceptionHandler) : base(exceptionHandler)
        {
            _accessor = accessor;
            _handler = handler;
            _exceptionHandler = exceptionHandler;
        }

        protected override async Task<RateLimitDescription> Execute(CancellationToken token)
        {
            var up = new UserParameter(_accessor.Credential.Id);
            var r = await ReceiveSingle((a, i) => a.GetFriendsIdsAsync(up, i, null, token), token).ConfigureAwait(false);
            // TODO: receive Friends, Followers,Blockings, NoRetweetIds, MuteIds
            return RateLimitDescription.Empty;
        }

        private async Task<IEnumerable<long>> ReceiveSingle(
            Func<ApiAccessor, long, Task<IApiResult<ICursorResult<IEnumerable<long>>>>> func,
            CancellationToken token)
        {
            var resultList = new List<long>();
            CursorResultExtension.ApiContinuationReader<IEnumerable<long>> reader =
                () => _accessor.ReadCursorApi(func, token);
            while (reader != null)
            {
                var result = await reader().ConfigureAwait(false);
                resultList.AddRange(result.Item1.Result);
                reader = result.Item2;
            }
            return resultList;
        }

        protected override long CalculateIntervalTicks(TimeSpan remain, RateLimitDescription rld)
        {
            // TODO: Constant intervals
            return base.CalculateIntervalTicks(remain, rld);
        }
    }

    public class RelationInfoResult
    {
        public IEnumerable<long> Friends { get; }

        public IEnumerable<long> Followers { get; }

        public IEnumerable<long> Blockings { get; }

        public IEnumerable<long> NoRetweets { get; }

        public IEnumerable<long> Muteds { get; }
    }
}
