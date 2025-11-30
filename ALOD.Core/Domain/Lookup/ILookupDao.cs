using ALOD.Core.Domain.Common;
using ALOD.Core.Domain.ServiceMembers;
using ALOD.Core.Domain.Users;
using ALOD.Core.Domain.Workflow;
using System;
using System.Collections.Generic;
using System.Data;

namespace ALOD.Core.Domain.Lookup
{
    public interface ILookupDao
    {
        Boolean DeleteCaseComments(int id);

        IList<LookUpItem> Get_LODPMs(int unitId);

        IList<LookUpItem> Get_WingSARCs(int unitId);

        DataSet GetAllDAWGRecommendations();

        IList<FindingByReasonOf> GetAllFindingByReasonOfs();

        DataSet GetAllFollowUpIntervals();

        DataSet GetAllMissedWorkDays();

        DataSet GetAllModules();

        IList<ReserveMedicalUnit> GetAllRMUs();

        DataSet GetAllSpecialistsRequiredForManagement();

        DataSet GetAllSuddenIncapaciationRisks();

        DataSet GetAllYearsSatisfactoryService();

        string GetCancelReasonDescription(int reasonId);

        DataSet GetCaseComments(int caseID, int ModuleID, int commentType, bool SortOrder);

        string GetCaseTypeName(int id);

        IList<LookUpItem> GetCategory();

        IList<LookUpItem> GetCauses();

        IList<LookUpItem> GetChainType();

        IList<LookUpItem> GetChildUnits(int cs_id, ReportingView rptView);

        String GetCommentsCreatedBy(int created_by);

        string GetCompletedByGroupName(int id);

        IList<LookUpItem> GetComponents();

        IList<LookUpItem> GetCompos();

        string GetDataTypeById(int id);

        DataSet GetDataTypes();

        IList<LookUpItem> GetDutyStatuses();

        FindingByReasonOf GetFindingByReasonOfById(int id);

        IList<LookUpItem> GetFindings();

        IList<FindingsLookUp> GetFindingTypes();

        DataSet GetForm348Snapshot(DateTime beginDate, DateTime endDate);

        string GetFormFieldParserById(int id);

        IList<LookUpItem> GetFromLocation();

        IList<UserGroup> GetGroupsByCompo(string compo);

        bool GetHasAppealLOD(int initialLodId);

        bool GetHasAppealSARC(int refId, int workflowId);

        bool GetHasReinvestigationLod(int initialLodId);

        bool GetHasSubCaseTypes(int id);

        ICD9Code GetIcd9ById(int id);

        IList<LookUpItem> GetInfoSources();

        int GetInitialStatus(int userId, byte workflow, int groupId);

        int GetInProgressReinvestigation(int initialLodId);

        IList<LookUpItem> GetIOList(int userId, short rptView, int memberCaseGradeCode);

        IList<int> GetIRILOTypeICDCodeIds(string typeName);

        bool GetIsReassessmentSpecialCase(int refId);

        bool GetIsReinvestigationLod(int lodId);

        IList<LookUpItem> GetMedGroupNames(string searchBegin);

        IList<LookUpItem> GetMedicalFacility();

        IList<LookUpItem> GetMemberComponentsByWorkflow(int workflowId);

        IList<LookUpItem> GetMemberInfluence();

        IList<LookUpItem> GetOccurrences();

        DataSet GetPEPPCaseTypes();

        DataSet GetPEPPDispositions(int id, int filter);

        DataSet GetPEPPRatings(int id, int filter);

        DataSet GetPEPPTypes(int id, int filter);

        IList<LookUpItem> GetProcess();

        IList<LookUpItem> GetPSCDMemberStatus();

        int GetPSCDMemoId(int refId, int templetId);

        DataSet GetPWCategories();

        UserRank GetRank(int code);

        string GetRankAbbreviationByType(UserRank rank, string typeName);

        IList<LookUpItem> GetRanks();

        //IList<> GetCategory();
        IList<UserRank> GetRanksAndGrades();

        string GetReasonDescription(short id);

        IList<int> GetReinvestigationLodFindings(int initialLodId);

        DataSet GetReportPeriod();

        IList<LookUpItem> GetRMU();

        ReserveMedicalUnit GetRMUById(int id);

        IList<LookUpItem> GetRMUNames(string searchBegin);

        int GetSCSubWorkflowTypeId(string workflowTitle);

        DataSet GetSCSubWorkflowTypes(int workflowId);

        ServiceMember GetServiceMemberBySSN(string ssn);

        DataTable GetServiceMembersByName(string lastName, string firstName, string middleName);

        IList<LookUpItem> GetSpecialCasePEPPTypes(int refId);

        IList<State> GetStates();

        IList<LookUpItem> GetStatus();

        IList<StatusCode> GetStatusCodesByModule(int moduleId);

        IList<WorkStatusTracking> GetStatusTracking(int refId, byte moduleType);

        string GetSubCaseTypeName(int id);

        Unit GetUnitById(int unitId);

        string GetUnitPasByIdAndName(int id, string name);

        string GetUnitText(int cs_id);

        DataSet GetWorkflowCancelReasons(int workflowId, bool isFormal);

        IList<FindingsLookUp> GetWorkflowFindings(int workflowId, int groupId);

        int GetWorkflowInitialStatusCode(int compo, int moduleId, int workflowId);

        DataSet GetWorkflowReturnReasons(int workflowId);

        DataSet GetWorkflowRwoaReasons(int workflowId);

        IList<ALOD.Core.Domain.Workflow.Workflow> GetWorkflowsByCompo(string compo);

        DataSet GetWorkStatusByWorkflow(byte workflow);

        IList<LookUpItem> GetWWDListByMemberSSN(string memberSSN, int searchType, int userId);

        bool LODHasActiveReinvestigation(int initialLodId);

        Boolean SetCaseComments(int caseID, string comments, int created_by, DateTime created_date, int ModuleID, int commentType, bool deleted);

        Boolean UpdateCaseComments(int id, int caseID, string comments, int created_by, DateTime created_date, int ModuleID, int commentType, bool deleted);
    }
}