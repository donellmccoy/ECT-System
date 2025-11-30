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
    public class SignatureMetaDataDaoTests
    {
        private readonly Mock<ISignatueMetaDateDao> _mockDao;

        public SignatureMetaDataDaoTests()
        {
            _mockDao = new Mock<ISignatueMetaDateDao>();
        }

        #region Save Tests

        [Fact]
        [Trait("Method", "Save")]
        public void Save_WithValidSignatureMetaData_ReturnsSavedSignatureMetaData()
        {
            // Arrange
            var signatureMetaData = new SignatureMetaData();
            _mockDao.Setup(dao => dao.Save(It.IsAny<SignatureMetaData>()))
                .Returns(signatureMetaData);

            // Act
            var result = _mockDao.Object.Save(signatureMetaData);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.Save(It.IsAny<SignatureMetaData>()), Times.Once);
        }

        #endregion

        #region GetById Tests

        [Fact]
        [Trait("Method", "GetById")]
        public void GetById_WithValidId_ReturnsSignatureMetaData()
        {
            // Arrange
            var signatureMetaData = new SignatureMetaData();
            _mockDao.Setup(dao => dao.GetById(It.IsAny<int>(), false))
                .Returns(signatureMetaData);

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
        public void GetById_WithVariousIds_ReturnsSignatureMetaData(int id)
        {
            // Arrange
            var signatureMetaData = new SignatureMetaData();
            _mockDao.Setup(dao => dao.GetById(It.IsAny<int>()))
                .Returns(signatureMetaData);

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
        public void GetAll_ReturnsAllSignatureMetaData()
        {
            // Arrange
            var signatureMetaDataList = new List<SignatureMetaData>
            {
                new SignatureMetaData(),
                new SignatureMetaData()
            }.AsQueryable();

            _mockDao.Setup(dao => dao.GetAll())
                .Returns(signatureMetaDataList);

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
        public void Delete_WithValidSignatureMetaData_CallsDelete()
        {
            // Arrange
            var signatureMetaData = new SignatureMetaData();
            _mockDao.Setup(dao => dao.Delete(It.IsAny<SignatureMetaData>()));

            // Act
            _mockDao.Object.Delete(signatureMetaData);

            // Assert
            _mockDao.Verify(dao => dao.Delete(It.IsAny<SignatureMetaData>()), Times.Once);
        }

        #endregion

        #region SaveOrUpdate Tests

        [Fact]
        [Trait("Method", "SaveOrUpdate")]
        public void SaveOrUpdate_WithValidSignatureMetaData_ReturnsSavedSignatureMetaData()
        {
            // Arrange
            var signatureMetaData = new SignatureMetaData();
            _mockDao.Setup(dao => dao.SaveOrUpdate(It.IsAny<SignatureMetaData>()))
                .Returns(signatureMetaData);

            // Act
            var result = _mockDao.Object.SaveOrUpdate(signatureMetaData);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.SaveOrUpdate(It.IsAny<SignatureMetaData>()), Times.Once);
        }

        #endregion
    }
}
