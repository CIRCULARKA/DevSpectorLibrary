using System;
using System.Threading.Tasks;
using Xunit;
using DevSpector.SDK.Authorization;
using DevSpector.SDK.Models;

namespace DevSpector.Tests.Common.SDK.Authorization
{
	public class AuthorizationManagerTests
    {
        private readonly string _hostname = "dev-devspector.herokuapp.com";

        [Fact]
        public void IsHostBuiltProperly()
        {
            // Arrange
            var expectedHost1 = "testhost";
            var expectedHost2 = "localhost";

            var expectedPort1 = 5000;
            var expectedPort2 = 5043;

            // Act
            var manager1 = new AuthorizationManager(expectedHost1, expectedPort1);
            var manager2 = new AuthorizationManager(expectedPort2);

            // Assert
            Assert.Equal(expectedHost1, manager1.Host.Host);
            Assert.Equal(expectedPort1, manager1.Host.Port);

            Assert.Equal(expectedHost2, manager2.Host.Host);
            Assert.Equal(expectedPort2, manager2.Host.Port);
        }

        [Fact]
        public async Task CanGetUser()
        {
            // Arrange
            var manager = new AuthorizationManager(_hostname);

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
            var manager = new AuthorizationManager(_hostname);

            var expectedLogin = "noname";
            var password = "no";

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(
                async () => await manager.TrySignIn(expectedLogin, password)
            );
        }
    }
}
