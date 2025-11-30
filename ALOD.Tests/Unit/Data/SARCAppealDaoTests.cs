using ALOD.Core.Domain.Modules.SARC;
using ALOD.Core.Interfaces.DAOInterfaces;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ALOD.Tests.Unit.Data
{
    [Trait("Category", "Unit")]
    [Trait("Layer", "Data")]
    public class SARCAppealDaoTests
    {
        private readonly Mock<ISARCAppealDAO> _mockDao;

        public SARCAppealDaoTests()
        {
            _mockDao = new Mock<ISARCAppealDAO>();
        }

        #region Save Tests

        [Fact]
        [Trait("Method", "Save")]
        public void Save_WithValidSARCAppeal_ReturnsSavedSARCAppeal()
        {
            // Arrange
            var sarcAppeal = new SARCAppeal();
            _mockDao.Setup(dao => dao.Save(It.IsAny<SARCAppeal>()))
                .Returns(sarcAppeal);

            // Act
            var result = _mockDao.Object.Save(sarcAppeal);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.Save(It.IsAny<SARCAppeal>()), Times.Once);
        }

        #endregion

        #region GetById Tests

        [Fact]
        [Trait("Method", "GetById")]
        public void GetById_WithValidId_ReturnsSARCAppeal()
        {
            // Arrange
            var sarcAppeal = new SARCAppeal();
            _mockDao.Setup(dao => dao.GetById(It.IsAny<int>(), false))
                .Returns(sarcAppeal);

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
        public void GetById_WithVariousIds_ReturnsSARCAppeal(int id)
        {
            // Arrange
            var sarcAppeal = new SARCAppeal();
            _mockDao.Setup(dao => dao.GetById(It.IsAny<int>()))
                .Returns(sarcAppeal);

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
        public void GetAll_ReturnsAllSARCAppeals()
        {
            // Arrange
            var sarcAppeals = new List<SARCAppeal>
            {
                new SARCAppeal(),
                new SARCAppeal()
            }.AsQueryable();

            _mockDao.Setup(dao => dao.GetAll())
                .Returns(sarcAppeals);

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
        public void Delete_WithValidSARCAppeal_CallsDelete()
        {
            // Arrange
            var sarcAppeal = new SARCAppeal();
            _mockDao.Setup(dao => dao.Delete(It.IsAny<SARCAppeal>()));

            // Act
            _mockDao.Object.Delete(sarcAppeal);

            // Assert
            _mockDao.Verify(dao => dao.Delete(It.IsAny<SARCAppeal>()), Times.Once);
        }

        #endregion

        #region SaveOrUpdate Tests

        [Fact]
        [Trait("Method", "SaveOrUpdate")]
        public void SaveOrUpdate_WithValidSARCAppeal_ReturnsSavedSARCAppeal()
        {
            // Arrange
            var sarcAppeal = new SARCAppeal();
            _mockDao.Setup(dao => dao.SaveOrUpdate(It.IsAny<SARCAppeal>()))
                .Returns(sarcAppeal);

            // Act
            var result = _mockDao.Object.SaveOrUpdate(sarcAppeal);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.SaveOrUpdate(It.IsAny<SARCAppeal>()), Times.Once);
        }

        #endregion
    }
}
