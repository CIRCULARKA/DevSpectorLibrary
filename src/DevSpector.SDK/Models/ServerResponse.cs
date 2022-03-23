using System.Net;

namespace DevSpector.SDK.Authorization
{
    public class ServerResponse
    {
        public HttpStatusCode ResponseStatus { get; }

        public string ResponseContent { get; }
    }
}
