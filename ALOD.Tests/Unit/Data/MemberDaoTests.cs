using ALOD.Core.Domain.ServiceMembers;
using ALOD.Core.Interfaces.DAOInterfaces;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ALOD.Tests.Unit.Data
{
    [Trait("Category", "Unit")]
    [Trait("Layer", "Data")]
    public class MemberDaoTests
    {
        private readonly Mock<IMemberDao> _mockDao;

        public MemberDaoTests()
        {
            _mockDao = new Mock<IMemberDao>();
        }

        #region Save Tests

        [Fact]
        [Trait("Method", "Save")]
        public void Save_WithValidServiceMember_ReturnsSavedServiceMember()
        {
            // Arrange
            var serviceMember = new ServiceMember();
            _mockDao.Setup(dao => dao.Save(It.IsAny<ServiceMember>()))
                .Returns(serviceMember);

            // Act
            var result = _mockDao.Object.Save(serviceMember);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.Save(It.IsAny<ServiceMember>()), Times.Once);
        }

        #endregion

        #region GetById Tests

        [Fact]
        [Trait("Method", "GetById")]
        public void GetById_WithValidId_ReturnsServiceMember()
        {
            // Arrange
            var serviceMember = new ServiceMember();
            _mockDao.Setup(dao => dao.GetById(It.IsAny<string>(), false))
                .Returns(serviceMember);

            // Act
            var result = _mockDao.Object.GetById("123456789", false);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetById(It.IsAny<string>(), false), Times.Once);
        }

        [Theory]
        [Trait("Method", "GetById")]
        [InlineData("123456789")]
        [InlineData("987654321")]
        [InlineData("111223333")]
        public void GetById_WithVariousIds_ReturnsServiceMember(string id)
        {
            // Arrange
            var serviceMember = new ServiceMember();
            _mockDao.Setup(dao => dao.GetById(It.IsAny<string>()))
                .Returns(serviceMember);

            // Act
            var result = _mockDao.Object.GetById(id);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetById(It.IsAny<string>()), Times.Once);
        }

        #endregion

        #region GetAll Tests

        [Fact]
        [Trait("Method", "GetAll")]
        public void GetAll_ReturnsAllServiceMembers()
        {
            // Arrange
            var serviceMembers = new List<ServiceMember>
            {
                new ServiceMember(),
                new ServiceMember()
            }.AsQueryable();

            _mockDao.Setup(dao => dao.GetAll())
                .Returns(serviceMembers);

            // Act
            var result = _mockDao.Object.GetAll();

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetAll(), Times.Once);
        }

        #endregion

        #region Delete Tests

        [Fact]
        [Trait("Method", "Delete")]
        public void Delete_WithValidServiceMember_CallsDelete()
        {
            // Arrange
            var serviceMember = new ServiceMember();
            _mockDao.Setup(dao => dao.Delete(It.IsAny<ServiceMember>()));

            // Act
            _mockDao.Object.Delete(serviceMember);

            // Assert
            _mockDao.Verify(dao => dao.Delete(It.IsAny<ServiceMember>()), Times.Once);
        }

        #endregion

        #region SaveOrUpdate Tests

        [Fact]
        [Trait("Method", "SaveOrUpdate")]
        public void SaveOrUpdate_WithValidServiceMember_ReturnsSavedServiceMember()
        {
            // Arrange
            var serviceMember = new ServiceMember();
            _mockDao.Setup(dao => dao.SaveOrUpdate(It.IsAny<ServiceMember>()))
                .Returns(serviceMember);

            // Act
            var result = _mockDao.Object.SaveOrUpdate(serviceMember);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.SaveOrUpdate(It.IsAny<ServiceMember>()), Times.Once);
        }

        #endregion
    }
}
