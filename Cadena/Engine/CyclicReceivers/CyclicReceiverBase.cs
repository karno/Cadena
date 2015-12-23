using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Cadena.Api;
using Cadena.Data;
using JetBrains.Annotations;

namespace Cadena.Engine.CyclicReceivers
{
    /// <summary>
    /// Base class for request cyclic information.
    /// </summary>
    public abstract class CyclicReceiverBase : IReceiver
    {
        private readonly Action<Exception> _exceptionHandler;

        protected CyclicReceiverBase([CanBeNull] Action<Exception> exceptionHandler)
        {
            _exceptionHandler = exceptionHandler;
        }

        protected virtual long MinimumIntervalTicks => TimeSpan.FromSeconds(30).Ticks;

        protected virtual double ApiConsumptionLimitRatio => 0.8;

        protected virtual TimeSpan LinearBackoffInitialWait => TimeSpan.FromMilliseconds(250);

        protected virtual TimeSpan LinearBackoffMaxWait => TimeSpan.FromMilliseconds(16000);

        protected virtual TimeSpan ExponentialBackoffInitialWait => TimeSpan.FromMilliseconds(5000);

        protected virtual TimeSpan ExponentialBackoffMaxWait => TimeSpan.FromMilliseconds(320000);

        private bool? _isExponentialBackoff = null;

        private long _currentBackoffWaitTick = 0;

        /// <summary>
        /// Execute request
        /// </summary>
        /// <param name="token">cancellation token</param>
        /// <returns>task object for awaiting completion</returns>
        protected abstract Task<RateLimitDescription> Execute(CancellationToken token);

        /// <summary>
        /// Get priority of this request
        /// </summary>
        public virtual RequestPriority Priority => RequestPriority.Middle;

        protected void CallExceptionHandler(Exception exception)
        {
            System.Diagnostics.Debug.WriteLine(exception);
            _exceptionHandler?.Invoke(exception);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task<TimeSpan> ExecuteAsync(CancellationToken token)
        {
            try
            {
                var rld = await Execute(token).ConfigureAwait(false);

                // initialize backoff rates
                _isExponentialBackoff = null;
                _currentBackoffWaitTick = 0;

                // calculate rates
                var remainTime = rld.Reset - DateTime.Now;
                if (remainTime < TimeSpan.Zero)
                {
                    // reset time was already arrived.
                    return TimeSpan.FromTicks(MinimumIntervalTicks);
                }

                // target interval(ticks per access)
                var targIntv = CalculateIntervalTicks(remainTime, rld);
                return TimeSpan.FromTicks(Math.Max(targIntv, MinimumIntervalTicks));
            }
            catch (TwitterApiException ex) when (ex.ProblemType == ProblemType.RateLimitation)
            {
                // rate limited. this is not an error but we can't acquire any information...
                // so, we just wait a minute.
                return TimeSpan.FromMinutes(1);
            }
            catch (TwitterApiException ex)
            {
                CallExceptionHandler(new ReceiverOperationException(ex.ProblemType,
                    $"{ex.Description} / {ex.Message}", ex));
                SetExponentialBackoff();
            }
            catch (HttpRequestException ex)
            {
                CallExceptionHandler(new ReceiverOperationException(ProblemType.TwitterInfrastructureError,
                    "(Exception handled by ExecuteAsync) Twitter infrastructure problem?", ex));
                SetExponentialBackoff();
            }
            catch (WebException ex)
            {
                CallExceptionHandler(new ReceiverOperationException(ProblemType.NetworkError,
                    "(Exception handled by ExecuteAsync) network problem?", ex));
                SetLinearBackoff();
            }
            catch (TaskCanceledException ex)
            {
                // timeout? 
                CallExceptionHandler(new ReceiverOperationException(ProblemType.NetworkError,
                    "(Exception handled by ExecuteAsync) timeout? (network problem?)", ex));
                SetLinearBackoff();
            }
            catch (Exception ex)
            {
                // unknown...
                CallExceptionHandler(new ReceiverOperationException(ProblemType.Unknown,
                    "(Exception handled by ExecuteAsync) unknown...", ex));
                SetLinearBackoff();
            }
            return TimeSpan.FromTicks(_currentBackoffWaitTick);
        }

        private void SetLinearBackoff()
        {
            if (_isExponentialBackoff == false)
            {
                _currentBackoffWaitTick += LinearBackoffInitialWait.Ticks;
                if (_currentBackoffWaitTick > LinearBackoffMaxWait.Ticks)
                {
                    _currentBackoffWaitTick = LinearBackoffMaxWait.Ticks;
                }
            }
            else
            {
                _isExponentialBackoff = false;
                _currentBackoffWaitTick = LinearBackoffInitialWait.Ticks;
            }
        }

        private void SetExponentialBackoff()
        {
            if (_isExponentialBackoff == true)
            {
                _currentBackoffWaitTick *= 2;
                if (_currentBackoffWaitTick > ExponentialBackoffMaxWait.Ticks)
                {
                    _currentBackoffWaitTick = ExponentialBackoffMaxWait.Ticks;
                }
            }
            else
            {
                _isExponentialBackoff = true;
                _currentBackoffWaitTick = ExponentialBackoffInitialWait.Ticks;
            }
        }

        protected virtual long CalculateIntervalTicks(TimeSpan remain, RateLimitDescription rld)
        {
            return (long)(remain.Ticks / (rld.Remain * ApiConsumptionLimitRatio));
        }

        ~CyclicReceiverBase()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
        }
    }
}
