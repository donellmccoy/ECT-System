using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using SRXLite.DataAccess;
using SRXLite.DataTypes;
using static SRXLite.Modules.Util;
using static SRXLite.DataAccess.DB;

namespace SRXLite.Classes
{
    /// <summary>
    /// Group class for managing document groups
    /// </summary>
    public class Group : IDisposable
    {
        private HttpContext _context;
        private AsyncDB _db;
        private string _errorMessage;
        private long _groupID;
        private bool _hasErrors = false;
        private int _subuserID;
        private AsyncCallback _userCallback;
        private short _userID;

        #region Constructors

        /// <summary>
        /// Constructor with user ID
        /// </summary>
        /// <param name="userID">ID of the requesting user.</param>
        public Group(short userID)
        {
            _db = new AsyncDB(HandleError);
            _userID = userID;
            _context = HttpContext.Current;
        }

        /// <summary>
        /// Constructor with user ID, subuser ID, and group ID
        /// </summary>
        /// <param name="userID">ID of the requesting user.</param>
        /// <param name="subuserID">Subuser ID.</param>
        /// <param name="groupID">ID of an existing group.</param>
        public Group(short userID, int subuserID, long groupID)
        {
            _db = new AsyncDB(HandleError);
            _userID = userID;
            _subuserID = subuserID;
            _groupID = groupID;
            _context = HttpContext.Current;
        }

        /// <summary>
        /// Constructor with user object
        /// </summary>
        /// <param name="user"></param>
        public Group(User user)
        {
            _db = new AsyncDB(HandleError);
            _userID = user.UserID;
            _subuserID = user.SubuserID;
            _context = HttpContext.Current;
        }

        /// <summary>
        /// Constructor with user object and group ID
        /// </summary>
        /// <param name="user"></param>
        /// <param name="groupID"></param>
        public Group(User user, long groupID)
        {
            _db = new AsyncDB(HandleError);
            _userID = user.UserID;
            _subuserID = user.SubuserID;
            _groupID = groupID;
            _context = HttpContext.Current;
        }

        #endregion

        #region Properties

        public string ErrorMessage
        {
            get { return _errorMessage; }
        }

        public bool HasErrors
        {
            get { return _hasErrors; }
        }

        #endregion

        #region AddDocument

        /// <summary>
        /// Starts an asynchronous operation for adding a document to the group.
        /// </summary>
        /// <param name="docID"></param>
        /// <param name="callback"></param>
        /// <param name="stateObject"></param>
        /// <returns></returns>
        public IAsyncResult BeginAddDocument(long docID, AsyncCallback callback, object stateObject)
        {
            SqlCommand command = new SqlCommand();
            command.CommandText = "dsp_GroupDocument_Insert";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(GetSqlParameter("@UserID", _userID));
            command.Parameters.Add(GetSqlParameter("@SubuserID", _subuserID));
            command.Parameters.Add(GetSqlParameter("@GroupID", _groupID));
            command.Parameters.Add(GetSqlParameter("@DocID", docID));
            command.Parameters.Add(GetSqlParameter("@ClientIP", GetClientIP(_context)));

            return _db.BeginExecuteNonQuery(command, callback, stateObject);
        }

        /// <summary>
        /// Ends the asynchronous operation
        /// </summary>
        /// <param name="result"></param>
        public void EndAddDocument(IAsyncResult result)
        {
            _db.EndExecuteNonQuery(result);
        }

        #endregion

        #region Create

        /// <summary>
        /// Starts an asynchronous operation for creating a new group.
        /// </summary>
        /// <param name="groupName">A name for the group.</param>
        /// <param name="callback"></param>
        /// <param name="stateObject"></param>
        /// <returns></returns>
        public IAsyncResult BeginCreate(string groupName, AsyncCallback callback, object stateObject)
        {
            SqlCommand command = new SqlCommand();
            command.CommandText = "dsp_Group_Insert";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(GetSqlParameter("@SubuserID", _subuserID));
            command.Parameters.Add(GetSqlParameter("@GroupName", groupName));
            command.Parameters.Add(GetSqlParameter("@OwnerUserID", _userID));
            command.Parameters.Add(GetSqlParameter("@ClientIP", GetClientIP(_context)));
            command.Parameters.Add(GetSqlParameter("@GroupID", null, SqlDbType.BigInt, ParameterDirection.Output));

            return _db.BeginExecuteNonQuery(command, callback, stateObject);
        }

        /// <summary>
        /// Ends the asynchronous operation and returns the group ID.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public long EndCreate(IAsyncResult result)
        {
            _db.EndExecuteNonQuery(result, "@GroupID", ref _groupID);
            return _groupID;
        }

        #endregion

        #region CreateCopy

        /// <summary>
        /// Starts an asynchronous operation for creating a new group and
        /// copying document references from the specified source group.
        /// </summary>
        /// <param name="groupName">A name for the group.</param>
        /// <param name="sourceGroupID">The ID of an existing group from which to copy.</param>
        /// <param name="callback"></param>
        /// <param name="stateObject"></param>
        /// <returns></returns>
        public IAsyncResult BeginCreateCopy(string groupName, long sourceGroupID, AsyncCallback callback, object stateObject)
        {
            SqlCommand command = new SqlCommand();
            command.CommandText = "dsp_Group_InsertCopy";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(GetSqlParameter("@SubuserID", _subuserID));
            command.Parameters.Add(GetSqlParameter("@GroupName", groupName));
            command.Parameters.Add(GetSqlParameter("@SourceGroupID", sourceGroupID));
            command.Parameters.Add(GetSqlParameter("@OwnerUserID", _userID));
            command.Parameters.Add(GetSqlParameter("@ClientIP", GetClientIP(_context)));
            command.Parameters.Add(GetSqlParameter("@GroupID", null, SqlDbType.BigInt, ParameterDirection.Output));

            return _db.BeginExecuteNonQuery(command, callback, stateObject);
        }

        /// <summary>
        /// Ends the asynchronous operation and returns the group ID.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public long EndCreateCopy(IAsyncResult result)
        {
            _db.EndExecuteNonQuery(result, "@GroupID", ref _groupID);
            return _groupID;
        }

        #endregion

        #region GetDocumentCount

        /// <summary>
        /// Returns a count of the number of documents in the group.
        /// </summary>
        /// <returns></returns>
        public int GetDocumentCount()
        {
            return IntCheck(DB.ExecuteScalar("dsp_Group_GetDocumentCount " + _groupID));
        }

        #endregion

        #region GetDocumentList

        /// <summary>
        /// Returns document data for all documents in the group.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="stateObject"></param>
        /// <returns></returns>
        public IAsyncResult BeginGetDocumentList(AsyncCallback callback, object stateObject)
        {
            SqlCommand command = new SqlCommand();
            command.CommandText = "dsp_Group_GetDocumentList";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(GetSqlParameter("@UserID", _userID));
            command.Parameters.Add(GetSqlParameter("@SubuserID", _subuserID));
            command.Parameters.Add(GetSqlParameter("@GroupID", _groupID));

            return _db.BeginExecuteReader(command, callback, stateObject);
        }

        /// <summary>
        /// Ends the async operation and returns document list
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public List<DocumentData> EndGetDocumentList(IAsyncResult result)
        {
            List<DocumentData> list = new List<DocumentData>();
            DocumentData docData;
            string appRootUrl = GetURL(_context);
            CryptoManager crypto = new CryptoManager(_context);

            using (SqlDataReader reader = _db.EndExecuteReader(result))
            {
                while (reader.Read())
                {
                    docData = new DocumentData();
                    docData.DocDate = DateCheck(reader["DocDate"], DateTime.MinValue);
                    docData.DocDescription = NullCheck(reader["DocDescription"]);
                    docData.DocID = LngCheck(reader["DocID"]);
                    docData.DocStatus = (DocumentStatus)reader["DocStatusID"];
                    docData.DocTypeID = IntCheck(reader["DocTypeID"]);
                    docData.DocTypeName = NullCheck(reader["DocTypeName"]);
                    docData.EntityName = NullCheck(reader["EntityName"]);
                    docData.FileExt = NullCheck(reader["FileExt"]);
                    docData.PageCount = ShortCheck(reader["PageCount"]);
                    docData.IconUrl = appRootUrl + "/icons/" + NullCheck(reader["IconFileName"]);
                    docData.UploadedBySubuserName = NullCheck(reader["SubuserName"]);
                    docData.UploadDate = DateCheck(reader["UploadDate"], DateTime.MinValue);
                    docData.IsAppendable = BoolCheck(reader["Appendable"]);
                    docData.OriginalFileName = NullCheck(reader["OriginalFileName"]);

                    list.Add(docData);
                }
            }

            return list;
        }

        #endregion

        #region MoveDocument

        /// <summary>
        /// Starts an asynchronous operation for moving a document to a different group.
        /// </summary>
        /// <param name="docID"></param>
        /// <param name="targetGroupID"></param>
        /// <param name="callback"></param>
        /// <param name="stateObject"></param>
        /// <returns></returns>
        public IAsyncResult BeginMoveDocument(long docID, long targetGroupID, AsyncCallback callback, object stateObject)
        {
            SqlCommand command = new SqlCommand();
            command.CommandText = "dsp_GroupDocument_Move";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(GetSqlParameter("@UserID", _userID));
            command.Parameters.Add(GetSqlParameter("@SubuserID", _subuserID));
            command.Parameters.Add(GetSqlParameter("@DocID", docID));
            command.Parameters.Add(GetSqlParameter("@SourceGroupID", _groupID));
            command.Parameters.Add(GetSqlParameter("@TargetGroupID", targetGroupID));

            return _db.BeginExecuteNonQuery(command, callback, stateObject);
        }

        /// <summary>
        /// Ends the asynchronous operation.
        /// </summary>
        /// <param name="result"></param>
        public void EndMoveDocument(IAsyncResult result)
        {
            _db.EndExecuteNonQuery(result);
        }

        #endregion

        #region RemoveDocument

        /// <summary>
        /// Starts an asynchronous operation for removing a document from the group.
        /// </summary>
        /// <param name="docID"></param>
        /// <param name="callback"></param>
        /// <param name="stateObject"></param>
        /// <returns></returns>
        public IAsyncResult BeginRemoveDocument(long docID, AsyncCallback callback, object stateObject)
        {
            SqlCommand command = new SqlCommand();
            command.CommandText = "dsp_GroupDocument_Delete";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(GetSqlParameter("@UserID", _userID));
            command.Parameters.Add(GetSqlParameter("@SubuserID", _subuserID));
            command.Parameters.Add(GetSqlParameter("@GroupID", _groupID));
            command.Parameters.Add(GetSqlParameter("@DocID", docID));
            command.Parameters.Add(GetSqlParameter("@ClientIP", GetClientIP(_context)));

            return _db.BeginExecuteNonQuery(command, callback, stateObject);
        }

        /// <summary>
        /// Ends the asynchronous operation.
        /// </summary>
        /// <param name="result"></param>
        public void EndRemoveDocument(IAsyncResult result)
        {
            _db.EndExecuteNonQuery(result);
        }

        #endregion

        #region CopyGroupDocuments

        /// <summary>
        /// Copy documents from one group to another
        /// </summary>
        /// <param name="oldGroupId"></param>
        /// <param name="newGroupId"></param>
        /// <param name="oldDocTypeId"></param>
        /// <param name="newDocTypeId"></param>
        /// <param name="callback"></param>
        /// <param name="stateObject"></param>
        /// <returns></returns>
        public IAsyncResult BeginCopyGroupDocuments(
            long oldGroupId,
            long newGroupId,
            long oldDocTypeId,
            long newDocTypeId,
            AsyncCallback callback,
            object stateObject)
        {
            SqlCommand command = new SqlCommand();
            command.CommandText = "DSP_CopyGroupDocuments";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(GetSqlParameter("@oldGroupId", oldGroupId));
            command.Parameters.Add(GetSqlParameter("@newGroupId", newGroupId));
            command.Parameters.Add(GetSqlParameter("@oldDocTypeId", oldDocTypeId));
            command.Parameters.Add(GetSqlParameter("@newDocTypeId", newDocTypeId));

            return _db.BeginExecuteNonQuery(command, callback, stateObject);
        }

        /// <summary>
        /// Ends the asynchronous operation.
        /// </summary>
        /// <param name="result"></param>
        public void EndCopyGroupDocuments(IAsyncResult result)
        {
            _db.EndExecuteNonQuery(result);
        }

        #endregion

        #region HandleError

        public void HandleError(IAsyncResult result, string errorMessage)
        {
            _hasErrors = true;
            _errorMessage = errorMessage;
            if (_userCallback != null)
            {
                _userCallback.Invoke(result);
                System.Threading.Thread.CurrentThread.Abort();
            }
        }

        #endregion

        #region IDisposable Support

        private bool disposedValue = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // Free managed resources
                }

                // Free unmanaged resources
                if (_db != null) _db.Dispose();

                disposedValue = true;
            }
        }

        #endregion
    }
}
