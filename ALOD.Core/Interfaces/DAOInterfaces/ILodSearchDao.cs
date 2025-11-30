using ALOD.Core.Domain.Modules.Lod;
using System.Data;

namespace ALOD.Core.Interfaces.DAOInterfaces
{
    public interface ILodSearchDao : IDao<LodSearch, int>
    {
        DataSet GetAll(string caseID, string ssn, string name, int status, int userId, byte rptView, string compo, int maxCount, byte moduleId, string IsFormal, int unitId, bool sarcpermission);

        //(string caseId, string ssn, string memberName, string memberUnit, string state, string date, int days, string description, string displayText, string pasPriority, string priorityRank);
        DataSet GetAll(string caseID, string ssn, string lastName, string firstName, string middleName, int status, int userId, byte rptView, string compo, int maxCount, byte moduleId, string IsFormal, int unitId, bool sarcpermission);

        LineOfDuty GetByCaseId(string caseId);

        DataSet GetByPilotUser(int wsId, int compo);

        DataSet GetByUser(string caseID, string ssn, string name, int status, int userId, byte rptView, string compo, int maxCount, byte moduleId, string IsFormal, int unitId, bool sarcpermission);

        DataSet GetByUser(string caseID, string ssn, string lastName, string firstName, string middleName, int status, int userId, byte rptView, string compo, int maxCount, byte moduleId, string IsFormal, int unitId, bool sarcpermission);

        DataSet GetByUserLOD_IO(string caseID, string ssn, string name, int status, int userId, byte rptView, string compo, int maxCount, byte moduleId, string IsFormal, int unitId, bool sarcpermission);

        DataSet GetByUserLODV3(string caseID, string ssn, string name, int status, int userId, byte rptView, string compo, int maxCount, byte moduleId, string IsFormal, int unitId, bool sarcpermission);

        DataSet GetLodsBySM(string ssn, bool sarcpermission);

        DataSet GetPostCompletion(string caseID, string ssn, string name, int userId, byte rptView, string compo, int maxCount, byte moduleId, string IsFormal, int unitId, bool sarcpermission, bool searchAllCases, int wsId);

        DataSet GetUndeleted(string caseID, string ssn, string name, int status, int userId, byte rptView, string compo, int maxCount, byte moduleId, string IsFormal, int unitId, bool sarcpermission, bool ovreridescope);

        DataSet GetUndeletedCases(string ssn, int userId, byte rptView, string compo, int unitId, bool sarcpermission, bool ovreridescope);
    }
}