using ALOD.Core.Domain.DBSign;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Core.Utils;
using ALOD.Logging;
using System;
using System.Data;

namespace ALOD.Data
{
    /// <summary>
    /// Data access object for managing <see cref="DBSignTemplate"/> entities.
    /// Handles digital signature template retrieval operations.
    /// </summary>
    public class DBSignTemplateDao : IDBSignTemplateDao
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
        /// Retrieves a digital signature template by ID.
        /// </summary>
        /// <param name="id">The template ID.</param>
        /// <returns>The <see cref="DBSignTemplate"/> object, or null if not found or on error.</returns>
        public DBSignTemplate GetById(int id)
        {
            try
            {
                if (id <= 0)
                    return null;

                DataSet dSet = DataSource.ExecuteDataSet("DBSign_sp_GetTemplateById", id);

                return DataHelpers.ExtractObjectFromDataSet<DBSignTemplate>(dSet);
            }
            catch (Exception e)
            {
                LogManager.LogError(e);
                return null;
            }
        }
    }
}