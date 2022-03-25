using System;
using System.IO;
using System.Text;
using System.Text.Unicode;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using DevSpector.SDK.Models;

namespace DevSpector.SDK
{
    public class JsonProvider : IRawDataProvider
    {
        private readonly HttpClient _client;

        private readonly IHostBuilder _builder;

        private readonly JsonSerializerOptions _serializationOptions;

        public JsonProvider(IHostBuilder builder)
        {
            _client = new HttpClient();
            _builder = builder;

            _serializationOptions = new JsonSerializerOptions();
            _serializationOptions.PropertyNameCaseInsensitive = true;
            _serializationOptions.Encoder = JavaScriptEncoder.Create(
                UnicodeRanges.BasicLatin,
                UnicodeRanges.Cyrillic
            );
        }

        public TOut Deserialize<TOut>(string json) =>
            JsonSerializer.Deserialize<TOut>(json, _serializationOptions);

        public string Serialize<T>(T obj) =>
            JsonSerializer.Serialize<T>(obj, _serializationOptions);

        public async Task<ServerResponse> GetDataFromServerAsync(string path, string accessToken = null, Dictionary<string, string> parameters = null)
        {
            Uri requestUri = _builder.BuildTargetEndpoint(path, parameters);

            var response = await SendGetRequestAsync(requestUri, accessToken);

            return new ServerResponse(
                response.StatusCode,
                await response.Content.ReadAsStringAsync()
            );
        }

        public async Task<ServerResponse> PostDataToServerAsync<T>(string path, T obj, string accessToken = null, Dictionary<string, string> parameteres = null)
        {
            Uri requestUri = _builder.BuildTargetEndpoint(path, parameteres);

            var response = await SendPostRequestAsync<T>(requestUri, obj, accessToken);

            return new ServerResponse(
                response.StatusCode,
                await response.Content.ReadAsStringAsync()
            );
        }

        private async Task<HttpResponseMessage> SendGetRequestAsync(Uri uri, string accessToken = null)
        {
            var request = new HttpRequestMessage {
                RequestUri = uri,
                Method = HttpMethod.Get
            };

            if (accessToken != null)
                request.Headers.Add("API", accessToken);

            return await _client.SendAsync(request);
        }

        private async Task<HttpResponseMessage> SendPostRequestAsync<T>(Uri uri, T obj, string accessToken = null)
        {
            var request = new HttpRequestMessage {
                RequestUri = uri,
                Method = HttpMethod.Post
            };

            if (accessToken != null)
                request.Headers.Add("API", accessToken);

            var serializedObject = Serialize(obj);
            request.Content = new StringContent(serializedObject, Encoding.UTF8, "application/json");

            return await _client.SendAsync(request);
        }
    }
}
