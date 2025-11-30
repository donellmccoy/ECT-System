using ALOD.Core.Domain.Documents;
using ALOD.Core.Interfaces.DAOInterfaces;
using NHibernate;
using NHibernate.Criterion;
using System.Collections.Generic;

namespace ALOD.Data
{
    /// <summary>
    /// Data access object for managing <see cref="DocumentView"/> entities.
    /// Handles document view operations including retrieval by description.
    /// </summary>
    public class DocumentViewDao : AbstractNHibernateDao<DocumentView, int>, IDocumentViewDao
    {
        /// <summary>
        /// Gets the current NHibernate session.
        /// </summary>
        private ISession Session
        {
            get { return NHibernateSessionManager.Instance.GetSession(); }
        }

        /// <summary>
        /// Retrieves a document view ID by its description.
        /// </summary>
        /// <param name="description">The description to search for.</param>
        /// <returns>The document view ID, or 0 if not found.</returns>
        public int GetIdByDescription(string description)
        {
            IList<DocumentView> list = Session.CreateCriteria(typeof(DocumentView))
            .Add(Expression.Eq("Description", description))
            .List<DocumentView>();

            if (list.Count > 0)
            {
                return list[0].Id;
            }
            else
            {
                return 0;
            }
        }
    }
}