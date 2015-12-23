using System;

namespace Cadena.Engine
{
    [Serializable]
    public sealed class ReceiverOperationException : Exception
    {
        public ProblemType ProblemType { get; }

        public ReceiverOperationException()
        {
        }

        public ReceiverOperationException(ProblemType type, string message) : base(message)
        {
            ProblemType = type;
        }

        public ReceiverOperationException(ProblemType type, string message, Exception inner) : base(message, inner)
        {
            ProblemType = type;
        }
    }
}
