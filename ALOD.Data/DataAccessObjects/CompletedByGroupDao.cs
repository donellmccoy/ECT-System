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
    /// Data access object for managing <see cref="CompletedByGroup"/> entities.
    /// Handles completed-by group operations including retrieval, insertion, updates, and workflow associations.
    /// </summary>
    public class CompletedByGroupDao : ICompletedByGroupDao
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
        /// Retrieves all completed-by groups from the database.
        /// </summary>
        /// <returns>A list of all CompletedByGroup entities.</returns>
        public IList<CompletedByGroup> GetAll()
        {
            DataSet dSet = DataSource.ExecuteDataSet("core_CompletedByGroup_sp_GetAll");

            return GetCompletedByGroups(dSet);
        }

        /// <summary>
        /// Retrieves a specific completed-by group by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the completed-by group. Must be greater than 0.</param>
        /// <returns>The CompletedByGroup entity with the specified ID, or null if not found.</returns>
        public CompletedByGroup GetById(int id)
        {
            DataSet dSet = DataSource.ExecuteDataSet("core_CompletedByGroup_sp_GetById", id);

            IList<CompletedByGroup> results = GetCompletedByGroups(dSet);

            if (results.Count == 0)
                return null;

            return results[0];
        }

        /// <summary>
        /// Retrieves all workflows associated with a specific completed-by group.
        /// </summary>
        /// <param name="cbg">The completed-by group ID. Must be greater than 0.</param>
        /// <returns>A list of Workflow entities associated with the group, or an empty list if none found.</returns>
        public IList<Workflow> GetCompletedByGroupWorkflows(int cbg)
        {
            List<Workflow> items = new List<Workflow>();

            if (cbg <= 0)
                return items;

            DataSet dSet = DataSource.ExecuteDataSet("core_CompletedByGroup_sp_GetCompletedByGroupWorkflows", cbg);

            if (dSet == null || dSet.Tables.Count == 0)
                return items;

            foreach (System.Data.DataRow row in dSet.Tables[0].Rows)
            {
                items.Add(WorkflowDao.GetById((byte)row["workflowId"]));
            }

            return items;
        }

        /// <summary>
        /// Retrieves all completed-by groups associated with a specific workflow.
        /// </summary>
        /// <param name="workflowId">The workflow ID. Must be greater than 0.</param>
        /// <returns>A list of CompletedByGroup entities associated with the workflow, or an empty list if none found.</returns>
        public IList<CompletedByGroup> GetWorkflowCompletedByGroups(int workflowId)
        {
            List<CompletedByGroup> items = new List<CompletedByGroup>();

            if (workflowId <= 0)
                return items;

            DataSet dSet = DataSource.ExecuteDataSet("core_CompletedByGroup_sp_GetWorkflowCompletedByGroups", workflowId);

            return GetCompletedByGroups(dSet);
        }

        /// <summary>
        /// Inserts a new completed-by group into the database.
        /// </summary>
        /// <param name="cbg">The CompletedByGroup entity to insert. Must not be null.</param>
        public void Insert(CompletedByGroup cbg)
        {
            if (cbg == null)
                return;

            DataSource.ExecuteNonQuery("core_CompletedByGroup_sp_Insert", cbg.Name);
        }

        /// <summary>
        /// Updates an existing completed-by group in the database.
        /// </summary>
        /// <param name="cbg">The CompletedByGroup entity to update. Must not be null.</param>
        public void Update(CompletedByGroup cbg)
        {
            if (cbg == null)
                return;

            DataSource.ExecuteNonQuery("core_CompletedByGroup_sp_Update", cbg.Id, cbg.Name);
        }

        /// <summary>
        /// Updates the workflow mappings for a specific completed-by group.
        /// </summary>
        /// <param name="completedByGroupId">The completed-by group ID. Must be greater than 0.</param>
        /// <param name="workflowIds">A list of workflow IDs to associate with the group. Can be empty to clear all mappings.</param>
        /// <remarks>
        /// This method uses a table-valued parameter to efficiently update the many-to-many relationship.
        /// </remarks>
        public void UpdateCompletedByGroupWorkflowMaps(int completedByGroupId, IList<int> workflowIds)
        {
            using (var con = DataSource.GetSqlConnection())
            {
                con.Open();

                string execCmd = String.Empty;

                if (workflowIds.Count > 0)
                {
                    execCmd = "EXEC core_CompletedByGroup_sp_UpdateCompletedByGroupWorkflowMaps @completedByGroupId, @workflowIds";
                }
                else
                {
                    execCmd = "EXEC core_CompletedByGroup_sp_UpdateCompletedByGroupWorkflowMaps @completedByGroupId";
                }

                using (SqlCommand cmd = new SqlCommand(execCmd, con))
                {
                    SqlParameter sqlParam = new SqlParameter("@completedByGroupId", completedByGroupId);
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

        private CompletedByGroup ExtractCompletedByGroup(DataRow row)
        {
            return new CompletedByGroup(int.Parse(row["Id"].ToString()), row["Name"].ToString());
        }

        private IList<CompletedByGroup> GetCompletedByGroups(DataSet dSet)
        {
            if (dSet == null)
                return null;

            if (dSet.Tables.Count == 0)
                return null;

            DataTable dTable = dSet.Tables[0];

            List<CompletedByGroup> values = new List<CompletedByGroup>();

            foreach (DataRow row in dTable.Rows)
            {
                CompletedByGroup value = ExtractCompletedByGroup(row);

                if (value != null)
                    values.Add(value);
            }

            return values;
        }
    }
}