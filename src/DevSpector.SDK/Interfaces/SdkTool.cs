using System;
using System.Net;
using DevSpector.SDK.Models;
using DevSpector.SDK.Exceptions;

namespace DevSpector.SDK
{
    public abstract class SdkTool
    {
        protected void ThrowIfNull(params object[] parameters)
        {
            foreach (var param in parameters)
                if (param == null) throw new ArgumentNullException();
        }

        protected void ThrowIfBadResponseStatus(ServerResponse response)
        {
            if (response.ResponseStatusCode == HttpStatusCode.Unauthorized)
                throw new UnauthorizedException($"Could not proceed operation: no access");
            if (!response.IsSucceed)
                throw new InvalidOperationException($"Could not proceed operation: {response.ResponseStatusCode} ({(int)response.ResponseStatusCode})");
        }

    }
}
