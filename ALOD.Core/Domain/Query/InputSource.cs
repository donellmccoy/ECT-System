using System;

namespace ALOD.Core.Domain.Query
{
    public class InputSource : Entity, IEquatable<InputSource>
    {
        public virtual string DataType { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual string FieldName { get; set; }
        public virtual string LookupSort { get; set; }
        public virtual string LookupSource { get; set; }
        public virtual string LookupText { get; set; }
        public virtual string LookupValue { get; set; }
        public virtual string LookupWhere { get; set; }
        public virtual string LookupWhereValue { get; set; }
        public virtual string TableName { get; set; }

        public static bool operator !=(InputSource lhs, InputSource rhs)
        {
            return !(lhs == rhs);
        }

        public static bool operator ==(InputSource lhs, InputSource rhs)
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
            return this.Equals(obj as InputSource);
        }

        /// <inheritdoc/>
        public virtual bool Equals(InputSource compareTo)
        {
            if (Object.ReferenceEquals(compareTo, null))
                return false;

            if (Object.ReferenceEquals(this, compareTo))
                return true;

            //if (this.GetType() != compareTo.GetType())
            //    return false;

            return (Id == compareTo.Id);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                // 17 & 23...Prime numbers Bones!
                int hash = 17;

                hash = hash * 23 + Id.GetHashCode();

                return hash;
            }
        }
    }
}