using ALOD.Core.Domain.Modules.Lod;
using ALOD.Core.Interfaces.DAOInterfaces;

namespace ALOD.Data
{
    /// <summary>
    /// Data access object for <see cref="RwoaReason"/> entities.
    /// Provides database operations for RWOA (Request for Waiver of Objection to Appeal) reasons.
    /// </summary>
    public class RwoaReasonDao : AbstractNHibernateDao<RwoaReason, int>, IRwoaReasonDao
    { }
}