using System;
using System.Threading.Tasks;
using Xunit;
using DevSpector.SDK.Authorization;
using DevSpector.SDK.Models;

namespace DevSpector.Tests.Common.SDK.Authorization
{
	public class AuthorizationManagerTests
    {
        private readonly string _hostname = "devspector.herokuapp.com";

        [Fact]
        public async Task CanGetUser()
        {
            // Arrange
            var manager = new AuthorizationManager(_hostname);

            var expectedLogin = "noname";

            var password = "no";

            // Act
            await Assert.ThrowsAsync(
                typeof(ArgumentException),
                async () => await manager.TrySignIn(expectedLogin, password)
            );
        }
    }
}
