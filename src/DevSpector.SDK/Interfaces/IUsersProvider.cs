using System.Threading.Tasks;
using System.Collections.Generic;
using DevSpector.SDK.Models;

namespace DevSpector.SDK
{
	public interface IUsersProvider
	{
		Task<List<User>> GetUsersAsync();

		Task<List<UserGroup>> GetUserGroupsAsync();
	}
}
