using ALOD.Core.Domain.Lookup;
using ALOD.Core.Domain.Workflow;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Core.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace ALOD.Data
{
    /// <summary>
    /// Data access object for managing <see cref="CertificationStamp"/> entities.
    /// Handles certification stamp operations including retrieval, insertion, updates, workflow associations, and stamp data parameter extraction.
    /// </summary>
    public class CertificationStampDao : ICertificationStampDao
    {
        private SqlDataStore _dataSource;
        private IWorkflowDao _workflowDao;

        public IWorkflowDao WorkflowDao
        {
            get
            {
                if (_workflowDao == null)
                {
                    _workflowDao = new NHibernateDaoFactory().GetWorkflowDao();
                }

                return _workflowDao;
            }
        }

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
        /// Retrieves all certification stamps from the database.
        /// </summary>
        /// <returns>A list of all CertificationStamp entities.</returns>
        public IList<CertificationStamp> GetAll()
        {
            DataSet dSet = DataSource.ExecuteDataSet("core_CertificationStamp_sp_GetAll");

            return GetCertificationStamps(dSet);
        }

        /// <summary>
        /// Retrieves a specific certification stamp by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the certification stamp. Must be greater than 0.</param>
        /// <returns>The CertificationStamp entity with the specified ID, or null if not found.</returns>
        public CertificationStamp GetById(int id)
        {
            DataSet dSet = DataSource.ExecuteDataSet("core_CertificationStamp_sp_GetById", id);

            IList<CertificationStamp> results = GetCertificationStamps(dSet);

            if (results == null || results.Count == 0)
                return null;

            return results[0];
        }

        /// <summary>
        /// Retrieves all workflows associated with a specific certification stamp.
        /// </summary>
        /// <param name="stampId">The certification stamp ID. Must be greater than 0.</param>
        /// <returns>A list of Workflow entities associated with the stamp, or an empty list if none found.</returns>
        public IList<Workflow> GetCertificationStampWorkflows(int stampId)
        {
            List<Workflow> items = new List<Workflow>();

            if (stampId <= 0)
                return items;

            DataSet dSet = DataSource.ExecuteDataSet("core_CertificationStamp_sp_GetCerticationStampWorkflows", stampId);

            if (dSet == null || dSet.Tables.Count == 0)
                return items;

            foreach (System.Data.DataRow row in dSet.Tables[0].Rows)
            {
                items.Add(WorkflowDao.GetById((byte)row["workflowId"]));
            }

            return items;
        }

        /// <summary>
        /// Retrieves the certification stamp for a special case.
        /// </summary>
        /// <param name="refId">The reference ID of the special case. Must be greater than 0.</param>
        /// <param name="selectSecondary">If true, retrieves the secondary stamp; otherwise, retrieves the primary stamp.</param>
        /// <returns>The CertificationStamp entity for the special case, or null if not found.</returns>
        public CertificationStamp GetSpecialCaseStamp(int refId, bool selectSecondary)
        {
            DataSet dSet = DataSource.ExecuteDataSet("core_CertificationStamp_sp_GetSpecialCaseStamp", refId, selectSecondary);

            IList<CertificationStamp> results = GetCertificationStamps(dSet);

            if (results == null || results.Count == 0)
                return null;

            return results[0];
        }

        /// <summary>
        /// Retrieves parameter field data for a certification stamp.
        /// </summary>
        /// <param name="refId">The reference ID. Must be greater than 0.</param>
        /// <param name="selectSecondary">If true, retrieves secondary stamp data; otherwise, retrieves primary stamp data.</param>
        /// <returns>A dictionary containing parameter names as keys and their corresponding values.</returns>
        /// <remarks>
        /// This method extracts stamp data parameters that can be used for mail merge or template substitution.
        /// </remarks>
        public IDictionary<string, string> GetStampData(int refId, bool selectSecondary)
        {
            IDictionary<string, string> values = new Dictionary<string, string>();

            SqlDataStore.RowReader del = (SqlDataStore source, IDataReader reader) =>
            {
                values.Add(source.GetString(reader, 0), source.GetString(reader, 1));
            };

            DataSource.ExecuteReader(del, "core_CertificationStamp_sp_GetParameterFieldData", refId, selectSecondary);

            return values;
        }

        /// <summary>
        /// Retrieves all certification stamps associated with a specific workflow.
        /// </summary>
        /// <param name="workflowId">The workflow ID. Must be greater than 0.</param>
        /// <returns>A list of CertificationStamp entities associated with the workflow, or an empty list if none found.</returns>
        public IList<CertificationStamp> GetWorkflowCertificationStamps(int workflowId)
        {
            List<CertificationStamp> items = new List<CertificationStamp>();

            if (workflowId <= 0)
                return items;

            DataSet dSet = DataSource.ExecuteDataSet("core_CertificationStamp_sp_GetWorkflowCertificationStamps", workflowId);

            return GetCertificationStamps(dSet);
        }

        /// <summary>
        /// Retrieves certification stamps for a workflow filtered by qualification disposition.
        /// </summary>
        /// <param name="workflowId">The workflow ID. Must be greater than 0.</param>
        /// <param name="isQualified">If true, retrieves qualified stamps; otherwise, retrieves non-qualified stamps.</param>
        /// <returns>A list of CertificationStamp entities matching the criteria, or an empty list if none found.</returns>
        public IList<CertificationStamp> GetWorkflowCertificationStampsByDisposition(int workflowId, bool isQualified)
        {
            List<CertificationStamp> items = new List<CertificationStamp>();

            if (workflowId <= 0)
                return items;

            DataSet dSet = DataSource.ExecuteDataSet("core_CertificationStamp_sp_GetWorkflowCertificationStamps", workflowId, isQualified);

            return GetCertificationStamps(dSet);
        }

        /// <summary>
        /// Inserts a new certification stamp into the database.
        /// </summary>
        /// <param name="cs">The CertificationStamp entity to insert. Must not be null.</param>
        public void Insert(CertificationStamp cs)
        {
            if (cs == null)
                return;

            DataSource.ExecuteNonQuery("core_CertificationStamp_sp_Insert", cs.Name, cs.Body, cs.IsQualified);
        }

        /// <summary>
        /// Updates an existing certification stamp in the database.
        /// </summary>
        /// <param name="cs">The CertificationStamp entity to update. Must not be null.</param>
        public void Update(CertificationStamp cs)
        {
            if (cs == null)
                return;

            DataSource.ExecuteNonQuery("core_CertificationStamp_sp_Update", cs.Id, cs.Name, cs.Body, cs.IsQualified);
        }

        /// <summary>
        /// Updates the workflow mappings for a specific certification stamp.
        /// </summary>
        /// <param name="stampId">The certification stamp ID. Must be greater than 0.</param>
        /// <param name="workflowIds">A list of workflow IDs to associate with the stamp. Can be empty to clear all mappings.</param>
        /// <remarks>
        /// This method uses a table-valued parameter to efficiently update the many-to-many relationship.
        /// </remarks>
        public void UpdateCertificationStampWorkflowsMaps(int stampId, IList<int> workflowIds)
        {
            using (var con = DataSource.GetSqlConnection())
            {
                con.Open();

                string execCmd = String.Empty;

                if (workflowIds.Count > 0)
                {
                    execCmd = "EXEC core_CertificationStamp_sp_UpdateCertificationStampWorkflowMaps @stampId, @workflowIds";
                }
                else
                {
                    execCmd = "EXEC core_CertificationStamp_sp_UpdateCertificationStampWorkflowMaps @stampId";
                }

                using (SqlCommand cmd = new SqlCommand(execCmd, con))
                {
                    SqlParameter sqlParam = new SqlParameter("@stampId", stampId);
                    sqlParam.DbType = DbType.Int32;
                    cmd.Parameters.Add(sqlParam);

                    if (workflowIds.Count > 0)
                    {
                        sqlParam = new SqlParameter("@workflowIds", SqlDbType.Structured);
                        sqlParam.TypeName = "dbo.tblIntegerList";
                        sqlParam.Value = CollectionHelpers.IntListToListOfSQLDataRecords(workflowIds);
                        cmd.Parameters.Add(sqlParam);
                    }

                    cmd.ExecuteNonQuery();
                }

                con.Close();
            }
        }

        private CertificationStamp ExtractCertificationStamp(DataRow row)
        {
            return new CertificationStamp(int.Parse(row["Id"].ToString()), row["Name"].ToString(), row["Body"].ToString(), bool.Parse(row["IsQualified"].ToString()));
        }

        private IList<CertificationStamp> GetCertificationStamps(DataSet dSet)
        {
            if (dSet == null)
                return null;

            if (dSet.Tables.Count == 0)
                return null;

            DataTable dTable = dSet.Tables[0];

            List<CertificationStamp> values = new List<CertificationStamp>();

            foreach (DataRow row in dTable.Rows)
            {
                CertificationStamp value = ExtractCertificationStamp(row);

                if (value != null)
                    values.Add(value);
            }

            return values;
        }
    }
}