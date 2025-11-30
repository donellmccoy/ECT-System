using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SRXLite.DataAccess;
using static SRXLite.Modules.Util;
using static SRXLite.DataAccess.DB;

namespace SRXLite.Classes
{
    public class DocumentPageGuid : IDisposable
    {
        private GuidData _data;
        private AsyncDB _db;
        private string _errorMessage;
        private Guid _guid;
        private bool _hasErrors = false;

        #region Constructors

        public DocumentPageGuid(string encryptedGuid)
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
            public long DocPageID { get; set; }
            public int SubuserID { get; set; }
            public short UserID { get; set; }
        }

        #endregion

        #region GetData

        /// <summary>
        /// Starts an asynchronous operation for creating a new GUID for a page.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="stateObject"></param>
        /// <returns></returns>
        public IAsyncResult BeginGetData(AsyncCallback callback, object stateObject)
        {
            SqlCommand command = new SqlCommand();
            command.CommandText = "dsp_DocumentPageGuid_GetData";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(GetSqlParameter("@DocPageGuid", _guid));
            command.Parameters.Add(GetSqlParameter("@UserID", null, SqlDbType.SmallInt, ParameterDirection.Output));
            command.Parameters.Add(GetSqlParameter("@SubuserID", null, SqlDbType.Int, ParameterDirection.Output));
            command.Parameters.Add(GetSqlParameter("@DocPageID", null, SqlDbType.BigInt, ParameterDirection.Output));

            return _db.BeginExecuteNonQuery(command, callback, stateObject);
        }

        /// <summary>
        /// Ends the asynchronous operation and sets the GuidData object.
        /// </summary>
        /// <param name="result"></param>
        public void EndGetData(IAsyncResult result)
        {
            Dictionary<string, object> outputParams = new Dictionary<string, object>();
            outputParams.Add("@UserID", null);
            outputParams.Add("@SubuserID", null);
            outputParams.Add("@DocPageID", null);

            _db.EndExecuteNonQuery(result, outputParams);

            _data = new GuidData
            {
                UserID = ShortCheck(outputParams["@UserID"]),
                SubuserID = IntCheck(outputParams["@SubuserID"]),
                DocPageID = LngCheck(outputParams["@DocPageID"])
            };
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
