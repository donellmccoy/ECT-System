using ALOD.Core.Domain.Common;
using ALOD.Core.Domain.Documents;
using ALOD.Core.Domain.Lookup;
using ALOD.Core.Domain.Modules.Common;
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
    public class SC_PWaivers : SpecialCase
    {
        public const int APPROVED_FINDING_VALUE = 1;
        public const int DENIED_FINDING_VALUE = 0;

        public SC_PWaivers()
        {
            //Testing Required Documents
            AllDocuments.Add(DocumentType.AFForm469.ToString(), false);
            AllDocuments.Add(DocumentType.NarrativeSummary.ToString(), false);
        }

        public virtual DateTime? AlternateApprovalDate { get; set; }
        public virtual DateTime? AlternateExpirationDate { get; set; }
        public virtual int? AlternatePWaiverLength { get; set; }
        public virtual DateTime? ApprovalDate { get; set; }

        /// <inheritdoc/>
        public override int DocumentViewId
        {
            get { return (int)DocumentViewType.PWaivers; }
        }

        /// <inheritdoc/>
        public override DateTime? ExpirationDate { get; set; }

        public virtual string ICD7thCharacter { get; set; }

        // sub-class specific Approvals
        public virtual int? ICD9Code { get; set; }

        public virtual String ICD9Description { get; set; }
        public virtual String ICD9Diagnosis { get; set; }
        public virtual int? MedGroupName { get; set; }
        public virtual string PocEmail { get; set; }
        public virtual string PocPhoneDSN { get; set; }
        public virtual string PocRankAndName { get; set; }
        public virtual int? PWaiverCategory { get; set; }
        public virtual string PWaiverCategoryDescription { get; set; }
        public virtual int? PWaiverLength { get; set; }
        public virtual int? RMUName { get; set; }

        /// <inheritdoc/>
        public override WorkStatus WorkflowStatus { get; set; }

        /// <inheritdoc/>
        public override Dictionary<String, PageAccessType> ReadSectionList(int role)
        {
            PageAccessType access;
            access = PageAccessType.None;

            Dictionary<String, PageAccessType> scAccessList = new Dictionary<String, PageAccessType>();
            if ((CurrentStatusCode == (int)SpecCasePWStatusCode.Approved))
            {
                access = PageAccessType.ReadOnly;
            }

            //'Add all pages as readonly

            scAccessList.Add(PWSectionNames.PW_MED_TECH_INIT.ToString(), access);
            scAccessList.Add(PWSectionNames.PW_HQT_REV.ToString(), access);
            scAccessList.Add(PWSectionNames.PW_BOARD_SG_REV.ToString(), access);
            scAccessList.Add(PWSectionNames.PW_SENIOR_MED_REV.ToString(), access);
            scAccessList.Add(PWSectionNames.PW_HQT_FINAL_REV.ToString(), access);
            scAccessList.Add(PWSectionNames.PW_APPROVED.ToString(), access);
            scAccessList.Add(PWSectionNames.PW_DENIED.ToString(), access);
            scAccessList.Add(PWSectionNames.PW_RLB.ToString(), access);

            //Modify access with user role
            switch (role)
            {
                case (int)UserGroups.MedicalTechnician:
                    if (CurrentStatusCode == (int)SpecCasePWStatusCode.InitiateCase)
                    {
                        scAccessList[PWSectionNames.PW_MED_TECH_INIT.ToString()] = PageAccessType.ReadWrite;
                        scAccessList[PWSectionNames.PW_RLB.ToString()] = PageAccessType.ReadWrite;
                    }
                    break;

                case (int)UserGroups.AFRCHQTechnician:
                    if (CurrentStatusCode == (int)SpecCasePWStatusCode.PackageReview)
                    {
                        scAccessList[PWSectionNames.PW_HQT_REV.ToString()] = PageAccessType.ReadWrite;
                        scAccessList[PWSectionNames.PW_RLB.ToString()] = PageAccessType.ReadOnly;
                    }
                    if (CurrentStatusCode == (int)SpecCasePWStatusCode.FinalReview)
                    {
                        scAccessList[PWSectionNames.PW_HQT_FINAL_REV.ToString()] = PageAccessType.ReadWrite;
                    }
                    break;

                case (int)UserGroups.BoardMedical:
                    if (CurrentStatusCode == (int)SpecCasePWStatusCode.PackageReview)
                    {
                        scAccessList[PWSectionNames.PW_BOARD_SG_REV.ToString()] = PageAccessType.ReadWrite;
                    }
                    break;

                case (int)UserGroups.SeniorMedicalReviewer:
                    if (CurrentStatusCode == (int)SpecCasePWStatusCode.SeniorMedicalReview)
                    {
                        scAccessList[PWSectionNames.PW_SENIOR_MED_REV.ToString()] = PageAccessType.ReadWrite;
                    }
                    break;

                default:
                    break;
            }

            return scAccessList;
        }

        /// <inheritdoc/>
        protected override void ApplyRulesToOption(WorkflowStatusOption o, WorkflowOptionRule r, int lastStatus, IDaoFactory daoFactory)
        {
            IMemoDao2 memoDao = daoFactory.GetMemoDao2();
            ILookupDao lookupDao = daoFactory.GetLookupDao();

            //last status should be the current
            string[] statuses;

            bool allExist;
            bool oneExist;

            if (r.RuleTypes.ruleTypeId == (int)RuleKind.Visibility)
            {
                //Visibility Rule
                switch (r.RuleTypes.Name.ToLower())
                {
                    //If the last status was either of these status codes then the option should be visible
                    //Example-if coming from app_auth or review board fwd to Board Med should not be visible (med tech section)
                    case "laststatuswas":
                        statuses = r.RuleValue.Split(','); //r.RuleValue.ToString().Split(",");
                        if (!statuses.Contains(lastStatus.ToString()))
                        {
                            o.OptionVisible = false;
                        }
                        break;

                    case "laststatuswasnot":
                        //If the last status was either of these status codes then the option should not be visible
                        //Example-if coming from app_auth or review board fwd to Board Med should not be visible (med tech section)
                        statuses = r.RuleValue.ToString().Split(',');
                        if (statuses.Contains(lastStatus.ToString()))
                        {
                            o.OptionVisible = false;
                        }
                        break;

                    case "wasinstatus":
                        bool isVisible = false;
                        statuses = r.RuleValue.ToString().Split(',');

                        IList<WorkStatusTracking> trackingData = lookupDao.GetStatusTracking(this.Id, (byte)moduleId);

                        if (trackingData == null || trackingData.Count == 0)
                        {
                            o.OptionVisible = false;
                            break; // Breaks out of the switch statement
                        }

                        foreach (WorkStatusTracking t in trackingData)
                        {
                            if (statuses.Contains(t.WorkflowStatus.Id.ToString()))
                            {
                                isVisible = true;
                                break; // Breaks out of the foreach loop
                            }
                        }

                        o.OptionVisible = isVisible;
                        break;

                    case "cancompleteasapproved":
                        o.OptionVisible = DetermineVisibilityForCanCompleteCaseRule(1);
                        break;

                    case "cancompleteasdenied":
                        o.OptionVisible = DetermineVisibilityForCanCompleteCaseRule(0);
                        break;

                    case "usergroupcontainsdecision":
                        o.OptionVisible = DetermineVisibilityForUserGroupContainsDecisionRule(new UserGroupContainsDecisionRuleData(r));
                        break;
                }
            }

            if (r.RuleTypes.ruleTypeId == (int)RuleKind.Validation)
            {
                //Validation Rule
                switch (r.RuleTypes.Name.ToLower())
                {
                    case "canforwardcase":
                        bool canforward = ValidateCanForwardCaseRule();

                        if (canforward != true)
                            o.OptionValid = false;
                        else
                            o.OptionValid = true;

                        break;

                    case "memo":
                        string[] memos;
                        allExist = true;
                        oneExist = false;
                        string memoName = String.Empty;
                        memos = r.RuleValue.ToString().Split(',');
                        for (int i = 0; i < memos.Length; i++)
                        {
                            if (!String.IsNullOrEmpty(memos[i]))
                            {
                                memoName = ((MemoType)(Convert.ToByte(memos[i]))).ToString();
                                IList<Memorandum2> memolist = (from m in memoDao.GetByRefnModule(Id, (int)ModuleType.SpecCaseBCMR) where m.Deleted == false && m.Template.Id == Convert.ToByte(memos[i]) select m).ToList<Memorandum2>();
                                if (memolist.Count > 0)
                                {
                                    oneExist = true;
                                }
                                else
                                {
                                    allExist = false;

                                    string description = (from m in memoDao.GetAllTemplates() where m.Id == Convert.ToByte(memos[i]) select m.Title).Single();
                                    AddValidationItem(new ValidationItem("Memos", "Memo", description + "  Memo  not found."));
                                }
                            }
                        }

                        if (r.CheckAll.Value == true)
                        {
                            o.OptionValid = allExist;
                        }
                        else
                        {
                            o.OptionValid = oneExist;
                        }

                        break;

                    case "document":
                        string[] docs = r.RuleValue.ToString().Split(',');
                        bool isValid = false;
                        allExist = true;
                        oneExist = false;

                        //This block is to make sure if there are more then one docs then isvalid is set after checking all the docs are present
                        for (int i = 0; i < docs.Length; i++)
                        {
                            if (!String.IsNullOrEmpty(docs[i]))
                            {
                                string docName = docs[i];
                                isValid = false;
                                if (AllDocuments.ContainsKey(docName))
                                {
                                    isValid = (bool)(Required[docName]);
                                }
                                else
                                {
                                    isValid = true;
                                }
                                if (!isValid)
                                {
                                    allExist = false;
                                }
                                else
                                {
                                    oneExist = true;
                                }
                            }
                        }

                        if (r.CheckAll.Value == true)
                        {
                            o.OptionValid = allExist;
                        }
                        else
                        {
                            o.OptionValid = oneExist;
                        }

                        break;
                }
            }
        }

        protected bool DetermineVisibilityForCanCompleteCaseRule(int expectedFinding)
        {
            if (!med_off_approved.HasValue)
            {
                AddValidationItem(new ValidationItem("Board Medical Officer", "Decision", "Board Medical Officer must provide a finding in order to close out the PW case."));
                return false;
            }

            if (ShouldUseSeniorMedicalReviewerFindings())
            {
                if (!SeniorMedicalReviewerApproved.HasValue || SeniorMedicalReviewerApproved.Value != expectedFinding)
                    return false;
            }
            else
            {
                if (med_off_approved.Value != expectedFinding)
                    return false;
            }

            return true;
        }

        protected bool ValidateCanForwardCaseRule()
        {
            if (CurrentStatusCode == (int)SpecCasePWStatusCode.InitiateCase)
                return ValidateCanForwardCaseRuleForCommonMedTechTabFields("Medical Technician");

            if (CurrentStatusCode == (int)SpecCasePWStatusCode.PackageReview)
                return ValidateCanForwardCaseRuleForCommonMedTechTabFields("HQ AFRC Technician");

            if (CurrentStatusCode == (int)SpecCasePWStatusCode.MedicalReview)
                return ValidateCanForwardCaseRuleForBoardMedicalOfficer();

            if (CurrentStatusCode == (int)SpecCasePWStatusCode.SeniorMedicalReview)
                return ValidateCanForwardCaseRuleForSeniorMedicalReviewer();

            return true;
        }

        private bool ValidateCanForwardCaseRuleForBoardMedicalOfficer()
        {
            bool canForward = true;

            if (!med_off_approved.HasValue)
            {
                AddValidationItem(new ValidationItem("Board Medical Officer", "Decision", "Must provide a Decision for the PW case."));
                canForward = false;
            }
            else if (med_off_approved.Value == APPROVED_FINDING_VALUE)
            {
                if (!ApprovalDate.HasValue)
                {
                    AddValidationItem(new ValidationItem("Board Medical Officer", "Approval Date", "Must enter an Approval Date for the PW case."));
                    canForward = false;
                }

                if (!ExpirationDate.HasValue)
                {
                    AddValidationItem(new ValidationItem("Board Medical Officer", "Expiration Date", "Must enter an Expiration Date for the PW case."));
                    canForward = false;
                }
            }

            return canForward;
        }

        private bool ValidateCanForwardCaseRuleForCommonMedTechTabFields(string userGroupNameBeingValidated)
        {
            bool canForward = true;

            if (!ICD9Code.HasValue || ICD9Code.Value == 0)
            {
                canForward = false;
                AddValidationItem(new ValidationItem(userGroupNameBeingValidated, "ICD9", "Must select a Diagnosis (ICD) for the PW case."));
            }

            if (string.IsNullOrEmpty(ICD9Diagnosis))
            {
                canForward = false;
                AddValidationItem(new ValidationItem(userGroupNameBeingValidated, "ICD Diagnosis", "Must enter an ICD Diagnosis for the PW case."));
            }

            if (!PWaiverCategory.HasValue || PWaiverCategory.Value == 0)
            {
                canForward = false;
                AddValidationItem(new ValidationItem(userGroupNameBeingValidated, "Category", "Must select a Category for the PW case."));
            }

            return canForward;
        }

        private bool ValidateCanForwardCaseRuleForSeniorMedicalReviewer()
        {
            bool canForward = true;

            if (string.IsNullOrEmpty(SeniorMedicalReviewerConcur))
            {
                canForward = false;
                AddValidationItem(new ValidationItem("Senior Medical Reviewer", "Decision", "Must provide a Concur/Non-Concur Decision for the PW case."));
            }
            else
            {
                if (SeniorMedicalReviewerConcur.Equals(DECISION_NONCONCUR))
                {
                    if (!SeniorMedicalReviewerApproved.HasValue)
                    {
                        canForward = false;
                        AddValidationItem(new ValidationItem("Senior Medical Reviewer", "Determination", "Must provide a Decision for the PW case."));
                    }
                    else if (SeniorMedicalReviewerApproved.Value == APPROVED_FINDING_VALUE)
                    {
                        if (!AlternateApprovalDate.HasValue)
                        {
                            canForward = false;
                            AddValidationItem(new ValidationItem("Senior Medical Reviewer", "Approval Date", "Must enter an Approval Date for the PW case."));
                        }

                        if (!AlternateExpirationDate.HasValue)
                        {
                            canForward = false;
                            AddValidationItem(new ValidationItem("Senior Medical Reviewer", "Expiration Date", "Must enter an Expiration Date for the PW case."));
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(SeniorMedicalReviewerComment))
            {
                AddValidationItem(new ValidationItem("Senior Medical Reviewer", "Decision Explanation", "Must provide a Decision Explanation for the PW case."));
                canForward = false;
            }

            return canForward;
        }
    }
}