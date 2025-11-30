using ALOD.Core.Domain.Lookup;
using ALOD.Core.Domain.Modules.SARC;
using ALOD.Core.Interfaces.DAOInterfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace ALOD.Data
{
    /// <summary>
    /// Data access object for managing SARC appeal post-processing entities in the Air National Guard (ANG) context.
    /// Handles post-completion operations for SARC (Sexual Assault Response Coordinator) appeal requests.
    /// </summary>
    internal class ANGSARCAppealPostProcessingDao : ISARCAppealPostProcessingDAO
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
        /// Retrieves SARC appeal post-processing information by ID.
        /// </summary>
        /// <param name="id">The SARC appeal ID.</param>
        /// <returns>The <see cref="SARCAppealPostProcessing"/> object, or null if not found.</returns>
        public SARCAppealPostProcessing GetById(int id)
        {
            DataSet dSet = DataSource.ExecuteDataSet("core_APSA_sp_GetPostCompletion", id);

            IList<SARCAppealPostProcessing> objects = ProcessDataset(dSet);

            if (objects == null || objects.Count == 0)
                return null;

            return objects[0];
        }

        /// <summary>
        /// Determines whether post-processing exists for a given SARC appeal.
        /// </summary>
        /// <param name="appealId">The SARC appeal ID to check.</param>
        /// <returns>True if post-processing exists; otherwise, false.</returns>
        public bool hasPostProcess(int appealId)
        {
            SqlDataStore store = new SqlDataStore();
            Object result = store.ExecuteScalar("core_sarc_sp_GetHasAppeal_PostProcess", appealId);

            if (result == null)
                return false;

            int iResult = (int)result;

            if (iResult == 0)
                return false;

            return true;
        }

        /// <summary>
        /// Inserts or updates SARC appeal post-processing information.
        /// </summary>
        /// <param name="appealPostProcessing">The SARC appeal post-processing data to save.</param>
        /// <returns>True if the operation succeeded; otherwise, false.</returns>
        public bool InsertOrUpdate(SARCAppealPostProcessing appealPostProcessing)
        {
            SqlDataStore store = new SqlDataStore();
            Object result = store.ExecuteScalar("core_sarc_sp_UpdateAppealPostCompletion", appealPostProcessing.appealId,
                                                                                            appealPostProcessing.AppealAddress.Street,
                                                                                            appealPostProcessing.AppealAddress.City,
                                                                                            appealPostProcessing.AppealAddress.State,
                                                                                            appealPostProcessing.AppealAddress.Zip,
                                                                                            appealPostProcessing.AppealAddress.Country,
                                                                                            appealPostProcessing.NotificationDate,
                                                                                            appealPostProcessing.email,
                                                                                            appealPostProcessing.HelpExtensionNumber);

            if (result == null)
                return false;

            int iResult = (int)result;

            if (iResult == 0)
                return false;

            return true;
        }

        /// <summary>
        /// Extracts an <see cref="Address"/> object from a data row.
        /// </summary>
        /// <param name="row">The data row containing address information.</param>
        /// <returns>The extracted <see cref="Address"/> object, or null if the row is null.</returns>
        private Address ExtractAddress(DataRow row)
        {
            if (row == null)
                return null;

            Address addr = new Address();

            addr.Street = row["appeal_street"].ToString();
            addr.City = row["appeal_city"].ToString();
            addr.State = row["appeal_state"].ToString();
            addr.Zip = row["appeal_zip"].ToString();
            addr.Country = row["appeal_country"].ToString();

            return addr;
        }

        /// <summary>
        /// Extracts a nullable DateTime from a data row.
        /// </summary>
        /// <param name="row">The data row containing date information.</param>
        /// <returns>The extracted date, or null if not present.</returns>
        private DateTime? ExtractDate(DataRow row)
        {
            DateTime? date = new DateTime();
            date = null;

            if (row == null)
                return null;

            if (!String.IsNullOrEmpty(row["member_notification_date"].ToString()))
                date = (DateTime)row["member_notification_date"];

            return date;
        }

        /// <summary>
        /// Extracts a <see cref="SARCAppealPostProcessing"/> object from a data row.
        /// </summary>
        /// <param name="row">The data row containing SARC appeal post-processing information.</param>
        /// <returns>The extracted object, or null if the row is null.</returns>
        private SARCAppealPostProcessing ExtractObject(DataRow row)
        {
            if (row == null)
                return null;

            return new SARCAppealPostProcessing(int.Parse(row["appeal_id"].ToString())
                                                   , row["helpExtensionNumber"].ToString()
                                                   , ExtractAddress(row)
                                                   , ExtractDate(row)
                                                   , row["email"].ToString());
        }

        /// <summary>
        /// Processes a dataset and extracts a list of <see cref="SARCAppealPostProcessing"/> objects.
        /// </summary>
        /// <param name="dSet">The dataset to process.</param>
        /// <returns>A list of extracted objects, or null if the dataset is invalid.</returns>
        private IList<SARCAppealPostProcessing> ProcessDataset(DataSet dSet)
        {
            if (dSet == null)
                return null;

            if (dSet.Tables.Count == 0)
                return null;

            DataTable dTable = dSet.Tables[0];

            if (dTable == null)
                return null;

            List<SARCAppealPostProcessing> objects = new List<SARCAppealPostProcessing>();

            foreach (DataRow row in dTable.Rows)
            {
                SARCAppealPostProcessing obj = ExtractObject(row);

                if (obj != null)
                    objects.Add(obj);
            }

            return objects;
        }
    }
}