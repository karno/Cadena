using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cadena.Data;
using Cadena.Twitter.Rest;
using JetBrains.Annotations;

namespace Cadena.Engine.CyclicReceivers.Relations
{
    public class NoRetweetsReceiver : CyclicRelationInfoReceiverBase
    {
        private readonly ApiAccessor _accessor;

        public NoRetweetsReceiver([NotNull] ApiAccessor accessor, [NotNull] Action<IEnumerable<long>> handler,
            [CanBeNull] Action<Exception> exceptionHandler) : base(handler, exceptionHandler)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            _accessor = accessor;
        }

        protected override async Task<RateLimitDescription> Execute(CancellationToken token)
        {
            var result = await _accessor.GetNoRetweetsIdsAsync(token).ConfigureAwait(false);
            CallHandler(result.Result);
            return result.RateLimit;
        }
    }
}
