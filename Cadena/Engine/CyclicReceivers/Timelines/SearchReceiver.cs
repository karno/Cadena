using System;
using System.Threading;
using System.Threading.Tasks;
using Cadena.Api.Parameters;
using Cadena.Api.Rest;
using Cadena.Data;
using Cadena._Internals;
using JetBrains.Annotations;

namespace Cadena.Engine.CyclicReceivers.Timelines
{
    public class SearchReceiver : CyclicTimelineReceiverBase
    {
        private readonly IApiAccessor _accessor;
        private readonly SearchParameter _parameter;

        public SearchReceiver([NotNull] IApiAccessor accessor, Action<TwitterStatus> handler,
            [CanBeNull] Action<Exception> exceptionHandler, [NotNull] SearchParameter parameter) : base(handler,
            exceptionHandler)
        {
            _accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
            _parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));
        }

        protected override async Task<RateLimitDescription> Execute(CancellationToken token)
        {
            try
            {
                var result = await _accessor.SearchAsync(_parameter, token).ConfigureAwait(false);
                result.CallForEachItems(CallHandler);
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