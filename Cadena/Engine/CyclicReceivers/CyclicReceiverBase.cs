using System.Threading.Tasks;
using Cadena.Data;

namespace Cadena.Engine.CyclicReceivers
{
    /// <summary>
    /// Base class for request cyclic information.
    /// </summary>
    public abstract class CyclicReceiverBase
    {
        /// <summary>
        /// Execute request
        /// </summary>
        /// <param name="manager">assigned receive manager</param>
        /// <returns>task object for awaiting completion</returns>
        public abstract Task<RateLimitDescription> Execute(ReceiveManager manager);

        /// <summary>
        /// Get priority of this request
        /// </summary>
        public abstract RequestPriority Priority { get; }
    }
}
