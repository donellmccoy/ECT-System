using System;

namespace ALOD.Core.Domain.Modules.Lod
{
    public class SignatureEntry
    {
        public virtual DateTime? DateSigned { get; set; }

        public virtual bool IsSigned
        {
            get
            {
                if (DateSigned == null)
                { return false; }
                else
                { return DateSigned.HasValue; }
            }
        }

        public virtual string NameAndRank { get; set; }
        public virtual string Title { get; set; }
    }
}