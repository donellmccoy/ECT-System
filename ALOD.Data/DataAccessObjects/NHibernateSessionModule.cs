using System;
using System.Web;

/// <summary>
/// Implements the Open-Session-In-View pattern using <see cref="NHibernateSessionManager" />.
/// Opens a transaction at the beginning of each HTTP request and commits/closes it at the end.
/// </summary>
/// <remarks>
/// <para>Assumes that each HTTP request is given a single transaction for the entire page lifecycle.</para>
/// <para>Only processes requests with .aspx or .asmx extensions.</para>
/// <para>Inspiration for this class came from Ed Courtenay at
/// http://sourceforge.net/forum/message.php?msg_id=2847509.</para>
/// </remarks>
namespace ALOD.Data
{
    /// <summary>
    /// HTTP module that manages NHibernate sessions for the duration of each HTTP request.
    /// </summary>
    public class NHibernateSessionModule : IHttpModule
    {
        /// <inheritdoc/>
        public void Dispose()
        { }

        /// <inheritdoc/>
        public void Init(HttpApplication context)
        {
            context.BeginRequest += new EventHandler(BeginTransaction);
            context.EndRequest += new EventHandler(CommitAndCloseSession);
        }

        /// <summary>
        /// Opens a session within a transaction at the beginning of the HTTP request.
        /// This doesn't actually open a connection to the database until needed.
        /// </summary>
        private void BeginTransaction(object sender, EventArgs e)
        {
            HttpRequest request = ((HttpApplication)sender).Request;

            string extension = System.IO.Path.GetExtension(request.Path).ToLower();

            if (extension == ".aspx" || extension == ".asmx")
            {
                NHibernateSessionManager.Instance.BeginTransaction();
            }
        }

        /// <summary>
        /// Commits and closes the NHibernate session provided by the supplied <see cref="NHibernateSessionManager"/>.
        /// Assumes a transaction was begun at the beginning of the request; but a transaction or session does
        /// not *have* to be opened for this to operate successfully.
        /// </summary>
        private void CommitAndCloseSession(object sender, EventArgs e)
        {
            HttpRequest request = ((HttpApplication)sender).Request;

            string extension = System.IO.Path.GetExtension(request.Path).ToLower();

            if (extension == ".aspx" || extension == ".asmx")
            {
                try
                {
                    NHibernateSessionManager.Instance.CommitTransaction();
                }
                finally
                {
                    NHibernateSessionManager.Instance.CloseSession();
                }
            }
        }
    }
}