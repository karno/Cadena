namespace Cadena.Data.Streams.Internals
{
    public sealed class StreamWaitMessage : InternalMessage
    {
        public IApiAccessor Accessor { get; }

        public long WaitSec { get; }

        public StreamWaitMessage(IApiAccessor accessor, long waitSec)
        {
            Accessor = accessor;
            WaitSec = waitSec;
        }
    }
}