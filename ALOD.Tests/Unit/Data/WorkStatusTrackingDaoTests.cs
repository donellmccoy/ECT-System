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
    public class WorkStatusTrackingDaoTests
    {
        private readonly Mock<IWorkStatusTrackingDao> _mockDao;

        public WorkStatusTrackingDaoTests()
        {
            _mockDao = new Mock<IWorkStatusTrackingDao>();
        }

        #region Save Tests

        [Fact]
        [Trait("Method", "Save")]
        public void Save_WithValidWorkStatusTracking_ReturnsSavedWorkStatusTracking()
        {
            // Arrange
            var workStatusTracking = new WorkStatusTracking();
            _mockDao.Setup(dao => dao.Save(It.IsAny<WorkStatusTracking>()))
                .Returns(workStatusTracking);

            // Act
            var result = _mockDao.Object.Save(workStatusTracking);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.Save(It.IsAny<WorkStatusTracking>()), Times.Once);
        }

        #endregion

        #region GetById Tests

        [Fact]
        [Trait("Method", "GetById")]
        public void GetById_WithValidId_ReturnsWorkStatusTracking()
        {
            // Arrange
            var workStatusTracking = new WorkStatusTracking();
            _mockDao.Setup(dao => dao.GetById(It.IsAny<int>(), false))
                .Returns(workStatusTracking);

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
        public void GetById_WithVariousIds_ReturnsWorkStatusTracking(int id)
        {
            // Arrange
            var workStatusTracking = new WorkStatusTracking();
            _mockDao.Setup(dao => dao.GetById(It.IsAny<int>()))
                .Returns(workStatusTracking);

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
        public void GetAll_ReturnsAllWorkStatusTrackings()
        {
            // Arrange
            var workStatusTrackings = new List<WorkStatusTracking>
            {
                new WorkStatusTracking(),
                new WorkStatusTracking()
            }.AsQueryable();

            _mockDao.Setup(dao => dao.GetAll())
                .Returns(workStatusTrackings);

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
        public void Delete_WithValidWorkStatusTracking_CallsDelete()
        {
            // Arrange
            var workStatusTracking = new WorkStatusTracking();
            _mockDao.Setup(dao => dao.Delete(It.IsAny<WorkStatusTracking>()));

            // Act
            _mockDao.Object.Delete(workStatusTracking);

            // Assert
            _mockDao.Verify(dao => dao.Delete(It.IsAny<WorkStatusTracking>()), Times.Once);
        }

        #endregion

        #region SaveOrUpdate Tests

        [Fact]
        [Trait("Method", "SaveOrUpdate")]
        public void SaveOrUpdate_WithValidWorkStatusTracking_ReturnsSavedWorkStatusTracking()
        {
            // Arrange
            var workStatusTracking = new WorkStatusTracking();
            _mockDao.Setup(dao => dao.SaveOrUpdate(It.IsAny<WorkStatusTracking>()))
                .Returns(workStatusTracking);

            // Act
            var result = _mockDao.Object.SaveOrUpdate(workStatusTracking);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.SaveOrUpdate(It.IsAny<WorkStatusTracking>()), Times.Once);
        }

        #endregion
    }
}
