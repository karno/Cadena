using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Codeplex.Data;

namespace Cadena.Api
{
    public class TwitterApiException : Exception
    {
        public TwitterApiException(HttpStatusCode httpCode, string message, TwitterErrorCode twitterCode)
            : base(message)
        {
            StatusCode = httpCode;
            TwitterErrorCode = twitterCode;

            // analyze error
            ProblemType problemType;
            string description;
            AnalyzeError(out problemType, out description);
            ProblemType = problemType;
            Description = description;

        }

        public TwitterApiException(HttpStatusCode statusCode, string message)
            : base(message)
        {
            StatusCode = statusCode;

            // analyze error
            ProblemType problemType;
            string description;
            AnalyzeError(out problemType, out description);
            ProblemType = problemType;
            Description = description;
        }

        private void AnalyzeError(out ProblemType problemType, out string description)
        {
            description = "HTTP/" + (int)StatusCode;
            if (TwitterErrorCode != null)
            {
                description += ", Twitter/" + (int)TwitterErrorCode + ": ";
                switch (TwitterErrorCode)
                {
                    // https://dev.twitter.com/overview/api/response-codes
                    case Api.TwitterErrorCode.AuthenticationFailed:
                        problemType = ProblemType.AuthorizationError;
                        description += "Could not authenticate you - " +
                                       "Your call could not be completed as dialed.";
                        return;
                    case Api.TwitterErrorCode.PageNotExist:
                        problemType = ProblemType.ProtocolViolation;
                        description += "Sorry, that page does not exist - " +
                                       "the specified resource was not found.";
                        return;
                    case Api.TwitterErrorCode.AccountSuspended:
                        problemType = ProblemType.AuthorizationError;
                        description += "Your account is suspended and is not permitted to access this feature - " +
                                       "Corresponds with an HTTP 403 / the access token being used belongs to a suspended user " +
                                       "and they can’t complete the action you’re trying to take";
                        return;
                    case Api.TwitterErrorCode.ApiNoLongerSupported:
                        problemType = ProblemType.ProtocolViolation;
                        description += "The Twitter REST API v1 is no longer active - " +
                                       "Please migrate to API v1.1.";
                        return;
                    case Api.TwitterErrorCode.RateLimitExceeded:
                        problemType = ProblemType.RateLimitation;
                        description += "Rate limit exceeded - " +
                                       "The request limit for this resource has been reached for the " +
                                       "current rate limit window.";
                        return;
                    case Api.TwitterErrorCode.InvalidOrExpiredToken:
                        problemType = ProblemType.AuthorizationError;
                        description += "Invalid or expired token - " +
                                       "The access token used in the request is incorrect or has expired.";
                        return;
                    case Api.TwitterErrorCode.SslRequired:
                        problemType = ProblemType.ProtocolViolation;
                        description += "SSL is required - " +
                                       "Only SSL connections are allowed in the API, you should update your " +
                                       "request to a secure connection.";
                        return;
                    case Api.TwitterErrorCode.OverCapacity:
                        problemType = ProblemType.TwitterInfrastructureError;
                        description += "Over capacity - Twitter is temporarily over capacity.";
                        return;
                    case Api.TwitterErrorCode.InternalError:
                        problemType = ProblemType.TwitterInfrastructureError;
                        description += "Internal Error - An unknown internal error occurred on Twitter.";
                        return;
                    case Api.TwitterErrorCode.InvalidSignature:
                        problemType = ProblemType.AuthorizationError;
                        description += "Could not authenticate you - " +
                                       "it means that your oauth_timestamp is either ahead or behind " +
                                       "our acceptable range. (system time is incorrect?)";
                        return;
                    case Api.TwitterErrorCode.TooManyFollow:
                        problemType = ProblemType.RateLimitation;
                        description += "You are unable to follow more people at this time - " +
                                       "thrown when a user cannot follow another user due to some kind of limit.";
                        return;
                    case Api.TwitterErrorCode.AuthorizationRequired:
                        problemType = ProblemType.AuthorizationError;
                        description += "Sorry, you are not authorized to see this status - " +
                                       "thrown when a Tweet cannot be viewed by the authenticating user, " +
                                       "usually due to the tweet’s author having protected their tweets.";
                        return;
                    case Api.TwitterErrorCode.StatusUpdateLimit:
                        problemType = ProblemType.RateLimitation;
                        description += "User is over daily status update limit - " +
                                       "thrown when a tweet cannot be posted due to the user having no allowance " +
                                       "remaining to post. Despite the text in the error message indicating that " +
                                       "this error is only thrown when a daily limit is reached, this error will be " +
                                       "thrown whenever a posting limitation has been reached. Posting allowances " +
                                       "have roaming windows of time of unspecified duration.";
                        return;
                    case Api.TwitterErrorCode.StatusDuplicated:
                        problemType = ProblemType.InvalidPayload;
                        description += "Status is a duplicate - " +
                                       "the status text has been Tweeted already by the authenticated account.";
                        return;
                    case Api.TwitterErrorCode.BadAuthenticationData:
                        problemType = ProblemType.AuthorizationError;
                        description += "Bad authentication data - " +
                                       "the method requires authentication but it was not presented or was " +
                                       "wholly invalid.";
                        return;
                    case Api.TwitterErrorCode.SuspiciousRequest:
                        problemType = ProblemType.AuthorizationError;
                        description += "This request looks like it might be automated - " +
                                       "to protect our users from spam and other malicious activity, we can’t " +
                                       "complete this action right now.";
                        return;
                    case Api.TwitterErrorCode.LoginVerificationNeeded:
                        problemType = ProblemType.AuthorizationError;
                        description += "User must verify login - " +
                                       "returned as a challenge in xAuth when the user has login verification " +
                                       "enabled on their account and needs to be directed to twitter.com to generate " +
                                       "a temporary password.";
                        return;
                    case Api.TwitterErrorCode.EndpointGone:
                        problemType = ProblemType.ProtocolViolation;
                        description += "This endpoint has been retired and should not be used - " +
                                       "corresponds to a HTTP request to a retired URL.";
                        return;
                    case Api.TwitterErrorCode.ApiPermissionDenined:
                        problemType = ProblemType.AuthorizationError;
                        description += "Application cannot perform write actions - " +
                                       "thrown when the application is restricted from POST, PUT, or DELETE actions.";
                        return;
                    case Api.TwitterErrorCode.TryToMuteYourself:
                        problemType = ProblemType.AuthorizationError;
                        description += "You can’t mute yourself - " +
                                       "the authenticated user account cannot mute itself.";
                        return;
                    case Api.TwitterErrorCode.CouldNotMute:
                        problemType = ProblemType.AuthorizationError;
                        description += "You are not muting the specified user - " +
                                       "the authenticated user account is not muting the account a call is " +
                                       "attempting to unmute.";
                        return;
                    case Api.TwitterErrorCode.DirectMessageTooLong:
                        problemType = ProblemType.AuthorizationError;
                        description += "The text of your direct message is over the max character limit - " +
                                       "the message size exceeds the number of characters permitted in a direct message.";
                        return;

                }
            }
            // check twitter error kinds
            switch (StatusCode)
            {
                case HttpStatusCode.OK: // 200
                case HttpStatusCode.NotModified: // 304
                                                 // successfully completed (in TCP/IP protocol)
                    problemType = ProblemType.Unknown;
                    description += "Request is succeeded, but exception thrown. (bug?)";
                    return;

                case HttpStatusCode.InternalServerError: // 500
                case HttpStatusCode.BadGateway: // 502
                case HttpStatusCode.ServiceUnavailable: // 503
                case HttpStatusCode.GatewayTimeout: // 504
                    problemType = ProblemType.TwitterInfrastructureError;
                    description += "Twitter server is temporarily down. Your request has not been completed.";
                    return;

                case HttpStatusCode.BadRequest: // 400
                case HttpStatusCode.NotFound: // 404
                case HttpStatusCode.Gone: // 410
                    problemType = ProblemType.ProtocolViolation;
                    description += "Endpoint is unknown or has gone.";
                    return;

                case (HttpStatusCode)420: // Enhance Your Calm (v1 Search API Rate Limit)
                case (HttpStatusCode)429: // Too Many Requests
                    problemType = ProblemType.RateLimitation;
                    description += "Request is up to rate limit.";
                    return;

                case HttpStatusCode.NotAcceptable: // 406 Search API invalid format request
                case (HttpStatusCode)422: // Unprocessable Entity
                    problemType = ProblemType.InvalidPayload;
                    description += "invalid format request/unprocessable entity.";
                    return;

                case HttpStatusCode.Unauthorized: // 401
                case HttpStatusCode.Forbidden: // 403
                    problemType = ProblemType.AuthorizationError;
                    description += "could not authenticate you.";
                    return;
            }
            problemType = ProblemType.Unknown;
            description += "unknown status code/twitter code: Twitter protocol violation?";
        }

        public HttpStatusCode StatusCode { get; }

        public TwitterErrorCode? TwitterErrorCode { get; }

        public ProblemType ProblemType { get; }

        public string Description { get; }

        public override string ToString()
        {
            return TwitterErrorCode.HasValue
                ? $"HTTP {StatusCode}/Twitter {TwitterErrorCode.Value}: {Message}"
                : InnerException.ToString();
        }
    }

    public class TwitterApiExceptionHandler : DelegatingHandler
    {
        public TwitterApiExceptionHandler(HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var resp = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
            if (!resp.IsSuccessStatusCode)
            {
                var rstr = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
                var json = DynamicJson.Parse(rstr);
                var ex = new TwitterApiException(resp.StatusCode, rstr);
                try
                {
                    if (json.errors() && json.errors[0].code() && json.errors[0].message())
                    {
                        ex = new TwitterApiException(resp.StatusCode,
                            json.errors[0].message, (TwitterErrorCode)((int)json.errors[0].code));
                    }
                }
                catch
                {
                    // ignore parse exception 
                }
                throw ex;
            }
            return resp.EnsureSuccessStatusCode();
        }
    }
}
