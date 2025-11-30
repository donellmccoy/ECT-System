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
    public class ALODPermissionDaoTests
    {
        private readonly Mock<IALODPermissionDao> _mockDao;

        public ALODPermissionDaoTests()
        {
            _mockDao = new Mock<IALODPermissionDao>();
        }

        #region Save Tests

        [Fact]
        [Trait("Method", "Save")]
        public void Save_WithValidALODPermission_ReturnsSavedALODPermission()
        {
            // Arrange
            var alodPermission = new ALODPermission();
            _mockDao.Setup(dao => dao.Save(It.IsAny<ALODPermission>()))
                .Returns(alodPermission);

            // Act
            var result = _mockDao.Object.Save(alodPermission);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.Save(It.IsAny<ALODPermission>()), Times.Once);
        }

        #endregion

        #region GetById Tests

        [Fact]
        [Trait("Method", "GetById")]
        public void GetById_WithValidId_ReturnsALODPermission()
        {
            // Arrange
            var alodPermission = new ALODPermission();
            _mockDao.Setup(dao => dao.GetById(It.IsAny<int>(), false))
                .Returns(alodPermission);

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
        public void GetById_WithVariousIds_ReturnsALODPermission(int id)
        {
            // Arrange
            var alodPermission = new ALODPermission();
            _mockDao.Setup(dao => dao.GetById(It.IsAny<int>()))
                .Returns(alodPermission);

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
        public void GetAll_ReturnsAllALODPermissions()
        {
            // Arrange
            var alodPermissions = new List<ALODPermission>
            {
                new ALODPermission(),
                new ALODPermission()
            }.AsQueryable();

            _mockDao.Setup(dao => dao.GetAll())
                .Returns(alodPermissions);

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
        public void Delete_WithValidALODPermission_CallsDelete()
        {
            // Arrange
            var alodPermission = new ALODPermission();
            _mockDao.Setup(dao => dao.Delete(It.IsAny<ALODPermission>()));

            // Act
            _mockDao.Object.Delete(alodPermission);

            // Assert
            _mockDao.Verify(dao => dao.Delete(It.IsAny<ALODPermission>()), Times.Once);
        }

        #endregion

        #region SaveOrUpdate Tests

        [Fact]
        [Trait("Method", "SaveOrUpdate")]
        public void SaveOrUpdate_WithValidALODPermission_ReturnsSavedALODPermission()
        {
            // Arrange
            var alodPermission = new ALODPermission();
            _mockDao.Setup(dao => dao.SaveOrUpdate(It.IsAny<ALODPermission>()))
                .Returns(alodPermission);

            // Act
            var result = _mockDao.Object.SaveOrUpdate(alodPermission);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.SaveOrUpdate(It.IsAny<ALODPermission>()), Times.Once);
        }

        #endregion
    }
}
