using ALOD.Core.Domain.Users;
using ALOD.Core.Interfaces.DAOInterfaces;

namespace ALOD.Data
{
    /// <summary>
    /// Data access object for managing unit chain entry entities.
    /// Provides basic CRUD operations through the base AbstractNHibernateDao class.
    /// </summary>
    public class UnitChainEntryDao : AbstractNHibernateDao<UnitChainEntry, int>, IUnitChainEntryDao
    { }
}