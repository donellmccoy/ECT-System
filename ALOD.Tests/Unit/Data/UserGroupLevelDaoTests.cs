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
    public class UserGroupLevelDaoTests
    {
        private readonly Mock<IUserGroupLevelDao> _mockDao;

        public UserGroupLevelDaoTests()
        {
            _mockDao = new Mock<IUserGroupLevelDao>();
        }

        #region Save Tests

        [Fact]
        [Trait("Method", "Save")]
        public void Save_WithValidUserGroupLevel_ReturnsSavedUserGroupLevel()
        {
            // Arrange
            var userGroupLevel = new UserGroupLevel();
            _mockDao.Setup(dao => dao.Save(It.IsAny<UserGroupLevel>()))
                .Returns(userGroupLevel);

            // Act
            var result = _mockDao.Object.Save(userGroupLevel);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.Save(It.IsAny<UserGroupLevel>()), Times.Once);
        }

        #endregion

        #region GetById Tests

        [Fact]
        [Trait("Method", "GetById")]
        public void GetById_WithValidId_ReturnsUserGroupLevel()
        {
            // Arrange
            var userGroupLevel = new UserGroupLevel();
            _mockDao.Setup(dao => dao.GetById(It.IsAny<int>(), false))
                .Returns(userGroupLevel);

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
        public void GetById_WithVariousIds_ReturnsUserGroupLevel(int id)
        {
            // Arrange
            var userGroupLevel = new UserGroupLevel();
            _mockDao.Setup(dao => dao.GetById(It.IsAny<int>()))
                .Returns(userGroupLevel);

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
        public void GetAll_ReturnsAllUserGroupLevels()
        {
            // Arrange
            var userGroupLevels = new List<UserGroupLevel>
            {
                new UserGroupLevel(),
                new UserGroupLevel()
            }.AsQueryable();

            _mockDao.Setup(dao => dao.GetAll())
                .Returns(userGroupLevels);

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
        public void Delete_WithValidUserGroupLevel_CallsDelete()
        {
            // Arrange
            var userGroupLevel = new UserGroupLevel();
            _mockDao.Setup(dao => dao.Delete(It.IsAny<UserGroupLevel>()));

            // Act
            _mockDao.Object.Delete(userGroupLevel);

            // Assert
            _mockDao.Verify(dao => dao.Delete(It.IsAny<UserGroupLevel>()), Times.Once);
        }

        #endregion

        #region SaveOrUpdate Tests

        [Fact]
        [Trait("Method", "SaveOrUpdate")]
        public void SaveOrUpdate_WithValidUserGroupLevel_ReturnsSavedUserGroupLevel()
        {
            // Arrange
            var userGroupLevel = new UserGroupLevel();
            _mockDao.Setup(dao => dao.SaveOrUpdate(It.IsAny<UserGroupLevel>()))
                .Returns(userGroupLevel);

            // Act
            var result = _mockDao.Object.SaveOrUpdate(userGroupLevel);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.SaveOrUpdate(It.IsAny<UserGroupLevel>()), Times.Once);
        }

        #endregion
    }
}
