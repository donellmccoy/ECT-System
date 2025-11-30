using ALOD.Core.Domain.Modules.Lod;
using ALOD.Core.Domain.Workflow;

namespace ALOD.Core.Interfaces.DAOInterfaces
{
    public interface ILineOfDutyDao : IDao<LineOfDuty, int>
    {
        string GetFromAndDirection(int refId);

        int GetPendingCount(int userId, bool sarc);

        int GetPendingIOCount(int userId, bool sarc);

        PageAccess.AccessLevel GetUserAccess(int userId, int refId);

        /* int GetPendingLegacyLodCount(int userId, bool sarc)*/

        int GetWorkflow(int refid);
    }
}