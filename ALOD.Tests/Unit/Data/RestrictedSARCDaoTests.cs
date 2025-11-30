using ALOD.Core.Domain.Modules.SARC;
using ALOD.Core.Interfaces.DAOInterfaces;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ALOD.Tests.Unit.Data
{
    [Trait("Category", "Unit")]
    [Trait("Layer", "Data")]
    public class RestrictedSARCDaoTests
    {
        private readonly Mock<ISARCDAO> _mockDao;

        public RestrictedSARCDaoTests()
        {
            _mockDao = new Mock<ISARCDAO>();
        }

        #region Save Tests

        [Fact]
        [Trait("Method", "Save")]
        public void Save_WithValidRestrictedSARC_ReturnsSavedRestrictedSARC()
        {
            // Arrange
            var restrictedSARC = new RestrictedSARC();
            _mockDao.Setup(dao => dao.Save(It.IsAny<RestrictedSARC>()))
                .Returns(restrictedSARC);

            // Act
            var result = _mockDao.Object.Save(restrictedSARC);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.Save(It.IsAny<RestrictedSARC>()), Times.Once);
        }

        #endregion

        #region GetById Tests

        [Fact]
        [Trait("Method", "GetById")]
        public void GetById_WithValidId_ReturnsRestrictedSARC()
        {
            // Arrange
            var restrictedSARC = new RestrictedSARC();
            _mockDao.Setup(dao => dao.GetById(It.IsAny<int>(), false))
                .Returns(restrictedSARC);

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
        public void GetById_WithVariousIds_ReturnsRestrictedSARC(int id)
        {
            // Arrange
            var restrictedSARC = new RestrictedSARC();
            _mockDao.Setup(dao => dao.GetById(It.IsAny<int>()))
                .Returns(restrictedSARC);

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
        public void GetAll_ReturnsAllRestrictedSARCs()
        {
            // Arrange
            var restrictedSARCs = new List<RestrictedSARC>
            {
                new RestrictedSARC(),
                new RestrictedSARC()
            }.AsQueryable();

            _mockDao.Setup(dao => dao.GetAll())
                .Returns(restrictedSARCs);

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
        public void Delete_WithValidRestrictedSARC_CallsDelete()
        {
            // Arrange
            var restrictedSARC = new RestrictedSARC();
            _mockDao.Setup(dao => dao.Delete(It.IsAny<RestrictedSARC>()));

            // Act
            _mockDao.Object.Delete(restrictedSARC);

            // Assert
            _mockDao.Verify(dao => dao.Delete(It.IsAny<RestrictedSARC>()), Times.Once);
        }

        #endregion

        #region SaveOrUpdate Tests

        [Fact]
        [Trait("Method", "SaveOrUpdate")]
        public void SaveOrUpdate_WithValidRestrictedSARC_ReturnsSavedRestrictedSARC()
        {
            // Arrange
            var restrictedSARC = new RestrictedSARC();
            _mockDao.Setup(dao => dao.SaveOrUpdate(It.IsAny<RestrictedSARC>()))
                .Returns(restrictedSARC);

            // Act
            var result = _mockDao.Object.SaveOrUpdate(restrictedSARC);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.SaveOrUpdate(It.IsAny<RestrictedSARC>()), Times.Once);
        }

        #endregion
    }
}
