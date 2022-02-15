using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using DevSpector.SDK.Models;

namespace DevSpector.SDK
{
    public class DevicesProvider : IDevicesProvider
	{
		private readonly IRawDataProvider _provider;

		public DevicesProvider(IRawDataProvider provider)
		{
			_provider = provider;
		}

		public async Task<IEnumerable<Appliance>> GetDevicesAsync(string accessToken) =>
			JsonSerializer.Deserialize<List<Appliance>>(
				await _provider.GetDevicesAsync(accessToken),
				new JsonSerializerOptions() {
					PropertyNameCaseInsensitive = true
				}
			);
	}
}
