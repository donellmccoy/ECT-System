using ALOD.Core.Domain.Modules.Lod;
using ALOD.Core.Interfaces.DAOInterfaces;

namespace ALOD.Data
{
    /// <summary>
    /// Data access object for <see cref="LineOfDutyMedical"/> entities.
    /// Provides database operations for LOD medical data.
    /// </summary>
    public class LineOfDutyMedicalDao : AbstractNHibernateDao<LineOfDutyMedical, int>, ILineOfDutyMedicalDao
    { }
}