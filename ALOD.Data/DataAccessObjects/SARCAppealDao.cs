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
    /// Data access object for <see cref="SARCAppeal"/> entities.
    /// Provides operations for managing SARC appeal cases including search, counts, and user access.
    /// </summary>
    internal class SARCAppealDao : AbstractNHibernateDao<SARCAppeal, int>, ISARCAppealDAO
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
        public int GetAppealCount(int initialId, int workflowId)
        {
            SqlDataStore store = new SqlDataStore();
            return (int)store.ExecuteScalar("core_sarc_sp_GetAppealCount", initialId, workflowId);
        }

        /// <inheritdoc/>
        public int GetAppealIdByInitId(int LodId, int workflowId)
        {
            SqlDataStore store = new SqlDataStore();
            return (int)store.ExecuteScalar("core_sarc_sp_GetAppealId", LodId, workflowId);
        }

        /// <inheritdoc/>
        public DataSet GetAppealRequests(int roleId, bool sarc)
        {
            SqlDataStore store = new SqlDataStore();
            return store.ExecuteDataSet("core_sarc_sp_GetAppealRequests", roleId, sarc);
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public DataSet GetCompletedAPs(int roleId, bool sarc)
        {
            SqlDataStore store = new SqlDataStore();
            return store.ExecuteDataSet("core_sarc_sp_GetCompletedAPs", roleId, sarc);
        }

        /// <inheritdoc/>
        public int GetPostCompletionCaseCount(int userId)
        {
            Object result = DataStore.ExecuteScalar("core_sarc_sp_GetPostCompletionAppealCasesCount", userId);

            if (result == null)
                return -1;
            else
                return (int)result;
        }

        /// <inheritdoc/>
        public DataSet GetPostSARCAppealCompletion(string caseID, string ssn, string name,
                                         int userId, byte rptView, string compo, int maxCount, byte moduleId,
                                         int unitId, bool sarcpermission)
        {
            SqlDataStore source = new SqlDataStore();
            return source.ExecuteDataSet("core_sarc_sp_PostAppealCompletionSearch", caseID, ssn, name, userId, rptView,
                  compo, maxCount, moduleId, unitId, sarcpermission);
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public DataSet SARCAppealRequestSearch(string caseID, string ssn, string name, int status, int userId, byte rptView, string compo, int maxCount, byte moduleId, int unitId)
        {
            SqlDataStore store = new SqlDataStore();
            return store.ExecuteDataSet("form348_AP_SARC_sp_Search", caseID, ssn, name, status, userId, rptView, compo, maxCount, moduleId, unitId, null);
        }
    }
}