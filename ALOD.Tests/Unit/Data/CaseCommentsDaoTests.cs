using ALOD.Core.Domain.Common;
using ALOD.Core.Interfaces.DAOInterfaces;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ALOD.Tests.Unit.Data
{
    [Trait("Category", "Unit")]
    [Trait("Layer", "Data")]
    public class CaseCommentsDaoTests
    {
        private readonly Mock<ICaseCommentsDao> _mockDao;

        public CaseCommentsDaoTests()
        {
            _mockDao = new Mock<ICaseCommentsDao>();
        }

        #region GetById Tests

        [Fact]
        [Trait("Method", "GetById")]
        public void GetById_WithValidId_ReturnsCaseComments()
        {
            // Arrange
            var caseComments = new CaseComments();
            _mockDao.Setup(dao => dao.GetById(It.IsAny<int>()))
                .Returns(caseComments);

            // Act
            var result = _mockDao.Object.GetById(1);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetById(It.IsAny<int>()), Times.Once);
        }

        #endregion

        #region GetByCase Tests

        [Fact]
        [Trait("Method", "GetByCase")]
        public void GetByCase_WithValidParameters_ReturnsCaseCommentsList()
        {
            // Arrange
            var commentsList = new List<CaseComments> { new CaseComments() };
            _mockDao.Setup(dao => dao.GetByCase(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()))
                .Returns(commentsList);

            // Act
            var result = _mockDao.Object.GetByCase(1, 1, 1, true);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetByCase(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()), Times.Once);
        }

        #endregion

        #region GetByCaseDialogue Tests

        [Fact]
        [Trait("Method", "GetByCaseDialogue")]
        public void GetByCaseDialogue_WithValidParameters_ReturnsCaseDialogueCommentsList()
        {
            // Arrange
            var dialogueCommentsList = new List<CaseDialogueComments> { new CaseDialogueComments() };
            _mockDao.Setup(dao => dao.GetByCaseDialogue(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()))
                .Returns(dialogueCommentsList);

            // Act
            var result = _mockDao.Object.GetByCaseDialogue(1, 1, 1, true);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetByCaseDialogue(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()), Times.Once);
        }

        #endregion

        #region GetChildByCase Tests

        [Fact]
        [Trait("Method", "GetChildByCase")]
        public void GetChildByCase_WithValidParameters_ReturnsChildCaseCommentsList()
        {
            // Arrange
            var childCommentsList = new List<ChildCaseComments> { new ChildCaseComments() };
            _mockDao.Setup(dao => dao.GetChildByCase(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(childCommentsList);

            // Act
            var result = _mockDao.Object.GetChildByCase(1, 1, 1, 1);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetChildByCase(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once);
        }

        #endregion

        #region GetChildById Tests

        [Fact]
        [Trait("Method", "GetChildById")]
        public void GetChildById_WithValidId_ReturnsChildCaseComments()
        {
            // Arrange
            var childComments = new ChildCaseComments();
            _mockDao.Setup(dao => dao.GetChildById(It.IsAny<int>()))
                .Returns(childComments);

            // Act
            var result = _mockDao.Object.GetChildById(1);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetChildById(It.IsAny<int>()), Times.Once);
        }

        #endregion

        #region GetDialogueById Tests

        [Fact]
        [Trait("Method", "GetDialogueById")]
        public void GetDialogueById_WithValidId_ReturnsCaseDialogueComments()
        {
            // Arrange
            var dialogueComments = new CaseDialogueComments();
            _mockDao.Setup(dao => dao.GetDialogueById(It.IsAny<int>()))
                .Returns(dialogueComments);

            // Act
            var result = _mockDao.Object.GetDialogueById(1);

            // Assert
            Assert.NotNull(result);
            _mockDao.Verify(dao => dao.GetDialogueById(It.IsAny<int>()), Times.Once);
        }

        #endregion

        #region SaveOrUpdate Tests

        [Fact]
        [Trait("Method", "SaveOrUpdate")]
        public void SaveOrUpdate_WithValidParameters_CallsSaveOrUpdate()
        {
            // Arrange
            _mockDao.Setup(dao => dao.SaveOrUpdate(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), 
                It.IsAny<string>(), It.IsAny<int>(), It.IsAny<DateTime>(), 
                It.IsAny<bool>(), It.IsAny<int>()));

            // Act
            _mockDao.Object.SaveOrUpdate(1, 1, 1, "comment", 1, DateTime.Now, false, 1);

            // Assert
            _mockDao.Verify(dao => dao.SaveOrUpdate(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), 
                It.IsAny<string>(), It.IsAny<int>(), It.IsAny<DateTime>(), 
                It.IsAny<bool>(), It.IsAny<int>()), Times.Once);
        }

        #endregion

        #region SaveOrUpdateChildComment Tests

        [Fact]
        [Trait("Method", "SaveOrUpdateChildComment")]
        public void SaveOrUpdateChildComment_WithValidParameters_CallsSaveOrUpdateChildComment()
        {
            // Arrange
            _mockDao.Setup(dao => dao.SaveOrUpdateChildComment(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<string>(), It.IsAny<int>(), It.IsAny<DateTime>(), 
                It.IsAny<int>(), It.IsAny<string>()));

            // Act
            _mockDao.Object.SaveOrUpdateChildComment(1, 1, 1, 1, "comment", 1, DateTime.Now, 1, "role");

            // Assert
            _mockDao.Verify(dao => dao.SaveOrUpdateChildComment(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<string>(), It.IsAny<int>(), It.IsAny<DateTime>(), 
                It.IsAny<int>(), It.IsAny<string>()), Times.Once);
        }

        #endregion

        #region SaveOrUpdateDialogue Tests

        [Fact]
        [Trait("Method", "SaveOrUpdateDialogue")]
        public void SaveOrUpdateDialogue_WithValidParameters_CallsSaveOrUpdateDialogue()
        {
            // Arrange
            _mockDao.Setup(dao => dao.SaveOrUpdateDialogue(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), 
                It.IsAny<string>(), It.IsAny<int>(), It.IsAny<DateTime>(), 
                It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<string>()));

            // Act
            _mockDao.Object.SaveOrUpdateDialogue(1, 1, 1, "comment", 1, DateTime.Now, false, 1, "role");

            // Assert
            _mockDao.Verify(dao => dao.SaveOrUpdateDialogue(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), 
                It.IsAny<string>(), It.IsAny<int>(), It.IsAny<DateTime>(), 
                It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<string>()), Times.Once);
        }

        #endregion
    }
}
