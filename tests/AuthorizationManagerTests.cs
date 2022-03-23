using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;
using DevSpector.SDK;
using DevSpector.SDK.Models;
using DevSpector.SDK.Authorization;

namespace DevSpector.Tests.Common.SDK.Authorization
{
	public class AuthorizationManagerTests
    {
        private readonly string _hostname = "dev-devspector.herokuapp.com";

        [Fact]
        public async Task CanGetUser()
        {
            // Arrange
            var jsonProvider = new JsonProvider(new HostBuilder(_hostname, scheme: "https"));
            var manager = new AuthorizationManager(jsonProvider);

            var login = "root";
            var password = "123Abc!";

            User expectedUser = await GetFromServerAsync<User>(
                "https://" +
                _hostname +
                $"/api/users/authorize?login={login}&password={password}"
            );

            // Act
            var actualUser = await manager.TryToSignInAsync(login, password);

            // Assert
            Assert.Equal(expectedUser.Login, actualUser.Login);
            Assert.Equal(expectedUser.AccessToken, actualUser.AccessToken);
            Assert.Equal(expectedUser.Group, actualUser.Group);
        }

        private async Task<T> GetFromServerAsync<T>(string address)
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
