using ALOD.Core.Domain.Modules.Lod;
using ALOD.Core.Interfaces.DAOInterfaces;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;

namespace ALOD.Data
{
    /// <summary>
    /// Data access object for <see cref="Return"/> entities.
    /// Manages return actions (sending cases back to previous workflow stages) for LOD cases.
    /// </summary>
    public class ReturnDao : AbstractNHibernateDao<Return, int>, IReturnDao
    {
        /// <summary>
        /// Gets the NHibernate session for database operations.
        /// </summary>
        private ISession Session
        {
            get { return NHibernateSessionManager.Instance.GetSession(); }
        }

        /// <inheritdoc/>
        public Return GetRecentReturn(int workflow, int refid)
        {
            IList<Return> list = Session.CreateCriteria(typeof(Return))
            .Add(Expression.Eq("Workflow", (Int16)workflow))
            .Add(Expression.Eq("RefId", refid))
            .AddOrder(Order.Desc("DateSent"))
            .List<Return>();

            if (list == null || list.Count == 0)
                return null;

            return list[0];
        }
    }
}