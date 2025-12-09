using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace ALOD.Data
{
    /// <summary>
    /// Data types for document service operations.
    /// Named DaoDocumentStatus to avoid conflict with ALOD.Core.Domain.Documents.DocumentStatus.
    /// </summary>
    public enum DaoDocumentStatus : byte
    {
        Pending = 1,
        Approved = 2,
        Deleted = 3,
        Test = 4,
        Batch = 5
    }

    /// <summary>
    /// Input type for document uploads.
    /// </summary>
    public enum InputType : byte
    {
        Upload = 1,
        Scan = 2,
        WebServiceUpload = 3
    }

    /// <summary>
    /// Keys for uploading a document.
    /// </summary>
    public class UploadKeys
    {
        public DateTime DocDate { get; set; }
        public string DocDescription { get; set; }
        public DaoDocumentStatus DocStatus { get; set; }
        public int DocTypeID { get; set; }
        public string EntityName { get; set; }
        public string FileName { get; set; }
        public InputType InputType { get; set; }
    }

    /// <summary>
    /// Keys for updating a document.
    /// </summary>
    public class DocumentKeys
    {
        public DateTime DocDate { get; set; }
        public string DocDescription { get; set; }
        public DaoDocumentStatus DocStatus { get; set; }
        public int DocTypeID { get; set; }
        public string EntityName { get; set; }
    }

    /// <summary>
    /// Document data returned from queries.
    /// </summary>
    public class DocumentData
    {
        public DateTime DocDate { get; set; }
        public string DocDescription { get; set; }
        public long DocID { get; set; }
        public DaoDocumentStatus DocStatus { get; set; }
        public int DocTypeID { get; set; }
        public string DocTypeName { get; set; }
        public string EntityName { get; set; }
        public string FileExt { get; set; }
        public string IconUrl { get; set; }
        public bool IsAppendable { get; set; }
        public string OriginalFileName { get; set; }
        public short PageCount { get; set; }
        public DateTime UploadDate { get; set; }
        public string UploadedBySubuserName { get; set; }
    }

    /// <summary>
    /// Document page data.
    /// </summary>
    public class DocumentPageData
    {
        public long DocPageID { get; set; }
        public short PageNumber { get; set; }
        public string PageUrl { get; set; }
    }

    /// <summary>
    /// Data access object for document service operations.
    /// Provides methods for managing documents, groups, and entities.
    /// </summary>
    public class DocumentServiceDao : IDisposable
    {
        private readonly SqlDataStore _dataStore;
        private bool _disposed;

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the DocumentServiceDao class.
        /// </summary>
        public DocumentServiceDao()
        {
            _dataStore = new SqlDataStore();
        }

        #endregion

        #region Group Operations

        /// <summary>
        /// Creates a new document group.
        /// </summary>
        /// <param name="groupName">The name of the group to create.</param>
        /// <param name="userID">The user ID creating the group.</param>
        /// <param name="subuserID">The subuser ID creating the group.</param>
        /// <param name="clientIP">The client IP address.</param>
        /// <returns>The ID of the newly created group.</returns>
        public long CreateGroup(string groupName, short userID, int subuserID, string clientIP)
        {
            DbCommand cmd = _dataStore.GetStoredProcCommand("dsp_Group_Insert");
            _dataStore.AddInParameter(cmd, "@GroupName", DbType.String, groupName);
            _dataStore.AddInParameter(cmd, "@UserID", DbType.Int16, userID);
            _dataStore.AddInParameter(cmd, "@SubuserID", DbType.Int32, subuserID);
            _dataStore.AddInParameter(cmd, "@ClientIP", DbType.String, clientIP ?? string.Empty);
            _dataStore.AddOutParameter(cmd, "@GroupID", DbType.Int64, 8);

            _dataStore.ExecuteNonQuery(cmd);

            object result = cmd.Parameters["@GroupID"].Value;
            return result != DBNull.Value ? Convert.ToInt64(result) : 0;
        }

        /// <summary>
        /// Gets the list of documents in a group.
        /// </summary>
        /// <param name="groupID">The group ID.</param>
        /// <param name="userID">The user ID requesting the list.</param>
        /// <param name="subuserID">The subuser ID requesting the list.</param>
        /// <returns>A list of document data for the group.</returns>
        public List<DocumentData> GetGroupDocumentList(long groupID, short userID, int subuserID)
        {
            var documents = new List<DocumentData>();

            DbCommand cmd = _dataStore.GetStoredProcCommand("dsp_Group_GetDocumentList");
            _dataStore.AddInParameter(cmd, "@GroupID", DbType.Int64, groupID);
            _dataStore.AddInParameter(cmd, "@UserID", DbType.Int16, userID);
            _dataStore.AddInParameter(cmd, "@SubuserID", DbType.Int32, subuserID);

            _dataStore.ExecuteReader((sender, reader) =>
            {
                var doc = new DocumentData
                {
                    DocID = sender.GetInt64(reader, 0),
                    DocDate = sender.GetDateTime(reader, 1),
                    DocDescription = sender.GetString(reader, 2),
                    DocStatus = (DaoDocumentStatus)sender.GetByte(reader, 3, 1),
                    DocTypeID = sender.GetInt32(reader, 4),
                    DocTypeName = sender.GetString(reader, 5),
                    EntityName = sender.GetString(reader, 6),
                    FileExt = sender.GetString(reader, 7),
                    OriginalFileName = sender.GetString(reader, 8),
                    PageCount = sender.GetInt16(reader, 9),
                    UploadDate = sender.GetDateTime(reader, 10),
                    UploadedBySubuserName = sender.GetString(reader, 11),
                    IsAppendable = sender.GetBoolean(reader, 12)
                };
                documents.Add(doc);
            }, cmd);

            return documents;
        }

        /// <summary>
        /// Moves a document from one group to another.
        /// </summary>
        /// <param name="docID">The document ID to move.</param>
        /// <param name="sourceGroupID">The source group ID.</param>
        /// <param name="targetGroupID">The target group ID.</param>
        /// <param name="userID">The user ID performing the move.</param>
        /// <param name="subuserID">The subuser ID performing the move.</param>
        /// <param name="clientIP">The client IP address.</param>
        public void MoveGroupDocument(long docID, long sourceGroupID, long targetGroupID, short userID, int subuserID, string clientIP)
        {
            DbCommand cmd = _dataStore.GetStoredProcCommand("dsp_Group_MoveDocument");
            _dataStore.AddInParameter(cmd, "@DocID", DbType.Int64, docID);
            _dataStore.AddInParameter(cmd, "@SourceGroupID", DbType.Int64, sourceGroupID);
            _dataStore.AddInParameter(cmd, "@TargetGroupID", DbType.Int64, targetGroupID);
            _dataStore.AddInParameter(cmd, "@UserID", DbType.Int16, userID);
            _dataStore.AddInParameter(cmd, "@SubuserID", DbType.Int32, subuserID);
            _dataStore.AddInParameter(cmd, "@ClientIP", DbType.String, clientIP ?? string.Empty);

            _dataStore.ExecuteNonQuery(cmd);
        }

        /// <summary>
        /// Copies documents from one group to another.
        /// </summary>
        /// <param name="oldGroupId">The source group ID.</param>
        /// <param name="newGroupId">The target group ID.</param>
        /// <param name="oldDocTypeId">The source document type ID.</param>
        /// <param name="newDocTypeId">The target document type ID.</param>
        /// <param name="userID">The user ID performing the copy.</param>
        /// <param name="subuserID">The subuser ID performing the copy.</param>
        /// <param name="clientIP">The client IP address.</param>
        public void CopyGroupDocuments(long oldGroupId, long newGroupId, long oldDocTypeId, long newDocTypeId, short userID, int subuserID, string clientIP)
        {
            DbCommand cmd = _dataStore.GetStoredProcCommand("dsp_Group_CopyDocuments");
            _dataStore.AddInParameter(cmd, "@OldGroupID", DbType.Int64, oldGroupId);
            _dataStore.AddInParameter(cmd, "@NewGroupID", DbType.Int64, newGroupId);
            _dataStore.AddInParameter(cmd, "@OldDocTypeID", DbType.Int64, oldDocTypeId);
            _dataStore.AddInParameter(cmd, "@NewDocTypeID", DbType.Int64, newDocTypeId);
            _dataStore.AddInParameter(cmd, "@UserID", DbType.Int16, userID);
            _dataStore.AddInParameter(cmd, "@SubuserID", DbType.Int32, subuserID);
            _dataStore.AddInParameter(cmd, "@ClientIP", DbType.String, clientIP ?? string.Empty);

            _dataStore.ExecuteNonQuery(cmd);
        }

        #endregion

        #region Document Operations

        /// <summary>
        /// Deletes a document.
        /// </summary>
        /// <param name="docID">The document ID to delete.</param>
        /// <param name="userID">The user ID performing the delete.</param>
        /// <param name="subuserID">The subuser ID performing the delete.</param>
        /// <param name="clientIP">The client IP address.</param>
        public void DeleteDocument(long docID, short userID, int subuserID, string clientIP)
        {
            DbCommand cmd = _dataStore.GetStoredProcCommand("dsp_Document_Delete");
            _dataStore.AddInParameter(cmd, "@DocID", DbType.Int64, docID);
            _dataStore.AddInParameter(cmd, "@UserID", DbType.Int16, userID);
            _dataStore.AddInParameter(cmd, "@SubuserID", DbType.Int32, subuserID);
            _dataStore.AddInParameter(cmd, "@ClientIP", DbType.String, clientIP ?? string.Empty);

            _dataStore.ExecuteNonQuery(cmd);
        }

        /// <summary>
        /// Updates the status of a document.
        /// </summary>
        /// <param name="docID">The document ID to update.</param>
        /// <param name="docStatus">The new status.</param>
        /// <param name="userID">The user ID performing the update.</param>
        /// <param name="subuserID">The subuser ID performing the update.</param>
        /// <param name="clientIP">The client IP address.</param>
        public void UpdateDocumentStatus(long docID, DaoDocumentStatus docStatus, short userID, int subuserID, string clientIP)
        {
            DbCommand cmd = _dataStore.GetStoredProcCommand("dsp_Document_UpdateStatus");
            _dataStore.AddInParameter(cmd, "@DocID", DbType.Int64, docID);
            _dataStore.AddInParameter(cmd, "@DocStatusID", DbType.Byte, (byte)docStatus);
            _dataStore.AddInParameter(cmd, "@UserID", DbType.Int16, userID);
            _dataStore.AddInParameter(cmd, "@SubuserID", DbType.Int32, subuserID);
            _dataStore.AddInParameter(cmd, "@ClientIP", DbType.String, clientIP ?? string.Empty);

            _dataStore.ExecuteNonQuery(cmd);
        }

        /// <summary>
        /// Updates the keys (metadata) of a document.
        /// </summary>
        /// <param name="docID">The document ID to update.</param>
        /// <param name="docKeys">The new document keys.</param>
        /// <param name="userID">The user ID performing the update.</param>
        /// <param name="subuserID">The subuser ID performing the update.</param>
        /// <param name="clientIP">The client IP address.</param>
        public void UpdateDocumentKeys(long docID, DocumentKeys docKeys, short userID, int subuserID, string clientIP)
        {
            if (docKeys == null)
                throw new ArgumentNullException(nameof(docKeys));

            DbCommand cmd = _dataStore.GetStoredProcCommand("dsp_Document_UpdateKeys");
            _dataStore.AddInParameter(cmd, "@DocID", DbType.Int64, docID);
            _dataStore.AddInParameter(cmd, "@DocDate", DbType.DateTime, docKeys.DocDate);
            _dataStore.AddInParameter(cmd, "@DocDescription", DbType.String, docKeys.DocDescription ?? string.Empty);
            _dataStore.AddInParameter(cmd, "@DocStatusID", DbType.Byte, (byte)docKeys.DocStatus);
            _dataStore.AddInParameter(cmd, "@DocTypeID", DbType.Int32, docKeys.DocTypeID);
            _dataStore.AddInParameter(cmd, "@EntityName", DbType.String, docKeys.EntityName ?? string.Empty);
            _dataStore.AddInParameter(cmd, "@UserID", DbType.Int16, userID);
            _dataStore.AddInParameter(cmd, "@SubuserID", DbType.Int32, subuserID);
            _dataStore.AddInParameter(cmd, "@ClientIP", DbType.String, clientIP ?? string.Empty);

            _dataStore.ExecuteNonQuery(cmd);
        }

        /// <summary>
        /// Uploads a new document.
        /// </summary>
        /// <param name="fileBytes">The file content as bytes.</param>
        /// <param name="uploadKeys">The upload keys/metadata.</param>
        /// <param name="groupID">The group ID to add the document to.</param>
        /// <param name="userID">The user ID uploading the document.</param>
        /// <param name="subuserID">The subuser ID uploading the document.</param>
        /// <param name="clientIP">The client IP address.</param>
        /// <returns>The ID of the newly uploaded document.</returns>
        public long UploadDocument(byte[] fileBytes, UploadKeys uploadKeys, long groupID, short userID, int subuserID, string clientIP)
        {
            if (fileBytes == null || fileBytes.Length == 0)
                throw new ArgumentException("File bytes cannot be null or empty.", nameof(fileBytes));

            if (uploadKeys == null)
                throw new ArgumentNullException(nameof(uploadKeys));

            // Insert the document record
            DbCommand cmd = _dataStore.GetStoredProcCommand("dsp_Document_Insert");
            _dataStore.AddInParameter(cmd, "@DocDate", DbType.DateTime, uploadKeys.DocDate);
            _dataStore.AddInParameter(cmd, "@DocDescription", DbType.String, uploadKeys.DocDescription ?? string.Empty);
            _dataStore.AddInParameter(cmd, "@DocStatusID", DbType.Byte, (byte)uploadKeys.DocStatus);
            _dataStore.AddInParameter(cmd, "@DocTypeID", DbType.Int32, uploadKeys.DocTypeID);
            _dataStore.AddInParameter(cmd, "@EntityName", DbType.String, uploadKeys.EntityName ?? string.Empty);
            _dataStore.AddInParameter(cmd, "@FileExt", DbType.String, FormatFileExt(uploadKeys.FileName));
            _dataStore.AddInParameter(cmd, "@OriginalFileName", DbType.String, FormatFileName(uploadKeys.FileName));
            _dataStore.AddInParameter(cmd, "@InputTypeID", DbType.Byte, (byte)uploadKeys.InputType);
            _dataStore.AddInParameter(cmd, "@OwnerUserID", DbType.Int16, userID);
            _dataStore.AddInParameter(cmd, "@OwnerSubuserID", DbType.Int32, subuserID);
            _dataStore.AddInParameter(cmd, "@DocGuid", DbType.Guid, Guid.NewGuid());
            _dataStore.AddInParameter(cmd, "@ClientIP", DbType.String, clientIP ?? string.Empty);
            _dataStore.AddOutParameter(cmd, "@DocID", DbType.Int64, 8);

            _dataStore.ExecuteNonQuery(cmd);

            object result = cmd.Parameters["@DocID"].Value;
            long docID = result != DBNull.Value ? Convert.ToInt64(result) : 0;

            if (docID > 0)
            {
                // Insert the document page with the file data
                AddDocumentPage(docID, fileBytes, userID, subuserID, clientIP);

                // Add document to group if specified
                if (groupID > 0)
                {
                    AddDocumentToGroup(docID, groupID, userID, subuserID, clientIP);
                }
            }

            return docID;
        }

        /// <summary>
        /// Adds a page to an existing document.
        /// </summary>
        /// <param name="docID">The document ID.</param>
        /// <param name="fileBytes">The page content as bytes.</param>
        /// <param name="userID">The user ID adding the page.</param>
        /// <param name="subuserID">The subuser ID adding the page.</param>
        /// <param name="clientIP">The client IP address.</param>
        /// <returns>The ID of the newly added page.</returns>
        public long AddDocumentPage(long docID, byte[] fileBytes, short userID, int subuserID, string clientIP)
        {
            DbCommand cmd = _dataStore.GetStoredProcCommand("dsp_DocumentPage_Insert");
            _dataStore.AddInParameter(cmd, "@UserID", DbType.Int16, userID);
            _dataStore.AddInParameter(cmd, "@SubuserID", DbType.Int32, subuserID);
            _dataStore.AddInParameter(cmd, "@DocID", DbType.Int64, docID);
            _dataStore.AddInParameter(cmd, "@ClientIP", DbType.String, clientIP ?? string.Empty);
            _dataStore.AddInParameter(cmd, "@FileData", DbType.Binary, fileBytes);
            _dataStore.AddOutParameter(cmd, "@DocPageID", DbType.Int64, 8);

            _dataStore.ExecuteNonQuery(cmd);

            object result = cmd.Parameters["@DocPageID"].Value;
            return result != DBNull.Value ? Convert.ToInt64(result) : 0;
        }

        /// <summary>
        /// Adds a document to a group.
        /// </summary>
        /// <param name="docID">The document ID.</param>
        /// <param name="groupID">The group ID.</param>
        /// <param name="userID">The user ID.</param>
        /// <param name="subuserID">The subuser ID.</param>
        /// <param name="clientIP">The client IP address.</param>
        public void AddDocumentToGroup(long docID, long groupID, short userID, int subuserID, string clientIP)
        {
            DbCommand cmd = _dataStore.GetStoredProcCommand("dsp_Group_AddDocument");
            _dataStore.AddInParameter(cmd, "@DocID", DbType.Int64, docID);
            _dataStore.AddInParameter(cmd, "@GroupID", DbType.Int64, groupID);
            _dataStore.AddInParameter(cmd, "@UserID", DbType.Int16, userID);
            _dataStore.AddInParameter(cmd, "@SubuserID", DbType.Int32, subuserID);
            _dataStore.AddInParameter(cmd, "@ClientIP", DbType.String, clientIP ?? string.Empty);

            _dataStore.ExecuteNonQuery(cmd);
        }

        #endregion

        #region Entity Operations

        /// <summary>
        /// Gets the list of documents for an entity.
        /// </summary>
        /// <param name="entityID">The entity ID.</param>
        /// <param name="userID">The user ID requesting the list.</param>
        /// <param name="subuserID">The subuser ID requesting the list.</param>
        /// <returns>A list of document data for the entity.</returns>
        public List<DocumentData> GetEntityDocumentList(string entityID, short userID, int subuserID)
        {
            var documents = new List<DocumentData>();

            DbCommand cmd = _dataStore.GetStoredProcCommand("dsp_Entity_GetDocumentList");
            _dataStore.AddInParameter(cmd, "@EntityName", DbType.String, entityID ?? string.Empty);
            _dataStore.AddInParameter(cmd, "@UserID", DbType.Int16, userID);
            _dataStore.AddInParameter(cmd, "@SubuserID", DbType.Int32, subuserID);

            _dataStore.ExecuteReader((sender, reader) =>
            {
                var doc = new DocumentData
                {
                    DocID = sender.GetInt64(reader, 0),
                    DocDate = sender.GetDateTime(reader, 1),
                    DocDescription = sender.GetString(reader, 2),
                    DocStatus = (DaoDocumentStatus)sender.GetByte(reader, 3, 1),
                    DocTypeID = sender.GetInt32(reader, 4),
                    DocTypeName = sender.GetString(reader, 5),
                    EntityName = sender.GetString(reader, 6),
                    FileExt = sender.GetString(reader, 7),
                    OriginalFileName = sender.GetString(reader, 8),
                    PageCount = sender.GetInt16(reader, 9),
                    UploadDate = sender.GetDateTime(reader, 10),
                    UploadedBySubuserName = sender.GetString(reader, 11),
                    IsAppendable = sender.GetBoolean(reader, 12)
                };
                documents.Add(doc);
            }, cmd);

            return documents;
        }

        #endregion

        #region URL Generation

        /// <summary>
        /// Generates a document upload URL with a GUID for secure access.
        /// </summary>
        /// <param name="entityName">The entity name.</param>
        /// <param name="docTypeID">The document type ID.</param>
        /// <param name="groupID">The group ID.</param>
        /// <param name="userID">The user ID.</param>
        /// <param name="subuserID">The subuser ID.</param>
        /// <returns>A GUID that can be used to construct the upload URL.</returns>
        public Guid CreateDocumentUploadGuid(string entityName, int docTypeID, long groupID, short userID, int subuserID)
        {
            Guid docGuid = Guid.NewGuid();

            DbCommand cmd = _dataStore.GetStoredProcCommand("dsp_DocumentGuid_Insert");
            _dataStore.AddInParameter(cmd, "@DocGuid", DbType.Guid, docGuid);
            _dataStore.AddInParameter(cmd, "@UserID", DbType.Int16, userID);
            _dataStore.AddInParameter(cmd, "@SubuserID", DbType.Int32, subuserID);
            _dataStore.AddInParameter(cmd, "@EntityName", DbType.String, entityName ?? string.Empty);
            _dataStore.AddInParameter(cmd, "@DocTypeID", DbType.Int32, docTypeID);
            _dataStore.AddInParameter(cmd, "@GroupID", DbType.Int64, groupID);

            _dataStore.ExecuteNonQuery(cmd);

            return docGuid;
        }

        /// <summary>
        /// Generates a document viewer URL with a GUID for secure access.
        /// </summary>
        /// <param name="docID">The document ID.</param>
        /// <param name="isReadOnly">Whether the viewer should be read-only.</param>
        /// <param name="userID">The user ID.</param>
        /// <param name="subuserID">The subuser ID.</param>
        /// <returns>A GUID that can be used to construct the viewer URL.</returns>
        public Guid CreateDocumentViewerGuid(long docID, bool isReadOnly, short userID, int subuserID)
        {
            Guid docGuid = Guid.NewGuid();

            DbCommand cmd = _dataStore.GetStoredProcCommand("dsp_DocumentGuid_InsertForViewer");
            _dataStore.AddInParameter(cmd, "@DocGuid", DbType.Guid, docGuid);
            _dataStore.AddInParameter(cmd, "@DocID", DbType.Int64, docID);
            _dataStore.AddInParameter(cmd, "@UserID", DbType.Int16, userID);
            _dataStore.AddInParameter(cmd, "@SubuserID", DbType.Int32, subuserID);
            _dataStore.AddInParameter(cmd, "@IsReadOnly", DbType.Boolean, isReadOnly);

            _dataStore.ExecuteNonQuery(cmd);

            return docGuid;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Formats a filename to extract only the file name without path.
        /// </summary>
        private static string FormatFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return string.Empty;

            int lastSlash = fileName.LastIndexOfAny(new[] { '\\', '/' });
            return lastSlash >= 0 ? fileName.Substring(lastSlash + 1) : fileName;
        }

        /// <summary>
        /// Extracts the file extension from a filename.
        /// </summary>
        private static string FormatFileExt(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return string.Empty;

            int lastDot = fileName.LastIndexOf('.');
            return lastDot >= 0 ? fileName.Substring(lastDot + 1).ToLowerInvariant() : string.Empty;
        }

        #endregion

        #region IDisposable Support

        /// <summary>
        /// Releases all resources used by this instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources if any
                }

                _disposed = true;
            }
        }

        #endregion
    }
}
