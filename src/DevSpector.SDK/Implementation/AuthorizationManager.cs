using System;
using System.Text.Json;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using DevSpector.SDK.Models;

namespace DevSpector.SDK.Authorization
{
    public class AuthorizationManager : IAuthorizationManager
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

            if (response.ResponseStatusCode == HttpStatusCode.Unauthorized)
                throw new InvalidOperationException("Could not authorize on server: wrong credentials");
            if (!response.IsSucceed)
                throw new InvalidOperationException($"Could not authorize on server: error {response.ResponseStatusCode}");

            return _provider.Deserialize<User>(response.ResponseContent);
        }
    }
}
