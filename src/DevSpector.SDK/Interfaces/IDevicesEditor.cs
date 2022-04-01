using System.Threading.Tasks;
using DevSpector.SDK.DTO;
using DevSpector.SDK.Models;

namespace DevSpector.SDK.Editors
{
	public interface IDevicesEditor
	{
		Task CreateDeviceAsync(DeviceToCreate deviceInfo);

		Task DeleteDeviceAsync(string inventoryNumber);

		Task UpdateDeviceAsync(string targetInventoryNumber, DeviceToCreate deviceInfo);

		Task AssignIPAsync(string inventoryNumber, string ipAddress);

		Task RemoveIPAsync(string inventoryNumber, string ipAddress);

		Task AddSoftwareAsync(string inventoryNumber, Software softwareInfo);

		Task RemoveSoftwareAsync(string inventoryNumber, Software softwareInfo);

		Task MoveAsync(string inventoryNumber, string cabinetID);
	}
}
