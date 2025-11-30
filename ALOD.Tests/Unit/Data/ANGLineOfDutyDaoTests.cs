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
    public class ANGLineOfDutyDaoTests
    {
        private readonly Mock<ANGILineOfDutyDao> _mockDao;

        public ANGLineOfDutyDaoTests()
        {
            _mockDao = new Mock<ANGILineOfDutyDao>();
        }

        #region Save Tests

        [Fact]
        [Trait("Method", "Save")]
        public void Save_WithValidANGLineOfDuty_ReturnsSavedANGLineOfDuty()
        {
            // Arrange
            var angLineOfDuty = new ANGLineOfDuty();
            _mockDao.Setup(dao => dao.Save(It.IsAny<ANGLineOfDuty>()))
                .Returns(angLineOfDuty);

            // Act
            var result = _mockDao.Object.Save(angLineOfDuty);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.Save(It.IsAny<ANGLineOfDuty>()), Times.Once);
        }

        #endregion

        #region GetById Tests

        [Fact]
        [Trait("Method", "GetById")]
        public void GetById_WithValidId_ReturnsANGLineOfDuty()
        {
            // Arrange
            var angLineOfDuty = new ANGLineOfDuty();
            _mockDao.Setup(dao => dao.GetById(It.IsAny<int>(), false))
                .Returns(angLineOfDuty);

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
        public void GetById_WithVariousIds_ReturnsANGLineOfDuty(int id)
        {
            // Arrange
            var angLineOfDuty = new ANGLineOfDuty();
            _mockDao.Setup(dao => dao.GetById(It.IsAny<int>()))
                .Returns(angLineOfDuty);

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
        public void GetAll_ReturnsAllANGLineOfDuties()
        {
            // Arrange
            var angLineOfDuties = new List<ANGLineOfDuty>
            {
                new ANGLineOfDuty(),
                new ANGLineOfDuty()
            }.AsQueryable();

            _mockDao.Setup(dao => dao.GetAll())
                .Returns(angLineOfDuties);

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
        public void Delete_WithValidANGLineOfDuty_CallsDelete()
        {
            // Arrange
            var angLineOfDuty = new ANGLineOfDuty();
            _mockDao.Setup(dao => dao.Delete(It.IsAny<ANGLineOfDuty>()));

            // Act
            _mockDao.Object.Delete(angLineOfDuty);

            // Assert
            _mockDao.Verify(dao => dao.Delete(It.IsAny<ANGLineOfDuty>()), Times.Once);
        }

        #endregion

        #region SaveOrUpdate Tests

        [Fact]
        [Trait("Method", "SaveOrUpdate")]
        public void SaveOrUpdate_WithValidANGLineOfDuty_ReturnsSavedANGLineOfDuty()
        {
            // Arrange
            var angLineOfDuty = new ANGLineOfDuty();
            _mockDao.Setup(dao => dao.SaveOrUpdate(It.IsAny<ANGLineOfDuty>()))
                .Returns(angLineOfDuty);

            // Act
            var result = _mockDao.Object.SaveOrUpdate(angLineOfDuty);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.SaveOrUpdate(It.IsAny<ANGLineOfDuty>()), Times.Once);
        }

        #endregion
    }
}
