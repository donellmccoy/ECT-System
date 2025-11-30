using System;
using System.Data;
using System.Data.SqlClient;
using SRXLite.DataAccess;
using static SRXLite.Modules.Util;
using static SRXLite.DataAccess.DB;

namespace SRXLite.Classes
{
    public class DocumentGuid : IDisposable
    {
        private GuidData _data;
        private AsyncDB _db;
        private string _errorMessage;
        private Guid _guid;
        private bool _hasErrors = false;

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public DocumentGuid()
        {
            _db = new AsyncDB(HandleError);
        }

        /// <summary>
        /// Constructor with encrypted GUID
        /// </summary>
        /// <param name="encryptedGuid"></param>
        public DocumentGuid(string encryptedGuid)
        {
            CryptoManager crypto = new CryptoManager();
            _guid = new Guid(crypto.Decrypt(encryptedGuid));
            _db = new AsyncDB(HandleError);
        }

        #endregion

        #region Properties

        public GuidData Data
        {
            get { return _data; }
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
        }

        public bool HasErrors
        {
            get { return _hasErrors; }
        }

        #endregion

        #region GuidData

        public struct GuidData
        {
            public Guid DocGuid { get; set; }
            public long DocID { get; set; }
            public int DocTypeID { get; set; }
            public string DocTypeName { get; set; }
            public int EntityID { get; set; }
            public string EntityName { get; set; }
            public long GroupID { get; set; }
            public bool IsReadOnly { get; set; }
            public int SubuserID { get; set; }
            public short UserID { get; set; }
        }

        #endregion

        #region Create

        /// <summary>
        /// Starts an asynchronous operation for creating a new GUID for a document.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="callback"></param>
        /// <param name="stateObject"></param>
        /// <returns></returns>
        public IAsyncResult BeginCreate(GuidData data, AsyncCallback callback, object stateObject)
        {
            SqlCommand command = new SqlCommand();
            command.CommandText = "dsp_DocumentGuid_Create";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(GetSqlParameter("@UserID", data.UserID));
            command.Parameters.Add(GetSqlParameter("@SubuserID", data.SubuserID));
            command.Parameters.Add(GetSqlParameter("@DocID", data.DocID));
            command.Parameters.Add(GetSqlParameter("@EntityName", data.EntityName));
            command.Parameters.Add(GetSqlParameter("@DocTypeID", data.DocTypeID));
            command.Parameters.Add(GetSqlParameter("@GroupID", data.GroupID));
            command.Parameters.Add(GetSqlParameter("@ReadOnly", BoolCheck(data.IsReadOnly)));
            command.Parameters.Add(GetSqlParameter("@DocGuid", null, SqlDbType.UniqueIdentifier, ParameterDirection.Output));

            return _db.BeginExecuteNonQuery(command, callback, stateObject);
        }

        /// <summary>
        /// Ends the asynchronous operation and returns the GUID.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public Guid EndCreate(IAsyncResult result)
        {
            _db.EndExecuteNonQuery(result, "@DocGuid", ref _guid);
            return _guid;
        }

        #endregion

        #region GetData

        /// <summary>
        /// Starts an asynchronous operation for retrieving data referenced by the GUID.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="stateObject"></param>
        /// <returns></returns>
        public IAsyncResult BeginGetData(AsyncCallback callback, object stateObject)
        {
            SqlCommand command = new SqlCommand();
            command.CommandText = "dsp_DocumentGuid_GetData";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(GetSqlParameter("@DocGuid", _guid));

            return _db.BeginExecuteReader(command, callback, stateObject);
        }

        /// <summary>
        /// Ends the asynchronous operation and returns a GuidData object.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public GuidData EndGetData(IAsyncResult result)
        {
            using (SqlDataReader reader = _db.EndExecuteReader(result))
            {
                while (reader.Read())
                {
                    _data = new GuidData
                    {
                        UserID = ShortCheck(reader["UserID"]),
                        SubuserID = IntCheck(reader["SubuserID"]),
                        DocID = LngCheck(reader["DocID"]),
                        EntityID = IntCheck(reader["EntityID"]),
                        EntityName = NullCheck(reader["EntityName"]),
                        DocTypeID = IntCheck(reader["DocTypeID"]),
                        DocTypeName = NullCheck(reader["DocTypeName"]),
                        GroupID = LngCheck(reader["GroupID"]),
                        IsReadOnly = BoolCheck(reader["ReadOnly"], true),
                        DocGuid = _guid
                    };
                }
            }

            return _data;
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
