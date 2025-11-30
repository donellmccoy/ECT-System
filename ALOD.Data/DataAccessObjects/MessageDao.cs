using ALOD.Core.Domain.Messaging;
using ALOD.Core.Interfaces.DAOInterfaces;

namespace ALOD.Data
{
    /// <summary>
    /// Data access object for <see cref="Message"/> entities.
    /// Provides database operations for system messages.
    /// </summary>
    public class MessageDao : AbstractNHibernateDao<Message, int>, IMessageDao
    { }
}