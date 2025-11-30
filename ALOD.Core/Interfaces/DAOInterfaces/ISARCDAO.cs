using ALOD.Core.Domain.Modules.SARC;
using ALOD.Core.Domain.Workflow;
using System.Data;

namespace ALOD.Core.Interfaces.DAOInterfaces
{
    public interface ISARCDAO : IDao<RestrictedSARC, int>
    {
        RestrictedSARC GetByCaseId(string caseId);

        int GetCaseCount(int userId);

        DataSet GetOpenCasesForUser(int id);

        int GetPostCompletionCaseCount(int userId);

        DataSet GetPostCompletionSearchResults(int userId, string caseId, string memberSSN, string memberName, int reportView, string compo, int unitId);

        DataSet GetSearchResults(int userId, string caseId, string memberSSN, string memberName, int reportView, string compo, int status, int unitId);

        PageAccess.AccessLevel GetUserAccess(int userId, int refId);
    }
}