using ALOD.Core.Domain.Modules.Lod;

namespace ALOD.Core.Interfaces.DAOInterfaces
{
    public interface IReturnDao : IDao<Return, int>
    {
        Return GetRecentReturn(int workflow, int refid);
    }
}