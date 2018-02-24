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

        #endregion error handling/backoff constants

        private readonly IApiAccessor _accessor;
        private readonly IStreamHandler _handler;

        private BackoffMode _backoffMode;

        private long _backoffWait;

        private int _hardErrorCount;

        private CancellationTokenSource _currentSource = null;

        [NotNull]
        private readonly HashSet<string> _trackKeywords = new HashSet<string>();

        #region User Stream properties

        /// <summary>
        /// Track keywords
        /// </summary>
        [NotNull]
        public IEnumerable<string> TrackKeywords
        {
            get => _trackKeywords;
            set
            {
                var changed = false;
                var set = new HashSet<string>(value);
                foreach (var item in set)
                {
                    if (_trackKeywords.Add(item))
                    {
                        Debug.WriteLine("track add: " + item + " into " + _accessor.Id);
                        changed = true;
                    }
                }
                foreach (var item in _trackKeywords.ToArray())
                {
                    if (!set.Remove(item))
                    {
                        _trackKeywords.Remove(item);
                        changed = true;
                    }
                }
                if (changed)
                {
                    Debug.WriteLine("track reconnect...");
                    Reconnect();
                }
            }
        }

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
        public bool StallWarnings { get; }

        /// <summary>
        /// filter_level parameter
        /// </summary>
        public StreamFilterLevel StreamFilterLevel { get; }

        /// <summary>
        /// current connection status
        /// </summary>
        public StreamState CurrentState { get; private set; }

        #endregion User Stream properties

        public UserStreamReceiver(IApiAccessor accessor, IStreamHandler handler)
        {
            _accessor = accessor;
            _handler = handler;
            ChangeState(StreamState.Disconnected);
            // set default values to parameters
            StallWarnings = true;
            StreamFilterLevel = StreamFilterLevel.None;
            CurrentState = StreamState.Connected;
            _backoffMode = BackoffMode.None;
            _backoffWait = 0;
            _hardErrorCount = 0;
        }

        public void Reconnect()
        {
            _currentSource?.Cancel();
        }

        protected override async Task ExecuteInternalAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                ChangeState(StreamState.Connecting);
                try
                {
                    _currentSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                    ChangeState(StreamState.Connected);
                    Debug.WriteLine("Connect userstream: " + _accessor.Id + ", with track: " +
                                    String.Join(",", TrackKeywords));
                    await UserStreams.ConnectAsync(_accessor, ParseLine, _userStreamTimeout, _currentSource.Token,
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
                    if (_currentSource.IsCancellationRequested)
                    {
                        Debug.WriteLine($"ID {_accessor.Id} : reconnect required.");
                        continue;
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
            // reset error count and backoff flag
            _hardErrorCount = 0;
            _backoffMode = BackoffMode.None;
            ChangeState(StreamState.Connected);
            UserStreamParser.ParseStreamLine(json, _handler);
        }

        private async Task<bool> HandleException(Exception ex)
        {
            Log("Exception on User Stream Receiver: " + Environment.NewLine + ex);
            if (ex is TwitterApiException tx)
            {
                // protocol error
                Log($"Twitter API Exception: [status-code: {tx.StatusCode} twitter-code: {tx.TwitterErrorCode}]");
                _handler.OnMessage(new StreamErrorMessage(_accessor, tx.StatusCode, tx.TwitterErrorCode));
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
                        Log("Permission denied / Parameter out var of range");
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
            _handler.OnMessage(new StreamWaitMessage(_accessor, _backoffWait));
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
            if (CurrentState == state) return;
            CurrentState = state;
            _handler.OnStateChanged(state);
        }
    }

    internal enum BackoffMode
    {
        None,
        NetworkError,
        ProtocolError,
    }
}