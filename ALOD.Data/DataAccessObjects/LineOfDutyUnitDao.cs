using ALOD.Core.Domain.Modules.Lod;
using ALOD.Core.Interfaces.DAOInterfaces;

namespace ALOD.Data
{
    /// <summary>
    /// Data access object for <see cref="LineOfDutyUnit"/> entities.
    /// Provides database operations for LOD unit data.
    /// </summary>
    public class LineOfDutyUnitDao : AbstractNHibernateDao<LineOfDutyUnit, int>, ILineOfDutyUnitDao
    { }
}