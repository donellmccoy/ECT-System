using ALOD.Core.Domain.Users;
using ALOD.Core.Interfaces.DAOInterfaces;
using System;

namespace ALOD.Data
{
    /// <summary>
    /// Data access object for managing unit chain type entities.
    /// Provides basic CRUD operations through the base AbstractNHibernateDao class.
    /// </summary>
    public class UnitChainTypeDao : AbstractNHibernateDao<UnitChainType, Byte>, IUnitChainTypeDao
    { }
}