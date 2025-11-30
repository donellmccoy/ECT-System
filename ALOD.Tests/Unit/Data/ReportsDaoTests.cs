using ALOD.Core.Domain.Reports;
using ALOD.Core.Interfaces.DAOInterfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Data;
using Xunit;

namespace ALOD.Tests.Unit.Data
{
    [Trait("Category", "Unit")]
    [Trait("Layer", "Data")]
    public class ReportsDaoTests
    {
        private readonly Mock<IReportsDao> _mockDao;

        public ReportsDaoTests()
        {
            _mockDao = new Mock<IReportsDao>();
        }

        #region DeleteAllStoredResults Tests

        [Fact]
        [Trait("Method", "DeleteAllStoredResults")]
        public void DeleteAllStoredResults_CallsDeleteAllStoredResults()
        {
            // Arrange
            _mockDao.Setup(dao => dao.DeleteAllStoredResults());

            // Act
            _mockDao.Object.DeleteAllStoredResults();

            // Assert
            _mockDao.Verify(dao => dao.DeleteAllStoredResults(), Times.Once);
        }

        #endregion

        #region GetStoredResult Tests

        [Fact]
        [Trait("Method", "GetStoredResult")]
        public void GetStoredResult_WithValidParameters_ReturnsString()
        {
            // Arrange
            _mockDao.Setup(dao => dao.GetStoredResult(It.IsAny<int>(), It.IsAny<string>()))
                .Returns("stored result");

            // Act
            var result = _mockDao.Object.GetStoredResult(1, "TestReport");

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetStoredResult(It.IsAny<int>(), It.IsAny<string>()), Times.Once);
        }

        #endregion

        #region SaveResult Tests

        [Fact]
        [Trait("Method", "SaveResult")]
        public void SaveResult_WithValidParameters_CallsSaveResult()
        {
            // Arrange
            _mockDao.Setup(dao => dao.SaveResult(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()));

            // Act
            _mockDao.Object.SaveResult(1, "TestReport", "result data");

            // Assert
            _mockDao.Verify(dao => dao.SaveResult(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        #endregion

        #region ExecuteLODAvgTimesMetricsReport Tests

        [Fact]
        [Trait("Method", "ExecuteLODAvgTimesMetricsReport")]
        public void ExecuteLODAvgTimesMetricsReport_WithValidArgs_ReturnsDataSet()
        {
            // Arrange
            var dataSet = new DataSet();
            var args = new LODAvgTimesMetricsReportArgs();
            _mockDao.Setup(dao => dao.ExecuteLODAvgTimesMetricsReport(It.IsAny<LODAvgTimesMetricsReportArgs>()))
                .Returns(dataSet);

            // Act
            var result = _mockDao.Object.ExecuteLODAvgTimesMetricsReport(args);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.ExecuteLODAvgTimesMetricsReport(It.IsAny<LODAvgTimesMetricsReportArgs>()), Times.Once);
        }

        #endregion

        #region ExecuteLODComplianceAccuracyReport Tests

        [Fact]
        [Trait("Method", "ExecuteLODComplianceAccuracyReport")]
        public void ExecuteLODComplianceAccuracyReport_WithValidParameters_ReturnsResultList()
        {
            // Arrange
            var resultList = new List<LODComplianceAccuracyResultItem>();
            var refIds = new List<int> { 1, 2, 3 };
            _mockDao.Setup(dao => dao.ExecuteLODComplianceAccuracyReport(
                It.IsAny<IList<int>>(), It.IsAny<int>(), It.IsAny<bool>()))
                .Returns(resultList);

            // Act
            var result = _mockDao.Object.ExecuteLODComplianceAccuracyReport(refIds, 1, false);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.ExecuteLODComplianceAccuracyReport(
                It.IsAny<IList<int>>(), It.IsAny<int>(), It.IsAny<bool>()), Times.Once);
        }

        #endregion

        #region ExecuteLODComplianceInit Tests

        [Fact]
        [Trait("Method", "ExecuteLODComplianceInit")]
        public void ExecuteLODComplianceInit_WithValidArgs_ReturnsRefIdList()
        {
            // Arrange
            var refIds = new List<int> { 1, 2, 3 };
            var args = new LODComplianceReportArgs();
            _mockDao.Setup(dao => dao.ExecuteLODComplianceInit(It.IsAny<LODComplianceReportArgs>()))
                .Returns(refIds);

            // Act
            var result = _mockDao.Object.ExecuteLODComplianceInit(args);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.ExecuteLODComplianceInit(It.IsAny<LODComplianceReportArgs>()), Times.Once);
        }

        #endregion

        #region ExecuteLODProgramStatusReport Tests

        [Fact]
        [Trait("Method", "ExecuteLODProgramStatusReport")]
        public void ExecuteLODProgramStatusReport_WithValidArgs_ReturnsResultList()
        {
            // Arrange
            var resultList = new List<LODProgramStatusReportResultItem>();
            var args = new LODProgramStatusReportArgs();
            _mockDao.Setup(dao => dao.ExecuteLODProgramStatusReport(It.IsAny<LODProgramStatusReportArgs>()))
                .Returns(resultList);

            // Act
            var result = _mockDao.Object.ExecuteLODProgramStatusReport(args);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.ExecuteLODProgramStatusReport(It.IsAny<LODProgramStatusReportArgs>()), Times.Once);
        }

        #endregion

        #region ExecuteLODSARCCasesReport Tests

        [Fact]
        [Trait("Method", "ExecuteLODSARCCasesReport")]
        public void ExecuteLODSARCCasesReport_WithValidArgs_ReturnsResultList()
        {
            // Arrange
            var resultList = new List<LODSARCCasesReportResultItem>();
            var args = new LODSARCCasesReportArgs();
            _mockDao.Setup(dao => dao.ExecuteLODSARCCasesReport(It.IsAny<LODSARCCasesReportArgs>()))
                .Returns(resultList);

            // Act
            var result = _mockDao.Object.ExecuteLODSARCCasesReport(args);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.ExecuteLODSARCCasesReport(It.IsAny<LODSARCCasesReportArgs>()), Times.Once);
        }

        #endregion

        #region ExecutePackagesReport Tests

        [Fact]
        [Trait("Method", "ExecutePackagesReport")]
        public void ExecutePackagesReport_WithValidDateRange_ReturnsDataSet()
        {
            // Arrange
            var dataSet = new DataSet();
            _mockDao.Setup(dao => dao.ExecutePackagesReport(It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
                .Returns(dataSet);

            // Act
            var result = _mockDao.Object.ExecutePackagesReport(DateTime.Now.AddDays(-30), DateTime.Now);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.ExecutePackagesReport(It.IsAny<DateTime?>(), It.IsAny<DateTime?>()), Times.Once);
        }

        #endregion

        #region ExecutePALDocumentsReport Tests

        [Fact]
        [Trait("Method", "ExecutePALDocumentsReport")]
        public void ExecutePALDocumentsReport_WithValidParameters_ReturnsDataSet()
        {
            // Arrange
            var dataSet = new DataSet();
            _mockDao.Setup(dao => dao.ExecutePALDocumentsReport(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(dataSet);

            // Act
            var result = _mockDao.Object.ExecutePALDocumentsReport("Smith", "1234");

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.ExecutePALDocumentsReport(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        #endregion
    }
}
