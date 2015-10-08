namespace Cadena.Data.Streams.Internals
{
    public sealed class StreamWaitMessage : InternalMessage
    {
        public IApiAccess Access { get; set; }

        public long WaitSec { get; set; }

        public StreamWaitMessage(IApiAccess access, long waitSec)
        {
            Access = access;
            WaitSec = waitSec;
        }
    }
}
