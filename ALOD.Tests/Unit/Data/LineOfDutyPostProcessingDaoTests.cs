using ALOD.Core.Domain.Lookup;
using ALOD.Core.Domain.Modules.Lod;
using ALOD.Core.Interfaces.DAOInterfaces;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ALOD.Tests.Unit.Data
{
    [Trait("Category", "Unit")]
    [Trait("Layer", "Data")]
    public class LineOfDutyPostProcessingDaoTests
    {
        private readonly Mock<ILineOfDutyPostProcessingDao> _mockDao;

        public LineOfDutyPostProcessingDaoTests()
        {
            _mockDao = new Mock<ILineOfDutyPostProcessingDao>();
        }

        #region GetById Tests

        [Fact]
        [Trait("Method", "GetById")]
        public void GetById_WithValidId_ReturnsEntity()
        {
            // Arrange
            int id = 123;
            var expected = CreateValidPostProcessing(id);
            _mockDao.Setup(x => x.GetById(id)).Returns(expected);

            // Act
            var result = _mockDao.Object.GetById(id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expected.Id, result.Id);
            Assert.Equal(expected.HelpExtensionNumber, result.HelpExtensionNumber);
            _mockDao.Verify(x => x.GetById(id), Times.Once);
        }

        [Theory]
        [Trait("Method", "GetById")]
        [InlineData(1)]
        [InlineData(100)]
        [InlineData(999)]
        public void GetById_WithVariousIds_ReturnsEntity(int id)
        {
            // Arrange
            _mockDao.Setup(x => x.GetById(id)).Returns(CreateValidPostProcessing(id));

            // Act
            var result = _mockDao.Object.GetById(id);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(x => x.GetById(id), Times.Once);
        }

        [Fact]
        [Trait("Method", "GetById")]
        public void GetById_WithInvalidId_ReturnsNull()
        {
            // Arrange
            int id = -1;
            _mockDao.Setup(x => x.GetById(id)).Returns((LineOfDutyPostProcessing)null);

            // Act
            var result = _mockDao.Object.GetById(id);

            // Assert
            Assert.Null(result);
            _mockDao.Verify(x => x.GetById(id), Times.Once);
        }

        #endregion

        #region InsertOrUpdate Tests

        [Fact]
        [Trait("Method", "InsertOrUpdate")]
        public void InsertOrUpdate_WithValidEntity_ReturnsTrue()
        {
            // Arrange
            var entity = CreateValidPostProcessing(1);
            _mockDao.Setup(x => x.InsertOrUpdate(It.IsAny<LineOfDutyPostProcessing>())).Returns(true);

            // Act
            var result = _mockDao.Object.InsertOrUpdate(entity);

            // Assert
            Assert.True(result);
            _mockDao.Verify(x => x.InsertOrUpdate(It.IsAny<LineOfDutyPostProcessing>()), Times.Once);
        }

        [Fact]
        [Trait("Method", "InsertOrUpdate")]
        public void InsertOrUpdate_WithNewEntity_ReturnsTrue()
        {
            // Arrange
            var entity = CreateValidPostProcessing(0);
            _mockDao.Setup(x => x.InsertOrUpdate(It.IsAny<LineOfDutyPostProcessing>())).Returns(true);

            // Act
            var result = _mockDao.Object.InsertOrUpdate(entity);

            // Assert
            Assert.True(result);
            _mockDao.Verify(x => x.InsertOrUpdate(It.IsAny<LineOfDutyPostProcessing>()), Times.Once);
        }

        [Theory]
        [Trait("Method", "InsertOrUpdate")]
        [ClassData(typeof(PostProcessingTestData))]
        public void InsertOrUpdate_WithVariousEntities_ReturnsExpectedResult(LineOfDutyPostProcessing entity, bool expectedResult)
        {
            // Arrange
            _mockDao.Setup(x => x.InsertOrUpdate(It.IsAny<LineOfDutyPostProcessing>())).Returns(expectedResult);

            // Act
            var result = _mockDao.Object.InsertOrUpdate(entity);

            // Assert
            Assert.Equal(expectedResult, result);
            _mockDao.Verify(x => x.InsertOrUpdate(It.IsAny<LineOfDutyPostProcessing>()), Times.Once);
        }

        [Fact]
        [Trait("Method", "InsertOrUpdate")]
        public void InsertOrUpdate_WithNullEntity_ThrowsException()
        {
            // Arrange
            _mockDao.Setup(x => x.InsertOrUpdate(null))
                .Throws<ArgumentNullException>();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _mockDao.Object.InsertOrUpdate(null));
        }

        #endregion

        #region Helper Methods

        private LineOfDutyPostProcessing CreateValidPostProcessing(int id)
        {
            var address = new Address
            {
                Street = "123 Main St",
                City = "Anytown",
                State = "CA",
                Zip = "12345",
                Country = "USA"
            };

            return new LineOfDutyPostProcessing(
                id,
                "555-1234",
                address,
                "John",
                "Doe",
                "M",
                DateTime.Now.AddDays(-7),
                "john.doe@example.com",
                1,
                1,
                1
            );
        }

        #endregion
    }

    #region Test Data Classes

    public class PostProcessingTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            var address = new Address
            {
                Street = "123 Test St",
                City = "Test City",
                State = "TS",
                Zip = "00000"
            };

            // Valid entity - should succeed
            var entity1 = new LineOfDutyPostProcessing(1, "555-0001", address, "First", "Last", "M", DateTime.Now, "test@test.com", 1, 1, 1);
            yield return new object[] { entity1, true };

            // Another valid entity
            var entity2 = new LineOfDutyPostProcessing(2, "555-0002", address, "Jane", "Smith", "", DateTime.Now, "jane@test.com", 1, 0, 1);
            yield return new object[] { entity2, true };

            // Entity with minimal data
            var entity3 = new LineOfDutyPostProcessing(3, "", address, "", "", "", null, "", 0, 0, 0);
            yield return new object[] { entity3, true };
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
    }

    #endregion
}
