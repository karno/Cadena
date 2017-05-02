using System;
using System.Threading;
using System.Threading.Tasks;
using Cadena.Api.Rest;
using Cadena.Data;
using JetBrains.Annotations;

namespace Cadena.Engine.CyclicReceivers.Infrastructures
{
    public class HelpReceiver : CyclicReceiverBase
    {
        private readonly IApiAccessor _accessor;
        private readonly Action<TwitterConfiguration> _handler;

        protected override long MinimumIntervalTicks => TimeSpan.FromHours(6).Ticks;

        public HelpReceiver([NotNull] IApiAccessor accessor, [NotNull] Action<TwitterConfiguration> handler,
            [CanBeNull] Action<Exception> exceptionHandler) : base(exceptionHandler)
        {
            _accessor = accessor;
            _handler = handler;
            if (accessor == null) throw new ArgumentNullException(nameof(accessor));
            if (handler == null) throw new ArgumentNullException(nameof(handler));
        }

        protected override async Task<RateLimitDescription> Execute(CancellationToken token)
        {
            var result = await _accessor.GetConfigurationAsync(token).ConfigureAwait(false);
            _handler?.Invoke(result.Result);
            return result.RateLimit;
        }
    }
}