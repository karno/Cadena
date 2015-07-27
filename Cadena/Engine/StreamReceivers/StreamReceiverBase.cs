using System;

namespace Cadena.Engine.StreamReceivers
{
    /// <summary>
    /// Base class for receiving streams.
    /// </summary>
    public abstract class StreamReceiverBase : IDisposable
    {
        private readonly ReceiveManager _manager;

        protected StreamReceiverBase(ReceiveManager manager)
        {
            _manager = manager;
        }

        /// <summary>
        /// Kill this connection and release all resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~StreamReceiverBase()
        {
            Dispose(false);
        }

        protected abstract void Dispose(bool disposing);
    }
}
