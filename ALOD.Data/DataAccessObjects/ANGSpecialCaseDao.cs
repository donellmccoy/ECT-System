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
    /// Data access object for managing <see cref="SpecialCase"/> entities in the Air National Guard context.
    /// Handles special case operations including INCAP, reassessment cases, searches, and various special case types (BCMR, BMT, Congress, etc.).
    /// </summary>
    internal class ANGSpecialCaseDao : AbstractNHibernateDao<SpecialCase, int>, ISpecialCaseDAO
    {
        private SqlDataStore _store;

        /// <summary>
        /// Gets the SQL data store instance for database operations.
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

        /// <summary>
        /// Creates INCAP (Incapacitation) appeal findings for a special case.
        /// Executes stored procedure: Form348_sp_InitiateIncapAppeal
        /// </summary>
        /// <param name="scId">The special case identifier.</param>
        /// <returns>The identifier of the created INCAP appeal findings, or 0 if creation failed.</returns>
        public int CreateINCAPAppealFindings(int scId)
        {
            Object result = DataStore.ExecuteDataSet("Form348_sp_InitiateIncapAppeal", scId);

            if (result == null)
                return 0;

            return (int)result;
        }

        /// <summary>
        /// Creates INCAP extension findings for a special case.
        /// Executes stored procedure: Form348_sp_InitiateIncapExt
        /// </summary>
        /// <param name="scId">The special case identifier.</param>
        /// <returns>The identifier of the created INCAP extension findings, or 0 if creation failed.</returns>
        public int CreateINCAPExtFindings(int scId)
        {
            Object result = DataStore.ExecuteDataSet("Form348_sp_InitiateIncapExt", scId);

            if (result == null)
                return 0;

            return (int)result;
        }

        /// <summary>
        /// Creates initial INCAP findings for a special case.
        /// Executes stored procedure: Form348_sp_InitiateIncapFindings
        /// </summary>
        /// <param name="scId">The special case identifier.</param>
        /// <returns>The identifier of the created INCAP findings, or 0 if creation failed.</returns>
        public int CreateINCAPFindings(int scId)
        {
            Object result = DataStore.ExecuteDataSet("Form348_sp_InitiateIncapFindings", scId);

            if (result == null)
                return 0;

            return (int)result;
        }

        /// <summary>
        /// Creates a reassessment case for a Restricted SARC (RS) case.
        /// Executes stored procedure: form348_SC_sp_ReassessRS
        /// </summary>
        /// <param name="userId">The user identifier creating the reassessment.</param>
        /// <param name="originalRefId">The original case reference identifier.</param>
        /// <param name="newCaseId">The new case identifier for the reassessment.</param>
        /// <param name="workflowId">The workflow identifier.</param>
        /// <returns>The identifier of the created reassessment case, or 0 if creation failed.</returns>
        public int CreateReassessRSCase(int userId, int originalRefId, string newCaseId, int workflowId)
        {
            Object result = DataStore.ExecuteScalar("form348_SC_sp_ReassessRS", userId, originalRefId, newCaseId, workflowId);

            if (result == null)
                return 0;

            return (int)result;
        }

        /// <summary>
        /// Retrieves all availability codes.
        /// Executes stored procedure: core_lookups_sp_GetAvailabilityCode
        /// </summary>
        /// <returns>A <see cref="DataSet"/> containing availability code lookup records.</returns>
        public DataSet GetAvailabilityCode()
        {
            return DataStore.ExecuteDataSet("core_lookups_sp_GetAvailabilityCode");
        }

        /// <summary>
        /// Retrieves all completed special cases (SCs) for the specified role.
        /// Executes stored procedure: core_lod_sp_GetCompletedSCs
        /// </summary>
        /// <param name="roleId">The role identifier.</param>
        /// <returns>A <see cref="DataSet"/> containing completed special case records.</returns>
        public DataSet GetCompletedSCs(int roleId)
        {
            return DataStore.ExecuteDataSet("core_lod_sp_GetCompletedSCs", roleId);
        }

        /// <summary>
        /// Retrieves all completed special cases for a specific member by their social security number.
        /// Executes stored procedure: core_lod_sp_GetCompletedSpecialCasesByMemberSSN
        /// </summary>
        /// <param name="ssn">The member's social security number.</param>
        /// <param name="userId">The user identifier performing the query.</param>
        /// <returns>A <see cref="DataSet"/> containing completed special case records for the member.</returns>
        public DataSet GetCompletedSpecialCasesByMemberSSN(string ssn, int userId)
        {
            return DataStore.ExecuteDataSet("core_lod_sp_GetCompletedSpecialCasesByMemberSSN", ssn, userId);
        }

        /// <summary>
        /// Determines if a special case has an associated reassessment case.
        /// Executes stored procedure: Form348_SC_sp_GetHasReassessmentSC
        /// </summary>
        /// <param name="refId">The special case reference identifier.</param>
        /// <returns>True if a reassessment case exists; otherwise, false.</returns>
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

        /// <summary>
        /// Retrieves all IRILO (Incapacitation Review/Incapacitation Line of Duty) status codes.
        /// Executes stored procedure: core_lookups_sp_GetIRILOStatus
        /// </summary>
        /// <returns>A <see cref="DataSet"/> containing IRILO status lookup records.</returns>
        public DataSet GetIRILOStatus()
        {
            return DataStore.ExecuteDataSet("core_lookups_sp_GetIRILOStatus");
        }

        /// <summary>
        /// Determines if a special case is itself a reassessment case.
        /// Executes stored procedure: Form348_SC_sp_GetIsReassessmentSC
        /// </summary>
        /// <param name="refId">The special case reference identifier.</param>
        /// <returns>True if this is a reassessment case; otherwise, false.</returns>
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

        /// <summary>
        /// Retrieves all medical disposition codes (AMRO - Aeromedical Review Office).
        /// Executes stored procedure: core_lookups_sp_GetAMRODisposition
        /// </summary>
        /// <returns>A <see cref="DataSet"/> containing AMRO disposition lookup records.</returns>
        public DataSet GetMedDisposition()
        {
            return DataStore.ExecuteDataSet("core_lookups_sp_GetAMRODisposition");
        }

        /// <summary>
        /// Retrieves the complete special case history for a member by their social security number.
        /// Executes stored procedure: form348_SC_sp_GetMemberSpecialCaseHistory
        /// </summary>
        /// <param name="memberSSN">The member's social security number.</param>
        /// <param name="userId">The user identifier performing the query.</param>
        /// <returns>A <see cref="DataSet"/> containing the member's special case history records.</returns>
        public DataSet GetMemberSpecialCaseHistory(string memberSSN, int userId)
        {
            return DataStore.ExecuteDataSet("form348_SC_sp_GetMemberSpecialCaseHistory", memberSSN, userId);
        }

        /// <summary>
        /// Retrieves IR (Incapacitation Review) special cases that can be associated with a PSCD (Presumed to have been in a Sound Condition of Discipline) case for a specific member.
        /// Executes stored procedure: core_sp_GetIRILOAssociableSpecialCases
        /// </summary>
        /// <param name="memberSSN">The member's social security number.</param>
        /// <returns>A <see cref="DataSet"/> containing associable IR special case records.</returns>
        public DataSet GetPSCDAssociableIRSpecialCases(string memberSSN)
        {
            return DataStore.ExecuteDataSet("core_sp_GetIRILOAssociableSpecialCases", memberSSN);
        }

        /// <summary>
        /// Retrieves RW (Rebuttal to Wrongful) special cases that can be associated with a PSCD case for a specific member.
        /// Executes stored procedure: core_sp_GetRWAssociableSpecialCasesForPSCD
        /// </summary>
        /// <param name="memberSSN">The member's social security number.</param>
        /// <returns>A <see cref="DataSet"/> containing associable RW special case records.</returns>
        public DataSet GetPSCDAssociableRWSpecialCases(string memberSSN)
        {
            return DataStore.ExecuteDataSet("core_sp_GetRWAssociableSpecialCasesForPSCD", memberSSN);
        }

        /// <summary>
        /// Retrieves the reassessment special case associated with an original case.
        /// Executes stored procedure: Form348_SC_sp_GetReassessmentIdByOriginalId
        /// </summary>
        /// <param name="originalRefId">The original case reference identifier.</param>
        /// <returns>The reassessment <see cref="SpecialCase"/> entity if found; otherwise, null.</returns>
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

        /// <summary>
        /// Gets the count of reassessment cases associated with an original case.
        /// Executes stored procedure: Form348_SC_sp_GetReassessmentCount
        /// </summary>
        /// <param name="originalRefId">The original case reference identifier.</param>
        /// <returns>The count of associated reassessment cases, or 0 if none exist.</returns>
        public int GetReassessmentCount(int originalRefId)
        {
            Object result = DataStore.ExecuteScalar("Form348_SC_sp_GetReassessmentCount", originalRefId);

            if (result == null)
                return 0;

            return (int)result;
        }

        /// <summary>
        /// Retrieves RW (Rebuttal to Wrongful) special cases that can be associated with other cases for a specific member.
        /// Executes stored procedure: core_sc_sp_GetRWAssociableSpecialCases
        /// </summary>
        /// <param name="memberSSN">The member's social security number.</param>
        /// <returns>A <see cref="DataSet"/> containing associable RW special case records.</returns>
        public DataSet GetRWAssociableSpecialCases(string memberSSN)
        {
            return DataStore.ExecuteDataSet("core_sc_sp_GetRWAssociableSpecialCases", memberSSN);
        }

        /// <summary>
        /// Retrieves special cases with a disposition for the specified module and user.
        /// Executes stored procedure: core_lod_sp_GetSpecialCaseDisposition
        /// </summary>
        /// <param name="moduleId">The module identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns>A <see cref="DataSet"/> containing special cases with dispositions.</returns>
        public DataSet GetSpecailCaseDisposition(int moduleId, int userId)
        {
            return DataStore.ExecuteDataSet("core_lod_sp_GetSpecialCaseDisposition", moduleId, userId);
        }

        /// <summary>
        /// Retrieves special cases without a disposition for the specified module and user.
        /// Executes stored procedure: core_lod_sp_GetSpecialCaseNoDisposition
        /// </summary>
        /// <param name="moduleId">The module identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns>A <see cref="DataSet"/> containing special cases without dispositions.</returns>
        public DataSet GetSpecailCaseNoDisposition(int moduleId, int userId)
        {
            return DataStore.ExecuteDataSet("core_lod_sp_GetSpecialCaseNoDisposition", moduleId, userId);
        }

        /// <summary>
        /// Gets special cases awaiting consultation for a specific workflow stage.
        /// </summary>
        /// <param name="wsId">The workflow stage identifier.</param>
        /// <returns>A <see cref="DataSet"/> containing special cases awaiting consultation.</returns>
        /// <exception cref="NotImplementedException">This method is not implemented in the Air National Guard context.</exception>
        DataSet ISpecialCaseDAO.GetSpecialAwaitingConsult(int wsId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves a special case by its unique case identifier.
        /// </summary>
        /// <param name="caseId">The unique case identifier.</param>
        /// <returns>The <see cref="SpecialCase"/> entity if found; otherwise, null.</returns>
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

        /// <summary>
        /// Retrieves detailed special case information by identifier.
        /// Executes stored procedure: core_lod_sp_GetSpecialCaseById
        /// </summary>
        /// <param name="scId">The special case identifier.</param>
        /// <returns>A <see cref="DataSet"/> containing detailed special case information.</returns>
        public DataSet GetSpecialCaseById(int scId)
        {
            return DataStore.ExecuteDataSet("core_lod_sp_GetSpecialCaseById", scId);
        }

        /// <summary>
        /// Gets the count of special cases not in holding status for a specific module and user.
        /// Executes stored procedure: core_lod_sp_GetSpecialCaseNotHoldingCount
        /// </summary>
        /// <param name="moduleId">The module identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns>The count of special cases not in holding status.</returns>
        public int GetSpecialCaseNotHoldingCount(int moduleId, int userId)
        {
            return (int)DataStore.ExecuteScalar("core_lod_sp_GetSpecialCaseNotHoldingCount", moduleId, userId);
        }

        /// <summary>
        /// Retrieves the PEPP (Physical Evaluation Processing Program) types associated with a special case.
        /// Executes stored procedure: Form348_SC_sp_GetPEPPTypes
        /// </summary>
        /// <param name="refId">The special case reference identifier.</param>
        /// <returns>A list of <see cref="LookUpItem"/> representing the PEPP types, or null if none exist.</returns>
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

        /// <summary>
        /// Retrieves all special cases for the specified role.
        /// Executes stored procedure: core_lod_sp_GetSpecialCases
        /// </summary>
        /// <param name="roleId">The role identifier.</param>
        /// <returns>A <see cref="DataSet"/> containing special case records.</returns>
        public DataSet GetSpecialCases(int roleId)
        {
            return DataStore.ExecuteDataSet("core_lod_sp_GetSpecialCases", roleId);
        }

        /// <summary>
        /// Retrieves all special cases for a specific member by their social security number.
        /// Executes stored procedure: core_lod_sp_GetSpecialCasesByMemberSSN
        /// </summary>
        /// <param name="ssn">The member's social security number.</param>
        /// <param name="userId">The user identifier performing the query.</param>
        /// <returns>A <see cref="DataSet"/> containing special case records for the member.</returns>
        public DataSet GetSpecialCasesByMemberSSN(string ssn, int userId)
        {
            return DataStore.ExecuteDataSet("core_lod_sp_GetSpecialCasesByMemberSSN", ssn, userId);
        }

        /// <summary>
        /// Retrieves all special cases for a specific module and role.
        /// Executes stored procedure: core_lod_sp_GetSpecialCasesByModuleId
        /// </summary>
        /// <param name="moduleId">The module identifier.</param>
        /// <param name="roleId">The role identifier.</param>
        /// <returns>A <see cref="DataSet"/> containing special cases for the specified module.</returns>
        public DataSet GetSpecialCasesByModule(int moduleId, int roleId)
        {
            return DataStore.ExecuteDataSet("core_lod_sp_GetSpecialCasesByModuleId", roleId, moduleId);
        }

        /// <summary>
        /// Gets the count of special cases for a specific module and role.
        /// Executes stored procedure: core_lod_sp_GetSpecialCasesByModuleCount
        /// </summary>
        /// <param name="moduleId">The module identifier.</param>
        /// <param name="roleId">The role identifier.</param>
        /// <returns>The count of special cases for the specified module.</returns>
        public int GetSpecialCasesByModuleCount(int moduleId, int roleId)
        {
            return (int)DataStore.ExecuteScalar("core_lod_sp_GetSpecialCasesByModuleCount", roleId, moduleId);
        }

        /// <summary>
        /// Gets the total count of special cases for the specified role.
        /// Executes stored procedure: core_lod_sp_GetSpecialCasesCount
        /// </summary>
        /// <param name="roleId">The role identifier.</param>
        /// <returns>The total count of special cases.</returns>
        public int GetSpecialCasesCount(int roleId)
        {
            return (int)DataStore.ExecuteScalar("core_lod_sp_GetSpecialCasesCount", roleId);
        }

        /// <summary>
        /// Retrieves all special cases in holding status for a specific module and user.
        /// Executes stored procedure: core_lod_sp_GetSpecialCaseHolding
        /// </summary>
        /// <param name="moduleId">The module identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns>A <see cref="DataSet"/> containing special cases in holding status.</returns>
        public DataSet GetSpecialCasesHolding(int moduleId, int userId)
        {
            return DataStore.ExecuteDataSet("core_lod_sp_GetSpecialCaseHolding", moduleId, userId);
        }

        /// <summary>
        /// Retrieves all special cases not in holding status for a specific module and user.
        /// Executes stored procedure: core_lod_sp_GetSpecialCaseNotHolding
        /// </summary>
        /// <param name="moduleId">The module identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns>A <see cref="DataSet"/> containing special cases not in holding status.</returns>
        public DataSet GetSpecialCasesNotHolding(int moduleId, int userId)
        {
            return DataStore.ExecuteDataSet("core_lod_sp_GetSpecialCaseNotHolding", moduleId, userId);
        }

        /// <summary>
        /// Retrieves subclass-specific properties for a special case.
        /// Executes stored procedure: form348_sc_sp_properties
        /// </summary>
        /// <param name="moduleId">The module identifier.</param>
        /// <param name="scId">The special case identifier.</param>
        /// <returns>A <see cref="DataSet"/> containing subclass property information.</returns>
        public DataSet GetSubClassProperties(int moduleId, int scId)
        {
            return DataStore.ExecuteDataSet("form348_sc_sp_properties", moduleId, scId);
        }

        /// <summary>
        /// Determines the user's access level for a specific special case within a module.
        /// Executes stored procedure: lod_sp_UserHasAccess
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="refId">The reference identifier of the special case.</param>
        /// <param name="moduleId">The module identifier.</param>
        /// <returns>The user's <see cref="PageAccess.AccessLevel"/> (None, ReadOnly, or ReadWrite).</returns>
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

        /// <summary>
        /// Determines if a special case has RWOA (Rebuttal to Wrongful - Officer Appointment) capability.
        /// Executes stored procedure: core_lookUps_sp_SpecCaseRWOA
        /// </summary>
        /// <param name="workflow">The workflow identifier.</param>
        /// <param name="refId">The special case reference identifier.</param>
        /// <returns>True if RWOA capability exists; otherwise, false.</returns>
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

        /// <summary>
        /// Searches for special cases using basic search criteria (combined name).
        /// Executes stored procedure: form348_sc_sp_search
        /// </summary>
        /// <param name="caseId">The case identifier to search for.</param>
        /// <param name="memberSSN">The member's social security number to search for.</param>
        /// <param name="memberName">The combined member name to search for.</param>
        /// <param name="statusId">The status identifier filter.</param>
        /// <param name="userId">The user identifier performing the search.</param>
        /// <param name="compoId">The component identifier filter.</param>
        /// <param name="unitId">The unit identifier filter.</param>
        /// <param name="rptViewId">The report view identifier.</param>
        /// <param name="module">The module identifier.</param>
        /// <param name="maxCount">The maximum number of results to return.</param>
        /// <param name="sarcpermission">True if the user has SARC permissions; otherwise, false.</param>
        /// <returns>A <see cref="DataSet"/> containing matching special case records.</returns>
        public DataSet SpecialCaseSearch(string caseId, string memberSSN, string memberName, int statusId, int userId, int compoId, int unitId, int rptViewId, int module, int maxCount, bool sarcpermission)
        {
            return DataStore.ExecuteDataSet("form348_sc_sp_search", caseId, memberSSN, memberName, statusId, userId, rptViewId, compoId, maxCount, module, unitId, sarcpermission, null);
        }

        /// <summary>
        /// Searches for special cases using detailed search criteria (separate name parts).
        /// Executes stored procedure: Form348_SC_sp_FullSearch
        /// </summary>
        /// <param name="caseId">The case identifier to search for.</param>
        /// <param name="memberSSN">The member's social security number to search for.</param>
        /// <param name="lastName">The member's last name to search for.</param>
        /// <param name="firstName">The member's first name to search for.</param>
        /// <param name="middleName">The member's middle name to search for.</param>
        /// <param name="statusId">The status identifier filter.</param>
        /// <param name="userId">The user identifier performing the search.</param>
        /// <param name="compoId">The component identifier filter.</param>
        /// <param name="unitId">The unit identifier filter.</param>
        /// <param name="rptViewId">The report view identifier.</param>
        /// <param name="module">The module identifier.</param>
        /// <param name="maxCount">The maximum number of results to return.</param>
        /// <param name="sarcpermission">True if the user has SARC permissions; otherwise, false.</param>
        /// <returns>A <see cref="DataSet"/> containing matching special case records.</returns>
        public DataSet SpecialCaseSearch(string caseId, string memberSSN, string lastName, string firstName, string middleName, int statusId, int userId, int compoId, int unitId, int rptViewId, int module, int maxCount, bool sarcpermission)
        {
            return DataStore.ExecuteDataSet("Form348_SC_sp_FullSearch", caseId, memberSSN, lastName, firstName, middleName, statusId, userId, rptViewId, compoId, maxCount, module, unitId, sarcpermission, null);
        }

        /// <summary>
        /// Updates the PEPP (Physical Evaluation Processing Program) types associated with a special case.
        /// Executes stored procedure: Form348_SC_sp_UpdatePEPPTypes with table-valued parameter
        /// </summary>
        /// <param name="refId">The special case reference identifier.</param>
        /// <param name="types">The list of PEPP type identifiers to associate with the case.</param>
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