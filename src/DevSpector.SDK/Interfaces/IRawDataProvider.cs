using System.Threading.Tasks;
using System.Collections.Generic;
using DevSpector.SDK.Models;

namespace DevSpector.SDK
{
	public interface IServerDataProvider
	{
        TOut Deserialize<TOut>(string json);

        string Serialize<T>(T obj);

        Task<ServerResponse> GetAsync(string path, Dictionary<string, string> parameters = null);

		Task<ServerResponse> PostAsync<T>(string path, T obj, Dictionary<string, string> parameters = null);
	}
}
