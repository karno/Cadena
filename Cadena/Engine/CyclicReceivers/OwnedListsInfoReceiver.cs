using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cadena.Api.Parameters;
using Cadena.Api.Rest;
using Cadena.Data;
using JetBrains.Annotations;

namespace Cadena.Engine.CyclicReceivers
{
    public class OwnedListsInfoReceiver : CyclicReceiverBase
    {
        private readonly IApiAccess _access;
        private readonly Action<TwitterList> _handler;
        private readonly Action<Exception> _exceptionHandler;

        public OwnedListsInfoReceiver([NotNull] IApiAccess access, [NotNull] Action<TwitterList> handler,
            [NotNull] Action<Exception> exceptionHandler)
        {
            if (access == null) throw new ArgumentNullException(nameof(access));
            if (handler == null) throw new ArgumentNullException(nameof(handler));
            if (exceptionHandler == null) throw new ArgumentNullException(nameof(exceptionHandler));
            _access = access;
            _handler = handler;
            _exceptionHandler = exceptionHandler;
        }

        protected async override Task<RateLimitDescription> Execute(CancellationToken token)
        {
            try
            {
                var result = await _access.GetListsAsync(new UserParameter(_access.Credential.Id), token)
                                          .ConfigureAwait(false);
                result.Result?.ForEach(i => _handler(i));
                return result.RateLimit;
            }
            catch (Exception ex)
            {
                _exceptionHandler(ex);
                throw;
            }
        }
    }
}
