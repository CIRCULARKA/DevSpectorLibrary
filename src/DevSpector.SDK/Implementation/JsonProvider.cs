using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace DevSpector.SDK
{
    public class JsonProvider : IRawDataProvider
    {
        private readonly HttpClient _client;

        private readonly IHostBuilder _builder;

        public JsonProvider(IHostBuilder builder)
        {
            _client = new HttpClient();
        }

        public Uri TargetHost =>
            _builder.Host;

        public async Task<string> GetJsonFrom(string path, string accessToken)
        {
            var request = new HttpRequestMessage {
                RequestUri = new Uri(path),
                Method = HttpMethod.Get
            };

            request.Headers.Add("API", accessToken);

            var response = await _client.SendAsync(request);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                throw new ArgumentException("Wrong API");

            return await response.Content.ReadAsStringAsync();
        }
    }
}
