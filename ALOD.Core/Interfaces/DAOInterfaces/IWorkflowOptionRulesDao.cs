using ALOD.Core.Domain.Workflow;

namespace ALOD.Core.Interfaces.DAOInterfaces
{
    public interface IWorkflowOptionRulesDao : IDao<WorkflowOptionRule, int>
    {
        void CopyRules(int dstwso, int srcwso);
    }
}