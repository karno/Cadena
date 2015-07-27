using System.Threading.Tasks;

namespace Cadena.Engine
{
    public sealed class ReceiveManager
    {
        private readonly int _maxWorkingThreads;

        private int _workingThreadCount;

        public ReceiveManager(int maxWorkingThreads)
        {
            _maxWorkingThreads = maxWorkingThreads;
            _workingThreadCount = 0;
        }

        public void Request(Task task, RequestPriority priority)
        {

        }
    }

    public enum RequestPriority
    {
        /// <summary>
        /// Requested from user operation(blocking) or other urgent source
        /// </summary>
        High,
        /// <summary>
        /// Requested from user operation(non-blocking) or other middle priority source
        /// </summary>
        Middle,
        /// <summary>
        /// Requested from cyclic receiving or other postponable source
        /// </summary>
        Low
    }
}
