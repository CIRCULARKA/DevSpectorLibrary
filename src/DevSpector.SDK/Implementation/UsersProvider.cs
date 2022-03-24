using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using DevSpector.SDK.Models;

namespace DevSpector.SDK
{
    public class UsersProvider : IUsersProvider
	{
		private readonly IRawDataProvider _provider;

		public UsersProvider(IRawDataProvider provider)
		{
			_provider = provider;
		}

		public async Task<List<User>> GetUsersAsync(string accessToken)
		{
			var response = await _provider.GetDataFromServerAsync("api/users", accessToken);

			if (!response.IsSucceed)
				throw new InvalidOperationException($"Could not get users from server: error {response.ResponseStatusCode}");

			return _provider.Deserialize<List<User>>(response.ResponseContent);
		}

		public async Task<List<UserGroup>> GetUserGroupsAsync(string accessToken)
		{
			var response = await _provider.GetDataFromServerAsync("api/users/groups", accessToken);

			if (!response.IsSucceed)
				throw new InvalidOperationException($"Could not get user groups from server: error {response.ResponseStatusCode}");

			return _provider.Deserialize<List<UserGroup>>(response.ResponseContent);
		}
	}
}
