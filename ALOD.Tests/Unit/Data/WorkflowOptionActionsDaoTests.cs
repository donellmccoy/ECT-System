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
    public class WorkflowOptionActionsDaoTests
    {
        private readonly Mock<IWorkflowOptionActionsDao> _mockDao;

        public WorkflowOptionActionsDaoTests()
        {
            _mockDao = new Mock<IWorkflowOptionActionsDao>();
        }

        #region Save Tests

        [Fact]
        [Trait("Method", "Save")]
        public void Save_WithValidEntity_ReturnsSavedEntity()
        {
            // Arrange
            var entity = new WorkflowOptionAction();
            _mockDao.Setup(x => x.Save(It.IsAny<WorkflowOptionAction>())).Returns(entity);

            // Act
            var result = _mockDao.Object.Save(entity);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(x => x.Save(It.IsAny<WorkflowOptionAction>()), Times.Once);
        }

        #endregion

        #region GetById Tests

        [Fact]
        [Trait("Method", "GetById")]
        public void GetById_WithValidId_ReturnsEntity()
        {
            // Arrange
            int id = 1;
            var entity = new WorkflowOptionAction();
            _mockDao.Setup(x => x.GetById(id, false)).Returns(entity);

            // Act
            var result = _mockDao.Object.GetById(id, false);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(x => x.GetById(id, false), Times.Once);
        }

        [Theory]
        [Trait("Method", "GetById")]
        [InlineData(1)]
        [InlineData(50)]
        [InlineData(200)]
        public void GetById_WithVariousIds_ReturnsWorkflowOptionAction(int id)
        {
            // Arrange
            _mockDao.Setup(x => x.GetById(id)).Returns(new WorkflowOptionAction());

            // Act
            var result = _mockDao.Object.GetById(id);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(x => x.GetById(id), Times.Once);
        }

        #endregion

        #region GetAll Tests

        [Fact]
        [Trait("Method", "GetAll")]
        public void GetAll_ReturnsAllEntities()
        {
            // Arrange
            var actions = new List<WorkflowOptionAction>
            {
                new WorkflowOptionAction(),
                new WorkflowOptionAction(),
                new WorkflowOptionAction()
            }.AsQueryable();
            _mockDao.Setup(x => x.GetAll()).Returns(actions);

            // Act
            var result = _mockDao.Object.GetAll();

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(x => x.GetAll(), Times.Once);
        }

        #endregion

        #region Delete Tests

        [Fact]
        [Trait("Method", "Delete")]
        public void Delete_WithValidEntity_CallsDelete()
        {
            // Arrange
            var entity = new WorkflowOptionAction();
            _mockDao.Setup(x => x.Delete(It.IsAny<WorkflowOptionAction>()));

            // Act
            _mockDao.Object.Delete(entity);

            // Assert
            _mockDao.Verify(x => x.Delete(It.IsAny<WorkflowOptionAction>()), Times.Once);
        }

        #endregion

        #region SaveOrUpdate Tests

        [Fact]
        [Trait("Method", "SaveOrUpdate")]
        public void SaveOrUpdate_WithValidEntity_ReturnsSavedEntity()
        {
            // Arrange
            var entity = new WorkflowOptionAction();
            _mockDao.Setup(x => x.SaveOrUpdate(It.IsAny<WorkflowOptionAction>())).Returns(entity);

            // Act
            var result = _mockDao.Object.SaveOrUpdate(entity);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(x => x.SaveOrUpdate(It.IsAny<WorkflowOptionAction>()), Times.Once);
        }

        #endregion
    }
}
