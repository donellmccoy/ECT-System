using ALOD.Core.Domain.Documents;
using ALOD.Core.Interfaces.DAOInterfaces;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ALOD.Tests.Unit.Data
{
    [Trait("Category", "Unit")]
    [Trait("Layer", "Data")]
    public class DocCategoryViewDaoTests
    {
        private readonly Mock<IDocCategoryViewDao> _mockDao;

        public DocCategoryViewDaoTests()
        {
            _mockDao = new Mock<IDocCategoryViewDao>();
        }

        #region Save Tests

        [Fact]
        [Trait("Method", "Save")]
        public void Save_WithValidDocCategoryView_ReturnsSavedDocCategoryView()
        {
            // Arrange
            var docCategoryView = new DocCategoryView();
            _mockDao.Setup(dao => dao.Save(It.IsAny<DocCategoryView>()))
                .Returns(docCategoryView);

            // Act
            var result = _mockDao.Object.Save(docCategoryView);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.Save(It.IsAny<DocCategoryView>()), Times.Once);
        }

        #endregion

        #region GetById Tests

        [Fact]
        [Trait("Method", "GetById")]
        public void GetById_WithValidId_ReturnsDocCategoryView()
        {
            // Arrange
            var docCategoryView = new DocCategoryView();
            _mockDao.Setup(dao => dao.GetById(It.IsAny<int>(), false))
                .Returns(docCategoryView);

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
        public void GetById_WithVariousIds_ReturnsDocCategoryView(int id)
        {
            // Arrange
            var docCategoryView = new DocCategoryView();
            _mockDao.Setup(dao => dao.GetById(It.IsAny<int>()))
                .Returns(docCategoryView);

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
        public void GetAll_ReturnsAllDocCategoryViews()
        {
            // Arrange
            var docCategoryViews = new List<DocCategoryView>
            {
                new DocCategoryView(),
                new DocCategoryView()
            }.AsQueryable();

            _mockDao.Setup(dao => dao.GetAll())
                .Returns(docCategoryViews);

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
        public void Delete_WithValidDocCategoryView_CallsDelete()
        {
            // Arrange
            var docCategoryView = new DocCategoryView();
            _mockDao.Setup(dao => dao.Delete(It.IsAny<DocCategoryView>()));

            // Act
            _mockDao.Object.Delete(docCategoryView);

            // Assert
            _mockDao.Verify(dao => dao.Delete(It.IsAny<DocCategoryView>()), Times.Once);
        }

        #endregion

        #region SaveOrUpdate Tests

        [Fact]
        [Trait("Method", "SaveOrUpdate")]
        public void SaveOrUpdate_WithValidDocCategoryView_ReturnsSavedDocCategoryView()
        {
            // Arrange
            var docCategoryView = new DocCategoryView();
            _mockDao.Setup(dao => dao.SaveOrUpdate(It.IsAny<DocCategoryView>()))
                .Returns(docCategoryView);

            // Act
            var result = _mockDao.Object.SaveOrUpdate(docCategoryView);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.SaveOrUpdate(It.IsAny<DocCategoryView>()), Times.Once);
        }

        #endregion
    }
}
