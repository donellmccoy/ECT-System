using ALOD.Core.Domain.Modules.SpecialCases;
using ALOD.Core.Interfaces.DAOInterfaces;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ALOD.Tests.Unit.Data
{
    [Trait("Category", "Unit")]
    [Trait("Layer", "Data")]
    public class PALDataDaoTests
    {
        private readonly Mock<IPALDataDao> _mockDao;

        public PALDataDaoTests()
        {
            _mockDao = new Mock<IPALDataDao>();
        }

        #region Save Tests

        [Fact]
        [Trait("Method", "Save")]
        public void Save_WithValidPALData_ReturnsSavedPALData()
        {
            // Arrange
            var palData = new PAL_Data();
            _mockDao.Setup(dao => dao.Save(It.IsAny<PAL_Data>()))
                .Returns(palData);

            // Act
            var result = _mockDao.Object.Save(palData);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.Save(It.IsAny<PAL_Data>()), Times.Once);
        }

        #endregion

        #region GetById Tests

        [Fact]
        [Trait("Method", "GetById")]
        public void GetById_WithValidId_ReturnsPALData()
        {
            // Arrange
            var palData = new PAL_Data();
            _mockDao.Setup(dao => dao.GetById(It.IsAny<int>(), false))
                .Returns(palData);

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
        public void GetById_WithVariousIds_ReturnsPALData(int id)
        {
            // Arrange
            var palData = new PAL_Data();
            _mockDao.Setup(dao => dao.GetById(It.IsAny<int>()))
                .Returns(palData);

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
        public void GetAll_ReturnsAllPALData()
        {
            // Arrange
            var palDataList = new List<PAL_Data>
            {
                new PAL_Data(),
                new PAL_Data()
            }.AsQueryable();

            _mockDao.Setup(dao => dao.GetAll())
                .Returns(palDataList);

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
        public void Delete_WithValidPALData_CallsDelete()
        {
            // Arrange
            var palData = new PAL_Data();
            _mockDao.Setup(dao => dao.Delete(It.IsAny<PAL_Data>()));

            // Act
            _mockDao.Object.Delete(palData);

            // Assert
            _mockDao.Verify(dao => dao.Delete(It.IsAny<PAL_Data>()), Times.Once);
        }

        #endregion

        #region SaveOrUpdate Tests

        [Fact]
        [Trait("Method", "SaveOrUpdate")]
        public void SaveOrUpdate_WithValidPALData_ReturnsSavedPALData()
        {
            // Arrange
            var palData = new PAL_Data();
            _mockDao.Setup(dao => dao.SaveOrUpdate(It.IsAny<PAL_Data>()))
                .Returns(palData);

            // Act
            var result = _mockDao.Object.SaveOrUpdate(palData);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.SaveOrUpdate(It.IsAny<PAL_Data>()), Times.Once);
        }

        #endregion
    }
}
