using System;
using System.Threading.Tasks;
using DevSpector.SDK.DTO;

namespace DevSpector.SDK
{
    public class DevicesEditor : IDevicesEditor
    {
        private readonly IRawDataProvider _provider;

        public DevicesEditor(IRawDataProvider provider) =>
            _provider = provider;

		public async Task CreateDevice(DeviceToCreate deviceInfo)
		{
            // var response = _provider.PostDataToServerAsync<DeviceToCreate>(
            //     "devices/create",
            //     deviceInfo,

            // );

            throw new NotImplementedException();
		}
    }
}
