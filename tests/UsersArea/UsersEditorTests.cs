using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Xunit;
using DevSpector.SDK;
using DevSpector.SDK.DTO;
using DevSpector.SDK.Models;
using DevSpector.SDK.Editors;
using DevSpector.SDK.Exceptions;

namespace DevSpector.Tests.Server.SDK
{
	[Collection(nameof(FixtureCollection))]
	public class UsersEditorTests
	{
		private readonly ServerConnectionFixture _connectionFixture;

		public UsersEditorTests(ServerConnectionFixture conFix)
		{
			_connectionFixture = conFix;
		}

		[Fact]
		public async Task CanCreateUser()
		{
			// Arrange
			IUsersEditor editor = await CreateUsersEditorAsync();

			List<UserGroup> userGroups = await GetUserGroupsAsync();

			var expectedUser = new UserToCreate {
				Login = Guid.NewGuid().ToString(),
				Password = Guid.NewGuid().ToString(),
				FirstName = Guid.NewGuid().ToString(),
				Surname = Guid.NewGuid().ToString(),
				Patronymic = Guid.NewGuid().ToString(),
				GroupID = userGroups.FirstOrDefault().ID
			};

			// Act
			await editor.CreateUserAsync(expectedUser);

			User actualUser = await GetUserFromServerAsync(expectedUser.Login);

			// Assert
			Assert.Equal(expectedUser.Login, actualUser.Login);
			Assert.Equal(expectedUser.FirstName, actualUser.FirstName);
			Assert.Equal(expectedUser.Surname, actualUser.Surname);
			Assert.Equal(expectedUser.Patronymic, actualUser.Patronymic);
		}

		[Fact]
		public async Task CantCreateUser()
		{
			// Arrange
			IUsersEditor invalidEditor = await CreateUsersEditorAsync(
				useWrongAccessKey: true
			);

			// Assert
			await Assert.ThrowsAsync<UnauthorizedException>(
				() => invalidEditor.CreateUserAsync(new UserToCreate() {
					Login = "whatever",
					Password = "whatever",
					GroupID = "whatever"
				})
			);

			await Assert.ThrowsAsync<ArgumentNullException>(
				() => invalidEditor.CreateUserAsync(null)
			);

			await Assert.ThrowsAsync<ArgumentNullException>(
				() => invalidEditor.CreateUserAsync(new UserToCreate {
					Login = "whatever",
					Password = "whatever"
				})
			);

			await Assert.ThrowsAsync<ArgumentNullException>(
				() => invalidEditor.CreateUserAsync(new UserToCreate {
					Password = "whatever",
					GroupID = "whatever"
				})
			);

			await Assert.ThrowsAsync<ArgumentNullException>(
				() => invalidEditor.CreateUserAsync(new UserToCreate {
					Login = "whatever",
					GroupID = "whatever"
				})
			);
		}

		[Fact]
		public async Task CanDeleteUser()
		{
			// Arrange
			IUsersEditor editor = await CreateUsersEditorAsync();

			UserToCreate targetUser = await CreateUserOnServerAsync();

			// Act
			await editor.DeleteUserAsync(targetUser.Login);

			User shouldBeNull = await GetUserFromServerAsync(targetUser.Login);

			// Assert
			Assert.Null(shouldBeNull);
		}

		[Fact]
		public async Task CantDeleteUser()
		{
			// Arrange
			IUsersEditor invalidEditor = await CreateUsersEditorAsync(
				useWrongAccessKey: true
			);

			// Assert
			await Assert.ThrowsAsync<UnauthorizedException>(
				() => invalidEditor.DeleteUserAsync("whatever")
			);

			await Assert.ThrowsAsync<ArgumentNullException>(
				() => invalidEditor.DeleteUserAsync(null)
			);
		}

		[Fact]
		public async Task CanUpdateUser()
		{
			// Arrange
			IUsersEditor editor = await CreateUsersEditorAsync();

			UserToCreate targetUser = await CreateUserOnServerAsync();

			var expectedUser = new UserToCreate {
				Login = Guid.NewGuid().ToString(),
				Password = Guid.NewGuid().ToString(),
				FirstName = Guid.NewGuid().ToString(),
				Surname = Guid.NewGuid().ToString(),
				Patronymic = Guid.NewGuid().ToString()
			};

			// Act
			await editor.UpdateUserAsync(targetUser.Login, expectedUser);

			User actualUser = await GetUserFromServerAsync(expectedUser.Login);

			// Assert
			Assert.NotNull(actualUser);
			Assert.Equal(expectedUser.Login, actualUser.Login);
			Assert.Equal(expectedUser.FirstName, actualUser.FirstName);
			Assert.Equal(expectedUser.Surname, actualUser.Surname);
			Assert.Equal(expectedUser.Patronymic, actualUser.Patronymic);
		}

		[Fact]
		public async Task CantUpdateUser()
		{
			// Arrange
			IUsersEditor invalidEditor = await CreateUsersEditorAsync(
				useWrongAccessKey: true
			);

			// Assert
			await Assert.ThrowsAsync<UnauthorizedException>(
				() => invalidEditor.UpdateUserAsync("whatever", new UserToCreate())
			);

			await Assert.ThrowsAsync<ArgumentNullException>(
				() => invalidEditor.UpdateUserAsync(null, new UserToCreate())
			);

			await Assert.ThrowsAsync<ArgumentNullException>(
				() => invalidEditor.UpdateUserAsync("whatever", null)
			);
		}

		private async Task<IUsersEditor> CreateUsersEditorAsync(bool useWrongAccessKey = false)
		{
			User superUser = await _connectionFixture.GetSuperUser();

			IServerDataProvider provider = new JsonProvider(
				useWrongAccessKey ? "wrongKey ": superUser.AccessToken,
				new HostBuilder(
					hostname: _connectionFixture.ServerHostname,
					scheme: "https"
				)
			);

			return new UsersEditor(provider);
		}

		private async Task<UserToCreate> CreateUserOnServerAsync()
		{
			List<UserGroup> groups = await GetUserGroupsAsync();

			var result = new UserToCreate {
				Login = Guid.NewGuid().ToString(),
				Password = Guid.NewGuid().ToString(),
				FirstName = Guid.NewGuid().ToString(),
				Surname = Guid.NewGuid().ToString(),
				Patronymic = Guid.NewGuid().ToString(),
				GroupID = groups.FirstOrDefault().ID
			};

			await _connectionFixture.SendChangesToServerAsync<UserToCreate>(
				"users/create",
				result,
				HttpMethod.Post
			);

			return result;
		}

		private async Task<User> GetUserFromServerAsync(string login)
		{
			List<User> users = await _connectionFixture.GetFromServerAsync<List<User>>(
				"users"
			);

			return users.FirstOrDefault(u => u.Login == login);
		}

		private async Task<List<UserGroup>> GetUserGroupsAsync() =>
			await _connectionFixture.GetFromServerAsync<List<UserGroup>>(
				"users/groups"
			);

		private async Task RemoveUserFromServerAsync(string login)
		{
			HttpStatusCode code = await _connectionFixture.DeleteFromServerAsync(
				"users",
				new Dictionary<string, string> { { "login", login } }
			);

			if (code != HttpStatusCode.OK)
				throw new InvalidOperationException($"Can't proceed with testing: server returned {(int)code}");
		}
	}
}
