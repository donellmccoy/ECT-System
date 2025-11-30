using ALOD.Core.Domain.Workflow;
using ALOD.Core.Interfaces.DAOInterfaces;
using Moq;
using System.Collections.Generic;
using System.Data;
using Xunit;

namespace ALOD.Tests.Unit.Data
{
    [Trait("Category", "Unit")]
    [Trait("Layer", "Data")]
    public class CaseLockDaoTests
    {
        private readonly Mock<ICaseLockDao> _mockDao;

        public CaseLockDaoTests()
        {
            _mockDao = new Mock<ICaseLockDao>();
        }

        #region GetAll Tests

        [Fact]
        [Trait("Method", "GetAll")]
        public void GetAll_ReturnsDataSet()
        {
            // Arrange
            var dataSet = new DataSet();
            _mockDao.Setup(dao => dao.GetAll())
                .Returns(dataSet);

            // Act
            var result = _mockDao.Object.GetAll();

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetAll(), Times.Once);
        }

        #endregion

        #region GetByReferenceId Tests

        [Fact]
        [Trait("Method", "GetByReferenceId")]
        public void GetByReferenceId_WithValidParameters_ReturnsCaseLock()
        {
            // Arrange
            var caseLock = new CaseLock();
            _mockDao.Setup(dao => dao.GetByReferenceId(It.IsAny<int>(), It.IsAny<byte>()))
                .Returns(caseLock);

            // Act
            var result = _mockDao.Object.GetByReferenceId(1, 1);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetByReferenceId(It.IsAny<int>(), It.IsAny<byte>()), Times.Once);
        }

        #endregion

        #region GetByUserId Tests

        [Fact]
        [Trait("Method", "GetByUserId")]
        public void GetByUserId_WithValidUserId_ReturnsCaseLocksList()
        {
            // Arrange
            var caseLocks = new List<CaseLock> { new CaseLock() };
            _mockDao.Setup(dao => dao.GetByUserId(It.IsAny<int>()))
                .Returns(caseLocks);

            // Act
            var result = _mockDao.Object.GetByUserId(1);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetByUserId(It.IsAny<int>()), Times.Once);
        }

        #endregion

        #region ClearAllLocks Tests

        [Fact]
        [Trait("Method", "ClearAllLocks")]
        public void ClearAllLocks_CallsClearAllLocks()
        {
            // Arrange
            _mockDao.Setup(dao => dao.ClearAllLocks());

            // Act
            _mockDao.Object.ClearAllLocks();

            // Assert
            _mockDao.Verify(dao => dao.ClearAllLocks(), Times.Once);
        }

        #endregion

        #region ClearLocksForUser Tests

        [Fact]
        [Trait("Method", "ClearLocksForUser")]
        public void ClearLocksForUser_WithValidUserId_CallsClearLocksForUser()
        {
            // Arrange
            _mockDao.Setup(dao => dao.ClearLocksForUser(It.IsAny<int>()));

            // Act
            _mockDao.Object.ClearLocksForUser(1);

            // Assert
            _mockDao.Verify(dao => dao.ClearLocksForUser(It.IsAny<int>()), Times.Once);
        }

        #endregion
    }
}
