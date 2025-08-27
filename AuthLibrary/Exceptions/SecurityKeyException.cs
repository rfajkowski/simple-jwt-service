using System;

namespace Cority.AuthLibrary.Exceptions
{
    public class SecurityKeyException : Exception
    {
        public SecurityKeyException() { }

        public SecurityKeyException(string message) : base(message) { }

        public SecurityKeyException(string message, Exception innerException) : base(message, innerException) { }
    }
}
