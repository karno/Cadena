using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cadena.Api.Rest;
using Cadena.Data;
using JetBrains.Annotations;

namespace Cadena.Engine.CyclicReceivers.Relations
{
    public class MutesReceiver : CyclicRelationInfoReceiverBase
    {
        private readonly IApiAccessor _accessor;

        public MutesReceiver([NotNull] IApiAccessor accessor, [NotNull] Action<IEnumerable<long>> handler,
            [CanBeNull] Action<Exception> exceptionHandler) : base(handler, exceptionHandler)
        {
            _accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
        }

        protected override async Task<RateLimitDescription> Execute(CancellationToken token)
        {
            var result = await RetrieveCursoredResult(_accessor,
                    (a, i) => a.GetMuteIdsAsync(i, token), CallExceptionHandler, token)
                .ConfigureAwait(false);
            CallHandler(result.Result);
            return result.RateLimit;
        }
    }
}