using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using DevSpector.SDK.Models;

namespace DevSpector.SDK.Providers
{
    public class LocationProvider : SdkTool, ILocationProvider
    {
        private readonly IServerDataProvider _provider;

        public LocationProvider(IServerDataProvider provider) =>
            _provider = provider;

        public async Task<List<Housing>> GetHousingsAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<List<Cabinet>> GetHousingCabinetsAsync(string housingID)
        {
            throw new NotImplementedException();
        }
    }
}
