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
            ThrowIfNull(deviceInfo);

            ServerResponse response = await _provider.PostAsync<DeviceToCreate>(
                "api/devices/add",
                deviceInfo
            );

            ThrowIfBadResponseStatus(response);
		}

		public async Task DeleteDevice(string inventoryNumber)
        {
            ThrowIfNull(inventoryNumber);

            ServerResponse response = await _provider.DeleteAsync(
                "api/devices/remove",
                new Dictionary<string, string> {
                    { "inventoryNumber", inventoryNumber }
                }
            );

            ThrowIfBadResponseStatus(response);
        }

        public async Task UpdateDevice(string targetInventoryNumber, DeviceToCreate deviceInfo)
        {
            ThrowIfNull(targetInventoryNumber, deviceInfo);

            ServerResponse response = await _provider.PutAsync<DeviceToCreate>(
                "api/devices/update",
                deviceInfo,
                new Dictionary<string, string> { { "targetInventoryNumber", targetInventoryNumber } }
            );

            ThrowIfBadResponseStatus(response);
        }

        public Task AssignIP(string inventoryNumber, string ipAddress)
        {
            ThrowIfNull(inventoryNumber, ipAddress);

            throw new NotImplementedException("Method not tested");
        }

        private void ThrowIfNull(params object[] parameters)
        {
            foreach (var param in parameters)
                if (param == null) throw new ArgumentNullException();
        }

        private void ThrowIfBadResponseStatus(ServerResponse response)
        {
            if (response.ResponseStatusCode == HttpStatusCode.Unauthorized)
                throw new UnauthorizedException($"Could not proceed operation: no access");
            if (!response.IsSucceed)
                throw new InvalidOperationException($"Could not proceed operation: {response.ResponseStatusCode} ({(int)response.ResponseStatusCode})");
        }
    }
}
