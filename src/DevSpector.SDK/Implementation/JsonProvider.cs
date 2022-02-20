using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace DevSpector.SDK
{
    public class JsonProvider : IRawDataProvider
    {
        private Uri _pathToDevices;

        private Uri _pathToUsers;

        private Uri _pathToFreeIpAddresses;

        private Uri _pathToHousings;

        private readonly HttpClient _client;

        private readonly IHostBuilder _builder;

        public JsonProvider(IHostBuilder builder)
        {
            _builder = builder;

            _client = new HttpClient();

            BuildEndpointPath();
        }

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

        private void BuildEndpointPath()
        {
            _pathToDevices = _builder.BuildTargetEndpoint("api/devices");
            _pathToUsers = _builder.BuildTargetEndpoint("api/users");
            _pathToFreeIpAddresses = _builder.BuildTargetEndpoint("api/free-ip/");
            _pathToHousings = _builder.BuildTargetEndpoint("api/location/housings/");
        }
    }
}
