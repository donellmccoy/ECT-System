using ALOD.Core.Domain.ServiceMembers;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Core.Utils;
using System.Collections.Generic;
using System.Data;

namespace ALOD.Data
{
    /// <summary>
    /// Data access object for ServiceMember entities, providing operations to manage military personnel records and change history.
    /// </summary>
    public class MemberDao : AbstractNHibernateDao<ServiceMember, string>, IMemberDao
    {
        private SqlDataStore _datastore;

        public SqlDataStore DataStore
        {
            get
            {
                if (_datastore == null)
                    _datastore = new SqlDataStore();

                return _datastore;
            }
        }

        /// <summary>
        /// Retrieves the MILPDS (Military Personnel Data System) change history for a service member.
        /// </summary>
        /// <param name="ssn">The Social Security Number of the service member. Must be exactly 9 digits.</param>
        /// <returns>A list of personnel change records from the MILPDS system.</returns>
        /// <exception cref="System.ArgumentException">Thrown when the SSN is not in the correct format.</exception>
        /// <remarks>
        /// This method executes the stored procedure member_sp_GetMILPDSChangeHistoryBySSN to retrieve historical personnel data changes.
        /// </remarks>
        public IList<ServiceMemberMILPDSChangeHistory> GetMILPDSChangeHistoryBySSN(string ssn)
        {
            DataSet dSet = DataStore.ExecuteDataSet("member_sp_GetMILPDSChangeHistoryBySSN", ssn);

            return DataHelpers.ExtractObjectsFromDataSet<ServiceMemberMILPDSChangeHistory>(dSet, new NHibernateDaoFactory());
        }

        /// <summary>
        /// Retrieves the system administrator change history for a service member.
        /// </summary>
        /// <param name="ssn">The Social Security Number of the service member. Must be exactly 9 digits.</param>
        /// <returns>A DataSet containing administrative change records made by system administrators.</returns>
        /// <exception cref="System.ArgumentException">Thrown when the SSN is not in the correct format.</exception>
        /// <remarks>
        /// This method executes the stored procedure member_sp_GetSystemAdminChangeHistoryBySSN to retrieve manual administrative updates.
        /// </remarks>
        public DataSet GetSystemAdminChangeHistoryBySSN(string ssn)
        {
            return DataStore.ExecuteDataSet("member_sp_GetSystemAdminChangeHistoryBySSN", ssn);
        }
    }
}