using ALOD.Core.Domain.Users;
using ALOD.Core.Interfaces.DAOInterfaces;
using NHibernate.Criterion;
using System.Collections.Generic;
using System.Data;

namespace ALOD.Data
{
    public class UseRoleRequestDao : AbstractNHibernateDao<UserRoleRequest, int>, IUserRoleRequestDao
    {
        /// <inheritdoc/>
        public DataSet GetAllPendingRequests(int userId)
        {
            SqlDataStore store = new SqlDataStore();
            return store.ExecuteDataSet("core_permissions_sp_GetAllPendingRequests", userId);
        }

        /// <inheritdoc/>
        public IList<UserRoleRequest> GetRequestsByUser(int userId)
        {
            return NHibernateSession.CreateCriteria(typeof(UserRoleRequest))
                .Add(Expression.Eq("User.Id", userId))
                .List<UserRoleRequest>();
        }
    }
}