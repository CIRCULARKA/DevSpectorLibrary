using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DevSpector.SDK.DTO;
using DevSpector.SDK.Models;

namespace DevSpector.SDK
{
    public class DevicesEditor : SdkTool, IDevicesEditor
    {
        private readonly IServerDataProvider _provider;

        public DevicesEditor(IServerDataProvider provider) =>
            _provider = provider;

		public async Task CreateDeviceAsync(DeviceToCreate deviceInfo)
		{
            ThrowIfNull(deviceInfo);

            ServerResponse response = await _provider.PostAsync<DeviceToCreate>(
                "api/devices/add",
                deviceInfo
            );

            ThrowIfBadResponseStatus(response);
		}

		public async Task DeleteDeviceAsync(string inventoryNumber)
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

        public async Task UpdateDeviceAsync(string targetInventoryNumber, DeviceToCreate deviceInfo)
        {
            ThrowIfNull(targetInventoryNumber, deviceInfo);

            ServerResponse response = await _provider.PutAsync<DeviceToCreate>(
                "api/devices/update",
                deviceInfo,
                new Dictionary<string, string> { { "targetInventoryNumber", targetInventoryNumber } }
            );

            ThrowIfBadResponseStatus(response);
        }

        public async Task AssignIPAsync(string inventoryNumber, string ipAddress)
        {
            ThrowIfNull(inventoryNumber, ipAddress);

            ServerResponse response = await _provider.PutAsync<string>(
                "api/devices/add-ip",
                ipAddress,
                new Dictionary<string, string> { { "inventoryNumber", inventoryNumber } }
            );

            ThrowIfBadResponseStatus(response);
        }

		public async Task RemoveIPAsync(string inventoryNumber, string ipAddress)
        {
            ThrowIfNull(inventoryNumber, ipAddress);

            ServerResponse response = await _provider.PutAsync<string>(
                "api/devices/remove-ip",
                ipAddress,
                new Dictionary<string, string> { { "inventoryNumber", inventoryNumber } }
            );

            ThrowIfBadResponseStatus(response);
        }

		public async Task AddSoftwareAsync(string inventoryNumber, Software softwareInfo)
        {
            ThrowIfNull(inventoryNumber, softwareInfo);

            if (softwareInfo.SoftwareName == null)
                throw new ArgumentNullException("Software name can't be null");

            ServerResponse response = await _provider.PutAsync<Software>(
                "api/devices/add-software",
                softwareInfo,
                new Dictionary<string, string> { { "inventoryNumber", inventoryNumber } }
            );

            ThrowIfBadResponseStatus(response);
        }

		public async Task RemoveSoftwareAsync(string inventoryNumber, Software softwareInfo)
        {
            ThrowIfNull(inventoryNumber, softwareInfo);

            if (softwareInfo.SoftwareName == null)
                throw new ArgumentNullException("Software name can't be null");

            ServerResponse response = await _provider.PutAsync<Software>(
                "api/devices/remove-software",
                softwareInfo,
                new Dictionary<string, string> { { "inventoryNumber", inventoryNumber } }
            );

            ThrowIfBadResponseStatus(response);
        }

        public async Task MoveAsync(string inventoryNumber, string cabinetID)
        {
            ThrowIfNull(inventoryNumber, cabinetID);

            ServerResponse response = await _provider.PutAsync<string>(
                "api/devices/move",
                cabinetID,
                new Dictionary<string, string> { { "inventoryNumber", inventoryNumber } }
            );

            ThrowIfBadResponseStatus(response);
        }
    }
}
