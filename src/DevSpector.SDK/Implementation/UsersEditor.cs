using System;
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
            throw new NotImplementedException("Not tested yet");
        }
    }
}
