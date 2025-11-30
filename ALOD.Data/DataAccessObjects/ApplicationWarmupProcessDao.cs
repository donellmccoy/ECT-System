using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Core.Utils;
using NHibernate;
using System;
using System.Data;

namespace ALOD.Data
{
    /// <summary>
    /// Data access object for managing application warmup process logging and tracking.
    /// Handles process execution status, log management, and activity monitoring.
    /// </summary>
    public class ApplicationWarmupProcessDao : IApplicationWarmupProcessDao
    {
        private SqlDataStore dataSource;

        /// <summary>
        /// Gets the SQL data store instance for database operations.
        /// </summary>
        private SqlDataStore DataSource
        {
            get
            {
                if (dataSource == null)
                {
                    dataSource = new SqlDataStore();
                }
                return dataSource;
            }
        }

        /// <summary>
        /// Gets the current NHibernate session.
        /// </summary>
        private ISession Session
        {
            get { return NHibernateSessionManager.Instance.GetSession(); }
        }

        /// <summary>
        /// Deletes an application warmup log entry by ID.
        /// </summary>
        /// <param name="logId">The log ID to delete.</param>
        public void DeleteLogById(int logId)
        {
            DataSource.ExecuteNonQuery("ApplicationWarmupProcess_sp_DeleteLogById", logId);
        }

        /// <summary>
        /// Finds the last execution date for a specific warmup process.
        /// </summary>
        /// <param name="processName">The name of the process.</param>
        /// <returns>The last execution date, or null if not found.</returns>
        public DateTime? FindProcessLastExecutionDate(string processName)
        {
            DateTime? result = null;

            DataSet dSet = DataSource.ExecuteDataSet("ApplicationWarmupProcess_sp_FindProcessLastExecutionDate", processName);

            if (dSet == null || dSet.Tables.Count == 0)
                return result;

            foreach (DataRow row in dSet.Tables[0].Rows)
            {
                result = DataHelpers.GetNullableDateTimeFromDataRow("ExecutionDate", row);
            }

            return result;
        }

        /// <summary>
        /// Retrieves all application warmup process logs.
        /// </summary>
        /// <returns>A DataSet containing all log entries.</returns>
        public DataSet GetAllLogs()
        {
            return DataSource.ExecuteDataSet("ApplicationWarmupProcess_sp_GetAllLogs");
        }

        /// <summary>
        /// Determines whether a warmup process is currently active.
        /// </summary>
        /// <param name="processName">The name of the process to check.</param>
        /// <returns>True if the process is active; otherwise, false.</returns>
        public bool IsProcessActive(string processName)
        {
            Object result = DataSource.ExecuteScalar("ApplicationWarmupProcess_sp_IsProcessActive", processName);

            if (result == null)
                return false;

            int iResult = (int)result;

            if (iResult == 0)
                return false;

            return true;
        }
    }
}