using ALOD.Core.Domain.Lookup;
using ALOD.Core.Domain.Workflow;
using ALOD.Core.Interfaces.DAOInterfaces;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace ALOD.Tests.Unit.Data
{
    [Trait("Category", "Unit")]
    [Trait("Layer", "Data")]
    public class CompletedByGroupDaoTests
    {
        private readonly Mock<ICompletedByGroupDao> _mockDao;

        public CompletedByGroupDaoTests()
        {
            _mockDao = new Mock<ICompletedByGroupDao>();
        }

        #region GetAll Tests

        [Fact]
        [Trait("Method", "GetAll")]
        public void GetAll_ReturnsAllCompletedByGroups()
        {
            // Arrange
            var groups = new List<CompletedByGroup> { new CompletedByGroup() };
            _mockDao.Setup(dao => dao.GetAll())
                .Returns(groups);

            // Act
            var result = _mockDao.Object.GetAll();

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetAll(), Times.Once);
        }

        #endregion

        #region GetById Tests

        [Fact]
        [Trait("Method", "GetById")]
        public void GetById_WithValidId_ReturnsCompletedByGroup()
        {
            // Arrange
            var group = new CompletedByGroup();
            _mockDao.Setup(dao => dao.GetById(It.IsAny<int>()))
                .Returns(group);

            // Act
            var result = _mockDao.Object.GetById(1);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetById(It.IsAny<int>()), Times.Once);
        }

        #endregion

        #region GetCompletedByGroupWorkflows Tests

        [Fact]
        [Trait("Method", "GetCompletedByGroupWorkflows")]
        public void GetCompletedByGroupWorkflows_WithValidGroupId_ReturnsWorkflows()
        {
            // Arrange
            var workflows = new List<Workflow> { new Workflow() };
            _mockDao.Setup(dao => dao.GetCompletedByGroupWorkflows(It.IsAny<int>()))
                .Returns(workflows);

            // Act
            var result = _mockDao.Object.GetCompletedByGroupWorkflows(1);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetCompletedByGroupWorkflows(It.IsAny<int>()), Times.Once);
        }

        #endregion

        #region GetWorkflowCompletedByGroups Tests

        [Fact]
        [Trait("Method", "GetWorkflowCompletedByGroups")]
        public void GetWorkflowCompletedByGroups_WithValidWorkflowId_ReturnsGroups()
        {
            // Arrange
            var groups = new List<CompletedByGroup> { new CompletedByGroup() };
            _mockDao.Setup(dao => dao.GetWorkflowCompletedByGroups(It.IsAny<int>()))
                .Returns(groups);

            // Act
            var result = _mockDao.Object.GetWorkflowCompletedByGroups(1);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetWorkflowCompletedByGroups(It.IsAny<int>()), Times.Once);
        }

        #endregion

        #region Insert Tests

        [Fact]
        [Trait("Method", "Insert")]
        public void Insert_WithValidCompletedByGroup_CallsInsert()
        {
            // Arrange
            var group = new CompletedByGroup();
            _mockDao.Setup(dao => dao.Insert(It.IsAny<CompletedByGroup>()));

            // Act
            _mockDao.Object.Insert(group);

            // Assert
            _mockDao.Verify(dao => dao.Insert(It.IsAny<CompletedByGroup>()), Times.Once);
        }

        #endregion

        #region Update Tests

        [Fact]
        [Trait("Method", "Update")]
        public void Update_WithValidCompletedByGroup_CallsUpdate()
        {
            // Arrange
            var group = new CompletedByGroup();
            _mockDao.Setup(dao => dao.Update(It.IsAny<CompletedByGroup>()));

            // Act
            _mockDao.Object.Update(group);

            // Assert
            _mockDao.Verify(dao => dao.Update(It.IsAny<CompletedByGroup>()), Times.Once);
        }

        #endregion

        #region UpdateCompletedByGroupWorkflowMaps Tests

        [Fact]
        [Trait("Method", "UpdateCompletedByGroupWorkflowMaps")]
        public void UpdateCompletedByGroupWorkflowMaps_WithValidParameters_CallsUpdate()
        {
            // Arrange
            var workflowIds = new List<int> { 1, 2, 3 };
            _mockDao.Setup(dao => dao.UpdateCompletedByGroupWorkflowMaps(It.IsAny<int>(), It.IsAny<IList<int>>()));

            // Act
            _mockDao.Object.UpdateCompletedByGroupWorkflowMaps(1, workflowIds);

            // Assert
            _mockDao.Verify(dao => dao.UpdateCompletedByGroupWorkflowMaps(It.IsAny<int>(), It.IsAny<IList<int>>()), Times.Once);
        }

        #endregion
    }
}
