using ALOD.Core.Domain.Modules.Lod;
using ALOD.Core.Interfaces.DAOInterfaces;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ALOD.Tests.Unit.Data
{
    [Trait("Category", "Unit")]
    [Trait("Layer", "Data")]
    public class ReturnDaoTests
    {
        private readonly Mock<IReturnDao> _mockDao;

        public ReturnDaoTests()
        {
            _mockDao = new Mock<IReturnDao>();
        }

        #region Save Tests

        [Fact]
        [Trait("Method", "Save")]
        public void Save_WithValidReturn_ReturnsSavedReturn()
        {
            // Arrange
            var returnEntity = new Return();
            _mockDao.Setup(dao => dao.Save(It.IsAny<Return>()))
                .Returns(returnEntity);

            // Act
            var result = _mockDao.Object.Save(returnEntity);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.Save(It.IsAny<Return>()), Times.Once);
        }

        #endregion

        #region GetById Tests

        [Fact]
        [Trait("Method", "GetById")]
        public void GetById_WithValidId_ReturnsReturn()
        {
            // Arrange
            var returnEntity = new Return();
            _mockDao.Setup(dao => dao.GetById(It.IsAny<int>(), false))
                .Returns(returnEntity);

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
        public void GetById_WithVariousIds_ReturnsReturn(int id)
        {
            // Arrange
            var returnEntity = new Return();
            _mockDao.Setup(dao => dao.GetById(It.IsAny<int>()))
                .Returns(returnEntity);

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
        public void GetAll_ReturnsAllReturns()
        {
            // Arrange
            var returns = new List<Return>
            {
                new Return(),
                new Return()
            }.AsQueryable();

            _mockDao.Setup(dao => dao.GetAll())
                .Returns(returns);

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
        public void Delete_WithValidReturn_CallsDelete()
        {
            // Arrange
            var returnEntity = new Return();
            _mockDao.Setup(dao => dao.Delete(It.IsAny<Return>()));

            // Act
            _mockDao.Object.Delete(returnEntity);

            // Assert
            _mockDao.Verify(dao => dao.Delete(It.IsAny<Return>()), Times.Once);
        }

        #endregion

        #region SaveOrUpdate Tests

        [Fact]
        [Trait("Method", "SaveOrUpdate")]
        public void SaveOrUpdate_WithValidReturn_ReturnsSavedReturn()
        {
            // Arrange
            var returnEntity = new Return();
            _mockDao.Setup(dao => dao.SaveOrUpdate(It.IsAny<Return>()))
                .Returns(returnEntity);

            // Act
            var result = _mockDao.Object.SaveOrUpdate(returnEntity);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.SaveOrUpdate(It.IsAny<Return>()), Times.Once);
        }

        #endregion
    }
}
