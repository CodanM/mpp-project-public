#nullable enable
using System;

namespace services
{
    public class ContestMgmtException : Exception
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
    }
}