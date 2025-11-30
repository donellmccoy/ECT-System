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
    public class PHQueryDaoTests
    {
        private readonly Mock<IPHQueryDao> _mockDao;

        public PHQueryDaoTests()
        {
            _mockDao = new Mock<IPHQueryDao>();
        }

        #region Save Tests

        [Fact]
        [Trait("Method", "Save")]
        public void Save_WithValidPHUserQuery_ReturnsSavedPHUserQuery()
        {
            // Arrange
            var phUserQuery = new PHUserQuery();
            _mockDao.Setup(dao => dao.Save(It.IsAny<PHUserQuery>()))
                .Returns(phUserQuery);

            // Act
            var result = _mockDao.Object.Save(phUserQuery);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.Save(It.IsAny<PHUserQuery>()), Times.Once);
        }

        #endregion

        #region GetById Tests

        [Fact]
        [Trait("Method", "GetById")]
        public void GetById_WithValidId_ReturnsPHUserQuery()
        {
            // Arrange
            var phUserQuery = new PHUserQuery();
            _mockDao.Setup(dao => dao.GetById(It.IsAny<int>(), false))
                .Returns(phUserQuery);

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
        public void GetById_WithVariousIds_ReturnsPHUserQuery(int id)
        {
            // Arrange
            var phUserQuery = new PHUserQuery();
            _mockDao.Setup(dao => dao.GetById(It.IsAny<int>()))
                .Returns(phUserQuery);

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
        public void GetAll_ReturnsAllPHUserQueries()
        {
            // Arrange
            var phUserQueries = new List<PHUserQuery>
            {
                new PHUserQuery(),
                new PHUserQuery()
            }.AsQueryable();

            _mockDao.Setup(dao => dao.GetAll())
                .Returns(phUserQueries);

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
        public void Delete_WithValidPHUserQuery_CallsDelete()
        {
            // Arrange
            var phUserQuery = new PHUserQuery();
            _mockDao.Setup(dao => dao.Delete(It.IsAny<PHUserQuery>()));

            // Act
            _mockDao.Object.Delete(phUserQuery);

            // Assert
            _mockDao.Verify(dao => dao.Delete(It.IsAny<PHUserQuery>()), Times.Once);
        }

        #endregion

        #region SaveOrUpdate Tests

        [Fact]
        [Trait("Method", "SaveOrUpdate")]
        public void SaveOrUpdate_WithValidPHUserQuery_ReturnsSavedPHUserQuery()
        {
            // Arrange
            var phUserQuery = new PHUserQuery();
            _mockDao.Setup(dao => dao.SaveOrUpdate(It.IsAny<PHUserQuery>()))
                .Returns(phUserQuery);

            // Act
            var result = _mockDao.Object.SaveOrUpdate(phUserQuery);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.SaveOrUpdate(It.IsAny<PHUserQuery>()), Times.Once);
        }

        #endregion
    }
}
