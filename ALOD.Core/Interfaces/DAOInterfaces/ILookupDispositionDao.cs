using ALOD.Core.Domain.Lookup;
using ALOD.Core.Domain.Workflow;
using System.Collections.Generic;

namespace ALOD.Core.Interfaces.DAOInterfaces
{
    public interface ILookupDispositionDao
    {
        IList<Disposition> GetAll();

        Disposition GetById(int id);

        IList<Workflow> GetDispositionWorkflows(int dispositionId);

        IList<Disposition> GetWorkflowDispositions(int workflowId);

        void InsertDisposition(string name);

        void UpdateDisposition(int id, string name);

        void UpdateDispositionWorkflowsMaps(int dispositionId, IList<int> workflowIds);
    }
}