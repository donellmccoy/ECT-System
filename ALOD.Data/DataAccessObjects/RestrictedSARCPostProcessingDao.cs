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
    /// Data access object for <see cref="RestrictedSARCPostProcessing"/> entities.
    /// Provides operations for managing SARC post-processing data including member notification and appeals information.
    /// </summary>
    public class RestrictedSARCPostProcessingDao : ISARCPostProcessingDao
    {
        private SqlDataStore _dataSource;

        /// <summary>
        /// Gets the SQL data store for executing database operations.
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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