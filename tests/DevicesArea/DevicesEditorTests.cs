using System;
using System.Linq;
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

		private readonly IDevicesEditor _editor;

		public DevicesEditorTests(ServerConnectionFixture conFix)
		{
			_connectionFixture = conFix;

			_rawDataProvider = new JsonProvider(new HostBuilder("dev-devspector.herokuapp.com", scheme: "https"));

			_editor = new DevicesEditor(_rawDataProvider);
		}

		[Fact]
		public async void CanAddDevice()
		{
			// Arrange
			List<DeviceType> deviceTypes = await _connectionFixture.
				GetFromServerAsync<List<DeviceType>>("devices/types");

			var expectedDevice = new DeviceToCreate {
				InventoryNumber = Guid.NewGuid().ToString(),
				NetworkName = Guid.NewGuid().ToString(),
				ModelName = Guid.NewGuid().ToString(),
				TypeID = deviceTypes.FirstOrDefault().ID
			};
		}
	}
}
