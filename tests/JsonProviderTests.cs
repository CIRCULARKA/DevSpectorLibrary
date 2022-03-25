using System;
using System.Linq;
using System.Net;
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

        private readonly string _host = "dev-devspector.herokuapp.com";

        private readonly string[] _endpoints = new string[] {
            "api/users",
            "api/users/groups",
            "api/devices",
            "api/devices/types",
            "api/location/housings",
        };

        private readonly IHostBuilder _hostBuilder;

        public JsonProviderTests(ServerConnectionFixture conFix)
        {
            _connectionFixture = conFix;

            _hostBuilder = new HostBuilder(_host, scheme: "https");
        }

        [Fact]
        public async void ReturnsObjects()
        {
            // Arrange
            var provider = new JsonProvider(_hostBuilder);

            User user = await _connectionFixture.GetAuthorizedUser();
            string accessToken = user.AccessToken;

            // Assert
            foreach (var endpoint in _endpoints)
            {
                ServerResponse response = await provider.GetDataFromServerAsync(endpoint, accessToken);

                Assert.Equal(HttpStatusCode.OK, response.ResponseStatusCode);
                Assert.True(response.IsSucceed);
                Assert.NotNull(response.ResponseContent);
            }
        }

        [Fact]
        public async void CantGetDataWithoutAccessKey()
        {
            // Arrange
            var provider = new JsonProvider(_hostBuilder);

            // Assert
            foreach (var endpoint in _endpoints)
            {
                var response = await provider.GetDataFromServerAsync(endpoint);

                Assert.Equal(HttpStatusCode.Unauthorized, response.ResponseStatusCode);
                Assert.False(response.IsSucceed);
            }
        }

        [Fact]
        public async void CanSendPostRequest()
        {
            // Arrange
            User superUser = await _connectionFixture.GetAuthorizedUser();

            var provider = new JsonProvider(_hostBuilder);

            List<UserGroup> userGroups = await _connectionFixture.GetFromServerAsync<List<UserGroup>>(
                $"{_connectionFixture.ServerFullAddress}/users/groups?api={superUser.AccessToken}"
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
            var response = await provider.PostDataToServerAsync<UserToCreate>(
                "api/users/create",
                expectedUser,
                superUser.AccessToken
            );

            // Assert
            List<User> actualUsers = await _connectionFixture.GetFromServerAsync<List<User>>(
                $"{_connectionFixture.ServerFullAddress}/users?api={superUser.AccessToken}"
            );

            User addedUser = actualUsers.FirstOrDefault(u => u.Login == expectedUser.Login);
            Assert.NotNull(addedUser);
            Assert.Equal(expectedUser.Login, addedUser.Login);
            Assert.Equal(expectedUser.FirstName, addedUser.FirstName);
            Assert.Equal(expectedUser.Surname, addedUser.Surname);
            Assert.Equal(expectedUser.Patronymic, addedUser.Patronymic);
        }
    }
}
