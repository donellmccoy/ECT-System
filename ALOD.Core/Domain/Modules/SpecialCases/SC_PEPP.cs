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
using System.Data;
using System.Linq;

namespace ALOD.Core.Domain.Modules.SpecialCases
{
    [Serializable]
    public class SC_PEPP : SpecialCase
    {
        public const int DISQUALIFY_FINDING_VALUE = 0;
        public const int QUALIFY_FINDING_VALUE = 1;

        public SC_PEPP()
        {
        }

        public virtual int? ALCLetterType { get; set; }
        public virtual DateTime? AlternateCertificationDate { get; set; }
        public virtual string AlternateDQParagraph { get; set; }
        public virtual DateTime? AlternateExpirationDate { get; set; }
        public virtual int? BaseAssign { get; set; }
        public virtual int? CaseType { get; set; }
        //public virtual int? SubType { get; set; }

        public virtual DateTime? CertificationDate { get; set; }

        public virtual int? CompletedByUnit { get; set; }

        public virtual DateTime? DateOut { get; set; }

        public virtual DateTime? DateReceived { get; set; }

        public virtual int? Disposition { get; set; }

        /// <inheritdoc/>
        public override int DocumentViewId
        {
            get { return (int)DocumentViewType.PEPP; }
        }

        // Medical Officer tab properties
        public virtual string DQParagraph { get; set; }

        public virtual string ICD7thCharacter { get; set; }

        // HQ AFRC Tech tab properties
        public virtual int? ICD9Code { get; set; }

        public virtual string ICD9Description { get; set; }
        public virtual string ICD9Diagnosis { get; set; }
        public virtual bool? IsWaiverRequired { get; set; }
        public virtual int? Rating { get; set; }
        public virtual string RatingName { get; set; }
        public virtual int? Renewal { get; set; }
        public virtual int? Type { get; set; }
        public virtual string TypeName { get; set; }
        public virtual DateTime? WaiverExpirationDate { get; set; }

        /// <inheritdoc/>
        public override Dictionary<String, PageAccessType> ReadSectionList(int role)
        {
            PageAccessType access;
            access = PageAccessType.None;

            Dictionary<String, PageAccessType> scAccessList = new Dictionary<String, PageAccessType>();
            if ((CurrentStatusCode == (int)SpecCasePEPPStatusCode.Complete))
            {
                access = PageAccessType.ReadOnly;
            }

            //'Add all pages as readonly
            scAccessList.Add(PESectionNames.PE_RLB.ToString(), access);
            scAccessList.Add(PESectionNames.PE_HQT_INIT.ToString(), access);
            scAccessList.Add(PESectionNames.PE_RWOA_HOLD.ToString(), access);
            scAccessList.Add(PESectionNames.PE_COMPLETE.ToString(), access);
            scAccessList.Add(PESectionNames.PE_BOARD_SG_REV.ToString(), access);
            scAccessList.Add(PESectionNames.PE_HQT_FINAL_REV.ToString(), access);
            scAccessList.Add(PESectionNames.PE_SENIOR_MED_REV.ToString(), access);

            //Modify access with user role
            switch (role)
            {
                case (int)UserGroups.AFRCHQTechnician:
                    if (CurrentStatusCode == (int)SpecCasePEPPStatusCode.InitiateCase)
                    {
                        scAccessList[PESectionNames.PE_HQT_INIT.ToString()] = PageAccessType.ReadWrite;
                        scAccessList[PESectionNames.PE_RLB.ToString()] = PageAccessType.ReadOnly;
                    }
                    if (CurrentStatusCode == (int)SpecCasePEPPStatusCode.RWOAHold || CurrentStatusCode == (int)SpecCasePEPPStatusCode.ExtAgencyDisposition)
                    {
                        scAccessList[PESectionNames.PE_RWOA_HOLD.ToString()] = PageAccessType.ReadWrite;
                    }

                    if (CurrentStatusCode == (int)SpecCasePEPPStatusCode.RWOAHold)
                    {
                        scAccessList[PESectionNames.PE_RLB.ToString()] = PageAccessType.ReadWrite;
                    }

                    if (CurrentStatusCode == (int)SpecCasePEPPStatusCode.FinalReview)
                    {
                        scAccessList[PESectionNames.PE_RLB.ToString()] = PageAccessType.ReadOnly;
                    }

                    break;

                case (int)UserGroups.BoardMedical:
                    if (CurrentStatusCode == (int)SpecCasePEPPStatusCode.MedicalReview)
                    {
                        scAccessList[PESectionNames.PE_BOARD_SG_REV.ToString()] = PageAccessType.ReadWrite;
                    }
                    break;

                case (int)UserGroups.SeniorMedicalReviewer:
                    if (CurrentStatusCode == (int)SpecCasePEPPStatusCode.SeniorMedicalReview)
                    {
                        scAccessList[PESectionNames.PE_SENIOR_MED_REV.ToString()] = PageAccessType.ReadWrite;
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

                    case "usergroupcontainsdecision":
                        o.OptionVisible = DetermineVisibilityForUserGroupContainsDecisionRule(new UserGroupContainsDecisionRuleData(r));
                        break;
                }
            }

            if (r.RuleTypes.ruleTypeId == (int)RuleKind.Validation)
            {
                switch (r.RuleTypes.Name.ToLower())
                {
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
                                IList<Memorandum2> memolist = (from m in memoDao.GetByRefnModule(Id, (int)ModuleType.SpecCasePEPP)
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

                    case "canforwardcase":
                        bool canforward = ValidateCanForwardCaseRule(lookupDao);

                        if (canforward != true)
                            o.OptionValid = false;
                        else
                            o.OptionValid = true;

                        break;
                }
            }
        }

        protected bool ValidateCanForwardCaseRule(ILookupDao lookupDao)
        {
            if (CurrentStatusCode == (int)SpecCasePEPPStatusCode.InitiateCase ||
                CurrentStatusCode == (int)SpecCasePEPPStatusCode.FinalReview ||
                CurrentStatusCode == (int)SpecCasePEPPStatusCode.RWOAHold ||
                CurrentStatusCode == (int)SpecCasePEPPStatusCode.ExtAgencyDisposition)
            {
                return ValidateCanForwardCaseRuleForHQAFRCTechnician(lookupDao);
            }

            if (CurrentStatusCode == (int)SpecCasePEPPStatusCode.MedicalReview)
            {
                return ValidateCanForwardCaseRuleForBoardMedical(lookupDao);
            }

            if (CurrentStatusCode == (int)SpecCasePEPPStatusCode.SeniorMedicalReview)
            {
                return ValidateCanForwardCaseRuleForSeniorMedicalReviewer(lookupDao);
            }

            return true;
        }

        protected bool ValidateCanForwardCaseRuleForBoardMedical(ILookupDao lookupDao)
        {
            bool canForward = true;

            if (!med_off_approved.HasValue)
            {
                canForward = false;
                AddValidationItem(new ValidationItem("Board Medical", "Determination", "Must provide a Determination for the PEPP case."));
            }
            else
            {
                if (med_off_approved.Value == DISQUALIFY_FINDING_VALUE)
                {
                    if (string.IsNullOrEmpty(DQParagraph))
                    {
                        canForward = false;
                        AddValidationItem(new ValidationItem("Board Medical", "DQ Paragraph", "Must enter a DQ Paragraph if you choose Disqualify for the PEPP Case."));
                    }
                }

                if (med_off_approved.Value == QUALIFY_FINDING_VALUE)
                {
                    if (!ExpirationDate.HasValue)
                    {
                        canForward = false;
                        AddValidationItem(new ValidationItem("Board Medical", "Expire Date", "Must enter the Expiration/Renewal Date for the PEPP case."));
                    }
                }
            }

            if (string.IsNullOrEmpty(med_off_approval_comment))
            {
                canForward = false;
                AddValidationItem(new ValidationItem("Board Medical", "Decision Explanation", "Must provide a Decision Explanation for the PEPP case."));
            }

            if (!CertificationDate.HasValue)
            {
                canForward = false;
                AddValidationItem(new ValidationItem("Board Medical", "Certification Date", "Must provide a Certification Date for the PEPP case."));
            }

            return canForward;
        }

        protected bool ValidateCanForwardCaseRuleForHQAFRCTechnician(ILookupDao lookupDao)
        {
            bool canForward = true;

            string user = string.Empty;
            user = "HQ AFRC Tech";

            IList<LookUpItem> types = lookupDao.GetSpecialCasePEPPTypes(this.Id);

            if (types == null || types.Count == 0)
            {
                canForward = false;
                AddValidationItem(new ValidationItem(user, "Type", "Must select at least ONE Type for the PEPP case."));
            }
            else
            {
                if (types.Where(x => x.Name.ToLower().Equals("other")).ToList().Count != 0 && string.IsNullOrEmpty(TypeName))
                {
                    canForward = false;
                    AddValidationItem(new ValidationItem(user, "Type Name", "Must enter the Type Name for the PEPP case."));
                }
            }

            if (!Rating.HasValue || Rating.Value == 0)
            {
                canForward = false;
                AddValidationItem(new ValidationItem(user, "Rating", "Must enter the Rating for the PEPP case."));
            }
            else
            {
                DataSet ds = lookupDao.GetPEPPRatings((int)Rating, 0);
                string ratingName = ds.Tables[0].Rows[0]["Name"].ToString();

                // If the "Other" rating is selected, then make sure the user actually entered a rating name in the provided textbox.
                if (ratingName.ToLower().Equals("other") == true && string.IsNullOrEmpty(RatingName))
                {
                    canForward = false;
                    AddValidationItem(new ValidationItem(user, "Rating Name", "Must enter the Rating Name for the PEPP case."));
                }
            }

            if (!Disposition.HasValue || Disposition.Value == 0)
            {
                canForward = false;
                AddValidationItem(new ValidationItem(user, "Disposition", "Must enter the Disposition for the PEPP case."));
            }

            if (!Renewal.HasValue)
            {
                canForward = false;
                AddValidationItem(new ValidationItem(user, "Renewal", "Must enter the Renewal for the PEPP case."));
            }

            if (!BaseAssign.HasValue || BaseAssign.Value == 0)
            {
                canForward = false;
                AddValidationItem(new ValidationItem(user, "Base Assign", "Must enter the Base Assign for the PEPP case."));
            }

            if (!DateReceived.HasValue)
            {
                canForward = false;
                AddValidationItem(new ValidationItem(user, "Date Received", "Must enter the Date Received for the PEPP case."));
            }

            if (!IsWaiverRequired.HasValue)
            {
                canForward = false;
                AddValidationItem(new ValidationItem(user, "Waiver Required", "Must state whether or not a waiver is required for the PEPP case."));
            }

            if (IsWaiverRequired.HasValue && IsWaiverRequired.Value == true)
            {
                if (!WaiverExpirationDate.HasValue)
                {
                    canForward = false;
                    AddValidationItem(new ValidationItem(user, "Expire Date", "Must enter the Waiver Expiration Date for the PEPP case."));
                }

                if (!ICD9Code.HasValue || ICD9Code.Value == 0)
                {
                    canForward = false;
                    AddValidationItem(new ValidationItem(user, "ICD9", "Must select a Diagnosis (ICD) for the PEPP case."));
                }

                if (string.IsNullOrEmpty(ICD9Diagnosis))
                {
                    canForward = false;
                    AddValidationItem(new ValidationItem(user, "ICD9 Description", "Must enter a Diagnosis Description for the PEPP case."));
                }
            }

            return canForward;
        }

        protected bool ValidateCanForwardCaseRuleForSeniorMedicalReviewer(ILookupDao lookupDao)
        {
            bool canForward = true;

            if (string.IsNullOrEmpty(SeniorMedicalReviewerConcur))
            {
                canForward = false;
                AddValidationItem(new ValidationItem("Senior Medical Reviewer", "Decision", "Must provide a Concur/Non-Concur Decision for the PEPP case."));
            }
            else
            {
                if (SeniorMedicalReviewerConcur.Equals(DECISION_NONCONCUR))
                {
                    if (!SeniorMedicalReviewerApproved.HasValue)
                    {
                        canForward = false;
                        AddValidationItem(new ValidationItem("Senior Medical Reviewer", "Determination", "Must provide a Decision for the PEPP case."));
                    }
                    else
                    {
                        if (SeniorMedicalReviewerApproved.Value == DISQUALIFY_FINDING_VALUE)
                        {
                            if (string.IsNullOrEmpty(AlternateDQParagraph))
                            {
                                canForward = false;
                                AddValidationItem(new ValidationItem("Senior Medical Reviewer", "DQ Paragraph", "Must enter a DQ Paragraph if you choose Disqualify for the PEPP Case."));
                            }
                        }

                        if (SeniorMedicalReviewerApproved.Value == QUALIFY_FINDING_VALUE)
                        {
                            if (!AlternateExpirationDate.HasValue)
                            {
                                canForward = false;
                                AddValidationItem(new ValidationItem("Senior Medical Reviewer", "Expire Date", "Must enter the Expiration/Renewal Date for the PEPP case."));
                            }
                        }
                    }

                    if (!AlternateCertificationDate.HasValue)
                    {
                        canForward = false;
                        AddValidationItem(new ValidationItem("Senior Medical Reviewer", "Certification Date", "Must provide a Certification Date for the PEPP case."));
                    }
                }
            }

            if (string.IsNullOrEmpty(SeniorMedicalReviewerComment))
            {
                AddValidationItem(new ValidationItem("Senior Medical Reviewer", "Decision Explanation", "Must provide a Decision Explanation for the PEPP case."));
                canForward = false;
            }

            return canForward;
        }
    }
}