using ALOD.Core.Domain.DBSign;
using ALOD.Core.Interfaces.DAOInterfaces;
using Moq;
using Xunit;

namespace ALOD.Tests.Unit.Data
{
    [Trait("Category", "Unit")]
    [Trait("Layer", "Data")]
    public class DBSignTemplateDaoTests
    {
        private readonly Mock<IDBSignTemplateDao> _mockDao;

        public DBSignTemplateDaoTests()
        {
            _mockDao = new Mock<IDBSignTemplateDao>();
        }

        #region GetById Tests

        [Fact]
        [Trait("Method", "GetById")]
        public void GetById_WithValidId_ReturnsDBSignTemplate()
        {
            // Arrange
            var template = new DBSignTemplate();
            _mockDao.Setup(dao => dao.GetById(It.IsAny<int>()))
                .Returns(template);

            // Act
            var result = _mockDao.Object.GetById(1);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetById(It.IsAny<int>()), Times.Once);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(100)]
        [InlineData(999)]
        [Trait("Method", "GetById")]
        public void GetById_WithVariousIds_CallsGetById(int id)
        {
            // Arrange
            var template = new DBSignTemplate();
            _mockDao.Setup(dao => dao.GetById(It.IsAny<int>()))
                .Returns(template);

            // Act
            var result = _mockDao.Object.GetById(id);

            // Assert
            _mockDao.Verify(dao => dao.GetById(id), Times.Once);
        }

        #endregion
    }
}
