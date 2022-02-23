using System;
using System.Threading.Tasks;
using Xunit;
using DevSpector.SDK.Authorization;

namespace DevSpector.SDK.Tests
{
    public class JsonProviderTests
    {
        private readonly string _host = "dev-devspector.herokuapp.com";

        [Fact]
        public async void CanLoadFromServer()
        {
            // Arrange
            var builder = new HostBuilder(_host, scheme: "https");

            var provider = new JsonProvider(builder);

            var authManager = new AuthorizationManager(builder);
            var accessToken = (await authManager.TrySignIn("root", "123Abc!")).AccessToken;

            var actions = new Func<Task<string>>[] {
                async () => await provider.GetDevicesAsync(accessToken),
                async () => await provider.GetFreeIPAsync(accessToken),
                async () => await provider.GetHousingsAsync(accessToken),
                async () => await provider.GetUsersAsync(accessToken)
            };
        }

        [Fact]
        public async void ThrowsOnWrongAPI()
        {
            // Arrange
            var provider = new JsonProvider(new HostBuilder(_host, scheme: "https"));

            var actions = new Func<Task<string>>[] {
                async () => await provider.GetDevicesAsync("wrongToken"),
                async () => await provider.GetFreeIPAsync("wrongToken"),
                async () => await provider.GetHousingAsync(Guid.Empty, "wrongToken"),
                async () => await provider.GetHousingsAsync("wrongToken"),
                async () => await provider.GetUsersAsync("wrongToken")
            };

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(
                async () => {
                    foreach (var action in actions)
                        await action.Invoke();
                }
            );
        }
    }
}
