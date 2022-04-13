using System.Collections.Generic;
using System.Threading.Tasks;
using DevSpector.SDK.Models;

namespace DevSpector.SDK.Authorization
{
    public class AuthorizationManager : SdkTool, IAuthorizationManager
    {
        private readonly IServerDataProvider _provider;

        public AuthorizationManager(IServerDataProvider provider)
        {
            _provider = provider;
        }

        public async Task<User> TryToSignInAsync(string login, string password)
        {
            var parameters = new Dictionary<string, string>() {
                { "login", login == null ? "_" : login },
                { "password", password == null ? "_" : password }
            };

            ServerResponse response = await _provider.GetAsync(
                path: "api/users/authorize",
                parameters: parameters
            );

            ThrowIfBadResponseStatus(response);

            return _provider.Deserialize<User>(response.ResponseContent);
        }

        public async Task<string> RevokeKeyAsync(string login, string password)
        {
            var parameters = new Dictionary<string, string> {
                { "login", login == null ? "_" : login },
                { "password", password == null ? "_" : password }
            };

            ServerResponse response = await _provider.GetAsync(
                "api/users/revoke-key",
                parameters
            );

            ThrowIfBadResponseStatus(response);

            return response.ResponseContent;
        }

        public async Task ChangePasswordAsync(string login, string currentPassword, string newPassword)
        {
            var parameters = new Dictionary<string, string> {
                { "login", login == null ? "_" : login },
                { "currentPassword", currentPassword == null ? "_" : currentPassword },
                { "newPassword", newPassword == null ? "_" : newPassword }
            };

            ServerResponse response = await _provider.GetAsync(
                "api/users/change-pwd",
                parameters
            );

            ThrowIfBadResponseStatus(response);
        }
    }
}
