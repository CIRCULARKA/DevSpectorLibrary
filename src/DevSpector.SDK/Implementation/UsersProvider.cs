using System;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;
using DevSpector.SDK.Models;
using DevSpector.SDK.Exceptions;

namespace DevSpector.SDK
{
    public class UsersProvider : IUsersProvider
	{
		private readonly IServerDataProvider _provider;

		public UsersProvider(IServerDataProvider provider)
		{
			_provider = provider;
		}

		public async Task<List<User>> GetUsersAsync()
		{
			var response = await _provider.GetAsync("api/users");

			if (response.ResponseStatusCode == HttpStatusCode.Unauthorized)
				throw new UnauthorizedException("Could not get users from server: no access");
			if (!response.IsSucceed)
				throw new InvalidOperationException($"Could not get users from server: error {response.ResponseStatusCode}");

			return _provider.Deserialize<List<User>>(response.ResponseContent);
		}

		public async Task<List<UserGroup>> GetUserGroupsAsync()
		{
			var response = await _provider.GetAsync("api/users/groups");

			if (response.ResponseStatusCode == HttpStatusCode.Unauthorized)
				throw new UnauthorizedException("Could not get user groups from server: no access");
			if (!response.IsSucceed)
				throw new InvalidOperationException($"Could not get user groups from server: error {response.ResponseStatusCode}");

			return _provider.Deserialize<List<UserGroup>>(response.ResponseContent);
		}
	}
}
