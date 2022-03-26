using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using DevSpector.SDK;
using DevSpector.SDK.Models;
using DevSpector.SDK.Exceptions;

namespace DevSpector.Tests.SDK
{
	[Collection(nameof(FixtureCollection))]
	public class DevicesProviderTests
	{
		private readonly ServerConnectionFixture _connectionFixture;

		public DevicesProviderTests(ServerConnectionFixture conFix)
		{
			_connectionFixture = conFix;
		}

		[Fact]
		public async void CanGetDevices()
		{
			// Arrange
			IDevicesProvider provider = await CreateDevicesProviderAsync();

			List<Device> expected = await _connectionFixture.GetFromServerAsync<List<Device>>(
				"devices"
			);

			// Act
			var actual = await provider.GetDevicesAsync();

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
			// Arrange
			IDevicesProvider provider = await CreateDevicesProviderAsync(
				useWrongAccessKey: true
			);

			// Assert
			await Assert.ThrowsAsync<UnauthorizedException>(
				async () => await provider.GetDevicesAsync()
			);
		}

		[Fact]
		public async void CanGetDeviceTypes()
		{
			// Arrange
			IDevicesProvider provider = await CreateDevicesProviderAsync();

			List<DeviceType> expected = await _connectionFixture.GetFromServerAsync<List<DeviceType>>(
				"devices/types"
			);

			// Act
			List<DeviceType> actual = await provider.GetDeviceTypesAsync();

			// Assert
			Assert.Equal(expected.Count, actual.Count);
			for (int i = 0; i < expected.Count; i++)
			{
				Assert.Equal(expected[i].ID, actual[i].ID);
				Assert.Equal(expected[i].Name, actual[i].Name);
			}
		}

		[Fact]
		public async void CantGetDeviceTypes()
		{
			// Arrange
			IDevicesProvider provider = await CreateDevicesProviderAsync(
				useWrongAccessKey: true
			);

			// Assert
			await Assert.ThrowsAsync<UnauthorizedException>(
				async () => await provider.GetDeviceTypesAsync()
			);
		}

		private async Task<IDevicesProvider> CreateDevicesProviderAsync(bool useWrongAccessKey = false)
		{
			User superUser = await _connectionFixture.GetSuperUser();

			IServerDataProvider provider = new JsonProvider(
				useWrongAccessKey ? "wrongKey ": superUser.AccessToken,
				new HostBuilder(
					hostname: _connectionFixture.ServerHostname,
					scheme: "https"
				)
			);

			return new DevicesProvider(provider);
		}
	}
}
