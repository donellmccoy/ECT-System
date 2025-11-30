using ALOD.Core.Domain.Workflow;
using ALOD.Core.Interfaces.DAOInterfaces;

namespace ALOD.Data
{
    /// <summary>
    /// Data access object for managing <see cref="ActionTypes"/> entities.
    /// Provides CRUD operations for workflow action types in the ALOD system.
    /// </summary>
    public class ActionTypesDao : AbstractNHibernateDao<ActionTypes, int>, IActionTypesDao
    { }
}