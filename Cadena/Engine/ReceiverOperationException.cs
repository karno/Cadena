﻿using System;
using System.Runtime.Serialization;

namespace Cadena.Engine
{
    [DataContract]
    public sealed class ReceiverOperationException : Exception
    {
        [DataMember]
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