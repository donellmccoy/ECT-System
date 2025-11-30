using ALOD.Core.Domain.Workflow;
using System.Collections.Generic;

namespace ALOD.Core.Interfaces.DAOInterfaces
{
    public interface IALODPermissionDao : IDao<ALODPermission, int>
    {
        int GetDocGroupIdByPermId(int permId);

        IList<ALODPermission> GetPermissionsByGroupId(int groupId);

        void InsertNewDocGroup(int permId, int docGroupId);

        void UpdateDocGroup(int oldPermId, int oldDocGroupId, int newPermId, int newDocGroupId);
    }
}