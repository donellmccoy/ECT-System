using ALOD.Core.Domain.Lookup;
using ALOD.Core.Domain.Modules.SARC;
using ALOD.Core.Interfaces.DAOInterfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace ALOD.Data
{
    /// <summary>
    /// Data access object for managing <see cref="SARCAppealPostProcessing"/> entities.
    /// Handles post-completion operations for SARC appeal requests.
    /// </summary>
    internal class SARCAppealPostProcessingDao : ISARCAppealPostProcessingDAO
    {
        private SqlDataStore _dataSource;

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
        /// Retrieves the post-processing information for a specific SARC appeal.
        /// </summary>
        /// <param name="id">The appeal ID. Must be greater than 0.</param>
        /// <returns>The SARCAppealPostProcessing entity with the specified ID, or null if not found.</returns>
        public SARCAppealPostProcessing GetById(int id)
        {
            DataSet dSet = DataSource.ExecuteDataSet("core_APSA_sp_GetPostCompletion", id);

            IList<SARCAppealPostProcessing> objects = ProcessDataset(dSet);

            if (objects == null || objects.Count == 0)
                return null;

            return objects[0];
        }

        /// <summary>
        /// Determines whether a SARC appeal has post-processing information.
        /// </summary>
        /// <param name="appealId">The appeal ID to check. Must be greater than 0.</param>
        /// <returns>True if post-processing information exists for the appeal; otherwise, false.</returns>
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
        /// Inserts or updates post-processing information for a SARC appeal.
        /// </summary>
        /// <param name="appealPostProcessing">The SARCAppealPostProcessing entity containing appeal post-completion data. Must not be null.</param>
        /// <returns>True if the operation succeeded; otherwise, false.</returns>
        /// <remarks>
        /// This method updates appeal address, notification date, email, and help extension number.
        /// </remarks>
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