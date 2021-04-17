#nullable enable
using System;
using System.Runtime.Serialization;

namespace services
{
    [Serializable]
    public class ContestMgmtException : ApplicationException
    {
        public ContestMgmtException()
        {
        }

        public ContestMgmtException(string? message) : base(message)
        {
        }

        public ContestMgmtException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected ContestMgmtException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}