using ALOD.Core.Domain.Workflow;
using System.Collections.Generic;

namespace ALOD.Core.Interfaces.DAOInterfaces
{
    public interface IPageAccessDao : IDao<PageAccess, int>
    {
        IList<PageAccess> GetByWorkflowAndStatus(string compo, byte workflowId, int statusId);

        IList<PageAccess> GetByWorkflowGroupAndStatus(byte workflowId, byte groupId, int workstatusId);

        void UpdateList(string compo, byte workflow, int status, IList<PageAccess> list);
    }
}