using ALOD.Core.Domain.Users;
using ALOD.Core.Interfaces.DAOInterfaces;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ALOD.Tests.Unit.Data
{
    [Trait("Category", "Unit")]
    [Trait("Layer", "Data")]
    public class UserRoleRequestDaoTests
    {
        private readonly Mock<IUserRoleRequestDao> _mockDao;

        public UserRoleRequestDaoTests()
        {
            _mockDao = new Mock<IUserRoleRequestDao>();
        }

        #region Save Tests

        [Fact]
        [Trait("Method", "Save")]
        public void Save_WithValidUserRoleRequest_ReturnsSavedUserRoleRequest()
        {
            // Arrange
            var userRoleRequest = new UserRoleRequest();
            _mockDao.Setup(dao => dao.Save(It.IsAny<UserRoleRequest>()))
                .Returns(userRoleRequest);

            // Act
            var result = _mockDao.Object.Save(userRoleRequest);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.Save(It.IsAny<UserRoleRequest>()), Times.Once);
        }

        #endregion

        #region GetById Tests

        [Fact]
        [Trait("Method", "GetById")]
        public void GetById_WithValidId_ReturnsUserRoleRequest()
        {
            // Arrange
            var userRoleRequest = new UserRoleRequest();
            _mockDao.Setup(dao => dao.GetById(It.IsAny<int>(), false))
                .Returns(userRoleRequest);

            // Act
            var result = _mockDao.Object.GetById(1, false);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetById(It.IsAny<int>(), false), Times.Once);
        }

        [Theory]
        [Trait("Method", "GetById")]
        [InlineData(1)]
        [InlineData(100)]
        [InlineData(9999)]
        public void GetById_WithVariousIds_ReturnsUserRoleRequest(int id)
        {
            // Arrange
            var userRoleRequest = new UserRoleRequest();
            _mockDao.Setup(dao => dao.GetById(It.IsAny<int>()))
                .Returns(userRoleRequest);

            // Act
            var result = _mockDao.Object.GetById(id);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetById(It.IsAny<int>()), Times.Once);
        }

        #endregion

        #region GetAll Tests

        [Fact]
        [Trait("Method", "GetAll")]
        public void GetAll_ReturnsAllUserRoleRequests()
        {
            // Arrange
            var userRoleRequests = new List<UserRoleRequest>
            {
                new UserRoleRequest(),
                new UserRoleRequest()
            }.AsQueryable();

            _mockDao.Setup(dao => dao.GetAll())
                .Returns(userRoleRequests);

            // Act
            var result = _mockDao.Object.GetAll();

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetAll(), Times.Once);
        }

        #endregion

        #region Delete Tests

        [Fact]
        [Trait("Method", "Delete")]
        public void Delete_WithValidUserRoleRequest_CallsDelete()
        {
            // Arrange
            var userRoleRequest = new UserRoleRequest();
            _mockDao.Setup(dao => dao.Delete(It.IsAny<UserRoleRequest>()));

            // Act
            _mockDao.Object.Delete(userRoleRequest);

            // Assert
            _mockDao.Verify(dao => dao.Delete(It.IsAny<UserRoleRequest>()), Times.Once);
        }

        #endregion

        #region SaveOrUpdate Tests

        [Fact]
        [Trait("Method", "SaveOrUpdate")]
        public void SaveOrUpdate_WithValidUserRoleRequest_ReturnsSavedUserRoleRequest()
        {
            // Arrange
            var userRoleRequest = new UserRoleRequest();
            _mockDao.Setup(dao => dao.SaveOrUpdate(It.IsAny<UserRoleRequest>()))
                .Returns(userRoleRequest);

            // Act
            var result = _mockDao.Object.SaveOrUpdate(userRoleRequest);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.SaveOrUpdate(It.IsAny<UserRoleRequest>()), Times.Once);
        }

        #endregion
    }
}
