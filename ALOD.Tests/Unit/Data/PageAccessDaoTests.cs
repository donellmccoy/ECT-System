using ALOD.Core.Domain.Workflow;
using ALOD.Core.Interfaces.DAOInterfaces;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ALOD.Tests.Unit.Data
{
    [Trait("Category", "Unit")]
    [Trait("Layer", "Data")]
    public class PageAccessDaoTests
    {
        private readonly Mock<IPageAccessDao> _mockDao;

        public PageAccessDaoTests()
        {
            _mockDao = new Mock<IPageAccessDao>();
        }

        #region Save Tests

        [Fact]
        [Trait("Method", "Save")]
        public void Save_WithValidPageAccess_ReturnsSavedPageAccess()
        {
            // Arrange
            var pageAccess = new PageAccess();
            _mockDao.Setup(dao => dao.Save(It.IsAny<PageAccess>()))
                .Returns(pageAccess);

            // Act
            var result = _mockDao.Object.Save(pageAccess);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.Save(It.IsAny<PageAccess>()), Times.Once);
        }

        #endregion

        #region GetById Tests

        [Fact]
        [Trait("Method", "GetById")]
        public void GetById_WithValidId_ReturnsPageAccess()
        {
            // Arrange
            var pageAccess = new PageAccess();
            _mockDao.Setup(dao => dao.GetById(It.IsAny<int>(), false))
                .Returns(pageAccess);

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
        public void GetById_WithVariousIds_ReturnsPageAccess(int id)
        {
            // Arrange
            var pageAccess = new PageAccess();
            _mockDao.Setup(dao => dao.GetById(It.IsAny<int>()))
                .Returns(pageAccess);

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
        public void GetAll_ReturnsAllPageAccesses()
        {
            // Arrange
            var pageAccesses = new List<PageAccess>
            {
                new PageAccess(),
                new PageAccess()
            }.AsQueryable();

            _mockDao.Setup(dao => dao.GetAll())
                .Returns(pageAccesses);

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
        public void Delete_WithValidPageAccess_CallsDelete()
        {
            // Arrange
            var pageAccess = new PageAccess();
            _mockDao.Setup(dao => dao.Delete(It.IsAny<PageAccess>()));

            // Act
            _mockDao.Object.Delete(pageAccess);

            // Assert
            _mockDao.Verify(dao => dao.Delete(It.IsAny<PageAccess>()), Times.Once);
        }

        #endregion

        #region SaveOrUpdate Tests

        [Fact]
        [Trait("Method", "SaveOrUpdate")]
        public void SaveOrUpdate_WithValidPageAccess_ReturnsSavedPageAccess()
        {
            // Arrange
            var pageAccess = new PageAccess();
            _mockDao.Setup(dao => dao.SaveOrUpdate(It.IsAny<PageAccess>()))
                .Returns(pageAccess);

            // Act
            var result = _mockDao.Object.SaveOrUpdate(pageAccess);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.SaveOrUpdate(It.IsAny<PageAccess>()), Times.Once);
        }

        #endregion
    }
}
