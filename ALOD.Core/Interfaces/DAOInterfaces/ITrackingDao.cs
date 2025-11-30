using ALOD.Core.Domain.Common;
using System.Data;
using System.Linq;

namespace ALOD.Core.Interfaces.DAOInterfaces
{
    public interface ITrackingDao : IDao<TrackingItem, int>
    {
        IQueryable<TrackingItem> GetByParentId(int parentId, byte moduleType, bool showAll);

        DataSet GetByUserId(int userId, bool showAll);
    }
}