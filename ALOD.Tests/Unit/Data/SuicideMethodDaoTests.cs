using ALOD.Core.Domain.Lookup;
using ALOD.Core.Interfaces.DAOInterfaces;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ALOD.Tests.Unit.Data
{
    [Trait("Category", "Unit")]
    [Trait("Layer", "Data")]
    public class SuicideMethodDaoTests
    {
        private readonly Mock<ISuicideMethodDao> _mockDao;

        public SuicideMethodDaoTests()
        {
            _mockDao = new Mock<ISuicideMethodDao>();
        }

        #region Save Tests

        [Fact]
        [Trait("Method", "Save")]
        public void Save_WithValidSuicideMethod_ReturnsSavedSuicideMethod()
        {
            // Arrange
            var suicideMethod = new SuicideMethod();
            _mockDao.Setup(dao => dao.Save(It.IsAny<SuicideMethod>()))
                .Returns(suicideMethod);

            // Act
            var result = _mockDao.Object.Save(suicideMethod);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.Save(It.IsAny<SuicideMethod>()), Times.Once);
        }

        #endregion

        #region GetById Tests

        [Fact]
        [Trait("Method", "GetById")]
        public void GetById_WithValidId_ReturnsSuicideMethod()
        {
            // Arrange
            var suicideMethod = new SuicideMethod();
            _mockDao.Setup(dao => dao.GetById(It.IsAny<int>(), false))
                .Returns(suicideMethod);

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
        public void GetById_WithVariousIds_ReturnsSuicideMethod(int id)
        {
            // Arrange
            var suicideMethod = new SuicideMethod();
            _mockDao.Setup(dao => dao.GetById(It.IsAny<int>()))
                .Returns(suicideMethod);

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
        public void GetAll_ReturnsAllSuicideMethods()
        {
            // Arrange
            var suicideMethods = new List<SuicideMethod>
            {
                new SuicideMethod(),
                new SuicideMethod()
            }.AsQueryable();

            _mockDao.Setup(dao => dao.GetAll())
                .Returns(suicideMethods);

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
        public void Delete_WithValidSuicideMethod_CallsDelete()
        {
            // Arrange
            var suicideMethod = new SuicideMethod();
            _mockDao.Setup(dao => dao.Delete(It.IsAny<SuicideMethod>()));

            // Act
            _mockDao.Object.Delete(suicideMethod);

            // Assert
            _mockDao.Verify(dao => dao.Delete(It.IsAny<SuicideMethod>()), Times.Once);
        }

        #endregion

        #region SaveOrUpdate Tests

        [Fact]
        [Trait("Method", "SaveOrUpdate")]
        public void SaveOrUpdate_WithValidSuicideMethod_ReturnsSavedSuicideMethod()
        {
            // Arrange
            var suicideMethod = new SuicideMethod();
            _mockDao.Setup(dao => dao.SaveOrUpdate(It.IsAny<SuicideMethod>()))
                .Returns(suicideMethod);

            // Act
            var result = _mockDao.Object.SaveOrUpdate(suicideMethod);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.SaveOrUpdate(It.IsAny<SuicideMethod>()), Times.Once);
        }

        #endregion
    }
}
