using System.Threading.Tasks;
using System.Collections.Generic;
using DevSpector.SDK.Models;

namespace DevSpector.SDK
{
	public interface IRawDataProvider
	{
        TOut Deserialize<TOut>(string json);
        Task<ServerResponse> GetDataFromServer(string path, string accessToken = null, Dictionary<string, string> parameters = null);
	}
}
