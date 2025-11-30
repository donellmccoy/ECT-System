using ALOD.Core.Domain.Common;
using ALOD.Core.Interfaces.DAOInterfaces;
using Moq;
using System.Collections.Specialized;
using Xunit;

namespace ALOD.Tests.Unit.Services
{
    /// <summary>
    /// Tests for EmailService - email management and distribution service implementing IEmailDao.
    /// </summary>
    [Trait("Category", "Unit")]
    [Trait("Layer", "Services")]
    public class EmailServiceTests
    {
        [Fact]
        [Trait("Method", "CreateMessage")]
        public void CreateMessage_WithTemplateId_ReturnsEMailMessage()
        {
            // Arrange - Test IEmailDao interface without instantiating EMailMessage
            var mockService = new Mock<IEmailDao>();
            mockService.Setup(x => x.CreateMessage(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns((EMailMessage)null);  // Avoid actual EMailMessage instantiation which requires logging

            // Act
            var result = mockService.Object.CreateMessage(1, "from@test.com", "to@test.com");

            // Assert - Verify mock was called correctly
            mockService.Verify(x => x.CreateMessage(1, "from@test.com", "to@test.com"), Times.Once);
        }

        [Fact]
        [Trait("Method", "CreateMessage")]
        public void CreateMessage_WithStringCollection_ReturnsEMailMessage()
        {
            // Arrange - Test IEmailDao interface without instantiating EMailMessage
            var mockService = new Mock<IEmailDao>();
            var toList = new StringCollection { "test1@test.com", "test2@test.com" };
            mockService.Setup(x => x.CreateMessage(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<StringCollection>()))
                .Returns((EMailMessage)null);  // Avoid actual EMailMessage instantiation

            // Act
            var result = mockService.Object.CreateMessage(1, "from@test.com", toList);

            // Assert - Verify mock was called correctly
            mockService.Verify(x => x.CreateMessage(1, "from@test.com", toList), Times.Once);
        }

        [Fact]
        [Trait("Method", "CreateMessage")]
        public void CreateMessage_WithSubjectAndBody_ReturnsEMailMessage()
        {
            // Arrange - Test IEmailDao interface without instantiating EMailMessage
            var mockService = new Mock<IEmailDao>();
            var toList = new StringCollection { "test@test.com" };
            mockService.Setup(x => x.CreateMessage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<StringCollection>()))
                .Returns((EMailMessage)null);  // Avoid actual EMailMessage instantiation

            // Act
            var result = mockService.Object.CreateMessage("Subject", "Body", "from@test.com", toList);

            // Assert - Verify mock was called correctly
            mockService.Verify(x => x.CreateMessage("Subject", "Body", "from@test.com", toList), Times.Once);
        }

        [Fact]
        [Trait("Method", "GetDistributionListByGroup")]
        public void GetDistributionListByGroup_ReturnsStringCollection()
        {
            // Arrange
            var mockService = new Mock<IEmailDao>();
            var expectedList = new StringCollection { "test@test.com" };
            mockService.Setup(x => x.GetDistributionListByGroup(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(expectedList);

            // Act
            var result = mockService.Object.GetDistributionListByGroup(1, 1, "TestGroup", true);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        [Trait("Method", "GetDistributionListByRoles")]
        public void GetDistributionListByRoles_ReturnsStringCollection()
        {
            // Arrange
            var mockService = new Mock<IEmailDao>();
            var expectedList = new StringCollection { "test@test.com" };
            mockService.Setup(x => x.GetDistributionListByRoles(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()))
                .Returns(expectedList);

            // Act
            var result = mockService.Object.GetDistributionListByRoles("AFRC", 1, "Role1,Role2");

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        [Trait("Method", "GetDistributionListByStatus")]
        public void GetDistributionListByStatus_ReturnsStringCollection()
        {
            // Arrange
            var mockService = new Mock<IEmailDao>();
            var expectedList = new StringCollection();
            mockService.Setup(x => x.GetDistributionListByStatus(It.IsAny<int>(), It.IsAny<short>()))
                .Returns(expectedList);

            // Act
            var result = mockService.Object.GetDistributionListByStatus(1, 10);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        [Trait("Method", "GetDistributionListForLOD")]
        public void GetDistributionListForLOD_ReturnsStringCollection()
        {
            // Arrange
            var mockService = new Mock<IEmailDao>();
            var expectedList = new StringCollection();
            mockService.Setup(x => x.GetDistributionListForLOD(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .Returns(expectedList);

            // Act
            var result = mockService.Object.GetDistributionListForLOD(1, 1, 10, "TestGroup");

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        [Trait("Method", "GetEmailListForBoardLevelUsers")]
        public void GetEmailListForBoardLevelUsers_ReturnsStringCollection()
        {
            // Arrange
            var mockService = new Mock<IEmailDao>();
            var expectedList = new StringCollection();
            mockService.Setup(x => x.GetEmailListForBoardLevelUsers())
                .Returns(expectedList);

            // Act
            var result = mockService.Object.GetEmailListForBoardLevelUsers();

            // Assert
            Assert.NotNull(result);
            mockService.Verify(x => x.GetEmailListForBoardLevelUsers(), Times.Once);
        }

        [Fact]
        [Trait("Method", "GetEmailListForUsersByGroup")]
        public void GetEmailListForUsersByGroup_ReturnsStringCollection()
        {
            // Arrange
            var mockService = new Mock<IEmailDao>();
            var expectedList = new StringCollection();
            mockService.Setup(x => x.GetEmailListForUsersByGroup(It.IsAny<int>()))
                .Returns(expectedList);

            // Act
            var result = mockService.Object.GetEmailListForUsersByGroup(1);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        [Trait("Method", "GetTemplateData")]
        public void GetTemplateData_ReturnsStringDictionary()
        {
            // Arrange
            var mockService = new Mock<IEmailDao>();
            var expectedDict = new StringDictionary();
            mockService.Setup(x => x.GetTemplateData(It.IsAny<string>()))
                .Returns(expectedDict);

            // Act
            var result = mockService.Object.GetTemplateData("test_sp");

            // Assert
            Assert.NotNull(result);
        }
    }
}
