using ALOD.Data.Services;
using Xunit;

namespace ALOD.Tests.Unit.Services
{
    /// <summary>
    /// Tests for DeersService - DEERS integration service for SSN/EDIPIN lookup.
    /// </summary>
    [Trait("Category", "Unit")]
    [Trait("Layer", "Services")]
    public class DeersServiceTests
    {
        [Fact]
        [Trait("Method", "GetSsnByEdipin")]
        public void GetSsnByEdipin_WithValidEdipin_ReturnsTestSSN()
        {
            // Arrange
            var service = new DeersService();
            const string testEdipin = "1234567890";

            // Act
            var result = service.GetSsnByEdipin(testEdipin);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("66666666A", result); // Test stub returns this value
        }

        [Theory]
        [InlineData("0000000000")]
        [InlineData("9999999999")]
        [InlineData("1111111111")]
        [Trait("Method", "GetSsnByEdipin")]
        public void GetSsnByEdipin_WithVariousEdipins_ReturnsTestSSN(string edipin)
        {
            // Arrange
            var service = new DeersService();

            // Act
            var result = service.GetSsnByEdipin(edipin);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("66666666A", result); // Current implementation returns test value
        }

        [Fact]
        [Trait("Method", "GetSsnByEdipin")]
        public void GetSsnByEdipin_WithEmptyString_ReturnsTestSSN()
        {
            // Arrange
            var service = new DeersService();

            // Act
            var result = service.GetSsnByEdipin(string.Empty);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        [Trait("Method", "GetSsnByEdipin")]
        public void GetSsnByEdipin_WithNull_ReturnsTestSSN()
        {
            // Arrange
            var service = new DeersService();

            // Act
            var result = service.GetSsnByEdipin(null);

            // Assert
            Assert.NotNull(result);
        }
    }
}
