using ALOD.Core.Domain.Modules.Lod;
using ALOD.Core.Interfaces.DAOInterfaces;

namespace ALOD.Data
{
    /// <summary>
    /// Data access object for managing <see cref="LineOfDutyMedical"/> entities.
    /// Provides data persistence operations for Air National Guard LOD medical information.
    /// </summary>
    public class ANGLineOfDutyMedicalDao : AbstractNHibernateDao<LineOfDutyMedical, int>, ILineOfDutyMedicalDao
    { }
}