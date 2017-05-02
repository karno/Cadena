using System.Net;
using Cadena.Api;

namespace Cadena.Data.Streams.Internals
{
    public sealed class StreamErrorMessage : InternalMessage
    {
        public IApiAccessor Accessor { get; }

        public HttpStatusCode Code { get; }

        public TwitterErrorCode? TwitterErrorCode { get; }

        public StreamErrorMessage(IApiAccessor accessor, HttpStatusCode hcode, TwitterErrorCode? tcode)
        {
            Accessor = accessor;
            Code = hcode;
            TwitterErrorCode = tcode;
        }
    }
}