using ALOD.Core.Domain.Workflow;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Core.Utils;
using System;
using System.Collections.Generic;
using System.Data;

namespace ALOD.Data
{
    /// <summary>
    /// Data access object for managing <see cref="ALODPermission"/> entities.
    /// Handles permission-related operations including group permissions and document group associations.
    /// </summary>
    public class ALODPermissionDao : AbstractNHibernateDao<ALODPermission, int>, IALODPermissionDao
    {
        /// <summary>
        /// Gets a new instance of <see cref="SqlDataStore"/> for database operations.
        /// </summary>
        private SqlDataStore DataStore
        {
            get { return new SqlDataStore(); }
        }

        /// <summary>
        /// Retrieves the document group ID associated with a specific permission ID.
        /// </summary>
        /// <param name="permId">The permission ID to lookup.</param>
        /// <returns>The document group ID, or 0 if not found.</returns>
        public int GetDocGroupIdByPermId(int permId)
        {
            SqlDataStore store = new SqlDataStore();
            Object result = store.ExecuteScalar("core_permissions_sp_GetDocGroupIdByPermId", permId);

            if (result == null)
                return 0;
            else
                return (int)result;
        }

        /// <summary>
        /// Retrieves all permissions associated with a specific group.
        /// </summary>
        /// <param name="groupId">The group ID to retrieve permissions for.</param>
        /// <returns>A list of <see cref="ALODPermission"/> objects for the group.</returns>
        public IList<ALODPermission> GetPermissionsByGroupId(int groupId)
        {
            SqlDataStore store = new SqlDataStore();
            DataSet dSet = DataStore.ExecuteDataSet("core_permissions_sp_GetGroupPermissions", groupId);

            IList<ALODPermission> perms = new List<ALODPermission>();

            if (dSet == null || dSet.Tables.Count == 0)
                return perms;

            DataTable dTable = dSet.Tables[0];

            if (dTable == null)
                return perms;

            foreach (DataRow row in dTable.Rows)
            {
                ALODPermission perm = new ALODPermission();
                perm.Description = DataHelpers.GetStringFromDataRow("permDesc", row);
                perm.Name = DataHelpers.GetStringFromDataRow("permName", row);

                perms.Add(perm);
            }

            return perms;
        }

        /// <summary>
        /// Inserts a new document group association for a permission.
        /// </summary>
        /// <param name="permId">The permission ID.</param>
        /// <param name="docGroupId">The document group ID to associate.</param>
        public void InsertNewDocGroup(int permId, int docGroupId)
        {
            SqlDataStore store = new SqlDataStore();
            store.ExecuteNonQuery("core_permissions_sp_InsertDocGroup", permId, docGroupId);
        }

        /// <summary>
        /// Updates a document group association by replacing old values with new ones.
        /// </summary>
        /// <param name="oldPermId">The old permission ID.</param>
        /// <param name="oldDocGroupId">The old document group ID.</param>
        /// <param name="newPermId">The new permission ID.</param>
        /// <param name="newDocGroupId">The new document group ID.</param>
        public void UpdateDocGroup(int oldPermId, int oldDocGroupId, int newPermId, int newDocGroupId)
        {
            SqlDataStore store = new SqlDataStore();
            store.ExecuteNonQuery("core_permissions_sp_UpdateDocGroup", oldPermId, oldDocGroupId, newPermId, newDocGroupId);
        }
    }
}