using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using static SRXLite.Modules.Util;

namespace SRXLite.DataAccess
{
    /// <summary>
    /// Asynchronous database operations
    /// </summary>
    public class AsyncDB : IDisposable
    {
        private SqlCommand _command;
        private string _connectionString;
        private OnErrorDelegate _onError;

        public delegate void OnErrorDelegate(IAsyncResult result, string errorMessage);

        #region Constructors

        /// <summary>
        /// Constructor with error handler delegate
        /// </summary>
        /// <param name="onError"></param>
        public AsyncDB(OnErrorDelegate onError)
        {
            _onError = onError;
            _connectionString = ConfigurationManager.ConnectionStrings["SRXLite"].ToString();
        }

        #endregion

        #region Properties

        public OnErrorDelegate OnError
        {
            get { return _onError; }
            set { _onError = value; }
        }

        #endregion

        #region ExecuteNonQuery

        /// <summary>
        /// Begin asynchronous execution of a non-query command
        /// </summary>
        /// <param name="command"></param>
        /// <param name="callback"></param>
        /// <param name="stateObject"></param>
        /// <returns></returns>
        public IAsyncResult BeginExecuteNonQuery(SqlCommand command, AsyncCallback callback, object stateObject)
        {
            try
            {
                if (_command != null)
                {
                    _command.Connection.Dispose();
                }

                _command = command;
                _command.Connection = new SqlConnection(_connectionString);
                _command.Connection.Open();

                return _command.BeginExecuteNonQuery(callback, stateObject); // Keep connection open
            }
            catch
            {
                _command.Connection.Dispose();
                throw;
            }
        }

        /// <summary>
        /// End asynchronous execution of a non-query command
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public int EndExecuteNonQuery(IAsyncResult result)
        {
            try
            {
                using (_command.Connection)
                {
                    return _command.EndExecuteNonQuery(result);
                }
            }
            catch (Exception ex)
            {
                if (_onError != null) _onError.Invoke(result, ex.ToString());
                return -1;
            }
        }

        /// <summary>
        /// End asynchronous execution with Guid output parameter
        /// </summary>
        /// <param name="result"></param>
        /// <param name="outputParamName"></param>
        /// <param name="outputParamValue"></param>
        /// <returns></returns>
        public int EndExecuteNonQuery(IAsyncResult result, string outputParamName, ref Guid outputParamValue)
        {
            object value = null;
            int rowCount = EndExecuteNonQuery(result, outputParamName, ref value);
            outputParamValue = new Guid(value.ToString());
            return rowCount;
        }

        /// <summary>
        /// End asynchronous execution with int output parameter
        /// </summary>
        /// <param name="result"></param>
        /// <param name="outputParamName"></param>
        /// <param name="outputParamValue"></param>
        /// <returns></returns>
        public int EndExecuteNonQuery(IAsyncResult result, string outputParamName, ref int outputParamValue)
        {
            object value = null;
            int rowCount = EndExecuteNonQuery(result, outputParamName, ref value);
            outputParamValue = IntCheck(value.ToString());
            return rowCount;
        }

        /// <summary>
        /// End asynchronous execution with long output parameter
        /// </summary>
        /// <param name="result"></param>
        /// <param name="outputParamName"></param>
        /// <param name="outputParamValue"></param>
        /// <returns></returns>
        public int EndExecuteNonQuery(IAsyncResult result, string outputParamName, ref long outputParamValue)
        {
            object value = null;
            int rowCount = EndExecuteNonQuery(result, outputParamName, ref value);
            outputParamValue = LngCheck(value);
            return rowCount;
        }

        /// <summary>
        /// End asynchronous execution with string output parameter
        /// </summary>
        /// <param name="result"></param>
        /// <param name="outputParamName"></param>
        /// <param name="outputParamValue"></param>
        /// <returns></returns>
        public int EndExecuteNonQuery(IAsyncResult result, string outputParamName, ref string outputParamValue)
        {
            object value = null;
            int rowCount = EndExecuteNonQuery(result, outputParamName, ref value);
            outputParamValue = NullCheck(value);
            return rowCount;
        }

        /// <summary>
        /// End asynchronous execution with object output parameter
        /// </summary>
        /// <param name="result"></param>
        /// <param name="outputParamName"></param>
        /// <param name="outputParamValue"></param>
        /// <returns></returns>
        public int EndExecuteNonQuery(IAsyncResult result, string outputParamName, ref object outputParamValue)
        {
            try
            {
                using (_command.Connection)
                {
                    int rowCount = _command.EndExecuteNonQuery(result);
                    outputParamValue = _command.Parameters[outputParamName].Value;

                    return rowCount;
                }
            }
            catch (Exception ex)
            {
                if (_onError != null) _onError.Invoke(result, ex.ToString());
                return -1;
            }
        }

        /// <summary>
        /// End asynchronous execution with multiple output parameters
        /// </summary>
        /// <param name="result"></param>
        /// <param name="outputParams"></param>
        /// <returns></returns>
        public int EndExecuteNonQuery(IAsyncResult result, Dictionary<string, object> outputParams)
        {
            try
            {
                using (_command.Connection)
                {
                    int rowCount = _command.EndExecuteNonQuery(result);

                    if (outputParams.Count > 0)
                    {
                        string[] keys = new string[outputParams.Count];
                        outputParams.Keys.CopyTo(keys, 0);
                        foreach (string key in keys)
                        {
                            outputParams[key] = _command.Parameters[key].Value;
                        }
                    }

                    return rowCount;
                }
            }
            catch (Exception ex)
            {
                if (_onError != null) _onError.Invoke(result, ex.ToString());
                return -1;
            }
        }

        #endregion

        #region ExecuteReader

        /// <summary>
        /// Begin asynchronous execution of a reader command
        /// </summary>
        /// <param name="command"></param>
        /// <param name="callback"></param>
        /// <param name="stateObject"></param>
        /// <returns></returns>
        public IAsyncResult BeginExecuteReader(SqlCommand command, AsyncCallback callback, object stateObject)
        {
            try
            {
                if (_command != null)
                {
                    _command.Connection.Dispose();
                }

                _command = command;
                _command.Connection = new SqlConnection(_connectionString);
                _command.Connection.Open();
                return _command.BeginExecuteReader(callback, stateObject, CommandBehavior.CloseConnection); // Connection is closed when SqlDataReader is closed
            }
            catch
            {
                _command.Connection.Dispose();
                throw;
            }
        }

        /// <summary>
        /// End asynchronous execution of a reader command
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public SqlDataReader EndExecuteReader(IAsyncResult result)
        {
            try
            {
                return _command.EndExecuteReader(result); // Calling code must close reader
            }
            catch (Exception ex)
            {
                _command.Connection.Dispose();
                if (_onError != null) _onError.Invoke(result, ex.ToString());
                return null;
            }
        }

        #endregion

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        /// <summary>
        /// Dispose pattern implementation
        /// </summary>
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

                // Free shared/unmanaged resources
                if (_command != null)
                {
                    _command.Connection.Dispose();
                    _command.Dispose();
                }

                disposedValue = true;
            }
        }

        #endregion
    }
}
