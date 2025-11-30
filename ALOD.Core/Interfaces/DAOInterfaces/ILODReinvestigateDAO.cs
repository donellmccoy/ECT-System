using ALOD.Core.Domain.Modules.Reinvestigations;
using ALOD.Core.Domain.Workflow;
using System.Data;

namespace ALOD.Core.Interfaces.DAOInterfaces
{
    public interface ILODReinvestigateDAO : IDao<LODReinvestigation, int>
    {
        LODReinvestigation GetByCaseId(string caseId);

        DataSet GetCompletedRRs(int roleId, bool sarc);

        int GetReinvestigationRequestCount(int userId, bool sarc);

        int GetReinvestigationRequestIdByInitLod(int LodId);

        int GetReinvestigationRequestIdByRLod(int LodId);

        DataSet GetReinvestigationRequests(int userId, bool sarc);

        int GetReinvestigationRequestsCount(int initialLODId);

        PageAccess.AccessLevel GetUserAccess(int userId, int refId);

        DataSet ReinvestigationRequestSearch(string caseID, string ssn, string name, int status, int userId, byte rptView, string compo, int maxCount, byte moduleId, int unitId);

        DataSet ReinvestigationRequestSearch(string caseID, string ssn, string lastName, string firstName, string middleName, int status, int userId, byte rptView, string compo, int maxCount, byte moduleId, int unitId);
    }
}