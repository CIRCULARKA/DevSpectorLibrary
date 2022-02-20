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
            var provider = new JsonProvider(_host);

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

        [Fact]
        public void DoesUriBuiltProperly()
        {
            // Arrange
            var expectedHost1 = "testHost1";
            var expectedPort1 = 1234;

            var expectedHost2 = "testHost2";
            int expectedPort2 = 443;

            var expectedHost3 = "localhost";
            var expectedPort3 = 5040;

            // Act
            var provider1 = new JsonProvider(expectedHost1, expectedPort1);
            var provider2 = new JsonProvider(expectedHost2);
            var provider3 = new JsonProvider(expectedPort3);

            var uri1 = provider1.Host;
            var uri2 = provider2.Host;
            var uri3 = provider3.Host;

            // Assert
            Assert.Equal(expectedHost1, uri1.Host, ignoreCase: true);
            Assert.Equal(expectedPort1, uri1.Port);

            Assert.Equal(expectedHost2, uri2.Host, ignoreCase: true);
            Assert.Equal(expectedPort2, uri2.Port);

            Assert.Equal(expectedHost3, uri3.Host, ignoreCase: true);
            Assert.Equal(expectedPort3, uri3.Port);
        }
    }
}
