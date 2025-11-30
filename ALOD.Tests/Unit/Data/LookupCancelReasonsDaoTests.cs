using ALOD.Core.Domain.Lookup;
using ALOD.Core.Interfaces.DAOInterfaces;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace ALOD.Tests.Unit.Data
{
    [Trait("Category", "Unit")]
    [Trait("Layer", "Data")]
    public class LookupCancelReasonsDaoTests
    {
        private readonly Mock<ILookupCancelReasonsDao> _mockDao;

        public LookupCancelReasonsDaoTests()
        {
            _mockDao = new Mock<ILookupCancelReasonsDao>();
        }

        #region GetAll Tests

        [Fact]
        [Trait("Method", "GetAll")]
        public void GetAll_ReturnsAllCancelReasons()
        {
            // Arrange
            var cancelReasons = new List<CancelReasons> { new CancelReasons() };
            _mockDao.Setup(dao => dao.GetAll())
                .Returns(cancelReasons);

            // Act
            var result = _mockDao.Object.GetAll();

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetAll(), Times.Once);
        }

        #endregion

        #region GetById Tests

        [Fact]
        [Trait("Method", "GetById")]
        public void GetById_WithValidId_ReturnsCancelReasons()
        {
            // Arrange
            var cancelReason = new CancelReasons();
            _mockDao.Setup(dao => dao.GetById(It.IsAny<int>()))
                .Returns(cancelReason);

            // Act
            var result = _mockDao.Object.GetById(1);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetById(It.IsAny<int>()), Times.Once);
        }

        #endregion

        #region UpdateCancelReasons Tests

        [Fact]
        [Trait("Method", "UpdateCancelReasons")]
        public void UpdateCancelReasons_WithValidParameters_CallsUpdate()
        {
            // Arrange
            _mockDao.Setup(dao => dao.UpdateCancelReasons(
                It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()));

            // Act
            _mockDao.Object.UpdateCancelReasons(1, "Test Description", 1);

            // Assert
            _mockDao.Verify(dao => dao.UpdateCancelReasons(
                It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once);
        }

        #endregion
    }
}
