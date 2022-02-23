using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Moq;
using Xunit;
using DevSpector.SDK;
using DevSpector.SDK.Models;

namespace DevSpector.Tests.Server.SDK
{
	public class UsersProviderTests
	{
		private readonly IRawDataProvider _mockDataProvider;

		private readonly Guid[] _mockUsersGuids;

		public UsersProviderTests()
		{
			_mockUsersGuids = new Guid[] {
				new Guid("16036105-5111-4420-8b26-a18deaeb8f9b"),
				new Guid("ed8c1437-07fd-4ce8-beb2-aba831d05e31"),
			};

			var moq = new Mock<IRawDataProvider>();
			moq.Setup(provider => provider.GetUsersAsync(Guid.Empty.ToString())).
				Returns(
					Task.FromResult<string>(
						@"[
							{
								""accessToken"": ""16036105-5111-4420-8b26-a18deaeb8f9b"",
								""login"": ""login1"",
								""group"": ""group1""
							},
							{
								""accessToken"": ""ed8c1437-07fd-4ce8-beb2-aba831d05e31"",
								""login"": ""login2"",
								""group"": ""group2""
							}
						]"
					)
				);

			_mockDataProvider = moq.Object;
		}

		[Fact]
		public async void AreDevicesDeserializedProperly()
		{
			// Arrange
			var provider = new UsersProvider(_mockDataProvider);

			var expected = new List<User>
			{
				new User(_mockUsersGuids[0].ToString(), "login1", "group1"),
				new User(_mockUsersGuids[1].ToString(), "login2", "group2")
			};

			// Act
			var actual = (await provider.GetUsersAsync(Guid.Empty.ToString())).ToList();

			// Assert
			Assert.Equal(expected.Count(), actual.Count());

			for (int i = 0; i < expected.Count(); i++)
			{
				Assert.Equal(expected[i].Login, actual[i].Login);
				Assert.Equal(expected[i].AccessToken, actual[i].AccessToken);
				Assert.Equal(expected[i].Group, actual[i].Group);
			}
		}
	}
}
