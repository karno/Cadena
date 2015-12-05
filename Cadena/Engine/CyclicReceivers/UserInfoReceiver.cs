using System;
using System.Threading;
using System.Threading.Tasks;
using Cadena.Api.Parameters;
using Cadena.Api.Rest;
using Cadena.Data;
using JetBrains.Annotations;

namespace Cadena.Engine.CyclicReceivers
{
    public class UserInfoReceiver : CyclicReceiverBase
    {
        private readonly IApiAccess _access;
        private readonly Action<TwitterUser> _handler;
        private readonly Action<Exception> _exceptionHandler;
        private readonly UserParameter _target;

        public UserInfoReceiver(IApiAccess access, [NotNull] Action<TwitterUser> handler,
            [NotNull] Action<Exception> exceptionHandler, [NotNull] UserParameter target)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));
            if (exceptionHandler == null) throw new ArgumentNullException(nameof(exceptionHandler));
            if (target == null) throw new ArgumentNullException(nameof(target));
            _access = access;
            _handler = handler;
            _exceptionHandler = exceptionHandler;
            _target = target;
        }

        protected override async Task<RateLimitDescription> Execute(CancellationToken token)
        {
            try
            {
                var result = await _access.ShowUserAsync(_target, token).ConfigureAwait(false);
                _handler(result.Result);
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
