using ALOD.Core.Domain.Users;
using ALOD.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ALOD.Core.Domain.Query
{
    public class UserQuery : Entity, IEquatable<UserQuery>
    {
        private const string FIELD_SEPERATOR = "|";

        public UserQuery()
        {
            Clauses = new List<QueryClause>();
        }

        public virtual IList<QueryClause> Clauses { get; set; }
        public virtual DateTime CreatedDate { get; set; }
        public virtual string FieldList { get; set; }
        public virtual DateTime ModifiedDate { get; set; }

        public virtual IList<string> OutputFields
        {
            get
            {
                if (string.IsNullOrEmpty(FieldList))
                {
                    FieldList = "";
                }

                return FieldList.Split(new string[] { FIELD_SEPERATOR },
                    StringSplitOptions.RemoveEmptyEntries).ToList<string>();
            }
        }

        public virtual Boolean Shared { get; set; }
        public virtual string SortFields { get; set; }
        public virtual string Title { get; set; }
        public virtual Boolean Transient { get; set; }
        public virtual AppUser User { get; set; }

        public static bool operator !=(UserQuery lhs, UserQuery rhs)
        {
            return !(lhs == rhs);
        }

        public static bool operator ==(UserQuery lhs, UserQuery rhs)
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
            if (Clauses == null || Clauses.Count == 0)
            {
                Clauses = new List<QueryClause>();
                QueryClause clause = new QueryClause();
                clause.Query = this;
                clause.WhereType = WhereType.AND;
                Clauses.Add(clause);
            }

            Clauses[0].AddParameter(parameter);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as UserQuery);
        }

        /// <inheritdoc/>
        public virtual bool Equals(UserQuery compareTo)
        {
            if (Object.ReferenceEquals(compareTo, null))
                return false;

            if (Object.ReferenceEquals(this, compareTo))
                return true;

            //if (this.GetType() != compareTo.GetType())
            //    return false;

            //if (Shared == false || compareTo.Shared == false)
            //    return false;

            return CollectionHelpers.ScrambledEquals<QueryClause>(Clauses, compareTo.Clauses);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                // 17 & 23...Prime numbers Bones!
                int hash = 17;

                foreach (QueryClause c in Clauses.OrderBy(i => i.Id))
                {
                    hash = hash * 23 + c.GetHashCode();
                }

                return hash;
            }
        }

        public virtual QuerySource GetPrimaryQuerySource()
        {
            foreach (QueryClause clause in Clauses)
            {
                foreach (QueryParameter param in clause.Parameters.OrderBy(x => x.ExecuteOrder))
                {
                    return new QuerySource(param.Source.TableName);
                }
            }

            return null;
        }

        public virtual void RemoveParameter(QueryParameter parameter)
        {
            if (Clauses == null || Clauses.Count == 0)
            {
                //with no clauses, we have no params to remove, so we're done
                return;
            }

            Clauses[0].RemoveParameter(parameter);
        }
    }
}