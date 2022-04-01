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
    public class JsonProvider : IServerDataProvider
    {
        private readonly HttpClient _client;

        private readonly IHostBuilder _builder;

        private readonly JsonSerializerOptions _serializationOptions;

        private readonly string _accessToken;

        public JsonProvider(IHostBuilder builder)
        {
            _client = new HttpClient();
            _builder = builder;

            _serializationOptions = ConfigureSerialization();
        }

        public JsonProvider(string accessToken, IHostBuilder builder)
        {
            _accessToken = accessToken;

            _client = new HttpClient();
            _builder = builder;

            _serializationOptions = ConfigureSerialization();

        }

        public TOut Deserialize<TOut>(string json) =>
            JsonSerializer.Deserialize<TOut>(json, _serializationOptions);

        public string Serialize<T>(T obj) =>
            JsonSerializer.Serialize<T>(obj, _serializationOptions);

        public async Task<ServerResponse> GetAsync(string path, Dictionary<string, string> parameters = null)
        {
            Uri requestUri = _builder.BuildTargetEndpoint(path, parameters);

            var response = await SendRequestAsync(requestUri, HttpMethod.Get);

            return new ServerResponse(
                response.StatusCode,
                await response.Content.ReadAsStringAsync()
            );
        }

        public async Task<ServerResponse> DeleteAsync(string path, Dictionary<string, string> parameters = null)
        {
            Uri requestUri = _builder.BuildTargetEndpoint(path, parameters);

            var response = await SendRequestAsync(requestUri, HttpMethod.Delete);

            return new ServerResponse(
                response.StatusCode,
                await response.Content.ReadAsStringAsync()
            );
        }

        public async Task<ServerResponse> PostAsync<T>(string path, T obj, Dictionary<string, string> parameters = null)
            where T: class
        {
            Uri requestUri = _builder.BuildTargetEndpoint(path, parameters);

            var response = await SendRequestWithBodyAsync<T>(requestUri, HttpMethod.Post, obj);

            return new ServerResponse(
                response.StatusCode,
                await response.Content.ReadAsStringAsync()
            );
        }

        public async Task<ServerResponse> PutAsync<T>(string path, T obj, Dictionary<string, string> parameters = null)
            where T: class
        {
            Uri requestUri = _builder.BuildTargetEndpoint(path, parameters);

            var response = await SendRequestWithBodyAsync<T>(requestUri, HttpMethod.Put, obj);

            return new ServerResponse(
                response.StatusCode,
                await response.Content.ReadAsStringAsync()
            );
        }

        private async Task<HttpResponseMessage> SendRequestAsync(Uri uri, HttpMethod method)
        {
            var request = new HttpRequestMessage {
                RequestUri = uri,
                Method = method
            };

            AddAccessKeyToHeader(request);

            return await _client.SendAsync(request);
        }

        private async Task<HttpResponseMessage> SendRequestWithBodyAsync<T>(Uri uri, HttpMethod method, T obj)
        {
            var request = new HttpRequestMessage {
                RequestUri = uri,
                Method = method
            };

            AddAccessKeyToHeader(request);

            var serializedObject = Serialize(obj);
            request.Content = new StringContent(serializedObject, Encoding.UTF8, "application/json");

            return await _client.SendAsync(request);
        }

        private void AddAccessKeyToHeader(HttpRequestMessage message)
        {
            if (_accessToken != null)
                message.Headers.Add("API", _accessToken);
        }

        private JsonSerializerOptions ConfigureSerialization()
        {
            var result = new JsonSerializerOptions();
            result.PropertyNameCaseInsensitive = true;
            result.Encoder = JavaScriptEncoder.Create(
                UnicodeRanges.BasicLatin,
                UnicodeRanges.Cyrillic
            );

            return result;
        }
    }
}
