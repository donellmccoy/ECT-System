using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Web;

namespace ALOD.Data
{
    public sealed class SqlDataStore
    {
        private Database dataBase;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <remarks>uses the default database connection</remarks>
        public SqlDataStore()
        {
            InitDataBase(string.Empty);
        }

        public delegate void RowReader(SqlDataStore sender, IDataReader reader);

        /// <summary>
        /// Add an input parameter to a command object
        /// </summary>
        /// <param name="cmd">The DBCommand object to add the command to</param>
        /// <param name="name">The name of the parameter</param>
        /// <param name="type">The type of the parameter</param>
        /// <param name="value">The value of the parameter</param>
        /// <remarks></remarks>
        public void AddInParameter(DbCommand cmd, string name, DbType type, object value)
        {
            dataBase.AddInParameter(cmd, name, type, value);
        }

        /// <summary>
        /// Add an output parameter to a command object
        /// </summary>
        /// <param name="cmd">The DBCommand object to add the command to</param>
        /// <param name="name">The name of the parameter</param>
        /// <param name="type">The type of the parameter</param>
        /// <param name="size">The maximum size of the data within the column</param>
        /// <remarks></remarks>
        public void AddOutParameter(DbCommand cmd, string name, DbType type, int size)
        {
            dataBase.AddOutParameter(cmd, name, type, size);
        }

        /// <summary>
        /// Get a dataset of values from the data source.
        /// </summary>
        /// <param name="procName">The Database command to process.</param>
        /// <returns>The data resultset desired.</returns>
        public DataSet ExecuteDataSet(string procName)
        {
            return ExecuteDataSet(GetStoredProcCommand(procName));
        }

        /// <summary>
        /// Get a dataset of values from the data source.
        /// </summary>
        /// <param name="procName">The Database command to process.</param>
        /// <param name="parameterValues">The data resultset desired.</param>
        /// <returns>The data resultset desired.</returns>
        public DataSet ExecuteDataSet(string procName, params object[] parameterValues)
        {
            return ExecuteDataSet(GetStoredProcCommand(procName, parameterValues));
        }

        /// <summary>
        /// Get a dataset of values from the data source.
        /// </summary>
        /// <param name="cmd">The Database command to process.</param>
        /// <returns>The data resultset desired.</returns>
        public DataSet ExecuteDataSet(DbCommand cmd)
        {
            DataSet ds = null;

            DateTime start = DateTime.Now;
            ds = dataBase.ExecuteDataSet(cmd);
            CheckQueryTime(start, cmd);

            return ds;
        }

        public DataSet ExecuteDataSet(DbCommand cmd, int timeout)
        {
            DataSet ds = null;
            DateTime start = DateTime.Now;
            cmd.CommandTimeout = timeout;
            ds = dataBase.ExecuteDataSet(cmd);
            CheckQueryTime(start, cmd);

            return ds;
        }

        /// <summary>
        /// Sends a command to be processed on the data source.
        /// </summary>
        /// <param name="procName">The stored procedure name to process.</param>
        /// <returns>The total rows affected by the process command.</returns>
        public int ExecuteNonQuery(string procName)
        {
            return ExecuteNonQuery(GetStoredProcCommand(procName));
        }

        /// <summary>
        /// Sends a command to be processed on the data source.
        /// </summary>
        /// <param name="procName">The stored procedure name to process.</param>
        /// <param name="parameterValues">The applicable stored procedure parameters.</param>
        /// <returns>The total rows affected by the process command.</returns>
        public int ExecuteNonQuery(string procName, params object[] parameterValues)
        {
            return ExecuteNonQuery(GetStoredProcCommand(procName, parameterValues));
        }

        /// <summary>
        /// Sends a command to be processed on the data source.
        /// </summary>
        /// <param name="cmd">The command to execute</param>
        /// <returns>The total rows affected by the process command.</returns>
        public int ExecuteNonQuery(DbCommand cmd)
        {
            DateTime start = DateTime.Now;
            int rows = dataBase.ExecuteNonQuery(cmd);
            CheckQueryTime(start, cmd);
            return rows;
        }

        public int ExecuteNonQueryUnWrapped(string procName, params object[] parameterValues)
        {
            return ExecuteNonQueryUnWrapped(GetStoredProcCommand(procName, parameterValues));
        }

        public int ExecuteNonQueryUnWrapped(DbCommand cmd)
        {
            int rows = dataBase.ExecuteNonQuery(cmd);
            return rows;
        }

        /// <summary>
        /// Returns or assigns results to a IDataReader.
        /// </summary>
        /// <param name="rowReader">A delegate used to assign the data results.</param>
        /// <param name="procName">The stored procedure name to process.</param>
        public void ExecuteReader(RowReader rowReader, string procName)
        {
            ExecuteReader(rowReader, GetStoredProcCommand(procName));
        }

        /// <summary>
        /// Returns or assigns results to a IDataReader.
        /// </summary>
        /// <param name="rowReader">A delegate used to assign the data results.</param>
        /// <param name="procName">The stored procedure name to process.</param>
        /// <param name="parameterValues">The applicable stored procedure parameters.</param>
        public void ExecuteReader(RowReader rowReader, string procName, params object[] parameterValues)
        {
            ExecuteReader(rowReader, GetStoredProcCommand(procName, parameterValues));
        }

        /// <summary>
        /// Returns or assigns results to a IDataReader.
        /// </summary>
        /// <param name="rowReader">A delegate used to assign the data results.</param>
        /// <param name="cmd">The Database command to process.</param>
        public void ExecuteReader(RowReader rowReader, DbCommand cmd)
        {
            DateTime start = DateTime.Now;
            using (IDataReader reader = dataBase.ExecuteReader(cmd))
            {
                while (reader.Read())
                {
                    rowReader.Invoke(this, reader);
                }
            }

            CheckQueryTime(start, cmd);
        }

        /// <summary>
        /// Processes a command to return a single value.ctl00$ContentMain$UnitTextBox
        /// </summary>
        /// <param name="procName">The stored procedure name to process.</param>
        /// <returns>The desired single value.</returns>
        public object ExecuteScalar(string procName)
        {
            return ExecuteScalar(GetStoredProcCommand(procName));
        }

        /// <summary>
        /// Sends a command to be processed on the data source.
        /// </summary>
        /// <param name="procName">The stored procedure name to process.</param>
        /// <param name="parameterValues">The applicable stored procedure parameters.</param>
        /// <returns>A single value</returns>
        public object ExecuteScalar(string procName, params object[] parameterValues)
        {
            return ExecuteScalar(GetStoredProcCommand(procName, parameterValues));
        }

        /// <summary>
        /// Processes a command to return a single value.
        /// </summary>
        /// <param name="cmd">The DB Command to process</param>
        /// <returns>The desired single value.</returns>
        public object ExecuteScalar(DbCommand cmd)
        {
            object scalar = null;

            DateTime start = DateTime.Now;
            scalar = dataBase.ExecuteScalar(cmd);
            CheckQueryTime(start, cmd);

            return scalar;
        }

        /// <summary>
        /// Returns a value as string
        /// </summary>
        /// <param name="reader">The datareader</param>
        /// <param name="index">The column index to read from</param>
        /// <param name="defaultValue">The default value</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public string GetAsString(IDataReader reader, int index, string defaultValue)
        {
            if ((!reader.IsDBNull(index)))
            {
                return reader[index].ToString();
            }
            return defaultValue;
        }

        /// <summary>
        /// Retrieves a Boolean value from the datareader
        /// </summary>
        /// <param name="reader">The datareader</param>
        /// <param name="index">The column index to read from</param>
        /// <param name="defaultValue">The default value to return of the column is DBNull</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool GetBoolean(IDataReader reader, int index, bool defaultValue)
        {
            if ((!reader.IsDBNull(index)))
            {
                return reader.GetBoolean(index);
            }
            return defaultValue;
        }

        /// <summary>
        /// Retrieves a Boolean value from the datareader
        /// </summary>
        /// <param name="reader">The datareader</param>
        /// <param name="index">The column index to read from</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public bool GetBoolean(IDataReader reader, int index)
        {
            return GetBoolean(reader, index, false);
        }

        /// <summary>
        /// Retrieves a Byte value from the datareader
        /// </summary>
        /// <param name="reader">The datareader</param>
        /// <param name="index">The column index to read from</param>
        /// <param name="defaultValue">The default value to return of the column is DBNull</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public byte GetByte(IDataReader reader, int index, byte defaultValue)
        {
            if ((!reader.IsDBNull(index)))
            {
                return reader.GetByte(index);
            }
            return defaultValue;
        }

        /// <summary>
        /// Retrieves a Byte value from the datareader
        /// </summary>
        /// <param name="reader">The datareader</param>
        /// <param name="index">The column index to read from</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public byte GetByte(IDataReader reader, int index)
        {
            return GetByte(reader, index, 0);
        }

        /// <summary>
        /// Retrieves a Char value from the datareader
        /// </summary>
        /// <param name="reader">The datareader</param>
        /// <param name="index">The column index to read from</param>
        /// <param name="defaultValue">The default value to return of the column is DBNull</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public char GetChar(IDataReader reader, int index, char defaultValue)
        {
            if ((!reader.IsDBNull(index)))
            {
                return reader.GetChar(index);
            }
            return defaultValue;
        }

        /// <summary>
        /// Retrieves a Char value from the datareader
        /// </summary>
        /// <param name="reader">The datareader</param>
        /// <param name="index">The column index to read from</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public char GetChar(IDataReader reader, int index)
        {
            return GetChar(reader, index, new Char());
        }

        /// <summary>
        /// Retrieves a DateTime value from the datareader
        /// </summary>
        /// <param name="reader">The datareader</param>
        /// <param name="index">The column index to read from</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public DateTime GetDateTime(IDataReader reader, int index)
        {
            if ((!reader.IsDBNull(index)))
            {
                return reader.GetDateTime(index);
            }
            return new System.DateTime();
        }

        /// <summary>
        /// Retrieves a DateTime value from the datareader
        /// </summary>
        /// <param name="reader">The datareader</param>
        /// <param name="index">The column index to read from</param>
        /// <param name="defaultValue">The default value to return of the column is DBNull</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public DateTime GetDateTime(IDataReader reader, int index, DateTime defaultValue)
        {
            if ((!reader.IsDBNull(index)))
            {
                return reader.GetDateTime(index);
            }
            return defaultValue;
        }

        /// <summary>
        /// Retrieves a Decimal value from the datareader
        /// </summary>
        /// <param name="reader">The datareader</param>
        /// <param name="index">The column index to read from</param>
        /// <param name="defaultValue">The default value to return of the column is DBNull</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public decimal GetDecimal(IDataReader reader, int index, decimal defaultValue)
        {
            if ((!reader.IsDBNull(index)))
            {
                return reader.GetDecimal(index);
            }
            return defaultValue;
        }

        /// <summary>
        /// Retrieves a Decimal value from the datareader
        /// </summary>
        /// <param name="reader">The datareader</param>
        /// <param name="index">The column index to read from</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public decimal GetDecimal(IDataReader reader, int index)
        {
            return GetDecimal(reader, index, 0);
        }

        /// <summary>
        /// Retrieves a Double value from the datareader
        /// </summary>
        /// <param name="reader">The datareader</param>
        /// <param name="index">The column index to read from</param>
        /// <param name="defaultValue">The default value to return of the column is DBNull</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public double GetDouble(IDataReader reader, int index, double defaultValue)
        {
            if ((!reader.IsDBNull(index)))
            {
                return reader.GetDouble(index);
            }
            return defaultValue;
        }

        /// <summary>
        /// Retrieves a Double value from the datareader
        /// </summary>
        /// <param name="reader">The datareader</param>
        /// <param name="index">The column index to read from</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public double GetDouble(IDataReader reader, int index)
        {
            return GetDouble(reader, index, 0);
        }

        /// <summary>
        /// Retrieves an Int16 value from the datareader
        /// </summary>
        /// <param name="reader">The datareader</param>
        /// <param name="index">The column index to read from</param>
        /// <param name="defaultValue">The default value to return of the column is DBNull</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public Int16 GetInt16(IDataReader reader, int index, Int16 defaultValue)
        {
            if ((!reader.IsDBNull(index)))
            {
                return reader.GetInt16(index);
            }
            return defaultValue;
        }

        /// <summary>
        /// Retrieves an Int16 value from the datareader
        /// </summary>
        /// <param name="reader">The datareader</param>
        /// <param name="index">The column index to read from</param>
        /// <param name="defaultValue">The default value to return of the column is DBNull</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public Int16 GetInt16(IDataReader reader, int index)
        {
            return GetInt16(reader, index, 0);
        }

        /// <summary>
        /// Retrieves an Int32 value from the datareader
        /// </summary>
        /// <param name="reader">The datareader</param>
        /// <param name="index">The column index to read from</param>
        /// <param name="defaultValue">The default value to return of the column is DBNull</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public Int32 GetInt32(IDataReader reader, int index, Int32 defaultValue)
        {
            if ((!reader.IsDBNull(index)))
            {
                return reader.GetInt32(index);
            }
            return defaultValue;
        }

        /// <summary>
        /// Retrieves an Int32 value from the datareader
        /// </summary>
        /// <param name="reader">The datareader</param>
        /// <param name="index">The column index to read from</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public Int32 GetInt32(IDataReader reader, int index)
        {
            return GetInt32(reader, index, 0);
        }

        /// <summary>
        /// Retrieves an Int64 value from the datareader
        /// </summary>
        /// <param name="reader">The datareader</param>
        /// <param name="index">The column index to read from</param>
        /// <param name="defaultValue">The default value to return of the column is DBNull</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public Int64 GetInt64(IDataReader reader, int index, Int64 defaultValue)
        {
            if ((!reader.IsDBNull(index)))
            {
                return reader.GetInt64(index);
            }
            return defaultValue;
        }

        /// <summary>
        /// Retrieves an Int64 value from the datareader
        /// </summary>
        /// <param name="reader">The datareader</param>
        /// <param name="index">The column index to read from</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public Int64 GetInt64(IDataReader reader, int index)
        {
            return GetInt64(reader, index, 0);
        }

        /// <summary>
        /// Retrieves an Integer value from the datareader
        /// </summary>
        /// <param name="reader">The datareader</param>
        /// <param name="index">The column index to read from</param>
        /// <param name="defaultValue">The default value to return of the column is DBNull</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public int GetInteger(IDataReader reader, int index, int defaultValue)
        {
            return GetInt32(reader, index, defaultValue);
        }

        /// <summary>
        /// Retrieves an Integer value from the datareader
        /// </summary>
        /// <param name="reader">The datareader</param>
        /// <param name="index">The column index to read from</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public int GetInteger(IDataReader reader, int index)
        {
            return GetInt32(reader, index, 0);
        }

        public int GetInteger(IDataReader reader, string columnName, int defaultValue)
        {
            object ob = reader[columnName];

            if (ob == DBNull.Value)
            {
                return defaultValue;
            }

            return (int)ob;
        }

        public int GetInteger(IDataReader reader, string columnName)
        {
            return GetInteger(reader, columnName, 0);
        }

        public Nullable<DateTime> GetNullableDate(IDataReader reader, int index, object defaultValue)
        {
            if ((!reader.IsDBNull(index)))
            {
                return reader.GetDateTime(index);
            }
            return (DateTime)defaultValue;
        }

        /// <summary>
        /// Returns a number from the reader
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="index"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public int GetNumber(IDataReader reader, int index, int defaultValue)
        {
            Type type = reader[index].GetType();

            if ((object.ReferenceEquals(type, typeof(Int32))))
            {
                return GetInt32(reader, index, defaultValue);
            }
            else if (object.ReferenceEquals(type, typeof(Int16)))
            {
                return GetInt16(reader, index, (Int16)defaultValue);
            }
            else if (object.ReferenceEquals(type, typeof(byte)))
            {
                return GetByte(reader, index, (byte)defaultValue);
            }

            return defaultValue;
        }

        public int GetNumber(IDataReader reader, int index)
        {
            int defaultValue = 0;
            Type type = reader[index].GetType();

            if ((object.ReferenceEquals(type, typeof(Int32))))
            {
                return GetInt32(reader, index, defaultValue);
            }
            else if (object.ReferenceEquals(type, typeof(Int16)))
            {
                return GetInt16(reader, index, (Int16)defaultValue);
            }
            else if (object.ReferenceEquals(type, typeof(byte)))
            {
                return GetByte(reader, index, (byte)defaultValue);
            }

            return defaultValue;
        }

        /// <summary>
        /// Creates and return a SqlConnection to the database.
        /// </summary>
        /// <returns></returns>
        public SqlConnection GetSqlConnection()
        {
            return (SqlConnection)dataBase.CreateConnection();
        }

        /// <summary>
        /// Gets the Sql string database command from the data source.
        /// </summary>
        /// <param name="query">The inline Sql query.</param>
        /// <returns>The database command.</returns>
        public DbCommand GetSqlStringCommand(string query)
        {
            return dataBase.GetSqlStringCommand(query);
        }

        /// <summary>
        /// Gets the Sql stored procedure command from the data source.
        /// </summary>
        /// <param name="procName">The stored procedure name.</param>
        /// <returns>The data base command.</returns>
        public DbCommand GetStoredProcCommand(string procName)
        {
            return dataBase.GetStoredProcCommand(procName);
        }

        /// <summary>
        /// Gets the Sql stored procedure command from the data source.
        /// </summary>
        /// <param name="procName">The stored procedure name.</param>
        /// <param name="parameterValues">The applicable stored procedure parameters.</param>
        /// <returns></returns>
        public DbCommand GetStoredProcCommand(string procName, params object[] parameterValues)
        {
            return dataBase.GetStoredProcCommand(procName, parameterValues);
        }

        /// <summary>
        /// Retrieves a String value from the datareader
        /// </summary>
        /// <param name="reader">The datareader</param>
        /// <param name="index">The column index to read from</param>
        /// <param name="defaultValue">The default value to return of the column is DBNull</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public string GetString(IDataReader reader, int index, string defaultValue)
        {
            if ((!reader.IsDBNull(index)))
            {
                return reader.GetString(index);
            }
            return defaultValue;
        }

        /// <summary>
        /// Retrieves a String value from the datareader
        /// </summary>
        /// <param name="reader">The DataReader</param>
        /// <param name="index">The column index</param>
        /// <returns></returns>
        public string GetString(IDataReader reader, int index)
        {
            return GetString(reader, index, "");
        }

        public string GetString(IDataReader reader, string columnName, string defaultValue)
        {
            object ob = reader[columnName];

            if (ob == DBNull.Value)
            {
                return defaultValue;
            }

            return (string)ob;
        }

        public string GetString(IDataReader reader, string columnName)
        {
            return GetString(reader, columnName, "");
        }

        public void InitDataBase(string dbName)
        {
            if ((dbName.Length == 0))
            {
                dataBase = DatabaseFactory.CreateDatabase();
            }
            else
            {
                dataBase = DatabaseFactory.CreateDatabase(dbName);
            }
        }

        private void CheckQueryTime(System.DateTime start, DbCommand cmd)
        {
            int duration = (int)DateTime.Now.Subtract(start).TotalSeconds;

            if (duration > int.Parse(System.Configuration.ConfigurationManager.AppSettings["QueryTimeLimit"]))
            {
                try
                {
                    string message = duration.ToString() + " seconds to execute query: " + cmd.CommandText;

                    string userName = "Unknown";

                    if (((HttpContext.Current.Session["UserName"] != null)))
                    {
                        userName = (string)HttpContext.Current.Session["UserName"];
                    }
                    else
                    {
                    }

                    InitDataBase("");

                    dataBase.ExecuteNonQuery("core_log_sp_RecordError",
                        userName,
                        HttpContext.Current.Request.RawUrl,
                        System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                        HttpContext.Current.Request.Browser.Browser.ToString() + " " + HttpContext.Current.Request.Browser.Version + "," + HttpContext.Current.Request.Browser.Platform,
                        HttpContext.Current.Server.HtmlEncode(message),
                        "", // stack trace empty
                        "", // caller empty
                        ""); // address empty
                }
                catch
                {
                    //if we threw an exception logging a database error we probably
                    //want to quit while we're ahead and not keep trying to log it
                    //so we just fail quietly here   
                }
            }
        }
    }
}