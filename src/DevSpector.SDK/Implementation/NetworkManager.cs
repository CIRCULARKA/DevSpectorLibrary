using System;
using System.Text.Json;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
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

        public Task GenerateIPRangeAsync(string networkAddress, int mask)
        {
            throw new NotImplementedException("Method not tested yet");
        }

        public Task<List<string>> GetFreeIPAsync()
        {
            throw new NotImplementedException("Method not tested yet");
        }
    }
}
