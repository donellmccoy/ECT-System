using ALOD.Core.Domain.Modules.Appeals;
using ALOD.Core.Domain.Workflow;
using ALOD.Core.Interfaces.DAOInterfaces;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Data;

namespace ALOD.Data
{
    internal class LODAppealDao : AbstractNHibernateDao<LODAppeal, int>, ILODAppealDAO
    {
        /// <summary>
        /// Searches for appeal requests using basic search criteria (combined name).
        /// Executes stored procedure: form348_AP_sp_Search
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
        /// <returns>A <see cref="DataSet"/> containing matching appeal request records.</returns>
        public DataSet AppealRequestSearch(string caseID, string ssn, string name, int status, int userId, byte rptView, string compo, int maxCount, byte moduleId, int unitId)
        {
            SqlDataStore store = new SqlDataStore();
            return store.ExecuteDataSet("form348_AP_sp_Search", caseID, ssn, name, status, userId, rptView, compo, maxCount, moduleId, unitId, null);
        }

        /// <summary>
        /// Searches for appeal requests using detailed search criteria (separate name parts).
        /// Executes stored procedure: form348_AP_sp_FullSearch
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
        /// <returns>A <see cref="DataSet"/> containing matching appeal request records.</returns>
        public DataSet AppealRequestSearch(string caseID, string ssn, string lastName, string firstName, string middleName, int status, int userId, byte rptView, string compo, int maxCount, byte moduleId, int unitId)
        {
            SqlDataStore store = new SqlDataStore();
            return store.ExecuteDataSet("form348_AP_sp_FullSearch", caseID, ssn, lastName, firstName, middleName, status, userId, rptView, compo, maxCount, moduleId, unitId, null);
        }

        /// <summary>
        /// Gets the count of appeal requests associated with an initial LOD.
        /// Executes stored procedure: core_lod_sp_GetAppealCount
        /// </summary>
        /// <param name="initialLODId">The initial LOD identifier.</param>
        /// <returns>The count of associated appeal requests.</returns>
        public int GetAppealCount(int initialLODId)
        {
            SqlDataStore store = new SqlDataStore();
            return (int)store.ExecuteScalar("core_lod_sp_GetAppealCount", initialLODId);
        }

        /// <summary>
        /// Retrieves the appeal request identifier associated with an initial LOD.
        /// Executes stored procedure: core_lod_sp_GetAppealId with IdType=1
        /// </summary>
        /// <param name="LodId">The initial LOD identifier.</param>
        /// <returns>The appeal request identifier.</returns>
        public int GetAppealIdByInitLod(int LodId)
        {
            int IdType = 1;
            SqlDataStore store = new SqlDataStore();
            return (int)store.ExecuteScalar("core_lod_sp_GetAppealId", LodId, IdType);
        }

        /// <summary>
        /// Retrieves all appeal requests for the specified role.
        /// Executes stored procedure: core_lod_sp_GetAppealRequests
        /// </summary>
        /// <param name="roleId">The role identifier.</param>
        /// <param name="sarc">True to include SARC cases; otherwise, false.</param>
        /// <returns>A <see cref="DataSet"/> containing appeal request records.</returns>
        public DataSet GetAppealRequests(int roleId, bool sarc)
        {
            SqlDataStore store = new SqlDataStore();
            return store.ExecuteDataSet("core_lod_sp_GetAppealRequests", roleId, sarc);
        }

        /// <summary>
        /// Retrieves a LOD appeal request by its unique case identifier.
        /// </summary>
        /// <param name="caseId">The unique case identifier.</param>
        /// <returns>The <see cref="LODAppeal"/> entity if found; otherwise, null.</returns>
        public LODAppeal GetByCaseId(string caseId)
        {
            ISession session = NHibernateSessionManager.Instance.GetSession();

            IList<LODAppeal> list = session.CreateCriteria(typeof(LODAppeal))
                .Add(Expression.Eq("CaseId", caseId))
                .List<LODAppeal>();

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
        /// Retrieves all completed appeal requests (APs) for the specified role.
        /// Executes stored procedure: core_lod_sp_GetCompletedAPs
        /// </summary>
        /// <param name="roleId">The role identifier.</param>
        /// <param name="sarc">True to include SARC cases; otherwise, false.</param>
        /// <returns>A <see cref="DataSet"/> containing completed appeal request records.</returns>
        public DataSet GetCompletedAPs(int roleId, bool sarc)
        {
            SqlDataStore store = new SqlDataStore();
            return store.ExecuteDataSet("core_lod_sp_GetCompletedAPs", roleId, sarc);
        }

        /// <summary>
        /// Searches for post-appeal completion records.
        /// Executes stored procedure: form348_sp_PostAppealCompletionSearch
        /// </summary>
        /// <param name="caseID">The case identifier to search for.</param>
        /// <param name="ssn">The social security number to search for.</param>
        /// <param name="name">The name to search for.</param>
        /// <param name="userId">The user identifier performing the search.</param>
        /// <param name="rptView">The report view type.</param>
        /// <param name="compo">The component filter.</param>
        /// <param name="maxCount">The maximum number of results to return.</param>
        /// <param name="moduleId">The module identifier.</param>
        /// <param name="IsFomal">The formal/informal indicator (converted to boolean or null).</param>
        /// <param name="unitId">The unit identifier filter.</param>
        /// <param name="sarcpermission">True if the user has SARC permissions; otherwise, false.</param>
        /// <returns>A <see cref="DataSet"/> containing post-appeal completion records.</returns>
        public DataSet GetPostAppealCompletion(string caseID, string ssn, string name,
                                         int userId, byte rptView, string compo, int maxCount, byte moduleId,
                                         string IsFomal, int unitId, bool sarcpermission)
        {
            if (IsFomal == "")
            {
                IsFomal = null;
            }
            else
            {
                Convert.ToBoolean(IsFomal);
            }

            SqlDataStore source = new SqlDataStore();
            return source.ExecuteDataSet("form348_sp_PostAppealCompletionSearch", caseID, ssn, name, userId, rptView,
                  compo, maxCount, moduleId, IsFomal, unitId, sarcpermission);
        }

        /// <summary>
        /// Determines the user's access level for a specific appeal request.
        /// Executes stored procedure: lod_sp_UserHasAccess with ModuleType.AppealRequest
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="refId">The reference identifier of the appeal request.</param>
        /// <returns>The user's <see cref="PageAccess.AccessLevel"/> (None, ReadOnly, or ReadWrite).</returns>
        public PageAccess.AccessLevel GetUserAccess(int userId, int refId)
        {
            SqlDataStore store = new SqlDataStore();
            int access = (int)store.ExecuteScalar("lod_sp_UserHasAccess", userId, refId, ModuleType.AppealRequest);

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