using ALOD.Core.Domain.Common;
using ALOD.Core.Interfaces.DAOInterfaces;
using NHibernate;
using NHibernate.Criterion;
using System.Collections.Generic;

namespace ALOD.Data
{
    /// <summary>
    /// Data access object for managing <see cref="EmailTemplate"/> entities.
    /// Handles email template operations including retrieval of system templates and templates by title.
    /// </summary>
    public class EmailTemplateDao : AbstractNHibernateDao<EmailTemplate, int>, IEmailTemplateDao
    {
        /// <summary>
        /// Gets the current NHibernate session.
        /// </summary>
        private ISession Session
        {
            get { return NHibernateSessionManager.Instance.GetSession(); }
        }

        /// <summary>
        /// Retrieves all system email templates (templates with titles starting with "System ").
        /// </summary>
        /// <returns>A list of system email templates.</returns>
        public IList<EmailTemplate> GetSystemTemplates()
        {
            IList<EmailTemplate> list = Session.CreateCriteria(typeof(EmailTemplate))
            .Add(Expression.Like("Title", "System %"))
            .List<EmailTemplate>();

            return list;
        }

        /// <summary>
        /// Retrieves an email template by its title.
        /// </summary>
        /// <param name="title">The template title to search for.</param>
        /// <returns>The matching <see cref="EmailTemplate"/>, or null if not found.</returns>
        public EmailTemplate GetTemplateByTitle(string title)
        {
            IList<EmailTemplate> list = Session.CreateCriteria(typeof(EmailTemplate))
            .Add(Expression.Like("Title", title))
            .List<EmailTemplate>();

            if (list == null || list.Count == 0)
                return null;

            return list[0];
        }
    }
}