using System;
using System.Collections.Generic;
using Xunit;
using DevSpector.SDK;
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
		public void CanAddDevice()
		{

		}
	}
}
