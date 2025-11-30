using ALOD.Core.Domain.Workflow;
using ALOD.Core.Interfaces.DAOInterfaces;

namespace ALOD.Data
{
    /// <summary>
    /// Data access object for managing workflow option action entities.
    /// Provides basic CRUD operations through the base AbstractNHibernateDao class.
    /// </summary>
    public class WorkflowOptionActionsDao : AbstractNHibernateDao<WorkflowOptionAction, int>, IWorkflowOptionActionsDao
    { }
}