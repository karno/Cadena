using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cadena.Engine.StreamReceivers
{
    /// <summary>
    /// Base class for receiving streams.
    /// </summary>
    public abstract class StreamReceiverBase : IReceiver, IDisposable
    {
        private bool _disposed;

        private CancellationTokenSource _cancellationTokenSource;

        protected StreamReceiverBase()
        {
            // initialize as null
            _cancellationTokenSource = null;
        }

        /// <summary>
        /// Begin receiving streams
        /// </summary>
        /// <param name="token">cancellation token</param>
        /// <returns>DateTime.MaxValue</returns>
        public async Task<TimeSpan> ExecuteAsync(CancellationToken token)
        {
            // create new token and swap for old one.
            var cts = CancellationTokenSource.CreateLinkedTokenSource(token);
            var oldcts = Interlocked.Exchange(ref _cancellationTokenSource, cts);

            // cancel previous connection
            try
            {
                oldcts?.Cancel();
            }
            catch
            {
                // ignored
            }
            finally
            {
                oldcts?.Dispose();
            }

            // call ExecuteInternalAsync asynchronously with created token
            try
            {
                await ExecuteInternalAsync(cts.Token).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                // ignored
            }

            // do not call this method periodically.
            return TimeSpan.MaxValue;
        }

        protected abstract Task ExecuteInternalAsync(CancellationToken cancellationToken);

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

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            try
            {
                if (disposing)
                {
                    _cancellationTokenSource?.Cancel();
                }
            }
            finally
            {
                _cancellationTokenSource?.Dispose();
            }
            _disposed = true;
        }
    }
}
