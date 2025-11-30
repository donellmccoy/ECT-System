using ALOD.Core.Domain.Lookup;
using ALOD.Core.Interfaces.DAOInterfaces;

namespace ALOD.Data
{
    /// <summary>
    /// Data access object for managing suicide method lookup entities.
    /// Provides basic CRUD operations through the base AbstractNHibernateDao class.
    /// </summary>
    public class SuicideMethodDao : AbstractNHibernateDao<SuicideMethod, int>, ISuicideMethodDao
    { }
}