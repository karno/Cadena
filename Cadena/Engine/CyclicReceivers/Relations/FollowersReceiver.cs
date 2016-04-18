using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cadena.Data;
using Cadena.Twitter.Parameters;
using Cadena.Twitter.Rest;
using JetBrains.Annotations;

namespace Cadena.Engine.CyclicReceivers.Relations
{
    public class FollowersReceiver : CyclicRelationInfoReceiverBase
    {
        private readonly ApiAccessor _accessor;

        public FollowersReceiver([NotNull] ApiAccessor accessor, [NotNull] Action<IEnumerable<long>> handler,
            [CanBeNull] Action<Exception> exceptionHandler) : base(handler, exceptionHandler)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            _accessor = accessor;
        }

        protected override async Task<RateLimitDescription> Execute(CancellationToken token)
        {
            var param = new UserParameter(_accessor.Credential.Id);
            var result = await RetrieveCursoredResult(_accessor,
                (a, i) => a.GetFollowersIdsAsync(param, i, null, token), CallExceptionHandler, token)
                .ConfigureAwait(false);
            CallHandler(result.Result);
            return result.RateLimit;
        }
    }
}
