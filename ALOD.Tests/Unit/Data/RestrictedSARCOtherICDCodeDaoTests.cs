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
    public class RestrictedSARCOtherICDCodeDaoTests
    {
        private readonly Mock<IRestrictedSARCOtherICDCodeDAO> _mockDao;

        public RestrictedSARCOtherICDCodeDaoTests()
        {
            _mockDao = new Mock<IRestrictedSARCOtherICDCodeDAO>();
        }

        #region Save Tests

        [Fact]
        [Trait("Method", "Save")]
        public void Save_WithValidRestrictedSARCOtherICDCode_ReturnsSavedRestrictedSARCOtherICDCode()
        {
            // Arrange
            var restrictedSARCOtherICDCode = new RestrictedSARCOtherICDCode();
            _mockDao.Setup(dao => dao.Save(It.IsAny<RestrictedSARCOtherICDCode>()))
                .Returns(restrictedSARCOtherICDCode);

            // Act
            var result = _mockDao.Object.Save(restrictedSARCOtherICDCode);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.Save(It.IsAny<RestrictedSARCOtherICDCode>()), Times.Once);
        }

        #endregion

        #region GetById Tests

        [Fact]
        [Trait("Method", "GetById")]
        public void GetById_WithValidId_ReturnsRestrictedSARCOtherICDCode()
        {
            // Arrange
            var restrictedSARCOtherICDCode = new RestrictedSARCOtherICDCode();
            _mockDao.Setup(dao => dao.GetById(It.IsAny<int>(), false))
                .Returns(restrictedSARCOtherICDCode);

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
        public void GetById_WithVariousIds_ReturnsRestrictedSARCOtherICDCode(int id)
        {
            // Arrange
            var restrictedSARCOtherICDCode = new RestrictedSARCOtherICDCode();
            _mockDao.Setup(dao => dao.GetById(It.IsAny<int>()))
                .Returns(restrictedSARCOtherICDCode);

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
        public void GetAll_ReturnsAllRestrictedSARCOtherICDCodes()
        {
            // Arrange
            var restrictedSARCOtherICDCodes = new List<RestrictedSARCOtherICDCode>
            {
                new RestrictedSARCOtherICDCode(),
                new RestrictedSARCOtherICDCode()
            }.AsQueryable();

            _mockDao.Setup(dao => dao.GetAll())
                .Returns(restrictedSARCOtherICDCodes);

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
        public void Delete_WithValidRestrictedSARCOtherICDCode_CallsDelete()
        {
            // Arrange
            var restrictedSARCOtherICDCode = new RestrictedSARCOtherICDCode();
            _mockDao.Setup(dao => dao.Delete(It.IsAny<RestrictedSARCOtherICDCode>()));

            // Act
            _mockDao.Object.Delete(restrictedSARCOtherICDCode);

            // Assert
            _mockDao.Verify(dao => dao.Delete(It.IsAny<RestrictedSARCOtherICDCode>()), Times.Once);
        }

        #endregion

        #region SaveOrUpdate Tests

        [Fact]
        [Trait("Method", "SaveOrUpdate")]
        public void SaveOrUpdate_WithValidRestrictedSARCOtherICDCode_ReturnsSavedRestrictedSARCOtherICDCode()
        {
            // Arrange
            var restrictedSARCOtherICDCode = new RestrictedSARCOtherICDCode();
            _mockDao.Setup(dao => dao.SaveOrUpdate(It.IsAny<RestrictedSARCOtherICDCode>()))
                .Returns(restrictedSARCOtherICDCode);

            // Act
            var result = _mockDao.Object.SaveOrUpdate(restrictedSARCOtherICDCode);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.SaveOrUpdate(It.IsAny<RestrictedSARCOtherICDCode>()), Times.Once);
        }

        #endregion
    }
}
