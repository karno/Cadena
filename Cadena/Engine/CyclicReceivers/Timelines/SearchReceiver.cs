using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cadena.Api.Parameters;
using Cadena.Api.Rest;
using Cadena.Data;
using JetBrains.Annotations;

namespace Cadena.Engine.CyclicReceivers.Timelines
{
    public class SearchReceiver : CyclicTimelineReceiverBase
    {
        private readonly ApiAccessor _accessor;
        private readonly SearchParameter _parameter;

        public SearchReceiver([NotNull] ApiAccessor accessor, Action<TwitterStatus> handler,
            [CanBeNull] Action<Exception> exceptionHandler, [NotNull] SearchParameter parameter) : base(handler, exceptionHandler)
        {
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            if (parameter == null) throw new ArgumentNullException(nameof(parameter));
            _accessor = accessor;
            _parameter = parameter;
        }

        protected override async Task<RateLimitDescription> Execute(CancellationToken token)
        {
            try
            {
                var result = await _accessor.SearchAsync(_parameter, token).ConfigureAwait(false);
                result.Result?.ForEach(CallHandler);
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
