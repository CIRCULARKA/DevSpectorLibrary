using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xunit;
using DevSpector.SDK;
using DevSpector.SDK.DTO;
using DevSpector.SDK.Models;

namespace DevSpector.Tests
{
    [Collection(nameof(FixtureCollection))]
    public class JsonProviderTests
    {
        private readonly ServerConnectionFixture _connectionFixture;

        private readonly string[] _endpoints = new string[] {
            "api/users",
            "api/users/groups",
            "api/devices",
            "api/devices/types",
            "api/location/housings",
        };

        public JsonProviderTests(ServerConnectionFixture conFix)
        {
            _connectionFixture = conFix;
        }

        [Fact]
        public async void ReturnsObjects()
        {
            // Arrange
            IServerDataProvider provider = await CreateJsonProvider();

            // Assert
            foreach (var endpoint in _endpoints)
            {
                ServerResponse response = await provider.GetAsync(endpoint);

                Assert.Equal(HttpStatusCode.OK, response.ResponseStatusCode);
                Assert.True(response.IsSucceed);
                Assert.NotNull(response.ResponseContent);
            }
        }

        [Fact]
        public async void CantGetDataWithoutAccessKey()
        {
            // Arrange
            IServerDataProvider provider = await CreateJsonProvider(
                useWrongAccessKey: true
            );

            // Assert
            foreach (var endpoint in _endpoints)
            {
                var response = await provider.GetAsync(endpoint);

                Assert.Equal(HttpStatusCode.Unauthorized, response.ResponseStatusCode);
                Assert.False(response.IsSucceed);
            }
        }

        [Fact]
        public async void CanSendPostRequest()
        {
            // Arrange
            IServerDataProvider provider = await CreateJsonProvider();

            List<UserGroup> userGroups = await _connectionFixture.GetFromServerAsync<List<UserGroup>>(
                "users/groups"
            );

            var expectedUser = new UserToCreate {
                Login = Guid.NewGuid().ToString(),
                FirstName = Guid.NewGuid().ToString(),
                Surname = Guid.NewGuid().ToString(),
                Patronymic = Guid.NewGuid().ToString(),
                Password = "123Abc!",
                GroupID = userGroups.FirstOrDefault().ID
            };

            // Act
            var response = await provider.PostAsync<UserToCreate>(
                "api/users/create",
                expectedUser
            );

            // Assert
            List<User> actualUsers = await _connectionFixture.GetFromServerAsync<List<User>>(
                "users"
            );

            User addedUser = actualUsers.FirstOrDefault(u => u.Login == expectedUser.Login);
            Assert.NotNull(addedUser);
            Assert.Equal(expectedUser.Login, addedUser.Login);
            Assert.Equal(expectedUser.FirstName, addedUser.FirstName);
            Assert.Equal(expectedUser.Surname, addedUser.Surname);
            Assert.Equal(expectedUser.Patronymic, addedUser.Patronymic);
        }

		private async Task<IServerDataProvider> CreateJsonProvider(bool useWrongAccessKey = false)
		{
			User superUser = await _connectionFixture.GetSuperUser();

			return new JsonProvider(
				useWrongAccessKey ? "wrongKey ": superUser.AccessToken,
				new HostBuilder(
					hostname: _connectionFixture.ServerHostname,
					scheme: "https"
				)
			);
		}

    }
}
