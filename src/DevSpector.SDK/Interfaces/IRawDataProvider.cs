using System.Threading.Tasks;
using System.Collections.Generic;
using DevSpector.SDK.Models;

namespace DevSpector.SDK
{
	public interface IRawDataProvider
	{
        TOut Deserialize<TOut>(string json);

        string Serialize<T>(T obj);

        Task<ServerResponse> GetDataFromServerAsync(string path, Dictionary<string, string> parameters = null);

		Task<ServerResponse> PostDataToServerAsync<T>(string path, T obj, Dictionary<string, string> parameters = null);
	}
}
