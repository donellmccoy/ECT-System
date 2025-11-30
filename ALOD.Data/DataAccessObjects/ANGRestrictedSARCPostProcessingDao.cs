using ALOD.Core.Domain.Modules.SARC;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Core.Utils;
using ALOD.Logging;
using System;
using System.Collections.Generic;
using System.Data;

namespace ALOD.Data
{
    /// <summary>
    /// Data access object for managing SARC post-processing entities in the Air National Guard context.
    /// Handles post-completion operations for restricted SARC cases.
    /// </summary>
    public class ANGRestrictedSARCPostProcessingDao : ISARCPostProcessingDao
    {
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

        /// <summary>
        /// Retrieves restricted SARC post-processing information by ID.
        /// </summary>
        /// <param name="id">The SARC case ID.</param>
        /// <returns>The <see cref="RestrictedSARCPostProcessing"/> object, or null if not found or on error.</returns>
        public RestrictedSARCPostProcessing GetById(int id)
        {
            try
            {
                if (id <= 0)
                    return null;

                DataSet dSet = DataSource.ExecuteDataSet("core_sarc_sp_GetPostCompletion", id);

                IList<RestrictedSARCPostProcessing> objects = DataHelpers.ExtractObjectsFromDataSet<RestrictedSARCPostProcessing>(dSet);

                if (objects == null || objects.Count == 0)
                    return null;

                return objects[0];
            }
            catch (Exception e)
            {
                LogManager.LogError(e);
                return null;
            }
        }

        /// <summary>
        /// Inserts or updates restricted SARC post-processing information.
        /// </summary>
        /// <param name="sarcPostProcessing">The SARC post-processing data to save.</param>
        /// <returns>True if the operation succeeded; otherwise, false.</returns>
        public bool InsertOrUpdate(RestrictedSARCPostProcessing sarcPostProcessing)
        {
            try
            {
                Object result = DataSource.ExecuteScalar("core_sarc_sp_UpdatePostCompletion", sarcPostProcessing.Id, sarcPostProcessing.MemberNotified, sarcPostProcessing.MemberNotificationDate, sarcPostProcessing.HelpExtensionNumber, sarcPostProcessing.AppealAddress.Street, sarcPostProcessing.AppealAddress.City, sarcPostProcessing.AppealAddress.State, sarcPostProcessing.AppealAddress.Zip, sarcPostProcessing.AppealAddress.Country, sarcPostProcessing.email);

                if (result == null)
                    return false;

                int iResult = (int)result;

                if (iResult == 0)
                    return false;

                return true;
            }
            catch (Exception e)
            {
                LogManager.LogError(e);
                return false;
            }
        }
    }
}