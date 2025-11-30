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
    public class Entity : IDisposable
    {
        private HttpContext _context;
        private AsyncDB _db;
        private string _entityName;
        private string _errorMessage;
        private bool _hasErrors = false;
        private int _subuserID;
        private User _user;
        private short _userID;

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public Entity()
        {
            _context = HttpContext.Current;
        }

        /// <summary>
        /// Constructor with user
        /// </summary>
        /// <param name="user"></param>
        public Entity(User user)
        {
            _user = user;
            _db = new AsyncDB(HandleError);
            _context = HttpContext.Current;
        }

        /// <summary>
        /// Constructor with user and entity name
        /// </summary>
        /// <param name="user"></param>
        /// <param name="entityName"></param>
        public Entity(User user, string entityName)
        {
            _user = user;
            _userID = user.UserID;
            _subuserID = user.SubuserID;
            _entityName = entityName;
            _db = new AsyncDB(HandleError);
            _context = HttpContext.Current;
        }

        /// <summary>
        /// Constructor with entity name and user IDs
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="userID"></param>
        /// <param name="subuserID"></param>
        public Entity(string entityName, short userID, int subuserID)
        {
            _entityName = entityName;
            _userID = userID;
            _subuserID = subuserID;
            _db = new AsyncDB(HandleError);
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

        #region GetBatchList

        /// <summary>
        /// Starts an asynchronous operation for retrieving a list of all batches for an Entity.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="stateObject"></param>
        /// <returns></returns>
        public IAsyncResult BeginGetBatchList(AsyncCallback callback, object stateObject)
        {
            SqlCommand command = new SqlCommand();
            command.CommandText = "dsp_Entity_GetBatchList";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(GetSqlParameter("@EntityName", _entityName));

            return _db.BeginExecuteReader(command, callback, stateObject);
        }

        /// <summary>
        /// Finishes the asynchronous operation and returns the list.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public List<BatchData> EndGetBatchList(IAsyncResult result)
        {
            List<BatchData> list = new List<BatchData>();
            BatchData data;
            string appRootUrl = GetURL(_context);

            using (SqlDataReader reader = _db.EndExecuteReader(result))
            {
                while (reader.Read())
                {
                    data = new BatchData();
                    data.EntityName = _entityName;
                    data.BatchID = IntCheck(reader["BatchID"]);
                    data.BatchType = (BatchType)reader["BatchTypeID"];
                    data.DateCreated = DateCheck(reader["DateCreated"], DateTime.MinValue);
                    data.DocTypeID = IntCheck(reader["DocTypeID"]);
                    data.DocTypeName = NullCheck(reader["DocTypeName"]);
                    data.DocDescription = NullCheck(reader["DocDescription"]);
                    data.Location = NullCheck(reader["Location"]);
                    data.PageCount = IntCheck(reader["PageCount"]);
                    data.UploadedBySubuserName = NullCheck(reader["SubuserName"]);

                    list.Add(data);
                }
            }

            return list;
        }

        #endregion

        #region GetDocumentList

        /// <summary>
        /// Starts an asynchronous operation for retrieving a list of all documents for an Entity
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="stateObject"></param>
        /// <returns></returns>
        public IAsyncResult BeginGetDocumentList(AsyncCallback callback, object stateObject)
        {
            SqlCommand command = new SqlCommand();
            command.CommandText = "dsp_Entity_GetDocumentList";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(GetSqlParameter("@EntityName", _entityName));

            return _db.BeginExecuteReader(command, callback, stateObject);
        }

        /// <summary>
        /// Finishes the asynchronous operation and returns the list.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public List<DocumentData> EndGetDocumentList(IAsyncResult result)
        {
            List<DocumentData> list = new List<DocumentData>();
            DocumentData data;
            string appRootUrl = GetURL(_context);

            using (SqlDataReader reader = _db.EndExecuteReader(result))
            {
                while (reader.Read())
                {
                    data = new DocumentData();
                    data.EntityName = _entityName;
                    data.DocDate = DateCheck(reader["DocDate"], DateTime.MinValue);
                    data.DocDescription = NullCheck(reader["DocDescription"]);
                    data.DocID = LngCheck(reader["DocID"]);
                    data.DocStatus = (DocumentStatus)reader["DocStatusID"];
                    data.DocTypeID = IntCheck(reader["DocTypeID"]);
                    data.DocTypeName = NullCheck(reader["DocTypeName"]);
                    data.FileExt = NullCheck(reader["FileExt"]);
                    data.PageCount = ShortCheck(reader["PageCount"]);
                    data.IconUrl = appRootUrl + "/icons/" + NullCheck(reader["IconFileName"]);
                    data.UploadedBySubuserName = NullCheck(reader["SubuserName"]);
                    data.UploadDate = DateCheck(reader["UploadDate"], DateTime.MinValue);
                    data.IsAppendable = BoolCheck(reader["Appendable"]);
                    data.OriginalFileName = NullCheck(reader["OriginalFileName"]);

                    list.Add(data);
                }
            }

            return list;
        }

        #endregion

        #region HandleError

        public void HandleError(IAsyncResult result, string errorMessage)
        {
            _hasErrors = true;
            _errorMessage = errorMessage;
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
