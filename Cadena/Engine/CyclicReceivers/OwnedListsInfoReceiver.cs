using System;
using System.Threading;
using System.Threading.Tasks;
using Cadena._Internals;
using Cadena.Api.Parameters;
using Cadena.Api.Rest;
using Cadena.Data;
using JetBrains.Annotations;

namespace Cadena.Engine.CyclicReceivers
{
    public class OwnedListsInfoReceiver : CyclicReceiverBase
    {
        private readonly IApiAccessor _accessor;
        private readonly Action<TwitterList> _handler;

        public OwnedListsInfoReceiver([NotNull] IApiAccessor accessor, [NotNull] Action<TwitterList> handler,
            [CanBeNull] Action<Exception> exceptionHandler) : base(exceptionHandler)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            if (handler == null) throw new ArgumentNullException(nameof(handler));
            _accessor = accessor;
            _handler = handler;
        }

        protected override async Task<RateLimitDescription> Execute(CancellationToken token)
        {
            try
            {
                var result = await _accessor.GetListsAsync(new UserParameter(_accessor.Id), token)
                                            .ConfigureAwait(false);
                result.CallForEachItems(_handler);
                return result.RateLimit;
            }
            catch (Exception ex)
            {
                CallExceptionHandler(ex);
                throw;
            }
        }
    }
}
