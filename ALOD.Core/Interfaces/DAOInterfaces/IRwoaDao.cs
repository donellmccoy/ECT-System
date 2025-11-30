using ALOD.Core.Domain.Modules.Lod;

namespace ALOD.Core.Interfaces.DAOInterfaces
{
    public interface IRwoaDao : IDao<Rwoa, int>
    {
        Rwoa GetRecentRWOA(int workflow, int refid);
    }
}