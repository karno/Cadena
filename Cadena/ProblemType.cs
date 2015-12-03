namespace Cadena
{
    public enum ProblemType
    {
        /// <summary>
        /// Unknown error
        /// </summary>
        Unknown = -1,
        /// <summary>
        /// Network connection was not completed correctly.
        /// </summary>
        NetworkError,
        /// <summary>
        /// Network connection is successfully completed, 
        /// but Twitter does not accept our request.
        /// </summary>
        TwitterInfrastructureError,
        /// <summary>
        /// Network connection is successfully completed,
        /// Twitter recognizes our request, 
        /// but deny our request due to invalid signature/authentication/authorization information.
        /// </summary>
        AuthorizationError,
        /// <summary>
        /// Network connection is successfully completed,
        /// Twitter recognizes our request, 
        /// but deny our request due to invalid request/endpoint/format.
        /// </summary>
        ProtocolViolation,
        /// <summary>
        /// Network connection is successfully completed,
        /// Twitter recognizes our request, 
        /// but deny our request due to invalid request object(user payload) type/format/etc.
        /// </summary>
        InvalidPayload,
        /// <summary>
        /// Network connection is successfully completed,
        /// Twitter recognizes our request, 
        /// but deny our request due to hit to some limitations.
        /// </summary>
        RateLimitation,
        /// <summary>
        /// Network connection is successfully completed,
        /// Twitter accepts our request and returned data,
        /// but we can't recognize it, or contains errors on it.
        /// </summary>
        InvalidReturnedData,
    }
}