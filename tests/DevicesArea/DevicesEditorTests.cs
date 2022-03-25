using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xunit;
using DevSpector.SDK;
using DevSpector.SDK.DTO;
using DevSpector.SDK.Models;
using DevSpector.SDK.Exceptions;

namespace DevSpector.Tests.SDK
{
	[Collection(nameof(FixtureCollection))]
	public class DevicesEditorTests
	{
		private readonly ServerConnectionFixture _connectionFixture;

		private readonly IRawDataProvider _rawDataProvider;

		public DevicesEditorTests(ServerConnectionFixture conFix)
		{
			_connectionFixture = conFix;
		}

		[Fact]
		public async Task CanAddDevice()
		{
			// Arrange
			IDevicesEditor editor = await CreateDevicesEditor();

			List<DeviceType> deviceTypes = await _connectionFixture.
				GetFromServerAsync<List<DeviceType>>("devices/types");

			DeviceType expectedType = deviceTypes.FirstOrDefault();

			var expectedDevice = new DeviceToCreate {
				InventoryNumber = Guid.NewGuid().ToString(),
				NetworkName = Guid.NewGuid().ToString(),
				ModelName = Guid.NewGuid().ToString(),
				TypeID = expectedType.ID
			};

			// Act
			await editor.CreateDevice(expectedDevice);

			List<Device> actualDevices = await _connectionFixture.GetFromServerAsync<List<Device>>(
				"devices"
			);

			Device addedDevice =
				actualDevices.FirstOrDefault(d => d.InventoryNumber == expectedDevice.InventoryNumber);

			// Assert
			Assert.Equal(expectedDevice.InventoryNumber, addedDevice.InventoryNumber);
			Assert.Equal(expectedDevice.NetworkName, addedDevice.NetworkName);
			Assert.Equal(expectedDevice.ModelName, addedDevice.Modelname);
			Assert.Equal(expectedType.Name, addedDevice.Type);
		}

		[Fact]
		public async Task CantAddDevice()
		{
			// Arrange
			IDevicesEditor editor = await CreateDevicesEditor(
				useWrongAccessKey: true
			);

			// Assert
			await Assert.ThrowsAsync<UnauthorizedException>(
				() => editor.CreateDevice(new DeviceToCreate {
					InventoryNumber = Guid.NewGuid().ToString()
				})
			);
		}

		private async Task<IDevicesEditor> CreateDevicesEditor(bool useWrongAccessKey = false)
		{
			User superUser = await _connectionFixture.GetSuperUser();

			IRawDataProvider provider = new JsonProvider(
				useWrongAccessKey ? "wrongKey" : superUser.AccessToken,
				new HostBuilder(
					hostname: _connectionFixture.ServerHostname,
					scheme: "https"
				)
			);

			return new DevicesEditor(provider);
		}
	}
}
