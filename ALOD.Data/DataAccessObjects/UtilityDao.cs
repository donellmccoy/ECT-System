using ALOD.Core.Utils;
using NHibernate;

namespace ALOD.Data
{
    internal class UtilityDao : IUtilityDao
    {
        private ISession Session
        {
            get { return NHibernateSessionManager.Instance.GetSession(); }
        }

        /// <inheritdoc/>
        public bool AssignIo(int refId, int ioUserId, int aaUserId, bool isFormal)
        {
            SqlDataStore store = new SqlDataStore();
            int result = (int)store.ExecuteScalar("Form348_sp_AssignIo", refId, ioUserId, aaUserId, isFormal);
            return (result > 0);
        }
    }
}