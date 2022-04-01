using System.Threading.Tasks;
using System.Collections.Generic;
using DevSpector.SDK.Models;
using DevSpector.SDK.Providers;

namespace DevSpector.SDK
{
    public class UsersProvider : SdkTool, IUsersProvider
	{
		private readonly IServerDataProvider _provider;

		public UsersProvider(IServerDataProvider provider)
		{
			_provider = provider;
		}

		public async Task<List<User>> GetUsersAsync()
		{
			var response = await _provider.GetAsync("api/users");

			ThrowIfBadResponseStatus(response);

			return _provider.Deserialize<List<User>>(response.ResponseContent);
		}

		public async Task<List<UserGroup>> GetUserGroupsAsync()
		{
			var response = await _provider.GetAsync("api/users/groups");

			ThrowIfBadResponseStatus(response);

			return _provider.Deserialize<List<UserGroup>>(response.ResponseContent);
		}
	}
}
