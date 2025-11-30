using ALOD.Core.Domain.Modules.Lod;
using ALOD.Core.Interfaces.DAOInterfaces;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;

namespace ALOD.Data
{
    /// <summary>
    /// Data access object for <see cref="Rwoa"/> entities.
    /// Manages RWOA (Request for Waiver of Objection to Appeal) actions for LOD cases.
    /// </summary>
    public class RwoaDao : AbstractNHibernateDao<Rwoa, int>, IRwoaDao
    {
        /// <summary>
        /// Gets the NHibernate session for database operations.
        /// </summary>
        private ISession Session
        {
            get { return NHibernateSessionManager.Instance.GetSession(); }
        }

        /// <inheritdoc/>
        public Rwoa GetRecentRWOA(int workflow, int refid)
        {
            IList<Rwoa> list = Session.CreateCriteria(typeof(Rwoa))
            .Add(Expression.Eq("Workflow", (Int16)workflow))
            .Add(Expression.Eq("RefId", refid))
            .AddOrder(Order.Desc("DateSent"))
            .List<Rwoa>();

            if (list == null || list.Count == 0)
                return null;

            return list[0];
        }
    }
}