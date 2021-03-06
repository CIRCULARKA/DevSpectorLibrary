using System.Threading.Tasks;
using System.Collections.Generic;
using DevSpector.SDK.Models;

namespace DevSpector.SDK
{
	public interface IServerDataProvider
	{
		string AccessToken { get; }

		void ChangeAccessToken(string newToken);

        TOut Deserialize<TOut>(string json);

        string Serialize<T>(T obj);

        Task<ServerResponse> GetAsync(string path, Dictionary<string, string> parameters = null);

		Task<ServerResponse> PostAsync<T>(string path, T obj, Dictionary<string, string> parameters = null)
			where T: class;

		Task<ServerResponse> PutAsync<T>(string path, T obj, Dictionary<string, string> parameters = null)
			where T: class;

		Task<ServerResponse> DeleteAsync(string path, Dictionary<string, string> parameters = null);
	}
}
