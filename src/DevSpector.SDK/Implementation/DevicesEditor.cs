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

        public async Task AssignIP(string inventoryNumber, string ipAddress)
        {
            ThrowIfNull(inventoryNumber, ipAddress);

            ServerResponse response = await _provider.PutAsync<string>(
                "api/devices/add-ip",
                ipAddress,
                new Dictionary<string, string> { { "inventoryNumber", inventoryNumber } }
            );

            ThrowIfBadResponseStatus(response);
        }

		public async Task RemoveIP(string inventoryNumber, string ipAddress)
        {
            ThrowIfNull(inventoryNumber, ipAddress);

            ServerResponse response = await _provider.PutAsync<string>(
                "api/devices/remove-ip",
                ipAddress,
                new Dictionary<string, string> { { "inventoryNumber", inventoryNumber } }
            );

            ThrowIfBadResponseStatus(response);
        }

		public async Task AddSoftware(string inventoryNumber, Software softwareInfo)
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

		public async Task RemoveSoftware(string inventoryNumber, Software softwareInfo)
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

        public async Task Move(string inventoryNumber, string cabinetID)
        {
            throw new NotImplementedException("Method not tested yet");

            ThrowIfNull(inventoryNumber, cabinetID);

            ServerResponse response = await _provider.PutAsync<string>(
                "api/devices/remove-software",
                cabinetID,
                new Dictionary<string, string> { { "inventoryNumber", inventoryNumber } }
            );

            ThrowIfBadResponseStatus(response);
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
