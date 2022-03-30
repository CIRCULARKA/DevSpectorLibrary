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
            ServerResponse response = await _provider.GetAsync("api/location/housings");

            ThrowIfBadResponseStatus(response);

            return _provider.Deserialize<List<Housing>>(response.ResponseContent);
        }

        public async Task<List<Cabinet>> GetHousingCabinetsAsync(string housingID)
        {
            ThrowIfNull(housingID);

            ServerResponse response = await _provider.GetAsync(
                "api/location/cabinets",
                new Dictionary<string, string> { { "housingID", housingID } }
            );

            ThrowIfBadResponseStatus(response);

            return _provider.Deserialize<List<Cabinet>>(response.ResponseContent);
        }
    }
}
