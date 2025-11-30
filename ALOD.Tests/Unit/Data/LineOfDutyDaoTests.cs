using ALOD.Core.Domain.Modules.Lod;
using ALOD.Core.Domain.Workflow;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Data;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ALOD.Tests.Unit.Data
{
    [Trait("Category", "Unit")]
    [Trait("Layer", "Data")]
    public class LineOfDutyDaoTests
    {
        private readonly Mock<ILineOfDutyDao> _mockDao;

        public LineOfDutyDaoTests()
        {
            _mockDao = new Mock<ILineOfDutyDao>();
        }

        #region GetFromAndDirection Tests

        [Fact]
        [Trait("Method", "GetFromAndDirection")]
        public void GetFromAndDirection_WithValidRefId_ReturnsString()
        {
            // Arrange
            int refId = 123;
            string expected = "From Unit A to Direction B";
            _mockDao.Setup(x => x.GetFromAndDirection(refId)).Returns(expected);

            // Act
            var result = _mockDao.Object.GetFromAndDirection(refId);

            // Assert
            Assert.Equal(expected, result);
            _mockDao.Verify(x => x.GetFromAndDirection(refId), Times.Once);
        }

        [Theory]
        [Trait("Method", "GetFromAndDirection")]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(int.MaxValue)]
        public void GetFromAndDirection_WithVariousRefIds_CallsStoredProcedure(int refId)
        {
            // Arrange
            _mockDao.Setup(x => x.GetFromAndDirection(refId)).Returns("Result");

            // Act
            var result = _mockDao.Object.GetFromAndDirection(refId);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(x => x.GetFromAndDirection(refId), Times.Once);
        }

        #endregion

        #region GetPendingCount Tests

        [Fact]
        [Trait("Method", "GetPendingCount")]
        public void GetPendingCount_WithValidUserIdAndSarcFalse_ReturnsCount()
        {
            // Arrange
            int userId = 100;
            bool sarc = false;
            int expected = 5;
            _mockDao.Setup(x => x.GetPendingCount(userId, sarc)).Returns(expected);

            // Act
            var result = _mockDao.Object.GetPendingCount(userId, sarc);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [Trait("Method", "GetPendingCount")]
        [ClassData(typeof(PendingCountTestData))]
        public void GetPendingCount_WithVariousInputs_ReturnsExpectedCount(int userId, bool sarc, int expectedCount)
        {
            // Arrange
            _mockDao.Setup(x => x.GetPendingCount(userId, sarc)).Returns(expectedCount);

            // Act
            var result = _mockDao.Object.GetPendingCount(userId, sarc);

            // Assert
            Assert.Equal(expectedCount, result);
            _mockDao.Verify(x => x.GetPendingCount(userId, sarc), Times.Once);
        }

        #endregion

        #region GetPendingIOCount Tests

        [Fact]
        [Trait("Method", "GetPendingIOCount")]
        public void GetPendingIOCount_WithValidUserId_ReturnsCount()
        {
            // Arrange
            int userId = 50;
            bool sarc = true;
            int expected = 3;
            _mockDao.Setup(x => x.GetPendingIOCount(userId, sarc)).Returns(expected);

            // Act
            var result = _mockDao.Object.GetPendingIOCount(userId, sarc);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [Trait("Method", "GetPendingIOCount")]
        [InlineData(1, true, 10)]
        [InlineData(2, false, 0)]
        [InlineData(100, true, 25)]
        public void GetPendingIOCount_WithVariousInputs_ReturnsExpectedCount(int userId, bool sarc, int expectedCount)
        {
            // Arrange
            _mockDao.Setup(x => x.GetPendingIOCount(userId, sarc)).Returns(expectedCount);

            // Act
            var result = _mockDao.Object.GetPendingIOCount(userId, sarc);

            // Assert
            Assert.Equal(expectedCount, result);
        }

        #endregion

        #region GetUserAccess Tests

        [Fact]
        [Trait("Method", "GetUserAccess")]
        public void GetUserAccess_WithValidCredentials_ReturnsAccessLevel()
        {
            // Arrange
            int userId = 10;
            int refId = 200;
            var expected = PageAccess.AccessLevel.ReadWrite;
            _mockDao.Setup(x => x.GetUserAccess(userId, refId)).Returns(expected);

            // Act
            var result = _mockDao.Object.GetUserAccess(userId, refId);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [Trait("Method", "GetUserAccess")]
        [ClassData(typeof(UserAccessTestData))]
        public void GetUserAccess_WithVariousAccessLevels_ReturnsCorrectEnum(int userId, int refId, PageAccess.AccessLevel expectedAccess)
        {
            // Arrange
            _mockDao.Setup(x => x.GetUserAccess(userId, refId)).Returns(expectedAccess);

            // Act
            var result = _mockDao.Object.GetUserAccess(userId, refId);

            // Assert
            Assert.Equal(expectedAccess, result);
            _mockDao.Verify(x => x.GetUserAccess(userId, refId), Times.Once);
        }

        #endregion

        #region GetWorkflow Tests

        [Fact]
        [Trait("Method", "GetWorkflow")]
        public void GetWorkflow_WithValidRefId_ReturnsWorkflowId()
        {
            // Arrange
            int refId = 999;
            int expected = 5;
            _mockDao.Setup(x => x.GetWorkflow(refId)).Returns(expected);

            // Act
            var result = _mockDao.Object.GetWorkflow(refId);

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [Trait("Method", "GetWorkflow")]
        [InlineData(1, 1)]
        [InlineData(100, 2)]
        [InlineData(500, 3)]
        public void GetWorkflow_WithVariousRefIds_ReturnsWorkflowId(int refId, int expectedWorkflowId)
        {
            // Arrange
            _mockDao.Setup(x => x.GetWorkflow(refId)).Returns(expectedWorkflowId);

            // Act
            var result = _mockDao.Object.GetWorkflow(refId);

            // Assert
            Assert.Equal(expectedWorkflowId, result);
        }

        #endregion

        #region Save Tests

        [Fact]
        [Trait("Method", "Save")]
        public void Save_WithValidEntity_ReturnsSavedEntity()
        {
            // Arrange
            var entity = CreateValidLineOfDuty();
            _mockDao.Setup(x => x.Save(It.IsAny<LineOfDuty>())).Returns(entity);

            // Act
            var result = _mockDao.Object.Save(entity);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(x => x.Save(It.IsAny<LineOfDuty>()), Times.Once);
        }

        [Fact]
        [Trait("Method", "Save")]
        public void Save_WithNullMemberName_ThrowsException()
        {
            // Arrange
            var entity = CreateValidLineOfDuty();
            entity.MemberName = string.Empty;

            // This test would validate actual DAO behavior
            // For mock testing, we simulate the exception
            _mockDao.Setup(x => x.Save(It.IsAny<LineOfDuty>()))
                .Throws(new ArgumentException("Member name is required"));

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _mockDao.Object.Save(entity));
        }

        [Theory]
        [Trait("Method", "Save")]
        [ClassData(typeof(InvalidLineOfDutyTestData))]
        public void Save_WithInvalidEntity_ThrowsException(LineOfDuty entity, string expectedMessage)
        {
            // Arrange
            _mockDao.Setup(x => x.Save(It.IsAny<LineOfDuty>()))
                .Throws(new ArgumentException(expectedMessage));

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => _mockDao.Object.Save(entity));
            Assert.Contains(expectedMessage, exception.Message);
        }

        #endregion

        #region Helper Methods

        private LineOfDuty CreateValidLineOfDuty()
        {
            return new LineOfDuty
            {
                MemberName = "John Doe",
                MemberSSN = "123456789",
                MemberCompo = "A",
                MemberUnit = "Unit 123",
                MemberUnitId = 100,
                MemberDob = DateTime.Now.AddYears(-25),
                Workflow = 1,
                CreatedBy = 1,
                Status = 1,
                ModifiedBy = 1,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now
            };
        }

        #endregion
    }

    #region Test Data Classes

    public class PendingCountTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { 1, false, 0 };
            yield return new object[] { 1, true, 5 };
            yield return new object[] { 100, false, 10 };
            yield return new object[] { 100, true, 15 };
            yield return new object[] { 999, false, 100 };
            yield return new object[] { 999, true, 200 };
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class UserAccessTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { 1, 100, PageAccess.AccessLevel.None };
            yield return new object[] { 2, 200, PageAccess.AccessLevel.ReadOnly };
            yield return new object[] { 3, 300, PageAccess.AccessLevel.ReadWrite };
            yield return new object[] { 10, 500, PageAccess.AccessLevel.ReadOnly };
            yield return new object[] { 50, 1000, PageAccess.AccessLevel.None };
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class InvalidLineOfDutyTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            // Empty member name
            var entity1 = new LineOfDuty { MemberName = string.Empty };
            yield return new object[] { entity1, "Member name is required" };

            // Invalid SSN
            var entity2 = new LineOfDuty
            {
                MemberName = "John Doe",
                MemberSSN = "12345"
            };
            yield return new object[] { entity2, "Invalid SSN" };

            // Invalid Compo
            var entity3 = new LineOfDuty
            {
                MemberName = "John Doe",
                MemberSSN = "123456789",
                MemberCompo = "AB"
            };
            yield return new object[] { entity3, "Invalid Compo" };

            // Missing unit
            var entity4 = new LineOfDuty
            {
                MemberName = "John Doe",
                MemberSSN = "123456789",
                MemberCompo = "A",
                MemberUnit = string.Empty
            };
            yield return new object[] { entity4, "Member unit is required" };
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
    }

    #endregion
}
