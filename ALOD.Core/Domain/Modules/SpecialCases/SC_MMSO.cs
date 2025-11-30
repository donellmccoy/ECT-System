using ALOD.Core.Domain.Common;
using ALOD.Core.Domain.Documents;
using ALOD.Core.Domain.Modules.Lod;
using ALOD.Core.Domain.Users;
using ALOD.Core.Domain.Workflow;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ALOD.Core.Domain.Modules.SpecialCases
{
    [Serializable]
    public class SC_MMSO : SpecialCase
    {
        private const string PARequest = "Pre-Authorization Request";

        public SC_MMSO()
        {
        }

        public virtual DateTime? DateIn { get; set; }
        public virtual DateTime? DateOut { get; set; }

        /// <inheritdoc/>
        public override int DocumentViewId
        {
            get { return (int)DocumentViewType.MMSO; }
        }

        public virtual String Follow_Up_Care { get; set; }
        public virtual String ICD9Id { get; set; }
        public virtual DateTime? InjuryIllnessDate { get; set; }
        public virtual IList<LineOfDutyFindings> LodFindings { get; set; }
        public virtual String Medical_Diagnosis { get; set; }
        public virtual String Medical_Profile_Info { get; set; }
        public virtual String Medical_Provider { get; set; }
        public virtual String Member_Address_City { get; set; }
        public virtual String Member_Address_State { get; set; }
        public virtual string Member_Address_Street { get; set; }
        public virtual String Member_Address_Zip { get; set; }
        public virtual String Member_Home_Phone { get; set; }
        public virtual int? Member_Tricare_Region { get; set; }
        public virtual String Military_Treatment_Facility_Initial { get; set; }
        public virtual String Military_Treatment_Facility_Suggested { get; set; }
        public virtual DateTime? MTF_Initial_Treatment_Date { get; set; }
        public virtual int? MTF_Suggested_Choice { get; set; }
        public virtual int? MTF_Suggested_Distance { get; set; }
        public virtual string Provider_POC { get; set; }
        public virtual string Provider_POC_Phone { get; set; }
        public virtual String Unit_Address1 { get; set; }
        public virtual String Unit_Address2 { get; set; }
        public virtual String Unit_City { get; set; }
        public virtual String Unit_POC_Email { get; set; }
        public virtual String Unit_POC_name { get; set; }
        public virtual String Unit_POC_Phone { get; set; }
        public virtual int? Unit_POC_rank { get; set; }
        public virtual String Unit_POC_title { get; set; }
        public virtual String Unit_State { get; set; }
        public virtual String Unit_UIC { get; set; }
        public virtual String Unit_Zip { get; set; }
        public virtual String UnitOfAssignment { get; set; }

        /// <inheritdoc/>
        public override Dictionary<String, PageAccessType> ReadSectionList(int role)
        {
            PageAccessType access;
            access = PageAccessType.None;

            Dictionary<String, PageAccessType> scAccessList = new Dictionary<String, PageAccessType>();
            if ((Status == (int)SpecCaseMMSOWorkStatus.Approved) || (Status == (int)SpecCaseMMSOWorkStatus.Denied))
            {
                access = PageAccessType.ReadOnly;
            }

            //'Add all pages as readonly
            scAccessList.Add(MMSectionNames.MM_UCINPUT.ToString(), access);
            scAccessList.Add(MMSectionNames.MM_INPUT.ToString(), access);
            scAccessList.Add(MMSectionNames.MM_REVIEW.ToString(), access);

            //Modify access with user role
            switch (role)
            {
                case (int)UserGroups.MedicalOfficer:
                case (int)UserGroups.MedicalTechnician:
                    scAccessList[MMSectionNames.MM_UCINPUT.ToString()] = PageAccessType.ReadOnly;
                    if (Status == (int)SpecCaseMMSOWorkStatus.InitiateCase)
                    {
                        scAccessList[MMSectionNames.MM_INPUT.ToString()] = PageAccessType.ReadWrite;
                    }
                    else
                    {
                        scAccessList[MMSectionNames.MM_INPUT.ToString()] = PageAccessType.ReadOnly;
                    }
                    break;

                case (int)UserGroups.UnitCommander:
                    scAccessList[MMSectionNames.MM_INPUT.ToString()] = PageAccessType.ReadOnly;
                    if (Status == (int)SpecCaseMMSOWorkStatus.SMRInput)
                    {
                        scAccessList[MMSectionNames.MM_UCINPUT.ToString()] = PageAccessType.ReadWrite;
                    }
                    break;

                case (int)UserGroups.MMSOContractRepresentative:
                case (int)UserGroups.MMSONurse:
                case (int)UserGroups.MMSOServicePOS:
                    scAccessList[MMSectionNames.MM_INPUT.ToString()] = PageAccessType.ReadOnly;
                    scAccessList[MMSectionNames.MM_UCINPUT.ToString()] = PageAccessType.ReadOnly;
                    break;

                default:
                    break;
            }
            return scAccessList;
        }

        /// <inheritdoc/>
        public override void Validate()
        {
            string section = string.Empty;
            Validations.Clear();
        }

        /// <summary>
        /// ApplyRulesToOption
        /// </summary>
        /// <param name="o"></param>
        /// <param name="r"></param>
        /// <param name="lastStatus"></param>
        /// <param name="memoDao"></param>
        /// <param name="lookupDao"></param>
        protected override void ApplyRulesToOption(WorkflowStatusOption o, WorkflowOptionRule r, int lastStatus, IDaoFactory daoFactory)
        {
            //last status should be the current
            string[] statuses;

            if (r.RuleTypes.ruleTypeId == (int)RuleKind.Visibility)
            {
                //Visibility Rule
                switch (r.RuleTypes.Name.ToLower())
                {
                    //If the last status was either of these status codes then the option should be visible
                    //Example-if coming from app_auth or review board fwd to med off should not be visible (med tech section)
                    case "laststatuswas":
                        statuses = r.RuleValue.Split(','); //r.RuleValue.ToString().Split(",");
                        if (!statuses.Contains(lastStatus.ToString()))
                        {
                            o.OptionVisible = false;
                        }
                        break;

                    case "laststatuswasnot":
                        //If the last status was either of these status codes then the option should not be visible
                        //Example-if coming from app_auth or review board fwd to med off should not be visible (med tech section)
                        statuses = r.RuleValue.ToString().Split(',');
                        if (statuses.Contains(lastStatus.ToString()))
                        {
                            o.OptionVisible = false;
                        }
                        break;
                }
            }

            if (r.RuleTypes.ruleTypeId == (int)RuleKind.Validation)
            {
                //Validation Rule
                Boolean valid = true;
                switch (r.RuleTypes.Name.ToLower())
                {
                    case "canforwardcase":
                        if (Member_Address_Street == null || Member_Address_Street == "")
                        {
                            AddValidationItem(new ValidationItem("Patient Data", "Address", "Address must not be blank"));
                            valid = false;
                        }
                        if (Medical_Diagnosis == null || Medical_Diagnosis == "")
                        {
                            AddValidationItem(new ValidationItem(PARequest, "Diagnosis", "Diagnosis must not be blank"));
                            valid = false;
                        }
                        if (Follow_Up_Care == null || Follow_Up_Care == "")
                        {
                            AddValidationItem(new ValidationItem(PARequest, "Follow Up Care", "Follow Up Care must not be blank"));
                            valid = false;
                        }
                        if (Medical_Provider == null || Medical_Provider == "")
                        {
                            AddValidationItem(new ValidationItem(PARequest, "Medical Provider", "Provider must not be blank"));
                            valid = false;
                        }
                        if (Provider_POC == null || Provider_POC == "")
                        {
                            AddValidationItem(new ValidationItem(PARequest, "Provider POC", "Provider POC must not be blank"));
                            valid = false;
                        }
                        if (Provider_POC_Phone == null || Provider_POC_Phone == "")
                        {
                            AddValidationItem(new ValidationItem(PARequest, "Provider POC Phone", "Provider POC Phone must not be blank"));
                            valid = false;
                        }
                        if (Medical_Profile_Info == null || Medical_Profile_Info == "")
                        {
                            AddValidationItem(new ValidationItem(PARequest, "Profile Information", "Profile Information must not be blank"));
                            valid = false;
                        }
                        if (!InjuryIllnessDate.HasValue)
                        {
                            AddValidationItem(new ValidationItem(PARequest, "Injury/Illness Date", "Injury/Illness Date must not be blank"));
                        }
                        if (!(DateIn.HasValue && DateOut.HasValue))
                        {
                            AddValidationItem(new ValidationItem(PARequest, "Duty Dates", "Duty Dates must not be blank"));
                        }
                        o.OptionValid = valid;
                        break;

                    case "uccanforwardcase":
                        if (Military_Treatment_Facility_Suggested == null || Military_Treatment_Facility_Suggested == "")
                        {
                            AddValidationItem(new ValidationItem(PARequest, "MTF Facility", "Name of nearest Military Treatment Facility must not be blank"));
                            valid = false;
                        }
                        if (MTF_Suggested_Distance == null)
                        {
                            AddValidationItem(new ValidationItem(PARequest, "MTF Facility Distance", "Distance of nearest Military Treatment Facility must not be blank"));
                            valid = false;
                        }
                        if (MTF_Suggested_Choice == null)
                        {
                            AddValidationItem(new ValidationItem(PARequest, "MTF Distance Type", "Either Place of Duty or Residence must be selected for distance type"));
                            valid = false;
                        }
                        if (Unit_POC_name == null || Unit_POC_name == "")
                        {
                            AddValidationItem(new ValidationItem(PARequest, "Unit POC Name", "Unit POC Name must not be blank"));
                            valid = false;
                        }
                        if (Unit_POC_Phone == null || Unit_POC_Phone == "")
                        {
                            AddValidationItem(new ValidationItem(PARequest, "Unit POC Phone", "Unit POC Phone must not be blank"));
                            valid = false;
                        }
                        if (Unit_POC_title == null || Unit_POC_title == "")
                        {
                            AddValidationItem(new ValidationItem(PARequest, "Unit POC Title", "Unit POC Title must not be blank"));
                            valid = false;
                        }
                        if (Unit_POC_rank == null)
                        {
                            AddValidationItem(new ValidationItem(PARequest, "Unit POC Rank", "Unit POC Rank must be selected"));
                            valid = false;
                        }
                        o.OptionValid = valid;
                        break;
                }
            }
        }

        private DateTime? GetDateFieldValue(object fieldval)
        {
            if (fieldval == DBNull.Value)
            {
                return null;
            }
            return (DateTime)fieldval;
        }

        private string GetFieldValue(object fieldval)
        {
            if (fieldval == DBNull.Value)
            {
                return null;
            }
            return (string)fieldval;
        }

        private int? GetIntFieldValue(object fieldval)
        {
            if (fieldval == DBNull.Value)
            {
                return null;
            }
            return (int)fieldval;
        }
    }
}