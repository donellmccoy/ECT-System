using ALOD.Core.Domain.Modules.Lod;

namespace ALOD.Core.Interfaces.DAOInterfaces
{
    public interface ILineOfDutyAuditDao : IDao<LineOfDutyAudit, int>
    {
        System.Data.DataSet GetAuditInfo(int groupId);
    }
}