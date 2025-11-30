using ALOD.Core.Domain.Query;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Core.Utils;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Data;
using System.Linq;
using System.Text;

namespace ALOD.Data
{
    public class PHQueryDao : AbstractNHibernateDao<PHUserQuery, int>, IPHQueryDao
    {
        #region SQL DataSource Property

        private SqlDataStore _dataSource;

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

        #endregion SQL DataSource Property

        private ISession Session
        {
            get { return NHibernateSessionManager.Instance.GetSession(); }
        }

        /// <summary>
        /// Deletes a PH query clause from the database.
        /// </summary>
        /// <param name="clause">The PHQueryClause object to delete.</param>
        public void DeleteClause(PHQueryClause clause)
        {
            Session.Delete(clause);
        }

        /// <summary>
        /// Deletes a PH query output field from the database.
        /// </summary>
        /// <param name="outputField">The PHQueryOutputField object to delete.</param>
        public void DeleteOutputField(PHQueryOutputField outputField)
        {
            Session.Delete(outputField);
        }

        /// <summary>
        /// Deletes a PH query parameter from the database.
        /// </summary>
        /// <param name="parameter">The PHQueryParameter object to delete.</param>
        public void DeleteParameter(PHQueryParameter parameter)
        {
            Session.Delete(parameter);
        }

        /// <summary>
        /// Executes a PH ad-hoc query for cases (not yet implemented).
        /// </summary>
        /// <param name="queryId">The query ID.</param>
        /// <param name="userId">The user ID executing the query.</param>
        /// <param name="scope">The scope of the query.</param>
        /// <param name="unitId">The unit ID filter.</param>
        /// <param name="viewType">The view type filter.</param>
        /// <returns>Always returns null (not implemented).</returns>
        public DataSet ExecuteCasesQuery(int queryId, int userId, byte scope, int unitId, int viewType)
        {
            return null;
        }

        /// <summary>
        /// Executes a PH ad-hoc totals query.
        /// Builds dynamic SQL conditions and executes stored procedure: report_sp_PHAdHocTotalsQuery
        /// </summary>
        /// <param name="queryId">The query ID.</param>
        /// <param name="userId">The user ID executing the query.</param>
        /// <param name="scope">The scope of the query.</param>
        /// <param name="unitId">The unit ID filter.</param>
        /// <param name="viewType">The view type filter.</param>
        /// <returns>A UserQueryResult containing query metadata and result data, or null if query is invalid or not found.</returns>
        public UserQueryResult ExecuteTotalsQuery(int queryId, int userId, byte scope, int unitId, int viewType)
        {
            if (queryId < 1)
                return null;

            PHUserQuery query = GetByBaseQueryId(queryId);

            if (query == null)
                return null;

            UserQueryResult queryResults = new UserQueryResult();

            queryResults.QueryId = query.Id;
            queryResults.QueryTitle = query.UserQuery.Title;

            StringBuilder workflowConditions = new StringBuilder();
            StringBuilder phFormConditions = new StringBuilder();

            // Build workflow conditions using the original UserQuery object...
            workflowConditions = BuildWorkflowConditions(query.UserQuery, viewType);

            // Build ph form conditions...
            phFormConditions = BuildTotalsPHFormConditions(query);

            queryResults.ResultData = DataSource.ExecuteDataSet("report_sp_PHAdHocTotalsQuery", query.Id, userId, scope, unitId, viewType, workflowConditions.ToString(), phFormConditions.ToString(), query.SortField);

            return queryResults;
        }

        /// <summary>
        /// Retrieves a PH user query by base query ID.
        /// </summary>
        /// <param name="queryId">The base UserQuery ID.</param>
        /// <returns>The PHUserQuery object matching the base query ID, or null if not found.</returns>
        public PHUserQuery GetByBaseQueryId(int queryId)
        {
            return Session.CreateCriteria(typeof(PHUserQuery))
                .Add(Expression.Eq("UserQuery.Id", queryId))
                .UniqueResult<PHUserQuery>();
        }

        /// <summary>
        /// Retrieves a PH query output field by ID.
        /// </summary>
        /// <param name="id">The output field ID.</param>
        /// <returns>The PHQueryOutputField object.</returns>
        public PHQueryOutputField GetOutputFieldById(int id)
        {
            return (PHQueryOutputField)NHibernateSession.Load(typeof(PHQueryOutputField), id);
        }

        /// <summary>
        /// Retrieves a PH query parameter by ID.
        /// </summary>
        /// <param name="id">The parameter ID.</param>
        /// <returns>The PHQueryParameter object.</returns>
        public PHQueryParameter GetParameterById(int id)
        {
            return (PHQueryParameter)NHibernateSession.Load(typeof(PHQueryParameter), id);
        }

        /// <summary>
        /// Saves or updates a PH query clause.
        /// </summary>
        /// <param name="clause">The PHQueryClause object to save or update.</param>
        public void SaveOrUpdateClause(PHQueryClause clause)
        {
            Session.SaveOrUpdate(clause);
        }

        /// <summary>
        /// Saves or updates a PH query output field.
        /// </summary>
        /// <param name="outputField">The PHQueryOutputField object to save or update.</param>
        public void SaveOrUpdateOutputField(PHQueryOutputField outputField)
        {
            Session.SaveOrUpdate(outputField);
        }

        /// <summary>
        /// Saves or updates a PH query parameter.
        /// </summary>
        /// <param name="parameter">The PHQueryParameter object to save or update.</param>
        public void SaveOrUpdateParameter(PHQueryParameter parameter)
        {
            Session.SaveOrUpdate(parameter);
        }

        /// <summary>
        /// Saves or updates a PH user query.
        /// </summary>
        /// <param name="query">The PHUserQuery object to save or update.</param>
        public void SaveOrUpdateQuery(PHUserQuery query)
        {
            Session.SaveOrUpdate(query);
        }

        /// <summary>
        /// Updates the execution order of PH query parameters.
        /// Executes stored procedure: ph_query_param_sp_UpdateExecuteOrders
        /// </summary>
        /// <param name="clauseId">The clause ID.</param>
        /// <param name="paramId">The parameter ID.</param>
        /// <param name="newOrder">The new execution order.</param>
        /// <param name="oldOrder">The old execution order.</param>
        /// <param name="filter">The filter criteria.</param>
        public void UpdateParamExecuteOrders(int clauseId, int paramId, int newOrder, int oldOrder, int filter)
        {
            DataSource.ExecuteNonQuery("ph_query_param_sp_UpdateExecuteOrders", clauseId, paramId, newOrder, oldOrder, filter);
        }

        private StringBuilder BuildTotalsPHFormConditions(PHUserQuery query)
        {
            StringBuilder conditions = new StringBuilder();
            bool isFirstParam = true;
            string paramValue = string.Empty;
            string paramSourceFieldName = "Total";  // At the moment the Ad Hoc PH Form Totals reports only support one field to query (Total)

            if (query.Clauses == null || query.Clauses.Count == 0)
            {
                conditions.Append("1 = 1");
            }
            else
            {
                foreach (PHQueryClause clause in query.Clauses)
                {
                    if (clause.Parameters == null || clause.Parameters.Count == 0)
                    {
                        conditions.Append("1 = 1");
                    }
                    else
                    {
                        isFirstParam = true;

                        foreach (PHQueryParameter param in clause.Parameters.OrderBy(x => x.ExecuteOrder))
                        {
                            paramValue = string.Empty;

                            if (!isFirstParam)
                            {
                                conditions.Append(param.WhereType == WhereType.AND ? " AND " : " OR ");
                            }
                            else
                            {
                                isFirstParam = false;
                            }

                            paramValue = param.StartValue;

                            switch (param.Operator)
                            {
                                case QueryOperator.LIKE:
                                    conditions.Append("ff.[" + paramSourceFieldName + "] ");
                                    conditions.Append("LIKE '%" + param.StartValue + "%'");
                                    break;

                                case QueryOperator.EQUALS:
                                    conditions.Append("ff.[" + paramSourceFieldName + "] ");
                                    conditions.Append("= " + paramValue);
                                    break;

                                case QueryOperator.NOT_EQUAL:
                                    conditions.Append("ff.[" + paramSourceFieldName + "] ");
                                    conditions.Append("<> " + paramValue);
                                    break;

                                case QueryOperator.GREATER_THAN:
                                    conditions.Append("ff.[" + paramSourceFieldName + "] ");
                                    conditions.Append("> " + paramValue);
                                    break;

                                case QueryOperator.LESS_THAN:
                                    conditions.Append("ff.[" + paramSourceFieldName + "] ");
                                    conditions.Append("< " + paramValue);
                                    break;

                                case QueryOperator.BETWEEN:
                                    conditions.Append("ff.[" + paramSourceFieldName + "] ");

                                    // For between operators we need to add a second paramater
                                    string paramSecondValue = param.EndValue;

                                    //Parameter is likely a number and should follow the standard between
                                    conditions.Append("BETWEEN " + paramValue);
                                    conditions.Append(" AND " + paramSecondValue);
                                    break;
                            }
                        }
                    }
                }
            }

            return conditions;
        }

        private StringBuilder BuildWorkflowConditions(UserQuery query, int viewType)
        {
            StringBuilder conditions = new StringBuilder();
            bool isFirstParam = true;
            string paramValue = string.Empty;

            if (query.Clauses == null || query.Clauses.Count == 0)
            {
                conditions.Append("1 = 1");
            }
            else
            {
                foreach (QueryClause clause in query.Clauses)
                {
                    if (clause.Parameters == null || clause.Parameters.Count == 0)
                    {
                        conditions.Append("1 = 1");
                    }
                    else
                    {
                        isFirstParam = true;

                        foreach (QueryParameter param in clause.Parameters.OrderBy(x => x.ExecuteOrder))
                        {
                            paramValue = string.Empty;

                            if (!isFirstParam)
                            {
                                conditions.Append(param.WhereType == WhereType.AND ? " AND " : " OR ");
                            }
                            else
                            {
                                isFirstParam = false;
                            }

                            paramValue = AdHocQueryHelpers.ConvertToValidDbValue(param.Source.DataType, AdHocQueryHelpers.SourceValueToParamValue(param.Source.DataType, param.StartValue));

                            switch (param.Operator)
                            {
                                case QueryOperator.LIKE:
                                    conditions.Append("s.[" + param.Source.FieldName + "] ");
                                    conditions.Append("LIKE '%" + param.StartValue + "%'");
                                    break;

                                case QueryOperator.EQUALS:
                                    // Special case: Unit name with the Include Subordinate Unit checkbox checked.
                                    if (AdHocQueryHelpers.IsUnitNameSource(param.Source.DisplayName) && String.IsNullOrEmpty(param.EndValue) == false && Int32.Parse(param.EndValue) > -1)
                                    {
                                        conditions.Append("s.[" + AdHocQueryHelpers.GetUnitNameSourceFieldName(param.Source.DisplayName) + "] ");
                                        conditions.Append("IN ( SELECT cs.CS_ID FROM Command_Struct_Tree cst INNER JOIN Command_Struct cs ON cs.CS_ID = cst.child_id WHERE cst.parent_id = " + param.EndValue + " AND cst.view_type = " + viewType + ")");
                                    }
                                    // Special case: special case case type and special case parameter
                                    else if (param.Source.DisplayName.Equals("Special Case") && param.Source.TableName.Equals("vw_sc") && Services.LookupService.GetSCSubWorkflowTypeId(param.StartDisplay) != -1)
                                    {
                                        conditions.Append("s.[sub_workflow_type] = " + paramValue);
                                    }
                                    else
                                    {
                                        conditions.Append("s.[" + param.Source.FieldName + "] ");
                                        conditions.Append("= " + paramValue);
                                    }
                                    break;

                                case QueryOperator.NOT_EQUAL:
                                    // Special case: special case case type and special case parameter
                                    if (param.Source.DisplayName.Equals("Special Case") && param.Source.TableName.Equals("vw_sc") && Services.LookupService.GetSCSubWorkflowTypeId(param.StartDisplay) != -1)
                                    {
                                        conditions.Append("s.[sub_workflow_type] <> " + paramValue);
                                    }
                                    else
                                    {
                                        conditions.Append("s.[" + param.Source.FieldName + "] ");
                                        conditions.Append("<> " + paramValue);
                                    }
                                    break;

                                case QueryOperator.GREATER_THAN:
                                    conditions.Append("s.[" + param.Source.FieldName + "] ");

                                    // If this is a date-range, we need to flip greater/less
                                    if (param.Source.DataType == "D" && !char.IsNumber(param.StartValue[param.StartValue.Length - 1]))
                                    {
                                        conditions.Append("< " + paramValue);
                                    }
                                    else
                                    {
                                        conditions.Append("> " + paramValue);
                                    }
                                    break;

                                case QueryOperator.LESS_THAN:
                                    conditions.Append("s.[" + param.Source.FieldName + "] ");

                                    // If this is a date-range, we need to flip greater/less
                                    if (param.Source.DataType == "D" && !char.IsNumber(param.StartValue[param.StartValue.Length - 1]))
                                    {
                                        conditions.Append("> " + paramValue);
                                    }
                                    else
                                    {
                                        conditions.Append("< " + paramValue);
                                    }
                                    break;

                                case QueryOperator.BETWEEN:
                                    conditions.Append("s.[" + param.Source.FieldName + "] ");

                                    // For between operators we need to add a second paramater
                                    string paramSecondValue = AdHocQueryHelpers.ConvertToValidDbValue(param.Source.DataType, AdHocQueryHelpers.SourceValueToParamValue(param.Source.DataType, param.EndValue));

                                    if (param.Source.DataType == "D")
                                    {
                                        if (DateTime.Parse(AdHocQueryHelpers.SourceValueToParamValue(param.Source.DataType, param.StartValue)) > DateTime.Parse(AdHocQueryHelpers.SourceValueToParamValue(param.Source.DataType, param.EndValue)))
                                        {
                                            //End value is less than start value
                                            conditions.Append("BETWEEN " + paramSecondValue);
                                            conditions.Append(" AND " + paramValue);
                                        }
                                        else
                                        {
                                            //Start value is less than end value
                                            conditions.Append("BETWEEN " + paramValue);
                                            conditions.Append(" AND " + paramSecondValue);
                                        }
                                    }
                                    else
                                    {
                                        //Parameter is likely a number and should follow the standard between
                                        conditions.Append("BETWEEN " + paramValue);
                                        conditions.Append(" AND " + paramSecondValue);
                                    }
                                    break;
                            }
                        }
                    }
                }
            }

            return conditions;
        }

        private StringBuilder BuildWorkflowSortStatement(UserQuery query)
        {
            StringBuilder sort = new StringBuilder();

            if (!string.IsNullOrEmpty(query.SortFields))
            {
                string[] sortColumns = query.SortFields.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);

                if (sortColumns.Length > 0)
                {
                    foreach (string sortColumn in sortColumns)
                    {
                        // The (ASC) and (DESC) are passed as part of the column name.
                        // We need to extract and tack that part
                        int start = sortColumn.LastIndexOf('(');
                        int stop = sortColumn.LastIndexOf(')');

                        if (start > 0)
                        {
                            //grab the order without the ()
                            string order = sortColumn.Substring(start + 1, sortColumn.Length - start - 2);
                            string colName = sortColumn.Substring(0, start - 1);

                            sort.Append("s.[" + colName + "] " + order + ",");
                        }
                    }

                    // Remove trailing comma...
                    sort.Remove(sort.Length - 1, 1);
                }
            }

            return sort;
        }
    }
}