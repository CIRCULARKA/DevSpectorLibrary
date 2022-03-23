using System;
using System.Collections.Generic;
using Xunit;
using DevSpector.SDK;
using DevSpector.SDK.Models;

namespace DevSpector.Tests.SDK
{
	[Collection(nameof(FixtureCollection))]
	public class DevicesProviderTests
	{
		private readonly ServerConnectionFixture _connectionFixture;

		private readonly IRawDataProvider _rawDataProvider;

		private readonly IDevicesProvider _devicesProvider;

		public DevicesProviderTests(ServerConnectionFixture conFix)
		{
			_connectionFixture = conFix;

			_rawDataProvider = new JsonProvider(new HostBuilder("dev-devspector.herokuapp.com", scheme: "https"));

			_devicesProvider = new DevicesProvider(_rawDataProvider);
		}

		[Fact]
		public async void CanGetDevices()
		{
			// Arrange
			User user = await _connectionFixture.GetAuthorizedUser();

			List<Appliance> expected = await _connectionFixture.GetFromServerAsync<List<Appliance>>(
				$"https://dev-devspector.herokuapp.com/api/devices?api={user.AccessToken}"
			);

			// Act
			var actual = await _devicesProvider.GetDevicesAsync(user.AccessToken);

			// Assert
			Assert.Equal(expected.Count, actual.Count);

			for (int i = 0; i < expected.Count; i++)
			{
				Assert.Equal(expected[i].ID, actual[i].ID);
				Assert.Equal(expected[i].InventoryNumber, actual[i].InventoryNumber);
				Assert.Equal(expected[i].NetworkName, actual[i].NetworkName);
				Assert.Equal(expected[i].Housing, actual[i].Housing);
				Assert.Equal(expected[i].Cabinet, actual[i].Cabinet);

				var expectedIPList = expected[i].IPAddresses;
				var actualIPList = actual[i].IPAddresses;
				Assert.Equal(expectedIPList.Count, actualIPList.Count);
				for (int j = 0; j < expectedIPList.Count; j++)
					Assert.Equal(expectedIPList[j], actualIPList[j]);

				var expectedSoftware = expected[i].Software;
				var actualSoftware = actual[i].Software;
				Assert.Equal(expectedSoftware.Count, actualSoftware.Count);
				for (int j = 0; j < expectedSoftware.Count; j++)
					Assert.Equal(expectedSoftware[j], actualSoftware[j]);
			}
		}

		[Fact]
		public async void CantGetDevices()
		{
			// Assert
			await Assert.ThrowsAsync<InvalidOperationException>(
				async () => await _devicesProvider.GetDevicesAsync("wrongAPI")
			);
		}
	}
}
