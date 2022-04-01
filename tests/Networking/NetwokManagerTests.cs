using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using DevSpector.SDK;
using DevSpector.SDK.Exceptions;
using DevSpector.SDK.DTO;
using DevSpector.SDK.Models;
using DevSpector.SDK.Networking;

namespace DevSpector.Tests.Common.SDK.Networking
{
    [Collection(nameof(FixtureCollection))]
	public class NetworkManagerTests
    {
        private readonly ServerConnectionFixture _connectionFixture;

        public NetworkManagerTests(ServerConnectionFixture fixture)
        {
            _connectionFixture = fixture;
        }

        [Fact]
        public async Task CanGetFreeIP()
        {
            // Arrange
            INetworkManager manager = await CreateNetworkManagerAsync();

            List<string> expected = await GetFreeIP();

            // Act
            List<string> actual = await manager.GetFreeIPAsync();

            // Assert
            Assert.Equal(expected.Count, actual.Count);
            for (int i = 0; i < expected.Count; i++)
                Assert.Equal(expected[i], actual[i]);

        }

        [Fact]
        public async Task CantGetFreeIP()
        {
            // Arrange
            INetworkManager manager = await CreateNetworkManagerAsync(
                useWrongAccessKey: true
            );

            // Assert
            await Assert.ThrowsAsync<UnauthorizedException>(
                () => manager.GetFreeIPAsync()
            );
        }

        [Fact]
        public async Task CantGenerateIPRange()
        {
            // Arrange
            INetworkManager manager = await CreateNetworkManagerAsync(
                useWrongAccessKey: true
            );

            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => manager.GenerateIPRangeAsync(null, 0)
            );

            await Assert.ThrowsAsync<UnauthorizedException>(
                () => manager.GenerateIPRangeAsync("whatever", 0)
            );

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => manager.GenerateIPRangeAsync("198.30.1.1", -1)
            );

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => manager.GenerateIPRangeAsync("wrongNetworkMask", 24)
            );
        }

		private async Task<INetworkManager> CreateNetworkManagerAsync(bool useWrongAccessKey = false)
		{
            User superUser = await _connectionFixture.GetSuperUser();

			IServerDataProvider provider = new JsonProvider(
				new HostBuilder(
					hostname: _connectionFixture.ServerHostname,
					scheme: "https"
				)
			);

			return new NetworkManager(provider);
		}

        private async Task<List<string>> GetFreeIP() =>
            await _connectionFixture.GetFromServerAsync<List<string>>(
                "ip/free"
            );
    }
}
