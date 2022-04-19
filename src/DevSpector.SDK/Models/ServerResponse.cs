using System.Net;
using System.Text.Json;
using DevSpector.SDK.DTO;

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

        public ServerError GetError()
        {
            if (IsSucceed) return null;

            return JsonSerializer.Deserialize<ServerError>(
                ResponseContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
        }

        public bool IsSucceed =>
            ResponseStatusCode == HttpStatusCode.OK ||
            ResponseStatusCode == HttpStatusCode.Accepted ||
            ResponseStatusCode == HttpStatusCode.Continue ||
            ResponseStatusCode == HttpStatusCode.Found;
    }
}
