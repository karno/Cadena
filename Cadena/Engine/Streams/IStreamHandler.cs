using System;
using System.Collections.Generic;
using Cadena.Data;
using Cadena.Data.Streams;
using JetBrains.Annotations;

namespace Cadena.Engine.Streams
{
    public interface IStreamHandler
    {
        void OnStatus(TwitterStatus status);

        void OnNotification(StreamMessage notification);

        void OnException(StreamParseException exception);
    }

    public sealed class StreamHandler : IStreamHandler
    {
        private readonly Action<TwitterStatus> _statusHandler;
        private readonly Action<StreamParseException> _exceptionHandler;
        private readonly Dictionary<Type, Action<StreamMessage>> _notificationHandlers;

        public static StreamHandler Create([NotNull] Action<TwitterStatus> statusHandler,
           [NotNull] Action<StreamParseException> exceptionHandler)
        {
            if (statusHandler == null) throw new ArgumentNullException(nameof(statusHandler));
            if (exceptionHandler == null) throw new ArgumentNullException(nameof(exceptionHandler));
            return new StreamHandler(statusHandler, exceptionHandler);
        }

        private StreamHandler(Action<TwitterStatus> statusHandler, Action<StreamParseException> exceptionHandler)
        {
            _statusHandler = statusHandler;
            _exceptionHandler = exceptionHandler;
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

        public void OnNotification(StreamMessage notification)
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
                    type = type.BaseType;
                } while (type != null && typeof(StreamMessage).IsAssignableFrom(type));
            }
            value?.Invoke(notification);
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
}
