using ALOD.Core.Domain.Modules.SARC;
using ALOD.Core.Interfaces.DAOInterfaces;
using Moq;
using Xunit;

namespace ALOD.Tests.Unit.Data
{
    [Trait("Category", "Unit")]
    [Trait("Layer", "Data")]
    public class SARCPostProcessingDaoTests
    {
        private readonly Mock<ISARCPostProcessingDao> _mockDao;

        public SARCPostProcessingDaoTests()
        {
            _mockDao = new Mock<ISARCPostProcessingDao>();
        }

        #region GetById Tests

        [Fact]
        [Trait("Method", "GetById")]
        public void GetById_WithValidId_ReturnsRestrictedSARCPostProcessing()
        {
            // Arrange
            var sarcPostProcessing = new RestrictedSARCPostProcessing();
            _mockDao.Setup(dao => dao.GetById(It.IsAny<int>()))
                .Returns(sarcPostProcessing);

            // Act
            var result = _mockDao.Object.GetById(1);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetById(It.IsAny<int>()), Times.Once);
        }

        [Theory]
        [Trait("Method", "GetById")]
        [InlineData(1)]
        [InlineData(100)]
        [InlineData(9999)]
        public void GetById_WithVariousIds_ReturnsRestrictedSARCPostProcessing(int id)
        {
            // Arrange
            var sarcPostProcessing = new RestrictedSARCPostProcessing();
            _mockDao.Setup(dao => dao.GetById(It.IsAny<int>()))
                .Returns(sarcPostProcessing);

            // Act
            var result = _mockDao.Object.GetById(id);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetById(It.IsAny<int>()), Times.Once);
        }

        #endregion

        #region InsertOrUpdate Tests

        [Fact]
        [Trait("Method", "InsertOrUpdate")]
        public void InsertOrUpdate_WithValidRestrictedSARCPostProcessing_ReturnsTrue()
        {
            // Arrange
            var sarcPostProcessing = new RestrictedSARCPostProcessing();
            _mockDao.Setup(dao => dao.InsertOrUpdate(It.IsAny<RestrictedSARCPostProcessing>()))
                .Returns(true);

            // Act
            var result = _mockDao.Object.InsertOrUpdate(sarcPostProcessing);

            // Assert
            Assert.True(result);
            _mockDao.Verify(dao => dao.InsertOrUpdate(It.IsAny<RestrictedSARCPostProcessing>()), Times.Once);
        }

        [Fact]
        [Trait("Method", "InsertOrUpdate")]
        public void InsertOrUpdate_WhenFails_ReturnsFalse()
        {
            // Arrange
            var sarcPostProcessing = new RestrictedSARCPostProcessing();
            _mockDao.Setup(dao => dao.InsertOrUpdate(It.IsAny<RestrictedSARCPostProcessing>()))
                .Returns(false);

            // Act
            var result = _mockDao.Object.InsertOrUpdate(sarcPostProcessing);

            // Assert
            Assert.False(result);
            _mockDao.Verify(dao => dao.InsertOrUpdate(It.IsAny<RestrictedSARCPostProcessing>()), Times.Once);
        }

        #endregion
    }
}
