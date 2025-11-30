using ALOD.Core.Domain.Reports;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Logging;
using System;
using System.Data;

namespace ALOD.Data
{
    /// <summary>
    /// Data access object for managing ARCNet (Army Reserve Component Network) data.
    /// Handles IAA (Inactive Duty for Active Duty) training data retrieval and import tracking.
    /// </summary>
    public class ARCNetDao : IARCNetDao
    {
        #region SQL DataSource Property

        private SqlDataStore _dataSource;

        /// <summary>
        /// Gets the SQL data store instance for database operations.
        /// </summary>
        private SqlDataStore DataSource
        {
            get
            {
                if (_dataSource == null)
                {
                    _dataSource = new SqlDataStore();
                }
                return _dataSource;
            }
        }

        #endregion SQL DataSource Property

        /// <summary>
        /// Retrieves the last execution date of the ARCNet import process.
        /// </summary>
        /// <returns>The last execution date, or null if not found or on error.</returns>
        public DateTime? GetARCNetImportLastExecutionDate()
        {
            try
            {
                Object result = DataSource.ExecuteScalar("arcnet_GetLastExecutionDate");

                if (result == null)
                    return null;

                return (DateTime)result;
            }
            catch (Exception e)
            {
                LogManager.LogError(e);
                return null;
            }
        }

        /// <summary>
        /// Retrieves IAA training data for users based on search criteria.
        /// </summary>
        /// <param name="args">The search arguments containing EDIPIN, name, and date range filters.</param>
        /// <returns>A DataSet containing training data, or an empty DataSet on error.</returns>
        public DataSet GetIAATrainingDataForUsers(ARCNetLookupReportArgs args)
        {
            DataSet dSet = new DataSet();

            try
            {
                dSet = DataSource.ExecuteDataSet("arcnet_GetIAATrainingDataForUsers", args.EDIPIN, args.LastName, args.FirstName, args.MiddleNames, args.BeginDate, args.EndDate);

                return dSet;
            }
            catch (Exception e)
            {
                LogManager.LogError(e);
                return dSet;
            }
        }
    }
}