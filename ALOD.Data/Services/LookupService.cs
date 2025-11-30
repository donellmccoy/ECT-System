using ALOD.Core.Domain.Common;
using ALOD.Core.Domain.Lookup;
using ALOD.Core.Domain.ServiceMembers;
using ALOD.Core.Domain.Users;
using ALOD.Core.Domain.Workflow;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace ALOD.Data.Services
{
    /// <summary>
    /// Service for accessing lookup data and reference information including ranks, units, workflows, and status codes.
    /// Provides a centralized interface for retrieving lookup values used throughout the application.
    /// </summary>
    public class LookupService
    {
        private static ILookupDao _dao;

        public static ILookupDao Dao
        {
            get
            {
                if (_dao == null)
                    _dao = new NHibernateDaoFactory().GetLookupDao();

                return _dao;
            }
        }

        public static DataSet GetAllModules()
        {
            return Dao.GetAllModules();
        }

        public static string GetCancelReasonDescription(int reasonId)
        {
            return Dao.GetCancelReasonDescription(reasonId);
        }

        public static IList<LookUpItem> GetChainType()
        {
            return Dao.GetChainType();
        }

        public static IList<LookUpItem> GetChildUnits(int cs_id, ReportingView rptView)
        {
            return Dao.GetChildUnits(cs_id, rptView);
        }

        public static IList<LookUpItem> GetCompos()
        {
            return Dao.GetCompos();
        }

        public static string GetFormFieldParserById(int id)
        {
            return Dao.GetFormFieldParserById(id);
        }

        public static IList<UserGroup> GetGroupsByCompo(string compo)
        {
            return Dao.GetGroupsByCompo(compo);
        }

        public static bool GetHasAppealLOD(int initialLodId)
        {
            return Dao.GetHasAppealLOD(initialLodId);
        }

        public static bool GetHasAppealSARC(int initialLodId, int workflowId)
        {
            return Dao.GetHasAppealSARC(initialLodId, workflowId);
        }

        public static bool GetHasReinvestigationLod(int initialLodId)
        {
            return Dao.GetHasReinvestigationLod(initialLodId);
        }

        public static ICD9Code GetIcd9CodeById(int id)
        {
            return Dao.GetIcd9ById(id);
        }

        public static int GetInitialStatus(int userId, int groupId, byte workflow)
        {
            return Dao.GetInitialStatus(userId, workflow, groupId);
        }

        public static int GetInProgressReinvestigation(int initialLodId)
        {
            return Dao.GetInProgressReinvestigation(initialLodId);
        }

        public static IList<LookUpItem> GetIOList(int userId, short rptView, int memberCaseGradeCode)
        {
            return Dao.GetIOList(userId, rptView, memberCaseGradeCode);
        }

        public static bool GetIsReinvestigationLod(int lodId)
        {
            return Dao.GetIsReinvestigationLod(lodId);
        }

        public static System.Data.DataSet GetPWCategories()
        {
            return Dao.GetPWCategories();
        }

        public static UserRank GetRank(int code)
        {
            return Dao.GetRank(code);
        }

        public static IList<UserRank> GetRanksAndGrades()
        {
            return Dao.GetRanksAndGrades();
        }

        public static IList<int> GetReinvestigationLodFindings(int initialLodId)
        {
            return Dao.GetReinvestigationLodFindings(initialLodId);
        }

        public static string GetRwoaReasonDescription(short Id)
        {
            return Dao.GetReasonDescription(Id);
        }

        public static int GetSCSubWorkflowTypeId(string workflowTitle)
        {
            return Dao.GetSCSubWorkflowTypeId(workflowTitle);
        }

        public static DataSet GetSCSubWorkflowTypes(int workflowId)
        {
            return Dao.GetSCSubWorkflowTypes(workflowId);
        }

        public static DataTable GetServerMembersByName(string lastName, string firstName, string middleName)
        {
            return Dao.GetServiceMembersByName(lastName, firstName, middleName);
        }

        public static ServiceMember GetServiceMemberBySSN(string ssn)
        {
            return Dao.GetServiceMemberBySSN(ssn);
        }

        public static IList<State> GetStates()
        {
            return Dao.GetStates();
        }

        public static IList<StatusCode> GetStatusCodesByModule(int moduleId)
        {
            return Dao.GetStatusCodesByModule(moduleId);
        }

        public static DataSet GetTimeZones()
        {
            SqlDataStore DataSource = new SqlDataStore();
            DbCommand cmd = DataSource.GetSqlStringCommand("Select * from core_lkupTimeZone");
            return DataSource.ExecuteDataSet(cmd);
        }

        public static Unit GetUnitById(int unitId)
        {
            return Dao.GetUnitById(unitId);
        }

        public static string GetUnitPasByIdAndName(int id, string name)
        {
            return Dao.GetUnitPasByIdAndName(id, name);
        }

        public static string GetUnitText(int cs_id)
        {
            return Dao.GetUnitText(cs_id);
        }

        public static DataSet GetWorkflowCancelReasons(int workflowId, bool isFormal)
        {
            return Dao.GetWorkflowCancelReasons(workflowId, isFormal);
        }

        public static int GetWorkflowInitialStatusCode(int compo, int moduleId, int workflowId)
        {
            return Dao.GetWorkflowInitialStatusCode(compo, moduleId, workflowId);
        }

        public static DataSet GetWorkflowReturnReasons(int workflowId)
        {
            return Dao.GetWorkflowReturnReasons(workflowId);
        }

        public static DataSet GetWorkflowRwoaReasons(int workflowId)
        {
            return Dao.GetWorkflowRwoaReasons(workflowId);
        }

        public static IList<Workflow> GetWorkflowsByCompo(string compo)
        {
            return Dao.GetWorkflowsByCompo(compo);
        }

        public static System.Data.DataSet GetWorkstatusByWorkflow(byte workflow)
        {
            return Dao.GetWorkStatusByWorkflow(workflow);
        }

        public static IList<WorkStatusTracking> GetWorkStatusTracking(int refId, byte moduleType)
        {
            return Dao.GetStatusTracking(refId, moduleType);
        }

        public static bool LODHasActiveReinvestigation(int initialLodId)
        {
            return Dao.LODHasActiveReinvestigation(initialLodId);
        }
    }
}