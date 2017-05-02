namespace Cadena.Data.Streams.Warnings
{
    /// <summary>
    /// Stall warnings
    /// </summary>
    /// <remarks>
    /// This message indicates: delivering queue fill rate.
    /// if queue up to full, stream is automatically disconnected.
    ///
    /// This element is supported by: (generic) streams, user streams, site streams.
    /// </remarks>
    public sealed class StreamStallWarning : StreamWarning<int>
    {
        public StreamStallWarning(string code, string message, int percentFull, string timestampMs)
            : base(code, message, percentFull, timestampMs)
        { }
    }
}