using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using ALOD.Data;
using ALODWebUtility.Common;
using ALODWebUtility.DataAccess;


namespace ALODWebUtility.Perms
{
    [Serializable]
    public class PermissionList : IList<Permission>
    {
        protected List<Permission> _perms;
        protected SqlDataStore sqDataStore;

        public PermissionList()
        {
            _perms = new List<Permission>();
        }

        protected SqlDataStore DataStore
        {
            get
            {
                if (sqDataStore == null)
                {
                    sqDataStore = new SqlDataStore();
                }

                return sqDataStore;
            }
        }

        /// <summary>
        /// Assigns the current permission list to the specified group
        /// </summary>
        /// <param name="groupId">The ID of the group</param>
        /// <remarks></remarks>
        public void AssignToGroup(short groupId)
        {
            if (Count == 0)
            {
                return;
            }

            XMLString xml = new XMLString("list");

            foreach (Permission perm in _perms)
            {
                xml.BeginElement("item");
                xml.WriteAttribute("permId", perm.Id.ToString());
                xml.EndElement();
            }

            // now pass it to SQL
            string st = xml.ToString();
            DataStore.ExecuteNonQuery("core_group_sp_UpdateGroupPermissions", groupId, xml.Value);
        }

        /// <summary>
        /// Assigns the current permission list to the specified user
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <remarks></remarks>
        public void AssignToUser(short userId)
        {
            XMLString xml = new XMLString("list");

            foreach (Permission perm in _perms)
            {
                xml.BeginElement("item");
                xml.WriteAttribute("permId", perm.Id.ToString());
                xml.WriteAttribute("status", perm.Status);
                xml.EndElement();
            }

            // now pass it to SQL
            string st = xml.ToString();
            DataStore.ExecuteNonQuery("core_permissions_sp_UpdateUserPermissions", userId, xml.Value);
        }

        public bool DeletePermission(int id)
        {
            int count = DataStore.ExecuteNonQuery("core_permissions_sp_Delete", id);
            return (count > 0);
        }

        public Permission Find(int permId)
        {
            foreach (Permission perm in _perms)
            {
                if (perm.Id == permId)
                {
                    return perm;
                }
            }

            return null;
        }

        public DataSets.PermissionDataTable GetAll()
        {
            _perms.Clear();
            DataStore.ExecuteReader(PermReader, "core_permissions_sp_GetALL");
            return this.ToDataSet();
        }

        public DataSets.PermissionDataTable GetByGroupId(int groupId)
        {
            _perms.Clear();
            DataStore.ExecuteReader(PermReader, "core_permissions_sp_GetByGroup", groupId);
            return this.ToDataSet();
        }

        public DataSets.PermissionDataTable GetByUserId(int userId)
        {
            _perms.Clear();
            DataStore.ExecuteReader(PermReader, "core_permissions_sp_GetByUserId", userId);
            return this.ToDataSet();
        }

        public DataSets.PermissionDataTable GetByUserName(string userName)
        {
            _perms.Clear();
            DataStore.ExecuteReader(PermReader, "core_permissions_sp_GetByUserName", userName);
            return this.ToDataSet();
        }

        public DataSets.PermissionDataTable GetUserAssignable()
        {
            _perms.Clear();
            DataStore.ExecuteReader(PermReader, "core_permissions_sp_GetUserAssignable");
            return this.ToDataSet();
        }

        public PermissionList GetUserPermissions(int userId)
        {
            _perms.Clear();
            DataStore.ExecuteReader(UserPermReader, "core_permissions_sp_GetUserPerms", userId);
            return this;
        }

        public bool InsertPermission(string name, string description, bool exclude)
        {
            int count = DataStore.ExecuteNonQuery("core_permissions_sp_Insert", name, description, exclude);
            return (count > 0);
        }

        public override string ToString()
        {
            StringBuilder buffer = new StringBuilder();

            foreach (Permission perm in _perms)
            {
                buffer.Append(perm.Name + ",");
            }

            if (buffer.Length > 0)
            {
                buffer = buffer.Remove(buffer.Length - 1, 1);
            }

            return buffer.ToString();
        }

        public bool UpdatePermission(int id, string name, string description, bool exclude)
        {
            int count = DataStore.ExecuteNonQuery("core_permissions_sp_Update", id, name, description, exclude);
            return (count > 0);
        }

        protected void PermReader(SqlDataStore adapter, IDataReader reader)
        {
            Permission perm = new Permission();
            perm.Id = adapter.GetNumber(reader, 0);
            perm.Name = adapter.GetString(reader, 1);
            perm.Description = adapter.GetString(reader, 2);
            perm.Allowed = adapter.GetBoolean(reader, 3);
            perm.Exclude = adapter.GetBoolean(reader, 4);
            _perms.Add(perm);
        }

        protected DataSets.PermissionDataTable ToDataSet()
        {
            DataSets.PermissionDataTable data = new DataSets.PermissionDataTable();
            DataSets.PermissionRow row;

            foreach (Permission perm in _perms)
            {
                row = data.NewPermissionRow();
                perm.ToDataRow(row);
                data.Rows.Add(row);
            }

            return data;
        }

        protected void UserPermReader(SqlDataStore adapter, IDataReader reader)
        {
            Permission perm = new Permission();
            perm.Id = adapter.GetInt16(reader, 0);
            perm.Name = adapter.GetString(reader, 1);
            perm.Description = adapter.GetString(reader, 2);
            perm.Status = adapter.GetString(reader, 3);
            perm.Exclude = adapter.GetBoolean(reader, 4);
            _perms.Add(perm);
        }

        #region IList

        public int Count
        {
            get { return _perms.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public Permission this[int index]
        {
            get { return _perms[index]; }
            set { _perms[index] = value; }
        }

        public void Add(Permission item)
        {
            _perms.Add(item);
        }

        public void Clear()
        {
            _perms.Clear();
        }

        public bool Contains(Permission item)
        {
            return _perms.Contains(item);
        }

        public void CopyTo(Permission[] array, int arrayIndex)
        {
            _perms.CopyTo(array, arrayIndex);
        }

        public IEnumerator<Permission> GetEnumerator()
        {
            return _perms.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _perms.GetEnumerator();
        }

        public int IndexOf(Permission item)
        {
            return _perms.IndexOf(item);
        }

        public void Insert(int index, Permission item)
        {
            _perms.Insert(index, item);
        }

        public bool Remove(Permission item)
        {
            return _perms.Remove(item);
        }

        public void RemoveAt(int index)
        {
            _perms.RemoveAt(index);
        }

        #endregion
    }
}
