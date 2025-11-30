using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace SRXLite.DataAccess
{
    /// <summary>
    /// Database access layer for SRXLite
    /// </summary>
    public static class DB
    {
        #region GetConnectionString

        /// <summary>
        /// Gets the connection string for SRXLite database
        /// </summary>
        /// <returns></returns>
        private static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["SRXLite"].ToString();
        }

        #endregion

        #region GetSqlParameter

        public static SqlParameter GetSqlParameter(string name, bool value, ParameterDirection direction = ParameterDirection.Input)
        {
            return GetSqlParameter(name, value, SqlDbType.Bit, direction);
        }

        public static SqlParameter GetSqlParameter(string name, byte value, ParameterDirection direction = ParameterDirection.Input)
        {
            return GetSqlParameter(name, value, SqlDbType.TinyInt, direction);
        }

        public static SqlParameter GetSqlParameter(string name, byte? value, ParameterDirection direction = ParameterDirection.Input)
        {
            return GetSqlParameter(name, value, SqlDbType.TinyInt, direction);
        }

        public static SqlParameter GetSqlParameter(string name, byte[] value, ParameterDirection direction = ParameterDirection.Input)
        {
            return GetSqlParameter(name, value, SqlDbType.VarBinary, direction);
        }

        public static SqlParameter GetSqlParameter(string name, DateTime value, ParameterDirection direction = ParameterDirection.Input)
        {
            return GetSqlParameter(name, value, SqlDbType.DateTime, direction);
        }

        public static SqlParameter GetSqlParameter(string name, long value, ParameterDirection direction = ParameterDirection.Input)
        {
            return GetSqlParameter(name, value, SqlDbType.BigInt, direction);
        }

        public static SqlParameter GetSqlParameter(string name, long? value, ParameterDirection direction = ParameterDirection.Input)
        {
            return GetSqlParameter(name, value, SqlDbType.BigInt, direction);
        }

        public static SqlParameter GetSqlParameter(string name, short value, ParameterDirection direction = ParameterDirection.Input)
        {
            return GetSqlParameter(name, value, SqlDbType.SmallInt, direction);
        }

        public static SqlParameter GetSqlParameter(string name, short? value, ParameterDirection direction = ParameterDirection.Input)
        {
            return GetSqlParameter(name, value, SqlDbType.SmallInt, direction);
        }

        public static SqlParameter GetSqlParameter(string name, int value, ParameterDirection direction = ParameterDirection.Input)
        {
            return GetSqlParameter(name, value, SqlDbType.Int, direction);
        }

        public static SqlParameter GetSqlParameter(string name, int? value, ParameterDirection direction = ParameterDirection.Input)
        {
            return GetSqlParameter(name, value, SqlDbType.Int, direction);
        }

        public static SqlParameter GetSqlParameter(string name, string value, ParameterDirection direction = ParameterDirection.Input)
        {
            return GetSqlParameter(name, value, SqlDbType.VarChar, direction);
        }

        public static SqlParameter GetSqlParameter(string name, Guid value, ParameterDirection direction = ParameterDirection.Input)
        {
            return GetSqlParameter(name, value, SqlDbType.UniqueIdentifier, direction);
        }

        public static SqlParameter GetSqlParameter(string name, object value, SqlDbType dbType, ParameterDirection direction = ParameterDirection.Input)
        {
            SqlParameter parameter = new SqlParameter(name, dbType);
            parameter.Direction = direction;
            parameter.Value = value ?? DBNull.Value;
            return parameter;
        }

        public static SqlParameter GetSqlParameter(string name, object value, SqlDbType dbType, int size, ParameterDirection direction = ParameterDirection.Input)
        {
            SqlParameter parameter = new SqlParameter(name, dbType, size);
            parameter.Direction = direction;
            parameter.Value = value ?? DBNull.Value;
            return parameter;
        }

        #endregion

        #region ExecuteDataset

        /// <summary>
        /// Executes SQL and returns a DataSet
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static DataSet ExecuteDataset(string sql)
        {
            return ExecuteDataset(new SqlCommand(sql));
        }

        /// <summary>
        /// Executes SQL command and returns a DataSet
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static DataSet ExecuteDataset(SqlCommand command)
        {
            SqlDataAdapter da = new SqlDataAdapter(command);
            DataSet ds = new DataSet();
            try
            {
                command.Connection = new SqlConnection(GetConnectionString());
                da.Fill(ds);
                return ds;
            }
            finally
            {
                ds.Dispose();
                da.Dispose();
                command.Connection.Dispose();
                command.Dispose();
            }
        }

        #endregion

        #region ExecuteNonQuery

        /// <summary>
        /// Executes a SQL command
        /// </summary>
        /// <param name="sql"></param>
        public static void ExecuteNonQuery(string sql)
        {
            ExecuteNonQuery(new SqlCommand(sql));
        }

        /// <summary>
        /// Executes a SQL command
        /// </summary>
        /// <param name="command"></param>
        public static void ExecuteNonQuery(SqlCommand command)
        {
            using (command)
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    command.Connection = connection;
                    command.Connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Executes a SQL command and returns an output parameter value
        /// </summary>
        /// <param name="command"></param>
        /// <param name="outputParamName"></param>
        /// <param name="outputParamValue"></param>
        public static void ExecuteNonQuery(SqlCommand command, string outputParamName, ref object outputParamValue)
        {
            using (command)
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    command.Connection = connection;
                    command.Connection.Open();
                    command.ExecuteNonQuery();
                    outputParamValue = command.Parameters[outputParamName].Value;
                }
            }
        }

        /// <summary>
        /// Executes a SQL command and returns an output parameter value as string
        /// </summary>
        /// <param name="command"></param>
        /// <param name="outputParamName"></param>
        /// <param name="outputParamValue"></param>
        public static void ExecuteNonQuery(SqlCommand command, string outputParamName, ref string outputParamValue)
        {
            object value = null;
            ExecuteNonQuery(command, outputParamName, ref value);
            outputParamValue = SRXLite.Modules.Util.NullCheck(value);
        }

        /// <summary>
        /// Executes a SQL command and returns multiple output parameters
        /// </summary>
        /// <param name="command"></param>
        /// <param name="outputParams"></param>
        public static void ExecuteNonQuery(SqlCommand command, ref Dictionary<string, object> outputParams)
        {
            using (command)
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    command.Connection = connection;
                    command.Connection.Open();
                    command.ExecuteNonQuery();

                    if (outputParams.Count > 0)
                    {
                        string[] keys = new string[outputParams.Count];
                        outputParams.Keys.CopyTo(keys, 0);
                        foreach (string key in keys)
                        {
                            outputParams[key] = command.Parameters[key].Value;
                        }
                    }
                }
            }
        }

        #endregion

        #region ExecuteReader

        /// <summary>
        /// Executes a SQL command and returns a SqlDataReader
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static SqlDataReader ExecuteReader(SqlCommand command)
        {
            try
            {
                command.Connection = new SqlConnection(GetConnectionString());
                command.Connection.Open();
                return command.ExecuteReader(CommandBehavior.CloseConnection); // Connection is closed when SqlDataReader is closed
            }
            catch (Exception ex)
            {
                command.Connection.Dispose();
                command.Dispose();
                throw ex;
            }
        }

        #endregion

        #region ExecuteScalar

        /// <summary>
        /// Executes SQL and returns the first column of the first row
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static object ExecuteScalar(string sql)
        {
            return ExecuteScalar(new SqlCommand(sql));
        }

        /// <summary>
        /// Executes SQL command and returns the first column of the first row
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static object ExecuteScalar(SqlCommand command)
        {
            SqlConnection connection = new SqlConnection(GetConnectionString());
            try
            {
                command.Connection = connection;
                connection.Open();
                return command.ExecuteScalar();
            }
            finally
            {
                connection.Dispose();
                command.Dispose();
            }
        }

        #endregion
    }
}
