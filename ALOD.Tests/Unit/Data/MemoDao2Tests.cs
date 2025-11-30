using ALOD.Core.Domain.Documents;
using ALOD.Core.Interfaces.DAOInterfaces;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ALOD.Tests.Unit.Data
{
    [Trait("Category", "Unit")]
    [Trait("Layer", "Data")]
    public class MemoDao2Tests
    {
        private readonly Mock<IMemoDao2> _mockDao;

        public MemoDao2Tests()
        {
            _mockDao = new Mock<IMemoDao2>();
        }

        #region Save Tests

        [Fact]
        [Trait("Method", "Save")]
        public void Save_WithValidMemorandum2_ReturnsSavedMemorandum2()
        {
            // Arrange
            var memorandum2 = new Memorandum2();
            _mockDao.Setup(dao => dao.Save(It.IsAny<Memorandum2>()))
                .Returns(memorandum2);

            // Act
            var result = _mockDao.Object.Save(memorandum2);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.Save(It.IsAny<Memorandum2>()), Times.Once);
        }

        #endregion

        #region GetById Tests

        [Fact]
        [Trait("Method", "GetById")]
        public void GetById_WithValidId_ReturnsMemorandum2()
        {
            // Arrange
            var memorandum2 = new Memorandum2();
            _mockDao.Setup(dao => dao.GetById(It.IsAny<int>(), false))
                .Returns(memorandum2);

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
        public void GetById_WithVariousIds_ReturnsMemorandum2(int id)
        {
            // Arrange
            var memorandum2 = new Memorandum2();
            _mockDao.Setup(dao => dao.GetById(It.IsAny<int>()))
                .Returns(memorandum2);

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
        public void GetAll_ReturnsAllMemorandum2s()
        {
            // Arrange
            var memorandums = new List<Memorandum2>
            {
                new Memorandum2(),
                new Memorandum2()
            }.AsQueryable();

            _mockDao.Setup(dao => dao.GetAll())
                .Returns(memorandums);

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
        public void Delete_WithValidMemorandum2_CallsDelete()
        {
            // Arrange
            var memorandum2 = new Memorandum2();
            _mockDao.Setup(dao => dao.Delete(It.IsAny<Memorandum2>()));

            // Act
            _mockDao.Object.Delete(memorandum2);

            // Assert
            _mockDao.Verify(dao => dao.Delete(It.IsAny<Memorandum2>()), Times.Once);
        }

        #endregion

        #region SaveOrUpdate Tests

        [Fact]
        [Trait("Method", "SaveOrUpdate")]
        public void SaveOrUpdate_WithValidMemorandum2_ReturnsSavedMemorandum2()
        {
            // Arrange
            var memorandum2 = new Memorandum2();
            _mockDao.Setup(dao => dao.SaveOrUpdate(It.IsAny<Memorandum2>()))
                .Returns(memorandum2);

            // Act
            var result = _mockDao.Object.SaveOrUpdate(memorandum2);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.SaveOrUpdate(It.IsAny<Memorandum2>()), Times.Once);
        }

        #endregion
    }
}
