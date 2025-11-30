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
    /// <summary>
    /// Data access object for managing <see cref="LODAppeal"/> entities in the Air National Guard context.
    /// Handles appeal request operations including searches, access control, and post-completion processing.
    /// </summary>
    internal class ANGLODAppealDao : AbstractNHibernateDao<LODAppeal, int>, ILODAppealDAO
    {
        /// <summary>
        /// Searches for appeal requests using basic criteria.
        /// </summary>
        /// <param name="caseID">The case ID to search for.</param>
        /// <param name="ssn">The member SSN.</param>
        /// <param name="name">The member name.</param>
        /// <param name="status">The status filter.</param>
        /// <param name="userId">The user performing the search.</param>
        /// <param name="rptView">The report view filter.</param>
        /// <param name="compo">The component filter.</param>
        /// <param name="maxCount">Maximum number of results.</param>
        /// <param name="moduleId">The module ID.</param>
        /// <param name="unitId">The unit ID filter.</param>
        /// <returns>A DataSet containing search results.</returns>
        public DataSet AppealRequestSearch(string caseID, string ssn, string name, int status, int userId, byte rptView, string compo, int maxCount, byte moduleId, int unitId)
        {
            SqlDataStore store = new SqlDataStore();
            return store.ExecuteDataSet("form348_AP_sp_Search", caseID, ssn, name, status, userId, rptView, compo, maxCount, moduleId, unitId, null);
        }

        /// <inheritdoc/>
        public DataSet AppealRequestSearch(string caseID, string ssn, string lastName, string firstName, string middleName, int status, int userId, byte rptView, string compo, int maxCount, byte moduleId, int unitId)
        {
            SqlDataStore store = new SqlDataStore();
            return store.ExecuteDataSet("form348_AP_sp_FullSearch", caseID, ssn, lastName, firstName, middleName, status, userId, rptView, compo, maxCount, moduleId, unitId, null);
        }

        /// <inheritdoc/>
        public int GetAppealCount(int initialLODId)
        {
            SqlDataStore store = new SqlDataStore();
            return (int)store.ExecuteScalar("core_lod_sp_GetAppealCount", initialLODId);
        }

        /// <inheritdoc/>
        public int GetAppealIdByInitLod(int LodId)
        {
            int IdType = 1;
            SqlDataStore store = new SqlDataStore();
            return (int)store.ExecuteScalar("core_lod_sp_GetAppealId", LodId, IdType);
        }

        /// <inheritdoc/>
        public DataSet GetAppealRequests(int roleId, bool sarc)
        {
            SqlDataStore store = new SqlDataStore();
            return store.ExecuteDataSet("core_lod_sp_GetAppealRequests", roleId, sarc);
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public DataSet GetCompletedAPs(int roleId, bool sarc)
        {
            SqlDataStore store = new SqlDataStore();
            return store.ExecuteDataSet("core_lod_sp_GetCompletedAPs", roleId, sarc);
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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