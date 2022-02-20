using System;
using System.Threading.Tasks;
using Xunit;

namespace DevSpector.SDK.Tests
{
    public class JsonProviderTests
    {
        private readonly string _host = "dev-devspector.herokuapp.com";

        [Fact]
        public async void ThrowsWrongAPI()
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
