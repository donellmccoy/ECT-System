namespace ALOD.Core.Interfaces.DAOInterfaces
{
    using ALOD.Core.Domain.Users;
    using System.Collections.Generic;

    public interface IUserGroupDao : IDao<UserGroup, int>
    {
        //void UpdateViewBy(int groupId, int views);
        List<UserGroup> GetAll(int compo);

        System.Data.DataSet GetAllWithManaged(int groupId);

        System.Data.DataSet GetAllWithViewBy(int groupId);

        System.Data.DataSet GetManagedGroups(int groupId);

        string GetNameById(int groupId, int compo);

        void UpdateManagedBy(int groupId, System.Data.DataSet groups);
    }
}