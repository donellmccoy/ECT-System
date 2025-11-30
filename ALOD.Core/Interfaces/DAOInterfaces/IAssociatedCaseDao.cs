using ALOD.Core.Domain.Modules.SpecialCases;
using System;
using System.Collections.Generic;

namespace ALOD.Core.Interfaces.DAOInterfaces
{
    public interface IAssociatedCaseDao
    {
        IList<AssociatedCase> GetAssociatedCases(int refId, int workflowId);

        IList<AssociatedCase> GetAssociatedCasesLOD(int refId, int workflowId);

        IList<AssociatedCase> GetAssociatedCasesSC(int refId, int workflowId);

        IList<Tuple<String, int, int>> GetLODListByMemberSSN(string memberSSN, int searchType, int userId);

        void Save(int refId, int workflowId, IList<int> associated_refId, IList<int> associated_workflowId, IList<string> associated_caseId);
    }
}