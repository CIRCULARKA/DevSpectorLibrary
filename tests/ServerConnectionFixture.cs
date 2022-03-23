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

        public async Task<T> GetFromServerAsync<T>(string address)
        {
            var response = await _client.GetAsync(address);
            var responseContent = await response.Content.ReadAsStringAsync();

            return DeserializeJson<T>(responseContent);
        }

        public async Task<User> GetAuthorizedUser()
        {
            var response = await _client.GetAsync("https://dev-devspector.herokuapp.com/api/users/authorize?login=root&password=123Abc!");
            var responseContent = await response.Content.ReadAsStringAsync();

            return DeserializeJson<User>(responseContent);
        }

        private T DeserializeJson<T>(string json) =>
            JsonSerializer.Deserialize<T>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
    }
}
