using ALOD.Core.Domain.Modules.SpecialCases;
using ALOD.Core.Domain.Users;
using ALOD.Core.Domain.Workflow;
using System.Collections.Generic;
using System.Data;

namespace ALOD.Core.Interfaces.DAOInterfaces
{
    public interface ISpecialCaseDAO : IDao<SpecialCase, int>
    {
        int CreateINCAPAppealFindings(int scId);

        int CreateINCAPExtFindings(int scId);

        int CreateINCAPFindings(int scId);

        int CreateReassessRSCase(int userId, int originalRefId, string newCaseId, int workflowId);

        DataSet GetAvailabilityCode();

        DataSet GetCompletedSCs(int roleId);

        DataSet GetCompletedSpecialCasesByMemberSSN(string ssn, int userId);

        bool GetHasReassessmentCase(int refId);

        DataSet GetIRILOStatus();

        bool GetIsReassessmentCase(int refId);

        DataSet GetMedDisposition();

        DataSet GetMemberSpecialCaseHistory(string memberSSN, int userId);

        DataSet GetPSCDAssociableIRSpecialCases(string memberSSN);

        DataSet GetPSCDAssociableRWSpecialCases(string memberSSN);

        SpecialCase GetReassessmentByOriginalId(int originalRefId);

        int GetReassessmentCount(int originalRefId);

        DataSet GetRWAssociableSpecialCases(string memberSSN);

        DataSet GetSpecailCaseDisposition(int moduleId, int userId);

        DataSet GetSpecailCaseNoDisposition(int moduleId, int userId);

        DataSet GetSpecialAwaitingConsult(int wsId);

        SpecialCase GetSpecialCaseByCaseId(string caseId);

        DataSet GetSpecialCaseById(int scId);

        int GetSpecialCaseNotHoldingCount(int moduleId, int userId);

        IList<LookUpItem> GetSpecialCasePEPPTypes(int refId);

        DataSet GetSpecialCases(int roleId);

        DataSet GetSpecialCasesByMemberSSN(string ssn, int userId);

        DataSet GetSpecialCasesByModule(int moduleId, int roleId);

        int GetSpecialCasesByModuleCount(int moduleId, int roleId);

        int GetSpecialCasesCount(int roleId);

        DataSet GetSpecialCasesHolding(int moduleId, int userId);

        DataSet GetSpecialCasesNotHolding(int moduleId, int userId);

        DataSet GetSubClassProperties(int moduleId, int scId);

        PageAccess.AccessLevel GetUserAccess(int userId, int refId, int moduleId);

        bool hasRWOA(int workflow, int refId);

        DataSet SpecialCaseSearch(string caseId, string memberSSN, string memberName, int statusId, int userId, int compoId, int unitId, int rptViewId, int module, int maxCount, bool sarcpermission);

        DataSet SpecialCaseSearch(string caseId, string memberSSN, string lastName, string firstName, string middleName, int statusId, int userId, int compoId, int unitId, int rptViewId, int module, int maxCount, bool sarcpermission);

        void UpdateSpecialCasePEPPTypes(int refId, IList<int> types);
    }
}