using ALOD.Core.Domain.Lookup;
using System;

namespace ALOD.Core.Domain.Modules.Appeals
{
    public class AppealPostProcessing
    {
        public AppealPostProcessing()
        {
        }

        public AppealPostProcessing(int id)
        {
            this.appealId = id;
            this.AppealAddress = new Address();
        }

        public AppealPostProcessing(int id, int initial_id, string helpExtensionNumber, Address appealAddress, DateTime? notificationDate, string email)
        {
            this.AppealAddress = new Address();

            this.appealId = id;
            this.InitialLodId = initial_id;
            this.HelpExtensionNumber = helpExtensionNumber;
            this.AppealAddress = appealAddress;
            this.NotificationDate = notificationDate;
            this.email = email;
        }

        public virtual Address AppealAddress { get; set; }
        public virtual int appealId { get; set; }
        public virtual string email { get; set; }
        public virtual string HelpExtensionNumber { get; set; }
        public virtual int InitialLodId { get; set; }
        public virtual DateTime? NotificationDate { get; set; }
    }
}