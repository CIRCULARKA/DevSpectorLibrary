using System.Threading.Tasks;
using DevSpector.SDK.Models;

namespace DevSpector.SDK.Authorization
{
	public interface IAuthorizationManager
	{
        Task<User> TryToSignInAsync(string login, string password);
    }
}
