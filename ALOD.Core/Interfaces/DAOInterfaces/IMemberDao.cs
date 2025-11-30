using ALOD.Core.Domain.ServiceMembers;
using System.Collections.Generic;
using System.Data;

namespace ALOD.Core.Interfaces.DAOInterfaces
{
    public interface IMemberDao : IDao<ServiceMember, string>
    {
        IList<ServiceMemberMILPDSChangeHistory> GetMILPDSChangeHistoryBySSN(string ssn);

        DataSet GetSystemAdminChangeHistoryBySSN(string ssn);
    }
}