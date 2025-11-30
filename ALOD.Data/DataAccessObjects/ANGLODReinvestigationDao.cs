using ALOD.Core.Domain.Modules.Reinvestigations;
using ALOD.Core.Domain.Workflow;
using ALOD.Core.Interfaces.DAOInterfaces;
using NHibernate;
using NHibernate.Criterion;
using System.Collections.Generic;
using System.Data;

namespace ALOD.Data
{
    /// <summary>
    /// Data access object for managing <see cref="LODReinvestigation"/> entities in the Air National Guard context.
    /// Handles reinvestigation request operations including searches, access control, and status tracking.
    /// </summary>
    internal class ANGLODReinvestigationDao : AbstractNHibernateDao<LODReinvestigation, int>, ILODReinvestigateDAO
    {
        /// <inheritdoc/>
        public LODReinvestigation GetByCaseId(string caseId)
        {
            ISession session = NHibernateSessionManager.Instance.GetSession();

            IList<LODReinvestigation> list = session.CreateCriteria(typeof(LODReinvestigation))
                .Add(Expression.Eq("CaseId", caseId))
                .List<LODReinvestigation>();

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
        public DataSet GetCompletedRRs(int roleId, bool sarc)
        {
            SqlDataStore store = new SqlDataStore();
            return store.ExecuteDataSet("core_lod_sp_GetCompletedRRs", roleId, sarc);
        }

        /// <inheritdoc/>
        public int GetReinvestigationRequestCount(int roleId, bool sarc)
        {
            SqlDataStore store = new SqlDataStore();
            return (int)store.ExecuteScalar("core_lod_sp_GetReinvestigationsCount", roleId, sarc);
        }

        /// <inheritdoc/>
        public int GetReinvestigationRequestIdByInitLod(int LodId)
        {
            int IdType = 1;
            SqlDataStore store = new SqlDataStore();
            return (int)store.ExecuteScalar("core_lod_sp_GetReinvestigationRequestId", LodId, IdType);
        }

        /// <inheritdoc/>
        public int GetReinvestigationRequestIdByRLod(int LodId)
        {
            int IdType = 2;
            SqlDataStore store = new SqlDataStore();
            return (int)store.ExecuteScalar("core_lod_sp_GetReinvestigationRequestId", LodId, IdType);
        }

        /// <inheritdoc/>
        public DataSet GetReinvestigationRequests(int roleId, bool sarc)
        {
            SqlDataStore store = new SqlDataStore();
            return store.ExecuteDataSet("core_lod_sp_GetReinvestigationRequests", roleId, sarc);
        }

        /// <inheritdoc/>
        public int GetReinvestigationRequestsCount(int initialLODId)
        {
            SqlDataStore store = new SqlDataStore();
            return (int)store.ExecuteScalar("core_lod_sp_GetReinvestigationRequestsCount", initialLODId);
        }

        /// <inheritdoc/>
        public PageAccess.AccessLevel GetUserAccess(int userId, int refId)
        {
            SqlDataStore store = new SqlDataStore();
            int access = (int)store.ExecuteScalar("lod_sp_UserHasAccess", userId, refId, ModuleType.ReinvestigationRequest);

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
        public DataSet ReinvestigationRequestSearch(string caseID, string ssn, string name, int status, int userId, byte rptView, string compo, int maxCount, byte moduleId, int unitId)
        {
            SqlDataStore store = new SqlDataStore();
            return store.ExecuteDataSet("form348_RR_sp_Search", caseID, ssn, name, status, userId, rptView, compo, maxCount, moduleId, unitId, null);
        }

        /// <inheritdoc/>
        public DataSet ReinvestigationRequestSearch(string caseID, string ssn, string lastName, string firstName, string middleName, int status, int userId, byte rptView, string compo, int maxCount, byte moduleId, int unitId)
        {
            SqlDataStore store = new SqlDataStore();
            return store.ExecuteDataSet("form348_RR_sp_FullSearch", caseID, ssn, lastName, firstName, middleName, status, userId, rptView, compo, maxCount, moduleId, unitId, null);
        }
    }
}