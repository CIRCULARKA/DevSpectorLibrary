using System.Threading.Tasks;
using System.Collections.Generic;
using DevSpector.SDK.Models;
using DevSpector.SDK.Providers;

namespace DevSpector.SDK
{
    public class DevicesProvider : SdkTool, IDevicesProvider
	{
		private readonly IServerDataProvider _provider;

		public DevicesProvider(IServerDataProvider provider)
		{
			_provider = provider;
		}

		public async Task<List<Device>> GetDevicesAsync()
		{
			var response = await _provider.GetAsync("api/devices");

			ThrowIfBadResponseStatus(response);

			return _provider.Deserialize<List<Device>>(
				response.ResponseContent
			);
		}

		public async Task<List<DeviceType>> GetDeviceTypesAsync()
		{
			var response = await _provider.GetAsync("api/devices/types");

			ThrowIfBadResponseStatus(response);

			return _provider.Deserialize<List<DeviceType>>(
				response.ResponseContent
			);
		}
	}
}
