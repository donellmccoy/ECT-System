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
using System.Linq;

namespace ALOD.Core.Domain.Modules.SpecialCases
{
    [Serializable]
    public class SC_MEB : SpecialCase
    {
        public const int DISQUALIFY_FINDING_VALUE = 0;
        public const int QUALIFY_FINDING_VALUE = 1;
        private int AdminLODUploaded = 0;
        private int DispositionUploaded = 0;

        public SC_MEB()
        {
        }

        public virtual int? ALCLetterType { get; set; }
        public virtual int? AlternateALCLetterType { get; set; }
        public virtual int? AlternateMedOffConcur { get; set; }
        public virtual int? AlternateMemoTemplateId { get; set; }
        public virtual int? ApprovingAuthorityType { get; set; }
        public virtual int? AssignmentLimitation { get; set; }
        public virtual DateTime? Code37InitDate { get; set; }
        public virtual int? DAWGRecommendation { get; set; }

        /// <inheritdoc/>
        public override int DocumentViewId
        {
            get { return (int)DocumentViewType.MEB; }
        }

        public virtual DateTime? EffectiveDate { get; set; }
        public virtual DateTime? ForwardDate { get; set; }
        public virtual string ICD7thCharacter { get; set; }
        public virtual int? ICD9Code { get; set; }
        public virtual string ICD9Description { get; set; }
        public virtual string ICD9Diagnosis { get; set; }
        public virtual IList<LineOfDutyFindings> LodFindings { get; set; }
        public virtual int? MedGroupName { get; set; }
        public virtual int? MedOffConcur { get; set; }
        public virtual DateTime? MemberNotifiedDate { get; set; }

        public virtual int? MemberStatus { get; set; }
        public virtual int? MemoTemplateID { get; set; }
        public virtual DateTime? ReturnToDutyDate { get; set; }
        public virtual int? RMUName { get; set; }
        // All are currently inherited from base

        /// <inheritdoc/>
        public override WorkStatus WorkflowStatus { get; set; }

        public virtual int? GetActiveMemoTemplateId()
        {
            if (ShouldUseSeniorMedicalReviewerFindings())
                return AlternateMemoTemplateId;

            return MemoTemplateID;
        }

        /// <inheritdoc/>
        public override Dictionary<string, PageAccessType> ReadSectionList(int role)
        {
            PageAccessType access;
            access = PageAccessType.None;

            Dictionary<string, PageAccessType> scAccessList = new Dictionary<string, PageAccessType>();
            if ((CurrentStatusCode == (int)SpecCaseMEBStatusCode.RTD) || (CurrentStatusCode == (int)SpecCaseMEBStatusCode.Disqualified))
            {
                access = PageAccessType.ReadOnly;
            }

            //'Add all pages as readonly
            scAccessList.Add(MBSectionNames.MB_RLB.ToString(), access);
            scAccessList.Add(MBSectionNames.MB_HQT_FINAL_REV.ToString(), access);
            scAccessList.Add(MBSectionNames.MB_RTD.ToString(), access);
            scAccessList.Add(MBSectionNames.MB_DISQUALIFIED.ToString(), access);
            scAccessList.Add(MBSectionNames.MB_SENIOR_MED_REV.ToString(), access);

            //Modify access with user role
            switch (role)
            {
                case (int)UserGroups.AFRCHQTechnician:
                    if (CurrentStatusCode == (int)SpecCaseMEBStatusCode.InitiateCase)
                    {
                        scAccessList[MBSectionNames.MB_HQT_FINAL_REV.ToString()] = PageAccessType.ReadWrite;
                    }
                    else
                    {
                        scAccessList[MBSectionNames.MB_RTD.ToString()] = PageAccessType.ReadWrite;
                        scAccessList[MBSectionNames.MB_DISQUALIFIED.ToString()] = PageAccessType.ReadWrite;
                    }

                    if (CurrentStatusCode == (int)SpecCaseMEBStatusCode.RWOAAction)
                    {
                        scAccessList[MBSectionNames.MB_RLB.ToString()] = PageAccessType.ReadWrite;
                    }
                    break;

                case (int)UserGroups.SeniorMedicalReviewer:
                    if (CurrentStatusCode == (int)SpecCaseMEBStatusCode.SeniorMedicalReview || CurrentStatusCode == (int)SpecCaseMEBStatusCode.SeniorMedicalReviewAFPC
                        || CurrentStatusCode == (int)SpecCaseMEBStatusCode.SeniorMedicalReviewIPEB || CurrentStatusCode == (int)SpecCaseMEBStatusCode.SeniorMedicalReviewFPEB
                        || CurrentStatusCode == (int)SpecCaseMEBStatusCode.SeniorMedicalReviewTransitionPhase)
                    {
                        scAccessList[MBSectionNames.MB_SENIOR_MED_REV.ToString()] = PageAccessType.ReadWrite;
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

            if (Status == (int)SpecCaseMEBWorkStatus.MedTechInput)
            {
                section = "Medical Technician Notification";

                if (MemberNotifiedDate == null)
                {
                    AddValidationItem(new ValidationItem(section, "MEBNotificationDateTextBox", "Member Notification Date is required."));
                }
            }
        }

        /// <inheritdoc/>
        protected override void ApplyRulesToOption(WorkflowStatusOption o, WorkflowOptionRule r, int lastStatus, IDaoFactory daoFactory)
        {
            IMemoDao2 memoDao = daoFactory.GetMemoDao2();
            bool canForward;

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
                            o.OptionVisible = false;
                        break;

                    case "laststatuswasnot":
                        //If the last status was either of these status codes then the option should not be visible
                        //Example-if coming from app_auth or review board fwd to Board Med should not be visible (med tech section)
                        statuses = r.RuleValue.ToString().Split(',');
                        if (statuses.Contains(lastStatus.ToString()))
                            o.OptionVisible = false;
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
                                IList<Memorandum2> memolist = (from m in memoDao.GetByRefnModule(Id, (int)ModuleType.SpecCaseMEB) where m.Deleted == false && m.Template.Id == Convert.ToByte(memos[i]) select m).ToList<Memorandum2>();
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

                    case "notificationdate":
                        if (MemberNotifiedDate == null)
                        {
                            o.OptionValid = false;
                        }

                        break;

                    case "canforwardcase":

                        o.OptionValid = ValidateCanForwardCaseRule();

                        break;

                    case "validatequalifymemos":

                        canForward = true;

                        if (Status == (int)SpecCaseMEBWorkStatus.FinalReview)
                        {
                            IList<Memorandum2> memolist2 = (from m in memoDao.GetByRefnModule(Id, (int)ModuleType.SpecCaseMEB)
                                                            where m.Deleted == false && m.Template.Id == Convert.ToByte(GetActiveMemoTemplateId())
                                                            select m).ToList<Memorandum2>();

                            if (memolist2.Count < 1 && GetActiveMemoTemplateId().HasValue)
                            {
                                canForward = false;
                                string memoName2 = (from m in memoDao.GetAllTemplates() where m.Id == Convert.ToByte(GetActiveMemoTemplateId()) select m.Title).Single();
                                AddValidationItem(new ValidationItem("Memos", "Memo", memoName2 + "  Memo  not found.", true));
                            }
                        }

                        // AFPC Disposition: sent back to Board Medical
                        if (Status == (int)SpecCaseMEBWorkStatus.MedicalReviewAFPC || Status == (int)SpecCaseMEBWorkStatus.MedicalReviewIPEB || Status == (int)SpecCaseMEBWorkStatus.MedicalReviewFPEB || Status == (int)SpecCaseMEBWorkStatus.MedicalReviewTransitionPhase)
                        {
                            if (GetActiveMemoTemplateId() != (int)MemoType.MEB_4_Non_ALC_C_SAF &&
                                GetActiveMemoTemplateId() != (int)MemoType.MEB_4_Non_ALC_C_SG &&
                                GetActiveMemoTemplateId() != (int)MemoType.MEB_RTD_AGR_SAF &&
                                GetActiveMemoTemplateId() != (int)MemoType.MEB_RTD_AGR_SG &&
                                GetActiveMemoTemplateId() != (int)MemoType.MEB_RTD_HIV_AGR_SAF &&
                                GetActiveMemoTemplateId() != (int)MemoType.MEB_RTD_HIV_AGR_SG &&
                                GetActiveMemoTemplateId() != (int)MemoType.MEB_RTD_HIV_SAF &&
                                GetActiveMemoTemplateId() != (int)MemoType.MEB_RTD_HIV_SG &&
                                GetActiveMemoTemplateId() != (int)MemoType.MEB_RTD_SAF &&
                                GetActiveMemoTemplateId() != (int)MemoType.MEB_RTD_SG &&
                                GetActiveMemoTemplateId() != (int)MemoType.MEB_4_Non_ALC_C_Exp_SAF &&
                                GetActiveMemoTemplateId() != (int)MemoType.MEB_4_Non_ALC_C_Exp_SG)

                            {
                                canForward = false;
                                AddValidationItem(new ValidationItem("Memos", "SAF RTD Memo", "Please select a Return to Duty memo.", true));
                            }
                        }

                        // AFPC Disposition: HQ Tech Final Review
                        if (Status == (int)SpecCaseMEBWorkStatus.FinalReviewAFPC || Status == (int)SpecCaseMEBWorkStatus.FinalReviewIPEB || Status == (int)SpecCaseMEBWorkStatus.FinalReviewFPEB || Status == (int)SpecCaseMEBWorkStatus.FinalReviewTransitionPhase)
                        {
                            IList<Memorandum2> memolist = (from m in memoDao.GetByRefnModule(Id, (int)ModuleType.SpecCaseMEB)
                                                           where m.Deleted == false && (m.Template.Id == (int)MemoType.MEB_4_Non_ALC_C_SAF ||
                                                                                       m.Template.Id == (int)MemoType.MEB_4_Non_ALC_C_SG ||
                                                                                       m.Template.Id == (int)MemoType.MEB_RTD_AGR_SAF ||
                                                                                       m.Template.Id == (int)MemoType.MEB_RTD_AGR_SG ||
                                                                                       m.Template.Id == (int)MemoType.MEB_RTD_HIV_AGR_SAF ||
                                                                                       m.Template.Id == (int)MemoType.MEB_RTD_HIV_AGR_SG ||
                                                                                       m.Template.Id == (int)MemoType.MEB_RTD_HIV_SAF ||
                                                                                       m.Template.Id == (int)MemoType.MEB_RTD_HIV_SG ||
                                                                                       m.Template.Id == (int)MemoType.MEB_RTD_SAF ||
                                                                                       m.Template.Id == (int)MemoType.MEB_RTD_SG ||
                                                                                       m.Template.Id == (int)MemoType.MEB_4_Non_ALC_C_Exp_SAF ||
                                                                                       m.Template.Id == (int)MemoType.MEB_4_Non_ALC_C_Exp_SG)
                                                           select m).ToList<Memorandum2>();

                            if (memolist.Count < 1 && GetActiveMemoTemplateId().HasValue)
                            {
                                canForward = false;
                                string memoRTDName = (from m in memoDao.GetAllTemplates() where m.Id == Convert.ToByte(GetActiveMemoTemplateId()) select m.Title).Single();
                                AddValidationItem(new ValidationItem("Memos", "RTD Memo", memoRTDName + "  Memo  not found.", true));
                            }
                        }

                        if (canForward != true)
                        {
                            o.OptionValid = false;
                        }
                        else
                        {
                            o.OptionValid = true;
                        }
                        break;
                }
            }
        }

        /// <inheritdoc/>
        protected override void PerformAdditionalDocumentProcessing(string docName, bool docFound, IDaoFactory daoFactory)
        {
            if (docFound)
            {
                if (docName == DocumentType.AdminLOD.ToString())
                    AdminLODUploaded = 1;

                if (docName == DocumentType.Disposition.ToString())
                    DispositionUploaded = 1;
            }
        }

        /// <inheritdoc/>
        protected override void UpdateDocumentCategoryValidation(string docName, bool isRequired, bool isValid, List<DocumentCategory2> viewCats, IDaoFactory daoFactory)
        {
            if (AllDocuments.ContainsKey(docName))
            {
                bool newIsRequired = isRequired;

                if (!isValid)
                {
                    int LODassociated = daoFactory.GetAssociatedCaseDao().GetAssociatedCasesLOD(Id, Workflow).Count;

                    if ((AdminLODUploaded == 0) && (LODassociated == 0) && (docName == "AdminLOD"))
                    {
                        AddDocumentCategoryValidationItem(docName, viewCats, "  Upload or uncheck.\nYour selection of " + Convert.ToChar(34) + "Admin LOD" + Convert.ToChar(34) + " requires this document.");
                    }
                    else if ((AdminLODUploaded == 0) && (LODassociated > 0) && (docName == "AdminLOD"))
                    {
                        //This is used to trap selections that of associate LOD and no adminlod which is valid.
                        newIsRequired = false;
                        Required[docName] = true;
                    }
                    else if ((DispositionUploaded == 1) && (med_off_approved == 0) && (docName == "Disposition"))
                    {
                        //This is used to trap selections that of associate LOD and no adminlod which is valid.
                        newIsRequired = true;
                        Required[docName] = true;
                    }
                    else if ((med_off_approved == 1) && (docName == "Disposition"))
                    {
                        //This is used to trap selections that of associate LOD and no adminlod which is valid.
                        newIsRequired = false;
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
            if (WorkflowStatus.Id == (int)SpecCaseMEBWorkStatus.InitiateCase)
                return ValidateCanForwardCaseRuleForInitiateCase();
            if (WorkflowStatus.Id == (int)SpecCaseMEBWorkStatus.MedicalReview)
                return ValidateCanForwardCaseRuleForMedicalReview();
            if (WorkflowStatus.Id == (int)SpecCaseMEBWorkStatus.SeniorMedicalReview)
                return ValidateCanForwardCaseRuleForSeniorMedicalReviewer();
            if (WorkflowStatus.Id == (int)SpecCaseMEBWorkStatus.FinalReview)
                return ValidateCanForwardCaseRuleForFinalReview();
            if (IsCaseInAHqTechDispositionStep())
                return ValidateCanForwardCaseRuleForMedicalDisposition();

            return true;
        }

        protected bool ValidateCanForwardCaseRuleForFinalReview()
        {
            bool canForward = true;
            int? boardMedicalFinding = med_off_approved;

            if (ShouldUseSeniorMedicalReviewerFindings())
            {
                boardMedicalFinding = SeniorMedicalReviewerApproved;
            }

            if (ForwardDate == null)
            {
                canForward = false;
                AddValidationItem(new ValidationItem("Medical Evaluation Board", "Forward Date", "Must provide a Forward Date for the MEB case."));
            }

            if (ApprovingAuthorityType == null || ApprovingAuthorityType == 0)
            {
                if (boardMedicalFinding.HasValue && boardMedicalFinding == DISQUALIFY_FINDING_VALUE)
                {
                    canForward = false;
                    AddValidationItem(new ValidationItem("Medical Evaluation Board", "Approval Authority", "Must select an Approval Authority for the MEB case."));
                }
            }

            if (boardMedicalFinding.HasValue && boardMedicalFinding.Value == QUALIFY_FINDING_VALUE)  // Concur with DAWG RTD or Non-Concur with DAWG Disqualify
            {
                if (ExpirationDate == null)
                {
                    canForward = false;
                    AddValidationItem(new ValidationItem("Medical Evaluation Board", "Expiration Date", "Must provide an Expiration Date for the MEB case."));
                }

                if (ReturnToDutyDate == null)
                {
                    canForward = false;
                    AddValidationItem(new ValidationItem("Medical Evaluation Board", "Return To Duty Date", "Must provide a Return To Duty Date for the MEB case."));
                }

                if (EffectiveDate == null)
                {
                    canForward = false;
                    AddValidationItem(new ValidationItem("Medical Evaluation Board", "Effective Date", "Must provide an Effective Date for the MEB case."));
                }
            }

            return canForward;
        }

        protected bool ValidateCanForwardCaseRuleForInitiateCase()
        {
            bool canForward = true;
            if (MedGroupName == null)
            {
                canForward = false;
                AddValidationItem(new ValidationItem("Medical Evaluation Board", "Med Group Name", "Must select a Med Group Name for the MEB case."));
            }
            if (RMUName == null)
            {
                canForward = false;
                AddValidationItem(new ValidationItem("Medical Evaluation Board", "RMU Name", "Must select an RMU Name for the MEB case."));
            }
            if ((MemberStatus == null) || (MemberStatus == 0))
            {
                canForward = false;
                AddValidationItem(new ValidationItem("Medical Evaluation Board", "Member Status", "Must select a Member Status for the MEB case."));
            }
            if ((ICD9Code == null) || (ICD9Code == 0))
            {
                canForward = false;
                AddValidationItem(new ValidationItem("Medical Evaluation Board", "ICD9", "Must select a Diagnosis (ICD) for the MEB case."));
            }
            if ((DAWGRecommendation == null) || (DAWGRecommendation == 0))
            {
                canForward = false;
                AddValidationItem(new ValidationItem("Medical Evaluation Board", "DAWG Recommendation", "Must provide a DAWG Recommendation for the MEB case."));
            }
            if (Code37InitDate == null)
            {
                canForward = false;
                AddValidationItem(new ValidationItem("Medical Evaluation Board", "Code 37 Date", "Must provide Initial Date of Code 37 for the MEB case."));
            }

            return canForward;
        }

        protected bool ValidateCanForwardCaseRuleForMedicalDisposition()
        {
            bool canForward = true;
            int? boardMedicalFinding = med_off_approved;

            if (ShouldUseSeniorMedicalReviewerFindings())
            {
                boardMedicalFinding = SeniorMedicalReviewerApproved;
            }

            if (DispositionUploaded != 1 && boardMedicalFinding.HasValue && boardMedicalFinding.Value == DISQUALIFY_FINDING_VALUE)
            {
                canForward = false;
                AddValidationItem(new ValidationItem("Medical Evaluation Board", "AFPC Disposition document", "Must upload a Disposition document for the Disqualified MEB case."));
            }

            return canForward;
        }

        protected bool ValidateCanForwardCaseRuleForMedicalReview()
        {
            bool canForward = true;

            if (med_off_approved == null)
            {
                canForward = false;
                AddValidationItem(new ValidationItem("Medical Evaluation Board", "Determination", "Must provide a Determination (RTD/Disqualify) for the MEB case."));
            }
            if (med_off_approved != null && !MemoTemplateID.HasValue)
            {
                canForward = false;
                AddValidationItem(new ValidationItem("Memos", "Med Off Select Memo", "Must select a Memo/Letter corresponding to Return to Duty decision."));
            }
            if (med_off_approved == 0)
            {
                if ((MemoTemplateID != (int)MemoType.MEB_DQ_AGR) && (MemoTemplateID != (int)MemoType.MEB_DQ))
                {
                    canForward = false;
                    AddValidationItem(new ValidationItem("Memos", "Disqul Memo", "Please select an MEB Disqualification Letter."));
                }
            }
            if (MedOffConcur == null)
            {
                canForward = false;
                AddValidationItem(new ValidationItem("Medical Evaluation Board", "Decision", "Must provide a Decision (Concur/Non-Concur) for the MEB case."));
            }
            if (string.IsNullOrEmpty(med_off_approval_comment))
            {
                canForward = false;
                AddValidationItem(new ValidationItem("Medical Evaluation Board", "Decision Explanation", "Must provide a Decision Explanation for the MEB case."));
            }
            if ((ALCLetterType == null) || (ALCLetterType == 0))
            {
                canForward = false;
                AddValidationItem(new ValidationItem("Medical Evaluation Board", "Assignment Limitation", "Must select an Assignment Limitation for the MEB case."));
            }

            return canForward;
        }

        protected bool ValidateCanForwardCaseRuleForSeniorMedicalReviewer()
        {
            bool canForward = true;

            if (string.IsNullOrEmpty(SeniorMedicalReviewerConcur))
            {
                canForward = false;
                AddValidationItem(new ValidationItem("Senior Medical Reviewer", "Decision", "Must provide a Concur/Non-Concur Decision for the MEB case."));
            }
            else
            {
                if (SeniorMedicalReviewerConcur.Equals(DECISION_NONCONCUR))
                {
                    if (!SeniorMedicalReviewerApproved.HasValue)
                    {
                        canForward = false;
                        AddValidationItem(new ValidationItem("Senior Medical Reviewer", "Determination", "Must provide a Decision for the MEB case."));
                    }
                    else
                    {
                        if (!AlternateMemoTemplateId.HasValue)
                        {
                            canForward = false;
                            AddValidationItem(new ValidationItem("Memos", "Med Off Select Memo", "Must select a Memo/Letter for the MEB case."));
                        }
                        else
                        {
                            if (SeniorMedicalReviewerApproved.Value == QUALIFY_FINDING_VALUE && IsMEBDisqualificationMemoId(AlternateMemoTemplateId))
                            {
                                canForward = false;
                                AddValidationItem(new ValidationItem("Memos", "Med Off Select Memo", "Must select a Memo/Letter corresponding to Return to Duty decision."));
                            }

                            if (SeniorMedicalReviewerApproved.Value == DISQUALIFY_FINDING_VALUE && !IsMEBDisqualificationMemoId(AlternateMemoTemplateId))
                            {
                                canForward = false;
                                AddValidationItem(new ValidationItem("Memos", "Disqul Memo", "Please select an MEB Disqualification Letter."));
                            }
                        }
                    }

                    if (string.IsNullOrEmpty(SeniorMedicalReviewerComment))
                    {
                        AddValidationItem(new ValidationItem("Senior Medical Reviewer", "Decision Explanation", "Must provide a Decision Explanation for the MEB case."));
                        canForward = false;
                    }

                    if (!AlternateMedOffConcur.HasValue)
                    {
                        canForward = false;
                        AddValidationItem(new ValidationItem("Senior Medical Reviewer", "Decision", "Must provide a DAWG Decision (Concur/Non-Concur) for the MEB case."));
                    }

                    if (!AlternateALCLetterType.HasValue || AlternateALCLetterType.Value == 0)
                    {
                        canForward = false;
                        AddValidationItem(new ValidationItem("Senior Medical Reviewer", "Assignment Limitation", "Must select an Assignment Limitation for the MEB case."));
                    }
                }
            }

            return canForward;
        }

        private bool IsCaseInAHqTechDispositionStep()
        {
            if (Status == (int)SpecCaseMEBWorkStatus.AwaitingAFPC || Status == (int)SpecCaseMEBWorkStatus.IPEBDisposition || Status == (int)SpecCaseMEBWorkStatus.FPEBDisposition || Status == (int)SpecCaseMEBWorkStatus.TransitionPhase)
                return true;

            return false;
        }

        private bool IsMEBDisqualificationMemoId(int? memoId)
        {
            if (!memoId.HasValue)
                return false;

            if (memoId.Value == (int)MemoType.MEB_DQ_AGR || memoId.Value == (int)MemoType.MEB_DQ)
                return true;

            return false;
        }
    }
}