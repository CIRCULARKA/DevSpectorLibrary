using System.Threading.Tasks;

namespace DevSpector.SDK
{
	public interface IRawDataProvider
	{
		Task<string> GetJsonFrom(string path, string accessToken);
	}
}
