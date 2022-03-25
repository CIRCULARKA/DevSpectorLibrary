using System;
using System.Threading.Tasks;

namespace DevSpector.SDK
{
    public class DevicesEditor : IDevicesEditor
    {
        private readonly IRawDataProvider _provider;

        public DevicesEditor(IRawDataProvider provider) =>
            _provider = provider;

		public async Task CreateDevice(string networkName, string inventoryNumber, string type)
		{
            throw new NotImplementedException();
		}
    }
}
