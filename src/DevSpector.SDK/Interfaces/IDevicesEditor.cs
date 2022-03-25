using System.Threading.Tasks;

namespace DevSpector.SDK
{
	public interface IDevicesEditor
	{
		Task CreateDevice(string networkName, string inventoryNumber, string type);
	}
}
