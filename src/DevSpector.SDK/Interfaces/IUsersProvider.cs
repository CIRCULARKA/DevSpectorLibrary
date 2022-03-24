using System.Threading.Tasks;
using System.Collections.Generic;
using DevSpector.SDK.Models;

namespace DevSpector.SDK
{
	public interface IUsersProvider
	{
		Task<List<User>> GetUsersAsync(string accessToken);

		Task<List<UserGroup>> GetUserGroupsAsync(string accessToken);
	}
}
