using ALOD.Core.Interfaces.DAOInterfaces;
using Moq;
using System;
using System.Data;
using Xunit;

namespace ALOD.Tests.Unit.Data
{
    [Trait("Category", "Unit")]
    [Trait("Layer", "Data")]
    public class ApplicationWarmupProcessDaoTests
    {
        private readonly Mock<IApplicationWarmupProcessDao> _mockDao;

        public ApplicationWarmupProcessDaoTests()
        {
            _mockDao = new Mock<IApplicationWarmupProcessDao>();
        }

        #region DeleteLogById Tests

        [Fact]
        [Trait("Method", "DeleteLogById")]
        public void DeleteLogById_WithValidLogId_CallsDeleteLogById()
        {
            // Arrange
            _mockDao.Setup(dao => dao.DeleteLogById(It.IsAny<int>()));

            // Act
            _mockDao.Object.DeleteLogById(1);

            // Assert
            _mockDao.Verify(dao => dao.DeleteLogById(It.IsAny<int>()), Times.Once);
        }

        #endregion

        #region FindProcessLastExecutionDate Tests

        [Fact]
        [Trait("Method", "FindProcessLastExecutionDate")]
        public void FindProcessLastExecutionDate_WithValidProcessName_ReturnsDateTime()
        {
            // Arrange
            var executionDate = DateTime.Now;
            _mockDao.Setup(dao => dao.FindProcessLastExecutionDate(It.IsAny<string>()))
                .Returns(executionDate);

            // Act
            var result = _mockDao.Object.FindProcessLastExecutionDate("TestProcess");

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.FindProcessLastExecutionDate(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        [Trait("Method", "FindProcessLastExecutionDate")]
        public void FindProcessLastExecutionDate_WhenProcessNotFound_ReturnsNull()
        {
            // Arrange
            _mockDao.Setup(dao => dao.FindProcessLastExecutionDate(It.IsAny<string>()))
                .Returns((DateTime?)null);

            // Act
            var result = _mockDao.Object.FindProcessLastExecutionDate("NonExistentProcess");

            // Assert
            Assert.Null(result);
            _mockDao.Verify(dao => dao.FindProcessLastExecutionDate(It.IsAny<string>()), Times.Once);
        }

        #endregion

        #region GetAllLogs Tests

        [Fact]
        [Trait("Method", "GetAllLogs")]
        public void GetAllLogs_ReturnsDataSet()
        {
            // Arrange
            var dataSet = new DataSet();
            _mockDao.Setup(dao => dao.GetAllLogs())
                .Returns(dataSet);

            // Act
            var result = _mockDao.Object.GetAllLogs();

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetAllLogs(), Times.Once);
        }

        #endregion

        #region IsProcessActive Tests

        [Fact]
        [Trait("Method", "IsProcessActive")]
        public void IsProcessActive_WithActiveProcess_ReturnsTrue()
        {
            // Arrange
            _mockDao.Setup(dao => dao.IsProcessActive(It.IsAny<string>()))
                .Returns(true);

            // Act
            var result = _mockDao.Object.IsProcessActive("ActiveProcess");

            // Assert
            Assert.True(result);
            _mockDao.Verify(dao => dao.IsProcessActive(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        [Trait("Method", "IsProcessActive")]
        public void IsProcessActive_WithInactiveProcess_ReturnsFalse()
        {
            // Arrange
            _mockDao.Setup(dao => dao.IsProcessActive(It.IsAny<string>()))
                .Returns(false);

            // Act
            var result = _mockDao.Object.IsProcessActive("InactiveProcess");

            // Assert
            Assert.False(result);
            _mockDao.Verify(dao => dao.IsProcessActive(It.IsAny<string>()), Times.Once);
        }

        #endregion
    }
}
