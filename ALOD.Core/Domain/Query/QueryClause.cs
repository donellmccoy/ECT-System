using ALOD.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ALOD.Core.Domain.Query
{
    public class QueryClause : Entity, IEquatable<QueryClause>
    {
        public QueryClause()
        {
            Parameters = new List<QueryParameter>();
        }

        public virtual IList<QueryParameter> Parameters { get; set; }
        public virtual UserQuery Query { get; set; }
        public virtual string WhereType { get; set; }

        public static bool operator !=(QueryClause lhs, QueryClause rhs)
        {
            return !(lhs == rhs);
        }

        public static bool operator ==(QueryClause lhs, QueryClause rhs)
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

        public virtual void AddParameter(QueryParameter parameter)
        {
            if (Parameters == null)
            {
                Parameters = new List<QueryParameter>();
            }

            parameter.Clause = this;
            Parameters.Add(parameter);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as QueryClause);
        }

        /// <inheritdoc/>
        public virtual bool Equals(QueryClause compareTo)
        {
            if (Object.ReferenceEquals(compareTo, null))
                return false;

            if (Object.ReferenceEquals(this, compareTo))
                return true;

            //if (this.GetType() != compareTo.GetType())
            //    return false;

            return (String.Equals(WhereType, compareTo.WhereType)) && (CollectionHelpers.ScrambledEquals<QueryParameter>(Parameters, compareTo.Parameters));
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                // 17 & 23...Prime numbers Bones!
                int hash = 17;

                hash = hash * 23 + WhereType.GetHashCode();

                foreach (QueryParameter p in Parameters.OrderBy(i => i.Source.Id))
                {
                    hash = hash * 23 + p.GetHashCode();
                }

                return hash;
            }
        }

        public virtual void RemoveParameter(QueryParameter param)
        {
            if (Parameters == null)
            {
                //nothing to remove
                return;
            }

            Parameters.Remove(param);
        }
    }
}