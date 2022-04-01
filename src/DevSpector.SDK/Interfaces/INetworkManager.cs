using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevSpector.SDK.Networking
{
	public interface INetworkManager
	{
        Task GenerateIPRangeAsync(string ip, int mask);

        Task<List<string>> GetFreeIPAsync();
    }
}
