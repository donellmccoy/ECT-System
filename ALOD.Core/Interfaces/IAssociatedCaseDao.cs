using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ALOD.Core.Domain.Modules.SpecialCases;

namespace ALOD.Core.Interfaces
{
    public interface IAssociatedCaseDao
    {
        IList<AssociatedCase> GetAssociatedCases(int refId, int workflowId);

        IList<Tuple<String, int, int>> GetLODListByMemberSSN(string memberSSN, int searchType, int userId);

        void Save(int refId, int workflowId, IList<int> associated_refId, IList<int> associated_workflowId);

        IList<Tuple<String, int, int>> GetLODCaseIds(int refId, int workflowId);
    }
}
