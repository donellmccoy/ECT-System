using ALOD.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ALOD.Core.Domain.Query
{
    public class PHUserQuery : Entity, IEquatable<PHUserQuery>
    {
        public PHUserQuery()
        {
            Clauses = new List<PHQueryClause>();
            OutputFields = new List<PHQueryOutputField>();
        }

        public virtual IList<PHQueryClause> Clauses { get; set; }
        public virtual IList<PHQueryOutputField> OutputFields { get; set; }
        public virtual int ReportType { get; set; }
        public virtual string SortField { get; set; }
        public virtual UserQuery UserQuery { get; set; }

        public static bool operator !=(PHUserQuery lhs, PHUserQuery rhs)
        {
            return !(lhs == rhs);
        }

        public static bool operator ==(PHUserQuery lhs, PHUserQuery rhs)
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

        public virtual void AddOutputField(PHQueryOutputField outputField)
        {
            if (outputField == null)
                return;

            if (OutputFields == null)
                OutputFields = new List<PHQueryOutputField>();

            outputField.Query = this;

            OutputFields.Add(outputField);
        }

        public virtual void AddParameter(PHQueryParameter parameter)
        {
            if (Clauses == null || Clauses.Count == 0)
            {
                Clauses = new List<PHQueryClause>();
                PHQueryClause clause = new PHQueryClause();
                clause.Query = this;
                clause.WhereType = WhereType.AND;
                Clauses.Add(clause);
            }

            Clauses[0].AddParameter(parameter);
        }

        public virtual bool ContainsOutputField(PHQueryOutputField outputField)
        {
            if (outputField == null)
                return false;

            if (OutputFields == null)
                return false;

            foreach (PHQueryOutputField f in OutputFields)
            {
                if (f.SectionId == outputField.SectionId && f.FieldId == outputField.FieldId && f.FieldTypeId == outputField.FieldTypeId)
                    return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as PHUserQuery);
        }

        /// <inheritdoc/>
        public virtual bool Equals(PHUserQuery compareTo)
        {
            if (Object.ReferenceEquals(compareTo, null))
                return false;

            if (Object.ReferenceEquals(this, compareTo))
                return true;

            //if (this.GetType() != compareTo.GetType())
            //    return false;

            //if (Shared == false || compareTo.Shared == false)
            //    return false;

            return (CollectionHelpers.ScrambledEquals<PHQueryClause>(Clauses, compareTo.Clauses) && CollectionHelpers.ScrambledEquals<PHQueryOutputField>(OutputFields, compareTo.OutputFields));
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                // 17 & 23...Prime numbers Bones!
                int hash = 17;

                hash = hash * ReportType.GetHashCode();
                hash = hash * SortField.GetHashCode();

                foreach (PHQueryClause c in Clauses.OrderBy(i => i.Id))
                {
                    hash = hash * 23 + c.GetHashCode();
                }

                foreach (PHQueryOutputField f in OutputFields.OrderBy(i => i.Id))
                {
                    hash = hash * 23 + f.GetHashCode();
                }

                return hash;
            }
        }

        public virtual void RemoveOutputField(PHQueryOutputField outputField)
        {
            if (outputField == null)
                return;

            if (OutputFields == null)
                return;

            OutputFields.Remove(outputField);
        }

        public virtual void RemoveParameter(PHQueryParameter parameter)
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