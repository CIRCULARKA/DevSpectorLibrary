using System;

namespace DevSpector.SDK.Exceptions
{
    /// <summary>
    /// This exception must be thrown if server denies access to its resources
    /// because issuer has no rights to get access
    /// </summary>
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message) : base(message) { }
    }
}
