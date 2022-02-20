using System;
using System.Collections.Generic;

namespace DevSpector.SDK
{
    public interface IHostBuilder
    {
        Uri Host { get; }

        Uri BuildTargetEndpoint(string path, Dictionary<string, string> parameters);
    }
}
