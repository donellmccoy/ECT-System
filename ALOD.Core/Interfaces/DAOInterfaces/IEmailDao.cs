using ALOD.Core.Domain.Common;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace ALOD.Core.Interfaces.DAOInterfaces
{
    public interface IEmailDao
    {
        EMailMessage CreateMessage(int templateId, string from, StringCollection toList);

        EMailMessage CreateMessage(int templateId, string from, string to);

        EMailMessage CreateMessage(string subject, string body, string from, StringCollection toList);

        StringCollection GetDistributionListByGroup(int refId, int groupId, string callingGroup, bool isFinal);

        StringCollection GetDistributionListByRoles(string compo, int unitId, string roles);

        StringCollection GetDistributionListByStatus(int refId, short workStatus);

        StringCollection GetDistributionListBySystemParameters(bool includeWork, bool includePersonal, bool includeUnit, IList<Microsoft.SqlServer.Server.SqlDataRecord> userGroups);

        StringCollection GetDistributionListForLOD(int refId, int groupId, int workStatus, string callingGroup);

        StringCollection GetEmailListForBoardLevelUsers();

        StringCollection GetEmailListForUsersByGroup(int groupId);

        StringDictionary GetTemplateData(string storeproc);
    }
}