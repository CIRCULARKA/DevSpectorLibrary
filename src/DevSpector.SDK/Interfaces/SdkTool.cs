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
                throw new UnauthorizedException($"{response.ServerError.Error}: {response.ServerError.GetCommaSeparatedDescription()}");
            if (!response.IsSucceed)
                throw new InvalidOperationException($"{response.ServerError.Error}: {response.ServerError.GetCommaSeparatedDescription()}");
        }
    }
}
