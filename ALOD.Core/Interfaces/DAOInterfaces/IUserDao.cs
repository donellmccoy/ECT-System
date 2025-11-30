using ALOD.Core.Domain.Users;
using System.Collections.Generic;
using System.Data;

namespace ALOD.Core.Interfaces.DAOInterfaces
{
    public interface IUserDao : IDao<AppUser, int>
    {
        void ClearOnlineUser(int userId);

        int DuplicateAccount(int copyFromId, int userId);

        IList<AppUser> FindByEDIPIN(string EDIPIN);

        IList<AppUser> FindBySSN(string SSN);

        IList<AppUser> FindByUsername(string username);

        AppUser GetByEDIPIN(string EDIPIN);

        IList<AppUser> GetByPasCode(int cscId);

        AppUser GetByServiceMemberSSN(string memberSSN);

        AppUser GetBySSN(string ssn);

        //int GetPendingLegacyLodCount(int userId);
        DataSet GetOnLineUsers();

        int GetPendingCount(int userId);

        string GetUserAlternateTitle(int userId, int groupId);

        string GetUserName(string firstName, string lastName);

        DataSet GetUsersAlternateTitleByGroup(int groupId);

        DataSet GetUsersAlternateTitleByGroupCompo(int groupId, int workCompo);

        DataSet GetUsersWithPermission(short permissionId);

        bool HasHQTechAccount(int originUserId, string edipin);

        bool IsEDIPINAvailable(string EDIPIN);

        void Logout(int userId);

        void UpdateUserAlternateTitle(int userId, int groupId, string newTitle);
    }
}