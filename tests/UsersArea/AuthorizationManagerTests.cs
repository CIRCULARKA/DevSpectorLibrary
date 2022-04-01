using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using DevSpector.SDK;
using DevSpector.SDK.Exceptions;
using DevSpector.SDK.DTO;
using DevSpector.SDK.Models;
using DevSpector.SDK.Authorization;

namespace DevSpector.Tests.Common.SDK.Authorization
{
    [Collection(nameof(FixtureCollection))]
	public class AuthorizationManagerTests
    {
        private readonly ServerConnectionFixture _connectionFixture;

        public AuthorizationManagerTests(ServerConnectionFixture fixture)
        {
            _connectionFixture = fixture;
        }

        [Fact]
        public async Task CanGetUser()
        {
            // Arrange
            var manager = await CreateAuthManagerAsync();

            UserToCreate targetUser = await CreateUserOnServerAsync();
            User createdTargetUser = await GetUserByLoginAsync(targetUser.Login);

            // Act
            User actualUser = await manager.TryToSignInAsync(targetUser.Login, targetUser.Password);

            // Assert
            Assert.Equal(createdTargetUser.Login, actualUser.Login);
            Assert.Equal(createdTargetUser.AccessToken, actualUser.AccessToken);
            Assert.Equal(createdTargetUser.Group, actualUser.Group);
            Assert.Equal(createdTargetUser.FirstName, actualUser.FirstName);
            Assert.Equal(createdTargetUser.Surname, actualUser.Surname);
            Assert.Equal(createdTargetUser.Patronymic, actualUser.Patronymic);

            // Clean
            await DeleteUserAsync(targetUser.Login);
        }

        [Fact]
        public async Task CantGetUser()
        {
            // Arrange
            var manager = await CreateAuthManagerAsync();

            UserToCreate targetUser = await CreateUserOnServerAsync();

            // Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await manager.TryToSignInAsync("wrongLogin", "wrongPassword")
            );

            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await manager.TryToSignInAsync("wrongLogin", targetUser.Password)
            );

            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await manager.TryToSignInAsync(targetUser.Login, "wrongPassword")
            );

            // Clean
            await DeleteUserAsync(targetUser.Login);
        }

        [Fact]
        public async Task CanRevokeKey()
        {
            // Arrange
            IAuthorizationManager manager = await CreateAuthManagerAsync();

            UserToCreate targetUser = await CreateUserOnServerAsync();
            User createdUser = await GetUserByLoginAsync(targetUser.Login);

            // Act
            await manager.RevokeKey(targetUser.Login, targetUser.Password);

            User actualUser = await GetUserByLoginAsync(targetUser.Login);

            // Assert
            Assert.NotEqual(createdUser.AccessToken, actualUser.AccessToken);
        }

        [Fact]
        public async Task CantRevokeKey()
        {
            // Arrange
            IAuthorizationManager manager = await CreateAuthManagerAsync();

            UserToCreate targetUser = await CreateUserOnServerAsync();

            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => manager.RevokeKey("whatever", null)
            );

            await Assert.ThrowsAsync<ArgumentNullException>(
                () => manager.RevokeKey(null, "whatever")
            );

            await Assert.ThrowsAsync<UnauthorizedException>(
                () => manager.RevokeKey(targetUser.Login, "wrongPassword")
            );

            await Assert.ThrowsAsync<UnauthorizedException>(
                () => manager.RevokeKey("wrongLogin", "wrongPassword")
            );

            await Assert.ThrowsAsync<UnauthorizedException>(
                () => manager.RevokeKey("wrongLogin", targetUser.Password)
            );

            // Clean
            await DeleteUserAsync(targetUser.Login);
        }

		private async Task<IAuthorizationManager> CreateAuthManagerAsync(bool useWrongAccessKey = false)
		{
			User superUser = await _connectionFixture.GetSuperUser();

			IServerDataProvider provider = new JsonProvider(
				useWrongAccessKey ? "wrongKey ": superUser.AccessToken,
				new HostBuilder(
					hostname: _connectionFixture.ServerHostname,
					scheme: "https"
				)
			);

			return new AuthorizationManager(provider);
		}

		private async Task<List<UserGroup>> GetUserGroupsAsync() =>
			await _connectionFixture.GetFromServerAsync<List<UserGroup>>(
				"users/groups"
			);

		private async Task<UserToCreate> CreateUserOnServerAsync()
		{
			List<UserGroup> groups = await GetUserGroupsAsync();

			var newUserInfo = new UserToCreate {
				Login = Guid.NewGuid().ToString(),
				Password = Guid.NewGuid().ToString(),
				FirstName = Guid.NewGuid().ToString(),
				Surname = Guid.NewGuid().ToString(),
				Patronymic = Guid.NewGuid().ToString(),
				GroupID = groups.FirstOrDefault().ID
			};

			await _connectionFixture.SendChangesToServerAsync<UserToCreate>(
				"users/create",
				newUserInfo,
				HttpMethod.Post
			);

            return newUserInfo;
		}

        private async Task<User> GetUserByLoginAsync(string login)
        {
            List<User> users = await _connectionFixture.GetFromServerAsync<List<User>>(
                "users"
            );

            return users.FirstOrDefault(u => u.Login == login);
        }

        private async Task DeleteUserAsync(string login)
        {
            HttpStatusCode code = await _connectionFixture.DeleteFromServerAsync(
                "users/remove",
                new Dictionary<string, string> { { "login", login } }
            );

            if (code != HttpStatusCode.OK)
                throw new InvalidOperationException();
        }
    }
}
