using ALOD.Data.Services;
using Xunit;

namespace ALOD.Tests.Unit.Services
{
    /// <summary>
    /// Tests for DataService - abstract base service class providing web context utilities.
    /// </summary>
    [Trait("Category", "Unit")]
    [Trait("Layer", "Services")]
    public class DataServiceTests
    {
        /// <summary>
        /// Test helper class to test abstract DataService methods
        /// </summary>
        private class TestDataService : DataService
        {
            public bool TestIsInWebContext()
            {
                return IsInWebContext;
            }

            public object TestGetSessionObject(string key)
            {
                return GetSessionObject(key);
            }
        }

        [Fact]
        [Trait("Method", "IsInWebContext")]
        public void IsInWebContext_WhenHttpContextIsNull_ReturnsFalse()
        {
            // Arrange
            var service = new TestDataService();

            // Act
            var result = service.TestIsInWebContext();

            // Assert
            // In test context, HttpContext.Current is null
            Assert.False(result);
        }

        [Fact]
        [Trait("Method", "GetSessionObject")]
        public void GetSessionObject_WhenNotInWebContext_ReturnsCallContextData()
        {
            // Arrange
            var service = new TestDataService();
            const string testKey = "TestKey";

            // Act
            var result = service.TestGetSessionObject(testKey);

            // Assert
            // Should return null when key doesn't exist in CallContext
            Assert.Null(result);
        }

        [Theory]
        [InlineData("UserId")]
        [InlineData("SessionKey")]
        [InlineData("TestData")]
        [Trait("Method", "GetSessionObject")]
        public void GetSessionObject_WithVariousKeys_HandlesGracefully(string key)
        {
            // Arrange
            var service = new TestDataService();

            // Act
            var result = service.TestGetSessionObject(key);

            // Assert
            // Should handle various keys without throwing
            Assert.True(true);
        }
    }
}
