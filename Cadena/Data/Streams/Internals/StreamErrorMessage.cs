using System.Net;
using Cadena.Api;

namespace Cadena.Data.Streams.Internals
{
    public sealed class StreamErrorMessage : InternalMessage
    {
        public IApiAccessor Accessor { get; set; }

        public HttpStatusCode Code { get; }

        public TwitterErrorCode? TwitterErrorCode { get; set; }

        public StreamErrorMessage(IApiAccessor accessor, HttpStatusCode hcode, TwitterErrorCode? tcode)
        {
            Accessor = accessor;
            Code = hcode;
            TwitterErrorCode = tcode;
        }
    }
}
