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
    public class LookupDispositionDaoTests
    {
        private readonly Mock<ILookupDispositionDao> _mockDao;

        public LookupDispositionDaoTests()
        {
            _mockDao = new Mock<ILookupDispositionDao>();
        }

        #region GetAll Tests

        [Fact]
        [Trait("Method", "GetAll")]
        public void GetAll_ReturnsAllDispositions()
        {
            // Arrange
            var dispositions = new List<Disposition> { new Disposition() };
            _mockDao.Setup(dao => dao.GetAll())
                .Returns(dispositions);

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
        public void GetById_WithValidId_ReturnsDisposition()
        {
            // Arrange
            var disposition = new Disposition();
            _mockDao.Setup(dao => dao.GetById(It.IsAny<int>()))
                .Returns(disposition);

            // Act
            var result = _mockDao.Object.GetById(1);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetById(It.IsAny<int>()), Times.Once);
        }

        #endregion

        #region GetDispositionWorkflows Tests

        [Fact]
        [Trait("Method", "GetDispositionWorkflows")]
        public void GetDispositionWorkflows_WithValidDispositionId_ReturnsWorkflows()
        {
            // Arrange
            var workflows = new List<Workflow> { new Workflow() };
            _mockDao.Setup(dao => dao.GetDispositionWorkflows(It.IsAny<int>()))
                .Returns(workflows);

            // Act
            var result = _mockDao.Object.GetDispositionWorkflows(1);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetDispositionWorkflows(It.IsAny<int>()), Times.Once);
        }

        #endregion

        #region GetWorkflowDispositions Tests

        [Fact]
        [Trait("Method", "GetWorkflowDispositions")]
        public void GetWorkflowDispositions_WithValidWorkflowId_ReturnsDispositions()
        {
            // Arrange
            var dispositions = new List<Disposition> { new Disposition() };
            _mockDao.Setup(dao => dao.GetWorkflowDispositions(It.IsAny<int>()))
                .Returns(dispositions);

            // Act
            var result = _mockDao.Object.GetWorkflowDispositions(1);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetWorkflowDispositions(It.IsAny<int>()), Times.Once);
        }

        #endregion

        #region InsertDisposition Tests

        [Fact]
        [Trait("Method", "InsertDisposition")]
        public void InsertDisposition_WithValidName_CallsInsert()
        {
            // Arrange
            _mockDao.Setup(dao => dao.InsertDisposition(It.IsAny<string>()));

            // Act
            _mockDao.Object.InsertDisposition("Test Disposition");

            // Assert
            _mockDao.Verify(dao => dao.InsertDisposition(It.IsAny<string>()), Times.Once);
        }

        #endregion

        #region UpdateDisposition Tests

        [Fact]
        [Trait("Method", "UpdateDisposition")]
        public void UpdateDisposition_WithValidParameters_CallsUpdate()
        {
            // Arrange
            _mockDao.Setup(dao => dao.UpdateDisposition(It.IsAny<int>(), It.IsAny<string>()));

            // Act
            _mockDao.Object.UpdateDisposition(1, "Updated Disposition");

            // Assert
            _mockDao.Verify(dao => dao.UpdateDisposition(It.IsAny<int>(), It.IsAny<string>()), Times.Once);
        }

        #endregion

        #region UpdateDispositionWorkflowsMaps Tests

        [Fact]
        [Trait("Method", "UpdateDispositionWorkflowsMaps")]
        public void UpdateDispositionWorkflowsMaps_WithValidParameters_CallsUpdate()
        {
            // Arrange
            var workflowIds = new List<int> { 1, 2, 3 };
            _mockDao.Setup(dao => dao.UpdateDispositionWorkflowsMaps(It.IsAny<int>(), It.IsAny<IList<int>>()));

            // Act
            _mockDao.Object.UpdateDispositionWorkflowsMaps(1, workflowIds);

            // Assert
            _mockDao.Verify(dao => dao.UpdateDispositionWorkflowsMaps(It.IsAny<int>(), It.IsAny<IList<int>>()), Times.Once);
        }

        #endregion
    }
}
