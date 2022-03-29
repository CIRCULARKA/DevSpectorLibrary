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

			List<DeviceType> deviceTypes = await GetDeviceTypes();

			DeviceType expectedType = deviceTypes.FirstOrDefault();

			var expectedDevice = new DeviceToCreate {
				InventoryNumber = Guid.NewGuid().ToString(),
				NetworkName = Guid.NewGuid().ToString(),
				ModelName = Guid.NewGuid().ToString(),
				TypeID = expectedType.ID
			};

			// Act
			await editor.CreateDevice(expectedDevice);

			List<Device> actualDevices = await GetDevicesAsync();

			Device addedDevice =
				actualDevices.FirstOrDefault(d => d.InventoryNumber == expectedDevice.InventoryNumber);

			// Assert
			Assert.Equal(expectedDevice.InventoryNumber, addedDevice.InventoryNumber);
			Assert.Equal(expectedDevice.NetworkName, addedDevice.NetworkName);
			Assert.Equal(expectedDevice.ModelName, addedDevice.ModelName);
			Assert.Equal(expectedType.Name, addedDevice.Type);

			// Clean
			await DeleteDeviceFromServerAsync(addedDevice.InventoryNumber);
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

			List<Device> actualDevices = await GetDevicesAsync();

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

			List<Device> devices = await GetDevicesAsync();

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

			List<DeviceType> devicesTypes = await GetDeviceTypes();

			DeviceType newDeviceType = devicesTypes.FirstOrDefault(dt => dt.ID == newDevice.TypeID);

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
			Assert.Equal(newDeviceType.Name, actualDevice.Type);

			// Clean
			await DeleteDeviceFromServerAsync(actualDevice.InventoryNumber);
		}

		[Fact]
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

			await Assert.ThrowsAsync<ArgumentNullException>(
				() => editorWithInvalidKey.UpdateDevice(null, new DeviceToCreate())
			);

			await Assert.ThrowsAsync<ArgumentNullException>(
				() => editorWithInvalidKey.UpdateDevice("invnum", null)
			);
		}

		[Fact]
		public async Task CanAssignIP()
		{
			// Arrange
			IDevicesEditor editor = await CreateDevicesEditor();

			DeviceToCreate targetDevice = await CreateNewDeviceOnServerAsync();

			var freeIP = await GetFreeIPAsync();

			// Act
			await editor.AssignIP(targetDevice.InventoryNumber, freeIP);

			Device actualDevice = await GetDeviceAsync(targetDevice.InventoryNumber);

			// Assert
			Assert.Equal(1, actualDevice.IPAddresses.Count);
			Assert.Equal(freeIP, actualDevice.IPAddresses[0]);

			// Clear
			await DeleteDeviceFromServerAsync(targetDevice.InventoryNumber);
		}

		[Fact]
		public async Task CanAddSoftware()
		{
			// Arrange
			IDevicesEditor editor = await CreateDevicesEditor();

			DeviceToCreate targetDevice = await CreateNewDeviceOnServerAsync();

			var expectedSoftware = new Software {
				SoftwareName = Guid.NewGuid().ToString(),
				SoftwareVersion = Guid.NewGuid().ToString()
			};

			// Act
			await editor.AddSoftware(targetDevice.InventoryNumber, expectedSoftware);

			Device actualDevice = await GetDeviceAsync(targetDevice.InventoryNumber);

			// Assert
			Assert.Equal(1, actualDevice.Software.Count);
			Assert.Equal(expectedSoftware.SoftwareName, actualDevice.Software[0].SoftwareName);
			Assert.Equal(expectedSoftware.SoftwareVersion, actualDevice.Software[0].SoftwareVersion);
		}

		[Fact]
		public async Task CantAddSoftware()
		{
			// Arrange
			IDevicesEditor editor = await CreateDevicesEditor(
				useWrongAccessKey: true
			);

			// Act
			await Assert.ThrowsAsync<UnauthorizedException>(
				() => editor.AddSoftware("whatever", new Software { SoftwareName = "whatever" })
			);

			await Assert.ThrowsAsync<ArgumentNullException>(
				() => editor.AddSoftware("whatever", new Software())
			);

			await Assert.ThrowsAsync<ArgumentNullException>(
				() => editor.AddSoftware("whatever", null)
			);

			await Assert.ThrowsAsync<ArgumentNullException>(
				() => editor.AddSoftware(null, new Software { SoftwareName = "whatever" })
			);
		}

		[Fact]
		public async Task CanRemoveSoftware()
		{
			// Arrange
			IDevicesEditor editor = await CreateDevicesEditor();

			DeviceToCreate targetDevice = await CreateNewDeviceOnServerAsync();

			var firstSoft = new Software {
				SoftwareName = "SoftWithManyVersions",
				SoftwareVersion = Guid.NewGuid().ToString()
			};

			var secondSoft = new Software {
				SoftwareName = firstSoft.SoftwareName,
				SoftwareVersion = Guid.NewGuid().ToString()
			};

			var thirdSoft = new Software {
				SoftwareName = "SecondSoftManyVersions",
				SoftwareVersion = Guid.NewGuid().ToString()
			};

			var fourthSoft = new Software {
				SoftwareName = thirdSoft.SoftwareName,
				SoftwareVersion = Guid.NewGuid().ToString()
			};

			await AddSoftwareToDeviceAsync(targetDevice.InventoryNumber, firstSoft);
			await AddSoftwareToDeviceAsync(targetDevice.InventoryNumber, secondSoft);
			await AddSoftwareToDeviceAsync(targetDevice.InventoryNumber, thirdSoft);
			await AddSoftwareToDeviceAsync(targetDevice.InventoryNumber, fourthSoft);

			// Act
			// Should delete software and all its versions
			await editor.RemoveSoftware(targetDevice.InventoryNumber, new Software { SoftwareName = firstSoft.SoftwareName });
			await editor.RemoveSoftware(targetDevice.InventoryNumber, thirdSoft);

			Device actualDevice = await GetDeviceAsync(targetDevice.InventoryNumber);

			// Assert
			Assert.Equal(1, actualDevice.Software.Count);
		}

		[Fact]
		public async Task CantRemoveSoftware()
		{
			// Arrange
			IDevicesEditor editor = await CreateDevicesEditor(
				useWrongAccessKey: true
			);

			// Act
			await Assert.ThrowsAsync<UnauthorizedException>(
				() => editor.RemoveSoftware("whatever", new Software { SoftwareName = "whatever" })
			);

			await Assert.ThrowsAsync<ArgumentNullException>(
				() => editor.RemoveSoftware(null, new Software { SoftwareName = "whatever" })
			);

			await Assert.ThrowsAsync<ArgumentNullException>(
				() => editor.RemoveSoftware("whatever", new Software())
			);
		}

		[Fact]
		public async Task CantAssignIP()
		{
			// Arrange
			IDevicesEditor editor = await CreateDevicesEditor(
				useWrongAccessKey: true
			);

			// Act
			await Assert.ThrowsAsync<UnauthorizedException>(
				() => editor.AssignIP("whatever", "whatever")
			);

			await Assert.ThrowsAsync<ArgumentNullException>(
				() => editor.AssignIP(null, "whatever")
			);

			await Assert.ThrowsAsync<ArgumentNullException>(
				() => editor.AssignIP("whatever", null)
			);
		}

		[Fact]
		public async Task CanRemoveIP()
		{
			// Arrange
			IDevicesEditor editor = await CreateDevicesEditor();

			DeviceToCreate newDevice = await CreateNewDeviceOnServerAsync();

			string targetIP = await GetFreeIPAsync();

			await AddIPToDeviceAsync(newDevice.InventoryNumber, targetIP);

			// Act
			await editor.RemoveIP(newDevice.InventoryNumber, targetIP);

			Device actualDevice = await GetDeviceAsync(newDevice.InventoryNumber);

			// Assert
			Assert.Empty(actualDevice.IPAddresses);

			// Clear
			await DeleteDeviceFromServerAsync(newDevice.InventoryNumber);
		}

		[Fact]
		public async Task CantRemoveIP()
		{
			// Arrange
			IDevicesEditor invalidEditor = await CreateDevicesEditor(
				useWrongAccessKey: true
			);

			// Assert
			await Assert.ThrowsAsync<UnauthorizedException>(
				() => invalidEditor.RemoveIP("whatever", "whatever")
			);

			await Assert.ThrowsAsync<ArgumentNullException>(
				() => invalidEditor.RemoveIP(null, "whatever")
			);

			await Assert.ThrowsAsync<ArgumentNullException>(
				() => invalidEditor.RemoveIP("whatever", null)
			);
		}

		[Fact]
		public async Task CanMoveDevice()
		{
			// Arrange
			IDevicesEditor editor = await CreateDevicesEditor();

			List<Housing> housings = await GetHousingsAsync();

			Housing expectedHousing = housings.Skip(1).FirstOrDefault();

			List<Cabinet> cabinets = await GetHousingCabinetsAsync(
				expectedHousing.HousingID
			);

			DeviceToCreate targetDevice = await CreateNewDeviceOnServerAsync();

			Cabinet expectedCabinet = cabinets.FirstOrDefault();

			// Act
			await editor.Move(
				targetDevice.InventoryNumber,
				expectedCabinet.CabinetID
			);

			Device actualDevice = await GetDeviceAsync(targetDevice.InventoryNumber);

			// Assert
			Assert.Equal(expectedCabinet.CabinetName, actualDevice.Cabinet);
			Assert.Equal(expectedHousing.HousingName, actualDevice.Housing);
		}

		[Fact]
		public async Task CantMoveDevice()
		{
			// Arrange
			IDevicesEditor invalidEditor = await CreateDevicesEditor(
				useWrongAccessKey: true
			);

			// Assert
			await Assert.ThrowsAsync<UnauthorizedException>(
				() => invalidEditor.Move("whatever", "whatever")
			);

			await Assert.ThrowsAsync<ArgumentNullException>(
				() => invalidEditor.Move(null, "whatever")
			);

			await Assert.ThrowsAsync<ArgumentNullException>(
				() => invalidEditor.RemoveIP("whatever", null)
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
			List<DeviceType> deviceTypes = await GetDeviceTypes();

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

		private async Task<List<Device>> GetDevicesAsync() =>
			await _connectionFixture.GetFromServerAsync<List<Device>>("devices");

		private async Task<Device> GetDeviceAsync(string inventoryNumber)
		{
			List<Device> devices = await GetDevicesAsync();

			return devices.FirstOrDefault(d => d.InventoryNumber == inventoryNumber);
		}

		private async Task DeleteDeviceFromServerAsync(string inventoryNumber)
		{
			HttpStatusCode responseCode = await _connectionFixture.DeleteFromServerAsync(
				"devices/remove",
				new Dictionary<string, string>{ { "inventoryNumber", inventoryNumber } }
			);

			if (responseCode != HttpStatusCode.OK)
				throw new InvalidOperationException($"Can't continue testing: device wasn't removed from server ({(int)responseCode})");
		}

		private async Task<List<DeviceType>> GetDeviceTypes() =>
			await _connectionFixture.GetFromServerAsync<List<DeviceType>>("devices/types");

		private async Task<List<string>> GetFreeIPsAsync() =>
			await _connectionFixture.GetFromServerAsync<List<string>>("ip/free");

		private async Task<string> GetFreeIPAsync()
		{
			List<string> freeIPs = await GetFreeIPsAsync();

			return freeIPs.FirstOrDefault();
		}

		private async Task AddIPToDeviceAsync(string inventoryNumber, string ip)
		{
			HttpStatusCode responseCode = await _connectionFixture.SendChangesToServerAsync<string>(
				"devices/add-ip",
				ip,
				HttpMethod.Put,
				new Dictionary<string, string> { { "inventoryNumber", inventoryNumber } }
			);

			if (responseCode != HttpStatusCode.OK)
				throw new InvalidOperationException($"Can't continue testing: ip address wasn't assigned to device ({(int)responseCode})");
		}

		private async Task AddSoftwareToDeviceAsync(string inventoryNumber, Software soft)
		{
			HttpStatusCode code =  await _connectionFixture.SendChangesToServerAsync<Software>(
				"devices/add-software",
				soft,
				HttpMethod.Put,
				new Dictionary<string, string> { { "inventoryNumber", inventoryNumber } }
			);


			if (code != HttpStatusCode.OK)
				throw new InvalidOperationException($"Can't continue testing: software wasn't added to device ({(int)code})");
		}

		private async Task<List<Housing>> GetHousingsAsync() =>
			await _connectionFixture.GetFromServerAsync<List<Housing>>(
				"location/housings"
			);

		private async Task<List<Cabinet>> GetHousingCabinetsAsync(string housingID) =>
			await _connectionFixture.GetFromServerAsync<List<Cabinet>>(
				"location/cabinets",
				new Dictionary<string, string> { { "housingID", housingID } }
			);
	}
}
