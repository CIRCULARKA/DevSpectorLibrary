using System.Collections.Generic;
using System.Threading.Tasks;
using DevSpector.SDK.DTO;
using DevSpector.SDK.Models;

namespace DevSpector.SDK.Editors
{
    public class UsersEditor : SdkTool, IUsersEditor
    {
        private readonly IServerDataProvider _provider;

        public UsersEditor(IServerDataProvider provider)
        {
            _provider = provider;
        }

        public async Task CreateUserAsync(UserToCreate userInfo)
        {
            ThrowIfNull(userInfo, userInfo?.Login, userInfo?.Password, userInfo?.GroupID);

            ServerResponse response = await _provider.PostAsync<UserToCreate>(
                "api/users/create",
                userInfo
            );

            ThrowIfBadResponseStatus(response);
        }

        public async Task DeleteUserAsync(string login)
        {
            ThrowIfNull(login);

            ServerResponse response = await _provider.DeleteAsync(
                "api/users/remove",
                new Dictionary<string, string> { { "login", login } }
            );

            ThrowIfBadResponseStatus(response);
        }

        public async Task UpdateUserAsync(string targetLogin, UserToCreate newUserInfo)
        {
            ThrowIfNull(targetLogin, newUserInfo);

            ServerResponse response = await _provider.PutAsync<UserToCreate>(
                "api/users/update",
                newUserInfo,
                new Dictionary<string, string> { { "targetUserLogin", targetLogin } }
            );

            ThrowIfBadResponseStatus(response);
        }
    }
}
