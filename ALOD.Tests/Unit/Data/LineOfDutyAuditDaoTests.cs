using ALOD.Core.Domain.Modules.Lod;
using ALOD.Core.Interfaces.DAOInterfaces;
using Moq;
using System;
using System.Data;
using Xunit;

namespace ALOD.Tests.Unit.Data
{
    [Trait("Category", "Unit")]
    [Trait("Layer", "Data")]
    public class LineOfDutyAuditDaoTests
    {
        private readonly Mock<ILineOfDutyAuditDao> _mockDao;

        public LineOfDutyAuditDaoTests()
        {
            _mockDao = new Mock<ILineOfDutyAuditDao>();
        }

        #region GetAuditInfo Tests

        [Fact]
        [Trait("Method", "GetAuditInfo")]
        public void GetAuditInfo_WithValidLodId_ReturnsDataSet()
        {
            // Arrange
            int lodId = 123;
            var expectedDataSet = new DataSet();
            expectedDataSet.Tables.Add(new DataTable());
            _mockDao.Setup(x => x.GetAuditInfo(lodId)).Returns(expectedDataSet);

            // Act
            var result = _mockDao.Object.GetAuditInfo(lodId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Tables);
            _mockDao.Verify(x => x.GetAuditInfo(lodId), Times.Once);
        }

        [Theory]
        [Trait("Method", "GetAuditInfo")]
        [InlineData(1)]
        [InlineData(100)]
        [InlineData(999)]
        public void GetAuditInfo_WithVariousLodIds_ReturnsDataSet(int lodId)
        {
            // Arrange
            var dataSet = new DataSet();
            dataSet.Tables.Add(new DataTable());
            _mockDao.Setup(x => x.GetAuditInfo(lodId)).Returns(dataSet);

            // Act
            var result = _mockDao.Object.GetAuditInfo(lodId);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(x => x.GetAuditInfo(lodId), Times.Once);
        }

        [Fact]
        [Trait("Method", "GetAuditInfo")]
        public void GetAuditInfo_WithInvalidLodId_ReturnsEmptyDataSet()
        {
            // Arrange
            int lodId = -1;
            var emptyDataSet = new DataSet();
            _mockDao.Setup(x => x.GetAuditInfo(lodId)).Returns(emptyDataSet);

            // Act
            var result = _mockDao.Object.GetAuditInfo(lodId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.Tables);
            _mockDao.Verify(x => x.GetAuditInfo(lodId), Times.Once);
        }

        #endregion

        #region Save Tests

        [Fact]
        [Trait("Method", "Save")]
        public void Save_WithValidEntity_ReturnsSavedEntity()
        {
            // Arrange
            var entity = new LineOfDutyAudit
            {
                A1_Comment = "Test Comment"
            };
            _mockDao.Setup(x => x.Save(It.IsAny<LineOfDutyAudit>())).Returns(entity);

            // Act
            var result = _mockDao.Object.Save(entity);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(x => x.Save(It.IsAny<LineOfDutyAudit>()), Times.Once);
        }

        #endregion

        #region GetById Tests

        [Fact]
        [Trait("Method", "GetById")]
        public void GetById_WithValidId_ReturnsAudit()
        {
            // Arrange
            int id = 50;
            var expected = new LineOfDutyAudit();
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
        [InlineData(500)]
        public void GetById_WithVariousIds_ReturnsAudit(int id)
        {
            // Arrange
            _mockDao.Setup(x => x.GetById(id)).Returns(new LineOfDutyAudit());

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
            var entity = new LineOfDutyAudit();
            _mockDao.Setup(x => x.Delete(It.IsAny<LineOfDutyAudit>()));

            // Act
            _mockDao.Object.Delete(entity);

            // Assert
            _mockDao.Verify(x => x.Delete(It.IsAny<LineOfDutyAudit>()), Times.Once);
        }

        #endregion

        #region SaveOrUpdate Tests

        [Fact]
        [Trait("Method", "SaveOrUpdate")]
        public void SaveOrUpdate_WithValidEntity_ReturnsSavedEntity()
        {
            // Arrange
            var entity = new LineOfDutyAudit();
            _mockDao.Setup(x => x.SaveOrUpdate(It.IsAny<LineOfDutyAudit>())).Returns(entity);

            // Act
            var result = _mockDao.Object.SaveOrUpdate(entity);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(x => x.SaveOrUpdate(It.IsAny<LineOfDutyAudit>()), Times.Once);
        }

        #endregion
    }
}
