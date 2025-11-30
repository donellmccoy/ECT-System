using ALOD.Core.Domain.Modules.Reinvestigations;
using ALOD.Core.Domain.Workflow;
using ALOD.Core.Interfaces.DAOInterfaces;
using NHibernate;
using NHibernate.Criterion;
using System.Collections.Generic;
using System.Data;

namespace ALOD.Data
{
    internal class LODReinvestigationDao : AbstractNHibernateDao<LODReinvestigation, int>, ILODReinvestigateDAO
    {
        /// <summary>
        /// Retrieves a LOD reinvestigation request by its unique case identifier.
        /// </summary>
        /// <param name="caseId">The unique case identifier.</param>
        /// <returns>The <see cref="LODReinvestigation"/> entity if found; otherwise, null.</returns>
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

        /// <summary>
        /// Retrieves all completed reinvestigation requests (RRs) for the specified role.
        /// Executes stored procedure: core_lod_sp_GetCompletedRRs
        /// </summary>
        /// <param name="roleId">The role identifier.</param>
        /// <param name="sarc">True to include SARC cases; otherwise, false.</param>
        /// <returns>A <see cref="DataSet"/> containing completed reinvestigation request records.</returns>
        public DataSet GetCompletedRRs(int roleId, bool sarc)
        {
            SqlDataStore store = new SqlDataStore();
            return store.ExecuteDataSet("core_lod_sp_GetCompletedRRs", roleId, sarc);
        }

        /// <summary>
        /// Gets the total count of reinvestigation requests for the specified role.
        /// Executes stored procedure: core_lod_sp_GetReinvestigationsCount
        /// </summary>
        /// <param name="roleId">The role identifier.</param>
        /// <param name="sarc">True to include SARC cases; otherwise, false.</param>
        /// <returns>The count of reinvestigation requests.</returns>
        public int GetReinvestigationRequestCount(int roleId, bool sarc)
        {
            SqlDataStore store = new SqlDataStore();
            return (int)store.ExecuteScalar("core_lod_sp_GetReinvestigationsCount", roleId, sarc);
        }

        /// <summary>
        /// Retrieves the reinvestigation request identifier associated with an initial LOD.
        /// Executes stored procedure: core_lod_sp_GetReinvestigationRequestId with IdType=1
        /// </summary>
        /// <param name="LodId">The initial LOD identifier.</param>
        /// <returns>The reinvestigation request identifier.</returns>
        public int GetReinvestigationRequestIdByInitLod(int LodId)
        {
            int IdType = 1;
            SqlDataStore store = new SqlDataStore();
            return (int)store.ExecuteScalar("core_lod_sp_GetReinvestigationRequestId", LodId, IdType);
        }

        /// <summary>
        /// Retrieves the reinvestigation request identifier associated with a reinvestigation LOD.
        /// Executes stored procedure: core_lod_sp_GetReinvestigationRequestId with IdType=2
        /// </summary>
        /// <param name="LodId">The reinvestigation LOD identifier.</param>
        /// <returns>The reinvestigation request identifier.</returns>
        public int GetReinvestigationRequestIdByRLod(int LodId)
        {
            int IdType = 2;
            SqlDataStore store = new SqlDataStore();
            return (int)store.ExecuteScalar("core_lod_sp_GetReinvestigationRequestId", LodId, IdType);
        }

        /// <summary>
        /// Retrieves all reinvestigation requests for the specified role.
        /// Executes stored procedure: core_lod_sp_GetReinvestigationRequests
        /// </summary>
        /// <param name="roleId">The role identifier.</param>
        /// <param name="sarc">True to include SARC cases; otherwise, false.</param>
        /// <returns>A <see cref="DataSet"/> containing reinvestigation request records.</returns>
        public DataSet GetReinvestigationRequests(int roleId, bool sarc)
        {
            SqlDataStore store = new SqlDataStore();
            return store.ExecuteDataSet("core_lod_sp_GetReinvestigationRequests", roleId, sarc);
        }

        /// <summary>
        /// Gets the count of reinvestigation requests associated with an initial LOD.
        /// Executes stored procedure: core_lod_sp_GetReinvestigationRequestsCount
        /// </summary>
        /// <param name="initialLODId">The initial LOD identifier.</param>
        /// <returns>The count of associated reinvestigation requests.</returns>
        public int GetReinvestigationRequestsCount(int initialLODId)
        {
            SqlDataStore store = new SqlDataStore();
            return (int)store.ExecuteScalar("core_lod_sp_GetReinvestigationRequestsCount", initialLODId);
        }

        /// <summary>
        /// Determines the user's access level for a specific reinvestigation request.
        /// Executes stored procedure: lod_sp_UserHasAccess with ModuleType.ReinvestigationRequest
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="refId">The reference identifier of the reinvestigation request.</param>
        /// <returns>The user's <see cref="PageAccess.AccessLevel"/> (None, ReadOnly, or ReadWrite).</returns>
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

        /// <summary>
        /// Searches for reinvestigation requests using basic search criteria (combined name).
        /// Executes stored procedure: form348_RR_sp_Search
        /// </summary>
        /// <param name="caseID">The case identifier to search for.</param>
        /// <param name="ssn">The social security number to search for.</param>
        /// <param name="name">The combined name to search for.</param>
        /// <param name="status">The status filter.</param>
        /// <param name="userId">The user identifier performing the search.</param>
        /// <param name="rptView">The report view type.</param>
        /// <param name="compo">The component filter.</param>
        /// <param name="maxCount">The maximum number of results to return.</param>
        /// <param name="moduleId">The module identifier.</param>
        /// <param name="unitId">The unit identifier filter.</param>
        /// <returns>A <see cref="DataSet"/> containing matching reinvestigation request records.</returns>
        public DataSet ReinvestigationRequestSearch(string caseID, string ssn, string name, int status, int userId, byte rptView, string compo, int maxCount, byte moduleId, int unitId)
        {
            SqlDataStore store = new SqlDataStore();
            return store.ExecuteDataSet("form348_RR_sp_Search", caseID, ssn, name, status, userId, rptView, compo, maxCount, moduleId, unitId, null);
        }

        /// <summary>
        /// Searches for reinvestigation requests using detailed search criteria (separate name parts).
        /// Executes stored procedure: form348_RR_sp_FullSearch
        /// </summary>
        /// <param name="caseID">The case identifier to search for.</param>
        /// <param name="ssn">The social security number to search for.</param>
        /// <param name="lastName">The last name to search for.</param>
        /// <param name="firstName">The first name to search for.</param>
        /// <param name="middleName">The middle name to search for.</param>
        /// <param name="status">The status filter.</param>
        /// <param name="userId">The user identifier performing the search.</param>
        /// <param name="rptView">The report view type.</param>
        /// <param name="compo">The component filter.</param>
        /// <param name="maxCount">The maximum number of results to return.</param>
        /// <param name="moduleId">The module identifier.</param>
        /// <param name="unitId">The unit identifier filter.</param>
        /// <returns>A <see cref="DataSet"/> containing matching reinvestigation request records.</returns>
        public DataSet ReinvestigationRequestSearch(string caseID, string ssn, string lastName, string firstName, string middleName, int status, int userId, byte rptView, string compo, int maxCount, byte moduleId, int unitId)
        {
            SqlDataStore store = new SqlDataStore();
            return store.ExecuteDataSet("form348_RR_sp_FullSearch", caseID, ssn, lastName, firstName, middleName, status, userId, rptView, compo, maxCount, moduleId, unitId, null);
        }
    }
}