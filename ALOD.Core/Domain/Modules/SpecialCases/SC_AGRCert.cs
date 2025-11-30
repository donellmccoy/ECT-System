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
    public class SC_AGRCert : SpecialCase
    {
        public const int APPROVED_FINDING_VALUE = 1;
        public const int DENIED_FINDING_VALUE = 0;
        public const int IS_ALC = 1;
        public const int IS_MAJCOM = 1;
        public const int MEMO_APPROVE_HQ_FON = 197;
        public const int MEMO_APPROVE_HQ_INITIAL = 199;
        public const int MEMO_APPROVE_RMU_FON = 196;
        public const int MEMO_APPROVE_RMU_INITIAL = 198;
        public const int MEMO_DENIED = 200;
        public const int RFA_FINDING_VALUE = 3;

        public SC_AGRCert()
        {
            //Testing Required Documents
            //AllDocuments.Add(DocumentType.AFForm469.ToString(), false);
            //AllDocuments.Add(DocumentType.NarrativeSummary.ToString(), false);
            AllDocuments.Add(DocumentType.ALcCoverLetter.ToString(), false);
        }

        public virtual string AFSC { get; set; }
        public virtual int? ALC { get; set; }

        public virtual int? AlternateALCLetterType { get; set; }

        public virtual int? AlternateAlternateALCLetterType { get; set; }

        public virtual DateTime? AlternateApprovalDate { get; set; }

        public virtual DateTime? AlternateExpirationDate { get; set; }

        public virtual int? AlternateMedOffConcur { get; set; }

        public virtual int? AlternateMemoTemplateID { get; set; }

        public virtual DateTime? ApprovalDate { get; set; }

        public virtual int? Disposition { get; set; }

        /// <inheritdoc/>
        public override int DocumentViewId
        {
            get { return (int)DocumentViewType.AGRCert; }
        }

        public virtual int? FollowOnTour { get; set; }

        public virtual int? InitialTour { get; set; }

        //checkbox
        public virtual int? MAJCOM { get; set; }

        //checkbox
        //checkbox
        //checkbox
        public virtual int? MemoTemplateID { get; set; }

        public virtual string MTFInitialFacility { get; set; }

        public virtual string MTFInitialFacilityCityStateZip { get; set; }

        public virtual DateTime? PHADate { get; set; }

        public virtual string PocEmail { get; set; }

        public virtual string PocPhoneDSN { get; set; }

        public virtual string POCRankAndName { get; set; }

        public virtual string PocUnit { get; set; }

        //public virtual int? MedGroupName { get; set; } //dropdown
        public virtual int? RMUName { get; set; } //dropdown

        #region test

        // For testing purposes only !!!

        //public override DateTime? ExpirationDate { get; set; }

        #endregion test

        /// <inheritdoc/>
        public override WorkStatus WorkflowStatus { get; set; }

        public virtual bool AreDispositionsDifferent(ILookupDispositionDao dispositionDao)
        {
            if (!Disposition.HasValue)
                return false;

            if (!GetActiveBoardMedicalFinding().HasValue)
                return false;

            if (NormalizeHQTechDisposition(dispositionDao) == GetActiveBoardMedicalFinding().Value)
                return false;

            return true;
        }

        public virtual int GetActiveALC()
        {
            if (GetActiveBoardMedicalALC().HasValue)
                return GetActiveBoardMedicalALC().Value;

            if (ALC.HasValue)
                return ALC.Value;

            return 0; // No Limitations
        }

        public virtual int? GetActiveBoardMedicalALC()
        {
            if (ShouldUseSeniorMedicalReviewerFindings())
                return AlternateAlternateALCLetterType;

            return AlternateALCLetterType;
        }

        public virtual int GetActiveDispositionValue(ILookupDispositionDao dispositionDao)
        {
            if (GetActiveBoardMedicalFinding().HasValue)
                return GetActiveBoardMedicalFinding().Value;

            if (Disposition.HasValue)
                return NormalizeHQTechDisposition(dispositionDao);

            return -1;
        }

        public virtual int? GetActiveMemoTemplateId()
        {
            if (ShouldUseSeniorMedicalReviewerFindings())
                return AlternateMemoTemplateID ?? MemoTemplateID;

            if (ShouldUseBoardMedicalReviewerindings())
                return AlternateMemoTemplateID ?? MemoTemplateID;

            return MemoTemplateID;
        }

        public virtual int NormalizeHQTechDisposition(ILookupDispositionDao dispositionDao)
        {
            if (!Disposition.HasValue)
                return -1;

            ALOD.Core.Domain.Lookup.Disposition d = dispositionDao.GetById(Disposition.Value);

            return NormalizeHQTechDisposition(d);
        }

        public virtual int NormalizeHQTechDisposition(Disposition disposition)
        {
            if (disposition == null)
                return -1;

            if (disposition.Name.Equals("Qualified"))
                return 1;

            if (disposition.Name.Equals("Disqualified"))
                return 0;

            return -1;
        }

        /// <inheritdoc/>
        public override Dictionary<String, PageAccessType> ReadSectionList(int role)
        {
            PageAccessType access;
            access = PageAccessType.None;

            Dictionary<String, PageAccessType> scAccessList = new Dictionary<String, PageAccessType>();
            if ((CurrentStatusCode == (int)SpecCaseAGRStatusCode.Approved))
            {
                access = PageAccessType.ReadOnly;
            }

            //'Add all pages as readonly

            scAccessList.Add(AGRSectionNames.AGR_MED_TECH_INIT.ToString(), access);
            scAccessList.Add(AGRSectionNames.AGR_HQT_REV.ToString(), access);
            scAccessList.Add(AGRSectionNames.AGR_BOARD_SG_REV.ToString(), access);
            scAccessList.Add(AGRSectionNames.AGR_SENIOR_MED_REV.ToString(), access);
            scAccessList.Add(AGRSectionNames.AGR_HQT_FINAL_REV.ToString(), access);
            scAccessList.Add(AGRSectionNames.AGR_APPROVED.ToString(), access);
            scAccessList.Add(AGRSectionNames.AGR_DENIED.ToString(), access);
            scAccessList.Add(AGRSectionNames.AGR_RLB.ToString(), access);

            //Modify access with user role
            switch (role)
            {
                case (int)UserGroups.MedicalTechnician:
                    if (CurrentStatusCode == (int)SpecCaseAGRStatusCode.InitiateCase)
                    {
                        scAccessList[AGRSectionNames.AGR_MED_TECH_INIT.ToString()] = PageAccessType.ReadWrite;
                        scAccessList[AGRSectionNames.AGR_RLB.ToString()] = PageAccessType.ReadWrite;
                    }
                    if (CurrentStatusCode == (int)SpecCaseAGRStatusCode.FinalReview)
                    {
                        scAccessList[AGRSectionNames.AGR_MED_TECH_INIT.ToString()] = PageAccessType.ReadWrite;
                        scAccessList[AGRSectionNames.AGR_RLB.ToString()] = PageAccessType.ReadWrite;
                    }
                    break;

                case (int)UserGroups.AFRCHQTechnician:
                    if (CurrentStatusCode == (int)SpecCaseAGRStatusCode.PackageReview)
                    {
                        scAccessList[AGRSectionNames.AGR_HQT_REV.ToString()] = PageAccessType.ReadWrite;
                        scAccessList[AGRSectionNames.AGR_RLB.ToString()] = PageAccessType.ReadOnly;
                    }
                    if (CurrentStatusCode == (int)SpecCaseAGRStatusCode.FinalReview)
                    {
                        scAccessList[AGRSectionNames.AGR_HQT_FINAL_REV.ToString()] = PageAccessType.ReadWrite;
                        scAccessList[AGRSectionNames.AGR_RLB.ToString()] = PageAccessType.ReadWrite;
                    }
                    break;

                case (int)UserGroups.BoardMedical:
                    if (CurrentStatusCode == (int)SpecCaseAGRStatusCode.PackageReview)
                    {
                        scAccessList[AGRSectionNames.AGR_BOARD_SG_REV.ToString()] = PageAccessType.ReadWrite;
                    }
                    break;

                case (int)UserGroups.SeniorMedicalReviewer:
                    if (CurrentStatusCode == (int)SpecCaseAGRStatusCode.SeniorMedicalReview)
                    {
                        scAccessList[AGRSectionNames.AGR_SENIOR_MED_REV.ToString()] = PageAccessType.ReadWrite;
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

                    case "hqtechapprove":
                        o.OptionVisible = DetermineVisibilityForHQTechCanCompleteCaseRule(APPROVED_FINDING_VALUE);
                        break;

                    case "hqtechdeny":
                        o.OptionVisible = DetermineVisibilityForHQTechCanCompleteCaseRule(DENIED_FINDING_VALUE);
                        break;

                    case "medtechapprove":
                        o.OptionVisible = DetermineVisibilityForMedTechCanCompleteCaseRule(APPROVED_FINDING_VALUE);
                        break;

                    case "medtechdeny":
                        o.OptionVisible = DetermineVisibilityForMedTechCanCompleteCaseRule(DENIED_FINDING_VALUE);
                        break;

                    case "cancompleteasapproved":
                        o.OptionVisible = DetermineVisibilityForCanCompleteCaseRule(APPROVED_FINDING_VALUE);
                        break;

                    case "cancompleteasdenied":
                        o.OptionVisible = DetermineVisibilityForCanCompleteCaseRule(DENIED_FINDING_VALUE);
                        break;

                    case "usergroupcontainsdecision":
                        o.OptionVisible = DetermineVisibilityForUserGroupContainsDecisionRule(new UserGroupContainsDecisionRuleData(r));
                        break;

                    case "isalcormajcom":
                        o.OptionVisible = DetermineVisibilityForAlcOrMajcomWingCanForwardCaseRule();
                        break;

                    case "isnotalcandmajcom":
                        o.OptionVisible = DetermineVisibilityForIsNotAlcOrMajcomWingCanForwardCaseRule();
                        break;

                    case "rfa":
                        o.OptionVisible = DetermineVisibiltyForRFARule(RFA_FINDING_VALUE);
                        break;

                    case "memoapprovehqinitial":
                        o.OptionVisible = DetermineVisibilityForHQApproveButton(r, memoDao);
                        break;

                    case "memoapprovehqfon":
                        o.OptionVisible = DetermineVisibilityForHQApproveButton(r, memoDao);
                        break;

                    case "memoapprovermuinitial":
                        o.OptionVisible = DetermineVisibilityForRMUApproveButton(r, memoDao);
                        break;

                    case "memoapprovermufon":
                        o.OptionVisible = DetermineVisibilityForRMUApproveButton(r, memoDao);
                        break;

                    case "memodenied":
                        o.OptionVisible = DetermineVisibilityForDeniedButton(r, memoDao);
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
                                IList<Memorandum2> memolist = (from m in memoDao.GetByRefnModule(Id, (int)ModuleType.SpecCaseAGR) where m.Deleted == false && m.Template.Id == Convert.ToByte(memos[i]) select m).ToList<Memorandum2>();
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

                        if (r.CheckAll != null && r.CheckAll.Value == true)
                        {
                            o.OptionValid = allExist;
                        }
                        else
                        {
                            o.OptionValid = oneExist;
                        }

                        break;

                    case "validatequalifymemos":
                        canforward = ValidateQualifyMemosRule(memoDao);

                        if (canforward != true)
                            o.OptionValid = false;
                        else
                            o.OptionValid = true;

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

                        if (r.CheckAll != null && r.CheckAll.Value == true)
                        {
                            o.OptionValid = allExist;
                        }
                        else
                        {
                            o.OptionValid = oneExist;
                        }

                        break;

                    case "memovalidatehqinitial":
                        canforward = DetermineVisibilityForHQApproveButton(r, memoDao);

                        if (canforward != true)
                            o.OptionValid = false;
                        else
                            o.OptionValid = true;

                        break;

                    case "memovalidatehqfon":
                        canforward = DetermineVisibilityForHQApproveButton(r, memoDao);

                        if (canforward != true)
                            o.OptionValid = false;
                        else
                            o.OptionValid = true;

                        break;

                    case "memovalidatermuinitial":
                        canforward = DetermineVisibilityForRMUApproveButton(r, memoDao);

                        if (canforward != true)
                            o.OptionValid = false;
                        else
                            o.OptionValid = true;

                        break;

                    case "memovalidatermufon":
                        canforward = DetermineVisibilityForRMUApproveButton(r, memoDao);

                        if (canforward != true)
                            o.OptionValid = false;
                        else
                            o.OptionValid = true;

                        break;

                    case "memovalidatedenied":
                        canforward = DetermineVisibilityForDeniedButton(r, memoDao);

                        if (canforward != true)
                            o.OptionValid = false;
                        else
                            o.OptionValid = true;

                        break;
                }
            }
        }

        protected bool DetermineVisibilityForAlcOrMajcomWingCanForwardCaseRule()
        {
            if (DetermineVisibilityForAlcWingCanForwardCaseRule(IS_ALC) || DetermineVisibilityForMajcomWingCanForwardCaseRule(IS_MAJCOM))
            {
                return true;
            }

            return false;
        }

        protected bool DetermineVisibilityForAlcWingCanForwardCaseRule(int expectedFinding)
        {
            if (ALC == null)
            {
                AddValidationItem(new ValidationItem("Local Medical Technician", "Decision", "Local Medical Technician must provide an ALC in order to close out the AGR case."));
                return false;
            }

            if (ALC == expectedFinding)
            {
                return true;
            }

            if (ALC != expectedFinding)
            {
                return false;
            }

            return true;
        }

        protected bool DetermineVisibilityForCanCompleteCaseRule(int expectedFinding)
        {
            if (hqt_approval1 == null)
            {
                AddValidationItem(new ValidationItem("Board Medical Officer", "Decision", "Board Medical Officer must provide a finding in order to close out the AGR case."));
                return false;
            }
            if (med_off_approved == null)
            {
                AddValidationItem(new ValidationItem("Medical Officer", "Decision", "Medical Officer must provide a finding in order to close out the AGR case."));
                return false;
            }

            if (ShouldUseSeniorMedicalReviewerFindings())
            {
                if (SeniorMedicalReviewerApproved == null || SeniorMedicalReviewerApproved != expectedFinding)
                    return false;
            }
            else
            {
                if (hqt_approval1 != expectedFinding)
                    return false;
            }

            if (ShouldUseWingMedicalReviewerindings())
            {
                if (med_off_approved == null || med_off_approved != expectedFinding)
                    return false;
            }

            return true;
        }

        protected bool DetermineVisibilityForIsNotAlcOrMajcomWingCanForwardCaseRule()
        {
            if (!DetermineVisibilityForAlcWingCanForwardCaseRule(IS_ALC) && !DetermineVisibilityForMajcomWingCanForwardCaseRule(IS_MAJCOM))
            {
                return true;
            }

            return false;
        }

        protected bool DetermineVisibilityForMajcomWingCanForwardCaseRule(int expectedFinding)
        {
            if (MAJCOM == null)
            {
                AddValidationItem(new ValidationItem("Local Medical Technician", "Decision", "Local Medical Technician must select MajCom in order to close out the AGR case."));
                return false;
            }

            if (MAJCOM == expectedFinding)
            {
                return true;
            }

            if (ALC != expectedFinding)
            {
                return false;
            }

            return true;
        }

        protected bool ValidateCanForwardCaseRule()
        {
            if (CurrentStatusCode == (int)SpecCaseAGRStatusCode.InitiateCase)
                return ValidateCanForwardCaseRuleForCommonMedTechTabFields("Medical Technician");

            if (CurrentStatusCode == (int)SpecCaseAGRStatusCode.LocalFinalReview)
                return ValidateCanForwardCaseRuleForCommonMedTechTabFields("Medical Technician");

            if (CurrentStatusCode == (int)SpecCaseAGRStatusCode.MedicalOfficerReview)
                return ValidateCanForwardCaseRuleForMedicalOfficer();

            if (CurrentStatusCode == (int)SpecCaseAGRStatusCode.HqTechInitiate)
                return ValidateCanForwardCaseRuleForCommonMedTechTabFields("HQ AFRC Technician");

            if (CurrentStatusCode == (int)SpecCaseAGRStatusCode.PackageReview)
                return ValidateCanForwardCaseRuleForCommonMedTechTabFields("HQ AFRC Technician");

            if (CurrentStatusCode == (int)SpecCaseAGRStatusCode.FinalReview)
                return ValidateCanForwardCaseRuleForCommonMedTechTabFields("HQ AFRC Technician");

            if (CurrentStatusCode == (int)SpecCaseAGRStatusCode.MedicalReview)
                return ValidateCanForwardCaseRuleForBoardMedicalOfficer();

            if (CurrentStatusCode == (int)SpecCaseAGRStatusCode.SeniorMedicalReview)
                return ValidateCanForwardCaseRuleForSeniorMedicalReviewer();

            return true;
        }

        private bool DetermineVisibilityForDeniedButton(WorkflowOptionRule r, IMemoDao2 memoDao)
        {
            string[] memos;
            memos = r.RuleValue.ToString().Split(',');
            for (int i = 0; i < memos.Length; i++)
            {
                IList<Memorandum2> memolist =
                    (from m in memoDao.GetByRefnModule(Id, (int)ModuleType.SpecCaseAGR)
                     where m.Deleted == false && m.Template.Id == Convert.ToByte(memos[i])
                     select m).ToList<Memorandum2>();

                foreach (Memorandum2 m in memolist)
                {
                    if (m.Template.Id == MEMO_DENIED)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool DetermineVisibilityForHQApproveButton(WorkflowOptionRule r, IMemoDao2 memoDao)
        {
            string[] memos;
            memos = r.RuleValue.ToString().Split(',');
            for (int i = 0; i < memos.Length; i++)
            {
                IList<Memorandum2> memolist =
                    (from m in memoDao.GetByRefnModule(Id, (int)ModuleType.SpecCaseAGR)
                     where m.Deleted == false && m.Template.Id == Convert.ToByte(memos[i])
                     select m).ToList<Memorandum2>();

                foreach (Memorandum2 m in memolist)
                {
                    if (m.Template.Id == MEMO_APPROVE_HQ_INITIAL | m.Template.Id == MEMO_APPROVE_HQ_FON)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool DetermineVisibilityForHQTechCanCompleteCaseRule(int expectedFinding)
        {
            if (hqt_approval1 == null)
                return false;

            if (hqt_approval1 != expectedFinding)
                return false;

            return true;
        }

        private bool DetermineVisibilityForMedTechCanCompleteCaseRule(int expectedFinding)
        {
            if (med_off_approved == null)
                return false;

            if (med_off_approved != expectedFinding)
                return false;

            return true;
        }

        private bool DetermineVisibilityForRMUApproveButton(WorkflowOptionRule r, IMemoDao2 memoDao)
        {
            string[] memos;
            memos = r.RuleValue.ToString().Split(',');
            for (int i = 0; i < memos.Length; i++)
            {
                IList<Memorandum2> memolist =
                    (from m in memoDao.GetByRefnModule(Id, (int)ModuleType.SpecCaseAGR)
                     where m.Deleted == false && m.Template.Id == Convert.ToByte(memos[i])
                     select m).ToList<Memorandum2>();

                foreach (Memorandum2 m in memolist)
                {
                    if (m.Template.Id == MEMO_APPROVE_RMU_INITIAL | m.Template.Id == MEMO_APPROVE_RMU_FON)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool DetermineVisibiltyForRFARule(int expectedFinding)
        {
            if (GetActiveBoardMedicalFinding() != null && GetActiveBoardMedicalFinding() == expectedFinding)
                return false;
            else
                return true;
        }

        private bool ValidateCanForwardCaseRuleForBoardMedicalOfficer()
        {
            bool canForward = true;

            if (med_off_approved == null)
            {
                AddValidationItem(new ValidationItem("Board Medical Officer", "Decision", "Must provide a Decision for the AGR case."));
                canForward = false;
            }
            else if (med_off_approved.Value == APPROVED_FINDING_VALUE)
            {
                if (AlternateApprovalDate == null)
                {
                    AddValidationItem(new ValidationItem("Board Medical Officer", "Approval Date", "Must enter an Approval Date for the AGR case."));
                    canForward = false;
                }

                //if (ExpirationDate == null)
                //{
                //    AddValidationItem(new ValidationItem("Board Medical Officer", "Expiration Date", "Must enter an Expiration Date for the AGR case."));
                //    canForward = false;
                //}
            }

            if (string.IsNullOrEmpty(hqt_approval1_comment))
            {
                AddValidationItem(new ValidationItem("Board Medical Officer", "Decision Comment", "Must enter an Decision Comment for the AGR case."));
                canForward = false;
            }

            return canForward;
        }

        private bool ValidateCanForwardCaseRuleForCommonMedTechTabFields(string userGroupNameBeingValidated)
        {
            bool canForward = true;

            if (string.IsNullOrEmpty(AFSC))
            {
                canForward = false;
                AddValidationItem(new ValidationItem(userGroupNameBeingValidated, "AFSC", "Must enter an AFSC for the AGR case."));
            }

            if (PHADate == null)
            {
                canForward = false;
                AddValidationItem(new ValidationItem(userGroupNameBeingValidated, "PHA Date", "Must enter a PHA Date for the AGR case."));
            }

            if (RMUName == null)
            {
                canForward = false;
                AddValidationItem(new ValidationItem(userGroupNameBeingValidated, "RMU Name", "Must select a Base Name for the AGR case."));
            }

            if (string.IsNullOrEmpty(PocUnit))
            {
                canForward = false;
                AddValidationItem(new ValidationItem(userGroupNameBeingValidated, "POC Unit", "Must enter POC Unit for the AGR case."));
            }

            if (string.IsNullOrEmpty(MTFInitialFacility))
            {
                canForward = false;
                AddValidationItem(new ValidationItem(userGroupNameBeingValidated, "POC Address 1", "Must enter POC Address 1 for the AGR case."));
            }

            if (string.IsNullOrEmpty(MTFInitialFacilityCityStateZip))
            {
                canForward = false;
                AddValidationItem(new ValidationItem(userGroupNameBeingValidated, "POC Address 2", "Must enter POC Address 2 for the AGR case."));
            }

            if (string.IsNullOrEmpty(POCRankAndName))
            {
                canForward = false;
                AddValidationItem(new ValidationItem(userGroupNameBeingValidated, "POC Name", "Must enter POC Name for the AGR case."));
            }

            if (string.IsNullOrEmpty(PocPhoneDSN))
            {
                canForward = false;
                AddValidationItem(new ValidationItem(userGroupNameBeingValidated, "POC Phone", "Must enter POC Phone for the AGR case."));
            }

            if (string.IsNullOrEmpty(PocEmail))
            {
                canForward = false;
                AddValidationItem(new ValidationItem(userGroupNameBeingValidated, "POC Email", "Must enter POC Email for the AGR case."));
            }

            if (ALC == null)
            {
                canForward = false;
                AddValidationItem(new ValidationItem(userGroupNameBeingValidated, "ALC", "Must select whether or not there's a ALC with AGR case."));
            }

            if (MAJCOM == null)
            {
                canForward = false;
                AddValidationItem(new ValidationItem(userGroupNameBeingValidated, "MAJCOM", "Must select whether or not there's a MAJCOM for the AGR case."));
            }

            if (InitialTour == null)
            {
                canForward = false;
                AddValidationItem(new ValidationItem(userGroupNameBeingValidated, "InitialTour", "Must select whether or not there's a InitialTour for the AGR case."));
            }

            if (FollowOnTour == null)
            {
                canForward = false;
                AddValidationItem(new ValidationItem(userGroupNameBeingValidated, "FollowOnTour", "Must select whether or not there's a FollowOnTour for the AGR case."));
            }
            return canForward;
        }

        private bool ValidateCanForwardCaseRuleForMedicalOfficer()
        {
            bool canForward = true;

            if (med_off_approved == null)
            {
                AddValidationItem(new ValidationItem("Medical Officer", "Decision", "Must provide a Decision for the AGR case."));
                canForward = false;
            }
            else if (med_off_approved.Value == APPROVED_FINDING_VALUE)
            {
                if (ApprovalDate == null)
                {
                    AddValidationItem(new ValidationItem("Board Medical Officer", "Approval Date", "Must enter an Approval Date for the AGR case."));
                    canForward = false;
                }

                //if (ExpirationDate == null)
                //{
                //    AddValidationItem(new ValidationItem("Medical Officer", "Expiration Date", "Must enter an Expiration Date for the AGR case."));
                //    canForward = false;
                //}
            }

            if (string.IsNullOrEmpty(med_off_approval_comment))
            {
                AddValidationItem(new ValidationItem("Medical Officer", "Decision Comment", "Must enter an Decision Comment for the AGR case."));
                canForward = false;
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
                    if (SeniorMedicalReviewerApproved == null)
                    {
                        canForward = false;
                        AddValidationItem(new ValidationItem("Senior Medical Reviewer", "Determination", "Must provide a Decision for the PW case."));
                    }
                    else if (SeniorMedicalReviewerApproved.Value == APPROVED_FINDING_VALUE)
                    {
                        if (AlternateApprovalDate == null)
                        {
                            canForward = false;
                            AddValidationItem(new ValidationItem("Senior Medical Reviewer", "Approval Date", "Must enter an Approval Date for the PW case."));
                        }

                        //if (!AlternateExpirationDate.HasValue)
                        //{
                        //    canForward = false;
                        //    AddValidationItem(new ValidationItem("Senior Medical Reviewer", "Expiration Date", "Must enter an Expiration Date for the PW case."));
                        //}
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

        private bool ValidateQualifyMemosRule(IMemoDao2 memoDao)
        {
            bool canForward = true;
            int? activeMemoTemplateId = GetActiveMemoTemplateId();
            int? activeBoardMedicalFinding = GetActiveBoardMedicalFinding();

            if (Status == (int)SpecCaseAGRWorkStatus.FinalReview || Status == (int)SpecCaseAGRWorkStatus.LocalFinalReview)
            {
                IList<Memorandum2> memolist = (from m in memoDao.GetByRefnModule(Id, (int)ModuleType.SpecCaseAGR)
                                               where m.Deleted == false && m.Template.Id == Convert.ToByte(activeMemoTemplateId)
                                               select m).ToList<Memorandum2>();

                if (memolist.Count < 1 && activeMemoTemplateId.HasValue)
                {
                    canForward = false;
                    string memoName = (from m in memoDao.GetAllTemplates() where m.Id == Convert.ToByte(activeMemoTemplateId) select m.Title).Single();
                    AddValidationItem(new ValidationItem("Memos", "Memo", memoName + "  Memo  not found.", true));
                }
                else
                {
                    if ((activeBoardMedicalFinding == (int)Finding.Qualify_RTD || activeBoardMedicalFinding == (int)Finding.Admin_Qualified) && !activeMemoTemplateId.HasValue)
                    {
                        canForward = false;
                        AddValidationItem(new ValidationItem("Memos", "Med Off Select Memo", "Must select a Memo/Letter corresponding to Return to Duty decision."));
                    }
                    else if (activeBoardMedicalFinding == (int)Finding.Admin_LOD && activeMemoTemplateId != (int)MemoType.IRILO_Admin_LOD_AFPC)
                    {
                        canForward = false;
                        AddValidationItem(new ValidationItem("Memos", "Admin LOD Memo", "Please select IRILO Admin LOD AFPC Letter.", true));
                    }
                }

                // check for MEB Disqul Letter from Board Medical step
                if (activeBoardMedicalFinding == (int)Finding.Disapprove)
                {
                    IList<Memorandum2> memolist2 = (from m in memoDao.GetByRefnModule(Id, (int)ModuleType.SpecCaseAGR)
                                                    where m.Deleted == false && m.Template.Id == Convert.ToByte(MemoType.AGR_Certiication_Denied)
                                                    select m).ToList<Memorandum2>();

                    if (memolist2.Count < 1)
                    {
                        canForward = false;
                        string memoName2 = (from m in memoDao.GetAllTemplates() where m.Id == Convert.ToByte(MemoType.AGR_Certiication_Denied) select m.Title).Single();
                        AddValidationItem(new ValidationItem("Memos", "AGR Memo", memoName2 + "  Memo  not found.", true));
                    }
                    else
                    {
                        canForward = true;
                    }
                }
            }

            if (Status == (int)SpecCaseAGRWorkStatus.MedicalReview)
            {
                if (activeBoardMedicalFinding == (int)Finding.Admin_LOD && activeMemoTemplateId != (int)MemoType.AGR_Approved_HQ_INIT)
                {
                    canForward = false;
                    AddValidationItem(new ValidationItem("Memos", "HQ Memo", "Please select AGR Approval Letter.", true));
                }
                if (activeBoardMedicalFinding == (int)Finding.Admin_LOD && activeMemoTemplateId != (int)MemoType.AGR_Approved_HQ_FON)
                {
                    canForward = false;
                    AddValidationItem(new ValidationItem("Memos", "HQ Memo", "Please select AGR Approval Letter.", true));
                }
            }

            return canForward;
        }
    }
}