using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cadena.Engine
{
    public interface IReceiver : IDisposable
    {
        /// <summary>
        /// Execute process and get timestamp for next invocation
        /// </summary>
        /// <param name="token">cancellation token</param>
        /// <returns>call interval(if zero is specified, disable re-calling)</returns>
        Task<TimeSpan> ExecuteAsync(CancellationToken token);
    }
}

