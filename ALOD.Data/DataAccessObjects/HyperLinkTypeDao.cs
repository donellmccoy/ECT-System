using ALOD.Core.Domain.WelcomePageBanner;
using ALOD.Core.Interfaces.DAOInterfaces;

namespace ALOD.Data
{
    /// <summary>
    /// Data access object for managing <see cref="HyperLinkType"/> entities.
    /// Extends the base NHibernate DAO without additional custom operations.
    /// </summary>
    public class HyperLinkTypeDao : AbstractNHibernateDao<HyperLinkType, int>, IHyperLinkTypeDao
    { }
}