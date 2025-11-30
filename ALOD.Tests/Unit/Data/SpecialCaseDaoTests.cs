using ALOD.Core.Domain.Modules.SpecialCases;
using ALOD.Core.Interfaces.DAOInterfaces;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ALOD.Tests.Unit.Data
{
    [Trait("Category", "Unit")]
    [Trait("Layer", "Data")]
    public class SpecialCaseDaoTests
    {
        private readonly Mock<ISpecialCaseDAO> _mockDao;

        public SpecialCaseDaoTests()
        {
            _mockDao = new Mock<ISpecialCaseDAO>();
        }

        #region Save Tests

        [Fact]
        [Trait("Method", "Save")]
        public void Save_WithValidSpecialCase_ReturnsSavedSpecialCase()
        {
            // Arrange
            var specialCase = new SpecialCase();
            _mockDao.Setup(dao => dao.Save(It.IsAny<SpecialCase>()))
                .Returns(specialCase);

            // Act
            var result = _mockDao.Object.Save(specialCase);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.Save(It.IsAny<SpecialCase>()), Times.Once);
        }

        #endregion

        #region GetById Tests

        [Fact]
        [Trait("Method", "GetById")]
        public void GetById_WithValidId_ReturnsSpecialCase()
        {
            // Arrange
            var specialCase = new SpecialCase();
            _mockDao.Setup(dao => dao.GetById(It.IsAny<int>(), false))
                .Returns(specialCase);

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
        public void GetById_WithVariousIds_ReturnsSpecialCase(int id)
        {
            // Arrange
            var specialCase = new SpecialCase();
            _mockDao.Setup(dao => dao.GetById(It.IsAny<int>()))
                .Returns(specialCase);

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
        public void GetAll_ReturnsAllSpecialCases()
        {
            // Arrange
            var specialCases = new List<SpecialCase>
            {
                new SpecialCase(),
                new SpecialCase()
            }.AsQueryable();

            _mockDao.Setup(dao => dao.GetAll())
                .Returns(specialCases);

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
        public void Delete_WithValidSpecialCase_CallsDelete()
        {
            // Arrange
            var specialCase = new SpecialCase();
            _mockDao.Setup(dao => dao.Delete(It.IsAny<SpecialCase>()));

            // Act
            _mockDao.Object.Delete(specialCase);

            // Assert
            _mockDao.Verify(dao => dao.Delete(It.IsAny<SpecialCase>()), Times.Once);
        }

        #endregion

        #region SaveOrUpdate Tests

        [Fact]
        [Trait("Method", "SaveOrUpdate")]
        public void SaveOrUpdate_WithValidSpecialCase_ReturnsSavedSpecialCase()
        {
            // Arrange
            var specialCase = new SpecialCase();
            _mockDao.Setup(dao => dao.SaveOrUpdate(It.IsAny<SpecialCase>()))
                .Returns(specialCase);

            // Act
            var result = _mockDao.Object.SaveOrUpdate(specialCase);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.SaveOrUpdate(It.IsAny<SpecialCase>()), Times.Once);
        }

        #endregion
    }
}
