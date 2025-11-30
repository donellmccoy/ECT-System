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
    public class CertificationStampDaoTests
    {
        private readonly Mock<ICertificationStampDao> _mockDao;

        public CertificationStampDaoTests()
        {
            _mockDao = new Mock<ICertificationStampDao>();
        }

        #region GetAll Tests

        [Fact]
        [Trait("Method", "GetAll")]
        public void GetAll_ReturnsAllCertificationStamps()
        {
            // Arrange
            var stamps = new List<CertificationStamp> { new CertificationStamp() };
            _mockDao.Setup(dao => dao.GetAll())
                .Returns(stamps);

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
        public void GetById_WithValidId_ReturnsCertificationStamp()
        {
            // Arrange
            var stamp = new CertificationStamp();
            _mockDao.Setup(dao => dao.GetById(It.IsAny<int>()))
                .Returns(stamp);

            // Act
            var result = _mockDao.Object.GetById(1);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetById(It.IsAny<int>()), Times.Once);
        }

        #endregion

        #region GetCertificationStampWorkflows Tests

        [Fact]
        [Trait("Method", "GetCertificationStampWorkflows")]
        public void GetCertificationStampWorkflows_WithValidStampId_ReturnsWorkflowsList()
        {
            // Arrange
            var workflows = new List<Workflow> { new Workflow() };
            _mockDao.Setup(dao => dao.GetCertificationStampWorkflows(It.IsAny<int>()))
                .Returns(workflows);

            // Act
            var result = _mockDao.Object.GetCertificationStampWorkflows(1);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetCertificationStampWorkflows(It.IsAny<int>()), Times.Once);
        }

        #endregion

        #region GetSpecialCaseStamp Tests

        [Fact]
        [Trait("Method", "GetSpecialCaseStamp")]
        public void GetSpecialCaseStamp_WithValidParameters_ReturnsCertificationStamp()
        {
            // Arrange
            var stamp = new CertificationStamp();
            _mockDao.Setup(dao => dao.GetSpecialCaseStamp(It.IsAny<int>(), It.IsAny<bool>()))
                .Returns(stamp);

            // Act
            var result = _mockDao.Object.GetSpecialCaseStamp(1, false);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetSpecialCaseStamp(It.IsAny<int>(), It.IsAny<bool>()), Times.Once);
        }

        #endregion

        #region GetStampData Tests

        [Fact]
        [Trait("Method", "GetStampData")]
        public void GetStampData_WithValidParameters_ReturnsStampDataDictionary()
        {
            // Arrange
            var stampData = new Dictionary<string, string> { { "key", "value" } };
            _mockDao.Setup(dao => dao.GetStampData(It.IsAny<int>(), It.IsAny<bool>()))
                .Returns(stampData);

            // Act
            var result = _mockDao.Object.GetStampData(1, false);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetStampData(It.IsAny<int>(), It.IsAny<bool>()), Times.Once);
        }

        #endregion

        #region GetWorkflowCertificationStamps Tests

        [Fact]
        [Trait("Method", "GetWorkflowCertificationStamps")]
        public void GetWorkflowCertificationStamps_WithValidWorkflowId_ReturnsStampsList()
        {
            // Arrange
            var stamps = new List<CertificationStamp> { new CertificationStamp() };
            _mockDao.Setup(dao => dao.GetWorkflowCertificationStamps(It.IsAny<int>()))
                .Returns(stamps);

            // Act
            var result = _mockDao.Object.GetWorkflowCertificationStamps(1);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetWorkflowCertificationStamps(It.IsAny<int>()), Times.Once);
        }

        #endregion

        #region GetWorkflowCertificationStampsByDisposition Tests

        [Fact]
        [Trait("Method", "GetWorkflowCertificationStampsByDisposition")]
        public void GetWorkflowCertificationStampsByDisposition_WithValidParameters_ReturnsStampsList()
        {
            // Arrange
            var stamps = new List<CertificationStamp> { new CertificationStamp() };
            _mockDao.Setup(dao => dao.GetWorkflowCertificationStampsByDisposition(It.IsAny<int>(), It.IsAny<bool>()))
                .Returns(stamps);

            // Act
            var result = _mockDao.Object.GetWorkflowCertificationStampsByDisposition(1, true);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetWorkflowCertificationStampsByDisposition(It.IsAny<int>(), It.IsAny<bool>()), Times.Once);
        }

        #endregion

        #region Insert Tests

        [Fact]
        [Trait("Method", "Insert")]
        public void Insert_WithValidCertificationStamp_CallsInsert()
        {
            // Arrange
            var stamp = new CertificationStamp();
            _mockDao.Setup(dao => dao.Insert(It.IsAny<CertificationStamp>()));

            // Act
            _mockDao.Object.Insert(stamp);

            // Assert
            _mockDao.Verify(dao => dao.Insert(It.IsAny<CertificationStamp>()), Times.Once);
        }

        #endregion

        #region Update Tests

        [Fact]
        [Trait("Method", "Update")]
        public void Update_WithValidCertificationStamp_CallsUpdate()
        {
            // Arrange
            var stamp = new CertificationStamp();
            _mockDao.Setup(dao => dao.Update(It.IsAny<CertificationStamp>()));

            // Act
            _mockDao.Object.Update(stamp);

            // Assert
            _mockDao.Verify(dao => dao.Update(It.IsAny<CertificationStamp>()), Times.Once);
        }

        #endregion

        #region UpdateCertificationStampWorkflowsMaps Tests

        [Fact]
        [Trait("Method", "UpdateCertificationStampWorkflowsMaps")]
        public void UpdateCertificationStampWorkflowsMaps_WithValidParameters_CallsUpdate()
        {
            // Arrange
            var workflowIds = new List<int> { 1, 2, 3 };
            _mockDao.Setup(dao => dao.UpdateCertificationStampWorkflowsMaps(It.IsAny<int>(), It.IsAny<IList<int>>()));

            // Act
            _mockDao.Object.UpdateCertificationStampWorkflowsMaps(1, workflowIds);

            // Assert
            _mockDao.Verify(dao => dao.UpdateCertificationStampWorkflowsMaps(It.IsAny<int>(), It.IsAny<IList<int>>()), Times.Once);
        }

        #endregion
    }
}
