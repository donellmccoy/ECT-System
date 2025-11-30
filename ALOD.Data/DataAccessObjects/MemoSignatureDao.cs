using ALOD.Core.Domain.Common;
using ALOD.Core.Interfaces;
using ALOD.Core.Utils;
using System.Data;

namespace ALOD.Data
{
    /// <summary>
    /// Data access object for memorandum signatures.
    /// Provides operations for managing digital signatures associated with memos.
    /// </summary>
    public class MemoSignatureDao : IMemoSignatureDao
    {
        private SqlDataStore _dataSource;

        /// <summary>
        /// Gets the SQL data store for executing database operations.
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

        /// <inheritdoc/>
        public MemoSignature GetSignature(int refId, int workflow, int ptype)
        {
            DataSet dSet = DataSource.ExecuteDataSet("core_Signature_sp_Get", refId, workflow, ptype);

            return DataHelpers.ExtractObjectFromDataSet<MemoSignature>(dSet);
        }

        /// <inheritdoc/>
        public void InsertSignature(int refId, int workflow, string sig, string sig_date, int userId, int ptype)
        {
            DataSource.ExecuteNonQuery("core_Signature_sp_Insert", refId, workflow, sig, sig_date, userId, ptype);
        }
    }
}