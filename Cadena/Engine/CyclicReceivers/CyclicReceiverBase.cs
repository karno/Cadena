using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Cadena.Api;
using Cadena.Data;

namespace Cadena.Engine.CyclicReceivers
{
    /// <summary>
    /// Base class for request cyclic information.
    /// </summary>
    public abstract class CyclicReceiverBase : IReceiver
    {
        protected virtual long MinimumIntervalTicks => TimeSpan.FromSeconds(30).Ticks;

        protected virtual double ApiConsumptionLimitRatio => 0.8;

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
            catch (TwitterApiException ex)
            {
                throw new ReceiverOperationException(ex.ProblemType,
                    $"{ex.Description} / {ex.Message}", ex);
            }
            catch (HttpRequestException ex)
            {
                throw new ReceiverOperationException(ProblemType.TwitterInfrastructureError,
                    "(Exception handled by ExecuteAsync) Twitter infrastructure problem?", ex);
            }
            catch (WebException ex)
            {
                throw new ReceiverOperationException(ProblemType.NetworkError,
                    "(Exception handled by ExecuteAsync) network problem?", ex);
            }
            catch (TaskCanceledException ex)
            {
                // timeout? 
                throw new ReceiverOperationException(ProblemType.NetworkError,
                    "(Exception handled by ExecuteAsync) timeout? (network problem?)", ex);
            }
            catch (Exception ex)
            {
                // retry after minimum timespan.
                throw new ReceiverOperationException(ProblemType.Unknown,
                    "(Exception handled by ExecuteAsync) unknown...", ex);
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
