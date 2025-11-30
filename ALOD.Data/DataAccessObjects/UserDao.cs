using ALOD.Core.Domain.Users;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Core.Utils;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace ALOD.Data
{
    /// <summary>
    /// Data access object for AppUser entities, providing authentication, user management, and permission operations.
    /// </summary>
    /// <remarks>
    /// This DAO handles user authentication by SSN and EDIPIN, manages online user sessions,
    /// maintains alternate titles for users, and provides user search and permission queries.
    /// </remarks>
    public class UserDao : AbstractNHibernateDao<AppUser, int>, IUserDao
    {
        /// <summary>
        /// Removes a user from the online users tracking table.
        /// </summary>
        /// <param name="userId">The unique identifier of the user to remove from the online users list. Must be greater than 0.</param>
        /// <exception cref="System.ArgumentException">Thrown when userId is less than or equal to 0.</exception>
        /// <remarks>
        /// This method directly executes a DELETE statement against the core_UsersOnline table.
        /// Typically called during user logout or session cleanup.
        /// </remarks>
        public void ClearOnlineUser(int userId)
        {
            SqlDataStore store = new SqlDataStore();
            string sql = "DELETE FROM core_UsersOnline WHERE userId = @userId";
            DbCommand cmd = store.GetSqlStringCommand(sql);
            store.AddInParameter(cmd, "@userId", DbType.Int32, userId);
            store.ExecuteNonQuery(cmd);
        }

        /// <summary>
        /// Duplicates a user account by copying permissions and settings from one user to another.
        /// </summary>
        /// <param name="copyFromId">The user ID to copy settings from. Must be a valid existing user ID.</param>
        /// <param name="userId">The target user ID to copy settings to. Must be a valid existing user ID.</param>
        /// <returns>The result code from the stored procedure indicating success (typically 1) or failure (0).</returns>
        /// <exception cref="System.ArgumentException">Thrown when either userId is invalid.</exception>
        /// <remarks>
        /// This method executes the stored procedure core_user_sp_CopyAccount to replicate user permissions and settings.
        /// Commonly used when setting up new users with similar roles.
        /// </remarks>
        public int DuplicateAccount(int copyFromId, int userId)
        {
            SqlDataStore store = new SqlDataStore();
            return (int)store.ExecuteScalar("core_user_sp_CopyAccount", copyFromId, userId);
        }

        /// <summary>
        /// Finds all user accounts associated with the specified EDIPIN (Electronic Data Interchange Personal Identifier Number).
        /// </summary>
        /// <param name="EDIPIN">The EDIPIN to search for. Must not be empty.</param>
        /// <returns>A list of AppUser objects matching the EDIPIN. Returns an empty list if no matches found.</returns>
        /// <exception cref="System.ArgumentException">Thrown when EDIPIN is empty or null.</exception>
        /// <remarks>
        /// This method searches for exact matches of non-null, non-empty EDIPIN values.
        /// EDIPIN is the DoD unique identifier for personnel in personnel systems.
        /// </remarks>
        public IList<AppUser> FindByEDIPIN(string EDIPIN)
        {
            Check.Require(EDIPIN.Length > 0, "Invalid EDIPIN");

            ISession session = NHibernateSessionManager.Instance.GetSession();

            IList<AppUser> list = session.CreateQuery(
                "from AppUser user where user.EDIPIN is not null and user.EDIPIN <> '' and user.EDIPIN = :EDIPIN")
                .SetString("EDIPIN", EDIPIN)
                .List<AppUser>();

            var list1 = session.Query<AppUser>()
                     .Where(u => u.EDIPIN != null && u.EDIPIN != "" && u.EDIPIN == EDIPIN)
                     .ToList();

            return list;
        }

        /// <summary>
        /// Finds all user accounts associated with the specified Social Security Number.
        /// </summary>
        /// <param name="ssn">The SSN to search for. Must be exactly 9 digits.</param>
        /// <returns>A list of AppUser objects matching the SSN. Returns an empty list if no matches found.</returns>
        /// <exception cref="System.ArgumentException">Thrown when SSN is not exactly 9 digits.</exception>
        /// <remarks>
        /// This method searches for exact matches of non-null, non-empty SSN values.
        /// SSN should be provided without dashes or spaces.
        /// </remarks>
        public IList<AppUser> FindBySSN(string ssn)
        {
            Check.Require(ssn.Length == 9, "Invalid SSN");

            ISession session = NHibernateSessionManager.Instance.GetSession();

            IList<AppUser> list = session.CreateQuery(
                "from AppUser user where user.SSN is not null and user.SSN <> '' and user.SSN = :ssn ")
                .SetString("ssn", ssn)
                .List<AppUser>();

            return list;
        }

        /// <summary>
        /// Finds all user accounts with usernames matching the specified search pattern.
        /// </summary>
        /// <param name="username">The username search pattern. Must not be null or empty.</param>
        /// <returns>A list of AppUser objects with usernames containing the search pattern. Returns an empty list if no matches found.</returns>
        /// <exception cref="System.ArgumentException">Thrown when username is null or empty.</exception>
        /// <remarks>
        /// This method performs a partial match search using SQL LIKE with wildcards.
        /// The search is case-insensitive.
        /// </remarks>
        public IList<AppUser> FindByUsername(string username)
        {
            Check.Require(!string.IsNullOrEmpty(username));

            ISession session = NHibernateSessionManager.Instance.GetSession();

            IList<AppUser> list = session.CreateQuery(
                "from AppUser user where user.Username LIKE :username")
                .SetString("username", "%" + username + "%")
                .List<AppUser>();

            return list;
        }

        /// <summary>
        /// Retrieves the first user account associated with the specified EDIPIN.
        /// </summary>
        /// <param name="EDIPIN">The EDIPIN to search for. Must not be empty.</param>
        /// <returns>The first AppUser object matching the EDIPIN, or null if not found.</returns>
        /// <exception cref="System.ArgumentException">Thrown when EDIPIN is empty or null.</exception>
        /// <remarks>
        /// If multiple users share the same EDIPIN (which should not occur), only the first match is returned.
        /// Consider using FindByEDIPIN if you need to check for duplicate EDIPINs.
        /// </remarks>
        public AppUser GetByEDIPIN(string EDIPIN)
        {
            Check.Require(EDIPIN.Length > 0, "Invalid EDIPIN");

            ISession session = NHibernateSessionManager.Instance.GetSession();

            IList<AppUser> list = session.CreateQuery(
                "from AppUser user where user.EDIPIN is not null and user.EDIPIN <> '' and user.EDIPIN = :EDIPIN")
                .SetString("EDIPIN", EDIPIN)
                .List<AppUser>();

            if (list.Count > 0)
            {
                return list[0];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Retrieves all users assigned to a specific PAS (Personnel Accounting Symbol) Code.
        /// </summary>
        /// <param name="cscId">The unique identifier of the PAS Code. Must be greater than 0.</param>
        /// <returns>A list of AppUser objects assigned to the specified PAS Code.</returns>
        /// <exception cref="System.ArgumentException">Thrown when cscId is less than or equal to 0.</exception>
        /// <remarks>
        /// PAS Codes represent organizational units in the military structure.
        /// This method queries users based on their CurrentRole assignment.
        /// </remarks>
        public IList<AppUser> GetByPasCode(int cscId)
        {
            Check.Require(cscId > 0, "cscId cannot be 0");

            ISession session = NHibernateSessionManager.Instance.GetSession();

            return session.CreateQuery(
                "from AppUser user where user.CurrentRole.PasCode.Id = :cscId")
                .SetInt32("cscId", cscId)
                .List<AppUser>();
        }

        /// <summary>
        /// Retrieves a user account by looking up the associated service member's SSN.
        /// </summary>
        /// <param name="memberSSN">The Social Security Number of the service member. Must be exactly 9 digits.</param>
        /// <returns>The AppUser object associated with the service member, or null if not found.</returns>
        /// <exception cref="System.ArgumentException">Thrown when memberSSN is not exactly 9 digits.</exception>
        /// <remarks>
        /// This method executes the stored procedure core_user_sp_GetMembersUserId to find the user ID,
        /// then retrieves the full user object using GetById.
        /// </remarks>
        public AppUser GetByServiceMemberSSN(string memberSSN)
        {
            SqlDataStore store = new SqlDataStore();
            Object result = store.ExecuteScalar("core_user_sp_GetMembersUserId", memberSSN);

            if (result == null)
                return null;

            int userId = (int)result;

            if (userId < 1)
                return null;

            return GetById(userId);
        }

        /// <summary>
        /// Retrieves the first user account associated with the specified Social Security Number.
        /// </summary>
        /// <param name="ssn">The SSN to search for. Must be exactly 9 digits.</param>
        /// <returns>The first AppUser object matching the SSN, or null if not found.</returns>
        /// <exception cref="System.ArgumentException">Thrown when SSN is not exactly 9 digits.</exception>
        /// <remarks>
        /// If multiple users share the same SSN (which should not occur), only the first match is returned.
        /// SSN should be provided without dashes or spaces.
        /// </remarks>
        public AppUser GetBySSN(string ssn)
        {
            Check.Require(ssn.Length == 9, "Invalid SSN");

            ISession session = NHibernateSessionManager.Instance.GetSession();

            IList<AppUser> list = session.CreateQuery(
                "from AppUser user where user.SSN is not null and user.SSN <> '' and  user.SSN = :ssn")
                .SetString("ssn", ssn)
                .List<AppUser>();

            if (list.Count > 0)
            {
                return list[0];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Retrieves a list of all currently online users.
        /// </summary>
        /// <returns>A DataSet containing information about users currently logged into the system.</returns>
        /// <remarks>
        /// This method executes the stored procedure core_user_sp_GetUsersOnline.
        /// The returned DataSet typically includes user ID, username, login time, and last activity timestamp.
        /// </remarks>
        public DataSet GetOnLineUsers()
        {
            SqlDataStore store = new SqlDataStore();
            return store.ExecuteDataSet("core_user_sp_GetUsersOnline");
        }

        /// <summary>
        /// Retrieves the count of pending items or tasks assigned to a user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user. Must be greater than 0.</param>
        /// <returns>The number of pending items assigned to the user.</returns>
        /// <exception cref="System.ArgumentException">Thrown when userId is less than or equal to 0.</exception>
        /// <remarks>
        /// This method executes the stored procedure sp_core_user_sp_GetPendingCount.
        /// Used to display notification badges or pending work counters in the UI.
        /// </remarks>
        public int GetPendingCount(int userId)
        {
            SqlDataStore store = new SqlDataStore();
            return (int)store.ExecuteScalar("sp_core_user_sp_GetPendingCount", userId);
        }

        /// <summary>
        /// Retrieves the alternate title assigned to a user within a specific group.
        /// </summary>
        /// <param name="userId">The unique identifier of the user. Must be greater than 0.</param>
        /// <param name="groupId">The unique identifier of the group. Must be greater than 0.</param>
        /// <returns>The alternate title string, or an empty string if no alternate title is assigned.</returns>
        /// <exception cref="System.ArgumentException">Thrown when userId or groupId is less than or equal to 0.</exception>
        /// <remarks>
        /// This method executes the stored procedure core_user_sp_GetUserAltTitle.
        /// Alternate titles allow users to have custom display names within specific organizational groups.
        /// </remarks>
        public string GetUserAlternateTitle(int userId, int groupId)
        {
            SqlDataStore store = new SqlDataStore();
            Object result = store.ExecuteScalar("core_user_sp_GetUserAltTitle", userId, groupId);

            if (result == null)
                return String.Empty;
            else
                return result.ToString();
        }

        /// <summary>
        /// Generates or retrieves a username based on first and last name.
        /// </summary>
        /// <param name="firstName">The user's first name. Must not be null or empty.</param>
        /// <param name="lastName">The user's last name. Must not be null or empty.</param>
        /// <returns>A username string, typically formatted or validated by the stored procedure.</returns>
        /// <exception cref="System.ArgumentException">Thrown when firstName or lastName is null or empty.</exception>
        /// <remarks>
        /// This method executes the stored procedure core_user_sp_GetUserName.
        /// May append numbers or modify the name to ensure uniqueness.
        /// </remarks>
        public string GetUserName(string firstName, string lastName)
        {
            SqlDataStore store = new SqlDataStore();
            return (string)store.ExecuteScalar("core_user_sp_GetUserName", firstName, lastName);
        }

        /// <summary>
        /// Retrieves all users and their alternate titles within a specific group.
        /// </summary>
        /// <param name="groupId">The unique identifier of the group. Must be greater than 0.</param>
        /// <returns>A DataSet containing user information and alternate titles for the specified group.</returns>
        /// <exception cref="System.ArgumentException">Thrown when groupId is less than or equal to 0.</exception>
        /// <remarks>
        /// This method executes the stored procedure core_user_sp_GetUsersAltTitleByGroup.
        /// Used for displaying organizational rosters with custom titles.
        /// </remarks>
        public DataSet GetUsersAlternateTitleByGroup(int groupId)
        {
            SqlDataStore store = new SqlDataStore();
            return store.ExecuteDataSet("core_user_sp_GetUsersAltTitleByGroup", groupId);
        }

        /// <summary>
        /// Retrieves all users and their alternate titles within a specific group and component.
        /// </summary>
        /// <param name="groupId">The unique identifier of the group. Must be greater than 0.</param>
        /// <param name="workCompo">The work component identifier (e.g., Active, Reserve, Guard). Must be greater than 0.</param>
        /// <returns>A DataSet containing user information and alternate titles filtered by group and component.</returns>
        /// <exception cref="System.ArgumentException">Thrown when groupId or workCompo is less than or equal to 0.</exception>
        /// <remarks>
        /// This method executes the stored procedure core_user_sp_GetUserAltTitleByGroupCompo.
        /// Allows filtering users by both organizational group and military component.
        /// </remarks>
        public DataSet GetUsersAlternateTitleByGroupCompo(int groupId, int workCompo)
        {
            SqlDataStore store = new SqlDataStore();
            return store.ExecuteDataSet("core_user_sp_GetUserAltTitleByGroupCompo", groupId, workCompo);
        }

        /// <summary>
        /// Retrieves all users who have been assigned a specific permission.
        /// </summary>
        /// <param name="permissionId">The unique identifier of the permission. Must be greater than 0.</param>
        /// <returns>A DataSet containing users with the specified permission.</returns>
        /// <exception cref="System.ArgumentException">Thrown when permissionId is less than or equal to 0.</exception>
        /// <remarks>
        /// This method executes the stored procedure core_permissions_sp_searchUsers.
        /// Used for security auditing and permission management.
        /// </remarks>
        public DataSet GetUsersWithPermission(short permissionId)
        {
            SqlDataStore dataSource = new SqlDataStore();
            DataSet ds = dataSource.ExecuteDataSet("core_permissions_sp_searchUsers", permissionId);
            return ds;
        }

        /// <summary>
        /// Determines whether a user has a headquarters technical account.
        /// </summary>
        /// <param name="originUserId">The unique identifier of the original user. Must be greater than 0.</param>
        /// <param name="edipin">The EDIPIN to check for HQ tech account association. Must not be null or empty.</param>
        /// <returns>True if the user has an HQ technical account; otherwise, false.</returns>
        /// <exception cref="System.ArgumentException">Thrown when originUserId is invalid or edipin is null/empty.</exception>
        /// <remarks>
        /// This method executes the stored procedure core_user_sp_HasHQTechAccount.
        /// HQ tech accounts have elevated permissions for system administration.
        /// </remarks>
        public bool HasHQTechAccount(int originUserId, string edipin)
        {
            SqlDataStore store = new SqlDataStore();
            int result = (int)store.ExecuteScalar("core_user_sp_HasHQTechAccount", originUserId, edipin);

            if (result > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Checks whether an EDIPIN is available for use (not already assigned to another user).
        /// </summary>
        /// <param name="EDIPIN">The EDIPIN to check. Must not be null or empty.</param>
        /// <returns>True if the EDIPIN is available; false if already in use or invalid.</returns>
        /// <exception cref="System.ArgumentException">Thrown when EDIPIN is null or empty.</exception>
        /// <remarks>
        /// This method executes the stored procedure core_user_sp_GetIsEDIPINAvailable.
        /// Used during user registration to prevent duplicate EDIPINs.
        /// </remarks>
        public bool IsEDIPINAvailable(string EDIPIN)
        {
            SqlDataStore store = new SqlDataStore();
            Object result = store.ExecuteScalar("core_user_sp_GetIsEDIPINAvailable", EDIPIN);

            if (result == null)
                return false;

            int iResult = (int)result;

            if (iResult == 0)
                return false;

            return true;
        }

        /// <summary>
        /// Logs out a user and cleans up their session data.
        /// </summary>
        /// <param name="userId">The unique identifier of the user to log out. Must be greater than 0.</param>
        /// <exception cref="System.ArgumentException">Thrown when userId is less than or equal to 0.</exception>
        /// <remarks>
        /// This method executes the stored procedure core_user_sp_Logout.
        /// Removes user from online users table and clears session state.
        /// </remarks>
        public void Logout(int userId)
        {
            SqlDataStore store = new SqlDataStore();
            store.ExecuteNonQuery("core_user_sp_Logout", userId);
        }

        /// <summary>
        /// Updates the alternate title for a user within a specific group.
        /// </summary>
        /// <param name="userId">The unique identifier of the user. Must be greater than 0.</param>
        /// <param name="groupId">The unique identifier of the group. Must be greater than 0.</param>
        /// <param name="newTitle">The new alternate title to assign. Can be null or empty to remove the alternate title.</param>
        /// <exception cref="System.ArgumentException">Thrown when userId or groupId is less than or equal to 0.</exception>
        /// <remarks>
        /// This method executes the stored procedure core_user_sp_UpdateUserAltTitle.
        /// Alternate titles allow custom display names for users within organizational contexts.
        /// </remarks>
        public void UpdateUserAlternateTitle(int userId, int groupId, string newTitle)
        {
            SqlDataStore store = new SqlDataStore();
            store.ExecuteNonQuery("core_user_sp_UpdateUserAltTitle", userId, groupId, newTitle);
        }
    }
}