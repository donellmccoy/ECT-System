using ALOD.Core.Domain.Workflow;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Core.Utils;
using NHibernate;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace ALOD.Data
{
    /// <summary>
    /// Data access object for managing <see cref="CaseLock"/> entities.
    /// Handles case locking operations to prevent concurrent modifications by multiple users.
    /// </summary>
    public class CaseLockDao : AbstractNHibernateDao<CaseLock, int>, ICaseLockDao
    {
        /// <summary>
        /// Clears all case locks from the system.
        /// </summary>
        public void ClearAllLocks()
        {
            SqlDataStore store = new SqlDataStore();
            string sql = "DELETE FROM core_workflowLocks";
            DbCommand cmd = store.GetSqlStringCommand(sql);
            store.ExecuteNonQuery(cmd);
        }

        /// <summary>
        /// Clears all case locks held by a specific user.
        /// </summary>
        /// <param name="userId">The user ID whose locks should be cleared.</param>
        public void ClearLocksForUser(int userId)
        {
            SqlDataStore store = new SqlDataStore();
            string sql = "DELETE FROM core_workflowLocks where userId = @userId";
            DbCommand cmd = store.GetSqlStringCommand(sql);
            store.AddInParameter(cmd, "@userId", System.Data.DbType.Int32, userId);
            store.ExecuteNonQuery(cmd);
        }

        /// <summary>
        /// Retrieves all case locks in the system.
        /// </summary>
        /// <returns>A DataSet containing all locks.</returns>
        public new DataSet GetAll()
        {
            SqlDataStore store = new SqlDataStore();
            return store.ExecuteDataSet("core_workflow_sp_GetAllLocks");
        }

        /// <summary>
        /// Retrieves a case lock by reference ID and module type.
        /// </summary>
        /// <param name="refId">The reference ID of the case.</param>
        /// <param name="module">The module type.</param>
        /// <returns>The <see cref="CaseLock"/> if found; otherwise, null.</returns>
        public CaseLock GetByReferenceId(int refId, byte module)
        {
            Check.Require(refId > 0, "Invalid reference id");
            Check.Require(module > 0, "Invalid module");

            ISession session = NHibernateSessionManager.Instance.GetSession();

            IList<CaseLock> list = session.CreateQuery(
                "from CaseLock lock where lock.ReferenceId = :REFID and lock.ModuleType = :MODULE")
                .SetInt32("REFID", refId)
                .SetByte("MODULE", module)
                .List<CaseLock>();

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
        /// Retrieves all case locks held by a specific user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <returns>A list of case locks held by the user.</returns>
        public IList<CaseLock> GetByUserId(int userId)
        {
            Check.Require(userId > 0, "Invalid UserId");

            ISession session = NHibernateSessionManager.Instance.GetSession();

            IList<CaseLock> list = session.CreateQuery(
                "from CaseLock lock where lock.UserId = :USERID")
                .SetInt32("USERID", userId)
                .List<CaseLock>();

            return list;
        }
    }
}