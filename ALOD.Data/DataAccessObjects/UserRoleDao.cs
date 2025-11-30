using ALOD.Core.Domain.Users;
using ALOD.Core.Interfaces.DAOInterfaces;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;

namespace ALOD.Data
{
    public class UserRoleDao : AbstractNHibernateDao<UserRole, int>, IUserRoleDao
    {
        /// <inheritdoc/>
        public IList<UserRole> GetByUserId(int userId)
        {
            return NHibernateSession.CreateCriteria(typeof(UserRole))
                .Add(Expression.Eq("User.Id", userId))
                .List<UserRole>();
        }

        /// <inheritdoc/>
        public UserRole GetByUserIdAndGroup(int userId, Int16 group)
        {
            IList<UserRole> list = NHibernateSession.CreateCriteria(typeof(UserRole))
             .Add(Expression.Eq("User.Id", userId))
             .Add(Expression.Eq("Group.Id", group))
             .List<UserRole>();

            if (list.Count > 0)
            {
                return list[0];
            }
            else
            {
                return null;
            }
        }

        /// <inheritdoc/>
        public void UpdateGroup(int roleId, short groupId, short status, int userId)
        {
            IQuery query = NHibernateSession.GetNamedQuery("UpdateUserRole")
                .SetInt32("roleId", roleId)
                .SetInt16("groupId", groupId)
                .SetInt16("status", status)
                .SetInt32("userId", userId);

            query.ExecuteUpdate();
        }
    }
}