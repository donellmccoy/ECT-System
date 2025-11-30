using ALOD.Core.Domain.Modules.Lod;
using ALOD.Core.Interfaces.DAOInterfaces;

namespace ALOD.Data
{
    /// <summary>
    /// Data access object for <see cref="LineOfDutyInvestigation"/> entities.
    /// Provides database operations for LOD investigation data.
    /// </summary>
    public class LineOfDutyInvestigationDao : AbstractNHibernateDao<LineOfDutyInvestigation, int>, ILineOfDutyInvestigationDao
    { }
}