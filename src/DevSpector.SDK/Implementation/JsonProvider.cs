﻿using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace DevSpector.SDK
{
    public class JsonProvider : IRawDataProvider
    {
        private readonly Uri _host;

        private Uri _pathToDevices;

        private Uri _pathToUsers;

        private Uri _pathToFreeIpAddresses;

        private Uri _pathToHousings;

        private readonly HttpClient _client;

        public JsonProvider()
        {
            _client = new HttpClient();

            _host = BuildDefaultHost();

            BuildEndpointPath();
        }

        public JsonProvider(string hostname)
        {
            _client = new HttpClient();

            _host = BuildHostFrom(hostname);

            BuildEndpointPath();
        }

        public JsonProvider(string hostname, HttpClient client)
        {
            _client = client;

            _host = BuildHostFrom(hostname);

            BuildEndpointPath();
        }

        public Uri Host => _host;

        public Task<string> GetDevicesAsync(string accessToken) =>
            GetContentFromUriAsync(_pathToDevices.AbsoluteUri, accessToken);

		public Task<string> GetUsersAsync(string acessToken) =>
            GetContentFromUriAsync(_pathToUsers.AbsoluteUri, acessToken);

        public Task<string> GetHousingsAsync(string accessToken) =>
            GetContentFromUriAsync(_pathToHousings.AbsoluteUri, accessToken);

        public Task<string> GetFreeIPAsync(string accessToken) =>
            GetContentFromUriAsync(_pathToFreeIpAddresses.AbsoluteUri, accessToken);

        public Task<string> GetHousingAsync(Guid housingID, string accessToken) =>
            GetContentFromUriAsync(_pathToHousings.AbsoluteUri + housingID, accessToken);

        private async Task<string> GetContentFromUriAsync(string path, string accessToken)
        {
            var request = new HttpRequestMessage {
                RequestUri = new Uri(path),
                Method = HttpMethod.Get
            };

            request.Headers.Add("API", accessToken);

            var response = await _client.SendAsync(request);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                throw new ArgumentException("Wrong API");

            return await response.Content.ReadAsStringAsync();
        }

        private Uri BuildDefaultHost()
        {
            var uriBuilder = CreateHostBuilder();
            uriBuilder.Host = "localhost";
            return uriBuilder.Uri;
        }

        private Uri BuildHostFrom(string hostname)
        {
            var uriBuilder = CreateHostBuilder();
            uriBuilder.Host = hostname;
            return uriBuilder.Uri;
        }

        private UriBuilder CreateHostBuilder()
        {
            var builder = new UriBuilder();
            builder.Port = 5000;
            builder.Scheme= "http";
            return builder;
        }

        private Uri BuildUriWithHostBaseAndPath(string path)
        {
            var uriBuilder = new UriBuilder();
            uriBuilder.Scheme = Host.Scheme;
            uriBuilder.Host = Host.Host;
            uriBuilder.Port = Host.Port;
            uriBuilder.Path = path;
            return uriBuilder.Uri;
        }

        private void BuildEndpointPath()
        {
            _pathToDevices = BuildUriWithHostBaseAndPath("api/devices/");
            _pathToUsers = BuildUriWithHostBaseAndPath("api/users/");
            _pathToFreeIpAddresses = BuildUriWithHostBaseAndPath("api/free-ip/");
            _pathToHousings = BuildUriWithHostBaseAndPath("api/location/housings/");
        }
    }
}
