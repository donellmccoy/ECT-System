using System;

namespace ALOD.Core.Domain.Query
{
    public class PHQueryOutputField : Entity, IEquatable<PHQueryOutputField>
    {
        public virtual int FieldId { get; set; }
        public virtual int FieldTypeId { get; set; }
        public virtual PHUserQuery Query { get; set; }
        public virtual int SectionId { get; set; }

        public static bool operator !=(PHQueryOutputField lhs, PHQueryOutputField rhs)
        {
            return !(lhs == rhs);
        }

        public static bool operator ==(PHQueryOutputField lhs, PHQueryOutputField rhs)
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
            return this.Equals(obj as PHQueryOutputField);
        }

        /// <inheritdoc/>
        public virtual bool Equals(PHQueryOutputField compareTo)
        {
            if (Object.ReferenceEquals(compareTo, null))
                return false;

            if (Object.ReferenceEquals(this, compareTo))
                return true;

            //if (this.GetType() != compareTo.GetType())
            //    return false;

            return (SectionId == compareTo.SectionId) && (FieldId == compareTo.FieldId) && (FieldTypeId == compareTo.FieldTypeId);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                // 17 & 23...Prime numbers Bones!
                int hash = 17;

                hash = hash * 23 + SectionId.GetHashCode();
                hash = hash * 23 + FieldId.GetHashCode();
                hash = hash * 23 + FieldTypeId.GetHashCode();

                return hash;
            }
        }
    }
}