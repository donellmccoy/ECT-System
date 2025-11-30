using ALOD.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ALOD.Core.Domain.Query
{
    public class PHQueryClause : Entity, IEquatable<PHQueryClause>
    {
        public PHQueryClause()
        {
            Parameters = new List<PHQueryParameter>();
        }

        public virtual IList<PHQueryParameter> Parameters { get; set; }
        public virtual PHUserQuery Query { get; set; }
        public virtual string WhereType { get; set; }

        public static bool operator !=(PHQueryClause lhs, PHQueryClause rhs)
        {
            return !(lhs == rhs);
        }

        public static bool operator ==(PHQueryClause lhs, PHQueryClause rhs)
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

        public virtual void AddParameter(PHQueryParameter parameter)
        {
            if (Parameters == null)
            {
                Parameters = new List<PHQueryParameter>();
            }

            parameter.Clause = this;
            Parameters.Add(parameter);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as PHQueryClause);
        }

        /// <inheritdoc/>
        public virtual bool Equals(PHQueryClause compareTo)
        {
            if (Object.ReferenceEquals(compareTo, null))
                return false;

            if (Object.ReferenceEquals(this, compareTo))
                return true;

            //if (this.GetType() != compareTo.GetType())
            //    return false;

            return (String.Equals(WhereType, compareTo.WhereType)) && (CollectionHelpers.ScrambledEquals<PHQueryParameter>(Parameters, compareTo.Parameters));
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                // 17 & 23...Prime numbers Bones!
                int hash = 17;

                hash = hash * 23 + WhereType.GetHashCode();

                foreach (PHQueryParameter p in Parameters.OrderBy(i => i.Id))
                {
                    hash = hash * 23 + p.GetHashCode();
                }

                return hash;
            }
        }

        public virtual void RemoveParameter(PHQueryParameter param)
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