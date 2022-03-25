using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Text.Json;
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

        // public string ServerHostname =>
        //     "dev-devspector.herokuapp.com";

        // public string ServerHostname =>
        //     "localhost";

        public string ServerHostname =>
            "dev-devspector.herokuapp.com";

        // public int ServerPort =>
        //     5000;

        public int ServerPort =>
            80;

        /// <summary>
        /// Input path without trailing '/'. Uses superuser access token
        /// </summary>
        public async Task<T> GetFromServerAsync<T>(string path, Dictionary<string, string> parameters = null)
        {
            // Get access key
            User superUser = await GetSuperUser();
            var accessKey = superUser.AccessToken;

            var uriBuilder = new UriBuilder($"http://{ServerHostname}:{ServerPort}/api/{path}?api={accessKey}");

            // Build uri from parameters
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

            var response = await _client.GetAsync(uriBuilder.Uri.ToString());
            var responseContent = await response.Content.ReadAsStringAsync();

            return DeserializeJson<T>(responseContent);
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
    }
}
