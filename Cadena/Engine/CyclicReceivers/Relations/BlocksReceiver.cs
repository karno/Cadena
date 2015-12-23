using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cadena.Api.Rest;
using Cadena.Data;
using JetBrains.Annotations;

namespace Cadena.Engine.CyclicReceivers.Relations
{
    public class BlocksReceiver : CyclicRelationInfoReceiverBase
    {
        private readonly ApiAccessor _accessor;

        public BlocksReceiver([NotNull] ApiAccessor accessor, [NotNull] Action<IEnumerable<long>> handler,
            [CanBeNull] Action<Exception> exceptionHandler) : base(handler, exceptionHandler)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            _accessor = accessor;
        }

        protected override async Task<RateLimitDescription> Execute(CancellationToken token)
        {
            var result = await RetrieveCursoredResult(_accessor,
                (a, i) => a.GetBlocksIdsAsync(i, token), CallExceptionHandler, token)
                .ConfigureAwait(false);
            CallHandler(result.Result);
            return result.RateLimit;
        }
    }
}
