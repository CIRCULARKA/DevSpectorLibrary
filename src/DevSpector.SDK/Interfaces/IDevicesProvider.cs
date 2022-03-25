using System.Threading.Tasks;
using System.Collections.Generic;
using DevSpector.SDK.Models;

namespace DevSpector.SDK
{
	public interface IDevicesProvider
	{
		Task<List<Device>> GetDevicesAsync();

		Task<List<DeviceType>> GetDeviceTypesAsync();
	}
}
