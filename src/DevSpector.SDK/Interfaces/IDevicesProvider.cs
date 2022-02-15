using System.Threading.Tasks;
using System.Collections.Generic;
using DevSpector.SDK.Models;

namespace DevSpector.SDK
{
	public interface IDevicesProvider
	{
		Task<IEnumerable<Appliance>> GetDevicesAsync(string accessToken);
	}
}
