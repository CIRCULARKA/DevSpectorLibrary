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
        private readonly string _path = "api/users/authorize";

        private readonly HttpClient _client = new HttpClient();

        public AuthorizationManager(int port)
        {
            Host = CreateHost(port: port);
        }

        public AuthorizationManager(string hostname, int port = 443)
        {
            Host = CreateHost(hostname, port, "https");
        }

        public Uri Host { get; private set; }

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

        private Uri CreateHost(string hostname = "localhost", int port = 443, string scheme = "http")
        {
            var buidler = new UriBuilder();

            buidler.Host = hostname;
            buidler.Port = port;
            buidler.Scheme = scheme;

            return buidler.Uri;
        }

        private Uri BuildTargetEndpoint(string login, string password)
        {
            var builder = new UriBuilder();

            builder.Host = Host.Host;
            builder.Port = Host.Port;
            builder.Path = _path;
            builder.Query = $"login={login}&password={password}";
            builder.Scheme = Host.Scheme;

            return builder.Uri;
        }
    }
}
