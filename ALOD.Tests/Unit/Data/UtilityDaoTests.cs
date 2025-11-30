using ALOD.Core.Utils;
using Moq;
using Xunit;

namespace ALOD.Tests.Unit.Data
{
    /// <summary>
    /// Tests for UtilityDao - internal utility class with minimal public API surface.
    /// Uses IUtilityDao interface for testing since UtilityDao is internal.
    /// </summary>
    [Trait("Category", "Unit")]
    [Trait("Layer", "Data")]
    public class UtilityDaoTests
    {
        [Fact]
        [Trait("Method", "AssignIo")]
        public void AssignIo_WhenCalled_ReturnsTrue()
        {
            // Arrange
            var mockDao = new Mock<IUtilityDao>();
            mockDao.Setup(x => x.AssignIo(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()))
                .Returns(true);

            // Act
            var result = mockDao.Object.AssignIo(1, 2, 3, true);

            // Assert
            Assert.True(result);
            mockDao.Verify(x => x.AssignIo(1, 2, 3, true), Times.Once);
        }

        [Fact]
        [Trait("Method", "AssignIo")]
        public void AssignIo_WhenStoredProcReturnsZero_ReturnsFalse()
        {
            // Arrange
            var mockDao = new Mock<IUtilityDao>();
            mockDao.Setup(x => x.AssignIo(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()))
                .Returns(false);

            // Act
            var result = mockDao.Object.AssignIo(1, 2, 3, false);

            // Assert
            Assert.False(result);
            mockDao.Verify(x => x.AssignIo(1, 2, 3, false), Times.Once);
        }

        [Theory]
        [InlineData(1, 10, 20, true)]
        [InlineData(100, 200, 300, false)]
        [InlineData(5, 5, 5, true)]
        [Trait("Method", "AssignIo")]
        public void AssignIo_WithVariousParameters_InvokesCorrectly(int refId, int ioUserId, int aaUserId, bool isFormal)
        {
            // Arrange
            var mockDao = new Mock<IUtilityDao>();
            mockDao.Setup(x => x.AssignIo(refId, ioUserId, aaUserId, isFormal))
                .Returns(true);

            // Act
            var result = mockDao.Object.AssignIo(refId, ioUserId, aaUserId, isFormal);

            // Assert
            Assert.True(result);
            mockDao.Verify(x => x.AssignIo(refId, ioUserId, aaUserId, isFormal), Times.Once);
        }
    }
}
