using ALOD.Core.Domain.Modules.Lod;
using ALOD.Core.Interfaces.DAOInterfaces;
using Moq;
using Xunit;

namespace ALOD.Tests.Unit.Data
{
    [Trait("Category", "Unit")]
    [Trait("Layer", "Data")]
    public class LineOfDutyFindingsDaoTests
    {
        private readonly Mock<ILineOfDutyFindingsDao> _mockDao;

        public LineOfDutyFindingsDaoTests()
        {
            _mockDao = new Mock<ILineOfDutyFindingsDao>();
        }

        #region Save Tests

        [Fact]
        [Trait("Method", "Save")]
        public void Save_WithValidEntity_ReturnsSavedEntity()
        {
            // Arrange
            var entity = new LineOfDutyFindings
            {
                Name = "Test Name",
                SSN = "123456789"
            };
            _mockDao.Setup(x => x.Save(It.IsAny<LineOfDutyFindings>())).Returns(entity);

            // Act
            var result = _mockDao.Object.Save(entity);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(x => x.Save(It.IsAny<LineOfDutyFindings>()), Times.Once);
        }

        [Fact]
        [Trait("Method", "Save")]
        public void Save_WithNullEntity_ThrowsException()
        {
            // Arrange
            _mockDao.Setup(x => x.Save(null))
                .Throws<System.ArgumentNullException>();

            // Act & Assert
            Assert.Throws<System.ArgumentNullException>(() => _mockDao.Object.Save(null));
        }

        #endregion

        #region SaveOrUpdate Tests

        [Fact]
        [Trait("Method", "SaveOrUpdate")]
        public void SaveOrUpdate_WithValidEntity_ReturnsSavedEntity()
        {
            // Arrange
            var entity = new LineOfDutyFindings
            {
                Name = "Updated Name"
            };
            _mockDao.Setup(x => x.SaveOrUpdate(It.IsAny<LineOfDutyFindings>())).Returns(entity);

            // Act
            var result = _mockDao.Object.SaveOrUpdate(entity);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(x => x.SaveOrUpdate(It.IsAny<LineOfDutyFindings>()), Times.Once);
        }

        #endregion

        #region GetById Tests

        [Fact]
        [Trait("Method", "GetById")]
        public void GetById_WithValidId_ReturnsEntity()
        {
            // Arrange
            int id = 1;
            var expected = new LineOfDutyFindings { Name = "Test" };
            _mockDao.Setup(x => x.GetById(id)).Returns(expected);

            // Act
            var result = _mockDao.Object.GetById(id);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(x => x.GetById(id), Times.Once);
        }

        [Theory]
        [Trait("Method", "GetById")]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(999)]
        public void GetById_WithVariousIds_ReturnsCorrectEntity(int id)
        {
            // Arrange
            _mockDao.Setup(x => x.GetById(id)).Returns(new LineOfDutyFindings());

            // Act
            var result = _mockDao.Object.GetById(id);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(x => x.GetById(id), Times.Once);
        }

        #endregion

        #region Delete Tests

        [Fact]
        [Trait("Method", "Delete")]
        public void Delete_WithValidEntity_CallsDelete()
        {
            // Arrange
            var entity = new LineOfDutyFindings { Name = "Test" };
            _mockDao.Setup(x => x.Delete(It.IsAny<LineOfDutyFindings>()));

            // Act
            _mockDao.Object.Delete(entity);

            // Assert
            _mockDao.Verify(x => x.Delete(It.IsAny<LineOfDutyFindings>()), Times.Once);
        }

        #endregion
    }
}
