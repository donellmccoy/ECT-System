using ALOD.Core.Domain.Users;
using ALOD.Core.Interfaces.DAOInterfaces;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using DomainUnit = ALOD.Core.Domain.Users.Unit;

namespace ALOD.Tests.Unit.Data
{
    [Trait("Category", "Unit")]
    [Trait("Layer", "Data")]
    public class UnitDaoTests
    {
        private readonly Mock<IUnitDao> _mockDao;

        public UnitDaoTests()
        {
            _mockDao = new Mock<IUnitDao>();
        }

        #region Save Tests

        [Fact]
        [Trait("Method", "Save")]
        public void Save_WithValidEntity_ReturnsSavedEntity()
        {
            // Arrange
            var entity = new DomainUnit();
            _mockDao.Setup(x => x.Save(It.IsAny<DomainUnit>())).Returns(entity);

            // Act
            var result = _mockDao.Object.Save(entity);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(x => x.Save(It.IsAny<DomainUnit>()), Times.Once);
        }

        #endregion

        #region GetById Tests

        [Fact]
        [Trait("Method", "GetById")]
        public void GetById_WithValidId_ReturnsEntity()
        {
            // Arrange
            int id = 1;
            var entity = new DomainUnit();
            _mockDao.Setup(x => x.GetById(id, false)).Returns(entity);

            // Act
            var result = _mockDao.Object.GetById(id, false);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(x => x.GetById(id, false), Times.Once);
        }

        [Theory]
        [Trait("Method", "GetById")]
        [InlineData(1)]
        [InlineData(100)]
        [InlineData(999)]
        public void GetById_WithVariousIds_ReturnsUnit(int id)
        {
            // Arrange
            _mockDao.Setup(x => x.GetById(id)).Returns(new DomainUnit());

            // Act
            var result = _mockDao.Object.GetById(id);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(x => x.GetById(id), Times.Once);
        }

        #endregion

        #region GetAll Tests

        [Fact]
        [Trait("Method", "GetAll")]
        public void GetAll_ReturnsAllEntities()
        {
            // Arrange
            var units = new List<DomainUnit>
            {
                new DomainUnit(),
                new DomainUnit(),
                new DomainUnit()
            }.AsQueryable();
            _mockDao.Setup(x => x.GetAll()).Returns(units);

            // Act
            var result = _mockDao.Object.GetAll();

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(x => x.GetAll(), Times.Once);
        }

        #endregion

        #region Delete Tests

        [Fact]
        [Trait("Method", "Delete")]
        public void Delete_WithValidEntity_CallsDelete()
        {
            // Arrange
            var entity = new DomainUnit();
            _mockDao.Setup(x => x.Delete(It.IsAny<DomainUnit>()));

            // Act
            _mockDao.Object.Delete(entity);

            // Assert
            _mockDao.Verify(x => x.Delete(It.IsAny<DomainUnit>()), Times.Once);
        }

        #endregion

        #region SaveOrUpdate Tests

        [Fact]
        [Trait("Method", "SaveOrUpdate")]
        public void SaveOrUpdate_WithValidEntity_ReturnsSavedEntity()
        {
            // Arrange
            var entity = new DomainUnit();
            _mockDao.Setup(x => x.SaveOrUpdate(It.IsAny<DomainUnit>())).Returns(entity);

            // Act
            var result = _mockDao.Object.SaveOrUpdate(entity);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(x => x.SaveOrUpdate(It.IsAny<DomainUnit>()), Times.Once);
        }

        #endregion
    }
}
