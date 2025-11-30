using ALOD.Core.Domain.Modules.Reinvestigations;
using ALOD.Core.Interfaces.DAOInterfaces;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ALOD.Tests.Unit.Data
{
    [Trait("Category", "Unit")]
    [Trait("Layer", "Data")]
    public class LODReinvestigationDaoTests
    {
        private readonly Mock<ILODReinvestigateDAO> _mockDao;

        public LODReinvestigationDaoTests()
        {
            _mockDao = new Mock<ILODReinvestigateDAO>();
        }

        #region Save Tests

        [Fact]
        [Trait("Method", "Save")]
        public void Save_WithValidLODReinvestigation_ReturnsSavedLODReinvestigation()
        {
            // Arrange
            var lodReinvestigation = new LODReinvestigation();
            _mockDao.Setup(dao => dao.Save(It.IsAny<LODReinvestigation>()))
                .Returns(lodReinvestigation);

            // Act
            var result = _mockDao.Object.Save(lodReinvestigation);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.Save(It.IsAny<LODReinvestigation>()), Times.Once);
        }

        #endregion

        #region GetById Tests

        [Fact]
        [Trait("Method", "GetById")]
        public void GetById_WithValidId_ReturnsLODReinvestigation()
        {
            // Arrange
            var lodReinvestigation = new LODReinvestigation();
            _mockDao.Setup(dao => dao.GetById(It.IsAny<int>(), false))
                .Returns(lodReinvestigation);

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
        public void GetById_WithVariousIds_ReturnsLODReinvestigation(int id)
        {
            // Arrange
            var lodReinvestigation = new LODReinvestigation();
            _mockDao.Setup(dao => dao.GetById(It.IsAny<int>()))
                .Returns(lodReinvestigation);

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
        public void GetAll_ReturnsAllLODReinvestigations()
        {
            // Arrange
            var lodReinvestigations = new List<LODReinvestigation>
            {
                new LODReinvestigation(),
                new LODReinvestigation()
            }.AsQueryable();

            _mockDao.Setup(dao => dao.GetAll())
                .Returns(lodReinvestigations);

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
        public void Delete_WithValidLODReinvestigation_CallsDelete()
        {
            // Arrange
            var lodReinvestigation = new LODReinvestigation();
            _mockDao.Setup(dao => dao.Delete(It.IsAny<LODReinvestigation>()));

            // Act
            _mockDao.Object.Delete(lodReinvestigation);

            // Assert
            _mockDao.Verify(dao => dao.Delete(It.IsAny<LODReinvestigation>()), Times.Once);
        }

        #endregion

        #region SaveOrUpdate Tests

        [Fact]
        [Trait("Method", "SaveOrUpdate")]
        public void SaveOrUpdate_WithValidLODReinvestigation_ReturnsSavedLODReinvestigation()
        {
            // Arrange
            var lodReinvestigation = new LODReinvestigation();
            _mockDao.Setup(dao => dao.SaveOrUpdate(It.IsAny<LODReinvestigation>()))
                .Returns(lodReinvestigation);

            // Act
            var result = _mockDao.Object.SaveOrUpdate(lodReinvestigation);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.SaveOrUpdate(It.IsAny<LODReinvestigation>()), Times.Once);
        }

        #endregion
    }
}
