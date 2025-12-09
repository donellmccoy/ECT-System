using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace ALOD.Data
{
    /// <summary>
    /// Batch type enumeration.
    /// </summary>
    public enum BatchType : byte
    {
        Entity = 1,
        DocType = 2
    }

    /// <summary>
    /// Batch data returned from queries.
    /// </summary>
    public class BatchData
    {
        public int BatchID { get; set; }
        public BatchType BatchType { get; set; }
        public DateTime DateCreated { get; set; }
        public string DocDescription { get; set; }
        public int DocTypeID { get; set; }
        public string DocTypeName { get; set; }
        public string EntityName { get; set; }
        public string Location { get; set; }
        public int PageCount { get; set; }
        public string UploadedBySubuserName { get; set; }
    }

    /// <summary>
    /// Keys for uploading a batch.
    /// </summary>
    public class BatchUploadKeys
    {
        public BatchType BatchType { get; set; }
        public int DocTypeID { get; set; }
        public string EntityName { get; set; }
        public string Location { get; set; }
    }

    /// <summary>
    /// Data access object for batch service operations.
    /// Provides methods for managing batch uploads and retrieving batch lists.
    /// </summary>
    public class BatchServiceDao : IDisposable
    {
        private readonly SqlDataStore _dataStore;
        private bool _disposed;

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the BatchServiceDao class.
        /// </summary>
        public BatchServiceDao()
        {
            _dataStore = new SqlDataStore();
        }

        #endregion

        #region Batch Operations

        /// <summary>
        /// Creates a GUID for batch upload URL generation.
        /// </summary>
        /// <param name="batchType">The type of batch.</param>
        /// <param name="location">The location identifier.</param>
        /// <param name="entityName">The entity name.</param>
        /// <param name="docTypeID">The document type ID.</param>
        /// <param name="userID">The user ID.</param>
        /// <param name="subuserID">The subuser ID.</param>
        /// <returns>A GUID that can be used to construct the batch upload URL.</returns>
        public Guid CreateBatchUploadGuid(BatchType batchType, string location, string entityName, int docTypeID, short userID, int subuserID)
        {
            Guid batchGuid = Guid.NewGuid();

            DbCommand cmd = _dataStore.GetStoredProcCommand("dsp_BatchGuid_Insert");
            _dataStore.AddInParameter(cmd, "@BatchGuid", DbType.Guid, batchGuid);
            _dataStore.AddInParameter(cmd, "@BatchTypeID", DbType.Byte, (byte)batchType);
            _dataStore.AddInParameter(cmd, "@Location", DbType.String, location ?? string.Empty);
            _dataStore.AddInParameter(cmd, "@EntityName", DbType.String, entityName ?? string.Empty);
            _dataStore.AddInParameter(cmd, "@DocTypeID", DbType.Int32, docTypeID);
            _dataStore.AddInParameter(cmd, "@UserID", DbType.Int16, userID);
            _dataStore.AddInParameter(cmd, "@SubuserID", DbType.Int32, subuserID);

            _dataStore.ExecuteNonQuery(cmd);

            return batchGuid;
        }

        /// <summary>
        /// Gets the list of batches for an entity.
        /// </summary>
        /// <param name="entityName">The entity name.</param>
        /// <param name="userID">The user ID requesting the list.</param>
        /// <param name="subuserID">The subuser ID requesting the list.</param>
        /// <returns>A list of batch data for the entity.</returns>
        public List<BatchData> GetEntityBatchList(string entityName, short userID, int subuserID)
        {
            var batches = new List<BatchData>();

            DbCommand cmd = _dataStore.GetStoredProcCommand("dsp_Entity_GetBatchList");
            _dataStore.AddInParameter(cmd, "@EntityName", DbType.String, entityName ?? string.Empty);
            _dataStore.AddInParameter(cmd, "@UserID", DbType.Int16, userID);
            _dataStore.AddInParameter(cmd, "@SubuserID", DbType.Int32, subuserID);

            _dataStore.ExecuteReader((sender, reader) =>
            {
                var batch = new BatchData
                {
                    BatchID = sender.GetInt32(reader, 0),
                    BatchType = (BatchType)sender.GetByte(reader, 1, 1),
                    DateCreated = sender.GetDateTime(reader, 2),
                    DocDescription = sender.GetString(reader, 3),
                    DocTypeID = sender.GetInt32(reader, 4),
                    DocTypeName = sender.GetString(reader, 5),
                    EntityName = sender.GetString(reader, 6),
                    Location = sender.GetString(reader, 7),
                    PageCount = sender.GetInt32(reader, 8),
                    UploadedBySubuserName = sender.GetString(reader, 9)
                };
                batches.Add(batch);
            }, cmd);

            return batches;
        }

        /// <summary>
        /// Creates a new batch record.
        /// </summary>
        /// <param name="uploadKeys">The batch upload keys.</param>
        /// <param name="userID">The user ID creating the batch.</param>
        /// <param name="subuserID">The subuser ID creating the batch.</param>
        /// <param name="clientIP">The client IP address.</param>
        /// <returns>The ID of the newly created batch.</returns>
        public int CreateBatch(BatchUploadKeys uploadKeys, short userID, int subuserID, string clientIP)
        {
            if (uploadKeys == null)
                throw new ArgumentNullException(nameof(uploadKeys));

            DbCommand cmd = _dataStore.GetStoredProcCommand("dsp_Batch_Insert");
            _dataStore.AddInParameter(cmd, "@BatchTypeID", DbType.Byte, (byte)uploadKeys.BatchType);
            _dataStore.AddInParameter(cmd, "@DocTypeID", DbType.Int32, uploadKeys.DocTypeID);
            _dataStore.AddInParameter(cmd, "@EntityName", DbType.String, uploadKeys.EntityName ?? string.Empty);
            _dataStore.AddInParameter(cmd, "@Location", DbType.String, uploadKeys.Location ?? string.Empty);
            _dataStore.AddInParameter(cmd, "@UserID", DbType.Int16, userID);
            _dataStore.AddInParameter(cmd, "@SubuserID", DbType.Int32, subuserID);
            _dataStore.AddInParameter(cmd, "@ClientIP", DbType.String, clientIP ?? string.Empty);
            _dataStore.AddOutParameter(cmd, "@BatchID", DbType.Int32, 4);

            _dataStore.ExecuteNonQuery(cmd);

            object result = cmd.Parameters["@BatchID"].Value;
            return result != DBNull.Value ? Convert.ToInt32(result) : 0;
        }

        /// <summary>
        /// Gets batch data by batch ID.
        /// </summary>
        /// <param name="batchID">The batch ID.</param>
        /// <param name="userID">The user ID.</param>
        /// <param name="subuserID">The subuser ID.</param>
        /// <returns>The batch data, or null if not found.</returns>
        public BatchData GetBatchById(int batchID, short userID, int subuserID)
        {
            BatchData batch = null;

            DbCommand cmd = _dataStore.GetStoredProcCommand("dsp_Batch_GetById");
            _dataStore.AddInParameter(cmd, "@BatchID", DbType.Int32, batchID);
            _dataStore.AddInParameter(cmd, "@UserID", DbType.Int16, userID);
            _dataStore.AddInParameter(cmd, "@SubuserID", DbType.Int32, subuserID);

            _dataStore.ExecuteReader((sender, reader) =>
            {
                batch = new BatchData
                {
                    BatchID = sender.GetInt32(reader, 0),
                    BatchType = (BatchType)sender.GetByte(reader, 1, 1),
                    DateCreated = sender.GetDateTime(reader, 2),
                    DocDescription = sender.GetString(reader, 3),
                    DocTypeID = sender.GetInt32(reader, 4),
                    DocTypeName = sender.GetString(reader, 5),
                    EntityName = sender.GetString(reader, 6),
                    Location = sender.GetString(reader, 7),
                    PageCount = sender.GetInt32(reader, 8),
                    UploadedBySubuserName = sender.GetString(reader, 9)
                };
            }, cmd);

            return batch;
        }

        /// <summary>
        /// Deletes a batch by ID.
        /// </summary>
        /// <param name="batchID">The batch ID to delete.</param>
        /// <param name="userID">The user ID performing the delete.</param>
        /// <param name="subuserID">The subuser ID performing the delete.</param>
        /// <param name="clientIP">The client IP address.</param>
        public void DeleteBatch(int batchID, short userID, int subuserID, string clientIP)
        {
            DbCommand cmd = _dataStore.GetStoredProcCommand("dsp_Batch_Delete");
            _dataStore.AddInParameter(cmd, "@BatchID", DbType.Int32, batchID);
            _dataStore.AddInParameter(cmd, "@UserID", DbType.Int16, userID);
            _dataStore.AddInParameter(cmd, "@SubuserID", DbType.Int32, subuserID);
            _dataStore.AddInParameter(cmd, "@ClientIP", DbType.String, clientIP ?? string.Empty);

            _dataStore.ExecuteNonQuery(cmd);
        }

        #endregion

        #region IDisposable Support

        /// <summary>
        /// Releases all resources used by this instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources if any
                }

                _disposed = true;
            }
        }

        #endregion
    }
}
