using System;
using System.Runtime.Serialization;

namespace Cadena.Engine.StreamReceivers
{
    [DataContract]
    public class StreamParseException : Exception
    {
        [DataMember]
        private readonly string _received;

        public StreamParseException(string received)
        {
            _received = received;
        }

        public StreamParseException(string message, string received)
            : base(message)
        {
            _received = received;
        }

        public StreamParseException(string message, string received, Exception inner)
            : base(message, inner)
        {
            _received = received;
        }

        public string ReceivedMessage
        {
            get { return _received; }
        }

        public override string ToString()
        {
            return base.ToString() + Environment.NewLine + "text: " + _received;
        }
    }
}