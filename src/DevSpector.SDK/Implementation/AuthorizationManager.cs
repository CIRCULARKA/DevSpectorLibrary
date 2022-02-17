using System;
using System.Text.Json;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using DevSpector.SDK.Models;

namespace DevSpector.SDK.Authorization
{
    public class AuthorizationManager : IAuthorizationManager
    {
        private readonly string _hostname;

        private readonly string _path = "api/users/authorize";

        private readonly HttpClient _client = new HttpClient();

        public AuthorizationManager() =>
            _hostname = "localhost";

        public AuthorizationManager(string hostname) =>
            _hostname = hostname;

        public async Task<User> TrySignIn(string login, string password)
        {
            var targetEndpoint = BuildTargetEndpoint(login, password);

            var response = await _client.GetAsync(targetEndpoint);

            if (response.StatusCode != HttpStatusCode.OK)
                throw new ArgumentException("Wrong credentials");

            var result = await JsonSerializer.DeserializeAsync<User>(
                await response.Content.ReadAsStreamAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            return result;
        }

        private Uri BuildTargetEndpoint(string login, string password)
        {
            var builder = new UriBuilder();

            if (_hostname == "localhost")
                builder.Port = 5000;

            builder.Host = _hostname;
            builder.Path = _path;
            builder.Query = $"login={login}&password={password}";
            builder.Scheme = "https";

            return builder.Uri;
        }
    }
}
