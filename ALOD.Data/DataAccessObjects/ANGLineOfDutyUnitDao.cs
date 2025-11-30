using ALOD.Core.Domain.Modules.Lod;
using ALOD.Core.Interfaces.DAOInterfaces;

namespace ALOD.Data
{
    /// <summary>
    /// Data access object for managing <see cref="LineOfDutyUnit"/> entities.
    /// Provides data persistence operations for Air National Guard LOD unit information.
    /// </summary>
    public class ANGLineOfDutyUnitDao : AbstractNHibernateDao<LineOfDutyUnit, int>, ILineOfDutyUnitDao
    { }
}