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
        private readonly string _path = "api/users/authorize";

        private readonly HttpClient _client = new HttpClient();

        private readonly IHostBuilder _builder;

        public AuthorizationManager(IHostBuilder builder)
        {
            _builder = builder;
        }

        public async Task<User> TrySignIn(string login, string password)
        {
            var parameters = new Dictionary<string, string>() {
                { nameof(login), login },
                { nameof(password), password }
            };
            var targetEndpoint = _builder.BuildTargetEndpoint(_path, parameters);

            var response = await _client.GetAsync(targetEndpoint);

            if (response.StatusCode != HttpStatusCode.OK)
                throw new ArgumentException("Wrong credentials");

            var result = await JsonSerializer.DeserializeAsync<User>(
                await response.Content.ReadAsStreamAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            return result;
        }
    }
}
