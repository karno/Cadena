using System;
using Cadena.Data;
using JetBrains.Annotations;

namespace Cadena.Engine.CyclicReceivers.Timelines
{
    public abstract class CyclicTimelineReceiverBase : CyclicReceiverBase
    {
        private readonly Action<TwitterStatus> _handler;
        protected long? LastSinceId { get; private set; }

        protected CyclicTimelineReceiverBase([NotNull] Action<TwitterStatus> handler,
            [CanBeNull] Action<Exception> exceptionHandler) : base(exceptionHandler)
        {
            _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        protected void CallHandler(TwitterStatus status)
        {
            if (status.Id > LastSinceId.GetValueOrDefault())
            {
                LastSinceId = status.Id;
            }
            _handler(status);
        }
    }
}