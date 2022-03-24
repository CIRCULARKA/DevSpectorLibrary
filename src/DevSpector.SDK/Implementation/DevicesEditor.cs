using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace DevSpector.SDK
{
    public class DevicesEditor
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
