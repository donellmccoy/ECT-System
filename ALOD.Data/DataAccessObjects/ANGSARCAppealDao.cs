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
    /// Data access object for managing <see cref="SARCAppeal"/> entities in the Air National Guard context.
    /// Handles SARC appeal operations including searches, access control, and post-completion processing.
    /// </summary>
    internal class ANGSARCAppealDao : AbstractNHibernateDao<SARCAppeal, int>, ISARCAppealDAO
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
        /// Gets the count of SARC appeal requests associated with an initial case.
        /// Executes stored procedure: core_sarc_sp_GetAppealCount
        /// </summary>
        /// <param name="initialId">The initial case identifier.</param>
        /// <param name="workflowId">The workflow identifier.</param>
        /// <returns>The count of associated SARC appeal requests.</returns>
        public int GetAppealCount(int initialId, int workflowId)
        {
            SqlDataStore store = new SqlDataStore();
            return (int)store.ExecuteScalar("core_sarc_sp_GetAppealCount", initialId, workflowId);
        }

        /// <summary>
        /// Retrieves the SARC appeal request identifier associated with an initial case.
        /// Executes stored procedure: core_sarc_sp_GetAppealId
        /// </summary>
        /// <param name="LodId">The initial LOD identifier.</param>
        /// <param name="workflowId">The workflow identifier.</param>
        /// <returns>The SARC appeal request identifier.</returns>
        public int GetAppealIdByInitId(int LodId, int workflowId)
        {
            SqlDataStore store = new SqlDataStore();
            return (int)store.ExecuteScalar("core_sarc_sp_GetAppealId", LodId, workflowId);
        }

        /// <summary>
        /// Retrieves all SARC appeal requests for the specified role.
        /// Executes stored procedure: core_sarc_sp_GetAppealRequests
        /// </summary>
        /// <param name="roleId">The role identifier.</param>
        /// <param name="sarc">True to include SARC cases; otherwise, false.</param>
        /// <returns>A <see cref="DataSet"/> containing SARC appeal request records.</returns>
        public DataSet GetAppealRequests(int roleId, bool sarc)
        {
            SqlDataStore store = new SqlDataStore();
            return store.ExecuteDataSet("core_sarc_sp_GetAppealRequests", roleId, sarc);
        }

        /// <summary>
        /// Retrieves a SARC appeal request by its unique case identifier.
        /// </summary>
        /// <param name="caseId">The unique case identifier.</param>
        /// <returns>The <see cref="SARCAppeal"/> entity if found; otherwise, null.</returns>
        public SARCAppeal GetByCaseId(string caseId)
        {
            ISession session = NHibernateSessionManager.Instance.GetSession();

            IList<SARCAppeal> list = session.CreateCriteria(typeof(SARCAppeal))
                .Add(Expression.Eq("CaseId", caseId))
                .List<SARCAppeal>();

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
        /// Retrieves all completed SARC appeal requests (APs) for the specified role.
        /// Executes stored procedure: core_sarc_sp_GetCompletedAPs
        /// </summary>
        /// <param name="roleId">The role identifier.</param>
        /// <param name="sarc">True to include SARC cases; otherwise, false.</param>
        /// <returns>A <see cref="DataSet"/> containing completed SARC appeal request records.</returns>
        public DataSet GetCompletedAPs(int roleId, bool sarc)
        {
            SqlDataStore store = new SqlDataStore();
            return store.ExecuteDataSet("core_sarc_sp_GetCompletedAPs", roleId, sarc);
        }

        /// <summary>
        /// Gets the count of post-completion SARC appeal cases for a specific user.
        /// Executes stored procedure: core_sarc_sp_GetPostCompletionAppealCasesCount
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>The count of post-completion SARC appeal cases, or -1 if the result is null.</returns>
        public int GetPostCompletionCaseCount(int userId)
        {
            Object result = DataStore.ExecuteScalar("core_sarc_sp_GetPostCompletionAppealCasesCount", userId);

            if (result == null)
                return -1;
            else
                return (int)result;
        }

        /// <summary>
        /// Searches for post-completion SARC appeal records.
        /// Executes stored procedure: core_sarc_sp_PostAppealCompletionSearch
        /// </summary>
        /// <param name="caseID">The case identifier to search for.</param>
        /// <param name="ssn">The social security number to search for.</param>
        /// <param name="name">The name to search for.</param>
        /// <param name="userId">The user identifier performing the search.</param>
        /// <param name="rptView">The report view type.</param>
        /// <param name="compo">The component filter.</param>
        /// <param name="maxCount">The maximum number of results to return.</param>
        /// <param name="moduleId">The module identifier.</param>
        /// <param name="unitId">The unit identifier filter.</param>
        /// <param name="sarcpermission">True if the user has SARC permissions; otherwise, false.</param>
        /// <returns>A <see cref="DataSet"/> containing post-completion SARC appeal records.</returns>
        public DataSet GetPostSARCAppealCompletion(string caseID, string ssn, string name,
                                         int userId, byte rptView, string compo, int maxCount, byte moduleId,
                                         int unitId, bool sarcpermission)
        {
            SqlDataStore source = new SqlDataStore();
            return source.ExecuteDataSet("core_sarc_sp_PostAppealCompletionSearch", caseID, ssn, name, userId, rptView,
                  compo, maxCount, moduleId, unitId, sarcpermission);
        }

        /// <summary>
        /// Determines the user's access level for a specific SARC appeal request.
        /// Executes stored procedure: lod_sp_UserHasAccess with ModuleType.SARCAppeal
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="refId">The reference identifier of the SARC appeal request.</param>
        /// <returns>The user's <see cref="PageAccess.AccessLevel"/> (None, ReadOnly, or ReadWrite).</returns>
        public PageAccess.AccessLevel GetUserAccess(int userId, int refId)
        {
            SqlDataStore store = new SqlDataStore();
            int access = (int)store.ExecuteScalar("lod_sp_UserHasAccess", userId, refId, ModuleType.SARCAppeal);

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

        /// <summary>
        /// Searches for SARC appeal requests using search criteria.
        /// Executes stored procedure: form348_AP_SARC_sp_Search
        /// </summary>
        /// <param name="caseID">The case identifier to search for.</param>
        /// <param name="ssn">The social security number to search for.</param>
        /// <param name="name">The name to search for.</param>
        /// <param name="status">The status filter.</param>
        /// <param name="userId">The user identifier performing the search.</param>
        /// <param name="rptView">The report view type.</param>
        /// <param name="compo">The component filter.</param>
        /// <param name="maxCount">The maximum number of results to return.</param>
        /// <param name="moduleId">The module identifier.</param>
        /// <param name="unitId">The unit identifier filter.</param>
        /// <returns>A <see cref="DataSet"/> containing matching SARC appeal request records.</returns>
        public DataSet SARCAppealRequestSearch(string caseID, string ssn, string name, int status, int userId, byte rptView, string compo, int maxCount, byte moduleId, int unitId)
        {
            SqlDataStore store = new SqlDataStore();
            return store.ExecuteDataSet("form348_AP_SARC_sp_Search", caseID, ssn, name, status, userId, rptView, compo, maxCount, moduleId, unitId, null);
        }
    }
}