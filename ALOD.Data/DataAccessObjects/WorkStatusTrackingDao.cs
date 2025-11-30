using ALOD.Core.Domain.Workflow;
using ALOD.Core.Interfaces.DAOInterfaces;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;

namespace ALOD.Data
{
    internal class WorkStatusTrackingDao : AbstractNHibernateDao<WorkStatusTracking, int>, IWorkStatusTrackingDao
    {
        private ISession Session
        {
            get { return NHibernateSessionManager.Instance.GetSession(); }
        }

        /// <inheritdoc/>
        public int GetLastStatus(int refId, short module)
        {
            IList<WorkStatusTracking> lst = Session.CreateCriteria(typeof(WorkStatusTracking))
               .Add(Expression.Eq("ReferenceId", refId))
               .Add(Expression.Eq("ModuleId", Convert.ToByte(module)))
               .Add(Expression.IsNotNull("EndDate"))
               .AddOrder(Order.Desc("Id"))
               .List<WorkStatusTracking>();

            if (lst.Count > 0)
            {
                return lst[0].WorkflowStatus.Id;
            }
            else
            {
                return 0;
            }
        }

        /// <inheritdoc/>
        public IList<WorkStatusTracking> GetWorkStatusTracking(int refId, short module)
        {
            IList<WorkStatusTracking> lst = Session.CreateCriteria(typeof(WorkStatusTracking))
               .Add(Expression.Eq("ReferenceId", refId))
               .Add(Expression.Eq("ModuleId", Convert.ToByte(module)))
               .Add(Expression.IsNotNull("EndDate"))
               .AddOrder(Order.Desc("Id"))
               .List<WorkStatusTracking>();

            return lst;
        }
    }
}