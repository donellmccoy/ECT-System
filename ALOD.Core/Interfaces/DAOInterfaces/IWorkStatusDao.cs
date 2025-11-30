using ALOD.Core.Domain.Workflow;
using System.Collections.Generic;
using System.Data;

namespace ALOD.Core.Interfaces.DAOInterfaces
{
    public interface IWorkStatusDao : IDao<WorkStatus, int>
    {
        //07-17-2019 S32019 - Curt Lucas
        IList<WorkStatus> GetByWorkflow(byte workflow);

        string GetDescription(int workstatusId);

        DataSet GetStatusDescription(int workstatusId);
    }
}