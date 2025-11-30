using ALOD.Core.Domain.Lookup;
using ALOD.Core.Domain.Users;
using System;

namespace ALOD.Core.Domain.ServiceMembers
{
    public class ServiceMember : EntityWithTypedId<string>
    {
        public ServiceMember()
        { }

        public ServiceMember(string ssn)
        {
            this.Id = ssn;
        }

        public virtual Unit AttachUnit { get; set; }

        public virtual string Component { get; set; }

        public virtual DateTime? DateOfBirth { get; set; }

        public virtual string DutyPhone { get; set; }

        public virtual string FirstName { get; set; }

        public virtual string FullName
        {
            get
            {
                if (string.IsNullOrEmpty(MiddleName))
                {
                    return LastName + " " + FirstName;
                }
                else
                {
                    if (MiddleName.Length == 1)
                        return LastName + " " + FirstName + " " + MiddleName;
                    else
                        return LastName + " " + FirstName + " " + MiddleName.Substring(0, 1);
                }
            }
        }

        public virtual string LastName { get; set; }

        public virtual Address MailingAddress { get; set; }

        public virtual string MiddleInitial
        {
            get
            {
                if (string.IsNullOrEmpty(MiddleName))
                    return string.Empty;

                return MiddleName[0].ToString();
            }
        }

        public virtual string MiddleName { get; set; }

        public virtual string MinifiedSSN
        {
            get
            {
                if (string.IsNullOrEmpty(SSN))
                    return string.Empty;

                if (SSN.Length <= 3)
                    return SSN;

                return SSN.Substring(SSN.Length - 4, 4);
            }
        }

        public virtual UserRank Rank { get; set; }

        public virtual string SSN
        { get { return Id; } }

        public virtual Unit Unit { get; set; }
    }
}