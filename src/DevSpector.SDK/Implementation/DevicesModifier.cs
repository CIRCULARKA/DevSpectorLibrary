using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DevSpector.SDK
{
    public class DevicesModifier
    {
        private readonly HttpClient _client = new HttpClient();

        private readonly string _targetHost;

        public DevicesModifier() =>
            _targetHost = "localhost";

        public DevicesModifier(string hostname) =>
            _targetHost = hostname;

		public async Task CreateDevice(string networkName, string inventoryNumber, string type)
		{
            var requestUrlBuilder = new UriBuilder();

            if (_targetHost == "localhost")
                requestUrlBuilder.Port = 5000;

            requestUrlBuilder.Scheme = "https";
            requestUrlBuilder.Host = _targetHost;
            requestUrlBuilder.Query = $"networkName={networkName}&inventoryNumber={inventoryNumber}&type={type}";

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                requestUrlBuilder.Uri
            );

            await _client.SendAsync(request);
		}
    }
}
