using System;
using System.Data;
using ALOD.Data;
using ALODWebUtility.DataAccess;


namespace ALODWebUtility.Perms
{
    public class OnlineUserList
    {
        private DataSets.OnlineUsersDataTable _users;

        public DataSets.OnlineUsersDataTable GetUsers()
        {
            _users = new DataSets.OnlineUsersDataTable();
            SqlDataStore adapter = new SqlDataStore();
            adapter.ExecuteReader(UserReader, "core_user_sp_GetUsersOnline");
            return _users;
        }

        private void UserReader(SqlDataStore adapter, IDataReader reader)
        {
            // 0-userid, 1-userName, 2-roleName, 3-regionName, 4-loginTime
            DataSets.OnlineUsersRow row = _users.NewOnlineUsersRow();

            row.userId = adapter.GetInteger(reader, 0);
            row.userName = adapter.GetString(reader, 1);
            row.roleName = adapter.GetString(reader, 2);
            row.regionName = adapter.GetString(reader, 3);
            row.loginTime = adapter.GetDateTime(reader, 4);
            row.timeOnline = DateTime.Now.Subtract(row.loginTime);

            _users.Rows.Add(row);
        }
    }
}
