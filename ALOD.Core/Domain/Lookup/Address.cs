using System;

namespace ALOD.Core.Domain.Lookup
{
    [Serializable]
    public class Address
    {
        public virtual string City { get; set; }
        public virtual string Country { get; set; }

        public virtual string FullAddress
        {
            get
            {
                return String.Format("{0}  {1}   {2}  {3}   {4}  ", Street ?? "", City ?? "", Zip ?? "", State ?? "", Country ?? "");
            }
        }

        public virtual string State { get; set; }
        public virtual string Street { get; set; }
        public virtual string Zip { get; set; }
    }
}