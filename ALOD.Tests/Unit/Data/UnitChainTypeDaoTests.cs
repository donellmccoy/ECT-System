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
    public class UnitChainTypeDaoTests
    {
        private readonly Mock<IUnitChainTypeDao> _mockDao;

        public UnitChainTypeDaoTests()
        {
            _mockDao = new Mock<IUnitChainTypeDao>();
        }

        #region Save Tests

        [Fact]
        [Trait("Method", "Save")]
        public void Save_WithValidUnitChainType_ReturnsSavedUnitChainType()
        {
            // Arrange
            var unitChainType = new UnitChainType();
            _mockDao.Setup(dao => dao.Save(It.IsAny<UnitChainType>()))
                .Returns(unitChainType);

            // Act
            var result = _mockDao.Object.Save(unitChainType);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.Save(It.IsAny<UnitChainType>()), Times.Once);
        }

        #endregion

        #region GetById Tests

        [Fact]
        [Trait("Method", "GetById")]
        public void GetById_WithValidId_ReturnsUnitChainType()
        {
            // Arrange
            var unitChainType = new UnitChainType();
            _mockDao.Setup(dao => dao.GetById(It.IsAny<byte>(), false))
                .Returns(unitChainType);

            // Act
            var result = _mockDao.Object.GetById(1, false);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetById(It.IsAny<byte>(), false), Times.Once);
        }

        [Theory]
        [Trait("Method", "GetById")]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(255)]
        public void GetById_WithVariousIds_ReturnsUnitChainType(byte id)
        {
            // Arrange
            var unitChainType = new UnitChainType();
            _mockDao.Setup(dao => dao.GetById(It.IsAny<byte>()))
                .Returns(unitChainType);

            // Act
            var result = _mockDao.Object.GetById(id);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetById(It.IsAny<byte>()), Times.Once);
        }

        #endregion

        #region GetAll Tests

        [Fact]
        [Trait("Method", "GetAll")]
        public void GetAll_ReturnsAllUnitChainTypes()
        {
            // Arrange
            var unitChainTypes = new List<UnitChainType>
            {
                new UnitChainType(),
                new UnitChainType()
            }.AsQueryable();

            _mockDao.Setup(dao => dao.GetAll())
                .Returns(unitChainTypes);

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
        public void Delete_WithValidUnitChainType_CallsDelete()
        {
            // Arrange
            var unitChainType = new UnitChainType();
            _mockDao.Setup(dao => dao.Delete(It.IsAny<UnitChainType>()));

            // Act
            _mockDao.Object.Delete(unitChainType);

            // Assert
            _mockDao.Verify(dao => dao.Delete(It.IsAny<UnitChainType>()), Times.Once);
        }

        #endregion

        #region SaveOrUpdate Tests

        [Fact]
        [Trait("Method", "SaveOrUpdate")]
        public void SaveOrUpdate_WithValidUnitChainType_ReturnsSavedUnitChainType()
        {
            // Arrange
            var unitChainType = new UnitChainType();
            _mockDao.Setup(dao => dao.SaveOrUpdate(It.IsAny<UnitChainType>()))
                .Returns(unitChainType);

            // Act
            var result = _mockDao.Object.SaveOrUpdate(unitChainType);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.SaveOrUpdate(It.IsAny<UnitChainType>()), Times.Once);
        }

        #endregion
    }
}
