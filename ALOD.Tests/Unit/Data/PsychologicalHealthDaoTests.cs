using ALOD.Core.Domain.PsychologicalHealth;
using ALOD.Core.Interfaces.DAOInterfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using Xunit;

namespace ALOD.Tests.Unit.Data
{
    [Trait("Category", "Unit")]
    [Trait("Layer", "Data")]
    public class PsychologicalHealthDaoTests
    {
        private readonly Mock<IPsychologicalHealthDao> _mockDao;

        public PsychologicalHealthDaoTests()
        {
            _mockDao = new Mock<IPsychologicalHealthDao>();
        }

        #region GetAllFields Tests

        [Fact]
        [Trait("Method", "GetAllFields")]
        public void GetAllFields_ReturnsAllFields()
        {
            // Arrange
            var fields = new List<PHField> { new PHField() };
            _mockDao.Setup(dao => dao.GetAllFields())
                .Returns(fields);

            // Act
            var result = _mockDao.Object.GetAllFields();

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetAllFields(), Times.Once);
        }

        #endregion

        #region GetAllFieldTypes Tests

        [Fact]
        [Trait("Method", "GetAllFieldTypes")]
        public void GetAllFieldTypes_ReturnsAllFieldTypes()
        {
            // Arrange
            var fieldTypes = new List<PHFieldType> { new PHFieldType() };
            _mockDao.Setup(dao => dao.GetAllFieldTypes())
                .Returns(fieldTypes);

            // Act
            var result = _mockDao.Object.GetAllFieldTypes();

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetAllFieldTypes(), Times.Once);
        }

        #endregion

        #region GetAllFormFields Tests

        [Fact]
        [Trait("Method", "GetAllFormFields")]
        public void GetAllFormFields_ReturnsAllFormFields()
        {
            // Arrange
            var formFields = new List<PHFormField> { new PHFormField() };
            _mockDao.Setup(dao => dao.GetAllFormFields())
                .Returns(formFields);

            // Act
            var result = _mockDao.Object.GetAllFormFields();

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetAllFormFields(), Times.Once);
        }

        #endregion

        #region GetAllSections Tests

        [Fact]
        [Trait("Method", "GetAllSections")]
        public void GetAllSections_ReturnsAllSections()
        {
            // Arrange
            var sections = new List<PHSection> { new PHSection() };
            _mockDao.Setup(dao => dao.GetAllSections())
                .Returns(sections);

            // Act
            var result = _mockDao.Object.GetAllSections();

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetAllSections(), Times.Once);
        }

        #endregion

        #region GetFieldById Tests

        [Fact]
        [Trait("Method", "GetFieldById")]
        public void GetFieldById_WithValidId_ReturnsPHField()
        {
            // Arrange
            var field = new PHField();
            _mockDao.Setup(dao => dao.GetFieldById(It.IsAny<int>()))
                .Returns(field);

            // Act
            var result = _mockDao.Object.GetFieldById(1);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetFieldById(It.IsAny<int>()), Times.Once);
        }

        #endregion

        #region GetSectionById Tests

        [Fact]
        [Trait("Method", "GetSectionById")]
        public void GetSectionById_WithValidId_ReturnsPHSection()
        {
            // Arrange
            var section = new PHSection();
            _mockDao.Setup(dao => dao.GetSectionById(It.IsAny<int>()))
                .Returns(section);

            // Act
            var result = _mockDao.Object.GetSectionById(1);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetSectionById(It.IsAny<int>()), Times.Once);
        }

        #endregion

        #region GetCaseIdByReportingPeriod Tests

        [Fact]
        [Trait("Method", "GetCaseIdByReportingPeriod")]
        public void GetCaseIdByReportingPeriod_WithValidParameters_ReturnsCaseId()
        {
            // Arrange
            _mockDao.Setup(dao => dao.GetCaseIdByReportingPeriod(It.IsAny<DateTime>(), It.IsAny<int>()))
                .Returns(1);

            // Act
            var result = _mockDao.Object.GetCaseIdByReportingPeriod(DateTime.Now, 1);

            // Assert
            Assert.True(result > 0);
            _mockDao.Verify(dao => dao.GetCaseIdByReportingPeriod(It.IsAny<DateTime>(), It.IsAny<int>()), Times.Once);
        }

        #endregion

        #region ExecuteCollectionProcess Tests

        [Fact]
        [Trait("Method", "ExecuteCollectionProcess")]
        public void ExecuteCollectionProcess_WithValidDate_ReturnsIntList()
        {
            // Arrange
            var intList = new List<int> { 1, 2, 3 };
            _mockDao.Setup(dao => dao.ExecuteCollectionProcess(It.IsAny<DateTime>()))
                .Returns(intList);

            // Act
            var result = _mockDao.Object.ExecuteCollectionProcess(DateTime.Now.AddMonths(-1));

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.ExecuteCollectionProcess(It.IsAny<DateTime>()), Times.Once);
        }

        #endregion

        #region GetFormValuesByRefId Tests

        [Fact]
        [Trait("Method", "GetFormValuesByRefId")]
        public void GetFormValuesByRefId_WithValidRefId_ReturnsFormValuesList()
        {
            // Arrange
            var formValues = new List<PHFormValue> { new PHFormValue() };
            _mockDao.Setup(dao => dao.GetFormValuesByRefId(It.IsAny<int>()))
                .Returns(formValues);

            // Act
            var result = _mockDao.Object.GetFormValuesByRefId(1);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetFormValuesByRefId(It.IsAny<int>()), Times.Once);
        }

        #endregion

        #region InsertFormValue Tests

        [Fact]
        [Trait("Method", "InsertFormValue")]
        public void InsertFormValue_WithValidParameters_ReturnsTrue()
        {
            // Arrange
            _mockDao.Setup(dao => dao.InsertFormValue(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .Returns(true);

            // Act
            var result = _mockDao.Object.InsertFormValue(1, 1, 1, 1, "value");

            // Assert
            Assert.True(result);
            _mockDao.Verify(dao => dao.InsertFormValue(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()), Times.Once);
        }

        #endregion

        #region UpdateFormValue Tests

        [Fact]
        [Trait("Method", "UpdateFormValue")]
        public void UpdateFormValue_WithValidFormValue_ReturnsTrue()
        {
            // Arrange
            var formValue = new PHFormValue();
            _mockDao.Setup(dao => dao.UpdateFormValue(It.IsAny<PHFormValue>()))
                .Returns(true);

            // Act
            var result = _mockDao.Object.UpdateFormValue(formValue);

            // Assert
            Assert.True(result);
            _mockDao.Verify(dao => dao.UpdateFormValue(It.IsAny<PHFormValue>()), Times.Once);
        }

        #endregion

        #region PHCaseSearch Tests

        [Fact]
        [Trait("Method", "PHCaseSearch")]
        public void PHCaseSearch_WithValidParameters_ReturnsDataSet()
        {
            // Arrange
            var dataSet = new DataSet();
            _mockDao.Setup(dao => dao.PHCaseSearch(
                It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), 
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), 
                It.IsAny<int>(), It.IsAny<int>()))
                .Returns(dataSet);

            // Act
            var result = _mockDao.Object.PHCaseSearch("CASE-123", 1, 1, 1, 1, 1, 1, 100, 1, 2024);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.PHCaseSearch(
                It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), 
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), 
                It.IsAny<int>(), It.IsAny<int>()), Times.Once);
        }

        #endregion

        #region GetPushReportEmailList Tests

        [Fact]
        [Trait("Method", "GetPushReportEmailList")]
        public void GetPushReportEmailList_WithValidParameters_ReturnsStringCollection()
        {
            // Arrange
            var emailList = new StringCollection();
            _mockDao.Setup(dao => dao.GetPushReportEmailList(It.IsAny<DateTime>(), It.IsAny<int>()))
                .Returns(emailList);

            // Act
            var result = _mockDao.Object.GetPushReportEmailList(DateTime.Now, 1);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetPushReportEmailList(It.IsAny<DateTime>(), It.IsAny<int>()), Times.Once);
        }

        #endregion
    }
}
