using System.Collections.Generic;

namespace ALOD.Core.Domain.Lookup
{
    public class CaseType
    {
        public CaseType()
        {
            this.Id = 0;
            this.Name = string.Empty;
            this.SubCaseTypes = new List<CaseType>();
        }

        public CaseType(int id, string name)
        {
            this.Id = id;
            this.Name = name;
            this.SubCaseTypes = new List<CaseType>();
        }

        public static string MEPSRequestName
        { get { return "MEPS Request for Physical"; } }

        public static string PalaceChaseFrontName
        { get { return "Palace Chase/Front"; } }

        public static string PalaceChaseName
        { get { return "Palace Chase"; } }

        public static string PalaceFrontName
        { get { return "Palace Front"; } }

        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual IList<CaseType> SubCaseTypes { get; set; }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            CaseType compareTo = obj as CaseType;

            if (ReferenceEquals(this, compareTo))
                return true;

            return (this.Id == compareTo.Id &&
                    this.Name.Equals(compareTo.Name));
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int hashCode = Id.GetHashCode() ^ Name.GetHashCode();

            foreach (CaseType cType in SubCaseTypes)
            {
                hashCode = hashCode ^ cType.GetHashCode();
            }

            return hashCode;
        }
    }
}