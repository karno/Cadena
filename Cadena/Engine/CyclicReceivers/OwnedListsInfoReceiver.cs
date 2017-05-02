using System;
using System.Threading;
using System.Threading.Tasks;
using Cadena.Api.Parameters;
using Cadena.Api.Rest;
using Cadena.Data;
using Cadena._Internals;
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
            _accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
            _handler = handler ?? throw new ArgumentNullException(nameof(handler));
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