using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cadena.Api.Rest;
using Cadena.Data;
using JetBrains.Annotations;

namespace Cadena.Engine.CyclicReceivers.Infrastructures
{
    public class SavedSearchesReceiver : CyclicReceiverBase
    {
        private readonly ApiAccessor _accessor;
        private readonly Action<IEnumerable<TwitterSavedSearch>> _handler;

        public SavedSearchesReceiver([NotNull] ApiAccessor accessor,
            [NotNull] Action<IEnumerable<TwitterSavedSearch>> handler, [CanBeNull] Action<Exception> exceptionHandler)
            : base(exceptionHandler)
        {
            _accessor = accessor;
            _handler = handler;
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            if (handler == null) throw new ArgumentNullException(nameof(handler));
        }

        protected override async Task<RateLimitDescription> Execute(CancellationToken token)
        {
            var result = await _accessor.GetSavedSearchesAsync(token).ConfigureAwait(false);
            _handler?.Invoke(result.Result);
            return result.RateLimit;
        }
    }
}
