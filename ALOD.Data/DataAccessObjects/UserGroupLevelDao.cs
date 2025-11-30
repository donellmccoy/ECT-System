using ALOD.Core.Domain.Users;
using ALOD.Core.Interfaces.DAOInterfaces;

namespace ALOD.Data
{
    /// <summary>
    /// Data access object for managing user group level entities.
    /// Provides basic CRUD operations through the base AbstractNHibernateDao class.
    /// </summary>
    public class UserGroupLevelDao : AbstractNHibernateDao<UserGroupLevel, int>, IUserGroupLevelDao
    {
    }
}