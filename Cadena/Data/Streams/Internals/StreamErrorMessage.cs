using System.Net;
using Cadena.Api;

namespace Cadena.Data.Streams.Internals
{
    public sealed class StreamErrorMessage : InternalMessage
    {
        public IApiAccess Access { get; set; }

        public HttpStatusCode Code { get; }

        public TwitterErrorCode? TwitterErrorCode { get; set; }

        public StreamErrorMessage(IApiAccess access, HttpStatusCode hcode, TwitterErrorCode? tcode)
        {
            Access = access;
            Code = hcode;
            TwitterErrorCode = tcode;
        }
    }
}
