using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xunit;
using DevSpector.SDK;
using DevSpector.SDK.Models;
using DevSpector.SDK.Exceptions;

namespace DevSpector.Tests.Server.SDK
{
	[Collection(nameof(FixtureCollection))]
	public class UsersProviderTests
	{
		private readonly ServerConnectionFixture _connectionFixture;

		private readonly IUsersProvider _usersProvider;

		private readonly IRawDataProvider _rawDataProvider;

		public UsersProviderTests(ServerConnectionFixture conFix)
		{
			_connectionFixture = conFix;

			_rawDataProvider = new JsonProvider(new HostBuilder(_connectionFixture.ServerHostname, scheme: "https"));
			_usersProvider = new UsersProvider(_rawDataProvider);
		}

		[Fact]
		public async Task CanGetUsers()
		{
			// Arrange
			User superUser = await _connectionFixture.GetSuperUser();

			List<User> expectedUsers = await _connectionFixture.GetFromServerAsync<List<User>>(
				"users"
			);

			// Act
			List<User> actualUsers = await _usersProvider.GetUsersAsync(superUser.AccessToken);

			// Assert
			Assert.Equal(expectedUsers.Count, actualUsers.Count);
			for (int i = 0; i < expectedUsers.Count; i++)
			{
				Assert.Equal(expectedUsers[i].Login, actualUsers[i].Login);
				Assert.Equal(expectedUsers[i].AccessToken, actualUsers[i].AccessToken);
				Assert.Equal(expectedUsers[i].Group, actualUsers[i].Group);
			}
		}

		[Fact]
		public async Task CantGetUsers()
		{
			// Assert
			await Assert.ThrowsAsync<UnauthorizedException>(
				async () => await _usersProvider.GetUsersAsync("wrongKey")
			);

			await Assert.ThrowsAsync<UnauthorizedException>(
				async () => await _usersProvider.GetUsersAsync(null)
			);
		}

		[Fact]
		public async Task CanGetUserGroups()
		{
			// Arrange
			User superUser = await _connectionFixture.GetSuperUser();

			List<UserGroup> expected = await _connectionFixture.GetFromServerAsync<List<UserGroup>>(
				"users/groups"
			);

			// Act
			List<UserGroup> actual =  await _usersProvider.GetUserGroupsAsync(superUser.AccessToken);

			// Assert
			Assert.Equal(expected.Count, actual.Count);
			for (int i = 0; i < expected.Count; i++)
			{
				Assert.Equal(expected[i].ID, actual[i].ID);
				Assert.Equal(expected[i].Name, actual[i].Name);
			}
		}

		[Fact]
		public async Task CantGetUserGroups()
		{
			// Assert
			await Assert.ThrowsAsync<UnauthorizedException>(
				async () => await _usersProvider.GetUserGroupsAsync("wrongKey")
			);

			await Assert.ThrowsAsync<UnauthorizedException>(
				async () => await _usersProvider.GetUserGroupsAsync(null)
			);
		}
	}
}
