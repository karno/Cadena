using System;

namespace Cadena.Data.Streams
{
    /// <summary>
    /// Friends lists
    /// </summary>
    /// <remarks>
    /// This notification indicates: friends list id.
    /// 
    /// This element is supported by: user streams
    /// </remarks>
    public sealed class StreamEnumeration : StreamMessage
    {
        public StreamEnumeration(long[] enumeration)
            : base(DateTime.Now) // stream enumeration is not have a timestamp
        {
            Enumeration = enumeration;
        }

        /// <summary>
        /// Enumerated Ids
        /// </summary>
        public long[] Enumeration { get; }
    }
}
