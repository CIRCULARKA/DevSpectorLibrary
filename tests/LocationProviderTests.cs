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

            // Assert
            await Assert.ThrowsAsync<UnauthorizedException>(
                async () => await badProvider.GetHousingsAsync()
            );
        }

        [Fact]
        public async Task CanGetCabinets()
        {
            // Assert
            ILocationProvider provider = await CreateLocationProviderAsync();

            List<Housing> housings = await GetHousingsAsync();

            var expectedCabinetsLists = new List<List<Cabinet>>();

            foreach (var housing in housings)
                expectedCabinetsLists.Add(
                    await GetHousingCabinetsAsync(housing.HousingID)
                );

            // Act
            var actualCabinetsLists = new List<List<Cabinet>>();

            foreach (var houising in housings)
                actualCabinetsLists.Add(
                    await provider.GetHousingCabinetsAsync(houising.HousingID)
                );

            // Assert
            for (int i = 0; i < expectedCabinetsLists.Count; i++)
            {
                Assert.Equal(expectedCabinetsLists[i].Count, actualCabinetsLists[i].Count);
                for (int j = 0; j < expectedCabinetsLists.Count; j++)
                {
                    Assert.Equal(expectedCabinetsLists[i][j].CabinetID, actualCabinetsLists[i][j].CabinetID);
                    Assert.Equal(expectedCabinetsLists[i][j].CabinetName, actualCabinetsLists[i][j].CabinetName);
                }
            }
        }

        [Fact]
        public async Task CantGetHousingCabinets()
        {
            // Arrange
            ILocationProvider badProvider = await CreateLocationProviderAsync(
                useWrongAccessKey: true
            );

            // Assert
            await Assert.ThrowsAsync<UnauthorizedException>(
                async () => await badProvider.GetHousingCabinetsAsync(null)
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

        private async Task<List<Cabinet>> GetHousingCabinetsAsync(string houisngID) =>
            await _connectionFixture.GetFromServerAsync<List<Cabinet>>(
                "location/housings",
                new Dictionary<string, string> { { "housingID", houisngID } }
            );
    }
}
