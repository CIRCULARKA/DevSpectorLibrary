using System.Threading.Tasks;
using System.Collections.Generic;
using DevSpector.SDK.Models;

namespace DevSpector.SDK.Providers
{
	public interface IDevicesProvider
	{
		Task<List<Device>> GetDevicesAsync();

		Task<List<DeviceType>> GetDeviceTypesAsync();
	}
}
