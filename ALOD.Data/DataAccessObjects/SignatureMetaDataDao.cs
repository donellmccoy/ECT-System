using ALOD.Core.Domain.Workflow;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Core.Utils;
using System.Collections.Generic;
using System.Data;

namespace ALOD.Data
{
    /// <summary>
    /// Data access object for <see cref="SignatureMetaData"/> entities.
    /// Manages digital signature metadata for workflow steps, including user, group, and status information.
    /// </summary>
    public class SignatureMetaDataDao : AbstractNHibernateDao<SignatureMetaData, int>, ISignatueMetaDateDao
    {
        private SqlDataStore dataSource;

        /// <summary>
        /// Gets the SQL data store for executing database operations.
        /// </summary>
        private SqlDataStore DataSource
        {
            get
            {
                if (dataSource == null)
                {
                    dataSource = new SqlDataStore();
                }
                return dataSource;
            }
        }

        /// <inheritdoc/>
        public void AddForWorkStatus(SignatureMetaData signature)
        {
            DataSource.ExecuteNonQuery("core_SignatureMetaData_sp_Add", signature.refId, signature.workflowId, signature.workStatus, signature.userGroup, signature.userId, signature.date, signature.NameAndRank, signature.Title);
        }

        /// <inheritdoc/>
        public void DeleteForWorkStatus(int refId, int workflowId, int workStatus)
        {
            DataSource.ExecuteNonQuery("core_SignatureMetaData_sp_Delete", refId, workflowId, workStatus);
        }

        /// <inheritdoc/>
        public IList<SignatureMetaData> GetAllForCase(int refId, int workflowId)
        {
            DataSet dSet = DataSource.ExecuteDataSet("core_SignatureMetaData_sp_GetAll", refId, workflowId);

            return DataHelpers.ExtractObjectsFromDataSet<SignatureMetaData>(dSet);
        }

        /// <inheritdoc/>
        public SignatureMetaData GetByUserGroup(int refId, int workflowId, int groupId)
        {
            DataSet dSet = DataSource.ExecuteDataSet("core_SignatureMetaData_sp_GetByUserGroup", refId, workflowId, groupId);

            return DataHelpers.ExtractObjectFromDataSet<SignatureMetaData>(dSet);
        }

        /// <inheritdoc/>
        public SignatureMetaData GetByWorkStatus(int refId, int workflowId, int workStatus)
        {
            DataSet dSet = DataSource.ExecuteDataSet("core_SignatureMetaData_sp_GetByWorkStatus", refId, workflowId, workStatus);

            return DataHelpers.ExtractObjectFromDataSet<SignatureMetaData>(dSet);
        }
    }
}