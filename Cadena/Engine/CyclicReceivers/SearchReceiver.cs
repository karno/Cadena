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
    public class SearchReceiver : CyclicTimelineReceiverBase
    {
        private readonly IApiAccess _access;
        private readonly Action<Exception> _exceptionHandler;
        private readonly SearchParameter _parameter;

        public SearchReceiver([NotNull] IApiAccess access, Action<TwitterStatus> handler,
            [NotNull] Action<Exception> exceptionHandler, [NotNull] SearchParameter parameter) : base(handler)
        {
            if (access == null) throw new ArgumentNullException(nameof(access));
            if (exceptionHandler == null) throw new ArgumentNullException(nameof(exceptionHandler));
            if (parameter == null) throw new ArgumentNullException(nameof(parameter));
            _access = access;
            _exceptionHandler = exceptionHandler;
            _parameter = parameter;
        }

        protected override async Task<RateLimitDescription> Execute(CancellationToken token)
        {
            try
            {
                var result = await _access.SearchAsync(_parameter, token).ConfigureAwait(false);
                result.Result?.ForEach(CallHandler);
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
