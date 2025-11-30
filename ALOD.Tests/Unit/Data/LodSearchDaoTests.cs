using ALOD.Core.Domain.Modules.Lod;
using ALOD.Core.Interfaces.DAOInterfaces;
using Moq;
using System.Data;
using Xunit;

namespace ALOD.Tests.Unit.Data
{
    [Trait("Category", "Unit")]
    [Trait("Layer", "Data")]
    public class LodSearchDaoTests
    {
        private readonly Mock<ILodSearchDao> _mockDao;

        public LodSearchDaoTests()
        {
            _mockDao = new Mock<ILodSearchDao>();
        }

        #region GetAll Tests

        [Fact]
        [Trait("Method", "GetAll")]
        public void GetAll_WithValidParameters_ReturnsDataSet()
        {
            // Arrange
            var dataSet = new DataSet();
            _mockDao.Setup(dao => dao.GetAll(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), 
                It.IsAny<int>(), It.IsAny<byte>(), It.IsAny<string>(), It.IsAny<int>(), 
                It.IsAny<byte>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>()))
                .Returns(dataSet);

            // Act
            var result = _mockDao.Object.GetAll("LOD-123", "123456789", "Smith", 1, 1, 1, "01", 100, 1, "Y", 1, false);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetAll(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), 
                It.IsAny<int>(), It.IsAny<byte>(), It.IsAny<string>(), It.IsAny<int>(), 
                It.IsAny<byte>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>()), Times.Once);
        }

        [Fact]
        [Trait("Method", "GetAll")]
        public void GetAll_WithFullNameParameters_ReturnsDataSet()
        {
            // Arrange
            var dataSet = new DataSet();
            _mockDao.Setup(dao => dao.GetAll(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), 
                It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<byte>(), 
                It.IsAny<string>(), It.IsAny<int>(), It.IsAny<byte>(), It.IsAny<string>(), 
                It.IsAny<int>(), It.IsAny<bool>()))
                .Returns(dataSet);

            // Act
            var result = _mockDao.Object.GetAll("LOD-123", "123456789", "Smith", "John", "A", 1, 1, 1, "01", 100, 1, "Y", 1, false);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetAll(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), 
                It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<byte>(), 
                It.IsAny<string>(), It.IsAny<int>(), It.IsAny<byte>(), It.IsAny<string>(), 
                It.IsAny<int>(), It.IsAny<bool>()), Times.Once);
        }

        #endregion

        #region GetByCaseId Tests

        [Fact]
        [Trait("Method", "GetByCaseId")]
        public void GetByCaseId_WithValidCaseId_ReturnsLineOfDuty()
        {
            // Arrange
            var lod = new LineOfDuty();
            _mockDao.Setup(dao => dao.GetByCaseId(It.IsAny<string>()))
                .Returns(lod);

            // Act
            var result = _mockDao.Object.GetByCaseId("LOD-123");

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetByCaseId(It.IsAny<string>()), Times.Once);
        }

        #endregion

        #region GetByPilotUser Tests

        [Fact]
        [Trait("Method", "GetByPilotUser")]
        public void GetByPilotUser_WithValidParameters_ReturnsDataSet()
        {
            // Arrange
            var dataSet = new DataSet();
            _mockDao.Setup(dao => dao.GetByPilotUser(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(dataSet);

            // Act
            var result = _mockDao.Object.GetByPilotUser(1, 1);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetByPilotUser(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
        }

        #endregion

        #region GetByUser Tests

        [Fact]
        [Trait("Method", "GetByUser")]
        public void GetByUser_WithValidParameters_ReturnsDataSet()
        {
            // Arrange
            var dataSet = new DataSet();
            _mockDao.Setup(dao => dao.GetByUser(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), 
                It.IsAny<int>(), It.IsAny<byte>(), It.IsAny<string>(), It.IsAny<int>(), 
                It.IsAny<byte>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>()))
                .Returns(dataSet);

            // Act
            var result = _mockDao.Object.GetByUser("LOD-123", "123456789", "Smith", 1, 1, 1, "01", 100, 1, "Y", 1, false);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetByUser(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), 
                It.IsAny<int>(), It.IsAny<byte>(), It.IsAny<string>(), It.IsAny<int>(), 
                It.IsAny<byte>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>()), Times.Once);
        }

        #endregion

        #region GetByUserLOD_IO Tests

        [Fact]
        [Trait("Method", "GetByUserLOD_IO")]
        public void GetByUserLOD_IO_WithValidParameters_ReturnsDataSet()
        {
            // Arrange
            var dataSet = new DataSet();
            _mockDao.Setup(dao => dao.GetByUserLOD_IO(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), 
                It.IsAny<int>(), It.IsAny<byte>(), It.IsAny<string>(), It.IsAny<int>(), 
                It.IsAny<byte>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>()))
                .Returns(dataSet);

            // Act
            var result = _mockDao.Object.GetByUserLOD_IO("LOD-123", "123456789", "Smith", 1, 1, 1, "01", 100, 1, "Y", 1, false);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetByUserLOD_IO(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), 
                It.IsAny<int>(), It.IsAny<byte>(), It.IsAny<string>(), It.IsAny<int>(), 
                It.IsAny<byte>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>()), Times.Once);
        }

        #endregion

        #region GetLodsBySM Tests

        [Fact]
        [Trait("Method", "GetLodsBySM")]
        public void GetLodsBySM_WithValidSSN_ReturnsDataSet()
        {
            // Arrange
            var dataSet = new DataSet();
            _mockDao.Setup(dao => dao.GetLodsBySM(It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(dataSet);

            // Act
            var result = _mockDao.Object.GetLodsBySM("123456789", false);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetLodsBySM(It.IsAny<string>(), It.IsAny<bool>()), Times.Once);
        }

        #endregion

        #region GetPostCompletion Tests

        [Fact]
        [Trait("Method", "GetPostCompletion")]
        public void GetPostCompletion_WithValidParameters_ReturnsDataSet()
        {
            // Arrange
            var dataSet = new DataSet();
            _mockDao.Setup(dao => dao.GetPostCompletion(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), 
                It.IsAny<byte>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<byte>(), 
                It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<int>()))
                .Returns(dataSet);

            // Act
            var result = _mockDao.Object.GetPostCompletion("LOD-123", "123456789", "Smith", 1, 1, "01", 100, 1, "Y", 1, false, true, 1);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetPostCompletion(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), 
                It.IsAny<byte>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<byte>(), 
                It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<int>()), Times.Once);
        }

        #endregion

        #region GetUndeleted Tests

        [Fact]
        [Trait("Method", "GetUndeleted")]
        public void GetUndeleted_WithValidParameters_ReturnsDataSet()
        {
            // Arrange
            var dataSet = new DataSet();
            _mockDao.Setup(dao => dao.GetUndeleted(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), 
                It.IsAny<int>(), It.IsAny<byte>(), It.IsAny<string>(), It.IsAny<int>(), 
                It.IsAny<byte>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(dataSet);

            // Act
            var result = _mockDao.Object.GetUndeleted("LOD-123", "123456789", "Smith", 1, 1, 1, "01", 100, 1, "Y", 1, false, false);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetUndeleted(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), 
                It.IsAny<int>(), It.IsAny<byte>(), It.IsAny<string>(), It.IsAny<int>(), 
                It.IsAny<byte>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Once);
        }

        #endregion

        #region GetUndeletedCases Tests

        [Fact]
        [Trait("Method", "GetUndeletedCases")]
        public void GetUndeletedCases_WithValidParameters_ReturnsDataSet()
        {
            // Arrange
            var dataSet = new DataSet();
            _mockDao.Setup(dao => dao.GetUndeletedCases(
                It.IsAny<string>(), It.IsAny<int>(), It.IsAny<byte>(), 
                It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(dataSet);

            // Act
            var result = _mockDao.Object.GetUndeletedCases("123456789", 1, 1, "01", 1, false, false);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetUndeletedCases(
                It.IsAny<string>(), It.IsAny<int>(), It.IsAny<byte>(), 
                It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Once);
        }

        #endregion
    }
}
