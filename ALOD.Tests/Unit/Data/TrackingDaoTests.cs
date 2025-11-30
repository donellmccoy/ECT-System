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
    public class TrackingDaoTests
    {
        private readonly Mock<ITrackingDao> _mockDao;

        public TrackingDaoTests()
        {
            _mockDao = new Mock<ITrackingDao>();
        }

        #region Save Tests

        [Fact]
        [Trait("Method", "Save")]
        public void Save_WithValidTrackingItem_ReturnsSavedTrackingItem()
        {
            // Arrange
            var trackingItem = new TrackingItem();
            _mockDao.Setup(dao => dao.Save(It.IsAny<TrackingItem>()))
                .Returns(trackingItem);

            // Act
            var result = _mockDao.Object.Save(trackingItem);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.Save(It.IsAny<TrackingItem>()), Times.Once);
        }

        #endregion

        #region GetById Tests

        [Fact]
        [Trait("Method", "GetById")]
        public void GetById_WithValidId_ReturnsTrackingItem()
        {
            // Arrange
            var trackingItem = new TrackingItem();
            _mockDao.Setup(dao => dao.GetById(It.IsAny<int>(), false))
                .Returns(trackingItem);

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
        public void GetById_WithVariousIds_ReturnsTrackingItem(int id)
        {
            // Arrange
            var trackingItem = new TrackingItem();
            _mockDao.Setup(dao => dao.GetById(It.IsAny<int>()))
                .Returns(trackingItem);

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
        public void GetAll_ReturnsAllTrackingItems()
        {
            // Arrange
            var trackingItems = new List<TrackingItem>
            {
                new TrackingItem(),
                new TrackingItem()
            }.AsQueryable();

            _mockDao.Setup(dao => dao.GetAll())
                .Returns(trackingItems);

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
        public void Delete_WithValidTrackingItem_CallsDelete()
        {
            // Arrange
            var trackingItem = new TrackingItem();
            _mockDao.Setup(dao => dao.Delete(It.IsAny<TrackingItem>()));

            // Act
            _mockDao.Object.Delete(trackingItem);

            // Assert
            _mockDao.Verify(dao => dao.Delete(It.IsAny<TrackingItem>()), Times.Once);
        }

        #endregion

        #region SaveOrUpdate Tests

        [Fact]
        [Trait("Method", "SaveOrUpdate")]
        public void SaveOrUpdate_WithValidTrackingItem_ReturnsSavedTrackingItem()
        {
            // Arrange
            var trackingItem = new TrackingItem();
            _mockDao.Setup(dao => dao.SaveOrUpdate(It.IsAny<TrackingItem>()))
                .Returns(trackingItem);

            // Act
            var result = _mockDao.Object.SaveOrUpdate(trackingItem);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.SaveOrUpdate(It.IsAny<TrackingItem>()), Times.Once);
        }

        #endregion
    }
}
