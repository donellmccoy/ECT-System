using ALOD.Core.Domain.Lookup;
using ALOD.Core.Domain.Modules.Lod;
using ALOD.Core.Interfaces.DAOInterfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace ALOD.Data
{
    /// <summary>
    /// Data access object for managing <see cref="LineOfDutyPostProcessing"/> entities in the Air National Guard context.
    /// Handles post-completion operations for LOD cases.
    /// </summary>
    public class ANGLineOfDutyPostProcessingDao : ILineOfDutyPostProcessingDao
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
        /// Retrieves LOD post-processing information by ID.
        /// </summary>
        /// <param name="id">The LOD ID.</param>
        /// <returns>The <see cref="LineOfDutyPostProcessing"/> object, or null if not found.</returns>
        public LineOfDutyPostProcessing GetById(int id)
        {
            DataSet dSet = DataSource.ExecuteDataSet("core_lod_sp_GetLODPostCompletion", id);

            IList<LineOfDutyPostProcessing> objects = ProcessDataset(dSet);

            if (objects == null || objects.Count == 0)
                return null;

            return objects[0];
        }

        /// <summary>
        /// Inserts or updates LOD post-processing information.
        /// </summary>
        /// <param name="lodPostProcessing">The LOD post-processing data to save.</param>
        /// <returns>True if the operation succeeded; otherwise, false.</returns>
        public bool InsertOrUpdate(LineOfDutyPostProcessing lodPostProcessing)
        {
            Object result = DataSource.ExecuteScalar("core_lod_sp_UpdateLODPostCompletion", lodPostProcessing.Id,
                                                                                            lodPostProcessing.HelpExtensionNumber,
                                                                                            lodPostProcessing.AppealAddress.Street,
                                                                                            lodPostProcessing.AppealAddress.City,
                                                                                            lodPostProcessing.AppealAddress.State,
                                                                                            lodPostProcessing.AppealAddress.Zip,
                                                                                            lodPostProcessing.AppealAddress.Country,
                                                                                            lodPostProcessing.NextOfKinFirstName,
                                                                                            lodPostProcessing.NextOfKinLastName,
                                                                                            lodPostProcessing.NextOfKinMiddleName,
                                                                                            lodPostProcessing.NotificationDate,
                                                                                            lodPostProcessing.email,
                                                                                            lodPostProcessing.chkAddress,
                                                                                            lodPostProcessing.chkEmail,
                                                                                            lodPostProcessing.chkPhone);

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

            addr.Street = row["appealStreet"].ToString();
            addr.City = row["appealCity"].ToString();
            addr.State = row["appealState"].ToString();
            addr.Zip = row["appealZip"].ToString();
            addr.Country = row["appealCountry"].ToString();

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

            if (!String.IsNullOrEmpty(row["notification_date"].ToString()))
                date = (DateTime)row["notification_date"];

            return date;
        }

        /// <summary>
        /// Extracts a <see cref="LineOfDutyPostProcessing"/> object from a data row.
        /// </summary>
        /// <param name="row">The data row containing LOD post-processing information.</param>
        /// <returns>The extracted object, or null if the row is null.</returns>
        private LineOfDutyPostProcessing ExtractObject(DataRow row)
        {
            if (row == null)
                return null;

            return new LineOfDutyPostProcessing(int.Parse(row["lodId"].ToString()), row["helpExtensionNumber"].ToString(), ExtractAddress(row), row["nokFirstName"].ToString(), row["nokLastName"].ToString(), row["nokMiddleName"].ToString(), ExtractDate(row), row["email"].ToString(), int.Parse(row["address_flag"].ToString()), int.Parse(row["phone_flag"].ToString()), int.Parse(row["email_flag"].ToString()));
        }

        /// <summary>
        /// Processes a dataset and extracts a list of <see cref="LineOfDutyPostProcessing"/> objects.
        /// </summary>
        /// <param name="dSet">The dataset to process.</param>
        /// <returns>A list of extracted objects, or null if the dataset is invalid.</returns>
        private IList<LineOfDutyPostProcessing> ProcessDataset(DataSet dSet)
        {
            if (dSet == null)
                return null;

            if (dSet.Tables.Count == 0)
                return null;

            DataTable dTable = dSet.Tables[0];

            if (dTable == null)
                return null;

            List<LineOfDutyPostProcessing> objects = new List<LineOfDutyPostProcessing>();

            foreach (DataRow row in dTable.Rows)
            {
                LineOfDutyPostProcessing obj = ExtractObject(row);

                if (obj != null)
                    objects.Add(obj);
            }

            return objects;
        }
    }
}