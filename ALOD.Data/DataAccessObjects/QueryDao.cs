using ALOD.Core.Domain.Query;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Core.Utils;
using ALOD.Logging;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace ALOD.Data
{
    public class QueryDao : AbstractNHibernateDao<UserQuery, int>, IQueryDao
    {
        private UserQueryResult _queryResult = null;

        protected UserQueryResult QueryResult
        {
            get
            {
                if (_queryResult == null)
                    _queryResult = new UserQueryResult();

                return _queryResult;
            }
        }

        private ISession Session
        {
            get { return NHibernateSessionManager.Instance.GetSession(); }
        }

        /// <inheritdoc/>
        public void DeleteClause(QueryClause clause)
        {
            Session.Delete(clause);
        }

        /// <inheritdoc/>
        public void DeleteParameter(QueryParameter parameter)
        {
            Session.Delete(parameter);
        }

        /// <inheritdoc/>
        public UserQueryResult ExecuteQuery(UserQueryArgs args)
        {
            try
            {
                IList<QuerySource> sources = new List<QuerySource>();
                IList<InputParam> outputParams = new List<InputParam>();

                StringBuilder selectClause = new StringBuilder();
                StringBuilder fromClause = new StringBuilder();
                StringBuilder whereClause = new StringBuilder();
                StringBuilder sortClause = new StringBuilder();
                StringBuilder buffer = new StringBuilder();

                UserQuery query = GetById(args.QueryId);

                if (query == null)
                    return QueryResult;

                QueryResult.QueryId = query.Id;
                QueryResult.QueryTitle = query.Title;

                whereClause = BuildWhereClause(outputParams, query, sources); // Building the WHERE clause populates the sources list so build this clause first...
                selectClause = BuildSelectClause(query);
                fromClause = BuildFromClause(sources, args.UserId);
                sortClause = BuildOrderByClause(query);

                buffer.Append(selectClause);
                buffer.Append(fromClause);
                buffer.Append(" ");
                buffer.Append(whereClause);
                buffer.Append(sortClause);

                InsertAdditionalOutputParameters(outputParams, sources, args);

                QueryResult.ResultData = ExecuteSQLQuery(buffer, outputParams);

                return QueryResult;
            }
            catch (Exception e)
            {
                LogManager.LogError(e);
                QueryResult.Errors.Add("Query failed to execute.");
                return QueryResult;
            }
        }

        /// <inheritdoc/>
        public IList<InputSource> GetAllInputSources()
        {
            ICriteria criteria = NHibernateSession.CreateCriteria(typeof(InputSource));
            return criteria.List<InputSource>();
        }

        /// <inheritdoc/>
        public DataSet GetInputChoices(InputSource source)
        {
            if (source.LookupSource.Substring(0, 2) == "L:")
            {
                //this has a pre-defined list
                DataSet data = new DataSet("Results");
                data.Tables.Add("Lookup");
                DataTable table = data.Tables[0];
                table.Columns.Add(source.LookupValue);
                table.Columns.Add(source.LookupText);

                string start = source.LookupSource.Substring(2);
                string[] values = start.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                DataRow row;

                foreach (string item in values)
                {
                    row = table.NewRow();
                    row[source.LookupValue] = item;
                    row[source.LookupText] = item;
                    table.Rows.Add(row);
                }

                return data;
            }
            else
            {
                //get from the database
                SqlDataStore store = new SqlDataStore();

                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT [" + source.LookupValue + "] ");
                sql.Append(",[" + source.LookupText + "] ");
                sql.Append(" FROM [" + source.LookupSource + "] ");
                if (!String.IsNullOrEmpty(source.LookupWhere))
                {
                    sql.Append("WHERE " + source.LookupWhere + " = " + source.LookupWhereValue);
                }
                sql.Append(" ORDER BY [" + source.LookupSort + "];");

                DbCommand cmd = store.GetSqlStringCommand(sql.ToString());
                return store.ExecuteDataSet(cmd);
            }
        }

        /// <summary>
        /// Returns all input sources (fields) matching table name.
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public IList<InputSource> GetInputSourcesByTableName(string tableName)
        {
            return Session.CreateCriteria(typeof(InputSource))
                .Add(Expression.Eq("TableName", tableName))
                .List<InputSource>();
        }

        /// <inheritdoc/>
        public IList<QueryOutputField> GetOutputFields()
        {
            return Session.CreateCriteria(typeof(QueryOutputField))
                .Add(Expression.Eq("QueryType", "adhoc"))
                .List<QueryOutputField>();
        }

        /// <summary>
        /// Returns all output fields matching the table name
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public IList<QueryOutputField> GetOutputFields(string tableName)
        {
            return Session.CreateCriteria(typeof(QueryOutputField))
                .Add(Expression.Eq("QueryType", "adhoc"))
                .Add(Expression.Eq("TableName", tableName))
                .List<QueryOutputField>();
        }

        /// <inheritdoc/>
        public QueryParameter GetParameterById(int id)
        {
            return (QueryParameter)NHibernateSession.Load(typeof(QueryParameter), id);
        }

        /// <inheritdoc/>
        public IList<UserQuery> GetSharedUserQueries()
        {
            return Session.CreateCriteria(typeof(UserQuery))
                .Add(Expression.Eq("Shared", true))
                .List<UserQuery>();
        }

        /// <inheritdoc/>
        public InputSource GetSourceById(int id)
        {
            return (InputSource)NHibernateSession.Load(typeof(InputSource), id);
        }

        /// <inheritdoc/>
        public IList<UserQuery> GetUserQueries(int userId)
        {
            return Session.CreateCriteria(typeof(UserQuery))
                .Add(Expression.Eq("User.Id", userId))
                .List<UserQuery>();
        }

        /// <inheritdoc/>
        public void SaveOrUpdateClause(QueryClause clause)
        {
            Session.SaveOrUpdate(clause);
        }

        /// <inheritdoc/>
        public void SaveOrUpdateParameter(QueryParameter parameter)
        {
            Session.SaveOrUpdate(parameter);
        }

        /// <inheritdoc/>
        public void SaveOrUpdateQuery(UserQuery query)
        {
            Session.SaveOrUpdate(query);
        }

        /// <inheritdoc/>
        public void SaveOrUpdateSource(InputSource source)
        {
            Session.SaveOrUpdate(source);
        }

        /// <inheritdoc/>
        public void UpdateParamExecuteOrders(int clauseId, int paramId, int newOrder, int oldOrder, int filter)
        {
            SqlDataStore store = new SqlDataStore();
            store.ExecuteNonQuery("query_param_sp_UpdateExecuteOrders", clauseId, paramId, newOrder, oldOrder, filter);
        }

        protected StringBuilder BuildFromClause(IList<QuerySource> sources, int userId)
        {
            try
            {
                StringBuilder from = new StringBuilder();

                from.Append(" FROM ");

                foreach (QuerySource source in sources)
                {
                    from.Append(source.TableName + " " + source.Alias);

                    if (source.TableName.CompareTo("vw_all_cases") == 0)
                    {
                        from.Append(" JOIN core_Permissions p ON " + source.Alias + ".Permission = p.permName");
                        from.Append(" JOIN core_Users u ON u.userID = @userId");
                        from.Append(" JOIN core_UserRoles ur ON u.currentRole = ur.userRoleID");
                        from.Append(" JOIN core_GroupPermissions gp ON ur.groupId = gp.groupId AND gp.permId = p.permId");
                    }

                    from.Append(",");
                }

                from.Remove(from.Length - 1, 1);

                return from;
            }
            catch (Exception e)
            {
                LogManager.LogError(e);
                QueryResult.Errors.Add("Failed to build from clause.");
                throw new Exception("Error: An exception has occured in QueryDao.BuildFromClause().");
            }
        }

        protected StringBuilder BuildLODOnlyWhereClauseConditions(IList<QuerySource> sources)
        {
            StringBuilder lodOnlyConditions = new StringBuilder();

            if (!IsLODOnlyQuery(sources))
                return lodOnlyConditions;

            //Add the Deleted filter
            lodOnlyConditions.Append(" AND vw_lod.[deleted] = 0 ");

            //now add the SARC permission filtering
            lodOnlyConditions.Append(" AND CASE WHEN (vw_lod.[sarc] = 1 AND vw_lod.[restricted]=1) Then 1 ELSE @sarcpermission END =@sarcpermission");

            return lodOnlyConditions;
        }

        protected StringBuilder BuildOrderByClause(UserQuery query)
        {
            try
            {
                StringBuilder sort = new StringBuilder();

                if (!string.IsNullOrEmpty(query.SortFields))
                {
                    string[] sortColumns = query.SortFields.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);

                    if (sortColumns.Length > 0)
                    {
                        sort.Append(" ORDER BY ");

                        foreach (string sortColumn in sortColumns)
                        {
                            //the (ASC) and (DESC) are passed as part of the column name.
                            //we need to extract and tack that part
                            int start = sortColumn.LastIndexOf('(');
                            int stop = sortColumn.LastIndexOf(')');

                            if (start > 0)
                            {
                                //grab the order without the ()
                                string order = sortColumn.Substring(start + 1, sortColumn.Length - start - 2);
                                string colName = sortColumn.Substring(0, start - 1);

                                sort.Append("[" + colName + "] " + order + ",");
                            }
                        }

                        sort.Remove(sort.Length - 1, 1);
                    }
                }

                return sort;
            }
            catch (Exception e)
            {
                LogManager.LogError(e);
                QueryResult.Errors.Add("Failed to build order by clause.");
                throw new Exception("Error: An exception has occured in QueryDao.BuildOrderByClause().");
            }
        }

        protected StringBuilder BuildScopeWhereClauseConditions(IList<QuerySource> sources)
        {
            StringBuilder scopeConditions = new StringBuilder();

            foreach (QuerySource source in sources)
            {
                scopeConditions.Append(" AND ((@scope = 3) OR (@scope = 2) OR (@scope = 1 AND " + source.TableName + ".[member_unit_id] IN ");
                scopeConditions.Append("(SELECT child_id FROM Command_Struct_Tree WHERE parent_id=@userUnit and view_type=@rptView))");
                scopeConditions.Append("OR " + source.TableName + ".[created_by] =@userId)");
            }

            return scopeConditions;
        }

        protected StringBuilder BuildSelectClause(UserQuery query)
        {
            try
            {
                StringBuilder select = new StringBuilder();

                select.Append("SELECT ");

                foreach (string field in query.OutputFields)
                {
                    select.Append(query.GetPrimaryQuerySource().Alias + ".[" + field + "] ,");
                }

                select[select.Length - 1] = ' ';

                return select;
            }
            catch (Exception e)
            {
                LogManager.LogError(e);
                QueryResult.Errors.Add("Failed to build select clause.");
                throw new Exception("Error: An exception has occured in QueryDao.BuildSelectClause().");
            }
        }

        protected StringBuilder BuildWhereClause(IList<InputParam> outputParams, UserQuery query, IList<QuerySource> sources)
        {
            try
            {
                StringBuilder where = new StringBuilder();
                int paramCount = 0;
                bool isFirstParam = true;

                where.Append("WHERE ( ");

                foreach (QueryClause clause in query.Clauses)
                {
                    if (clause.Parameters.Count == 0)
                    {
                        where.Append("1 = 1");
                    }
                    else
                    {
                        isFirstParam = true;

                        foreach (QueryParameter param in clause.Parameters.OrderBy(x => x.ExecuteOrder))
                        {
                            //is this a new data source?
                            QuerySource tmp = (from s in sources
                                               where s.TableName == param.Source.TableName
                                               && s.AllowMultiple == true
                                               select s)
                                       .FirstOrDefault<QuerySource>();

                            if (tmp == null)
                            {
                                //get a new datasource for this one
                                tmp = new QuerySource(param.Source.TableName);
                                sources.Add(tmp);
                            }

                            //now append our wheres
                            if (!isFirstParam)
                            {
                                where.Append(param.WhereType == WhereType.AND ? " AND " : " OR ");
                            }
                            else
                            {
                                isFirstParam = false;
                            }

                            string paramName = "@p" + paramCount.ToString();
                            paramCount++;

                            InputParam input = new InputParam();
                            input.Name = paramName;
                            input.Value = param.StartValue;
                            input.Value = AdHocQueryHelpers.SourceValueToParamValue(param.Source.DataType, param.StartValue);
                            input.Type = AdHocQueryHelpers.SourceTypeToDbType(param.Source.DataType);

                            outputParams.Add(input);

                            switch (param.Operator)
                            {
                                case QueryOperator.LIKE:
                                    where.Append(BuildWhereClauseLikeCondition(tmp, param, paramName));
                                    input.Value = "%" + param.StartValue + "%";
                                    break;

                                case QueryOperator.EQUALS:
                                    where.Append(BuildWhereClauseEqualsCondition(tmp, param, paramName));

                                    if (IsUnitNameWithIncludeSubUnitParam(param))
                                    {
                                        input.Value = Int32.Parse(param.EndValue);
                                    }
                                    break;

                                case QueryOperator.NOT_EQUAL:
                                    where.Append(BuildWhereClauseNotEqualsCondition(tmp, param, paramName));
                                    break;

                                case QueryOperator.GREATER_THAN:
                                    where.Append(BuildWhereClauseGreaterThanCondition(tmp, param, paramName));
                                    break;

                                case QueryOperator.LESS_THAN:
                                    where.Append(BuildWhereClauseLessThanCondition(tmp, param, paramName));
                                    break;

                                case QueryOperator.BETWEEN:
                                    // For between operators we need to add a second paramater
                                    InputParam second = new InputParam();

                                    second.Name = "@p" + paramCount.ToString();
                                    second.Type = AdHocQueryHelpers.SourceTypeToDbType(param.Source.DataType);
                                    second.Value = param.EndValue;
                                    second.Value = AdHocQueryHelpers.SourceValueToParamValue(param.Source.DataType, param.EndValue);

                                    paramCount++;
                                    outputParams.Add(second);

                                    where.Append(BuildWhereClauseBetweenCondition(tmp, param, paramName, input, second));
                                    break;
                            }
                        }
                    }

                    where.Append(" )");
                }

                where.Append(BuildLODOnlyWhereClauseConditions(sources));
                where.Append(BuildScopeWhereClauseConditions(sources));

                return where;
            }
            catch (Exception e)
            {
                LogManager.LogError(e);
                QueryResult.Errors.Add("Failed to build where clause.");
                throw new Exception("Error: An exception has occured in QueryDao.BuildWhereClause().");
            }
        }

        protected StringBuilder BuildWhereClauseBetweenCondition(QuerySource source, QueryParameter param, string paramName, InputParam first, InputParam second)
        {
            try
            {
                StringBuilder condition = new StringBuilder();

                condition.Append(source.Alias + ".[" + param.Source.FieldName + "] ");

                if (param.Source.DataType == "D")
                {
                    if (DateTime.Parse((String)first.Value) > DateTime.Parse((String)second.Value))
                    {
                        // End value is less than start value
                        condition.Append(" BETWEEN " + second.Name);
                        condition.Append(" AND " + paramName);
                    }
                    else
                    {
                        // Start value is less than end value
                        condition.Append(" BETWEEN " + paramName);
                        condition.Append(" AND " + second.Name);
                    }
                }
                else
                {
                    // Parameter is likely a number and should follow the standard between
                    condition.Append(" BETWEEN " + paramName);
                    condition.Append(" AND " + second.Name);
                }

                return condition;
            }
            catch (Exception e)
            {
                LogManager.LogError(e);
                throw new Exception("Error: An exception has occured in QueryDao.BuildWhereClauseBetweenCondition().");
            }
        }

        protected StringBuilder BuildWhereClauseEqualsCondition(QuerySource source, QueryParameter param, string paramName)
        {
            try
            {
                StringBuilder condition = new StringBuilder();

                if (IsUnitNameWithIncludeSubUnitParam(param))
                {
                    condition.Append(source.Alias + ".[" + AdHocQueryHelpers.GetUnitNameSourceFieldName(param.Source.DisplayName) + "] ");
                    condition.Append("IN ( SELECT cs.CS_ID FROM Command_Struct_Tree cst INNER JOIN Command_Struct cs ON cs.CS_ID = cst.child_id WHERE cst.parent_id = " + paramName + " AND cst.view_type = @rptView)");
                }
                else if (IsSpecialCaseAndSpecialCaseTypeParam(param))
                {
                    condition.Append(source.Alias + ".[sub_workflow_type] = " + paramName);
                }
                else
                {
                    if (param.Source.DataType == "D")
                    {
                        condition.Append("CONVERT(DATE," + source.Alias + ".[" + param.Source.FieldName + "]) ");
                        condition.Append(" = CONVERT(DATE," + paramName + ")");
                    }
                    else
                    {
                        condition.Append(source.Alias + ".[" + param.Source.FieldName + "] ");
                        condition.Append(" = " + paramName);
                    }
                }

                return condition;
            }
            catch (Exception e)
            {
                LogManager.LogError(e);
                throw new Exception("Error: An exception has occured in QueryDao.BuildWhereClauseEqualsCondition().");
            }
        }

        protected StringBuilder BuildWhereClauseGreaterThanCondition(QuerySource source, QueryParameter param, string paramName)
        {
            try
            {
                StringBuilder condition = new StringBuilder();

                condition.Append(source.Alias + ".[" + param.Source.FieldName + "] ");

                //if this is a date-range, we need to flip greater/less
                if (param.Source.DataType == "D" && !char.IsNumber(param.StartValue[param.StartValue.Length - 1]))
                {
                    condition.Append(" < " + paramName);
                }
                else
                {
                    condition.Append(" > " + paramName);
                }

                return condition;
            }
            catch (Exception e)
            {
                LogManager.LogError(e);
                throw new Exception("Error: An exception has occured in QueryDao.BuildWhereClauseGreaterThanCondition().");
            }
        }

        protected StringBuilder BuildWhereClauseLessThanCondition(QuerySource source, QueryParameter param, string paramName)
        {
            try
            {
                StringBuilder condition = new StringBuilder();

                condition.Append(source.Alias + ".[" + param.Source.FieldName + "] ");

                //if this is a date-range, we need to flip greater/less
                if (param.Source.DataType == "D" && !char.IsNumber(param.StartValue[param.StartValue.Length - 1]))
                {
                    condition.Append(" > " + paramName);
                }
                else
                {
                    condition.Append(" < " + paramName);
                }

                return condition;
            }
            catch (Exception e)
            {
                LogManager.LogError(e);
                throw new Exception("Error: An exception has occured in QueryDao.BuildWhereClauseLessThanCondition().");
            }
        }

        protected StringBuilder BuildWhereClauseLikeCondition(QuerySource source, QueryParameter param, string paramName)
        {
            try
            {
                StringBuilder condition = new StringBuilder();

                condition.Append(source.Alias + ".[" + param.Source.FieldName + "] ");
                condition.Append(" LIKE " + paramName);

                return condition;
            }
            catch (Exception e)
            {
                LogManager.LogError(e);
                throw new Exception("Error: An exception has occured in QueryDao.BuildWhereClauseLikeCondition().");
            }
        }

        protected StringBuilder BuildWhereClauseNotEqualsCondition(QuerySource source, QueryParameter param, string paramName)
        {
            try
            {
                StringBuilder condition = new StringBuilder();

                if (IsSpecialCaseAndSpecialCaseTypeParam(param))
                {
                    condition.Append(source.Alias + ".[sub_workflow_type] <> " + paramName);
                }
                else
                {
                    condition.Append(source.Alias + ".[" + param.Source.FieldName + "] ");
                    condition.Append(" <> " + paramName);
                }

                return condition;
            }
            catch (Exception e)
            {
                LogManager.LogError(e);
                throw new Exception("Error: An exception has occured in QueryDao.BuildWhereClauseNotEqualsCondition().");
            }
        }

        protected DataSet ExecuteSQLQuery(StringBuilder buffer, IList<InputParam> outputParams)
        {
            try
            {
                DbCommand cmd = null;
                SqlDataStore store = new SqlDataStore();

                cmd = store.GetSqlStringCommand(buffer.ToString());

                foreach (InputParam values in outputParams)
                {
                    store.AddInParameter(cmd, values.Name, values.Type, values.Value);
                }

                return store.ExecuteDataSet(cmd);
            }
            catch (Exception e)
            {
                LogManager.LogError(e);
                QueryResult.Errors.Add("Query execution failed.");
                throw new Exception("Error: An exception has occured in QueryDao.ExecuteSQLQuery().");
            }
        }

        protected void InsertAdditionalOutputParameters(IList<InputParam> outputParams, IList<QuerySource> sources, UserQueryArgs args)
        {
            if (IsLODOnlyQuery(sources))
            {
                outputParams.Add(new InputParam("@sarcpermission", args.HasSarc, DbType.Boolean));
            }

            outputParams.Add(new InputParam("@scope", args.Scope, DbType.Byte));
            outputParams.Add(new InputParam("@userUnit", args.UnitId, DbType.Int32));
            outputParams.Add(new InputParam("@rptView", args.ViewType, DbType.Int32));
            outputParams.Add(new InputParam("@userId", args.UserId, DbType.Int32));
        }

        protected bool IsLODOnlyQuery(IList<QuerySource> sources)
        {
            bool isLOD = false;

            foreach (QuerySource source in sources)
            {
                if (source.TableName.CompareTo("vw_lod") == 0)
                {
                    isLOD = true;
                }
            }

            return isLOD;
        }

        protected bool IsSpecialCaseAndSpecialCaseTypeParam(QueryParameter param)
        {
            return (param.Source.DisplayName.Equals("Special Case") && param.Source.TableName.Equals("vw_sc") && Services.LookupService.GetSCSubWorkflowTypeId(param.StartDisplay) != -1);
        }

        protected bool IsUnitNameWithIncludeSubUnitParam(QueryParameter param)
        {
            return (AdHocQueryHelpers.IsUnitNameSource(param.Source.DisplayName) && String.IsNullOrEmpty(param.EndValue) == false && Int32.Parse(param.EndValue) > -1);
        }
    }
}