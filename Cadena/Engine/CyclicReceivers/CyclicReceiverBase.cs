using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Cadena.Api;
using Cadena.Data;

namespace Cadena.Engine.CyclicReceivers
{
    /// <summary>
    /// Base class for request cyclic information.
    /// </summary>
    public abstract class CyclicReceiverBase : IReceiver
    {
        protected virtual long MinimumIntervalTicks => TimeSpan.FromSeconds(30).Ticks;

        protected virtual double ApiConsumptionLimitRatio => 0.8;

        /// <summary>
        /// Execute request
        /// </summary>
        /// <param name="token">cancellation token</param>
        /// <returns>task object for awaiting completion</returns>
        protected abstract Task<RateLimitDescription> Execute(CancellationToken token);

        /// <summary>
        /// Get priority of this request
        /// </summary>
        public virtual RequestPriority Priority => RequestPriority.Middle;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task<TimeSpan> ExecuteAsync(CancellationToken token)
        {
            try
            {
                var rld = await Execute(token).ConfigureAwait(false);
                // calculate rates
                var remainTime = rld.Reset - DateTime.Now;
                if (remainTime < TimeSpan.Zero)
                {
                    // reset time was already arrived.
                    return TimeSpan.FromTicks(1);
                }

                // target interval(ticks per access)
                var targIntv = remainTime.Ticks / (rld.Remain * ApiConsumptionLimitRatio);
                return TimeSpan.FromTicks(Math.Max((long)targIntv, MinimumIntervalTicks));
            }
            catch (TwitterApiException tx)
            {
                var msg = $"(Exception handled by ExecuteAsync) {(int)tx.StatusCode}: {tx.StatusCode}";
                if (tx.TwitterErrorCode != null)
                {
                    msg += $" / T{(int)tx.TwitterErrorCode.Value}: {tx.TwitterErrorCode.Value}";
                    switch (tx.TwitterErrorCode)
                    {
                        // https://dev.twitter.com/overview/api/response-codes
                        case TwitterErrorCode.AuthenticationFailed:
                            throw new ReceiverOperationException(ProblemType.AuthorizationError,
                                "32: Could not authenticate you - " +
                                "Your call could not be completed as dialed.", tx);
                        case TwitterErrorCode.PageNotExist:
                            throw new ReceiverOperationException(ProblemType.ProtocolViolation,
                                "34: Sorry, that page does not exist - " +
                                "the specified resource was not found. / HTTP 404", tx);
                        case TwitterErrorCode.AccountSuspended:
                            throw new ReceiverOperationException(ProblemType.AuthorizationError,
                                "64: Your account is suspended and is not permitted to access this feature - " +
                                "Corresponds with an HTTP 403 / the access token being used belongs to a suspended user " +
                                "and they can’t complete the action you’re trying to take", tx);
                        case TwitterErrorCode.ApiNoLongerSupported:
                            throw new ReceiverOperationException(ProblemType.ProtocolViolation,
                                "68: The Twitter REST API v1 is no longer active - " +
                                "Please migrate to API v1.1.", tx);
                        case TwitterErrorCode.RateLimitExceeded:
                            throw new ReceiverOperationException(ProblemType.RateLimitation,
                                "88: Rate limit exceeded - The request limit for this resource has been " +
                                "reached for the current rate limit window.", tx);
                        case TwitterErrorCode.InvalidOrExpiredToken:
                            throw new ReceiverOperationException(ProblemType.AuthorizationError,
                                "89: Invalid or expired token - The access token used in the request is " +
                                "incorrect or has expired.", tx);
                        case TwitterErrorCode.SslRequired:
                            throw new ReceiverOperationException(ProblemType.ProtocolViolation,
                                "92: SSL is required - Only SSL connections are allowed in the API, " +
                                "you should update your request to a secure connection.", tx);
                        case TwitterErrorCode.OverCapacity:
                            throw new ReceiverOperationException(ProblemType.TwitterInfrastructureError,
                                "130: Over capacity - Twitter is temporarily over capacity. / HTTP 503", tx);
                        case TwitterErrorCode.InternalError:
                            throw new ReceiverOperationException(ProblemType.TwitterInfrastructureError,
                                "131: Internal Error - An unknown internal error occurred on Twitter. / HTTP 500", tx);
                        case TwitterErrorCode.InvalidSignature:
                            throw new ReceiverOperationException(ProblemType.AuthorizationError,
                                "135: Could not authenticate you - it means that your oauth_timestamp is either " +
                                "ahead or behind our acceptable range. (system time is incorrect?) / HTTP 401", tx);
                        case TwitterErrorCode.TooManyFollow:
                            throw new ReceiverOperationException(ProblemType.RateLimitation,
                                "161: You are unable to follow more people at this time - thrown when a user " +
                                "cannot follow another user due to some kind of limit. / HTTP 403", tx);
                        case TwitterErrorCode.AuthorizationRequired:
                            throw new ReceiverOperationException(ProblemType.AuthorizationError,
                                "179: Sorry, you are not authorized to see this status - thrown when a Tweet cannot be " +
                                "viewed by the authenticating user, usually due to the tweet’s author having protected " +
                                "their tweets. / HTTP 403", tx);
                        case TwitterErrorCode.StatusUpdateLimit:
                            throw new ReceiverOperationException(ProblemType.RateLimitation,
                                "185: User is over daily status update limit - thrown when a tweet cannot be posted " +
                                "due to the user having no allowance remaining to post. Despite the text in the error " +
                                "message indicating that this error is only thrown when a daily limit is reached, this " +
                                "error will be thrown whenever a posting limitation has been reached. Posting allowances " +
                                "have roaming windows of time of unspecified duration. / HTTP 403", tx);
                        case TwitterErrorCode.StatusDuplicated:
                            throw new ReceiverOperationException(ProblemType.InvalidPayload,
                                "187: Status is a duplicate - the status text has been Tweeted already by the " +
                                "authenticated account.", tx);
                        case TwitterErrorCode.BadAuthenticationData:
                            throw new ReceiverOperationException(ProblemType.AuthorizationError,
                                "215: Bad authentication data - the method requires authentication but it was not " +
                                "presented or was wholly invalid. / HTTP 400", tx);
                        case TwitterErrorCode.SuspiciousRequest:
                            throw new ReceiverOperationException(ProblemType.AuthorizationError,
                                "226: This request looks like it might be automated - to protect our users from spam and " +
                                "other malicious activity, we can’t complete this action right now.", tx);
                        case TwitterErrorCode.LoginVerificationNeeded:
                            throw new ReceiverOperationException(ProblemType.AuthorizationError,
                                "231: User must verify login - returned as a challenge in xAuth when the user has login " +
                                "verification enabled on their account and needs to be directed to twitter.com to " +
                                "generate a temporary password.", tx);
                        case TwitterErrorCode.EndpointGone:
                            throw new ReceiverOperationException(ProblemType.ProtocolViolation,
                                "251: This endpoint has been retired and should not be used - corresponds to a HTTP " +
                                "request to a retired URL. / HTTP 410", tx);
                        case TwitterErrorCode.ApiPermissionDenined:
                            throw new ReceiverOperationException(ProblemType.AuthorizationError,
                                "261: Application cannot perform write actions - thrown when the application is restricted " +
                                "from POST, PUT, or DELETE actions. / HTTP 403", tx);
                        case TwitterErrorCode.TryToMuteYourself:
                            throw new ReceiverOperationException(ProblemType.AuthorizationError,
                                "271: You can’t mute yourself - the authenticated user account cannot mute itself. " +
                                "/ HTTP 403", tx);
                        case TwitterErrorCode.CouldNotMute:
                            throw new ReceiverOperationException(ProblemType.AuthorizationError,
                                "272: You are not muting the specified user - the authenticated user account is not " +
                                "muting the account a call is attempting to unmute. / HTTP 403", tx);
                        case TwitterErrorCode.DirectMessageTooLong:
                            throw new ReceiverOperationException(ProblemType.AuthorizationError,
                                "354: The text of your direct message is over the max character limit - the message size " +
                                "exceeds the number of characters permitted in a direct message. / HTTP 403", tx);

                    }
                }
                // check twitter error kinds
                switch (tx.StatusCode)
                {
                    case HttpStatusCode.OK: // 200
                    case HttpStatusCode.NotModified: // 304
                        // successfully completed (in TCP/IP protocol)
                        break;

                    case HttpStatusCode.InternalServerError: // 500
                    case HttpStatusCode.BadGateway: // 502
                    case HttpStatusCode.ServiceUnavailable: // 503
                    case HttpStatusCode.GatewayTimeout: // 504
                        throw new ReceiverOperationException(ProblemType.TwitterInfrastructureError, msg, tx);

                    case HttpStatusCode.BadRequest: // 400
                    case HttpStatusCode.NotFound: // 404
                    case HttpStatusCode.Gone: // 410
                        throw new ReceiverOperationException(ProblemType.ProtocolViolation, msg, tx);

                    case (HttpStatusCode)420: // Enhance Your Calm (v1 Search API Rate Limit)
                    case (HttpStatusCode)429: // Too Many Requests
                        throw new ReceiverOperationException(ProblemType.RateLimitation, msg, tx);

                    case HttpStatusCode.NotAcceptable: // 406 Search API invalid format request
                    case (HttpStatusCode)422: // Unprocessable Entity
                        throw new ReceiverOperationException(ProblemType.InvalidPayload, msg);

                    case HttpStatusCode.Unauthorized: // 401
                    case HttpStatusCode.Forbidden: // 403
                        throw new ReceiverOperationException(ProblemType.AuthorizationError, msg, tx);
                }
                throw new ReceiverOperationException(ProblemType.ProtocolViolation,
                    msg + " [Unhandled: Twitter protocol violation?]", tx);
            }
            catch (HttpRequestException ex)
            {
                throw new ReceiverOperationException(ProblemType.TwitterInfrastructureError,
                    "(Exception handled by ExecuteAsync) Twitter infrastructure problem?", ex);
            }
            catch (WebException ex)
            {
                throw new ReceiverOperationException(ProblemType.NetworkError,
                    "(Exception handled by ExecuteAsync) network problem?", ex);
            }
            catch (Exception ex)
            {
                // retry after minimum timespan.
                throw new ReceiverOperationException(ProblemType.Unknown,
                    "(Exception handled by ExecuteAsync) unknown...", ex);
            }
        }

        ~CyclicReceiverBase()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
        }
    }
}
