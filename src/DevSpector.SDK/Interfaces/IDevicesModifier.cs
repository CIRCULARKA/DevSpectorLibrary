using System.Threading.Tasks;

namespace DevSpector.SDK
{
	public interface IDevicesModifier
	{
		Task CreateDevice(string networkName, string inventoryNumber, string type);
	}
}
