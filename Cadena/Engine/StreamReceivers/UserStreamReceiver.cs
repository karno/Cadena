using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Cadena.Api;
using Cadena.Api.Streams;
using Cadena.Data.Streams.Internals;
using Cadena.Engine._Internals.Parsers;
using JetBrains.Annotations;

namespace Cadena.Engine.StreamReceivers
{
    public sealed class UserStreamReceiver : StreamReceiverBase
    {
        private readonly TimeSpan _userStreamTimeout = TimeSpan.FromSeconds(70);

        #region error handling/backoff constants

        private const int MaxHardErrorCount = 3;

        private const int ProtocolErrorInitialWait = 5000; // 5 sec

        private const int ProtocolErrorMaxWait = 320000; // 320 sec

        private const int NetworkErrorInitialWait = 250; // 0.25 sec

        private const int NetworkErrorMaxWait = 16000; // 16 sec

        #endregion

        private readonly IApiAccess _access;
        private readonly IStreamHandler _handler;

        private bool _disposed;

        private StreamState _currentState = StreamState.Connected;

        private BackoffMode _backoffMode = BackoffMode.None;

        private long _backoffWait = 0;

        private int _hardErrorCount = 0;

        #region User Stream properties

        /// <summary>
        /// Track keywords
        /// </summary>
        [CanBeNull]
        public IEnumerable<string> TrackKeywords { get; set; }

        /// <summary>
        /// replies=all flag
        /// </summary>
        public bool RepliesAll { get; set; }

        /// <summary>
        /// include_followings_activity flag
        /// </summary>
        public bool IncludeFollowingsActivities { get; set; }

        /// <summary>
        /// stall_warnings flag
        /// </summary>
        public bool StallWarnings { get; set; }

        /// <summary>
        /// filter_level parameter
        /// </summary>
        public StreamFilterLevel StreamFilterLevel { get; set; }

        #endregion

        public UserStreamReceiver(IApiAccess access, IStreamHandler handler)
        {
            _access = access;
            _handler = handler;
            ChangeState(StreamState.Disconnected);
            // set parameter default value
            StallWarnings = true;
            StreamFilterLevel = StreamFilterLevel.None;
        }

        protected override async Task ExecuteInternalAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                ChangeState(StreamState.Connecting);
                _handler.OnStateChanged(StreamState.Connecting);
                try
                {
                    await UserStreams.Connect(_access, ParseLine, _userStreamTimeout, cancellationToken,
                        TrackKeywords, StallWarnings, StreamFilterLevel,
                        RepliesAll, IncludeFollowingsActivities)
                                     .ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    ChangeState(StreamState.Waiting);
                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }
                    if (await HandleException(ex).ConfigureAwait(false))
                    {
                        // if handled: continue running
                        continue;
                    }
                    ChangeState(StreamState.Disconnected);
                    throw;
                }
            }
            ChangeState(StreamState.Disconnected);
        }

        private void ParseLine(string json)
        {
            // reset counts
            _hardErrorCount = 0;
            ChangeState(StreamState.Connected);
            UserStreamParser.ParseStreamLine(json, _handler);
        }

        private async Task<bool> HandleException(Exception ex)
        {
            Log("Exception on User Stream Receiver: " + Environment.NewLine + ex);
            var tx = ex as TwitterApiException;
            if (tx != null)
            {
                // protocol error
                Log($"Twitter API Exception: [status-code: {tx.StatusCode} twitter-code: {tx.TwitterErrorCode}]");
                _handler.OnMessage(new StreamErrorMessage(_access, tx.StatusCode, tx.TwitterErrorCode));
                switch (tx.StatusCode)
                {
                    case HttpStatusCode.Unauthorized:
                        Log("Authorization failed.");
                        if (_hardErrorCount > MaxHardErrorCount)
                        {
                            return false;
                        }
                        break;
                    case HttpStatusCode.Forbidden:
                    case HttpStatusCode.NotFound:
                        Log("Endpoint not found / not accessible.");
                        if (_hardErrorCount > MaxHardErrorCount)
                        {
                            return false;
                        }
                        break;
                    case HttpStatusCode.NotAcceptable:
                    case HttpStatusCode.RequestEntityTooLarge:
                        Log("Specified argument could not be accepted.");
                        return false;
                    case HttpStatusCode.RequestedRangeNotSatisfiable:
                        Log("Permission denied / Parameter out of range");
                        return false;
                    case (HttpStatusCode)420: // Too many connections
                        Log("Too many connections are established.");
                        return false;
                }
                // general protocol error
                if (_backoffMode == BackoffMode.ProtocolError)
                {
                    // exponential backoff
                    _backoffWait *= 2;
                }
                else
                {
                    _backoffWait = ProtocolErrorInitialWait;
                    _backoffMode = BackoffMode.ProtocolError;
                }
                if (_backoffWait >= ProtocolErrorMaxWait)
                {
                    Log("Protocol backoff limit exceeded.");
                    return false;
                }
            }
            else
            {
                // network error
                if (_backoffMode == BackoffMode.NetworkError)
                {
                    // linear backoff
                    _backoffWait += NetworkErrorInitialWait;
                }
                else
                {
                    _backoffWait = NetworkErrorInitialWait;
                    _backoffMode = BackoffMode.NetworkError;
                }
                if (_backoffWait >= NetworkErrorMaxWait)
                {
                    Log("Network backoff limit exceeded.");
                    return false;
                }
            }
            Log($"Waiting reconnection... [{_backoffWait} ms]");
            _handler.OnMessage(new StreamWaitMessage(_access, _backoffWait));
            await Task.Delay(TimeSpan.FromMilliseconds(_backoffWait)).ConfigureAwait(false);
            return true;
        }

        private void Log(string body)
        {
            var splitEach = body.Split('\r', '\n')
                                .Select(t => t.Trim())
                                .Where(t => !String.IsNullOrEmpty(t));
            foreach (var text in splitEach)
            {
                Debug.WriteLine("[USER-STREAMS] " + text);
                _handler.Log(text);
            }
        }

        private void ChangeState(StreamState state)
        {
            if (_currentState != state)
            {
                _currentState = state;
                _handler.OnStateChanged(state);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;
            }
        }
    }

    internal enum BackoffMode
    {
        None,
        NetworkError,
        ProtocolError,
    }
}
