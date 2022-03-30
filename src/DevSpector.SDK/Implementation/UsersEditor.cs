using System;
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

        public async Task CreateUser(UserToCreate userInfo)
        {
            ThrowIfNull(userInfo, userInfo?.Login, userInfo?.Password, userInfo?.GroupID);

            ServerResponse response = await _provider.PostAsync<UserToCreate>(
                "api/users/create",
                userInfo
            );

            ThrowIfBadResponseStatus(response);
        }

        public async Task DeleteUser(string login)
        {
            ThrowIfNull(login);

            ServerResponse response = await _provider.DeleteAsync(
                "api/users/remove",
                new Dictionary<string, string> { { "login", login } }
            );

            ThrowIfBadResponseStatus(response);
        }

        public async Task UpdateUser(string targetLogin, UserToCreate newUserInfo)
        {
            throw new NotImplementedException("Method not tested");
        }
    }
}
