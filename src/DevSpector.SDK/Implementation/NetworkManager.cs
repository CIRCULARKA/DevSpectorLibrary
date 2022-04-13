using System.Collections.Generic;
using System.Threading.Tasks;
using DevSpector.SDK.Models;

namespace DevSpector.SDK.Networking
{
    public class NetworkManager : SdkTool, INetworkManager
    {
        private readonly IServerDataProvider _provider;

        public NetworkManager(IServerDataProvider provider)
        {
            _provider = provider;
        }

        public async Task GenerateIPRangeAsync(string networkAddress, int mask)
        {
            ThrowIfNull(networkAddress);

            ServerResponse response = await _provider.PutAsync(
                "api/ip/generate",
                new LANInfo {
                    NetworkAddress = networkAddress,
                    Mask = mask
                }
            );

            ThrowIfBadResponseStatus(response);
        }

        public async Task<List<string>> GetFreeIPAsync()
        {
            ServerResponse response = await _provider.GetAsync(
                "api/ip/free"
            );

            ThrowIfBadResponseStatus(response);

            return _provider.Deserialize<List<string>>(response.ResponseContent);
        }

        private class LANInfo
        {
            public string NetworkAddress { get; set; }

            public int Mask { get; set; }
        }
    }
}
