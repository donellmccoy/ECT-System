using ALOD.Core.Domain.Modules.Lod;
using ALOD.Core.Utils;
using NHibernate;
using NHibernate.Cache;
using NHibernate.Cfg;
using System;
using System.Runtime.Remoting.Messaging;
using System.Web;

namespace ALOD.Data
{
    public sealed class NHibernateSessionManager
    {
        #region Thread-safe, lazy Singleton

        private NHibernateSessionManager()
        {
            // Use lazy initialization - don't initialize session factory in constructor
            // This prevents startup issues and allows for better error handling
        }

        public static NHibernateSessionManager Instance
        {
            get
            {
                return Nested.NHibernateSessionManager;
            }
        }

        private sealed class Nested
        {
            internal static readonly NHibernateSessionManager NHibernateSessionManager =
                new NHibernateSessionManager();

            static Nested()
            {
            }
        }

        #endregion Thread-safe, lazy Singleton

        private const string SESSION_KEY = "CONTEXT_SESSION";
        private const string TRANSACTION_KEY = "CONTEXT_TRANSACTION";
        private ISessionFactory sessionFactory;
        private readonly object sessionFactoryLock = new object();

        private ISession ContextSession
        {
            get
            {
                if (IsInWebContext())
                {
                    return (ISession)HttpContext.Current.Items[SESSION_KEY];
                }
                else
                {
                    return (ISession)CallContext.GetData(SESSION_KEY);
                }
            }
            set
            {
                if (IsInWebContext())
                {
                    HttpContext.Current.Items[SESSION_KEY] = value;
                }
                else
                {
                    CallContext.SetData(SESSION_KEY, value);
                }
            }
        }

        private ITransaction ContextTransaction
        {
            get
            {
                if (IsInWebContext())
                {
                    return (ITransaction)HttpContext.Current.Items[TRANSACTION_KEY];
                }
                else
                {
                    return (ITransaction)CallContext.GetData(TRANSACTION_KEY);
                }
            }
            set
            {
                if (IsInWebContext())
                {
                    HttpContext.Current.Items[TRANSACTION_KEY] = value;
                }
                else
                {
                    CallContext.SetData(TRANSACTION_KEY, value);
                }
            }
        }

        public void BeginTransaction()
        {
            ITransaction transaction = ContextTransaction;

            if (transaction == null)
            {
                transaction = GetSession().BeginTransaction();
                ContextTransaction = transaction;
            }
        }

        public void CloseSession()
        {
            ISession session = ContextSession;

            if (session != null && session.IsOpen)
            {
                session.Clear();
                session.Close();
            }

            ContextSession = null;
        }

        public void CommitTransaction()
        {
            ITransaction transaction = ContextTransaction;

            try
            {
                if (HasOpenTransaction())
                {
                    transaction.Commit();
                    ContextTransaction = null;
                }
            }
            catch (HibernateException)
            {
                RollbackTransaction();
                throw;
            }
        }

        public bool DoesFindingExist(int lodId, int ptype)
        {
            ISession session = GetSession();
            var query = session.CreateQuery("SELECT COUNT(*) FROM LineOfDutyFindings WHERE LODID = :lodId AND PType = :ptype");
            query.SetParameter("lodId", lodId);
            query.SetParameter("ptype", ptype);
            return (long)query.UniqueResult() > 0;
        }

        public ISession GetSession()
        {
            return GetSession(null);
        }

        public bool HasOpenTransaction()
        {
            ITransaction transaction = ContextTransaction;

            return transaction != null && !transaction.WasCommitted && !transaction.WasRolledBack;
        }

        public void InsertFinding(LineOfDutyFindings finding)
        {
            if (finding.LODID.HasValue && DoesFindingExist(finding.LODID.Value, finding.PType))
            {
                throw new InvalidOperationException($"A finding with LODID {finding.LODID} and ptype {finding.PType} already exists.");
            }

            if (!IsValidPType(finding.PType))
            {
                throw new InvalidOperationException($"The PType {finding.PType} is not valid.");
            }

            ISession session = GetSession();
            using (ITransaction transaction = session.BeginTransaction())
            {
                try
                {
                    session.Save(finding);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new InvalidOperationException("An error occurred while inserting the finding.", ex);
                }
            }
        }

        public bool IsValidPType(short ptype)
        {
            ISession session = GetSession();
            var query = session.CreateQuery("SELECT COUNT(*) FROM core_lkupPersonnelTypes WHERE Id = :ptype");
            query.SetParameter("ptype", ptype);
            return (long)query.UniqueResult() > 0;
        }

        public void RegisterInterceptor(IInterceptor interceptor)
        {
            ISession session = ContextSession;

            if (session != null && session.IsOpen)
            {
                throw new CacheException("You cannot register an interceptor once a session has already been opened");
            }

            GetSession(interceptor);
        }

        public void RollbackTransaction()
        {
            ITransaction transaction = ContextTransaction;

            try
            {
                if (HasOpenTransaction())
                {
                    transaction.Rollback();
                }

                ContextTransaction = null;
            }
            finally
            {
                CloseSession();
            }
        }

        private ISession GetSession(IInterceptor interceptor)
        {
            // Ensure session factory is initialized
            EnsureSessionFactoryInitialized();
            
            ISession session = ContextSession;

            if (session == null)
            {
                if (interceptor != null)
                {
                    session = sessionFactory.WithOptions().Interceptor(interceptor).OpenSession();
                }
                else
                {
                    session = sessionFactory.OpenSession();
                }

                ContextSession = session;
            }

            Check.Ensure(session != null, "session was null");

            return session;
        }

        private void EnsureSessionFactoryInitialized()
        {
            if (sessionFactory == null)
            {
                lock (sessionFactoryLock)
                {
                    if (sessionFactory == null)
                    {
                        InitSessionFactory();
                    }
                }
            }
        }

        private void InitSessionFactory()
        {
            // Validate connection string first
            ValidateConnectionString();
            
            int maxRetries = 3;
            int retryDelay = 2000; // 2 seconds
            
            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    sessionFactory = new Configuration().Configure().BuildSessionFactory();
                    break; // Success, exit retry loop
                }
                catch (Exception ex)
                {
                    if (attempt == maxRetries)
                    {
                        // Log the error and rethrow on final attempt
                        System.Diagnostics.Debug.WriteLine($"Failed to initialize NHibernate session factory after {maxRetries} attempts. Error: {ex.Message}");
                        throw;
                    }
                    
                    // Wait before retrying
                    System.Threading.Thread.Sleep(retryDelay);
                    System.Diagnostics.Debug.WriteLine($"NHibernate session factory initialization attempt {attempt} failed. Retrying in {retryDelay}ms...");
                }
            }
        }

        private void ValidateConnectionString()
        {
            try
            {
                var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["LOD"]?.ConnectionString;
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new InvalidOperationException("LOD connection string is not configured or is empty.");
                }

                // Test the connection
                using (var connection = new System.Data.SqlClient.SqlConnection(connectionString))
                {
                    connection.Open();
                    System.Diagnostics.Debug.WriteLine("Database connection validation successful.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Database connection validation failed: {ex.Message}");
                throw new InvalidOperationException($"Database connection validation failed: {ex.Message}", ex);
            }
        }

        private bool IsInWebContext()
        {
            return HttpContext.Current != null;
        }
    }
}