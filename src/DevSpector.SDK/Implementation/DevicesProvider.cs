using System;
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

		public async Task<List<Appliance>> GetDevicesAsync(string accessToken)
		{
			var response = await _provider.GetDataFromServerAsync("api/devices", accessToken);

			if (!response.IsSucceed)
				throw new InvalidOperationException($"Failed to load devices from server: error {response.ResponseStatusCode}");

			return _provider.Deserialize<List<Appliance>>(
				response.ResponseContent
			);
		}

		public async Task<List<DeviceType>> GetDeviceTypesAsync(string accessToken)
		{
			throw new NotImplementedException();
		}
	}
}
