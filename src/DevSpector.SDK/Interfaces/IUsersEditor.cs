using System.Threading.Tasks;
using DevSpector.SDK.DTO;

namespace DevSpector.SDK.Editors
{
    public interface IUsersEditor
    {
        Task CreateUserAsync(UserToCreate userInfo);

        Task DeleteUserAsync(string login);

        Task UpdateUserAsync(string targetLogin, UserToCreate newUserInfo);
    }
}
