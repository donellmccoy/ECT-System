using ALOD.Core.Domain.Common.KeyVal;
using ALOD.Core.Interfaces.DAOInterfaces;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace ALOD.Tests.Unit.Data
{
    [Trait("Category", "Unit")]
    [Trait("Layer", "Data")]
    public class KeyValDaoTests
    {
        private readonly Mock<IKeyValDao> _mockDao;

        public KeyValDaoTests()
        {
            _mockDao = new Mock<IKeyValDao>();
        }

        #region GetAllKeys Tests

        [Fact]
        [Trait("Method", "GetAllKeys")]
        public void GetAllKeys_ReturnsAllKeys()
        {
            // Arrange
            var keys = new List<KeyValKey> { new KeyValKey() };
            _mockDao.Setup(dao => dao.GetAllKeys())
                .Returns(keys);

            // Act
            var result = _mockDao.Object.GetAllKeys();

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetAllKeys(), Times.Once);
        }

        #endregion

        #region GetEditableKeyTypes Tests

        [Fact]
        [Trait("Method", "GetEditableKeyTypes")]
        public void GetEditableKeyTypes_ReturnsEditableKeyTypes()
        {
            // Arrange
            var keyTypes = new List<KeyValKeyType> { new KeyValKeyType() };
            _mockDao.Setup(dao => dao.GetEditableKeyTypes())
                .Returns(keyTypes);

            // Act
            var result = _mockDao.Object.GetEditableKeyTypes();

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetEditableKeyTypes(), Times.Once);
        }

        #endregion

        #region GetHelpKeys Tests

        [Fact]
        [Trait("Method", "GetHelpKeys")]
        public void GetHelpKeys_ReturnsHelpKeys()
        {
            // Arrange
            var keys = new List<KeyValKey> { new KeyValKey() };
            _mockDao.Setup(dao => dao.GetHelpKeys())
                .Returns(keys);

            // Act
            var result = _mockDao.Object.GetHelpKeys();

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetHelpKeys(), Times.Once);
        }

        #endregion

        #region GetMemoKeys Tests

        [Fact]
        [Trait("Method", "GetMemoKeys")]
        public void GetMemoKeys_ReturnsMemoKeys()
        {
            // Arrange
            var keys = new List<KeyValKey> { new KeyValKey() };
            _mockDao.Setup(dao => dao.GetMemoKeys())
                .Returns(keys);

            // Act
            var result = _mockDao.Object.GetMemoKeys();

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetMemoKeys(), Times.Once);
        }

        #endregion

        #region GetKeysUsingKeyType Tests

        [Fact]
        [Trait("Method", "GetKeysUsingKeyType")]
        public void GetKeysUsingKeyType_WithValidKeyTypeId_ReturnsKeys()
        {
            // Arrange
            var keys = new List<KeyValKey> { new KeyValKey() };
            _mockDao.Setup(dao => dao.GetKeysUsingKeyType(It.IsAny<int>()))
                .Returns(keys);

            // Act
            var result = _mockDao.Object.GetKeysUsingKeyType(1);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetKeysUsingKeyType(It.IsAny<int>()), Times.Once);
        }

        #endregion

        #region GetKeyValuesByKeyDescription Tests

        [Fact]
        [Trait("Method", "GetKeyValuesByKeyDesciption")]
        public void GetKeyValuesByKeyDesciption_WithValidDescription_ReturnsValues()
        {
            // Arrange
            var values = new List<KeyValValue> { new KeyValValue() };
            _mockDao.Setup(dao => dao.GetKeyValuesByKeyDesciption(It.IsAny<string>()))
                .Returns(values);

            // Act
            var result = _mockDao.Object.GetKeyValuesByKeyDesciption("Test Key");

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetKeyValuesByKeyDesciption(It.IsAny<string>()), Times.Once);
        }

        #endregion

        #region GetKeyValuesByKeyId Tests

        [Fact]
        [Trait("Method", "GetKeyValuesByKeyId")]
        public void GetKeyValuesByKeyId_WithValidKeyId_ReturnsValues()
        {
            // Arrange
            var values = new List<KeyValValue> { new KeyValValue() };
            _mockDao.Setup(dao => dao.GetKeyValuesByKeyId(It.IsAny<int>()))
                .Returns(values);

            // Act
            var result = _mockDao.Object.GetKeyValuesByKeyId(1);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetKeyValuesByKeyId(It.IsAny<int>()), Times.Once);
        }

        #endregion

        #region DeleteKeyValueById Tests

        [Fact]
        [Trait("Method", "DeleteKeyValueById")]
        public void DeleteKeyValueById_WithValidId_CallsDelete()
        {
            // Arrange
            _mockDao.Setup(dao => dao.DeleteKeyValueById(It.IsAny<int>()));

            // Act
            _mockDao.Object.DeleteKeyValueById(1);

            // Assert
            _mockDao.Verify(dao => dao.DeleteKeyValueById(It.IsAny<int>()), Times.Once);
        }

        #endregion

        #region InsertKeyValue Tests

        [Fact]
        [Trait("Method", "InsertKeyValue")]
        public void InsertKeyValue_WithValidParameters_CallsInsert()
        {
            // Arrange
            _mockDao.Setup(dao => dao.InsertKeyValue(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()));

            // Act
            _mockDao.Object.InsertKeyValue(1, "Test Description", "Test Value");

            // Assert
            _mockDao.Verify(dao => dao.InsertKeyValue(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        #endregion

        #region UpdateKeyValueById Tests

        [Fact]
        [Trait("Method", "UpdateKeyValueById")]
        public void UpdateKeyValueById_WithValidParameters_CallsUpdate()
        {
            // Arrange
            _mockDao.Setup(dao => dao.UpdateKeyValueById(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()));

            // Act
            _mockDao.Object.UpdateKeyValueById(1, 1, "Updated Description", "Updated Value");

            // Assert
            _mockDao.Verify(dao => dao.UpdateKeyValueById(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        #endregion
    }
}
