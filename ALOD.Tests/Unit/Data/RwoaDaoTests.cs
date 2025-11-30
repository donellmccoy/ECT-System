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
    public class RwoaDaoTests
    {
        private readonly Mock<IRwoaDao> _mockDao;

        public RwoaDaoTests()
        {
            _mockDao = new Mock<IRwoaDao>();
        }

        #region Save Tests

        [Fact]
        [Trait("Method", "Save")]
        public void Save_WithValidRwoa_ReturnsSavedRwoa()
        {
            // Arrange
            var rwoa = new Rwoa();
            _mockDao.Setup(dao => dao.Save(It.IsAny<Rwoa>()))
                .Returns(rwoa);

            // Act
            var result = _mockDao.Object.Save(rwoa);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.Save(It.IsAny<Rwoa>()), Times.Once);
        }

        #endregion

        #region GetById Tests

        [Fact]
        [Trait("Method", "GetById")]
        public void GetById_WithValidId_ReturnsRwoa()
        {
            // Arrange
            var rwoa = new Rwoa();
            _mockDao.Setup(dao => dao.GetById(It.IsAny<int>(), false))
                .Returns(rwoa);

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
        public void GetById_WithVariousIds_ReturnsRwoa(int id)
        {
            // Arrange
            var rwoa = new Rwoa();
            _mockDao.Setup(dao => dao.GetById(It.IsAny<int>()))
                .Returns(rwoa);

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
        public void GetAll_ReturnsAllRwoas()
        {
            // Arrange
            var rwoas = new List<Rwoa>
            {
                new Rwoa(),
                new Rwoa()
            }.AsQueryable();

            _mockDao.Setup(dao => dao.GetAll())
                .Returns(rwoas);

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
        public void Delete_WithValidRwoa_CallsDelete()
        {
            // Arrange
            var rwoa = new Rwoa();
            _mockDao.Setup(dao => dao.Delete(It.IsAny<Rwoa>()));

            // Act
            _mockDao.Object.Delete(rwoa);

            // Assert
            _mockDao.Verify(dao => dao.Delete(It.IsAny<Rwoa>()), Times.Once);
        }

        #endregion

        #region SaveOrUpdate Tests

        [Fact]
        [Trait("Method", "SaveOrUpdate")]
        public void SaveOrUpdate_WithValidRwoa_ReturnsSavedRwoa()
        {
            // Arrange
            var rwoa = new Rwoa();
            _mockDao.Setup(dao => dao.SaveOrUpdate(It.IsAny<Rwoa>()))
                .Returns(rwoa);

            // Act
            var result = _mockDao.Object.SaveOrUpdate(rwoa);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.SaveOrUpdate(It.IsAny<Rwoa>()), Times.Once);
        }

        #endregion
    }
}
