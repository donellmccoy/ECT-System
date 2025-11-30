using ALOD.Core.Domain.Reports;
using ALOD.Core.Interfaces.DAOInterfaces;
using Moq;
using System;
using System.Data;
using Xunit;

namespace ALOD.Tests.Unit.Data
{
    [Trait("Category", "Unit")]
    [Trait("Layer", "Data")]
    public class ARCNetDaoTests
    {
        private readonly Mock<IARCNetDao> _mockDao;

        public ARCNetDaoTests()
        {
            _mockDao = new Mock<IARCNetDao>();
        }

        #region GetARCNetImportLastExecutionDate Tests

        [Fact]
        [Trait("Method", "GetARCNetImportLastExecutionDate")]
        public void GetARCNetImportLastExecutionDate_WhenDataExists_ReturnsDateTime()
        {
            // Arrange
            var executionDate = DateTime.Now;
            _mockDao.Setup(dao => dao.GetARCNetImportLastExecutionDate())
                .Returns(executionDate);

            // Act
            var result = _mockDao.Object.GetARCNetImportLastExecutionDate();

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetARCNetImportLastExecutionDate(), Times.Once);
        }

        [Fact]
        [Trait("Method", "GetARCNetImportLastExecutionDate")]
        public void GetARCNetImportLastExecutionDate_WhenNoDataExists_ReturnsNull()
        {
            // Arrange
            _mockDao.Setup(dao => dao.GetARCNetImportLastExecutionDate())
                .Returns((DateTime?)null);

            // Act
            var result = _mockDao.Object.GetARCNetImportLastExecutionDate();

            // Assert
            Assert.Null(result);
            _mockDao.Verify(dao => dao.GetARCNetImportLastExecutionDate(), Times.Once);
        }

        #endregion

        #region GetIAATrainingDataForUsers Tests

        [Fact]
        [Trait("Method", "GetIAATrainingDataForUsers")]
        public void GetIAATrainingDataForUsers_WithValidArgs_ReturnsDataSet()
        {
            // Arrange
            var dataSet = new DataSet();
            var args = new ARCNetLookupReportArgs();
            _mockDao.Setup(dao => dao.GetIAATrainingDataForUsers(It.IsAny<ARCNetLookupReportArgs>()))
                .Returns(dataSet);

            // Act
            var result = _mockDao.Object.GetIAATrainingDataForUsers(args);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetIAATrainingDataForUsers(It.IsAny<ARCNetLookupReportArgs>()), Times.Once);
        }

        #endregion
    }
}
