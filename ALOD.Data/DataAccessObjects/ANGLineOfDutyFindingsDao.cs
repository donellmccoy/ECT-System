using ALOD.Core.Domain.Modules.Lod;
using ALOD.Core.Interfaces.DAOInterfaces;

namespace ALOD.Data
{
    /// <summary>
    /// Data access object for managing <see cref="LineOfDutyFindings"/> entities.
    /// Provides data persistence operations for Air National Guard LOD findings.
    /// </summary>
    internal class ANGLineOfDutyFindingsDao : AbstractNHibernateDao<LineOfDutyFindings, int>, ILineOfDutyFindingsDao
    {
        /// <inheritdoc/>
        public override LineOfDutyFindings Save(LineOfDutyFindings entity)
        {
            return base.Save(entity);
        }

        /// <inheritdoc/>
        public override LineOfDutyFindings SaveOrUpdate(LineOfDutyFindings entity)
        {
            return base.SaveOrUpdate(entity);
        }
    }
}