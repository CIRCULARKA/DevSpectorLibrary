using System.Net;

namespace DevSpector.SDK.Models
{
    public class ServerResponse
    {
        public ServerResponse(HttpStatusCode statusCode, string content = null)
        {
            ResponseStatusCode = statusCode;
            ResponseContent = content;
        }

        public HttpStatusCode ResponseStatusCode { get; }

        public string ResponseContent { get; }

        public bool IsSucceed =>
            ResponseStatusCode == HttpStatusCode.OK ||
            ResponseStatusCode == HttpStatusCode.Accepted ||
            ResponseStatusCode == HttpStatusCode.Continue ||
            ResponseStatusCode == HttpStatusCode.Found;
    }
}
