using ALOD.Core.Domain.Users;
using System;
using System.Collections.Generic;

namespace ALOD.Core.Interfaces.DAOInterfaces
{
    public interface IUserRoleDao : IDao<UserRole, int>
    {
        IList<UserRole> GetByUserId(int userId);

        UserRole GetByUserIdAndGroup(int userId, Int16 group);

        void UpdateGroup(int roleId, short groupId, short status, int userId);
    }
}