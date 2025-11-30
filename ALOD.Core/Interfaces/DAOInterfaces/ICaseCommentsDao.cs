using ALOD.Core.Domain.Common;
using System;
using System.Collections.Generic;

namespace ALOD.Core.Interfaces.DAOInterfaces
{
    public interface ICaseCommentsDao
    {
        IList<CaseComments> GetByCase(int refId, int module, int commentType, bool sorted);

        IList<CaseDialogueComments> GetByCaseDialogue(int refId, int module, int commentType, bool sorted);

        CaseComments GetById(int id);

        IList<ChildCaseComments> GetChildByCase(int refId, int module, int commentType, int parentCommentId);

        ChildCaseComments GetChildById(int id);

        CaseDialogueComments GetDialogueById(int id);

        void SaveOrUpdate(int id, int refId, int module, string comment, int userId, DateTime createdDate, bool deleted, int commentType);

        void SaveOrUpdateChildComment(int commentId, int id, int refId, int module, string comment, int userId, DateTime createdDate, int commentType, string role);

        void SaveOrUpdateDialogue(int id, int refId, int module, string comment, int userId, DateTime createdDate, bool deleted, int commentType, string role);
    }
}