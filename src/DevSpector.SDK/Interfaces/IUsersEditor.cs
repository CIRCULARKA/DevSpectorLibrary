using System.Threading.Tasks;
using DevSpector.SDK.DTO;

namespace DevSpector.SDK.Editors
{
    public interface IUsersEditor
    {
        Task CreateUser(UserToCreate userInfo);

        Task DeleteUser(string login);

        Task UpdateUser(string targetLogin, UserToCreate newUserInfo);
    }
}
