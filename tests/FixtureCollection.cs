using Xunit;

namespace DevSpector.Tests
{
    [CollectionDefinition(nameof(ServerConnectionFixture))]
    public class FixtureCollection : ICollectionFixture<ServerConnectionFixture> { }
}
