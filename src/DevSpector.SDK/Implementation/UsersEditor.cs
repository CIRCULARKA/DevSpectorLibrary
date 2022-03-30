using System;
using System.Threading.Tasks;
using DevSpector.SDK.DTO;

namespace DevSpector.SDK.Editors
{
    public class UsersEditor : IUsersEditor
    {
        private readonly IServerDataProvider _provider;

        public UsersEditor(IServerDataProvider provider)
        {
            _provider = provider;
        }

        public Task CreateUser(UserToCreate userInfo)
        {
            throw new NotImplementedException("Method not tested yet");
        }
    }
}
