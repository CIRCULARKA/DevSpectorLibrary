using System;
using System.Threading.Tasks;

namespace DevSpector.SDK
{
	public interface IRawDataProvider
	{
        Uri TargetHost { get; }

		Task<string> GetJsonFrom(string path, string accessToken);
	}
}
