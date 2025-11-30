using ALOD.Core.Domain.Modules.SARC;
using ALOD.Core.Domain.Workflow;
using System.Data;

namespace ALOD.Core.Interfaces.DAOInterfaces
{
    public interface ISARCAppealDAO : IDao<SARCAppeal, int>
    {
        int GetAppealCount(int initialId, int workflowId);

        int GetAppealIdByInitId(int LodId, int workflowId);

        DataSet GetAppealRequests(int userId, bool sarc);

        SARCAppeal GetByCaseId(string caseId);

        DataSet GetCompletedAPs(int roleId, bool sarc);

        int GetPostCompletionCaseCount(int userId);

        DataSet GetPostSARCAppealCompletion(string caseID, string ssn, string name, int userId, byte rptView, string compo, int maxCount, byte moduleId, int unitId, bool sarcpermission);

        PageAccess.AccessLevel GetUserAccess(int userId, int refId);

        DataSet SARCAppealRequestSearch(string caseID, string ssn, string name, int status, int userId, byte rptView, string compo, int maxCount, byte moduleId, int unitId);
    }
}