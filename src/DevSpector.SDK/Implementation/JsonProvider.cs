﻿using System;
using System.Text.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using DevSpector.SDK.Models;

namespace DevSpector.SDK
{
    public class JsonProvider : IRawDataProvider
    {
        private readonly HttpClient _client;

        private readonly IHostBuilder _builder;

        private readonly JsonSerializerOptions _serializationOptions;

        public JsonProvider(IHostBuilder builder)
        {
            _client = new HttpClient();
            _builder = builder;

            _serializationOptions.PropertyNameCaseInsensitive = true;
        }

        public TOut Deserialize<TOut>(string json) =>
            JsonSerializer.Deserialize<TOut>(json, _serializationOptions);

        public async Task<ServerResponse> GetDataFromServer(string path, string accessToken = null, Dictionary<string, string> parameters = null)
        {
            Uri requestUri = _builder.BuildTargetEndpoint(path, parameters);

            var response = await SendGetRequestAsync(requestUri, accessToken);

            return new ServerResponse(
                response.StatusCode,
                await response.Content.ReadAsStringAsync()
            );
        }

        private async Task<HttpResponseMessage> SendGetRequestAsync(Uri uri, string accessToken = null)
        {
            var request = new HttpRequestMessage {
                RequestUri = uri,
                Method = HttpMethod.Get
            };

            if (accessToken != null)
                request.Headers.Add("API", accessToken);

            return await _client.SendAsync(request);
        }
    }
}
