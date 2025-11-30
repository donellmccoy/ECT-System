using ALOD.Core.Domain.Modules.SpecialCases;
using ALOD.Core.Domain.Users;
using ALOD.Core.Domain.Workflow;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Core.Utils;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace ALOD.Data
{
    /// <summary>
    /// Data access object for managing special case entities including INCAP, reassessment, PEPP types, and RWOA special cases.
    /// Provides comprehensive CRUD operations and specialized query methods for various special case workflows.
    /// </summary>
    internal class SpecialCaseDao : AbstractNHibernateDao<SpecialCase, int>, ISpecialCaseDAO
    {
        private SqlDataStore _store;

        /// <summary>
        /// Gets the data store instance for database operations.
        /// Lazily initializes the SqlDataStore if not already created.
        /// </summary>
        public SqlDataStore DataStore
        {
            get
            {
                if (_store == null)
                    _store = new SqlDataStore();

                return _store;
            }
        }

        /// <inheritdoc/>
        public int CreateINCAPAppealFindings(int scId)
        {
            Object result = DataStore.ExecuteDataSet("Form348_sp_InitiateIncapAppeal", scId);

            if (result == null)
                return 0;

            return 0;
        }

        /// <inheritdoc/>
        public int CreateINCAPExtFindings(int scId)
        {
            DataRow dr = DataStore.ExecuteDataSet("Form348_sp_InitiateIncapExt", scId).Tables[0].Rows[0];
            int result = int.Parse(dr.ItemArray[0].ToString());
            //int extNum = result.Rows.col
            if (result == 0)
                return 0;

            return result;
        }

        /// <inheritdoc/>
        public int CreateINCAPFindings(int scId)
        {
            Object result = DataStore.ExecuteDataSet("Form348_sp_InitiateIncapFindings", scId);

            if (result == null)
                return 0;

            return 0;
        }

        /// <inheritdoc/>
        public int CreateReassessRSCase(int userId, int originalRefId, string newCaseId, int workflowId)
        {
            Object result = DataStore.ExecuteScalar("form348_SC_sp_ReassessRS", userId, originalRefId, newCaseId, workflowId);

            if (result == null)
                return 0;

            return (int)result;
        }

        /// <inheritdoc/>
        public DataSet GetAvailabilityCode()
        {
            return DataStore.ExecuteDataSet("core_lookups_sp_GetAvailabilityCode");
        }

        /// <inheritdoc/>
        public DataSet GetCompletedSCs(int roleId)
        {
            return DataStore.ExecuteDataSet("core_lod_sp_GetCompletedSCs", roleId);
        }

        /// <inheritdoc/>
        public DataSet GetCompletedSpecialCasesByMemberSSN(string ssn, int userId)
        {
            return DataStore.ExecuteDataSet("core_lod_sp_GetCompletedSpecialCasesByMemberSSN", ssn, userId);
        }

        /// <inheritdoc/>
        public bool GetHasReassessmentCase(int refId)
        {
            Object result = DataStore.ExecuteScalar("Form348_SC_sp_GetHasReassessmentSC", refId);

            if (result == null)
                return false;

            int iResult = (int)result;

            if (iResult == 0)
                return false;

            return true;
        }

        /// <inheritdoc/>
        public DataSet GetIRILOStatus()
        {
            return DataStore.ExecuteDataSet("core_lookups_sp_GetIRILOStatus");
        }

        /// <inheritdoc/>
        public bool GetIsReassessmentCase(int refId)
        {
            Object result = DataStore.ExecuteScalar("Form348_SC_sp_GetIsReassessmentSC", refId);

            if (result == null)
                return false;

            int iResult = (int)result;

            if (iResult == 0)
                return false;

            return true;
        }

        /// <inheritdoc/>
        public DataSet GetMedDisposition()
        {
            return DataStore.ExecuteDataSet("core_lookups_sp_GetAMRODisposition");
        }

        /// <inheritdoc/>
        public DataSet GetMemberSpecialCaseHistory(string memberSSN, int userId)
        {
            return DataStore.ExecuteDataSet("form348_SC_sp_GetMemberSpecialCaseHistory", memberSSN, userId);
        }

        /// <inheritdoc/>
        public DataSet GetPSCDAssociableIRSpecialCases(string memberSSN)
        {
            return DataStore.ExecuteDataSet("core_sp_GetIRILOAssociableSpecialCases", memberSSN);
        }

        /// <inheritdoc/>
        public DataSet GetPSCDAssociableRWSpecialCases(string memberSSN)
        {
            return DataStore.ExecuteDataSet("core_sp_GetRWAssociableSpecialCasesForPSCD", memberSSN);
        }

        /// <inheritdoc/>
        public SpecialCase GetReassessmentByOriginalId(int originalRefId)
        {
            Object result = DataStore.ExecuteScalar("Form348_SC_sp_GetReassessmentIdByOriginalId", originalRefId);
            int iResult;

            if (result == null)
                return null;

            iResult = (int)result;

            if (iResult <= 0)
                return null;

            return GetById(iResult);
        }

        /// <inheritdoc/>
        public int GetReassessmentCount(int originalRefId)
        {
            Object result = DataStore.ExecuteScalar("Form348_SC_sp_GetReassessmentCount", originalRefId);

            if (result == null)
                return 0;

            return (int)result;
        }

        /// <inheritdoc/>
        public DataSet GetRWAssociableSpecialCases(string memberSSN)
        {
            return DataStore.ExecuteDataSet("core_sc_sp_GetRWAssociableSpecialCases", memberSSN);
        }

        /// <inheritdoc/>
        public DataSet GetSpecailCaseDisposition(int moduleId, int userId)
        {
            return DataStore.ExecuteDataSet("core_lod_sp_GetSpecialCaseDisposition", moduleId, userId);
        }

        /// <inheritdoc/>
        public DataSet GetSpecailCaseNoDisposition(int moduleId, int userId)
        {
            return DataStore.ExecuteDataSet("core_lod_sp_GetSpecialCaseNoDisposition", moduleId, userId);
        }

        /// <inheritdoc/>
        public DataSet GetSpecialAwaitingConsult(int wsId)
        {
            return DataStore.ExecuteDataSet("form348_sp_InConsultCaseSearch", wsId);
        }

        /// <summary>
        /// Gets the count of special cases awaiting consultation for a specific workflow status.
        /// </summary>
        /// <param name="wsId">The workflow status ID.</param>
        /// <returns>The count of special cases awaiting consultation.</returns>
        public int GetSpecialAwaitingConsultCount(int wsId)
        {
            return (int)DataStore.ExecuteScalar("form348_sp_InConsultCaseSearch", wsId);
        }

        /// <inheritdoc/>
        public SpecialCase GetSpecialCaseByCaseId(string caseId)
        {
            ISession session = NHibernateSessionManager.Instance.GetSession();

            IList<SpecialCase> list = session.CreateCriteria(typeof(SpecialCase))
                .Add(Expression.Eq("CaseId", caseId))
                .List<SpecialCase>();

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
        public DataSet GetSpecialCaseById(int scId)
        {
            return DataStore.ExecuteDataSet("core_lod_sp_GetSpecialCaseById", scId);
        }

        /// <summary>
        /// Retrieves a special case by reference ID.
        /// </summary>
        /// <param name="refId">The reference ID of the special case.</param>
        /// <returns>The SpecialCase object, or null if not found.</returns>
        public SpecialCase GetSpecialCaseByRefId(int refId)
        {
            SpecialCase specialCase = GetById(refId);
            return specialCase;
        }

        /// <summary>
        /// Gets the count of special cases not in holding status for a unit, group, and module.
        /// </summary>
        /// <param name="moduleId">The ID of the module.</param>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>The count of special cases not in holding status.</returns>
        public int GetSpecialCaseNotHoldingCount(int moduleId, int userId)
        {
            return (int)DataStore.ExecuteScalar("core_lod_sp_GetSpecialCaseNotHoldingCount", moduleId, userId);
        }

        /// <inheritdoc/>
        public IList<LookUpItem> GetSpecialCasePEPPTypes(int refId)
        {
            System.Data.DataSet ds = DataStore.ExecuteDataSet("Form348_SC_sp_GetPEPPTypes", refId);

            if (ds == null || ds.Tables.Count == 0)
                return null;

            List<LookUpItem> items = new List<LookUpItem>();

            foreach (System.Data.DataRow row in ds.Tables[0].Rows)
            {
                LookUpItem item = new LookUpItem();

                item.Value = (int)row["Id"];
                item.Name = row["Name"].ToString();

                items.Add(item);
            }

            return items;
        }

        /// <inheritdoc/>
        public DataSet GetSpecialCases(int roleId)
        {
            return DataStore.ExecuteDataSet("core_lod_sp_GetSpecialCases", roleId);
        }

        /// <inheritdoc/>
        public DataSet GetSpecialCasesByMemberSSN(string ssn, int userId)
        {
            return DataStore.ExecuteDataSet("core_lod_sp_GetSpecialCasesByMemberSSN", ssn, userId);
        }

        /// <inheritdoc/>
        public DataSet GetSpecialCasesByModule(int moduleId, int roleId)
        {
            return DataStore.ExecuteDataSet("core_lod_sp_GetSpecialCasesByModuleId", roleId, moduleId);
        }

        /// <inheritdoc/>
        public int GetSpecialCasesByModuleCount(int moduleId, int roleId)
        {
            return (int)DataStore.ExecuteScalar("core_lod_sp_GetSpecialCasesByModuleCount", roleId, moduleId);
        }

        /// <inheritdoc/>
        public int GetSpecialCasesCount(int roleId)
        {
            return (int)DataStore.ExecuteScalar("core_lod_sp_GetSpecialCasesCount", roleId);
        }

        /// <summary>
        /// Gets the count of special cases in holding status for a module and user.
        /// </summary>
        /// <param name="moduleId">The ID of the module.</param>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>The count of special cases in holding status.</returns>
        public int GetSpecialCaseHoldingCount(int moduleId, int userId)
        {
            int count = (int)DataStore.ExecuteScalar("core_lod_sp_GetSpecialCaseHoldingCount", moduleId, userId);
            return count;
        }

        /// <summary>
        /// Gets special cases in holding status for a unit, group, and module.
        /// </summary>
        /// <param name="moduleId">The ID of the module.</param>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A DataSet containing special cases in holding status.</returns>
        public DataSet GetSpecialCasesHolding(int moduleId, int userId)
        {
            return DataStore.ExecuteDataSet("core_lod_sp_GetSpecialCaseHolding", moduleId, userId);
        }

        /// <inheritdoc/>
        public DataSet GetSpecialCasesNotHolding(int moduleId, int userId)
        {
            if (moduleId == 30)
            {
                return DataStore.ExecuteDataSet("core_lod_sp_GetPSCDSpecialCaseNotHolding", moduleId, userId);
            }
            else
            {
                return DataStore.ExecuteDataSet("core_lod_sp_GetSpecialCaseNotHolding", moduleId, userId);
            }
        }

        /// <inheritdoc/>
        public DataSet GetSubClassProperties(int moduleId, int scId)
        {
            return DataStore.ExecuteDataSet("form348_sc_sp_properties", moduleId, scId);
        }

        /// <inheritdoc/>
        public PageAccess.AccessLevel GetUserAccess(int userId, int refId, int moduleId)
        {
            int access = (int)DataStore.ExecuteScalar("lod_sp_UserHasAccess", userId, refId, moduleId);

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
        public bool hasRWOA(int workflow, int refId)
        {
            Object result = DataStore.ExecuteScalar("core_lookUps_sp_SpecCaseRWOA", workflow, refId);

            if (result == null)
                return false;

            int iResult = (int)result;

            if (iResult == 0)
                return false;

            return true;
        }

        /// <inheritdoc/>
        public DataSet SpecialCaseSearch(string caseId, string memberSSN, string memberName, int statusId, int userId, int compoId, int unitId, int rptViewId, int module, int maxCount, bool sarcpermission)
        {
            return DataStore.ExecuteDataSet("form348_sc_sp_search", caseId, memberSSN, memberName, statusId, userId, rptViewId, compoId, maxCount, module, unitId, sarcpermission, null);
        }

        /// <inheritdoc/>
        public DataSet SpecialCaseSearch(string caseId, string memberSSN, string lastName, string firstName, string middleName, int statusId, int userId, int compoId, int unitId, int rptViewId, int module, int maxCount, bool sarcpermission)
        {
            return DataStore.ExecuteDataSet("form348_SC_sp_FullSearch", caseId, memberSSN, lastName, firstName, middleName, statusId, userId, rptViewId, compoId, maxCount, module, unitId, sarcpermission, null);
        }

        /// <inheritdoc/>
        public void UpdateSpecialCasePEPPTypes(int refId, IList<int> types)
        {
            // Had to circumvent the SqlDataStore in order to get table valued parameters to work...
            using (var con = DataStore.GetSqlConnection())
            {
                con.Open();

                string execCmd = String.Empty;

                if (types.Count > 0)
                {
                    execCmd = "EXEC Form348_SC_sp_UpdatePEPPTypes @refId, @peppTypeList";
                }
                else
                {
                    execCmd = "EXEC Form348_SC_sp_UpdatePEPPTypes @refId";
                }

                using (SqlCommand cmd = new SqlCommand(execCmd, con))
                {
                    SqlParameter sqlParam = new SqlParameter("@refId", refId);
                    sqlParam.DbType = DbType.Int32;
                    cmd.Parameters.Add(sqlParam);

                    if (types.Count > 0)
                    {
                        sqlParam = new SqlParameter("@peppTypeList", SqlDbType.Structured);
                        sqlParam.TypeName = "dbo.tblIntegerList";
                        sqlParam.Value = CollectionHelpers.IntListToListOfSQLDataRecords(types);
                        cmd.Parameters.Add(sqlParam);
                    }

                    cmd.ExecuteNonQuery();
                }

                con.Close();
            }
        }
    }
}