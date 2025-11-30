using ALOD.Core.Domain.Common;
using ALOD.Core.Domain.Documents;
using ALOD.Core.Domain.Modules.Common;
using ALOD.Core.Domain.Modules.Lod;
using ALOD.Core.Domain.Users;
using ALOD.Core.Domain.Workflow;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Core.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ALOD.Core.Domain.Modules.SpecialCases
{
    [Serializable]
    public class SC_MH : SpecialCase
    {
        public const int APPROVED_FINDING_VALUE = 1;
        public const int DENIED_FINDING_VALUE = 0;

        //private int AdminLODUploaded = 0;
        private int DispositionUploaded = 0;

        public SC_MH()
        {
        }

        public virtual int? ALCLetterType { get; set; }

        public virtual DateTime? AlternateExpirationDate { get; set; }

        public virtual int? ApprovingAuthorityType { get; set; }

        public virtual int? AssignmentLimitation { get; set; }

        /// <inheritdoc/>
        public override int? AssociatedWWD { get; set; }

        public virtual int? DAWGRecommendation { get; set; }

        public virtual string DeciExpl { get; set; }

        /// <inheritdoc/>
        public override int DocumentViewId
        {
            get { return (int)DocumentViewType.MH; }
        }

        public virtual DateTime? EffectiveDate { get; set; }

        /// <inheritdoc/>
        public override DateTime? ExpirationDate { get; set; }

        public virtual DateTime? ForwardDate { get; set; }

        // HQ
        public virtual DateTime? HighTenureDate { get; set; }

        public virtual string ICD7thCharacter { get; set; }
        public virtual int? ICD9Code { get; set; }
        public virtual string ICD9Description { get; set; }
        public virtual string ICD9Diagnosis { get; set; }

        // All are currently inherited from base
        public virtual IList<LineOfDutyFindings> LodFindings { get; set; }

        //  Do we need these?
        public virtual int? MedGroupName { get; set; }

        public virtual int? MedOffConcur { get; set; }

        public virtual DateTime? MemberNotifiedDate { get; set; }

        public virtual int? MemberStatus { get; set; }

        //Decision Explanation
        public virtual DateTime? ReturnToDutyDate { get; set; }

        public virtual int? RMUName { get; set; }

        // Board Medical
        public virtual new DateTime? sig_date_med_off { get; set; }

        /// <inheritdoc/>
        public override Dictionary<string, PageAccessType> ReadSectionList(int role)
        {
            PageAccessType access;
            access = PageAccessType.None;

            Dictionary<string, PageAccessType> scAccessList = new Dictionary<string, PageAccessType>();
            if ((CurrentStatusCode == (int)SpecCaseMHStatusCode.Approved) || (CurrentStatusCode == (int)SpecCaseMHStatusCode.Disapproved))
            {
                access = PageAccessType.ReadOnly;
            }

            //'Add all pages as readonly
            scAccessList.Add(MHSectionNames.MH_RLB.ToString(), access);
            scAccessList.Add(MHSectionNames.MH_HQT_INIT.ToString(), access);
            scAccessList.Add(MHSectionNames.MH_BOARD_SG.ToString(), access);
            scAccessList.Add(MHSectionNames.MH_SENIOR_MED_REV.ToString(), access);
            scAccessList.Add(MHSectionNames.MH_HQT_FINAL_REV.ToString(), access);
            scAccessList.Add(MHSectionNames.MH_APPROVED.ToString(), access);
            scAccessList.Add(MHSectionNames.MH_DISAPPROVED.ToString(), access);

            //Modify access with user role
            switch (role)
            {
                case (int)UserGroups.AFRCHQTechnician:
                    if (CurrentStatusCode == (int)SpecCaseMHStatusCode.InitiateCase || CurrentStatusCode == (int)SpecCaseMHStatusCode.Holding)
                    {
                        scAccessList[MHSectionNames.MH_HQT_INIT.ToString()] = PageAccessType.ReadWrite;
                        scAccessList[MHSectionNames.MH_BOARD_SG.ToString()] = PageAccessType.ReadOnly;
                    }
                    if (CurrentStatusCode == (int)SpecCaseMHStatusCode.FinalReview)
                    {
                        scAccessList[MHSectionNames.MH_HQT_FINAL_REV.ToString()] = PageAccessType.ReadWrite;
                        scAccessList[MHSectionNames.MH_BOARD_SG.ToString()] = PageAccessType.ReadOnly;
                    }

                    break;

                case (int)UserGroups.BoardMedical:
                    if (CurrentStatusCode == (int)SpecCaseMHStatusCode.MedicalReview)
                    {
                        scAccessList[MHSectionNames.MH_BOARD_SG.ToString()] = PageAccessType.ReadWrite;
                    }
                    break;

                case (int)UserGroups.SeniorMedicalReviewer:
                    if (CurrentStatusCode == (int)SpecCaseMHStatusCode.SeniorMedicalReview)
                    {
                        scAccessList[MHSectionNames.MH_SENIOR_MED_REV.ToString()] = PageAccessType.ReadWrite;
                    }
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

        /// <inheritdoc/>
        protected override void ApplyRulesToOption(WorkflowStatusOption o, WorkflowOptionRule r, int lastStatus, IDaoFactory daoFactory)
        {
            IMemoDao2 memoDao = daoFactory.GetMemoDao2();

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
                        statuses = r.RuleValue.Split(',');
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

                        o.OptionValid = ValidateCanForwardCaseRule();

                        break;

                    case "memo":
                        string[] memos;
                        allExist = true;
                        oneExist = false;
                        string memoName = string.Empty;
                        memos = r.RuleValue.ToString().Split(',');
                        for (int i = 0; i < memos.Length; i++)
                        {
                            if (!string.IsNullOrEmpty(memos[i]))
                            {
                                memoName = ((MemoType)(Convert.ToByte(memos[i]))).ToString();
                                IList<Memorandum2> memolist = (from m in memoDao.GetByRefnModule(Id, (int)ModuleType.SpecCaseMH)
                                                               where m.Deleted == false && m.Template.Id == Convert.ToByte(memos[i])
                                                               select m).ToList<Memorandum2>();
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
                            if (!string.IsNullOrEmpty(docs[i]))
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
                AddValidationItem(new ValidationItem("Board Medical Officer", "Decision", "Board Medical Officer must provide a finding in order to close out the MH case."));
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

        /// <inheritdoc/>
        protected override void PerformAdditionalDocumentProcessing(string docName, bool docFound, IDaoFactory daoFactory)
        {
            if (docFound && docName == DocumentType.Disposition.ToString())
                DispositionUploaded = 1;
        }

        /// <inheritdoc/>
        protected override void UpdateDocumentCategoryValidation(string docName, bool isRequired, bool isValid, List<DocumentCategory2> viewCats, IDaoFactory daoFactory)
        {
            if (AllDocuments.ContainsKey(docName))
            {
                bool newIsRequired = isRequired;

                if (!isValid)
                {
                    if ((DispositionUploaded == 1) && (docName == "Disposition"))
                    {
                        //This is used to trap selections that of associate LOD and no adminlod which is valid.
                        newIsRequired = true;
                        Required[docName] = true;
                    }
                    else
                    {
                        AddDocumentCategoryValidationItem(docName, viewCats);
                    }
                }

                AllDocuments[docName] = newIsRequired;
            }
        }

        protected bool ValidateCanForwardCaseRule()
        {
            if (CurrentStatusCode == (int)SpecCaseMHStatusCode.InitiateCase || CurrentStatusCode == (int)SpecCaseMHStatusCode.Holding)
                return ValidateCanForwardCaseRuleForCommonHQTechTabFields();

            if (CurrentStatusCode == (int)SpecCaseMHStatusCode.MedicalReview)
                return ValidateCanForwardCaseRuleForBoardSGTabFields();

            if (CurrentStatusCode == (int)SpecCaseMHStatusCode.SeniorMedicalReview)
                return ValidateCanForwardCaseRuleForSMRTabFields();

            return true;
        }

        private bool ValidateCanForwardCaseRuleForBoardSGTabFields()
        {
            bool canForward = true;

            if (med_off_approved == null)
            {
                canForward = false;
                AddValidationItem(new ValidationItem("Board Medical", "Determination", "Must provide a Determination for the MH case."));
            }

            if (string.IsNullOrEmpty(med_off_approval_comment))
            {
                canForward = false;
                AddValidationItem(new ValidationItem("Board Medical", "Decision Explanation", "Must provide a Decision Explanation for the MH case."));
            }

            if (med_off_approved == 1 && ExpirationDate == null)
            {
                canForward = false;
                AddValidationItem(new ValidationItem("Board Medical", "Expiration Date", "Must provide an Expiration Date for the MH case."));
            }

            return canForward;
        }

        private bool ValidateCanForwardCaseRuleForCommonHQTechTabFields()
        {
            bool canForward = true;

            if ((ICD9Code == null) || (ICD9Code == 0))
            {
                canForward = false;
                AddValidationItem(new ValidationItem("HQ ARFC Tech", "ICD9", "Must select a Diagnosis (ICD) for the MH case."));
            }

            if ((ICD9Diagnosis == null || ICD9Diagnosis == ""))
            {
                canForward = false;
                AddValidationItem(new ValidationItem("HQ ARFC Tech", "ICD9 Description", "Must enter a Diagnosis Description for the MH case."));
            }

            if (HighTenureDate == null)
            {
                canForward = false;
                AddValidationItem(new ValidationItem("HQ ARFC Tech", "High Tenure Date", "Must enter High Tenure Date for the MH case."));
            }

            return canForward;
        }

        private bool ValidateCanForwardCaseRuleForSMRTabFields()
        {
            bool canForward = true;

            if (string.IsNullOrEmpty(SeniorMedicalReviewerConcur))
            {
                canForward = false;
                AddValidationItem(new ValidationItem("Senior Medical Reviewer", "Decision", "Must provide a Decision for the MH case."));
            }
            else
            {
                if (SeniorMedicalReviewerConcur.Equals(DECISION_NONCONCUR) && !SeniorMedicalReviewerApproved.HasValue)
                {
                    canForward = false;
                    AddValidationItem(new ValidationItem("Senior Medical Reviewer", "Determination", "Must provide a Determination for the MH case."));
                }
                else if (SeniorMedicalReviewerConcur.Equals(DECISION_NONCONCUR) && SeniorMedicalReviewerApproved == APPROVED_FINDING_VALUE && AlternateExpirationDate == null)
                {
                    canForward = false;
                    AddValidationItem(new ValidationItem("Board Medical", "Expiration Date", "Must provide an Expiration Date for the MH case."));
                }
            }

            if (med_off_approved.HasValue && string.IsNullOrEmpty(SeniorMedicalReviewerConcur))
            {
                canForward = false;
                AddValidationItem(new ValidationItem("Senior Medical Reviewer", "Decision", "Must provide a Decision for the MH case."));
            }
            else
            {
                if ((!med_off_approved.HasValue || SeniorMedicalReviewerConcur.Equals("N")) && !SeniorMedicalReviewerApproved.HasValue)
                {
                    canForward = false;
                    AddValidationItem(new ValidationItem("Senior Medical Reviewer", "Determination", "Must provide a Determination for the MH case."));
                }
            }

            if (string.IsNullOrEmpty(SeniorMedicalReviewerComment))
            {
                canForward = false;
                AddValidationItem(new ValidationItem("Senior Medical Reviewer", "Decision Explanation", "Must provide a Decision Explanation for the MH case."));
            }

            return canForward;
        }
    }
}