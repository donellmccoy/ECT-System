using ALOD.Core.Domain.Lookup;
using ALOD.Core.Interfaces;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Core.Utils;
using ALOD.Logging;
using System;
using System.Data;

namespace ALOD.Core.Domain.Modules.SARC
{
    public class RestrictedSARCPostProcessing : IExtractedEntity
    {
        public RestrictedSARCPostProcessing()
        {
            SetPropertiesToDefaultValues();
        }

        private RestrictedSARCPostProcessing(DataRow row)
        { }

        public virtual Address AppealAddress { get; set; }
        public virtual string email { get; set; }
        public virtual string HelpExtensionNumber { get; set; }
        public virtual int Id { get; set; }
        public virtual DateTime? MemberNotificationDate { get; set; }
        public virtual bool? MemberNotified { get; set; }

        /// <inheritdoc/>
        public bool ExtractFromDataRow(DataRow row)
        {
            try
            {
                this.Id = DataHelpers.GetIntFromDataRow("sarc_id", row);
                this.MemberNotified = DataHelpers.GetNullableBoolFromDataRow("memberNotified", row);
                this.MemberNotificationDate = DataHelpers.GetNullableDateTimeFromDataRow("memberNotificationDate", row);
                this.HelpExtensionNumber = DataHelpers.GetStringFromDataRow("helpExtensionNumber", row);
                this.email = DataHelpers.GetStringFromDataRow("email", row);

                this.AppealAddress = new Address();
                this.AppealAddress.Street = DataHelpers.GetStringFromDataRow("appealStreet", row);
                this.AppealAddress.City = DataHelpers.GetStringFromDataRow("appealCity", row);
                this.AppealAddress.State = DataHelpers.GetStringFromDataRow("appealState", row);
                this.AppealAddress.Zip = DataHelpers.GetStringFromDataRow("appealZip", row);
                this.AppealAddress.Country = DataHelpers.GetStringFromDataRow("appealCountry", row);

                return true;
            }
            catch (Exception e)
            {
                SetPropertiesToDefaultValues();
                LogManager.LogError(e);
                return false;
            }
        }

        /// <inheritdoc/>
        public bool ExtractFromDataRow(DataRow row, IDaoFactory daoFactory)
        {
            try
            {
                return ExtractFromDataRow(row);
            }
            catch (Exception e)
            {
                SetPropertiesToDefaultValues();
                LogManager.LogError(e);
                return false;
            }
        }

        protected void SetPropertiesToDefaultValues()
        {
            this.Id = 0;
            this.MemberNotified = null;
            this.MemberNotificationDate = null;
            this.HelpExtensionNumber = string.Empty;
            this.AppealAddress = new Address();
        }
    }
}