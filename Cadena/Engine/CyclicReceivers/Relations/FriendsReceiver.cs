using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cadena.Api.Parameters;
using Cadena.Api.Rest;
using Cadena.Data;
using JetBrains.Annotations;

namespace Cadena.Engine.CyclicReceivers.Relations
{
    public class FriendsReceiver : CyclicRelationInfoReceiverBase
    {
        private readonly IApiAccessor _accessor;

        public FriendsReceiver([NotNull] IApiAccessor accessor, [NotNull] Action<IEnumerable<long>> handler,
            [CanBeNull] Action<Exception> exceptionHandler)
            : base(handler, exceptionHandler)
        {
            _accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
        }

        protected override async Task<RateLimitDescription> Execute(CancellationToken token)
        {
            var param = new UserParameter(_accessor.Id);
            var result = await RetrieveCursoredResult(_accessor,
                    (a, i) => a.GetFriendsIdsAsync(param, i, null, token), CallExceptionHandler, token)
                .ConfigureAwait(false);
            CallHandler(result.Result);
            return result.RateLimit;
        }
    }
}