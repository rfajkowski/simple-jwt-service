using System;

namespace AuthLibrary.Exceptions
{
    /// <summary>
    /// Exception thrown for security key-related errors.
    /// </summary>
    public class SecurityKeyException : Exception
    {
        public SecurityKeyException() { }
        public SecurityKeyException(string message) : base(message) { }
        public SecurityKeyException(string message, Exception innerException) : base(message, innerException) { }
    }
}
