using System;
using System.Net;
using System.Net.Http;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xunit;
using DevSpector.SDK;
using DevSpector.SDK.DTO;
using DevSpector.SDK.Models;
using DevSpector.SDK.Exceptions;
using DevSpector.SDK.Providers;

namespace DevSpector.Tests.SDK
{

	[Collection(nameof(FixtureCollection))]
	public class LocationProviderTests
	{
		private readonly ServerConnectionFixture _connectionFixture;

		public LocationProviderTests(ServerConnectionFixture conFix)
		{
			_connectionFixture = conFix;
		}

        [Fact]
        public async Task CanGetHousings()
        {
            // Arrange
            ILocationProvider provider = await CreateLocationProviderAsync();

            List<Housing> expectedHousings = await GetHousingsAsync();

            // Act
            List<Housing> actualHousings = await provider.GetHousingsAsync();

            // Assert
            Assert.Equal(expectedHousings.Count, actualHousings.Count);
            for (int i = 0; i < expectedHousings.Count; i++)
            {
                Assert.Equal(expectedHousings[i].HousingID, actualHousings[i].HousingID);
                Assert.Equal(expectedHousings[i].HousingName, actualHousings[i].HousingName);
            }
        }

        [Fact]
        public async Task CantGetHousings()
        {
            // Arrange
            ILocationProvider badProvider = await CreateLocationProviderAsync(
                useWrongAccessKey: true
            );

            await Assert.ThrowsAsync<UnauthorizedException>(
                async () => await badProvider.GetHousingsAsync()
            );
        }

		private async Task<ILocationProvider> CreateLocationProviderAsync(bool useWrongAccessKey = false)
		{
			User superUser = await _connectionFixture.GetSuperUser();

			IServerDataProvider provider = new JsonProvider(
				useWrongAccessKey ? "wrongKey" : superUser.AccessToken,
				new HostBuilder(
					hostname: _connectionFixture.ServerHostname,
					port: _connectionFixture.ServerPort,
					scheme: "http"
				)
			);

			return new LocationProvider(provider);
		}

        private async Task<List<Housing>> GetHousingsAsync() =>
            await _connectionFixture.GetFromServerAsync<List<Housing>>(
                "location/housings"
            );
    }
}
