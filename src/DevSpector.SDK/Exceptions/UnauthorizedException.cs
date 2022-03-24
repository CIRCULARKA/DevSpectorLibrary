using System;

namespace DevSpector.SDK.Exceptions
{
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message) : base(message) { }
    }
}
