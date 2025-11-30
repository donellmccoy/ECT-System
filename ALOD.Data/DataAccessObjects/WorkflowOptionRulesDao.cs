using ALOD.Core.Domain.Workflow;
using ALOD.Core.Interfaces.DAOInterfaces;

namespace ALOD.Data
{
    public class WorkflowOptionRulesDao : AbstractNHibernateDao<WorkflowOptionRule, int>, IWorkflowOptionRulesDao
    {
        /// <inheritdoc/>
        public void CopyRules(int dstwso, int srcwso)
        {
            SqlDataStore dataSource = new SqlDataStore();
            dataSource.ExecuteNonQuery("core_workflow_sp_CopyRules", dstwso, srcwso);
        }
    }
}