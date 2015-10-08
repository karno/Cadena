using System;

namespace Cadena.Data.Streams.Internals
{
    public abstract class InternalMessage : StreamMessage
    {
        protected InternalMessage() : base(DateTime.Now)
        {
        }
    }
}
