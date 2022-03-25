using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using DevSpector.SDK;
using DevSpector.SDK.Models;
using DevSpector.SDK.Authorization;

namespace DevSpector.Tests.Common.SDK.Authorization
{
    [Collection(nameof(FixtureCollection))]
	public class AuthorizationManagerTests
    {
        private readonly ServerConnectionFixture _connectionFixture;

        private readonly string _hostname = "dev-devspector.herokuapp.com";

        private readonly string _testUserLogin = "root";

        private readonly string _testUserPassword = "123Abc!";

        public AuthorizationManagerTests(ServerConnectionFixture fixture)
        {
            _connectionFixture = fixture;
        }

        [Fact]
        public async Task CanGetUser()
        {
            // Arrange
            var manager = await CreateAuthManagerAsync();

            User expectedUser = await _connectionFixture.GetFromServerAsync<User>(
                "users/authorize",
                new Dictionary<string, string> {
                    { "login", _testUserLogin }, { "password", _testUserPassword }
                }
            );

            // Act
            var actualUser = await manager.TryToSignInAsync(_testUserLogin, _testUserPassword);

            // Assert
            Assert.Equal(expectedUser.Login, actualUser.Login);
            Assert.Equal(expectedUser.AccessToken, actualUser.AccessToken);
            Assert.Equal(expectedUser.Group, actualUser.Group);
        }

        [Fact]
        public async Task CantGetUser()
        {
            // Arrange
            var manager = await CreateAuthManagerAsync();

            // Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await manager.TryToSignInAsync("wrongLogin", "wrongPassword")
            );

            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await manager.TryToSignInAsync("wrongLogin", _testUserPassword)
            );

            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await manager.TryToSignInAsync(_testUserPassword, "wrongPassword")
            );
        }

		private async Task<IAuthorizationManager> CreateAuthManagerAsync(bool useWrongAccessKey = false)
		{
			User superUser = await _connectionFixture.GetSuperUser();

			IRawDataProvider provider = new JsonProvider(
				useWrongAccessKey ? "wrongKey ": superUser.AccessToken,
				new HostBuilder(
					hostname: _connectionFixture.ServerHostname,
					scheme: "https"
				)
			);

			return new AuthorizationManager(provider);
		}
    }
}
