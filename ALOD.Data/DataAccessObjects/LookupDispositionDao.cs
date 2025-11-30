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
    public class LookupDispositionDao : ILookupDispositionDao
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

        /// <inheritdoc/>
        public IList<Disposition> GetAll()
        {
            DataSet dSet = DataSource.ExecuteDataSet("core_lookups_sp_GetDispositions");

            return GetDispositions(dSet);
        }

        /// <inheritdoc/>
        public Disposition GetById(int id)
        {
            DataSet dSet = DataSource.ExecuteDataSet("core_lookups_sp_GetDispositionById", id);

            IList<Disposition> results = GetDispositions(dSet);

            if (results.Count == 0)
                return null;

            return results[0];
        }

        /// <inheritdoc/>
        public IList<Workflow> GetDispositionWorkflows(int dispositionId)
        {
            List<Workflow> items = new List<Workflow>();

            if (dispositionId <= 0)
                return items;

            DataSet dSet = DataSource.ExecuteDataSet("core_lookups_sp_GetDispositionWorkflows", dispositionId);

            if (dSet == null || dSet.Tables.Count == 0)
                return items;

            foreach (System.Data.DataRow row in dSet.Tables[0].Rows)
            {
                items.Add(WorkflowDao.GetById((byte)row["workflowId"]));
            }

            return items;
        }

        /// <inheritdoc/>
        public IList<Disposition> GetWorkflowDispositions(int workflowId)
        {
            List<Disposition> items = new List<Disposition>();

            if (workflowId <= 0)
                return items;

            DataSet dSet = DataSource.ExecuteDataSet("core_lookups_sp_GetWorkflowDispositions", workflowId);

            return GetDispositions(dSet);
        }

        /// <inheritdoc/>
        public void InsertDisposition(string name)
        {
            if (string.IsNullOrEmpty(name))
                return;

            DataSource.ExecuteNonQuery("core_lookups_sp_InsertDisposition", name);
        }

        /// <inheritdoc/>
        public void UpdateDisposition(int id, string name)
        {
            if (id <= 0)
                return;

            if (string.IsNullOrEmpty(name))
                return;

            DataSource.ExecuteNonQuery("core_lookups_sp_UpdateDisposition", id, name);
        }

        /// <inheritdoc/>
        public void UpdateDispositionWorkflowsMaps(int dispositionId, IList<int> workflowIds)
        {
            using (var con = DataSource.GetSqlConnection())
            {
                con.Open();

                string execCmd = String.Empty;

                if (workflowIds.Count > 0)
                {
                    execCmd = "EXEC core_lookups_sp_UpdateWorkflowDispositionMaps @dispositionId, @workflowIds";
                }
                else
                {
                    execCmd = "EXEC core_lookups_sp_UpdateWorkflowDispositionMaps @dispositionId";
                }

                using (SqlCommand cmd = new SqlCommand(execCmd, con))
                {
                    SqlParameter sqlParam = new SqlParameter("@dispositionId", dispositionId);
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

        private Disposition ExtractDisposition(DataRow row)
        {
            return new Disposition(int.Parse(row["Id"].ToString()), row["Name"].ToString());
        }

        private IList<Disposition> GetDispositions(DataSet dSet)
        {
            if (dSet == null)
                return null;

            if (dSet.Tables.Count == 0)
                return null;

            DataTable dTable = dSet.Tables[0];

            List<Disposition> values = new List<Disposition>();

            foreach (DataRow row in dTable.Rows)
            {
                Disposition value = ExtractDisposition(row);

                if (value != null)
                    values.Add(value);
            }

            return values;
        }
    }
}