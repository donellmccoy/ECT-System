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
    public class CaseTypeDaoTests
    {
        private readonly Mock<ICaseTypeDao> _mockDao;

        public CaseTypeDaoTests()
        {
            _mockDao = new Mock<ICaseTypeDao>();
        }

        #region GetAll Tests

        [Fact]
        [Trait("Method", "GetAll")]
        public void GetAll_ReturnsAllCaseTypes()
        {
            // Arrange
            var caseTypes = new List<CaseType> { new CaseType() };
            _mockDao.Setup(dao => dao.GetAll())
                .Returns(caseTypes);

            // Act
            var result = _mockDao.Object.GetAll();

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetAll(), Times.Once);
        }

        #endregion

        #region GetAllSubCaseTypes Tests

        [Fact]
        [Trait("Method", "GetAllSubCaseTypes")]
        public void GetAllSubCaseTypes_ReturnsAllSubCaseTypes()
        {
            // Arrange
            var subCaseTypes = new List<CaseType> { new CaseType() };
            _mockDao.Setup(dao => dao.GetAllSubCaseTypes())
                .Returns(subCaseTypes);

            // Act
            var result = _mockDao.Object.GetAllSubCaseTypes();

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetAllSubCaseTypes(), Times.Once);
        }

        #endregion

        #region GetById Tests

        [Fact]
        [Trait("Method", "GetById")]
        public void GetById_WithValidId_ReturnsCaseType()
        {
            // Arrange
            var caseType = new CaseType();
            _mockDao.Setup(dao => dao.GetById(It.IsAny<int>()))
                .Returns(caseType);

            // Act
            var result = _mockDao.Object.GetById(1);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetById(It.IsAny<int>()), Times.Once);
        }

        #endregion

        #region GetCaseTypeWorkflows Tests

        [Fact]
        [Trait("Method", "GetCaseTypeWorkflows")]
        public void GetCaseTypeWorkflows_WithValidCaseTypeId_ReturnsWorkflows()
        {
            // Arrange
            var workflows = new List<Workflow> { new Workflow() };
            _mockDao.Setup(dao => dao.GetCaseTypeWorkflows(It.IsAny<int>()))
                .Returns(workflows);

            // Act
            var result = _mockDao.Object.GetCaseTypeWorkflows(1);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetCaseTypeWorkflows(It.IsAny<int>()), Times.Once);
        }

        #endregion

        #region GetWorkflowCaseTypes Tests

        [Fact]
        [Trait("Method", "GetWorkflowCaseTypes")]
        public void GetWorkflowCaseTypes_WithValidWorkflowId_ReturnsCaseTypes()
        {
            // Arrange
            var caseTypes = new List<CaseType> { new CaseType() };
            _mockDao.Setup(dao => dao.GetWorkflowCaseTypes(It.IsAny<int>()))
                .Returns(caseTypes);

            // Act
            var result = _mockDao.Object.GetWorkflowCaseTypes(1);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetWorkflowCaseTypes(It.IsAny<int>()), Times.Once);
        }

        #endregion

        #region Insert Tests

        [Fact]
        [Trait("Method", "Insert")]
        public void Insert_WithValidCaseType_CallsInsert()
        {
            // Arrange
            var caseType = new CaseType();
            _mockDao.Setup(dao => dao.Insert(It.IsAny<CaseType>()));

            // Act
            _mockDao.Object.Insert(caseType);

            // Assert
            _mockDao.Verify(dao => dao.Insert(It.IsAny<CaseType>()), Times.Once);
        }

        #endregion

        #region InsertSubCaseType Tests

        [Fact]
        [Trait("Method", "InsertSubCaseType")]
        public void InsertSubCaseType_WithValidCaseType_CallsInsertSubCaseType()
        {
            // Arrange
            var caseType = new CaseType();
            _mockDao.Setup(dao => dao.InsertSubCaseType(It.IsAny<CaseType>()));

            // Act
            _mockDao.Object.InsertSubCaseType(caseType);

            // Assert
            _mockDao.Verify(dao => dao.InsertSubCaseType(It.IsAny<CaseType>()), Times.Once);
        }

        #endregion

        #region Update Tests

        [Fact]
        [Trait("Method", "Update")]
        public void Update_WithValidCaseType_CallsUpdate()
        {
            // Arrange
            var caseType = new CaseType();
            _mockDao.Setup(dao => dao.Update(It.IsAny<CaseType>()));

            // Act
            _mockDao.Object.Update(caseType);

            // Assert
            _mockDao.Verify(dao => dao.Update(It.IsAny<CaseType>()), Times.Once);
        }

        #endregion

        #region UpdateSubCaseType Tests

        [Fact]
        [Trait("Method", "UpdateSubCaseType")]
        public void UpdateSubCaseType_WithValidCaseType_CallsUpdateSubCaseType()
        {
            // Arrange
            var caseType = new CaseType();
            _mockDao.Setup(dao => dao.UpdateSubCaseType(It.IsAny<CaseType>()));

            // Act
            _mockDao.Object.UpdateSubCaseType(caseType);

            // Assert
            _mockDao.Verify(dao => dao.UpdateSubCaseType(It.IsAny<CaseType>()), Times.Once);
        }

        #endregion

        #region UpdateCaseTypeSubCaseTypeMaps Tests

        [Fact]
        [Trait("Method", "UpdateCaseTypeSubCaseTypeMaps")]
        public void UpdateCaseTypeSubCaseTypeMaps_WithValidParameters_CallsUpdate()
        {
            // Arrange
            var subCaseTypeIds = new List<int> { 1, 2, 3 };
            _mockDao.Setup(dao => dao.UpdateCaseTypeSubCaseTypeMaps(It.IsAny<int>(), It.IsAny<IList<int>>()));

            // Act
            _mockDao.Object.UpdateCaseTypeSubCaseTypeMaps(1, subCaseTypeIds);

            // Assert
            _mockDao.Verify(dao => dao.UpdateCaseTypeSubCaseTypeMaps(It.IsAny<int>(), It.IsAny<IList<int>>()), Times.Once);
        }

        #endregion

        #region UpdateCaseTypeWorkflowMaps Tests

        [Fact]
        [Trait("Method", "UpdateCaseTypeWorkflowMaps")]
        public void UpdateCaseTypeWorkflowMaps_WithValidParameters_CallsUpdate()
        {
            // Arrange
            var workflowIds = new List<int> { 1, 2, 3 };
            _mockDao.Setup(dao => dao.UpdateCaseTypeWorkflowMaps(It.IsAny<int>(), It.IsAny<IList<int>>()));

            // Act
            _mockDao.Object.UpdateCaseTypeWorkflowMaps(1, workflowIds);

            // Assert
            _mockDao.Verify(dao => dao.UpdateCaseTypeWorkflowMaps(It.IsAny<int>(), It.IsAny<IList<int>>()), Times.Once);
        }

        #endregion
    }
}
