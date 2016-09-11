using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cadena.Data;
using Cadena.Twitter.Parameters;
using Cadena.Twitter.Rest;
using JetBrains.Annotations;

namespace Cadena.Engine.CyclicReceivers
{
    public class OwnedListsInfoReceiver : CyclicReceiverBase
    {
        private readonly ApiAccessor _accessor;
        private readonly Action<TwitterList> _handler;

        public OwnedListsInfoReceiver([NotNull] ApiAccessor accessor, [NotNull] Action<TwitterList> handler,
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
                var result = await _accessor.GetListsAsync(new UserParameter(_accessor.Credential.Id), token)
                                            .ConfigureAwait(false);
                result.Result?.ForEach(i => _handler(i));
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
