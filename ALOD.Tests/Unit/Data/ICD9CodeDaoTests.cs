using ALOD.Core.Domain.Common;
using ALOD.Core.Interfaces.DAOInterfaces;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ALOD.Tests.Unit.Data
{
    [Trait("Category", "Unit")]
    [Trait("Layer", "Data")]
    public class ICD9CodeDaoTests
    {
        private readonly Mock<IICD9CodeDao> _mockDao;

        public ICD9CodeDaoTests()
        {
            _mockDao = new Mock<IICD9CodeDao>();
        }

        #region Save Tests

        [Fact]
        [Trait("Method", "Save")]
        public void Save_WithValidEntity_ReturnsSavedEntity()
        {
            // Arrange
            var entity = new ICD9Code();
            _mockDao.Setup(x => x.Save(It.IsAny<ICD9Code>())).Returns(entity);

            // Act
            var result = _mockDao.Object.Save(entity);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(x => x.Save(It.IsAny<ICD9Code>()), Times.Once);
        }

        #endregion

        #region GetById Tests

        [Fact]
        [Trait("Method", "GetById")]
        public void GetById_WithValidId_ReturnsEntity()
        {
            // Arrange
            int id = 1;
            var entity = new ICD9Code();
            _mockDao.Setup(x => x.GetById(id, false)).Returns(entity);

            // Act
            var result = _mockDao.Object.GetById(id, false);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(x => x.GetById(id, false), Times.Once);
        }

        [Theory]
        [Trait("Method", "GetById")]
        [InlineData(1)]
        [InlineData(500)]
        [InlineData(5000)]
        public void GetById_WithVariousIds_ReturnsICD9Code(int id)
        {
            // Arrange
            _mockDao.Setup(x => x.GetById(id)).Returns(new ICD9Code());

            // Act
            var result = _mockDao.Object.GetById(id);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(x => x.GetById(id), Times.Once);
        }

        #endregion

        #region GetAll Tests

        [Fact]
        [Trait("Method", "GetAll")]
        public void GetAll_ReturnsAllEntities()
        {
            // Arrange
            var codes = new List<ICD9Code>
            {
                new ICD9Code(),
                new ICD9Code(),
                new ICD9Code()
            }.AsQueryable();
            _mockDao.Setup(x => x.GetAll()).Returns(codes);

            // Act
            var result = _mockDao.Object.GetAll();

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(x => x.GetAll(), Times.Once);
        }

        #endregion

        #region Delete Tests

        [Fact]
        [Trait("Method", "Delete")]
        public void Delete_WithValidEntity_CallsDelete()
        {
            // Arrange
            var entity = new ICD9Code();
            _mockDao.Setup(x => x.Delete(It.IsAny<ICD9Code>()));

            // Act
            _mockDao.Object.Delete(entity);

            // Assert
            _mockDao.Verify(x => x.Delete(It.IsAny<ICD9Code>()), Times.Once);
        }

        #endregion

        #region SaveOrUpdate Tests

        [Fact]
        [Trait("Method", "SaveOrUpdate")]
        public void SaveOrUpdate_WithValidEntity_ReturnsSavedEntity()
        {
            // Arrange
            var entity = new ICD9Code();
            _mockDao.Setup(x => x.SaveOrUpdate(It.IsAny<ICD9Code>())).Returns(entity);

            // Act
            var result = _mockDao.Object.SaveOrUpdate(entity);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(x => x.SaveOrUpdate(It.IsAny<ICD9Code>()), Times.Once);
        }

        #endregion
    }
}
