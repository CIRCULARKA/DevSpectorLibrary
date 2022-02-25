using System.Collections.Generic;
using DevSpector.SDK;
using Xunit;

namespace DevSpector.Tests.SDK
{
    public class HostBuilderTests
    {
        private readonly string expectedHost = "dev-devspector.com";
        private readonly int expectedPort = 5001;
        private readonly string expectedScheme = "https";

        private readonly string expectedPath = "/api/devices";

        private readonly string expectedParameter1 = "id=123";

        private readonly string expectedParameter2 = "token=456";

        [Fact]
        public void DoesHostBuildRight()
        {
            // Arrange
            var builder = new HostBuilder(expectedHost, expectedPort, expectedScheme);

            // Act
            var actualEndpoint = builder.BuildTargetEndpoint("api/devices", new Dictionary<string, string> {
                { "id", "123" },
                { "token", "456" }
            });

            // Assert
            Assert.Equal(expectedHost, actualEndpoint.Host);
            Assert.Equal(expectedPort, actualEndpoint.Port);
            Assert.Equal(expectedScheme, actualEndpoint.Scheme);
            Assert.Equal(expectedPath, actualEndpoint.LocalPath);

            Assert.Contains(actualEndpoint.Query, expectedParameter1);
            Assert.Contains(actualEndpoint.Query, expectedParameter2);
        }
    }
}
