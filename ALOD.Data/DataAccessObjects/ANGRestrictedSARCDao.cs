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
    /// Data access object for managing <see cref="RestrictedSARC"/> (Sexual Assault Response Coordinator) entities in the Air National Guard context.
    /// Handles restricted SARC case operations including searches, access control, and case management.
    /// </summary>
    internal class ANGRestrictedSARCDao : AbstractNHibernateDao<RestrictedSARC, int>, ISARCDAO
    {
        private SqlDataStore _sqlDataStore;

        /// <summary>
        /// Gets the SQL data store instance for database operations.
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

        /// <summary>
        /// Retrieves a restricted SARC case by its unique case identifier.
        /// </summary>
        /// <param name="caseId">The unique case identifier.</param>
        /// <returns>The <see cref="RestrictedSARC"/> entity if found; otherwise, null.</returns>
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

        /// <summary>
        /// Gets the count of open restricted SARC cases for a specific user.
        /// Executes stored procedure: core_sarc_sp_GetOpenCasesCount
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>The count of open cases, or -1 if the result is null.</returns>
        public int GetCaseCount(int userId)
        {
            Object result = DataStore.ExecuteScalar("core_sarc_sp_GetOpenCasesCount", userId);

            if (result == null)
                return -1;
            else
                return (int)result;
        }

        /// <summary>
        /// Retrieves all open restricted SARC cases for a specific user.
        /// Executes stored procedure: core_sarc_sp_GetOpenCases
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>A <see cref="DataSet"/> containing open restricted SARC case records.</returns>
        public DataSet GetOpenCasesForUser(int userId)
        {
            return DataStore.ExecuteDataSet("core_sarc_sp_GetOpenCases", userId);
        }

        /// <summary>
        /// Gets the count of post-completion restricted SARC cases for a specific user.
        /// Executes stored procedure: core_sarc_sp_GetPostCompletionCasesCount
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>The count of post-completion cases, or -1 if the result is null.</returns>
        public int GetPostCompletionCaseCount(int userId)
        {
            Object result = DataStore.ExecuteScalar("core_sarc_sp_GetPostCompletionCasesCount", userId);

            if (result == null)
                return -1;
            else
                return (int)result;
        }

        /// <summary>
        /// Searches for post-completion restricted SARC cases.
        /// Executes stored procedure: core_sarc_sp_PostCompletionSearch
        /// </summary>
        /// <param name="userId">The user identifier performing the search.</param>
        /// <param name="caseId">The case identifier to search for.</param>
        /// <param name="memberSSN">The member's social security number to search for.</param>
        /// <param name="memberName">The member's name to search for.</param>
        /// <param name="reportView">The report view type.</param>
        /// <param name="compo">The component filter.</param>
        /// <param name="unitId">The unit identifier filter.</param>
        /// <returns>A <see cref="DataSet"/> containing post-completion restricted SARC case records.</returns>
        public DataSet GetPostCompletionSearchResults(int userId, string caseId, string memberSSN, string memberName, int reportView, string compo, int unitId)
        {
            return DataStore.ExecuteDataSet("core_sarc_sp_PostCompletionSearch", userId, caseId, memberSSN, memberName, reportView, compo, unitId);
        }

        /// <summary>
        /// Searches for restricted SARC cases using various search criteria.
        /// Executes stored procedure: core_sarc_sp_Search
        /// </summary>
        /// <param name="userId">The user identifier performing the search.</param>
        /// <param name="caseId">The case identifier to search for.</param>
        /// <param name="memberSSN">The member's social security number to search for.</param>
        /// <param name="memberName">The member's name to search for.</param>
        /// <param name="reportView">The report view type.</param>
        /// <param name="compo">The component filter.</param>
        /// <param name="status">The status filter.</param>
        /// <param name="unitId">The unit identifier filter.</param>
        /// <returns>A <see cref="DataSet"/> containing matching restricted SARC case records.</returns>
        public DataSet GetSearchResults(int userId, string caseId, string memberSSN, string memberName, int reportView, string compo, int status, int unitId)
        {
            return DataStore.ExecuteDataSet("core_sarc_sp_Search", userId, caseId, memberSSN, memberName, reportView, compo, status, unitId);
        }

        /// <summary>
        /// Determines the user's access level for a specific restricted SARC case.
        /// Executes stored procedure: lod_sp_UserHasAccess with ModuleType.SARC
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="refId">The reference identifier of the restricted SARC case.</param>
        /// <returns>The user's <see cref="PageAccess.AccessLevel"/> (None, ReadOnly, or ReadWrite).</returns>
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