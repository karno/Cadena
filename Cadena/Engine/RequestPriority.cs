namespace Cadena.Engine
{
    public enum RequestPriority
    {
        /// <summary>
        /// Requested from user operation(blocking) or other urgent source
        /// </summary>
        High = 0,

        /// <summary>
        /// Requested from user operation(non-blocking) or other middle priority source
        /// </summary>
        Middle = 1,

        /// <summary>
        /// Requested from cyclic receiving or other postponable source
        /// </summary>
        Low = 2
    }
}