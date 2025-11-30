using ALOD.Core.Domain.Modules.Appeals;
using ALOD.Core.Interfaces.DAOInterfaces;
using Moq;
using Xunit;

namespace ALOD.Tests.Unit.Data
{
    [Trait("Category", "Unit")]
    [Trait("Layer", "Data")]
    public class AppealPostProcessingDaoTests
    {
        private readonly Mock<IAppealPostProcessingDAO> _mockDao;

        public AppealPostProcessingDaoTests()
        {
            _mockDao = new Mock<IAppealPostProcessingDAO>();
        }

        #region GetById Tests

        [Fact]
        [Trait("Method", "GetById")]
        public void GetById_WithValidId_ReturnsAppealPostProcessing()
        {
            // Arrange
            var appealPostProcessing = new AppealPostProcessing();
            _mockDao.Setup(dao => dao.GetById(It.IsAny<int>()))
                .Returns(appealPostProcessing);

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
        public void GetById_WithVariousIds_ReturnsAppealPostProcessing(int id)
        {
            // Arrange
            var appealPostProcessing = new AppealPostProcessing();
            _mockDao.Setup(dao => dao.GetById(It.IsAny<int>()))
                .Returns(appealPostProcessing);

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
        public void InsertOrUpdate_WithValidAppealPostProcessing_ReturnsTrue()
        {
            // Arrange
            var appealPostProcessing = new AppealPostProcessing();
            _mockDao.Setup(dao => dao.InsertOrUpdate(It.IsAny<AppealPostProcessing>()))
                .Returns(true);

            // Act
            var result = _mockDao.Object.InsertOrUpdate(appealPostProcessing);

            // Assert
            Assert.True(result);
            _mockDao.Verify(dao => dao.InsertOrUpdate(It.IsAny<AppealPostProcessing>()), Times.Once);
        }

        #endregion

        #region hasPostProcess Tests

        [Fact]
        [Trait("Method", "hasPostProcess")]
        public void HasPostProcess_WithValidAppealId_ReturnsBoolean()
        {
            // Arrange
            _mockDao.Setup(dao => dao.hasPostProcess(It.IsAny<int>()))
                .Returns(true);

            // Act
            var result = _mockDao.Object.hasPostProcess(1);

            // Assert
            Assert.True(result);
            _mockDao.Verify(dao => dao.hasPostProcess(It.IsAny<int>()), Times.Once);
        }

        [Theory]
        [Trait("Method", "hasPostProcess")]
        [InlineData(1, true)]
        [InlineData(2, false)]
        [InlineData(999, true)]
        public void HasPostProcess_WithVariousIds_ReturnsExpectedResult(int appealId, bool expected)
        {
            // Arrange
            _mockDao.Setup(dao => dao.hasPostProcess(appealId))
                .Returns(expected);

            // Act
            var result = _mockDao.Object.hasPostProcess(appealId);

            // Assert
            Assert.Equal(expected, result);
            _mockDao.Verify(dao => dao.hasPostProcess(appealId), Times.Once);
        }

        #endregion
    }
}
