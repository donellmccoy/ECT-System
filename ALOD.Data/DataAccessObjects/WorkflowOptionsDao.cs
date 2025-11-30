using ALOD.Core.Domain.Workflow;
using ALOD.Core.Interfaces.DAOInterfaces;

namespace ALOD.Data
{
    /// <summary>
    /// Data access object for managing workflow status option entities.
    /// Provides basic CRUD operations through the base AbstractNHibernateDao class.
    /// </summary>
    public class WorkflowOptionsDao : AbstractNHibernateDao<WorkflowStatusOption, int>, IWorkflowOptionDao
    { }
}