using System.Net;
using Cadena.Twitter;

namespace Cadena.Data.Streams.Internals
{
    public sealed class StreamErrorMessage : InternalMessage
    {
        public ApiAccessor Accessor { get; set; }

        public HttpStatusCode Code { get; }

        public TwitterErrorCode? TwitterErrorCode { get; set; }

        public StreamErrorMessage(ApiAccessor accessor, HttpStatusCode hcode, TwitterErrorCode? tcode)
        {
            Accessor = accessor;
            Code = hcode;
            TwitterErrorCode = tcode;
        }
    }
}
