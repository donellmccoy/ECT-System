using ALOD.Core.Domain.Modules.Lod;
using ALOD.Core.Interfaces.DAOInterfaces;

namespace ALOD.Data
{
    /// <summary>
    /// Data access object for managing <see cref="LineOfDutyInvestigation"/> entities.
    /// Provides data persistence operations for Air National Guard LOD investigations.
    /// </summary>
    public class ANGLineOfDutyInvestigationDao : AbstractNHibernateDao<LineOfDutyInvestigation, int>, ILineOfDutyInvestigationDao
    { }
}