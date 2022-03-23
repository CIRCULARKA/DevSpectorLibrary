using System.Text.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace DevSpector.Tests
{
    public class ServerConnectionFixture
    {
        public async Task<T> GetFromServerAsync<T>(string address)
        {
            var client = new HttpClient();
            var response = await client.GetAsync(address);
            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<T>(
                responseContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
        }
    }
}
