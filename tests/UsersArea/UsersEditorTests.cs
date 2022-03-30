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

			var expectedUser = new UserToCreate {
				Login = Guid.NewGuid().ToString(),
				Password = Guid.NewGuid().ToString(),
				FirstName = Guid.NewGuid().ToString(),
				Surname = Guid.NewGuid().ToString(),
				Patronymic = Guid.NewGuid().ToString(),
				GroupID = Guid.NewGuid().ToString()
			};

			// Act
			await editor.CreateUser(expectedUser);

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
				() => invalidEditor.CreateUser(new UserToCreate())
			);

			await Assert.ThrowsAsync<ArgumentNullException>(
				() => invalidEditor.CreateUser(null)
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
			var result = new UserToCreate {
				Login = Guid.NewGuid().ToString(),
				Password = Guid.NewGuid().ToString(),
				FirstName = Guid.NewGuid().ToString(),
				Surname = Guid.NewGuid().ToString(),
				Patronymic = Guid.NewGuid().ToString(),
				GroupID = Guid.NewGuid().ToString()
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
