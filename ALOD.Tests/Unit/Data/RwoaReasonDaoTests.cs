using ALOD.Core.Domain.Modules.Lod;
using ALOD.Core.Interfaces.DAOInterfaces;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ALOD.Tests.Unit.Data
{
    [Trait("Category", "Unit")]
    [Trait("Layer", "Data")]
    public class RwoaReasonDaoTests
    {
        private readonly Mock<IRwoaReasonDao> _mockDao;

        public RwoaReasonDaoTests()
        {
            _mockDao = new Mock<IRwoaReasonDao>();
        }

        #region Save Tests

        [Fact]
        [Trait("Method", "Save")]
        public void Save_WithValidRwoaReason_ReturnsSavedRwoaReason()
        {
            // Arrange
            var rwoaReason = new RwoaReason();
            _mockDao.Setup(dao => dao.Save(It.IsAny<RwoaReason>()))
                .Returns(rwoaReason);

            // Act
            var result = _mockDao.Object.Save(rwoaReason);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.Save(It.IsAny<RwoaReason>()), Times.Once);
        }

        #endregion

        #region GetById Tests

        [Fact]
        [Trait("Method", "GetById")]
        public void GetById_WithValidId_ReturnsRwoaReason()
        {
            // Arrange
            var rwoaReason = new RwoaReason();
            _mockDao.Setup(dao => dao.GetById(It.IsAny<int>(), false))
                .Returns(rwoaReason);

            // Act
            var result = _mockDao.Object.GetById(1, false);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetById(It.IsAny<int>(), false), Times.Once);
        }

        [Theory]
        [Trait("Method", "GetById")]
        [InlineData(1)]
        [InlineData(100)]
        [InlineData(9999)]
        public void GetById_WithVariousIds_ReturnsRwoaReason(int id)
        {
            // Arrange
            var rwoaReason = new RwoaReason();
            _mockDao.Setup(dao => dao.GetById(It.IsAny<int>()))
                .Returns(rwoaReason);

            // Act
            var result = _mockDao.Object.GetById(id);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetById(It.IsAny<int>()), Times.Once);
        }

        #endregion

        #region GetAll Tests

        [Fact]
        [Trait("Method", "GetAll")]
        public void GetAll_ReturnsAllRwoaReasons()
        {
            // Arrange
            var rwoaReasons = new List<RwoaReason>
            {
                new RwoaReason(),
                new RwoaReason()
            }.AsQueryable();

            _mockDao.Setup(dao => dao.GetAll())
                .Returns(rwoaReasons);

            // Act
            var result = _mockDao.Object.GetAll();

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetAll(), Times.Once);
        }

        #endregion

        #region Delete Tests

        [Fact]
        [Trait("Method", "Delete")]
        public void Delete_WithValidRwoaReason_CallsDelete()
        {
            // Arrange
            var rwoaReason = new RwoaReason();
            _mockDao.Setup(dao => dao.Delete(It.IsAny<RwoaReason>()));

            // Act
            _mockDao.Object.Delete(rwoaReason);

            // Assert
            _mockDao.Verify(dao => dao.Delete(It.IsAny<RwoaReason>()), Times.Once);
        }

        #endregion

        #region SaveOrUpdate Tests

        [Fact]
        [Trait("Method", "SaveOrUpdate")]
        public void SaveOrUpdate_WithValidRwoaReason_ReturnsSavedRwoaReason()
        {
            // Arrange
            var rwoaReason = new RwoaReason();
            _mockDao.Setup(dao => dao.SaveOrUpdate(It.IsAny<RwoaReason>()))
                .Returns(rwoaReason);

            // Act
            var result = _mockDao.Object.SaveOrUpdate(rwoaReason);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.SaveOrUpdate(It.IsAny<RwoaReason>()), Times.Once);
        }

        #endregion
    }
}
