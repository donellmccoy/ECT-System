using ALOD.Core.Domain.Workflow;
using ALOD.Core.Interfaces.DAOInterfaces;
using System;

namespace ALOD.Data
{
    public class WorkflowDao : AbstractNHibernateDao<Workflow, int>, IWorkflowDao
    {
        private SqlDataStore dataSource;

        private SqlDataStore DataSource
        {
            get
            {
                if (dataSource == null)
                {
                    dataSource = new SqlDataStore();
                }
                return dataSource;
            }
        }

        /// <inheritdoc/>
        public bool CanViewWorkflow(int groupId, int workflowId, bool isFormal)
        {
            System.Data.DataSet ds = DataSource.ExecuteDataSet("core_Permission_GetByWorkflow", groupId, workflowId);

            if (ds == null || ds.Tables.Count == 0)
                return false;

            foreach (System.Data.DataRow row in ds.Tables[0].Rows)
            {
                if (isFormal)
                {
                    if (row["permName"].ToString().ToLower().Contains("viewformal"))
                    {
                        return true;
                    }
                }
                else if (row["permName"].ToString().ToLower().Contains("view"))
                {
                    return true;
                }
            }

            return false;
        }

        /// <inheritdoc/>
        public String GetCaseType(int ModuleId, int SubWorkflowTypeId = 0, int MaxLength = 255)
        {
            if (MaxLength < 18)
            {
                MaxLength = 18;
            }
            if (MaxLength > 255)
            {
                MaxLength = 255;
            }

            SqlDataStore store = new SqlDataStore();
            Object result = DataSource.ExecuteScalar("core_workflow_sp_GetWorkflowTitle", ModuleId, SubWorkflowTypeId);

            if (result != null)
            {
                if (result.ToString().Length > MaxLength)
                {
                    return result.ToString().Substring(0, 6) + " ... " + result.ToString().Substring((result.ToString().Length - 4), (result.ToString().Length - 1));
                }

                return result.ToString();
            }

            return String.Empty;
        }

        /// <inheritdoc/>
        public int GetModuleFromWorkflow(int workflowId)
        {
            return Convert.ToInt32(DataSource.ExecuteScalar("core_workflow_sp_GetModuleFromWorkflow", workflowId));
        }

        /// <inheritdoc/>
        public String GetStatusDescription(int WorkFlowId)
        {
            SqlDataStore store = new SqlDataStore();
            Object result = DataSource.ExecuteScalar("core_workflow_sp_GetWorkflowTitleByWorkStatusId", WorkFlowId, 0);

            if (result != null)
            {
                return result.ToString();
            }

            return String.Empty;
        }

        /// <inheritdoc/>
        public int GetWorkflowFromModule(int moduleId)
        {
            return Convert.ToInt32(DataSource.ExecuteScalar("core_workflow_sp_GetWorkflowFromModule", moduleId));
        }
    }
}