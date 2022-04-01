using System.Threading.Tasks;
using System.Collections.Generic;
using DevSpector.SDK.Models;

namespace DevSpector.SDK.Providers
{
	public interface IUsersProvider
	{
		Task<List<User>> GetUsersAsync();

		Task<List<UserGroup>> GetUserGroupsAsync();
	}
}
