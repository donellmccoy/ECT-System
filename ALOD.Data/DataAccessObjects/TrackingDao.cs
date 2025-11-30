using ALOD.Core.Domain.Common;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Core.Utils;
using NHibernate;
using System.Linq;

namespace ALOD.Data
{
    internal class TrackingDao : AbstractNHibernateDao<TrackingItem, int>, ITrackingDao
    {
        /// <inheritdoc/>
        public IQueryable<TrackingItem> GetByParentId(int parentId, byte moduleType, bool showAll)
        {
            Check.Require(parentId > 0, "parentId cannot be 0");
            Check.Require(moduleType > 0, "moduleType cannot be 0");

            ISession session = NHibernateSessionManager.Instance.GetSession();

            IQuery query = session.GetNamedQuery("GetTrackingByRefId");
            query.SetByte("moduleId", moduleType);
            query.SetInt32("referenceId", parentId);
            query.SetBoolean("showAll", showAll);

            return query.List<TrackingItem>().AsQueryable<TrackingItem>();
        }

        /// <inheritdoc/>
        public System.Data.DataSet GetByUserId(int userId, bool showAll)
        {
            SqlDataStore source = new SqlDataStore();
            return source.ExecuteDataSet("core_tracking_sp_GetByUserId", userId, showAll);
        }
    }
}