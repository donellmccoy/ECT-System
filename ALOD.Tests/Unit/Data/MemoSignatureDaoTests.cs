using ALOD.Core.Domain.Common;
using ALOD.Core.Interfaces;
using Moq;
using Xunit;

namespace ALOD.Tests.Unit.Data
{
    /// <summary>
    /// Tests for MemoSignatureDao - implements IMemoSignatureDao interface.
    /// Handles memo signature retrieval and insertion operations.
    /// </summary>
    [Trait("Category", "Unit")]
    [Trait("Layer", "Data")]
    public class MemoSignatureDaoTests
    {
        [Fact]
        [Trait("Method", "GetSignature")]
        public void GetSignature_WithValidParameters_ReturnsMemoSignature()
        {
            // Arrange
            var mockDao = new Mock<IMemoSignatureDao>();
            var expectedSignature = new MemoSignature();
            
            mockDao.Setup(x => x.GetSignature(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(expectedSignature);

            // Act
            var result = mockDao.Object.GetSignature(1, 2, 3);

            // Assert
            Assert.NotNull(result);
            mockDao.Verify(x => x.GetSignature(1, 2, 3), Times.Once);
        }

        [Theory]
        [InlineData(1, 1, 1)]
        [InlineData(100, 5, 2)]
        [InlineData(999, 10, 3)]
        [Trait("Method", "GetSignature")]
        public void GetSignature_WithVariousParameters_ReturnsSignature(int refId, int workflow, int ptype)
        {
            // Arrange
            var mockDao = new Mock<IMemoSignatureDao>();
            var expectedSignature = new MemoSignature();
            
            mockDao.Setup(x => x.GetSignature(refId, workflow, ptype))
                .Returns(expectedSignature);

            // Act
            var result = mockDao.Object.GetSignature(refId, workflow, ptype);

            // Assert
            Assert.NotNull(result);
            mockDao.Verify(x => x.GetSignature(refId, workflow, ptype), Times.Once);
        }

        [Fact]
        [Trait("Method", "InsertSignature")]
        public void InsertSignature_WithValidParameters_ExecutesInsert()
        {
            // Arrange
            var mockDao = new Mock<IMemoSignatureDao>();
            mockDao.Setup(x => x.InsertSignature(
                It.IsAny<int>(), 
                It.IsAny<int>(), 
                It.IsAny<string>(), 
                It.IsAny<string>(), 
                It.IsAny<int>(), 
                It.IsAny<int>()));

            // Act
            mockDao.Object.InsertSignature(1, 2, "signature", "2024-01-01", 100, 1);

            // Assert
            mockDao.Verify(x => x.InsertSignature(1, 2, "signature", "2024-01-01", 100, 1), Times.Once);
        }

        [Theory]
        [InlineData(1, 1, "John Doe", "2024-01-15", 10, 1)]
        [InlineData(100, 5, "Jane Smith", "2024-02-20", 20, 2)]
        [InlineData(999, 10, "Test User", "2024-03-25", 30, 3)]
        [Trait("Method", "InsertSignature")]
        public void InsertSignature_WithVariousParameters_InvokesCorrectly(
            int refId, int workflow, string sig, string sigDate, int userId, int ptype)
        {
            // Arrange
            var mockDao = new Mock<IMemoSignatureDao>();
            mockDao.Setup(x => x.InsertSignature(refId, workflow, sig, sigDate, userId, ptype));

            // Act
            mockDao.Object.InsertSignature(refId, workflow, sig, sigDate, userId, ptype);

            // Assert
            mockDao.Verify(x => x.InsertSignature(refId, workflow, sig, sigDate, userId, ptype), Times.Once);
        }

        [Fact]
        [Trait("Method", "InsertSignature")]
        public void InsertSignature_WithEmptySignature_InvokesCorrectly()
        {
            // Arrange
            var mockDao = new Mock<IMemoSignatureDao>();
            mockDao.Setup(x => x.InsertSignature(
                It.IsAny<int>(), 
                It.IsAny<int>(), 
                It.IsAny<string>(), 
                It.IsAny<string>(), 
                It.IsAny<int>(), 
                It.IsAny<int>()));

            // Act
            mockDao.Object.InsertSignature(1, 1, string.Empty, string.Empty, 1, 1);

            // Assert
            mockDao.Verify(x => x.InsertSignature(1, 1, string.Empty, string.Empty, 1, 1), Times.Once);
        }
    }
}
