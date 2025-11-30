using ALOD.Core.Domain.Modules.SARC;
using ALOD.Core.Domain.Workflow;
using ALOD.Core.Interfaces.DAOInterfaces;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Data;

namespace ALOD.Data
{
    /// <summary>
    /// Data access object for <see cref="RestrictedSARC"/> entities.
    /// Provides operations for managing restricted Sexual Assault Response Coordinator (SARC) cases.
    /// </summary>
    internal class RestrictedSARCDao : AbstractNHibernateDao<RestrictedSARC, int>, ISARCDAO
    {
        private SqlDataStore _sqlDataStore;

        /// <summary>
        /// Gets the SQL data store for executing stored procedures.
        /// </summary>
        public SqlDataStore DataStore
        {
            get
            {
                if (_sqlDataStore == null)
                    _sqlDataStore = new SqlDataStore();

                return _sqlDataStore;
            }
        }

        /// <inheritdoc/>
        public RestrictedSARC GetByCaseId(string caseId)
        {
            ISession session = NHibernateSessionManager.Instance.GetSession();

            IList<RestrictedSARC> list = session.CreateCriteria(typeof(RestrictedSARC))
                .Add(Expression.Eq("CaseId", caseId))
                .List<RestrictedSARC>();

            if (list.Count > 0)
            {
                return list[0];
            }
            else
            {
                return null;
            }
        }

        /// <inheritdoc/>
        public int GetCaseCount(int userId)
        {
            Object result = DataStore.ExecuteScalar("core_sarc_sp_GetOpenCasesCount", userId);

            if (result == null)
                return -1;
            else
                return (int)result;
        }

        /// <inheritdoc/>
        public DataSet GetOpenCasesForUser(int userId)
        {
            return DataStore.ExecuteDataSet("core_sarc_sp_GetOpenCases", userId);
        }

        /// <inheritdoc/>
        public int GetPostCompletionCaseCount(int userId)
        {
            Object result = DataStore.ExecuteScalar("core_sarc_sp_GetPostCompletionCasesCount", userId);

            if (result == null)
                return -1;
            else
                return (int)result;
        }

        /// <inheritdoc/>
        public DataSet GetPostCompletionSearchResults(int userId, string caseId, string memberSSN, string memberName, int reportView, string compo, int unitId)
        {
            return DataStore.ExecuteDataSet("core_sarc_sp_PostCompletionSearch", userId, caseId, memberSSN, memberName, reportView, compo, unitId);
        }

        /// <inheritdoc/>
        public DataSet GetSearchResults(int userId, string caseId, string memberSSN, string memberName, int reportView, string compo, int status, int unitId)
        {
            return DataStore.ExecuteDataSet("core_sarc_sp_Search", userId, caseId, memberSSN, memberName, reportView, compo, status, unitId);
        }

        /// <inheritdoc/>
        public PageAccess.AccessLevel GetUserAccess(int userId, int refId)
        {
            SqlDataStore store = new SqlDataStore();
            int access = (int)store.ExecuteScalar("lod_sp_UserHasAccess", userId, refId, ModuleType.SARC);

            switch (access)
            {
                case 0:
                    return PageAccess.AccessLevel.None;

                case 1:
                    return PageAccess.AccessLevel.ReadOnly;

                case 2:
                    return PageAccess.AccessLevel.ReadWrite;

                default:
                    return PageAccess.AccessLevel.None;
            }
        }
    }
}