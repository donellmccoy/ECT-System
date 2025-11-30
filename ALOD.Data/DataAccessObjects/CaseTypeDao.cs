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
    /// Data access object for managing <see cref="CaseType"/> entities.
    /// Handles case type operations including retrieval, insertion, updates, and workflow associations.
    /// </summary>
    public class CaseTypeDao : ICaseTypeDao
    {
        private SqlDataStore _dataSource;
        private IWorkflowDao _workflowDao;

        /// <summary>
        /// Gets the workflow DAO for retrieving workflow information.
        /// </summary>
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
        /// Retrieves all case types from the database.
        /// </summary>
        /// <returns>A list of all CaseType entities including their sub-case types.</returns>
        public IList<CaseType> GetAll()
        {
            DataSet dSet = DataSource.ExecuteDataSet("core_CaseType_sp_GetAll");

            return GetCaseTypes(dSet);
        }

        /// <summary>
        /// Retrieves all sub-case types from the database.
        /// </summary>
        /// <returns>A list of all sub-CaseType entities.</returns>
        public IList<CaseType> GetAllSubCaseTypes()
        {
            DataSet dSet = DataSource.ExecuteDataSet("core_CaseType_sp_GetAllSubCaseTypes");

            return GetSubCaseTypes(dSet);
        }

        /// <summary>
        /// Retrieves a specific case type by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the case type. Must be greater than 0.</param>
        /// <returns>The CaseType entity with the specified ID including its sub-case types, or null if not found.</returns>
        public CaseType GetById(int id)
        {
            DataSet dSet = DataSource.ExecuteDataSet("core_CaseType_sp_GetById", id);

            IList<CaseType> results = GetCaseTypes(dSet);

            if (results.Count == 0)
                return null;

            return results[0];
        }

        /// <summary>
        /// Retrieves all workflows associated with a specific case type.
        /// </summary>
        /// <param name="caseTypeId">The case type ID. Must be greater than 0.</param>
        /// <returns>A list of Workflow entities associated with the case type, or an empty list if none found.</returns>
        public IList<Workflow> GetCaseTypeWorkflows(int caseTypeId)
        {
            List<Workflow> items = new List<Workflow>();

            if (caseTypeId <= 0)
                return items;

            DataSet dSet = DataSource.ExecuteDataSet("core_CaseType_sp_GetCaseTypeWorkflows", caseTypeId);

            if (dSet == null || dSet.Tables.Count == 0)
                return items;

            foreach (System.Data.DataRow row in dSet.Tables[0].Rows)
            {
                items.Add(WorkflowDao.GetById((byte)row["workflowId"]));
            }

            return items;
        }

        /// <summary>
        /// Retrieves all case types associated with a specific workflow.
        /// </summary>
        /// <param name="workflowId">The workflow ID. Must be greater than 0.</param>
        /// <returns>A list of CaseType entities associated with the workflow, or an empty list if none found.</returns>
        public IList<CaseType> GetWorkflowCaseTypes(int workflowId)
        {
            List<CaseType> items = new List<CaseType>();

            if (workflowId <= 0)
                return items;

            DataSet dSet = DataSource.ExecuteDataSet("core_CaseType_sp_GetWorkflowCaseTypes", workflowId);

            return GetCaseTypes(dSet);
        }

        /// <summary>
        /// Inserts a new case type into the database.
        /// </summary>
        /// <param name="ct">The CaseType entity to insert. Must not be null.</param>
        public void Insert(CaseType ct)
        {
            if (ct == null)
                return;

            DataSource.ExecuteNonQuery("core_CaseType_sp_Insert", ct.Name);
        }

        /// <summary>
        /// Inserts a new sub-case type into the database.
        /// </summary>
        /// <param name="sct">The sub-CaseType entity to insert. Must not be null.</param>
        public void InsertSubCaseType(CaseType sct)
        {
            if (sct == null)
                return;

            DataSource.ExecuteNonQuery("core_CaseType_sp_InsertSubCaseType", sct.Name);
        }

        /// <summary>
        /// Updates an existing case type in the database.
        /// </summary>
        /// <param name="ct">The CaseType entity to update. Must not be null.</param>
        public void Update(CaseType ct)
        {
            if (ct == null)
                return;

            DataSource.ExecuteNonQuery("core_CaseType_sp_Update", ct.Id, ct.Name);
        }

        /// <inheritdoc/>
        public void UpdateCaseTypeSubCaseTypeMaps(int caseTypeId, IList<int> subCaseTypeIds)
        {
            using (var con = DataSource.GetSqlConnection())
            {
                con.Open();

                string execCmd = String.Empty;

                if (subCaseTypeIds.Count > 0)
                {
                    execCmd = "EXEC core_CaseType_sp_UpdateCaseTypeSubCaseTypeMaps @caseTypeId, @subCaseTypeIds";
                }
                else
                {
                    execCmd = "EXEC core_CaseType_sp_UpdateCaseTypeSubCaseTypeMaps @caseTypeId";
                }

                using (SqlCommand cmd = new SqlCommand(execCmd, con))
                {
                    SqlParameter sqlParam = new SqlParameter("@caseTypeId", caseTypeId);
                    sqlParam.DbType = DbType.Int32;
                    cmd.Parameters.Add(sqlParam);

                    if (subCaseTypeIds.Count > 0)
                    {
                        sqlParam = new SqlParameter("@subCaseTypeIds", SqlDbType.Structured);
                        sqlParam.TypeName = "dbo.tblIntegerList";
                        sqlParam.Value = CollectionHelpers.IntListToListOfSQLDataRecords(subCaseTypeIds);
                        cmd.Parameters.Add(sqlParam);
                    }

                    cmd.ExecuteNonQuery();
                }

                con.Close();
            }
        }

        /// <summary>
        /// Updates the workflow mappings for a specific case type.
        /// </summary>
        /// <param name="caseTypeId">The case type ID. Must be greater than 0.</param>
        /// <param name="workflowIds">A list of workflow IDs to associate with the case type. Can be empty to clear all mappings.</param>
        /// <remarks>
        /// This method uses a table-valued parameter to efficiently update the many-to-many relationship.
        /// </remarks>
        public void UpdateCaseTypeWorkflowMaps(int caseTypeId, IList<int> workflowIds)
        {
            using (var con = DataSource.GetSqlConnection())
            {
                con.Open();

                string execCmd = String.Empty;

                if (workflowIds.Count > 0)
                {
                    execCmd = "EXEC core_CaseType_sp_UpdateCaseTypeWorkflowMaps @caseTypeId, @workflowIds";
                }
                else
                {
                    execCmd = "EXEC core_CaseType_sp_UpdateCaseTypeWorkflowMaps @caseTypeId";
                }

                using (SqlCommand cmd = new SqlCommand(execCmd, con))
                {
                    SqlParameter sqlParam = new SqlParameter("@caseTypeId", caseTypeId);
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

        /// <summary>
        /// Updates an existing sub-case type in the database.
        /// </summary>
        /// <param name="sct">The sub-CaseType entity to update. Must not be null.</param>
        public void UpdateSubCaseType(CaseType sct)
        {
            if (sct == null)
                return;

            DataSource.ExecuteNonQuery("core_CaseType_sp_UpdateSubCaseType", sct.Id, sct.Name);
        }

        private CaseType ExtractCaseType(DataRow row)
        {
            return new CaseType(int.Parse(row["Id"].ToString()), row["Name"].ToString());
        }

        private IList<CaseType> GetAssociatedSubCaseTypes(int caseTypeId)
        {
            DataSet dSet = DataSource.ExecuteDataSet("core_CaseType_sp_GetSubCaseTypes", caseTypeId);

            List<CaseType> subCaseTypes = new List<CaseType>();

            if (dSet == null)
                return subCaseTypes;

            if (dSet.Tables.Count == 0)
                return subCaseTypes;

            DataTable dTable = dSet.Tables[0];

            foreach (DataRow row in dTable.Rows)
            {
                CaseType sct = ExtractCaseType(row);

                if (sct != null)
                    subCaseTypes.Add(sct);
            }

            return subCaseTypes;
        }

        private IList<CaseType> GetCaseTypes(DataSet dSet)
        {
            if (dSet == null)
                return null;

            if (dSet.Tables.Count == 0)
                return null;

            DataTable dTable = dSet.Tables[0];

            List<CaseType> caseTypes = new List<CaseType>();

            foreach (DataRow row in dTable.Rows)
            {
                CaseType ct = ExtractCaseType(row);

                if (ct != null)
                {
                    ct.SubCaseTypes = GetAssociatedSubCaseTypes(ct.Id);
                    caseTypes.Add(ct);
                }
            }

            return caseTypes;
        }

        private IList<CaseType> GetSubCaseTypes(DataSet dSet)
        {
            if (dSet == null)
                return null;

            if (dSet.Tables.Count == 0)
                return null;

            DataTable dTable = dSet.Tables[0];

            List<CaseType> caseTypes = new List<CaseType>();

            foreach (DataRow row in dTable.Rows)
            {
                CaseType ct = ExtractCaseType(row);

                if (ct != null)
                {
                    caseTypes.Add(ct);
                }
            }

            return caseTypes;
        }
    }
}