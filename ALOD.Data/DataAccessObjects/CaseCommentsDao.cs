using ALOD.Core.Domain.Common;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Core.Utils;
using System;
using System.Collections.Generic;
using System.Data;

namespace ALOD.Data
{
    /// <summary>
    /// Data access object for managing case comments, dialogue comments, and child comments.
    /// Handles comment operations including retrieval, creation, updating, and deletion for various case types.
    /// </summary>
    public class CaseCommentsDao : ICaseCommentsDao
    {
        private SqlDataStore _dataSource;

        /// <summary>
        /// Gets the SQL data store instance for database operations.
        /// </summary>
        private SqlDataStore DataSource
        {
            get
            {
                if (_dataSource == null)
                {
                    _dataSource = new SqlDataStore();
                }
                return _dataSource;
            }
        }

        /// <summary>
        /// Retrieves all case comments for a specific case.
        /// </summary>
        /// <param name="refId">The reference ID of the case. Must be greater than 0.</param>
        /// <param name="module">The module identifier. Must be greater than 0.</param>
        /// <param name="commentType">The type of comments to retrieve.</param>
        /// <param name="sorted">If true, returns comments sorted by date; otherwise, unsorted.</param>
        /// <returns>A list of CaseComments entities for the specified case.</returns>
        public IList<CaseComments> GetByCase(int refId, int module, int commentType, bool sorted)
        {
            DataSet dSet = DataSource.ExecuteDataSet("core_CaseComments_GetByCase", refId, module, commentType, sorted);

            return DataHelpers.ExtractObjectsFromDataSet<CaseComments>(dSet, new NHibernateDaoFactory());
        }

        /// <summary>
        /// Retrieves all dialogue comments for a specific case.
        /// </summary>
        /// <param name="refId">The reference ID of the case. Must be greater than 0.</param>
        /// <param name="module">The module identifier. Must be greater than 0.</param>
        /// <param name="commentType">The type of dialogue comments to retrieve.</param>
        /// <param name="sorted">If true, returns comments sorted by date; otherwise, unsorted.</param>
        /// <returns>A list of CaseDialogueComments entities for the specified case.</returns>
        public IList<CaseDialogueComments> GetByCaseDialogue(int refId, int module, int commentType, bool sorted)
        {
            DataSet dSet = DataSource.ExecuteDataSet("core_CaseDialogueComments_GetByCase", refId, module, commentType, sorted);

            return DataHelpers.ExtractObjectsFromDataSet<CaseDialogueComments>(dSet, new NHibernateDaoFactory());
        }

        /// <summary>
        /// Retrieves a specific case comment by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the case comment. Must be greater than 0.</param>
        /// <returns>The CaseComments entity with the specified ID, or null if not found.</returns>
        public CaseComments GetById(int id)
        {
            DataSet dSet = DataSource.ExecuteDataSet("core_CaseComments_GetById", id);

            return DataHelpers.ExtractObjectFromDataSet<CaseComments>(dSet, new NHibernateDaoFactory());
        }

        /// <summary>
        /// Retrieves all child comments for a specific parent comment in a case.
        /// </summary>
        /// <param name="refId">The reference ID of the case. Must be greater than 0.</param>
        /// <param name="module">The module identifier. Must be greater than 0.</param>
        /// <param name="commentType">The type of child comments to retrieve.</param>
        /// <param name="parentCommentId">The ID of the parent comment. Must be greater than 0.</param>
        /// <returns>A list of ChildCaseComments entities for the specified parent comment.</returns>
        public IList<ChildCaseComments> GetChildByCase(int refId, int module, int commentType, int parentCommentId)
        {
            DataSet dSet = DataSource.ExecuteDataSet("core_ChildCaseComment_GetByCase", refId, module, commentType, parentCommentId);

            return DataHelpers.ExtractObjectsFromDataSet<ChildCaseComments>(dSet, new NHibernateDaoFactory());
        }

        /// <summary>
        /// Retrieves a specific child comment by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the child comment. Must be greater than 0.</param>
        /// <returns>The ChildCaseComments entity with the specified ID, or null if not found.</returns>
        public ChildCaseComments GetChildById(int id)
        {
            DataSet dSet = DataSource.ExecuteDataSet("core_ChildCaseComment_GetById", id);

            return DataHelpers.ExtractObjectFromDataSet<ChildCaseComments>(dSet, new NHibernateDaoFactory());
        }

        // Child Comment of Parent Comment
        /// <summary>
        /// Retrieves a specific dialogue comment by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the dialogue comment. Must be greater than 0.</param>
        /// <returns>The CaseDialogueComments entity with the specified ID, or null if not found.</returns>
        public CaseDialogueComments GetDialogueById(int id)
        {
            DataSet dSet = DataSource.ExecuteDataSet("core_CaseDialogueComments_GetById", id);

            return DataHelpers.ExtractObjectFromDataSet<CaseDialogueComments>(dSet, new NHibernateDaoFactory());
        }

        /// <summary>
        /// Inserts a new case comment or updates an existing one.
        /// </summary>
        /// <param name="id">The comment ID (0 for insert, existing ID for update).</param>
        /// <param name="refId">The reference ID of the case. Must be greater than 0.</param>
        /// <param name="module">The module identifier. Must be greater than 0.</param>
        /// <param name="comment">The comment text. Must not be null or empty.</param>
        /// <param name="userId">The ID of the user creating/updating the comment. Must be greater than 0.</param>
        /// <param name="createdDate">The date and time the comment was created.</param>
        /// <param name="deleted">True if the comment is marked as deleted; otherwise, false.</param>
        /// <param name="commentType">The type of comment.</param>
        public void SaveOrUpdate(int id, int refId, int module, string comment, int userId, DateTime createdDate, bool deleted, int commentType)
        {
            DataSource.ExecuteNonQuery("core_CaseComments_SaveOrUpdateComment", id, refId, module, comment, userId, createdDate, deleted, commentType);
        }

        /// <summary>
        /// Inserts a new child comment or updates an existing one.
        /// </summary>
        /// <param name="commentId">The parent comment ID. Must be greater than 0.</param>
        /// <param name="id">The child comment ID (0 for insert, existing ID for update).</param>
        /// <param name="refId">The reference ID of the case. Must be greater than 0.</param>
        /// <param name="module">The module identifier. Must be greater than 0.</param>
        /// <param name="comment">The comment text. Must not be null or empty.</param>
        /// <param name="userId">The ID of the user creating/updating the comment. Must be greater than 0.</param>
        /// <param name="createdDate">The date and time the comment was created.</param>
        /// <param name="commentType">The type of comment.</param>
        /// <param name="role">The role of the user posting the comment.</param>
        public void SaveOrUpdateChildComment(int commentId, int id, int refId, int module, string comment, int userId, DateTime createdDate, int commentType, string role)
        {
            DataSource.ExecuteNonQuery("core_ChildCaseComments_SaveOrUpdateComment", commentId, id, refId, module, comment, userId, createdDate, commentType, role);
        }

        /// <summary>
        /// Inserts a new dialogue comment or updates an existing one.
        /// </summary>
        /// <param name="id">The dialogue comment ID (0 for insert, existing ID for update).</param>
        /// <param name="refId">The reference ID of the case. Must be greater than 0.</param>
        /// <param name="module">The module identifier. Must be greater than 0.</param>
        /// <param name="comment">The comment text. Must not be null or empty.</param>
        /// <param name="userId">The ID of the user creating/updating the comment. Must be greater than 0.</param>
        /// <param name="createdDate">The date and time the comment was created.</param>
        /// <param name="deleted">True if the comment is marked as deleted; otherwise, false.</param>
        /// <param name="commentType">The type of comment.</param>
        /// <param name="role">The role of the user posting the comment.</param>
        public void SaveOrUpdateDialogue(int id, int refId, int module, string comment, int userId, DateTime createdDate, bool deleted, int commentType, string role)
        {
            DataSource.ExecuteNonQuery("core_CaseDialogueComments_SaveOrUpdateComment", id, refId, module, comment, userId, createdDate, deleted, commentType, role);
        }
    }
}