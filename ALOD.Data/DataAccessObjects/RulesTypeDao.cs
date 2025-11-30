using ALOD.Core.Domain.Workflow;
using ALOD.Core.Interfaces.DAOInterfaces;

namespace ALOD.Data
{
    /// <summary>
    /// Data access object for <see cref="RuleType"/> entities.
    /// Provides database operations for workflow rule types.
    /// </summary>
    public class RulesTypeDao : AbstractNHibernateDao<RuleType, int>, IRuleTypeDao
    { }
}