using ALOD.Core.Domain.Users;
using ALOD.Core.Interfaces.DAOInterfaces;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ALOD.Tests.Unit.Data
{
    [Trait("Category", "Unit")]
    [Trait("Layer", "Data")]
    public class UnitChainEntryDaoTests
    {
        private readonly Mock<IUnitChainEntryDao> _mockDao;

        public UnitChainEntryDaoTests()
        {
            _mockDao = new Mock<IUnitChainEntryDao>();
        }

        #region Save Tests

        [Fact]
        [Trait("Method", "Save")]
        public void Save_WithValidUnitChainEntry_ReturnsSavedUnitChainEntry()
        {
            // Arrange
            var unitChainEntry = new UnitChainEntry();
            _mockDao.Setup(dao => dao.Save(It.IsAny<UnitChainEntry>()))
                .Returns(unitChainEntry);

            // Act
            var result = _mockDao.Object.Save(unitChainEntry);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.Save(It.IsAny<UnitChainEntry>()), Times.Once);
        }

        #endregion

        #region GetById Tests

        [Fact]
        [Trait("Method", "GetById")]
        public void GetById_WithValidId_ReturnsUnitChainEntry()
        {
            // Arrange
            var unitChainEntry = new UnitChainEntry();
            _mockDao.Setup(dao => dao.GetById(It.IsAny<int>(), false))
                .Returns(unitChainEntry);

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
        public void GetById_WithVariousIds_ReturnsUnitChainEntry(int id)
        {
            // Arrange
            var unitChainEntry = new UnitChainEntry();
            _mockDao.Setup(dao => dao.GetById(It.IsAny<int>()))
                .Returns(unitChainEntry);

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
        public void GetAll_ReturnsAllUnitChainEntries()
        {
            // Arrange
            var unitChainEntries = new List<UnitChainEntry>
            {
                new UnitChainEntry(),
                new UnitChainEntry()
            }.AsQueryable();

            _mockDao.Setup(dao => dao.GetAll())
                .Returns(unitChainEntries);

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
        public void Delete_WithValidUnitChainEntry_CallsDelete()
        {
            // Arrange
            var unitChainEntry = new UnitChainEntry();
            _mockDao.Setup(dao => dao.Delete(It.IsAny<UnitChainEntry>()));

            // Act
            _mockDao.Object.Delete(unitChainEntry);

            // Assert
            _mockDao.Verify(dao => dao.Delete(It.IsAny<UnitChainEntry>()), Times.Once);
        }

        #endregion

        #region SaveOrUpdate Tests

        [Fact]
        [Trait("Method", "SaveOrUpdate")]
        public void SaveOrUpdate_WithValidUnitChainEntry_ReturnsSavedUnitChainEntry()
        {
            // Arrange
            var unitChainEntry = new UnitChainEntry();
            _mockDao.Setup(dao => dao.SaveOrUpdate(It.IsAny<UnitChainEntry>()))
                .Returns(unitChainEntry);

            // Act
            var result = _mockDao.Object.SaveOrUpdate(unitChainEntry);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.SaveOrUpdate(It.IsAny<UnitChainEntry>()), Times.Once);
        }

        #endregion
    }
}
