using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace DevSpector.SDK
{
    public class JsonProvider : IRawDataProvider
    {
        private readonly Uri _host;

        private Uri _pathToDevices;

        private Uri _pathToUsers;

        private Uri _pathToFreeIpAddresses;

        private Uri _pathToHousings;

        private readonly HttpClient _client;

        private readonly int _localhostPort;

        /// <summary>
        /// Creates provider that targets to localhost
        /// </summary>
        public JsonProvider(int port)
        {
            _localhostPort = port;

            _client = new HttpClient();

            _host = BuildDefaultHost(port);

            BuildEndpointPath();
        }

        public JsonProvider(string hostname, int? port = null)
        {
            _client = new HttpClient();

            _host = BuildHostFrom(hostname, port);

            BuildEndpointPath();
        }

        public JsonProvider(string hostname, HttpClient client, int? port = null)
        {
            _client = client;

            _host = BuildHostFrom(hostname, port);

            BuildEndpointPath();
        }

        public Uri Host => _host;

        public Task<string> GetDevicesAsync(string accessToken) =>
            GetContentFromUriAsync(_pathToDevices.AbsoluteUri, accessToken);

		public Task<string> GetUsersAsync(string acessToken) =>
            GetContentFromUriAsync(_pathToUsers.AbsoluteUri, acessToken);

        public Task<string> GetHousingsAsync(string accessToken) =>
            GetContentFromUriAsync(_pathToHousings.AbsoluteUri, accessToken);

        public Task<string> GetFreeIPAsync(string accessToken) =>
            GetContentFromUriAsync(_pathToFreeIpAddresses.AbsoluteUri, accessToken);

        public Task<string> GetHousingAsync(Guid housingID, string accessToken) =>
            GetContentFromUriAsync(_pathToHousings.AbsoluteUri + housingID, accessToken);

        private async Task<string> GetContentFromUriAsync(string path, string accessToken)
        {
            var request = new HttpRequestMessage {
                RequestUri = new Uri(path),
                Method = HttpMethod.Get
            };

            request.Headers.Add("API", accessToken);

            var response = await _client.SendAsync(request);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                throw new ArgumentException("Wrong API");

            return await response.Content.ReadAsStringAsync();
        }

        private Uri BuildDefaultHost(int port)
        {
            var uriBuilder = CreateHostBuilder(_localhostPort);
            uriBuilder.Host = "localhost";
            return uriBuilder.Uri;
        }

        private Uri BuildHostFrom(string hostname, int? port)
        {
            var uriBuilder = CreateHostBuilder(port, "https");
            uriBuilder.Host = hostname;
            return uriBuilder.Uri;
        }

        private UriBuilder CreateHostBuilder(int? port, string scheme = "http")
        {
            var builder = new UriBuilder();
            if (port != null)
                builder.Port = (int)port;
            builder.Scheme = scheme;
            return builder;
        }

        private Uri BuildUriWithHostBaseAndPath(string path)
        {
            var uriBuilder = new UriBuilder();
            uriBuilder.Scheme = Host.Scheme;
            uriBuilder.Host = Host.Host;
            uriBuilder.Port = Host.Port;
            uriBuilder.Path = path;
            return uriBuilder.Uri;
        }

        private void BuildEndpointPath()
        {
            _pathToDevices = BuildUriWithHostBaseAndPath("api/devices/");
            _pathToUsers = BuildUriWithHostBaseAndPath("api/users/");
            _pathToFreeIpAddresses = BuildUriWithHostBaseAndPath("api/free-ip/");
            _pathToHousings = BuildUriWithHostBaseAndPath("api/location/housings/");
        }
    }
}
