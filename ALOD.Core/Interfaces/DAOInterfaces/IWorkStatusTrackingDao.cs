using ALOD.Core.Domain.Workflow;
using System.Collections.Generic;

namespace ALOD.Core.Interfaces.DAOInterfaces
{
    public interface IWorkStatusTrackingDao : IDao<WorkStatusTracking, int>
    {
        int GetLastStatus(int refId, short module);

        IList<WorkStatusTracking> GetWorkStatusTracking(int refId, short module);
    }
}