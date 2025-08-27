using System;

namespace Cority.AuthLibrary.Exceptions
{
    /// <summary>
    /// Exception thrown for token-related errors.
    /// </summary>
    public class TokenException : Exception
    {
        public TokenException() { }
        public TokenException(string message) : base(message) { }
        public TokenException(string message, Exception innerException) : base(message, innerException) { }
    }
}
