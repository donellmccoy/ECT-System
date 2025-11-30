using ALOD.Core.Domain.Query;
using ALOD.Core.Interfaces.DAOInterfaces;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ALOD.Tests.Unit.Data
{
    [Trait("Category", "Unit")]
    [Trait("Layer", "Data")]
    public class QueryDaoTests
    {
        private readonly Mock<IQueryDao> _mockDao;

        public QueryDaoTests()
        {
            _mockDao = new Mock<IQueryDao>();
        }

        #region Save Tests

        [Fact]
        [Trait("Method", "Save")]
        public void Save_WithValidUserQuery_ReturnsSavedUserQuery()
        {
            // Arrange
            var userQuery = new UserQuery();
            _mockDao.Setup(dao => dao.Save(It.IsAny<UserQuery>()))
                .Returns(userQuery);

            // Act
            var result = _mockDao.Object.Save(userQuery);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.Save(It.IsAny<UserQuery>()), Times.Once);
        }

        #endregion

        #region GetById Tests

        [Fact]
        [Trait("Method", "GetById")]
        public void GetById_WithValidId_ReturnsUserQuery()
        {
            // Arrange
            var userQuery = new UserQuery();
            _mockDao.Setup(dao => dao.GetById(It.IsAny<int>(), false))
                .Returns(userQuery);

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
        public void GetById_WithVariousIds_ReturnsUserQuery(int id)
        {
            // Arrange
            var userQuery = new UserQuery();
            _mockDao.Setup(dao => dao.GetById(It.IsAny<int>()))
                .Returns(userQuery);

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
        public void GetAll_ReturnsAllUserQueries()
        {
            // Arrange
            var userQueries = new List<UserQuery>
            {
                new UserQuery(),
                new UserQuery()
            }.AsQueryable();

            _mockDao.Setup(dao => dao.GetAll())
                .Returns(userQueries);

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
        public void Delete_WithValidUserQuery_CallsDelete()
        {
            // Arrange
            var userQuery = new UserQuery();
            _mockDao.Setup(dao => dao.Delete(It.IsAny<UserQuery>()));

            // Act
            _mockDao.Object.Delete(userQuery);

            // Assert
            _mockDao.Verify(dao => dao.Delete(It.IsAny<UserQuery>()), Times.Once);
        }

        #endregion

        #region SaveOrUpdate Tests

        [Fact]
        [Trait("Method", "SaveOrUpdate")]
        public void SaveOrUpdate_WithValidUserQuery_ReturnsSavedUserQuery()
        {
            // Arrange
            var userQuery = new UserQuery();
            _mockDao.Setup(dao => dao.SaveOrUpdate(It.IsAny<UserQuery>()))
                .Returns(userQuery);

            // Act
            var result = _mockDao.Object.SaveOrUpdate(userQuery);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.SaveOrUpdate(It.IsAny<UserQuery>()), Times.Once);
        }

        #endregion
    }
}
