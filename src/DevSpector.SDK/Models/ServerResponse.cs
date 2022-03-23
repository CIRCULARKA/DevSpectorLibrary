using System.Net;

namespace DevSpector.SDK.Models
{
    public class ServerResponse
    {
        public HttpStatusCode ResponseStatus { get; }

        public string ResponseContent { get; }
    }
}
