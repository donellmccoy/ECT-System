namespace ALOD.Core.Domain.Query
{
    public class QuerySource
    {
        public QuerySource()
        {
            TableName = "";
            Alias = "";
            AllowMultiple = true;
        }

        public QuerySource(string tableName)
        {
            this.TableName = tableName;
            this.Alias = tableName.Replace(' ', '_');
            this.AllowMultiple = true;
        }

        public QuerySource(string tableName, string alias)
        {
            this.TableName = tableName;
            this.Alias = alias.Replace(' ', '_');
            this.AllowMultiple = true;
        }

        public QuerySource(string tableName, string alias, bool allowMultiple)
        {
            this.TableName = tableName;
            this.Alias = alias.Replace(' ', '_');
            this.AllowMultiple = allowMultiple;
        }

        public QuerySource(string tableName, bool allowMultiple)
        {
            this.TableName = tableName;
            this.Alias = tableName.Replace(' ', '_');
            this.AllowMultiple = allowMultiple;
        }

        public string Alias { get; set; }
        public bool AllowMultiple { get; set; }
        public string TableName { get; set; }
    }
}