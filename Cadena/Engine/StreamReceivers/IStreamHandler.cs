using System;
using System.Collections.Generic;
using System.Reflection;
using Cadena.Data;
using Cadena.Data.Streams;
using JetBrains.Annotations;

namespace Cadena.Engine.StreamReceivers
{
    public interface IStreamHandler
    {
        void OnStatus(TwitterStatus status);

        void OnMessage(StreamMessage notification);

        void OnException(StreamParseException exception);

        void OnStateChanged(StreamState state);

        void Log(string log);
    }

    public sealed class StreamHandler : IStreamHandler
    {
        private readonly Action<TwitterStatus> _statusHandler;
        private readonly Action<StreamParseException> _exceptionHandler;
        private readonly Action<StreamState> _stateHandler;
        private readonly Dictionary<Type, Action<StreamMessage>> _notificationHandlers;

        [CanBeNull]
        private readonly Action<string> _logHandler;

        public static StreamHandler Create([NotNull] Action<TwitterStatus> statusHandler,
            [NotNull] Action<StreamParseException> exceptionHandler, [NotNull] Action<StreamState> stateHandler)
        {
            if (statusHandler == null) throw new ArgumentNullException(nameof(statusHandler));
            if (exceptionHandler == null) throw new ArgumentNullException(nameof(exceptionHandler));
            return new StreamHandler(statusHandler, exceptionHandler, stateHandler, null);
        }

        public static StreamHandler Create([NotNull] Action<TwitterStatus> statusHandler,
            [NotNull] Action<StreamParseException> exceptionHandler, [NotNull] Action<StreamState> stateHandler,
            [NotNull] Action<string> logHandler)
        {
            if (statusHandler == null) throw new ArgumentNullException(nameof(statusHandler));
            if (exceptionHandler == null) throw new ArgumentNullException(nameof(exceptionHandler));
            if (logHandler == null) throw new ArgumentNullException(nameof(logHandler));
            return new StreamHandler(statusHandler, exceptionHandler, stateHandler, logHandler);
        }

        private StreamHandler([NotNull] Action<TwitterStatus> statusHandler,
            [NotNull] Action<StreamParseException> exceptionHandler, [NotNull] Action<StreamState> stateHandler,
            [CanBeNull] Action<string> logHandler)
        {
            if (statusHandler == null) throw new ArgumentNullException(nameof(statusHandler));
            if (exceptionHandler == null) throw new ArgumentNullException(nameof(exceptionHandler));
            if (stateHandler == null) throw new ArgumentNullException(nameof(stateHandler));
            _statusHandler = statusHandler;
            _exceptionHandler = exceptionHandler;
            _stateHandler = stateHandler;
            _logHandler = logHandler;
            _notificationHandlers = new Dictionary<Type, Action<StreamMessage>>();
        }

        public void OnStatus(TwitterStatus status)
        {
            _statusHandler(status);
        }

        public void OnException(StreamParseException exception)
        {
            _exceptionHandler(exception);
        }

        public void OnMessage(StreamMessage notification)
        {
            Action<StreamMessage> value;
            var type = notification.GetType();
            lock (_notificationHandlers)
            {
                do
                {
                    if (_notificationHandlers.TryGetValue(type, out value))
                    {
                        break;
                    }
                    type = type.GetTypeInfo().BaseType;
                } while (type != null && typeof(StreamMessage).IsAssignableFrom(type));
            }
            // invoke if value is not null
            value?.Invoke(notification);
        }

        public void OnStateChanged(StreamState state)
        {
            _stateHandler(state);
        }

        public void Log(string log)
        {
            _logHandler?.Invoke("[" + DateTime.Now.ToString("yy/MM/dd hh:mm:ss tt [zz]") + "]" + log);
        }

        public StreamHandler AddHandler<T>(Action<T> handler) where T : StreamMessage
        {
            lock (_notificationHandlers)
            {
                if (_notificationHandlers.ContainsKey(typeof(T)))
                {
                    throw new ArgumentException("a handler for the type " + typeof(T).Name + " is already registered.");
                }
                _notificationHandlers.Add(typeof(T), i => handler((T)i));
            }
            return this;
        }
    }

    public enum StreamState
    {
        /// <summary>
        /// Opening stream channel
        /// </summary>
        Connecting,

        /// <summary>
        /// Stream channel is connected
        /// </summary>
        Connected,

        /// <summary>
        /// Waiting reconnection for protecting twitter infrastructure
        /// </summary>
        Waiting,

        /// <summary>
        /// Stream channel is disconnected<para />
        /// Stream engine is not started or disconnected 
        /// (Unhandled exception thrown, reaching limit of connection trial, etc...)
        /// </summary>
        Disconnected,
    }
}
