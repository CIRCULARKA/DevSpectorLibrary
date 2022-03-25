using System.Threading.Tasks;
using DevSpector.SDK.DTO;

namespace DevSpector.SDK
{
	public interface IDevicesEditor
	{
		Task CreateDevice(DeviceToCreate deviceInfo);
	}
}
