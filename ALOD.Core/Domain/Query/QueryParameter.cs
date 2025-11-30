using System;

namespace ALOD.Core.Domain.Query
{
    public class QueryParameter : Entity, IEquatable<QueryParameter>
    {
        public virtual QueryClause Clause { get; set; }
        public virtual string EndDisplay { get; set; }
        public virtual string EndValue { get; set; }
        public virtual int ExecuteOrder { get; set; }
        public virtual string Operator { get; set; }
        public virtual InputSource Source { get; set; }
        public virtual string StartDisplay { get; set; }
        public virtual string StartValue { get; set; }
        public virtual string WhereType { get; set; }

        public static bool operator !=(QueryParameter lhs, QueryParameter rhs)
        {
            return !(lhs == rhs);
        }

        public static bool operator ==(QueryParameter lhs, QueryParameter rhs)
        {
            // Check for null on left side...
            if (Object.ReferenceEquals(lhs, null))
            {
                // Check for null on right side...
                if (Object.ReferenceEquals(rhs, null))
                {
                    // null == null = true
                    return true;
                }

                // Only the left side is null...
                return false;
            }

            // Equals handles the case of null on the right side...
            return lhs.Equals(rhs);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as QueryParameter);
        }

        /// <inheritdoc/>
        public virtual bool Equals(QueryParameter compareTo)
        {
            if (Object.ReferenceEquals(compareTo, null))
                return false;

            if (Object.ReferenceEquals(this, compareTo))
                return true;

            //if (this.GetType() != compareTo.GetType())
            //    return false;

            return (Source.Equals(compareTo.Source)) && (String.Equals(Operator, compareTo.Operator)) && (String.Equals(WhereType, compareTo.WhereType));
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                // 17 & 23...Prime numbers Bones!
                int hash = 17;

                hash = hash * 23 + Operator.GetHashCode();
                hash = hash * 23 + WhereType.GetHashCode();
                hash = hash * 23 + Source.GetHashCode();

                return hash;
            }
        }
    }
}