using ALOD.Core.Domain.Modules.Appeals;
using ALOD.Core.Domain.Workflow;
using System.Data;

namespace ALOD.Core.Interfaces.DAOInterfaces
{
    public interface ILODAppealDAO : IDao<LODAppeal, int>
    {
        DataSet AppealRequestSearch(string caseID, string ssn, string name, int status, int userId, byte rptView, string compo, int maxCount, byte moduleId, int unitId);

        DataSet AppealRequestSearch(string caseID, string ssn, string lastName, string firstName, string middleName, int status, int userId, byte rptView, string compo, int maxCount, byte moduleId, int unitId);

        int GetAppealCount(int initialLODId);

        int GetAppealIdByInitLod(int LodId);

        DataSet GetAppealRequests(int userId, bool sarc);

        LODAppeal GetByCaseId(string caseId);

        DataSet GetCompletedAPs(int roleId, bool sarc);

        System.Data.DataSet GetPostAppealCompletion(string caseID, string ssn, string name,
                                     int userId, byte rptView, string compo, int maxCount, byte moduleId, string IsFormal, int unitId, bool sarcpermission);

        PageAccess.AccessLevel GetUserAccess(int userId, int refId);
    }
}