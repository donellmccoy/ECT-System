using ALOD.Core.Domain.WelcomePageBanner;
using ALOD.Core.Interfaces.DAOInterfaces;

namespace ALOD.Data
{
    /// <summary>
    /// Data access object for managing <see cref="HyperLink"/> entities.
    /// Extends the base NHibernate DAO without additional custom operations.
    /// </summary>
    public class HyperLinkDao : AbstractNHibernateDao<HyperLink, int>, IHyperLinkDao
    { }
}