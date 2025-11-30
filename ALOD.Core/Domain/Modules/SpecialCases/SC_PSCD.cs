using ALOD.Core.Domain.Common;
using ALOD.Core.Domain.Documents;
using ALOD.Core.Domain.Lookup;
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
    public class SC_PSCD : SpecialCase
    {
        protected IList<ValidationItem> _validations;

        public SC_PSCD()
        {
        }

        public virtual string AccidentOrHistoryDetails { get; set; }

        /// <inheritdoc/>
        public override int DocumentViewId
        {
            get { return (int)DocumentViewType.PSCD; }
        }

        public virtual DateTime? DurationOfServiceFrom { get; set; }

        public virtual DateTime? DurationOfServiceTo { get; set; }

        public virtual string FacilityLocation { get; set; }

        public virtual int? IAWAFI { get; set; }

        public virtual string ICD7thCharacter { get; set; }

        public virtual int? ICD9Code { get; set; }

        public virtual string ICD9Description { get; set; }

        public virtual string ICD9Diagnosis { get; set; }

        public virtual DateTime? InitialTreatmentDate { get; set; }

        public virtual string MemberCategory { get; set; }

        //Med Tech tab
        public virtual string MemberStatus { get; set; }

        public virtual string PocEmail { get; set; }
        public virtual string PocPhoneDSN { get; set; }
        public virtual string PocRankAndName { get; set; }
        public virtual IList<SC_PSCD_Findings> PSCDFindings { get; set; }
        public virtual int? RMU { get; set; }

        /// <inheritdoc/>
        public override Dictionary<String, PageAccessType> ReadSectionList(int role)
        {
            PageAccessType access;
            access = PageAccessType.None;

            Dictionary<String, PageAccessType> scAccessList = new Dictionary<String, PageAccessType>();

            //Add All Pages as Readonly
            scAccessList.Add(PSCDSectionNames.PSCD_RLB.ToString(), access);
            scAccessList.Add(PSCDSectionNames.PSCD_MEDTECH_INIT.ToString(), access);
            scAccessList.Add(PSCDSectionNames.PSCD_MEDOFF.ToString(), access);
            scAccessList.Add(PSCDSectionNames.PSCD_HQMED.ToString(), access);
            scAccessList.Add(PSCDSectionNames.PSCD_BOARDMED.ToString(), access);
            scAccessList.Add(PSCDSectionNames.PSCD_SENIORMED.ToString(), access);
            scAccessList.Add(PSCDSectionNames.PSCD_BOARDLEGAL.ToString(), access);
            scAccessList.Add(PSCDSectionNames.PSCD_BOARDADMIN.ToString(), access);
            scAccessList.Add(PSCDSectionNames.PSCD_APPROVINGAUTHORITY.ToString(), access);

            //Set Read/Write
            switch (role)
            {
                case (int)UserGroups.BoardTechnician:
                    if (CurrentStatusCode == (int)SpecCasePSCDStatusCode.HQMedTech || CurrentStatusCode == 300) // PS Board Technican Input
                    {
                        scAccessList[PSCDSectionNames.PSCD_HQMED.ToString()] = PageAccessType.ReadWrite;
                    }
                    break;

                case (int)UserGroups.BoardMedical:
                    if (CurrentStatusCode == (int)SpecCasePSCDStatusCode.BoardMedicalOfficerHQ || CurrentStatusCode == 301) //PS Medical Officer (HQ) Review
                    {
                        scAccessList[PSCDSectionNames.PSCD_BOARDMED.ToString()] = PageAccessType.ReadWrite;
                    }
                    break;

                case (int)UserGroups.SeniorMedicalReviewer:
                    if (CurrentStatusCode == (int)SpecCasePSCDStatusCode.BoardMedicalOfficerHQSMR || CurrentStatusCode == 302) //PS Medical Officer (HQ SMR) Review
                    {
                        scAccessList[PSCDSectionNames.PSCD_SENIORMED.ToString()] = PageAccessType.ReadWrite;
                    }
                    break;

                case (int)UserGroups.BoardLegal:
                    if (CurrentStatusCode == (int)SpecCasePSCDStatusCode.AFRCJA || CurrentStatusCode == 303)// PS Board Legal Review
                    {
                        scAccessList[PSCDSectionNames.PSCD_BOARDLEGAL.ToString()] = PageAccessType.ReadWrite;
                    }
                    break;

                case (int)UserGroups.BoardAdministrator:
                    if (CurrentStatusCode == (int)SpecCasePSCDStatusCode.BoardPersonnel)// PS Board Personnel (Board Admin) Review
                    {
                        scAccessList[PSCDSectionNames.PSCD_BOARDADMIN.ToString()] = PageAccessType.ReadWrite;
                    }
                    break;

                case (int)UserGroups.BoardApprovalAuthority:
                    if (CurrentStatusCode == (int)SpecCasePSCDStatusCode.ApprovingAuthority || CurrentStatusCode == 304)//PS Approving Authority Action
                    {
                        scAccessList[PSCDSectionNames.PSCD_APPROVINGAUTHORITY.ToString()] = PageAccessType.ReadWrite;
                    }
                    break;
            }

            return scAccessList;
        }

        #region Findings

        public virtual SC_PSCD_Findings FindByType(short pType)
        {
            try
            {
                IEnumerable<SC_PSCD_Findings> lst = (from p in PSCDFindings
                                                     where p.PType == pType
                                                     select p);
                if (lst.Count() > 0)
                {
                    SC_PSCD_Findings old = lst.First();
                    return old;
                }
            }
            catch
            {
            }
            return null;
        }

        public virtual SC_PSCD_Findings SetFindingByType(SC_PSCD_Findings fnd)
        {
            int i = 0;

            if (PSCDFindings.Count > 0)
            {
                foreach (SC_PSCD_Findings item in PSCDFindings)
                {
                    if (item.PType == fnd.PType)
                    {
                        PSCDFindings.ElementAt(i).Modify(fnd);
                        return PSCDFindings.ElementAt(i);
                    }
                    i = i + 1;
                }
            }
            PSCDFindings.Add(fnd);
            //PSCDFindings.Add(fnd);

            return PSCDFindings.ElementAt(i);
        }

        #endregion Findings

        #region workflow options and rules

        /// <inheritdoc/>
        protected override void ApplyRulesToOption(WorkflowStatusOption o, WorkflowOptionRule r, int lastStatus, IDaoFactory daoFactory)
        {
            IMemoDao2 memoDao = daoFactory.GetMemoDao2();
            ILookupDao lookupDao = daoFactory.GetLookupDao();

            //string[] statuses;

            //bool allExist;
            //bool oneExist;

            if (r.RuleTypes.ruleTypeId == (int)RuleKind.Visibility)
            {
                ////Visibility Rules
                //switch (r.RuleTypes.Name.ToLower()) -- used for adding cases to the Visibility Rules
                //{
                //}
            }

            if (r.RuleTypes.ruleTypeId == (int)RuleKind.Validation)
            {
                //Validation Rule
                switch (r.RuleTypes.Name.ToLower())
                {
                    case "canforwardcase":
                        bool canforward = ValidateCanForwardCaseRule(lookupDao);

                        if (canforward) o.OptionValid = true;
                        else o.OptionValid = false;
                        break;
                }
            }
        }

        private bool ValidateCanForwardCaseRule(ILookupDao lookupDao)
        {
            bool canForward = true;
            string user = string.Empty;

            if (CurrentStatusCode == (int)SpecCasePSCDStatusCode.MedTechInitiate)
            {
                user = "Med Tech";

                if (String.IsNullOrEmpty(MemberStatus))
                {
                    canForward = false;
                    AddValidationItem(new ValidationItem(user, "MemberStatus", "Must select a Member Status."));
                }
                if (String.IsNullOrEmpty(MemberCategory))
                {
                    canForward = false;
                    AddValidationItem(new ValidationItem(user, "MemberCategory", "Must select a Member Category."));
                }

                if (!(ICD9Code.HasValue))
                {
                    canForward = false;
                    AddValidationItem(new ValidationItem(user, "ddlICDChapter,ddlICDSection,ddlICDDiagnosisLevel1,ddlICDDiagnosisLevel2,ddlICDDiagnosisLevel3,ddlICDDiagnosisLevel4", "ICD code  is required"));
                }

                if (String.IsNullOrEmpty(ICD9Diagnosis))
                {
                    canForward = false;
                    AddValidationItem(new ValidationItem(user, "ICD9Diagnosis", "Diagnosis Text is required."));
                }
            }

            return canForward;
        }

        #endregion workflow options and rules
    }
}