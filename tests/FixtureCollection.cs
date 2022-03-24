using Xunit;

namespace DevSpector.Tests
{
    [CollectionDefinition(nameof(FixtureCollection))]
    public class FixtureCollection : ICollectionFixture<ServerConnectionFixture> { }
}
