using ALOD.Core.Domain.Lookup;
using ALOD.Core.Domain.Workflow;
using System.Collections.Generic;

namespace ALOD.Core.Interfaces.DAOInterfaces
{
    public interface ICaseTypeDao
    {
        IList<CaseType> GetAll();

        IList<CaseType> GetAllSubCaseTypes();

        CaseType GetById(int id);

        IList<Workflow> GetCaseTypeWorkflows(int caseTypeId);

        IList<CaseType> GetWorkflowCaseTypes(int workflowId);

        void Insert(CaseType ct);

        void InsertSubCaseType(CaseType ct);

        void Update(CaseType ct);

        void UpdateCaseTypeSubCaseTypeMaps(int caseTypeId, IList<int> subCaseTypeIds);

        void UpdateCaseTypeWorkflowMaps(int caseTypeId, IList<int> workflowIds);

        void UpdateSubCaseType(CaseType ct);

        //void UpdateSubCaseTypeCaseTypeMaps(int subCaseTypeId, IList<int> caseTypeIds);
    }
}