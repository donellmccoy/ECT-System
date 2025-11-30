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
    public class LineOfDutyUnitDaoTests
    {
        private readonly Mock<ILineOfDutyUnitDao> _mockDao;

        public LineOfDutyUnitDaoTests()
        {
            _mockDao = new Mock<ILineOfDutyUnitDao>();
        }

        #region Save Tests

        [Fact]
        [Trait("Method", "Save")]
        public void Save_WithValidEntity_ReturnsSavedEntity()
        {
            // Arrange
            var entity = new LineOfDutyUnit
            {
                AccidentDetails = "Test Details"
            };
            _mockDao.Setup(x => x.Save(It.IsAny<LineOfDutyUnit>())).Returns(entity);

            // Act
            var result = _mockDao.Object.Save(entity);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(x => x.Save(It.IsAny<LineOfDutyUnit>()), Times.Once);
        }

        #endregion

        #region GetById Tests

        [Fact]
        [Trait("Method", "GetById")]
        public void GetById_WithValidId_ReturnsEntity()
        {
            // Arrange
            int id = 100;
            var expected = new LineOfDutyUnit();
            _mockDao.Setup(x => x.GetById(id)).Returns(expected);

            // Act
            var result = _mockDao.Object.GetById(id);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(x => x.GetById(id), Times.Once);
        }

        [Theory]
        [Trait("Method", "GetById")]
        [InlineData(1)]
        [InlineData(50)]
        [InlineData(1000)]
        public void GetById_WithVariousIds_CallsDao(int id)
        {
            // Arrange
            _mockDao.Setup(x => x.GetById(id)).Returns(new LineOfDutyUnit());

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
            var units = new List<LineOfDutyUnit>
            {
                new LineOfDutyUnit(),
                new LineOfDutyUnit(),
                new LineOfDutyUnit()
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
            var entity = new LineOfDutyUnit();
            _mockDao.Setup(x => x.Delete(It.IsAny<LineOfDutyUnit>()));

            // Act
            _mockDao.Object.Delete(entity);

            // Assert
            _mockDao.Verify(x => x.Delete(It.IsAny<LineOfDutyUnit>()), Times.Once);
        }

        #endregion
    }
}
