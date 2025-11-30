using ALOD.Core.Domain.Modules.Lod;
using ALOD.Core.Interfaces.DAOInterfaces;

namespace ALOD.Data
{
    /// <summary>
    /// Data access object for <see cref="LineOfDutyFindings"/> entities.
    /// Provides database operations for LOD findings data.
    /// </summary>
    internal class LineOfDutyFindingsDao : AbstractNHibernateDao<LineOfDutyFindings, int>, ILineOfDutyFindingsDao
    {
        /// <summary>
        /// Saves a new LOD findings entity to the database.
        /// </summary>
        /// <param name="entity">The <see cref="LineOfDutyFindings"/> entity to save.</param>
        /// <returns>The saved <see cref="LineOfDutyFindings"/> entity with updated identifier.</returns>
        public override LineOfDutyFindings Save(LineOfDutyFindings entity)
        {
            return base.Save(entity);
        }

        /// <summary>
        /// Saves or updates an existing LOD findings entity in the database.
        /// </summary>
        /// <param name="entity">The <see cref="LineOfDutyFindings"/> entity to save or update.</param>
        /// <returns>The saved or updated <see cref="LineOfDutyFindings"/> entity.</returns>
        public override LineOfDutyFindings SaveOrUpdate(LineOfDutyFindings entity)
        {
            return base.SaveOrUpdate(entity);
        }
    }
}