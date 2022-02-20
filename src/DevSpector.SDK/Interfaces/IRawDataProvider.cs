using System;
using System.Threading.Tasks;

namespace DevSpector.SDK
{
	public interface IRawDataProvider
	{
		Task<string> GetDevicesAsync(string accessToken);

		Task<string> GetUsersAsync(string accessToken);

		Task<string> GetFreeIPAsync(string accessToken);

		Task<string> GetHousingsAsync(string accessToken);

		Task<string> GetHousingAsync(Guid housingID, string accessToken);
	}
}
