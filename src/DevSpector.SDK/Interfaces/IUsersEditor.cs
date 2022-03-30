using System.Threading.Tasks;
using DevSpector.SDK.DTO;

namespace DevSpector.SDK.Editors
{
    public interface IUsersEditor
    {
        Task CreateUser(UserToCreate userInfo);
    }
}
