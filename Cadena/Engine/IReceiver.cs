using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cadena.Engine
{
    public interface IReceiver
    {
        /// <summary>
        /// Execute process and get timestamp for next invocation
        /// </summary>
        /// <param name="token">cancellation token</param>
        /// <returns>call interval(if TimeSpan.MaxValue is specified, callback is disabled.)</returns>
        Task<TimeSpan> ExecuteAsync(CancellationToken token);
    }
}

