namespace ALOD.Core.Domain.Query
{
    public class QueryOutputField : Entity
    {
        public virtual string FieldName { get; set; }
        public virtual string QueryType { get; set; }
        public virtual int SortOrder { get; set; }
        public virtual string TableName { get; set; }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(QueryOutputField))
            {
                if (obj == null)
                {
                    return false;
                }

                QueryOutputField field = (QueryOutputField)obj;

                if (string.IsNullOrEmpty(field.FieldName) || string.IsNullOrEmpty(this.FieldName))
                {
                    return false;
                }

                return (string.Compare(this.FieldName, field.FieldName) == 0);
            }

            if (obj.GetType() == typeof(string))
            {
                return (string.Compare(this.FieldName, (string)obj) == 0);
            }

            return base.Equals(obj);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}