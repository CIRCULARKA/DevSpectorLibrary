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
		private readonly IServerDataProvider _provider;

		public DevicesProvider(IServerDataProvider provider)
		{
			_provider = provider;
		}

		public async Task<List<Device>> GetDevicesAsync()
		{
			var response = await _provider.GetAsync("api/devices");

			if (response.ResponseStatusCode == HttpStatusCode.Unauthorized)
				throw new UnauthorizedException("Failed to load devices from server: no access");
			if (!response.IsSucceed)
				throw new InvalidOperationException($"Failed to load devices from server: error {response.ResponseStatusCode}");

			return _provider.Deserialize<List<Device>>(
				response.ResponseContent
			);
		}

		public async Task<List<DeviceType>> GetDeviceTypesAsync()
		{
			var response = await _provider.GetAsync("api/devices/types");

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
