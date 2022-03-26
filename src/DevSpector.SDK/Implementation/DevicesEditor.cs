using System;
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
            throw new NotImplementedException();
        }
    }
}
