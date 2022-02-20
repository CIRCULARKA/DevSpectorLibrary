using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace DevSpector.SDK
{
    public class DevicesModifier
    {
        private readonly string _path = "api/devices/create";

        private readonly HttpClient _client = new HttpClient();

        private readonly IHostBuilder _builder;

        public DevicesModifier(IHostBuilder builder) =>
            _builder = builder;

		public async Task CreateDevice(string networkName, string inventoryNumber, string type)
		{
            var parameters = new Dictionary<string, string> {
                { nameof(inventoryNumber), inventoryNumber },
                { nameof(networkName), networkName },
                { nameof(type), type }
            };

            var requestUri = _builder.BuildTargetEndpoint(_path, parameters);

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                requestUri
            );

            await _client.SendAsync(request);
		}
    }
}
