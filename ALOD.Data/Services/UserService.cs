using ALOD.Core.Domain.Common;
using ALOD.Core.Domain.Users;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Core.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.Common;

namespace ALOD.Data.Services
{
    /// <summary>
    /// Service for managing user accounts, permissions, and authentication.
    /// Provides functionality for user CRUD operations, login management, and user-related data access.
    /// </summary>
    public class UserService : DataService
    {
        private static IUserDao _dao;
        private static IUserRoleDao _roleDao;

        public static IUserDao Dao
        {
            get
            {
                if (_dao == null)
                    _dao = new NHibernateDaoFactory().GetUserDao();

                return _dao;
            }
        }

        public static IUserRoleDao roleDao
        {
            get
            {
                if (_roleDao == null)
                    _roleDao = new NHibernateDaoFactory().GetUserRoleDao();

                return _roleDao;
            }
        }

        public static AppUser CurrentUser()
        {
            int id = (int)GetSessionObject("UserId");
            return Dao.GetById(id);
        }

        public static int DuplicateAccount(int copyFromId, int userId)
        {
            return Dao.DuplicateAccount(copyFromId, userId);
        }

        public static IList<AppUser> FindByEDIPIN(string edipin)
        {
            return Dao.FindByEDIPIN(edipin);
        }

        public static IList<AppUser> FindBySSN(string ssn)
        {
            return Dao.FindBySSN(ssn);
        }

        public static IList<AppUser> FindByUsername(string username)
        {
            return Dao.FindByUsername(username);
        }

        public static string GetApprovingAuthorityName()
        {
            SqlDataStore DataSource = new SqlDataStore();

            DbCommand cmd = DataSource.GetSqlStringCommand("Select Top 1 Name from form348_approval_authorities order by effective_date desc ");

            String name = (string)DataSource.ExecuteScalar(cmd);
            return name;
        }

        public static AppUser GetByEDIPIN(string EDIPIN)
        {
            return Dao.GetByEDIPIN(EDIPIN);
        }

        public static AppUser GetById(int userId)
        {
            return Dao.GetById(userId);
        }

        public static AppUser GetByServiceMemberSSN(string memberSSN)
        {
            return Dao.GetByServiceMemberSSN(memberSSN);
        }

        //public static int GetPendingLegacyLodCount(int userId)
        //{
        //    return Dao.GetPendingLegacyLodCount(userId);
        //}
        public static AppUser GetBySSN(string ssn)
        {
            return Dao.GetBySSN(ssn);
        }

        public static int GetIDByCredentials(string firstName, string lastName, string userName, string ssn)
        {
            SqlDataStore store = new SqlDataStore();
            int userId = (int)store.ExecuteScalar("core_user_sp_GetByCredentials", firstName, lastName, userName, ssn);
            return userId;
        }

        public static DataSet GetOnLineUsers()
        {
            return Dao.GetOnLineUsers();
        }

        public static int GetPendingCount(int userId)
        {
            return Dao.GetPendingCount(userId);
        }

        public static string GetUserAlternateTitle(int userId, int groupId)
        {
            return Dao.GetUserAlternateTitle(userId, groupId);
        }

        public static string GetUserName(string firstName, string lastName)
        {
            return Dao.GetUserName(firstName, lastName);
        }

        public static DataSet GetUsersAlternateTitleByGroup(int groupId)
        {
            return Dao.GetUsersAlternateTitleByGroup(groupId);
        }

        public static DataSet GetUsersAlternateTitleByGroupCompo(int groupId, int workCompo)
        {
            return Dao.GetUsersAlternateTitleByGroupCompo(groupId, workCompo);
        }

        public static DataSet GetUsersWithPermission(short permissionId)
        {
            return Dao.GetUsersWithPermission(permissionId);
        }

        public static bool HasHQTechAccount(int originUserId, string edipin)
        {
            return Dao.HasHQTechAccount(originUserId, edipin);
        }

        public static bool IsMemberPartOfAttachUnit(int userId, int attachUnit)
        {
            SqlDataStore dataSource = new SqlDataStore();
            bool returnValue = false;
            returnValue = (bool)dataSource.ExecuteScalar("core_user_sp_Belongs_To_Attach", userId, attachUnit);
            return returnValue;
        }

        public static void Logout(int userId)
        {
            Dao.Logout(userId);
        }

        public static DataSet SearchMemberData(int userId, string ssn, string lastName, string firstName, string middleName, int unitId, int rptView)
        {
            SqlDataStore dataSource = new SqlDataStore();
            DataSet ds = dataSource.ExecuteDataSet("core_user_sp_SearchMamberData", userId, ssn, lastName, firstName, middleName, unitId, rptView);
            return ds;
        }

        public static void SendAccountModifiedEmail(int adminId, int userId, String Applink, String Activity)
        {
            EmailService mailService = new EmailService();
            MailManager manager = new MailManager(mailService);
            AppUser user = Dao.GetById(userId);
            String otherRoles = user.AllRolesString();
            StringCollection toList = mailService.GetEmailListForUser(userId);
            //24--Status Changed Email
            manager.AddTemplate((int)LODTemplate.AccountModified, "", toList);
            manager.SetField("NAME", user.SignatureName);
            manager.SetField("ROLE", user.CurrentRoleName);
            manager.SetField("STATUS", user.StatusDescription);
            manager.SetField("OTHER_ROLES", otherRoles);
            manager.SetField("APP_LINK", Applink);
            manager.SetField("ACTIVITY", Activity);

            manager.SendAll();
        }

        public static void SendAccountRegisteredEmail(string compo, string username, string rolename, int unitId, String Applink)
        {
            EmailService mailService = new EmailService();
            MailManager manager = new MailManager(mailService);
            String sysAdminRoles = ((int)UserGroups.SystemAdministrator).ToString();

            StringCollection toList = mailService.GetDistributionListByRoles(compo, unitId, sysAdminRoles);
            //8 Registration notification
            manager.AddTemplate((int)LODTemplate.UserRegistered, "", toList);
            manager.SetField("USER_NAME", username);
            manager.SetField("USER_ROLE", rolename);
            manager.SetField("APP_LINK", Applink);
            manager.SendAll();
        }

        public static void SendModifyRequestEmail(string compo, string username, int unitId, String Applink)
        {
            EmailService mailService = new EmailService();
            MailManager manager = new MailManager(mailService);
            String sysAdminRoles = ((int)UserGroups.SystemAdministrator).ToString();
            StringCollection toList = mailService.GetDistributionListByRoles(compo, unitId, sysAdminRoles);
            //7 Mod request
            manager.AddTemplate((int)LODTemplate.AccountChangeRequest, "", toList);
            manager.SetField("USER_NAME", username);
            manager.SetField("APP_LINK", Applink);
            manager.SendAll();
        }

        public static void Update(AppUser user)
        {
            Dao.SaveOrUpdate(user);
        }

        public static void UpdateUserAlternateTitle(int userId, int groupId, string title)
        {
            Dao.UpdateUserAlternateTitle(userId, groupId, title);
        }

        public static void UpdateUserGroup(int roleId, short groupId, short status, int userId)
        {
            roleDao.UpdateGroup(roleId, groupId, status, userId);
        }
    }
}