using System.Threading.Tasks;
using System.Collections.Generic;
using DevSpector.SDK.Models;

namespace DevSpector.SDK
{
	public interface IUsersProvider
	{
		Task<IEnumerable<User>> GetUsersAsync(string accessToken);
	}
}
