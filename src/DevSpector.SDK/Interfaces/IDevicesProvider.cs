using System.Threading.Tasks;
using System.Collections.Generic;
using DevSpector.SDK.Models;

namespace DevSpector.SDK
{
	public interface IDevicesProvider
	{
		Task<List<Appliance>> GetDevicesAsync(string accessToken);

		Task<List<ApplianceType>> GetApplianceTypesAsync(string accessToken);
	}
}
