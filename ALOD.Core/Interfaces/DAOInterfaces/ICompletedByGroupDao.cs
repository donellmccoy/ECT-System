using ALOD.Core.Domain.Lookup;
using ALOD.Core.Domain.Workflow;
using System.Collections.Generic;

namespace ALOD.Core.Interfaces.DAOInterfaces
{
    public interface ICompletedByGroupDao
    {
        IList<CompletedByGroup> GetAll();

        CompletedByGroup GetById(int id);

        IList<Workflow> GetCompletedByGroupWorkflows(int completedByGroupId);

        IList<CompletedByGroup> GetWorkflowCompletedByGroups(int workflowId);

        void Insert(CompletedByGroup cbg);

        void Update(CompletedByGroup cbg);

        void UpdateCompletedByGroupWorkflowMaps(int completedByGroupId, IList<int> workflowIds);
    }
}