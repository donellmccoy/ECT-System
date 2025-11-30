using ALOD.Core.Domain.Modules.SARC;
using ALOD.Core.Interfaces.DAOInterfaces;

namespace ALOD.Data
{
    /// <summary>
    /// Data access object for managing <see cref="RestrictedSARCOtherICDCode"/> entities.
    /// Provides data persistence operations for additional ICD codes associated with Air National Guard SARC cases.
    /// </summary>
    public class ANGRestrictedSARCOtherICDCodeDAO : AbstractNHibernateDao<RestrictedSARCOtherICDCode, int>, IRestrictedSARCOtherICDCodeDAO
    { }
}