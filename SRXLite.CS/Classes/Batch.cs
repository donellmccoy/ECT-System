using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using SRXLite.DataAccess;
using SRXLite.DataTypes;
using static SRXLite.Modules.Util;
using static SRXLite.DataAccess.DB;

namespace SRXLite.Classes
{
    public class Batch : IDisposable
    {
        private int _batchID;
        private BatchType _batchType;
        private HttpContext _context;
        private AsyncDB _db;
        private Document _doc;
        private long _docID;
        private string _errorMessage;
        private bool _hasErrors = false;
        private string _location;
        private int _subuserID;
        private User _user;
        private short _userID;

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public Batch()
        {
            _context = HttpContext.Current;
        }

        /// <summary>
        /// Constructor with user
        /// </summary>
        /// <param name="user"></param>
        public Batch(User user)
        {
            _user = user;
            _db = new AsyncDB(HandleError);
            _context = HttpContext.Current;
        }

        /// <summary>
        /// Constructor with document ID and user IDs
        /// </summary>
        /// <param name="docID"></param>
        /// <param name="userID"></param>
        /// <param name="subuserID"></param>
        public Batch(long docID, short userID, int subuserID)
        {
            _docID = docID;
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

        #region Create

        /// <summary>
        /// Starts an asynchronous operation for creating a new batch.
        /// </summary>
        /// <param name="batchType"></param>
        /// <param name="location"></param>
        /// <param name="docUploadKeys"></param>
        /// <param name="callback"></param>
        /// <param name="stateObject"></param>
        /// <returns></returns>
        public IAsyncResult BeginCreate(BatchType batchType, string location, UploadKeys docUploadKeys, AsyncCallback callback, object stateObject)
        {
            if (_docID == 0) throw new Exception("DocID is missing");

            // Save to DB, get docID
            SqlCommand command = new SqlCommand();
            command.CommandText = "dsp_Batch_Insert";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(GetSqlParameter("@DocID", _docID));
            command.Parameters.Add(GetSqlParameter("@BatchTypeID", (byte)batchType));
            command.Parameters.Add(GetSqlParameter("@Location", location));
            command.Parameters.Add(GetSqlParameter("@EntityName", docUploadKeys.EntityName));
            command.Parameters.Add(GetSqlParameter("@DocTypeID", docUploadKeys.DocTypeID));
            command.Parameters.Add(GetSqlParameter("@OwnerUserID", _userID));
            command.Parameters.Add(GetSqlParameter("@OwnerSubuserID", _subuserID));
            command.Parameters.Add(GetSqlParameter("@ClientIP", GetClientIP(_context)));
            command.Parameters.Add(GetSqlParameter("@BatchID", null, SqlDbType.BigInt, ParameterDirection.Output));

            // Insert a new document record, output the docID
            return _db.BeginExecuteNonQuery(command, callback, stateObject);
        }

        /// <summary>
        /// Finishes the asynchronous operation for creating a new batch.
        /// </summary>
        /// <param name="result"></param>
        public void EndCreate(IAsyncResult result)
        {
            _db.EndExecuteNonQuery(result, "@BatchID", ref _batchID);
        }

        #endregion

        #region GetUploadUrl

        /// <summary>
        /// Starts an asynchronous operation for generating a URL to upload a batch document.
        /// </summary>
        /// <param name="batchType"></param>
        /// <param name="location"></param>
        /// <param name="entityName"></param>
        /// <param name="docTypeID"></param>
        /// <param name="stylesheetUrl"></param>
        /// <param name="entityDisplayText"></param>
        /// <param name="callback"></param>
        /// <param name="stateObject"></param>
        /// <returns></returns>
        public IAsyncResult BeginGetUploadUrl(
            BatchType batchType,
            string location,
            string entityName,
            int docTypeID,
            string stylesheetUrl,
            string entityDisplayText,
            AsyncCallback callback,
            object stateObject)
        {
            _batchType = batchType;
            _location = location;

            switch (_batchType)
            {
                case DataTypes.BatchType.Entity:
                    docTypeID = (int)DataTypes.BatchDocumentType.BatchImport;
                    break;
                case DataTypes.BatchType.DocType:
                    entityName = "BatchImport";
                    break;
            }

            _doc = new Document(_user);
            return _doc.BeginGetUploadUrl(entityName, docTypeID, 0, stylesheetUrl, entityDisplayText, callback, stateObject);
        }

        /// <summary>
        /// Finishes the asynchronous operation and returns a URL to upload a batch document.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public string EndGetUploadUrl(IAsyncResult result)
        {
            string[] values = { ByteCheck(_batchType).ToString(), _location };
            string id = string.Format("bt={0}&l={1}", values);
            CryptoManager crypto = new CryptoManager(_context);
            return _doc.EndGetUploadUrl(result) + "&bid=" + crypto.EncryptForUrl(id);
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
                if (_doc != null) _doc.Dispose();

                disposedValue = true;
            }
        }

        #endregion
    }
}
