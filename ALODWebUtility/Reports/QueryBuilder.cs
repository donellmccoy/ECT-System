using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using ALOD.Core.Utils;
using ALOD.Data;

namespace ALODWebUtility.Reports
{
    public class QueryBuilder
{
        protected SqlDataStore _adapter;
        protected DbCommand _cmd;
        protected bool _distinct = false;
        protected List<QueryCondition> _params;
        protected StringBuilder _query;
        protected Dictionary<string, QuerySource> _sources;
        protected StringBuilder _where;

        // these are for the source aliasing
        private static StringBuilder _suffix = new StringBuilder();

        private static int _suffixChar = 64;
        private static int _suffixIndex = 0;

        public QueryBuilder()
        {
            _adapter = new SqlDataStore();
            _query = new StringBuilder();
            _where = new StringBuilder();
            _sources = new Dictionary<string, QuerySource>();
            _params = new List<QueryCondition>();
        }

        public bool Distinct
        {
            get
            {
                return _distinct;
            }
            set
            {
                _distinct = value;
            }
        }

        public void AddCondition(string field, object value, DbType valueType, string filter)
        {
            if (value != null && value.ToString().Length > 0)
            {
                _where.Append(" AND " + filter);
                _params.Add(new QueryCondition(field, value, valueType, filter));
            }
        }

        public void AddCondition(string field, string filter)
        {
            _where.Append(" AND " + filter);
            _params.Add(new QueryCondition(field, filter));
        }

        public void AddDefaultOrder(string source, params string[] orderFields)
        {
            _sources[source].AddOrderFields(orderFields);
        }

        public void AddFields(string source, params string[] fields)
        {
            _sources[source].AddFields(fields);
        }

        public QuerySource AddSource(string sourceName)
        {
            QuerySource source = new QuerySource();
            source.Name = sourceName;
            source.TableAlias = GetNextAlias();
            source.JoinType = JoinType.None;

            _sources.Add(sourceName, source);
            return source;
        }

        public QuerySource AddSource(string sourceName, string sourceKey, string joinName, string joinKey, JoinType join)
        {
            QuerySource source = new QuerySource();
            source.Name = sourceName;
            source.TableAlias = GetNextAlias();
            source.JoinType = join;

            QuerySource target = _sources[joinName];

            if (target == null)
            {
                return null;
            }

            source.JoinCondition = source.TableAlias + "." + sourceKey + " = " + target.TableAlias + "." + joinKey;

            _sources.Add(sourceName, source);
            return source;
        }

        public QuerySource AddSource(string sourceName, string joinName, string joinCondition, JoinType join)
        {
            QuerySource source = new QuerySource();
            source.Name = sourceName;
            source.TableAlias = GetNextAlias();
            source.JoinType = join;

            QuerySource target = _sources[joinName];

            if (target == null)
            {
                return null;
            }

            source.JoinCondition = joinCondition;

            _sources.Add(sourceName, source);
            return source;
        }

        public DataSet GetData()
        {
            BuildQuery();

            _cmd = _adapter.GetSqlStringCommand(_query.ToString());

            foreach (QueryCondition condition in _params)
            {
                _adapter.AddInParameter(_cmd, condition.Name, condition.ValueType, condition.Value);
            }

            return _adapter.ExecuteDataSet(_cmd, 180);
        }

        /// <summary>
        /// Gets the next available source alias
        /// </summary>
        /// <returns></returns>
        /// <remarks>Starts with [A-Z] then [AA-ZZ], etc</remarks>
        private static string GetNextAlias()
        {
            _suffixChar += 1;

            if (_suffixChar == 65)
            {
                // this is the first source
                _suffix.Append((char)_suffixChar);
            }
            else if (_suffixChar > 90)
            {
                // we've wrapped
                _suffixIndex += 1;
                _suffixChar = 65;
                // reset to A
                _suffix.Append((char)_suffixChar);
            }

            _suffix[_suffixIndex] = (char)_suffixChar;

            return _suffix.ToString();
        }

        private void BuildQuery()
        {
            StringBuilder query = new StringBuilder();
            StringBuilder from = new StringBuilder();
            StringBuilder orderBy = new StringBuilder();

            // iterate our sources building our query as we go
            foreach (QuerySource source in _sources.Values)
            {
                // append the fields for this source
                query.Append(source.FieldList);

                // append order fields
                orderBy.Append(source.OrderFieldList);

                // append the source
                if (source.JoinType == JoinType.None)
                {
                    from.Append(" FROM " + source.Name + " AS " + source.TableAlias);
                }
                else
                {
                    from.Append(" " + source.JoinType.ToString().ToUpper() + " JOIN " + source.Name + " AS " + source.TableAlias + " ON " + source.JoinCondition);
                }
            }

            if (query.Length > 0)
            {
                query = query.Remove(query.Length - 2, 1); // remove the trailing ,
            }

            if (orderBy.Length > 0)
            {
                orderBy = orderBy.Remove(orderBy.Length - 2, 1); // remove the trailing ,
            }

            // now join our parts
            _query = new StringBuilder();
            _query.Append("SELECT ");
            if (Distinct)
            {
                _query.Append("DISTINCT ");
            }
            _query.Append(query.ToString());
            _query.Append(" ");
            _query.Append(from.ToString());
            _query.Append(" WHERE 1=1 ");

            if (_where.Length > 0)
            {
                _query.Append(_where.ToString());
            }

            if (orderBy.ToString().Length > 0)
            {
                _query.Append(" ORDER BY ");
                _query.Append(orderBy.ToString());
            }
        }
    }
}
