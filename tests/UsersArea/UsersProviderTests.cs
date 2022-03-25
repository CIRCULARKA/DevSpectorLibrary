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

		public UsersProviderTests(ServerConnectionFixture conFix)
		{
			_connectionFixture = conFix;
		}

		[Fact]
		public async Task CanGetUsers()
		{
			// Arrange
			IUsersProvider provider = await CreateUsersProviderAsync();

			List<User> expectedUsers = await _connectionFixture.GetFromServerAsync<List<User>>(
				"users"
			);

			// Act
			List<User> actualUsers = await provider.GetUsersAsync();

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
			// Arrange
			IUsersProvider provider = await CreateUsersProviderAsync(
				useWrongAccessKey: true
			);

			// Assert
			await Assert.ThrowsAsync<UnauthorizedException>(
				async () => await provider.GetUsersAsync()
			);
		}

		[Fact]
		public async Task CanGetUserGroups()
		{
			// Arrange
			IUsersProvider provider = await CreateUsersProviderAsync();

			List<UserGroup> expected = await _connectionFixture.GetFromServerAsync<List<UserGroup>>(
				"users/groups"
			);

			// Act
			List<UserGroup> actual =  await provider.GetUserGroupsAsync();

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
			// Arrange
			IUsersProvider provider = await CreateUsersProviderAsync(
				useWrongAccessKey: true
			);

			// Assert
			await Assert.ThrowsAsync<UnauthorizedException>(
				async () => await provider.GetUserGroupsAsync()
			);
		}

		private async Task<IUsersProvider> CreateUsersProviderAsync(bool useWrongAccessKey = false)
		{
			User superUser = await _connectionFixture.GetSuperUser();

			IRawDataProvider provider = new JsonProvider(
				useWrongAccessKey ? "wrongKey ": superUser.AccessToken,
				new HostBuilder(
					hostname: _connectionFixture.ServerHostname,
					scheme: "https"
				)
			);

			return new UsersProvider(provider);
		}
	}
}
