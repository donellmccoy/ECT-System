using ALOD.Core.Domain.Lookup;
using ALOD.Core.Interfaces;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Core.Utils;
using ALOD.Logging;
using System;
using System.Data;

namespace ALOD.Core.Domain.ServiceMembers
{
    public class ServiceMemberMILPDSChangeHistory : IExtractedEntity
    {
        public ServiceMemberMILPDSChangeHistory()
        {
            SetPropertiesToDefaultValues();
        }

        private ServiceMemberMILPDSChangeHistory(DataSet dSet)
        { }

        public virtual string AttachedPAS { get; set; }
        public virtual DateTime ChangeDate { get; set; }
        public virtual string ChangeType { get; set; }
        public virtual string ChangeTypeAbbreviation { get; set; }
        public virtual DateTime? DateOfBirth { get; set; }
        public virtual Address DomMailingAddress { get; set; }
        public virtual string DutyPhone { get; set; }
        public virtual string DyPosnNr { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string HomePhone { get; set; }
        public virtual string LastName { get; set; }
        public virtual Address LocalMailingAddress { get; set; }
        public virtual ServiceMember Member { get; set; }
        public virtual string MiddleNames { get; set; }

        public virtual string OfficeSymbol { get; set; }
        public virtual string PAS { get; set; }
        public virtual string PASGaining { get; set; }
        public virtual string PASNumber { get; set; }
        public virtual int? RankCode { get; set; }
        public virtual string Sex { get; set; }
        public virtual string Suffix { get; set; }

        /// <inheritdoc/>
        public bool ExtractFromDataRow(DataRow row)
        {
            try
            {
                Member = new ServiceMember();
                FirstName = DataHelpers.GetStringFromDataRow("FIRST_NAME", row);
                LastName = DataHelpers.GetStringFromDataRow("LAST_NAME", row);
                MiddleNames = DataHelpers.GetStringFromDataRow("MIDDLE_NAMES", row);
                Suffix = DataHelpers.GetStringFromDataRow("SUFFIX", row);
                Sex = DataHelpers.GetStringFromDataRow("SEX_SVC_MBR", row);
                DateOfBirth = DataHelpers.GetNullableDateTimeFromDataRow("DOB", row);
                PAS = DataHelpers.GetStringFromDataRow("PAS", row);
                PASGaining = DataHelpers.GetStringFromDataRow("PAS_GAINING", row);
                PASNumber = DataHelpers.GetStringFromDataRow("PAS_NUMBER", row);
                AttachedPAS = DataHelpers.GetStringFromDataRow("ATTACH_PAS", row);
                DutyPhone = DataHelpers.GetStringFromDataRow("COMM_DUTY_PHONE", row);
                HomePhone = DataHelpers.GetStringFromDataRow("HOME_PHONE", row);
                OfficeSymbol = DataHelpers.GetStringFromDataRow("OFFICE_SYMBOL", row);
                RankCode = DataHelpers.GetNullableIntFromDataRow("GR_CURR", row);
                DyPosnNr = DataHelpers.GetStringFromDataRow("DY_POSN_NR", row);
                ChangeTypeAbbreviation = DataHelpers.GetStringFromDataRow("ChangeType", row);
                ChangeDate = DataHelpers.GetDateTimeFromDataRow("Date", row);
                LocalMailingAddress = ExtractLocalMailingAddress(row);
                DomMailingAddress = ExtractDomMailingAddress(row);
                ChangeType = DetermineChangeTypeFromAbbreviation();

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
                bool baseResult = ExtractFromDataRow(row);

                Member = daoFactory.GetMemberDao().GetById(DataHelpers.GetStringFromDataRow("SSAN", row));

                return (baseResult && true);
            }
            catch (Exception e)
            {
                SetPropertiesToDefaultValues();
                LogManager.LogError(e);
                return false;
            }
        }

        protected virtual string DetermineChangeTypeFromAbbreviation()
        {
            switch (ChangeTypeAbbreviation)
            {
                case "A":
                    return "Added";

                case "AD":
                    return "Added After Deletion";

                case "M":
                    return "Modified";

                case "D":
                    return "Deleted";

                default:
                    return "UNKNOWN";
            }
        }

        protected virtual Address ExtractDomMailingAddress(DataRow row)
        {
            Address addr = new Address();

            addr.City = DataHelpers.GetStringFromDataRow("ADRS_MAIL_DOM_CITY", row);
            addr.State = DataHelpers.GetStringFromDataRow("ADRS_MAIL_DOM_STATE", row);
            addr.Zip = DataHelpers.GetStringFromDataRow("ADRS_MAIL_ZIP", row);

            return addr;
        }

        protected virtual Address ExtractLocalMailingAddress(DataRow row)
        {
            Address addr = new Address();

            addr.Street = DataHelpers.GetStringFromDataRow("LOCAL_ADDR_STREET", row);
            addr.City = DataHelpers.GetStringFromDataRow("LOCAL_ADDR_CITY", row);
            addr.State = DataHelpers.GetStringFromDataRow("LOCAL_ADDR_STATE", row);
            addr.Zip = DataHelpers.GetStringFromDataRow("ZIP", row);

            return addr;
        }

        // Duty Position Number?
        protected virtual void SetPropertiesToDefaultValues()
        {
            Member = new ServiceMember();
            FirstName = string.Empty;
            LastName = string.Empty;
            MiddleNames = string.Empty;
            Suffix = string.Empty;
            Sex = string.Empty;
            DateOfBirth = new DateTime();
            PAS = string.Empty;
            PASGaining = string.Empty;
            PASNumber = string.Empty;
            AttachedPAS = string.Empty;
            DutyPhone = string.Empty;
            HomePhone = string.Empty;
            OfficeSymbol = string.Empty;
            RankCode = null;
            DyPosnNr = string.Empty;
            ChangeTypeAbbreviation = string.Empty;
            ChangeType = string.Empty;
            ChangeDate = new DateTime();
            LocalMailingAddress = new Address();
            DomMailingAddress = new Address();
        }
    }
}