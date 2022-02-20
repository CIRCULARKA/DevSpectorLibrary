using System;
using System.Threading.Tasks;
using Xunit;
using DevSpector.SDK;
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
            var manager = new AuthorizationManager(new HostBuilder(_hostname, scheme: "https"));

            var login = "root";
            var password = "123Abc!";

            // Act
            var result = await manager.TrySignIn(login, password);

            // Assert
            Assert.Equal("root", result.Login);
            Assert.NotEmpty(result.Group);
            Assert.NotEmpty(result.AccessToken);
        }

        [Fact]
        public async Task CantGetUser()
        {
            // Arrange
            var manager = new AuthorizationManager(new HostBuilder(_hostname, scheme: "https"));

            var expectedLogin = "noname";
            var password = "no";

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(
                async () => await manager.TrySignIn(expectedLogin, password)
            );
        }
    }
}
