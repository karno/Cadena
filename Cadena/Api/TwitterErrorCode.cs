namespace Cadena.Api
{
    public enum TwitterErrorCode
    {
        AuthenticationFailed = 32,
        PageNotExist = 34,
        AccountSuspended = 64,
        ApiNoLongerSupported = 68,
        RateLimitExceeded = 88,
        InvalidOrExpiredToken = 89,
        SslRequired = 92,
        OverCapacity = 130,
        InternalError = 131,
        InvalidSignature = 135,
        TooManyFollow = 161,
        AuthorizationRequired = 179,
        StatusUpdateLimit = 185,
        StatusDuplicated = 187,
        BadAuthenticationData = 215,
        SuspiciousRequest = 226,
        LoginVerificationNeeded = 231,
        EndpointGone = 251,
        ApiPermissionDenined = 261,
        TryToMuteYourself = 271,
        CouldNotMute = 272,
        DirectMessageTooLong = 354,
    }
}