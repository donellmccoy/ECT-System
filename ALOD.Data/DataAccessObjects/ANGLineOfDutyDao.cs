using ALOD.Core.Domain.Modules.Lod;
using ALOD.Core.Domain.Workflow;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Core.Utils;
using System;

namespace ALOD.Data
{
    /// <summary>
    /// Data access object for managing <see cref="ANGLineOfDuty"/> entities.
    /// Provides Air National Guard-specific line of duty operations including access control, workflow management, and pending case counts.
    /// </summary>
    public class ANGLineOfDutyDao : AbstractNHibernateDao<ANGLineOfDuty, int>, ANGILineOfDutyDao
    {
        /// <summary>
        /// Retrieves the from and direction information for a Line of Duty case.
        /// </summary>
        /// <param name="refId">The reference ID of the LOD case.</param>
        /// <returns>A string containing from and direction information.</returns>
        public string GetFromAndDirection(int refId)
        {
            SqlDataStore store = new SqlDataStore();
            string result = store.ExecuteScalar("core_lod_sp_GetFromAndDirection", refId).ToString();

            return result;
        }

        /// <summary>
        /// Retrieves the count of pending LOD cases for a user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="sarc">Whether to include SARC cases.</param>
        /// <returns>The number of pending LOD cases.</returns>
        public int GetPendingCount(int userId, bool sarc)
        {
            //**Returns the New LOD Count**//
            SqlDataStore store = new SqlDataStore();
            return (int)store.ExecuteScalar("core_lod_sp_GetPendingCount_LODV3", userId, sarc);
        }

        //public int GetPendingLegacyLodCount(int userId, bool sarc)
        //{
        //    //**Returns the Legacy LOD Count**//
        //    SqlDataStore store = new SqlDataStore();
        //    return (int)store.ExecuteScalar("core_lod_sp_GetPendingCount", userId, sarc);
        //}
        /// <summary>
        /// Retrieves the count of pending informal officer (IO) LOD cases for a user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="sarc">Whether to include SARC cases.</param>
        /// <returns>The number of pending IO LOD cases.</returns>
        public int GetPendingIOCount(int userId, bool sarc)
        {
            //**Returns the New LOD Count**//
            SqlDataStore store = new SqlDataStore();
            return (int)store.ExecuteScalar("core_lod_sp_GetPendingIOCount", userId, sarc);
        }

        /// <summary>
        /// Determines the access level a user has for a specific LOD case.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="refId">The reference ID of the LOD case.</param>
        /// <returns>The access level (None, ReadOnly, or ReadWrite).</returns>
        public PageAccess.AccessLevel GetUserAccess(int userId, int refId)
        {
            SqlDataStore store = new SqlDataStore();
            int access = (int)store.ExecuteScalar("lod_sp_UserHasAccess", userId, refId, ModuleType.LOD);

            switch (access)
            {
                case 0:
                    return PageAccess.AccessLevel.None;

                case 1:
                    return PageAccess.AccessLevel.ReadOnly;

                case 2:
                    return PageAccess.AccessLevel.ReadWrite;

                default:
                    return PageAccess.AccessLevel.None;
            }
        }

        /// <summary>
        /// Retrieves the workflow ID for a given LOD case.
        /// </summary>
        /// <param name="refid">The reference ID of the LOD case.</param>
        /// <returns>The workflow ID.</returns>
        public int GetWorkflow(int refid)
        {
            SqlDataStore store = new SqlDataStore();
            return (int)store.ExecuteScalar("core_lod_sp_GetWorkflow", refid);
        }

        /// <summary>
        /// Saves an ANG Line of Duty entity with validation.
        /// </summary>
        /// <param name="entity">The <see cref="ANGLineOfDuty"/> entity to save.</param>
        /// <returns>The saved entity.</returns>
        /// <exception cref="PreconditionException">Thrown when required fields are missing or invalid.</exception>
        public override ANGLineOfDuty Save(ANGLineOfDuty entity)
        {
            Check.Require(entity.MemberName.Length > 0, "Member name is required");
            Check.Require(entity.MemberSSN.Length == 9, "Invalid SSN");
            Check.Require(entity.MemberCompo.Length == 1, "Invalid Compo");
            Check.Require(entity.MemberUnit.Length > 0, "Member unit is required");
            Check.Require(entity.MemberUnitId > 0, "Member unit id is required");
            Check.Require(entity.MemberRank.Id > 0, "Member grade is required");
            Check.Require(entity.MemberDob.HasValue, "Member DOB is required");

            Check.Require(entity.Workflow > 0, "Workflow cannot be 0");
            Check.Require(entity.CreatedBy > 0, "Created By is required");
            Check.Require(entity.Status > 0, "Status cannot be 0");
            Check.Require(entity.ModifiedBy > 0, "Modified by is required");

            if (entity.CreatedDate.Ticks == 0)
                entity.CreatedDate = DateTime.Now;

            if (entity.ModifiedDate.Ticks == 0)
                entity.ModifiedDate = DateTime.Now;

            entity.CaseId = String.Empty;
            return base.Save(entity);
        }
    }
}