using System.Collections.Generic;
using DevSpector.SDK.DTO;
using Xunit;

namespace DevSpector.Tests.SDK
{
    public class ServerErrorTests
    {
        [Fact]
        public void ConvertsErrorsFromListToString()
        {
            // Arrange
            var description1 = new List<string> {
                "first error",
                "second error",
                "lastError"
            };

            var description2 = new List<string> {
                "single error"
            };

            var description3 = new List<string>();

            var dto1 = new ServerError(
                error: "Oops! Error has occured!!",
                description: description1
            );

            var dto2 = new ServerError(
                error: "Oops! Error has occured!!",
                description: description2
            );

            var dto3 = new ServerError(
                error: "Oops! Error has occured!!",
                description: description3
            );

            var dto4 = new ServerError(
                error: "Oops! Error has occured!!",
                description: null
            );

            var expected1 = "first error, second error, lastError";
            var expected2 = "single error";
            var expected3 = string.Empty;
            var expected4 = string.Empty;

            // Act
            string actual1 = dto1.GetCommaSeparatedDescription();
            string actual2 = dto2.GetCommaSeparatedDescription();
            string actual3 = dto3.GetCommaSeparatedDescription();
            string actual4 = dto4.GetCommaSeparatedDescription();

            // Assert
            Assert.Equal(expected1, actual1);
            Assert.Equal(expected2, actual2);
            Assert.Equal(expected3, actual3);
            Assert.Equal(expected4, actual4);
        }
    }
}
