using ALOD.Core.Domain.Workflow;
using System;

namespace ALOD.Core.Interfaces.DAOInterfaces
{
    public interface IWorkflowDao : IDao<Workflow, int>
    {
        bool CanViewWorkflow(int groupId, int workflowId, bool isFormal);

        String GetCaseType(int ModuleId, int SubWorkflowTypeId = 0, int MaxLength = 255);

        int GetModuleFromWorkflow(int workflowId);

        String GetStatusDescription(int WorkFlowId);

        int GetWorkflowFromModule(int moduleId);
    }
}