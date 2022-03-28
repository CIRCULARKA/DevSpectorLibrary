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

namespace DevSpector.Tests.SDK
{
	[Collection(nameof(FixtureCollection))]
	public class DevicesEditorTests
	{
		private readonly ServerConnectionFixture _connectionFixture;

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
			Assert.Equal(expectedDevice.ModelName, addedDevice.ModelName);
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

			await Assert.ThrowsAsync<ArgumentNullException>(
				() => editor.CreateDevice(null)
			);
		}

		[Fact]
		public async Task CanDeleteDevice()
		{
			// Arrange
			IDevicesEditor editor = await CreateDevicesEditor();

			var targetDevice = await CreateNewDeviceOnServerAsync();

			// Act
			await editor.DeleteDevice(targetDevice.InventoryNumber);

			List<Device> actualDevices = await _connectionFixture.GetFromServerAsync<List<Device>>(
				"devices"
			);

			Device shouldBeNull = actualDevices.FirstOrDefault(d => d.InventoryNumber == targetDevice.InventoryNumber);

			// Assert
			Assert.Null(shouldBeNull);
		}

		[Fact]
		public async Task CantDeleteDevice()
		{
			// Arrange
			IDevicesEditor editor = await CreateDevicesEditor(
				useWrongAccessKey: true
			);

			IDevicesEditor validEditor = await CreateDevicesEditor();

			List<Device> devices = await _connectionFixture.GetFromServerAsync<List<Device>>(
				"devices"
			);

			Device targetDevice = devices.FirstOrDefault();

			// Assert
			await Assert.ThrowsAsync<UnauthorizedException>(
				() => editor.DeleteDevice(targetDevice.InventoryNumber)
			);

			await Assert.ThrowsAsync<InvalidOperationException>(
				() => validEditor.DeleteDevice("wrong inv numv")
			);
		}

		[Fact]
		public async Task CanUpdateDevice()
		{
			// Arrange
			IDevicesEditor editor = await CreateDevicesEditor();

			DeviceToCreate newDevice = await CreateNewDeviceOnServerAsync();

			var expectedDevice = new DeviceToCreate {
				InventoryNumber = Guid.NewGuid().ToString(),
				ModelName = Guid.NewGuid().ToString(),
				NetworkName = Guid.NewGuid().ToString()
			};

			// Act
			await editor.UpdateDevice(newDevice.InventoryNumber, expectedDevice);

			Device actualDevice = await GetDeviceAsync(expectedDevice.InventoryNumber);

			// Assert
			Assert.Equal(expectedDevice.InventoryNumber, actualDevice.InventoryNumber);
			Assert.Equal(expectedDevice.ModelName, actualDevice.ModelName);
			Assert.Equal(expectedDevice.NetworkName, actualDevice.NetworkName);
			Assert.Equal(expectedDevice.TypeID, actualDevice.Type);
		}

		public async Task CantUpdateDevice()
		{
			// Arrange
			IDevicesEditor editorWithInvalidKey = await CreateDevicesEditor(
				useWrongAccessKey: true
			);

			// Act
			await Assert.ThrowsAsync<UnauthorizedException>(
				() => editorWithInvalidKey.UpdateDevice("invnumb", new DeviceToCreate())
			);
		}

		private async Task<IDevicesEditor> CreateDevicesEditor(bool useWrongAccessKey = false)
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

			return new DevicesEditor(provider);
		}

		private async Task<DeviceToCreate> CreateNewDeviceOnServerAsync()
		{
			List<DeviceType> deviceTypes = await _connectionFixture.
				GetFromServerAsync<List<DeviceType>>("devices/types");

			var targetDevice = new DeviceToCreate {
				InventoryNumber = Guid.NewGuid().ToString(),
				ModelName = Guid.NewGuid().ToString(),
				NetworkName = Guid.NewGuid().ToString(),
				TypeID = deviceTypes.FirstOrDefault().ID
			};

			HttpStatusCode response = await _connectionFixture.SendChangesToServerAsync(
				"devices/add",
				targetDevice,
				HttpMethod.Post
			);

			if (response != HttpStatusCode.OK)
				throw new InvalidOperationException($"Can't continue testing: device on server wasn't created ({(int)response})");

			return targetDevice;
		}

		private async Task<Device> GetDeviceAsync(string inventoryNumber)
		{
			List<Device> devices = await _connectionFixture.GetFromServerAsync<List<Device>>(
				"devices"
			);

			return devices.FirstOrDefault(d => d.InventoryNumber == inventoryNumber);
		}
	}
}
