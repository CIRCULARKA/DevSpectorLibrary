using System.Threading.Tasks;
using DevSpector.SDK.DTO;

namespace DevSpector.SDK
{
	public interface IDevicesEditor
	{
		Task CreateDevice(DeviceToCreate deviceInfo);

		Task DeleteDevice(string inventoryNumber);

		Task UpdateDevice(string targetInventoryNumber, DeviceToCreate deviceInfo);

		Task AssignIP(string inventoryNumber, string ipAddress);

		Task RemoveIP(string inventoryNumber, string ipAddress);

		Task AddSoftware(string inventoryNumber, Software softwareInfo);
	}
}
