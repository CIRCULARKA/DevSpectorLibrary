using System;
using System.Text.Json;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
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
                { nameof(login), login },
                { nameof(password), password }
            };

            var response = await _provider.GetAsync(
                path: "api/users/authorize",
                parameters: parameters
            );

            ThrowIfBadResponseStatus(response);

            return _provider.Deserialize<User>(response.ResponseContent);
        }

        public Task<string> RevokeKey(string login, string password)
        {
            throw new NotImplementedException("Method not tested yet");
        }
    }
}
