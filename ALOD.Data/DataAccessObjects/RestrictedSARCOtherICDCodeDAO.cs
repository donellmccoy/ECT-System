using ALOD.Core.Domain.Modules.SARC;
using ALOD.Core.Interfaces.DAOInterfaces;

namespace ALOD.Data
{
    /// <summary>
    /// Data access object for <see cref="RestrictedSARCOtherICDCode"/> entities.
    /// Provides database operations for managing other ICD codes associated with restricted SARC cases.
    /// </summary>
    public class RestrictedSARCOtherICDCodeDAO : AbstractNHibernateDao<RestrictedSARCOtherICDCode, int>, IRestrictedSARCOtherICDCodeDAO
    { }
}