using System;
using Cadena.Data;
using JetBrains.Annotations;

namespace Cadena.Engine.CyclicReceivers
{
    public abstract class CyclicTimelineReceiverBase : CyclicReceiverBase
    {
        private readonly Action<TwitterStatus> _handler;
        protected long? LastSinceId { get; private set; }

        protected CyclicTimelineReceiverBase([NotNull] Action<TwitterStatus> handler)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));
            _handler = handler;
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
