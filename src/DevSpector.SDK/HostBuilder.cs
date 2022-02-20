using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace DevSpector.SDK
{
    public class HostBuilder : IHostBuilder
    {
        private string _hostname;

        private int _port;

        private string _scheme;

        public HostBuilder(string hostname = "localhost", int port = 443, string scheme = "http")
        {
            _hostname = hostname;
            _port = port;
            _scheme = scheme;

            Host = CreateHost();
        }

        public Uri Host { get; }

        private Uri CreateHost()
        {
            var buidler = new UriBuilder();

            buidler.Host = _hostname;
            buidler.Port = _port;
            buidler.Scheme = _scheme;

            return buidler.Uri;
        }

        public Uri BuildTargetEndpoint(string path, Dictionary<string, string> parameters = null)
        {
            var builder = new UriBuilder();

            builder.Host = Host.Host;
            builder.Port = Host.Port;
            builder.Scheme = Host.Scheme;
            builder.Path = path;

            if (parameters == null)
                return builder.Uri;

            // Build query upon dictionary parameters
            // by using key as parameter name and value as parameter value
            // Also append '&' on the end of each parameter if it is not last
            var query = new StringBuilder();
            for (int i = 0; i < parameters.Count; i++)
            {
                var pair = parameters.ElementAt(i);
                query.Append($"{pair.Key}={pair.Key}");
                if (i < parameters.Count - 1)
                    query.Append("&");
            }

            builder.Query = query.ToString();

            return builder.Uri;
        }
    }
}
