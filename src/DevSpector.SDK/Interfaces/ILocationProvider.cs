using System.Threading.Tasks;
using System.Collections.Generic;
using DevSpector.SDK.Models;

namespace DevSpector.SDK.Providers
{
    public interface ILocationProvider
    {
        Task<List<Housing>> GetHousingsAsync();

        Task<List<Cabinet>> GetHousingCabinetsAsync(string housingID);
    }
}
