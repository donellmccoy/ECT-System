using ALOD.Core.Domain.Users;
using System.Collections.Generic;
using System.Data;

namespace ALOD.Core.Interfaces.DAOInterfaces
{
    public interface IUserRoleRequestDao : IDao<UserRoleRequest, int>
    {
        DataSet GetAllPendingRequests(int userId);

        IList<UserRoleRequest> GetRequestsByUser(int userId);
    }
}