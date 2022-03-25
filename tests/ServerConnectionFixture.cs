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

        public string ServerHostname =>
            "dev-devspector.herokuapp.com";

        public string ServerFullAddress =>
            "https://dev-devspector.herokuapp.com/api";

        /// <summary>
        /// Input path without trailing '/'. Uses superuser access token
        /// </summary>
        public async Task<T> GetFromServerAsync<T>(string path)
        {
            User superUser = await GetSuperUser();
            var accessKey = superUser.AccessToken;

            var response = await _client.GetAsync($"{ServerFullAddress}/{path}?api={accessKey}");
            var responseContent = await response.Content.ReadAsStringAsync();

            return DeserializeJson<T>(responseContent);
        }

        public async Task<User> GetSuperUser()
        {
            var response = await _client.GetAsync($"{ServerFullAddress}/users/authorize?login=root&password=123Abc!");
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
