using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Text.Json;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using DevSpector.SDK.Models;

namespace DevSpector.Tests
{
    public class ServerConnectionFixture
    {
        private readonly HttpClient _client;

        public ServerConnectionFixture()
        {
            _client = new HttpClient();
        }

        public string ServerHostname =>
            "dev-devspector.herokuapp.com";

        public int ServerPort =>
            80;

        /// <summary>
        /// Input path without trailing '/'. Uses superuser access token
        /// </summary>
        public async Task<T> GetFromServerAsync<T>(
            string path,
            Dictionary<string, string> parameters = null,
            Dictionary<string, string> headers = null
        )
        {
            var request = await ConstructRequestMessage(path, HttpMethod.Get, parameters, headers);

            var response = await _client.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            return DeserializeJson<T>(responseContent);
        }

        public async Task<HttpStatusCode> SendChangesToServerAsync<T>(
            string path,
            T obj,
            HttpMethod method,
            Dictionary<string, string> parameters = null,
            Dictionary<string, string> headers = null

        )
        {
            if (method == HttpMethod.Head || method == HttpMethod.Get || method == HttpMethod.Trace)
                throw new ArgumentException("Specified method not allowed");

            var request = await ConstructRequestMessage(path, method, parameters, headers);

            request.Content = new StringContent(SerializeObject<T>(obj), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _client.SendAsync(request);

            return response.StatusCode;
        }

        public async Task<HttpStatusCode> DeleteFromServerAsync(
            string path,
            Dictionary<string, string> parameters = null,
            Dictionary<string, string> headers = null
        )
        {
            var request = await ConstructRequestMessage(path, HttpMethod.Delete, parameters, headers);

            HttpResponseMessage response = await _client.SendAsync(request);

            return response.StatusCode;
        }

        private async Task<HttpRequestMessage> ConstructRequestMessage(
            string path,
            HttpMethod method,
            Dictionary<string, string> parameters = null,
            Dictionary<string, string> headers = null
        )
        {
            // Get access key
            User superUser = await GetSuperUser();
            var accessKey = superUser.AccessToken;

            var uriBuilder = new UriBuilder($"http://{ServerHostname}:{ServerPort}/api/{path}?api={accessKey}");

            if (parameters != null)
            {
                parameters.Add("api", accessKey);

                var query = new StringBuilder();
                for (int i = 0; i < parameters.Count; i++)
                {
                    var pair = parameters.ElementAt(i);
                    query.Append($"{pair.Key}={pair.Value}");
                    if (i < parameters.Count - 1)
                        query.Append("&");
                }

                uriBuilder.Query = query.ToString();
            }

            var request = new HttpRequestMessage(method, uriBuilder.Uri);

            if (headers != null)
                foreach (var keyValuePair in headers)
                    request.Headers.Add(keyValuePair.Key, keyValuePair.Value);

            return request;
        }

        public async Task<User> GetSuperUser()
        {
            var response = await _client.GetAsync($"http://{ServerHostname}:{ServerPort}/api/users/authorize?login=root&password=123Abc!");
            var responseContent = await response.Content.ReadAsStringAsync();

            return DeserializeJson<User>(responseContent);
        }

        private T DeserializeJson<T>(string json) =>
            JsonSerializer.Deserialize<T>(
                json,
                new JsonSerializerOptions {
                    PropertyNameCaseInsensitive = true,
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic)
                }
            );

        private string SerializeObject<T>(T obj) =>
            JsonSerializer.Serialize<T>(
                obj,
                new JsonSerializerOptions {
                    PropertyNameCaseInsensitive = true,
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic)
                }
            );
    }
}
