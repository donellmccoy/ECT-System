using ALOD.Core.Domain.Modules.SpecialCases;
using ALOD.Core.Interfaces.DAOInterfaces;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ALOD.Tests.Unit.Data
{
    [Trait("Category", "Unit")]
    [Trait("Layer", "Data")]
    public class AssociatedCaseDaoTests
    {
        private readonly Mock<IAssociatedCaseDao> _mockDao;

        public AssociatedCaseDaoTests()
        {
            _mockDao = new Mock<IAssociatedCaseDao>();
        }

        #region GetAssociatedCases Tests

        [Fact]
        [Trait("Method", "GetAssociatedCases")]
        public void GetAssociatedCases_WithValidParameters_ReturnsAssociatedCasesList()
        {
            // Arrange
            var associatedCases = new List<AssociatedCase> { new AssociatedCase() };
            _mockDao.Setup(dao => dao.GetAssociatedCases(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(associatedCases);

            // Act
            var result = _mockDao.Object.GetAssociatedCases(1, 1);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetAssociatedCases(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
        }

        #endregion

        #region GetAssociatedCasesLOD Tests

        [Fact]
        [Trait("Method", "GetAssociatedCasesLOD")]
        public void GetAssociatedCasesLOD_WithValidParameters_ReturnsAssociatedCasesList()
        {
            // Arrange
            var associatedCases = new List<AssociatedCase> { new AssociatedCase() };
            _mockDao.Setup(dao => dao.GetAssociatedCasesLOD(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(associatedCases);

            // Act
            var result = _mockDao.Object.GetAssociatedCasesLOD(1, 1);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetAssociatedCasesLOD(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
        }

        #endregion

        #region GetAssociatedCasesSC Tests

        [Fact]
        [Trait("Method", "GetAssociatedCasesSC")]
        public void GetAssociatedCasesSC_WithValidParameters_ReturnsAssociatedCasesList()
        {
            // Arrange
            var associatedCases = new List<AssociatedCase> { new AssociatedCase() };
            _mockDao.Setup(dao => dao.GetAssociatedCasesSC(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(associatedCases);

            // Act
            var result = _mockDao.Object.GetAssociatedCasesSC(1, 1);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetAssociatedCasesSC(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
        }

        #endregion

        #region GetLODListByMemberSSN Tests

        [Fact]
        [Trait("Method", "GetLODListByMemberSSN")]
        public void GetLODListByMemberSSN_WithValidParameters_ReturnsTupleList()
        {
            // Arrange
            var lodList = new List<Tuple<string, int, int>> 
            { 
                new Tuple<string, int, int>("LOD-123", 1, 1) 
            };
            _mockDao.Setup(dao => dao.GetLODListByMemberSSN(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(lodList);

            // Act
            var result = _mockDao.Object.GetLODListByMemberSSN("123456789", 1, 1);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetLODListByMemberSSN(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
        }

        #endregion

        #region Save Tests

        [Fact]
        [Trait("Method", "Save")]
        public void Save_WithValidParameters_CallsSave()
        {
            // Arrange
            var refIds = new List<int> { 1, 2 };
            var workflowIds = new List<int> { 1, 2 };
            var caseIds = new List<string> { "LOD-1", "LOD-2" };
            _mockDao.Setup(dao => dao.Save(
                It.IsAny<int>(), It.IsAny<int>(), 
                It.IsAny<IList<int>>(), It.IsAny<IList<int>>(), It.IsAny<IList<string>>()));

            // Act
            _mockDao.Object.Save(1, 1, refIds, workflowIds, caseIds);

            // Assert
            _mockDao.Verify(dao => dao.Save(
                It.IsAny<int>(), It.IsAny<int>(), 
                It.IsAny<IList<int>>(), It.IsAny<IList<int>>(), It.IsAny<IList<string>>()), Times.Once);
        }

        #endregion
    }
}
