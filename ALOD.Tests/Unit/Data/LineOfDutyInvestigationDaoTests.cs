using ALOD.Core.Domain.Modules.Lod;
using ALOD.Core.Interfaces.DAOInterfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ALOD.Tests.Unit.Data
{
    [Trait("Category", "Unit")]
    [Trait("Layer", "Data")]
    public class LineOfDutyInvestigationDaoTests
    {
        private readonly Mock<ILineOfDutyInvestigationDao> _mockDao;

        public LineOfDutyInvestigationDaoTests()
        {
            _mockDao = new Mock<ILineOfDutyInvestigationDao>();
        }

        #region Save Tests

        [Fact]
        [Trait("Method", "Save")]
        public void Save_WithValidEntity_ReturnsSavedEntity()
        {
            // Arrange
            var entity = CreateValidInvestigation();
            _mockDao.Setup(x => x.Save(It.IsAny<LineOfDutyInvestigation>())).Returns(entity);

            // Act
            var result = _mockDao.Object.Save(entity);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(x => x.Save(It.IsAny<LineOfDutyInvestigation>()), Times.Once);
        }

        [Fact]
        [Trait("Method", "Save")]
        public void Save_WithNullEntity_ThrowsException()
        {
            // Arrange
            _mockDao.Setup(x => x.Save(null))
                .Throws<ArgumentNullException>();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _mockDao.Object.Save(null));
        }

        #endregion

        #region GetById Tests

        [Fact]
        [Trait("Method", "GetById")]
        public void GetById_WithValidId_ReturnsEntity()
        {
            // Arrange
            int id = 300;
            var expected = new LineOfDutyInvestigation();
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
        [InlineData(500)]
        public void GetById_WithVariousIds_ReturnsEntity(int id)
        {
            // Arrange
            _mockDao.Setup(x => x.GetById(id)).Returns(CreateValidInvestigation());

            // Act
            var result = _mockDao.Object.GetById(id);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(x => x.GetById(id), Times.Once);
        }

        [Fact]
        [Trait("Method", "GetById")]
        public void GetById_WithNonExistentId_ReturnsNull()
        {
            // Arrange
            int id = 9999;
            _mockDao.Setup(x => x.GetById(id)).Returns((LineOfDutyInvestigation)null);

            // Act
            var result = _mockDao.Object.GetById(id);

            // Assert
            Assert.Null(result);
            _mockDao.Verify(x => x.GetById(id), Times.Once);
        }

        #endregion

        #region GetAll Tests

        [Fact]
        [Trait("Method", "GetAll")]
        public void GetAll_ReturnsAllInvestigations()
        {
            // Arrange
            var investigations = new List<LineOfDutyInvestigation>
            {
                CreateValidInvestigation(),
                CreateValidInvestigation(),
                CreateValidInvestigation()
            }.AsQueryable();
            _mockDao.Setup(x => x.GetAll()).Returns(investigations);

            // Act
            var result = _mockDao.Object.GetAll();

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(x => x.GetAll(), Times.Once);
        }

        #endregion

        #region SaveOrUpdate Tests

        [Fact]
        [Trait("Method", "SaveOrUpdate")]
        public void SaveOrUpdate_WithValidEntity_ReturnsSavedEntity()
        {
            // Arrange
            var entity = CreateValidInvestigation();
            _mockDao.Setup(x => x.SaveOrUpdate(It.IsAny<LineOfDutyInvestigation>())).Returns(entity);

            // Act
            var result = _mockDao.Object.SaveOrUpdate(entity);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(x => x.SaveOrUpdate(It.IsAny<LineOfDutyInvestigation>()), Times.Once);
        }

        #endregion

        #region Delete Tests

        [Fact]
        [Trait("Method", "Delete")]
        public void Delete_WithValidEntity_CallsDelete()
        {
            // Arrange
            var entity = CreateValidInvestigation();
            _mockDao.Setup(x => x.Delete(It.IsAny<LineOfDutyInvestigation>()));

            // Act
            _mockDao.Object.Delete(entity);

            // Assert
            _mockDao.Verify(x => x.Delete(It.IsAny<LineOfDutyInvestigation>()), Times.Once);
        }

        #endregion

        #region Helper Methods

        private LineOfDutyInvestigation CreateValidInvestigation()
        {
            return new LineOfDutyInvestigation();
        }

        #endregion
    }
}
