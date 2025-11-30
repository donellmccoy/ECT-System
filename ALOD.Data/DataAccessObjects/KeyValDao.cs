using ALOD.Core.Domain.Common.KeyVal;
using ALOD.Core.Interfaces.DAOInterfaces;
using System.Collections.Generic;
using System.Data;

namespace ALOD.Data
{
    /// <summary>
    /// Data access object for managing key-value pair entities.
    /// Handles operations for keys, key types, and key-value pairs including retrieval, insertion, updates, and deletion.
    /// Supports help keys, memo keys, and editable key types.
    /// </summary>
    public class KeyValDao : IKeyValDao
    {
        private SqlDataStore _dataSource;

        /// <summary>
        /// Gets the SQL data store instance for database operations.
        /// </summary>
        private SqlDataStore DataSource
        {
            get
            {
                if (_dataSource == null)
                {
                    _dataSource = new SqlDataStore();
                }
                return _dataSource;
            }
        }

        /// <summary>
        /// Deletes a key-value pair by its unique identifier.
        /// Executes stored procedure: core_KeyVal_sp_DeleteKeyValueById
        /// </summary>
        /// <param name="id">The unique identifier of the key-value pair to delete.</param>
        public void DeleteKeyValueById(int id)
        {
            DataSource.ExecuteNonQuery("core_KeyVal_sp_DeleteKeyValueById", id);
        }

        /// <summary>
        /// Retrieves all keys in the system.
        /// Executes stored procedure: core_KeyVale_sp_GetAllKeys
        /// </summary>
        /// <returns>A list of all <see cref="KeyValKey"/> objects.</returns>
        public IList<KeyValKey> GetAllKeys()
        {
            DataSet dSet = DataSource.ExecuteDataSet("core_KeyVale_sp_GetAllKeys");

            return GetKeys(dSet);
        }

        /// <summary>
        /// Retrieves all editable key types.
        /// Executes stored procedure: core_KeyVal_sp_GetEditableKeyTypes
        /// </summary>
        /// <returns>A list of editable <see cref="KeyValKeyType"/> objects.</returns>
        public IList<KeyValKeyType> GetEditableKeyTypes()
        {
            DataSet dSet = DataSource.ExecuteDataSet("core_KeyVal_sp_GetEditableKeyTypes");

            return GetKeyTypes(dSet);
        }

        /// <summary>
        /// Retrieves all help-related keys.
        /// Executes stored procedure: core_KeyVal_sp_GetHelpKeys
        /// </summary>
        /// <returns>A list of help <see cref="KeyValKey"/> objects.</returns>
        public IList<KeyValKey> GetHelpKeys()
        {
            return GetKeys(DataSource.ExecuteDataSet("core_KeyVal_sp_GetHelpKeys"));
        }

        /// <summary>
        /// Retrieves all keys of a specific key type.
        /// Executes stored procedure: core_KeyVal_sp_GetKeysUsingKeyType
        /// </summary>
        /// <param name="keyTypeID">The key type identifier.</param>
        /// <returns>A list of <see cref="KeyValKey"/> objects of the specified type.</returns>
        public IList<KeyValKey> GetKeysUsingKeyType(int keyTypeID)
        {
            return GetKeys(DataSource.ExecuteDataSet("core_KeyVal_sp_GetKeysUsingKeyType", keyTypeID));
        }

        /// <summary>
        /// Retrieves all key-value pairs for a specific key description.
        /// Executes stored procedure: core_KeyVal_sp_GetKeyValuesByKeyDescription
        /// </summary>
        /// <param name="keyDesc">The key description to search for.</param>
        /// <returns>A list of <see cref="KeyValValue"/> objects matching the description.</returns>
        public IList<KeyValValue> GetKeyValuesByKeyDesciption(string keyDesc)
        {
            DataSet dSet = DataSource.ExecuteDataSet("core_KeyVal_sp_GetKeyValuesByKeyDescription", keyDesc);

            return GetKeyValues(dSet);
        }

        /// <summary>
        /// Retrieves all key-value pairs for a specific key identifier.
        /// Executes stored procedure: core_KeyVal_sp_GetKeyValuesByKeyId
        /// </summary>
        /// <param name="keyId">The key identifier.</param>
        /// <returns>A list of <see cref="KeyValValue"/> objects for the specified key.</returns>
        public IList<KeyValValue> GetKeyValuesByKeyId(int keyId)
        {
            DataSet dSet = DataSource.ExecuteDataSet("core_KeyVal_sp_GetKeyValuesByKeyId", keyId);

            return GetKeyValues(dSet);
        }

        /// <summary>
        /// Retrieves all memo-related keys.
        /// Executes stored procedure: core_memo_sp_GetMemoKeys
        /// </summary>
        /// <returns>A list of memo <see cref="KeyValKey"/> objects.</returns>
        public IList<KeyValKey> GetMemoKeys()
        {
            return GetKeys(DataSource.ExecuteDataSet("core_memo_sp_GetMemoKeys"));
        }

        /// <summary>
        /// Inserts a new key-value pair.
        /// Executes stored procedure: core_KeyVal_sp_InsertKeyValue
        /// </summary>
        /// <param name="keyId">The key identifier.</param>
        /// <param name="valueDescription">The description of the value.</param>
        /// <param name="value">The actual value.</param>
        public void InsertKeyValue(int keyId, string valueDescription, string value)
        {
            DataSource.ExecuteNonQuery("core_KeyVal_sp_InsertKeyValue", keyId, valueDescription, value);
        }

        /// <summary>
        /// Updates an existing key-value pair by its unique identifier.
        /// Executes stored procedure: core_KeyVal_sp_UpdateKeyValueById
        /// </summary>
        /// <param name="id">The unique identifier of the key-value pair to update.</param>
        /// <param name="keyId">The key identifier.</param>
        /// <param name="valueDescription">The updated description of the value.</param>
        /// <param name="value">The updated value.</param>
        public void UpdateKeyValueById(int id, int keyId, string valueDescription, string value)
        {
            DataSource.ExecuteNonQuery("core_KeyVal_sp_UpdateKeyValueById", id, keyId, valueDescription, value);
        }

        private KeyValKey ExtractKey(DataRow row)
        {
            return new KeyValKey(int.Parse(row["KeyId"].ToString()), row["KeyDescription"].ToString(), ExtractKeyType(row));
        }

        private KeyValKeyType ExtractKeyType(DataRow row)
        {
            return new KeyValKeyType(int.Parse(row["KeyTypeId"].ToString()), row["KeyTypeName"].ToString());
        }

        private KeyValValue ExtractValue(DataRow row)
        {
            return new KeyValValue(int.Parse(row["Id"].ToString()), int.Parse(row["ValueId"].ToString()), row["Value"].ToString(), row["ValueDescription"].ToString(), ExtractKey(row));
        }

        private IList<KeyValKey> GetKeys(DataSet dSet)
        {
            if (dSet == null)
                return null;

            if (dSet.Tables.Count == 0)
                return null;

            DataTable dTable = dSet.Tables[0];

            List<KeyValKey> keys = new List<KeyValKey>();

            foreach (DataRow row in dTable.Rows)
            {
                KeyValKey key = ExtractKey(row);

                if (key != null)
                    keys.Add(key);
            }

            return keys;
        }

        private IList<KeyValKeyType> GetKeyTypes(DataSet dSet)
        {
            if (dSet == null)
                return null;

            if (dSet.Tables.Count == 0)
                return null;

            DataTable dTable = dSet.Tables[0];

            List<KeyValKeyType> keys = new List<KeyValKeyType>();

            foreach (DataRow row in dTable.Rows)
            {
                KeyValKeyType key = ExtractKeyType(row);

                if (key != null)
                    keys.Add(key);
            }

            return keys;
        }

        private IList<KeyValValue> GetKeyValues(DataSet dSet)
        {
            if (dSet == null)
                return null;

            if (dSet.Tables.Count == 0)
                return null;

            DataTable dTable = dSet.Tables[0];

            List<KeyValValue> values = new List<KeyValValue>();

            foreach (DataRow row in dTable.Rows)
            {
                KeyValValue value = ExtractValue(row);

                if (value != null)
                    values.Add(value);
            }

            return values;
        }
    }
}