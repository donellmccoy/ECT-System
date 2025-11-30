using ALOD.Core.Domain.Modules.SpecialCases;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Core.Utils;
using ALOD.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace ALOD.Data
{
    /// <summary>
    /// Data access object for managing <see cref="AssociatedCase"/> entities.
    /// Handles associations between different cases (LOD, Special Cases, etc.) for tracking related investigations.
    /// </summary>
    public class AssociatedCaseDao : IAssociatedCaseDao
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
        /// Retrieves all associated cases for a given reference ID and workflow.
        /// </summary>
        /// <param name="refId">The reference ID of the primary case.</param>
        /// <param name="workflowId">The workflow ID.</param>
        /// <returns>A list of associated cases.</returns>
        public IList<AssociatedCase> GetAssociatedCases(int refId, int workflowId)
        {
            DataSet dSet = DataSource.ExecuteDataSet("core_AssociatedCases_GetAssociatedCases", refId, workflowId);

            return DataHelpers.ExtractObjectsFromDataSet<AssociatedCase>(dSet, new NHibernateDaoFactory());
        }

        /// <summary>
        /// Retrieves associated LOD cases for a given reference ID and workflow.
        /// </summary>
        /// <param name="refId">The reference ID of the primary case.</param>
        /// <param name="workflowId">The workflow ID.</param>
        /// <returns>A list of associated LOD cases.</returns>
        public IList<AssociatedCase> GetAssociatedCasesLOD(int refId, int workflowId)
        {
            DataSet dSet = DataSource.ExecuteDataSet("core_AssociatedCases_GetAssociatedCasesLOD", refId, workflowId);

            return DataHelpers.ExtractObjectsFromDataSet<AssociatedCase>(dSet, new NHibernateDaoFactory());
        }

        /// <summary>
        /// Retrieves associated special cases for a given reference ID and workflow.
        /// </summary>
        /// <param name="refId">The reference ID of the primary case.</param>
        /// <param name="workflowId">The workflow ID.</param>
        /// <returns>A list of associated special cases.</returns>
        public IList<AssociatedCase> GetAssociatedCasesSC(int refId, int workflowId)
        {
            DataSet dSet = DataSource.ExecuteDataSet("core_AssociatedCases_GetAssociatedCasesSC", refId, workflowId);

            return DataHelpers.ExtractObjectsFromDataSet<AssociatedCase>(dSet, new NHibernateDaoFactory());
        }

        /// <summary>
        /// Retrieves a list of LOD cases by member SSN.
        /// </summary>
        /// <param name="memberSSN">The member's SSN.</param>
        /// <param name="searchType">The type of search to perform.</param>
        /// <param name="userId">The user performing the search.</param>
        /// <returns>A list of tuples containing case ID, reference ID, and workflow ID.</returns>
        public IList<Tuple<string, int, int>> GetLODListByMemberSSN(string memberSSN, int searchType, int userId)
        {
            DataSet dSet = DataSource.ExecuteDataSet("core_lod_sp_GetLODListByMemberSSN", memberSSN, searchType, userId);

            try
            {
                List<Tuple<string, int, int>> extractedObjects = new List<Tuple<string, int, int>>();

                if (dSet == null)
                    return extractedObjects;

                if (dSet.Tables.Count == 0)
                    return extractedObjects;

                DataTable dTable = dSet.Tables[0];

                if (dTable == null)
                    return extractedObjects;

                foreach (DataRow row in dTable.Rows)
                {
                    String CaseId = DataHelpers.GetStringFromDataRow("caseId", row);
                    int workflowId = DataHelpers.GetIntFromDataRow("workflowId", row);
                    int refId = DataHelpers.GetIntFromDataRow("refId", row);

                    extractedObjects.Add(new Tuple<string, int, int>(CaseId, refId, workflowId));
                }

                return extractedObjects;
            }
            catch (Exception e)
            {
                LogManager.LogError(e);
                return new List<Tuple<string, int, int>>();
            }
        }

        /// <summary>
        /// Saves associated case relationships for a primary case.
        /// </summary>
        /// <param name="refId">The reference ID of the primary case.</param>
        /// <param name="workflowId">The workflow ID of the primary case.</param>
        /// <param name="associated_refIds">List of associated case reference IDs.</param>
        /// <param name="associated_workflowIds">List of associated case workflow IDs.</param>
        /// <param name="associated_CaseIds">List of associated case IDs.</param>
        public void Save(int refId, int workflowId, IList<int> associated_refIds, IList<int> associated_workflowIds, IList<string> associated_CaseIds)
        {
            try
            {
                SqlCommand sqlCmd = new SqlCommand("core_AssocaitedCases_SaveAssociatedCases @refId, @workflowId, @assocaited_refIds, @associated_workflowIds, @associated_CaseIds");

                SqlParameter sqlParam = new SqlParameter("@refId", refId);
                sqlParam.DbType = DbType.Int32;
                sqlCmd.Parameters.Add(sqlParam);

                sqlParam = new SqlParameter("@workflowId", workflowId);
                sqlParam.DbType = DbType.Int32;
                sqlCmd.Parameters.Add(sqlParam);

                sqlParam = new SqlParameter("@assocaited_refIds", SqlDbType.Structured);
                sqlParam.TypeName = "dbo.tblIntegerListUnordered";
                sqlParam.Value = CollectionHelpers.IntListToListOfSQLDataRecords(associated_refIds);
                sqlCmd.Parameters.Add(sqlParam);

                sqlParam = new SqlParameter("@associated_workflowIds", SqlDbType.Structured);
                sqlParam.TypeName = "dbo.tblIntegerListUnordered";
                sqlParam.Value = CollectionHelpers.IntListToListOfSQLDataRecords(associated_workflowIds);
                sqlCmd.Parameters.Add(sqlParam);

                sqlParam = new SqlParameter("@associated_CaseIds", SqlDbType.Structured);
                sqlParam.TypeName = "dbo.tblVarCharListUnordered";
                sqlParam.Value = CollectionHelpers.StringListToListOfSQLDataRecords(associated_CaseIds);
                sqlCmd.Parameters.Add(sqlParam);

                DataSource.ExecuteScalar(sqlCmd);
            }
            catch (Exception e)
            {
                LogManager.LogError(e);
            }
        }
    }
}