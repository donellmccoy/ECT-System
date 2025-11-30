using ALOD.Core.Domain.Log;
using System;
using System.Data;

namespace ALOD.Data.Services
{
    /// <summary>
    /// Service for managing audit logs and change sets.
    /// Implements ILogDao to provide logging functionality for tracking changes to entities.
    /// </summary>
    public class LogService : ILogDao
    {
        private SqlDataStore dataSource;

        public LogService()
        {
            dataSource = new SqlDataStore();
        }

        private delegate void D(SqlDataStore source, IDataReader reader);

        /// <summary>
        /// Retrieves the change set associated with a specific log entry.
        /// </summary>
        /// <param name="logId">The ID of the log entry.</param>
        /// <returns>A ChangeSet containing all change rows for the specified log ID.</returns>
        public ChangeSet GetChangeSetByLogId(int logId)
        {
            ChangeSet set = new ChangeSet();

            ALOD.Data.SqlDataStore.RowReader del = (SqlDataStore source, IDataReader reader) =>
            {
                ChangeRow row = new ChangeRow();
                row.Id = source.GetNumber(reader, 0, 0);
                row.Section = source.GetString(reader, 1, "");
                row.Field = source.GetString(reader, 2, "");
                row.OldVal = source.GetString(reader, 3, "");
                row.NewVal = source.GetString(reader, 4, "");

                set.Add(row);
            };

            dataSource.ExecuteReader(del, "core_log_sp_GetChangeSetByLogId", logId);
            return set;
        }

        /// <summary>
        /// Retrieves all change sets associated with a specific reference ID and module type.
        /// </summary>
        /// <param name="refId">The reference ID of the entity.</param>
        /// <param name="moduleType">The module type identifier.</param>
        /// <returns>A ChangeSet containing all change rows for the specified reference ID and module.</returns>
        public ChangeSet GetChangeSetByReferenceId(int refId, byte moduleType)
        {
            ChangeSet set = new ChangeSet();

            ALOD.Data.SqlDataStore.RowReader del = (SqlDataStore source, IDataReader reader) =>
            {
                ChangeRow row = new ChangeRow();
                row.Id = source.GetNumber(reader, 0, 0);
                row.Section = source.GetString(reader, 1, "");
                row.Field = source.GetString(reader, 2, "");
                row.OldVal = source.GetString(reader, 3, "");
                row.NewVal = source.GetString(reader, 4, "");

                set.Add(row);
            };

            dataSource.ExecuteReader(del, "core_log_sp_GetChangeSetsByRefId", refId, moduleType);
            return set;
        }

        /// <summary>
        /// Retrieves all change sets made by a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A ChangeSet containing all change rows made by the specified user.</returns>
        public ChangeSet GetChangeSetByUserId(int userId)
        {
            ChangeSet set = new ChangeSet();

            ALOD.Data.SqlDataStore.RowReader del = (SqlDataStore source, IDataReader reader) =>
            {
                ChangeRow row = new ChangeRow();
                row.Id = source.GetNumber(reader, 0, 0);
                row.Section = source.GetString(reader, 1, "");
                row.Field = source.GetString(reader, 2, "");
                row.OldVal = source.GetString(reader, 3, "");
                row.NewVal = source.GetString(reader, 4, "");

                set.Add(row);
            };

            dataSource.ExecuteReader(del, "core_log_sp_GetChangeSetByUserId", userId);
            return set;
        }

        /// <summary>
        /// Retrieves the most recent change set for the specified ID.
        /// </summary>
        /// <param name="id">The entity ID.</param>
        /// <returns>A ChangeSet containing the last set of changes.</returns>
        /// <exception cref="NotImplementedException">This method is not yet implemented.</exception>
        public ChangeSet GetLastChangeSet(int id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Saves a change set to the database associated with a specific log entry.
        /// </summary>
        /// <param name="logId">The ID of the log entry to associate the changes with.</param>
        /// <param name="changes">The ChangeSet to save.</param>
        /// <exception cref="NotImplementedException">This method is not yet implemented.</exception>
        public void SaveChangeSet(int logId, ChangeSet changes)
        {
            throw new NotImplementedException();
        }
    }
}