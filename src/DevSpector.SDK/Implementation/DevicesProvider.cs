using System;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;
using DevSpector.SDK.Models;
using DevSpector.SDK.Exceptions;

namespace DevSpector.SDK
{
    public class DevicesProvider : IDevicesProvider
	{
		private readonly IRawDataProvider _provider;

		public DevicesProvider(IRawDataProvider provider)
		{
			_provider = provider;
		}

		public async Task<List<Device>> GetDevicesAsync(string accessToken)
		{
			var response = await _provider.GetDataFromServerAsync("api/devices", accessToken);

			if (response.ResponseStatusCode == HttpStatusCode.Unauthorized)
				throw new UnauthorizedException("Failed to load devices from server: no access");
			if (!response.IsSucceed)
				throw new InvalidOperationException($"Failed to load devices from server: error {response.ResponseStatusCode}");

			return _provider.Deserialize<List<Device>>(
				response.ResponseContent
			);
		}

		public async Task<List<DeviceType>> GetDeviceTypesAsync(string accessToken)
		{
			var response = await _provider.GetDataFromServerAsync("api/devices/types", accessToken);

			if (response.ResponseStatusCode == HttpStatusCode.Unauthorized)
				throw new UnauthorizedException("Failed to load device types from server: no access");
			if (!response.IsSucceed)
				throw new InvalidOperationException($"Failed to load device types from server: error {response.ResponseStatusCode}");

			return _provider.Deserialize<List<DeviceType>>(
				response.ResponseContent
			);
		}
	}
}
