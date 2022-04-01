using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevSpector.SDK.Networking
{
	public interface INetworkManager
	{
        Task GenerateIPRange(string ip, int mask);

        Task<List<string>> GetFreeIP();
    }
}
