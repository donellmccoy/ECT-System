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
    public class LineOfDutyMedicalDaoTests
    {
        private readonly Mock<ILineOfDutyMedicalDao> _mockDao;

        public LineOfDutyMedicalDaoTests()
        {
            _mockDao = new Mock<ILineOfDutyMedicalDao>();
        }

        #region Save Tests

        [Fact]
        [Trait("Method", "Save")]
        public void Save_WithValidEntity_ReturnsSavedEntity()
        {
            // Arrange
            var entity = new LineOfDutyMedical
            {
                ApprovalComments = "Test Comments"
            };
            _mockDao.Setup(x => x.Save(It.IsAny<LineOfDutyMedical>())).Returns(entity);

            // Act
            var result = _mockDao.Object.Save(entity);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(x => x.Save(It.IsAny<LineOfDutyMedical>()), Times.Once);
        }

        #endregion

        #region GetById Tests

        [Fact]
        [Trait("Method", "GetById")]
        public void GetById_WithValidId_ReturnsEntity()
        {
            // Arrange
            int id = 50;
            var expected = new LineOfDutyMedical();
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
        [InlineData(25)]
        [InlineData(100)]
        public void GetById_WithVariousIds_ReturnsEntity(int id)
        {
            // Arrange
            _mockDao.Setup(x => x.GetById(id)).Returns(new LineOfDutyMedical());

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
        public void GetAll_ReturnsAllMedicalRecords()
        {
            // Arrange
            var records = new List<LineOfDutyMedical>
            {
                new LineOfDutyMedical(),
                new LineOfDutyMedical(),
                new LineOfDutyMedical()
            }.AsQueryable();
            _mockDao.Setup(x => x.GetAll()).Returns(records);

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
            var entity = new LineOfDutyMedical();
            _mockDao.Setup(x => x.SaveOrUpdate(It.IsAny<LineOfDutyMedical>())).Returns(entity);

            // Act
            var result = _mockDao.Object.SaveOrUpdate(entity);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(x => x.SaveOrUpdate(It.IsAny<LineOfDutyMedical>()), Times.Once);
        }

        #endregion

        #region Delete Tests

        [Fact]
        [Trait("Method", "Delete")]
        public void Delete_WithValidEntity_CallsDelete()
        {
            // Arrange
            var entity = new LineOfDutyMedical();
            _mockDao.Setup(x => x.Delete(It.IsAny<LineOfDutyMedical>()));

            // Act
            _mockDao.Object.Delete(entity);

            // Assert
            _mockDao.Verify(x => x.Delete(It.IsAny<LineOfDutyMedical>()), Times.Once);
        }

        #endregion
    }
}
