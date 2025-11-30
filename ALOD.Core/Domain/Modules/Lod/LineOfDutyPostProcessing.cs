using ALOD.Core.Domain.Lookup;
using System;

namespace ALOD.Core.Domain.Modules.Lod
{
    public class LineOfDutyPostProcessing
    {
        public LineOfDutyPostProcessing()
        {
            this.AppealAddress = new Address();
        }

        public LineOfDutyPostProcessing(int id)
        {
            this.Id = id;
            this.AppealAddress = new Address();
        }

        public LineOfDutyPostProcessing(int id, string helpExtensionNumber, Address appealAddress, string nokFirstName, string nokLastName, string nokMiddleName, DateTime? notificationDate, string email, int chkAddress, int chkPhone, int chkEmail)
        {
            this.AppealAddress = new Address();

            this.Id = id;
            this.HelpExtensionNumber = helpExtensionNumber;
            this.AppealAddress = appealAddress;
            this.NextOfKinFirstName = nokFirstName;
            this.NextOfKinLastName = nokLastName;
            this.NextOfKinMiddleName = nokMiddleName;
            this.NotificationDate = notificationDate;
            this.email = email;
            this.chkAddress = chkAddress;
            this.chkPhone = chkPhone;
            this.chkEmail = chkEmail;
        }

        public virtual Address AppealAddress { get; set; }
        public virtual int chkAddress { get; set; }
        public virtual int chkEmail { get; set; }
        public virtual int chkPhone { get; set; }
        public virtual string email { get; set; }
        public virtual string HelpExtensionNumber { get; set; }
        public virtual int Id { get; set; }
        public virtual string NextOfKinFirstName { get; set; }

        public virtual string NextOfKinFullName
        {
            get
            {
                if (string.IsNullOrEmpty(NextOfKinMiddleName))
                {
                    return NextOfKinFirstName + " " + NextOfKinLastName;
                }
                else
                {
                    return NextOfKinFirstName + " " + NextOfKinMiddleName + " " + NextOfKinLastName;
                }
            }
        }

        public virtual string NextOfKinLastName { get; set; }
        public virtual string NextOfKinMiddleName { get; set; }
        public virtual DateTime? NotificationDate { get; set; }
    }
}