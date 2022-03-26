using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;
using DevSpector.SDK.Exceptions;
using DevSpector.SDK.DTO;
using DevSpector.SDK.Models;

namespace DevSpector.SDK
{
    public class DevicesEditor : IDevicesEditor
    {
        private readonly IServerDataProvider _provider;

        public DevicesEditor(IServerDataProvider provider) =>
            _provider = provider;

		public async Task CreateDevice(DeviceToCreate deviceInfo)
		{
            if (deviceInfo == null)
                throw new ArgumentNullException("You must provide information about device to create it");

            ServerResponse response = await _provider.PostAsync<DeviceToCreate>(
                "api/devices/add",
                deviceInfo
            );

            if (response.ResponseStatusCode == HttpStatusCode.Unauthorized)
                throw new UnauthorizedException("Could not create device: no access");
            if (!response.IsSucceed)
                throw new InvalidOperationException($"Could not create device: {response.ResponseStatusCode}");
		}

		public async Task DeleteDevice(string inventoryNumber)
        {
            if (inventoryNumber == null)
                throw new ArgumentNullException("You must provide inventory number to delete device");

            ServerResponse response = await _provider.DeleteAsync(
                "api/devices/remove",
                new Dictionary<string, string> {
                    { "inventoryNumber", inventoryNumber }
                }
            );

            if (response.ResponseStatusCode == HttpStatusCode.Unauthorized)
                throw new UnauthorizedException("Could not delete device: no access");
            if (!response.IsSucceed)
                throw new InvalidOperationException($"Could not delete device: {response.ResponseStatusCode}");
        }
    }
}
