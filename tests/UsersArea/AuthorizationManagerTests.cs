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
                await manager.TryToSignInAsync(null, "whatever")
            );

            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await manager.TryToSignInAsync("whatever", null)
            );

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
        public async Task CanRevokeKeyAsync()
        {
            // Arrange
            IAuthorizationManager manager = await CreateAuthManagerAsync();

            UserToCreate targetUser = await CreateUserOnServerAsync();
            User createdUser = await GetUserByLoginAsync(targetUser.Login);

            // Act
            string newKey = await manager.RevokeKeyAsync(targetUser.Login, targetUser.Password);

            User actualUser = await GetUserByLoginAsync(targetUser.Login);

            // Assert
            Assert.Equal(newKey, actualUser.AccessToken);

            // Clean
            await DeleteUserAsync(targetUser.Login);
        }

        [Fact]
        public async Task CantRevokeKeyAsync()
        {
            // Arrange
            IAuthorizationManager manager = await CreateAuthManagerAsync();

            UserToCreate targetUser = await CreateUserOnServerAsync();

            // Assert
            await Assert.ThrowsAsync<UnauthorizedException>(
                () => manager.RevokeKeyAsync("whatever", null)
            );

            await Assert.ThrowsAsync<UnauthorizedException>(
                () => manager.RevokeKeyAsync(null, "whatever")
            );

            await Assert.ThrowsAsync<UnauthorizedException>(
                () => manager.RevokeKeyAsync(targetUser.Login, "wrongPassword")
            );

            await Assert.ThrowsAsync<UnauthorizedException>(
                () => manager.RevokeKeyAsync("wrongLogin", "wrongPassword")
            );

            await Assert.ThrowsAsync<UnauthorizedException>(
                () => manager.RevokeKeyAsync("wrongLogin", targetUser.Password)
            );

            // Clean
            await DeleteUserAsync(targetUser.Login);
        }

        [Fact]
        public async Task CanChangePassword()
        {
            // Arrange
            IAuthorizationManager manager = await CreateAuthManagerAsync();

            UserToCreate targetUser = await CreateUserOnServerAsync();

            // Act
            await manager.ChangePasswordAsync(
                targetUser.Login,
                targetUser.Password,
                Guid.NewGuid().ToString()
            );

            // Assert
            Assert.False(await CanAuthorize(targetUser, targetUser.Password));

            // Clean
            await DeleteUserAsync(targetUser.Login);
        }

        [Fact]
        public async Task CantChangePassword()
        {
            // Arrange
            IAuthorizationManager validManager = await CreateAuthManagerAsync();
            IAuthorizationManager invalidManager = await CreateAuthManagerAsync(
                useWrongAccessKey: true
            );

            UserToCreate targetUser = await CreateUserOnServerAsync();

            // Assert
            await Assert.ThrowsAsync<UnauthorizedException>(
                async () => await invalidManager.ChangePasswordAsync("whatever", "whatever", "whatever")
            );

            await Assert.ThrowsAsync<UnauthorizedException>(
                async () => await invalidManager.ChangePasswordAsync(targetUser.Login, targetUser.Password, Guid.NewGuid().ToString())
            );

            await Assert.ThrowsAsync<UnauthorizedException>(
                async () => await validManager.ChangePasswordAsync(targetUser.Login, "wrongPwd", Guid.NewGuid().ToString())
            );

            await Assert.ThrowsAsync<UnauthorizedException>(
                async () => await validManager.ChangePasswordAsync("wrongLogin", targetUser.Password, Guid.NewGuid().ToString())
            );

            await Assert.ThrowsAsync<UnauthorizedException>(
                async () => await validManager.ChangePasswordAsync(null, targetUser.Password, Guid.NewGuid().ToString())
            );

            await Assert.ThrowsAsync<UnauthorizedException>(
                async () => await validManager.ChangePasswordAsync(targetUser.Login, null, Guid.NewGuid().ToString())
            );
        }

		private async Task<IAuthorizationManager> CreateAuthManagerAsync(bool useWrongAccessKey = false)
		{
            User superUser = await _connectionFixture.GetSuperUser();

			IServerDataProvider provider = new JsonProvider(
                useWrongAccessKey ? "wrongKey" : superUser.AccessToken,
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

        private async Task<bool> CanAuthorize(UserToCreate user, string password)
        {
            string result = await _connectionFixture.GetFromServerAsync(
                "users/authorize",
                new Dictionary<string, string> { { "login", user.Login }, { "password", password } }
            );

            // Ultimate error detection 3000!
            return !result.Contains("error");
        }
    }
}
