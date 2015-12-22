namespace Cadena.Data.Streams.Internals
{
    public sealed class StreamWaitMessage : InternalMessage
    {
        public ApiAccessor Accessor { get; set; }

        public long WaitSec { get; set; }

        public StreamWaitMessage(ApiAccessor accessor, long waitSec)
        {
            Accessor = accessor;
            WaitSec = waitSec;
        }
    }
}
