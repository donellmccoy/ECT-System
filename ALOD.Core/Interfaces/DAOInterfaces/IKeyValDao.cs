using ALOD.Core.Domain.Common.KeyVal;
using System.Collections.Generic;

namespace ALOD.Core.Interfaces.DAOInterfaces
{
    public interface IKeyValDao
    {
        void DeleteKeyValueById(int id);

        IList<KeyValKey> GetAllKeys();

        IList<KeyValKeyType> GetEditableKeyTypes();

        IList<KeyValKey> GetHelpKeys();

        IList<KeyValKey> GetKeysUsingKeyType(int keyTypeID);

        IList<KeyValValue> GetKeyValuesByKeyDesciption(string keyDesc);

        IList<KeyValValue> GetKeyValuesByKeyId(int keyId);

        IList<KeyValKey> GetMemoKeys();

        void InsertKeyValue(int keyId, string valueDescription, string value);

        void UpdateKeyValueById(int id, int keyId, string valueDescription, string value);
    }
}