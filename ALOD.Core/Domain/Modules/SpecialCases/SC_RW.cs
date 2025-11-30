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
    public class SC_RW : SpecialCase
    {
        public const int PROCESS_AS_FULLCASEDIRECTED = 3;
        public const int PROCESS_AS_FULLMEB = 2;
        public const int PROCESS_AS_FULLWWD = 1;
        public const string WORKFLOW_TITLE = "Retention Waiver Renewal";

        public SC_RW()
        {
        }

        public virtual int? ALCLetterType { get; set; }
        public virtual int? AlternateALCLetterType { get; set; }
        public virtual DateTime? AlternateDQCompletionDate { get; set; }
        public virtual string AlternateDQParagraph { get; set; }
        public virtual DateTime? AlternateExpirationDate { get; set; }
        public virtual int? AlternateMemoTemplateID { get; set; }
        public virtual int? AlternateProcessAs { get; set; }
        public virtual DateTime? AlternateReturnToDutyDate { get; set; }
        public virtual int? AssociatedSC { get; set; }

        public virtual decimal? BodyMassIndex { get; set; }
        public virtual string DAFSC { get; set; }
        public virtual int? DAWGRecommendation { get; set; }

        /// <inheritdoc/>
        public override int DocumentViewId
        {
            get { return (int)DocumentViewType.RW; }
        }

        public virtual DateTime? DQCompletionDate { get; set; }
        public virtual string DQParagraph { get; set; }
        public virtual bool? DxInterferenceWithDuties { get; set; }
        public virtual string ERorUrgentCareVisitList { get; set; }
        public virtual bool? HasBeenHospitalized { get; set; }
        public virtual bool? HasHadERorUrgentCareVisits { get; set; }
        public virtual string HospitalizationList { get; set; }
        public virtual string ICD7thCharacter { get; set; }
        public virtual int? ICD9Code { get; set; }
        public virtual string ICD9Description { get; set; }
        public virtual string ICD9Diagnosis { get; set; }
        public virtual int? MedGroupName { get; set; }
        public virtual string MedicationsAndDosages { get; set; }
        public virtual int? MemoTemplateID { get; set; }
        public virtual int? MissedWorkDays { get; set; }
        public virtual int? ProcessAs { get; set; }
        public virtual string Prognosis { get; set; }
        public virtual int? RecommendedFollowUpInterval { get; set; }
        public virtual DateTime? RenewalDate { get; set; }
        public virtual int? RequiresSpecialistForMgmt { get; set; }
        public virtual DateTime? ReturnToDutyDate { get; set; }
        public virtual int? RiskForSuddenIncapacitation { get; set; }
        public virtual int? RMUName { get; set; }
        public virtual string Treatment { get; set; }
        public virtual int? YearsSatisfactoryService { get; set; }

        public virtual int? GetActiveMemoTemplateId()
        {
            if (ShouldUseSeniorMedicalReviewerFindings())
                return AlternateMemoTemplateID;

            return MemoTemplateID;
        }

        /// <inheritdoc/>
        public override Dictionary<String, PageAccessType> ReadSectionList(int role)
        {
            PageAccessType access;
            access = PageAccessType.None;

            Dictionary<String, PageAccessType> scAccessList = new Dictionary<String, PageAccessType>();
            if (CurrentStatusCode == (int)SpecCaseRWStatusCode.RTD || CurrentStatusCode == (int)SpecCaseRWStatusCode.Disqualified || CurrentStatusCode == (int)SpecCaseRWStatusCode.AdminLOD)
            {
                access = PageAccessType.ReadOnly;
            }

            // Add all pages as readonly
            scAccessList.Add(RWSectionNames.RW_RLB.ToString(), access);
            scAccessList.Add(RWSectionNames.RW_MED_TECH_INIT.ToString(), access);
            scAccessList.Add(RWSectionNames.RW_HQ_TECH_INIT.ToString(), access);
            scAccessList.Add(RWSectionNames.RW_DISQUALIFIED.ToString(), access);
            scAccessList.Add(RWSectionNames.RW_RTD.ToString(), access);
            scAccessList.Add(RWSectionNames.RW_BOARD_SG_REV.ToString(), access);
            scAccessList.Add(RWSectionNames.RW_HQT_REV.ToString(), access);
            scAccessList.Add(RWSectionNames.RW_HQT_FINAL_REV.ToString(), access);
            scAccessList.Add(RWSectionNames.RW_SENIOR_MED_REV.ToString(), access);

            // Modify access with user role
            switch (role)
            {
                case (int)UserGroups.MedicalTechnician:
                    if (CurrentStatusCode == (int)SpecCaseRWStatusCode.MedTechInitiateCase)
                    {
                        scAccessList[RWSectionNames.RW_MED_TECH_INIT.ToString()] = PageAccessType.ReadWrite;
                        scAccessList[RWSectionNames.RW_RLB.ToString()] = PageAccessType.ReadWrite;
                    }
                    break;

                case (int)UserGroups.AFRCHQTechnician:
                    if (CurrentStatusCode == (int)SpecCaseRWStatusCode.HQAFRCTechInitiateCase)
                    {
                        scAccessList[RWSectionNames.RW_HQ_TECH_INIT.ToString()] = PageAccessType.ReadWrite;
                        scAccessList[RWSectionNames.RW_RLB.ToString()] = PageAccessType.ReadWrite;
                    }

                    if (CurrentStatusCode == (int)SpecCaseRWStatusCode.PackageReview)
                    {
                        scAccessList[RWSectionNames.RW_HQT_REV.ToString()] = PageAccessType.ReadWrite;
                    }

                    if (CurrentStatusCode == (int)SpecCaseRWStatusCode.FinalReview)
                    {
                        scAccessList[RWSectionNames.RW_HQT_FINAL_REV.ToString()] = PageAccessType.ReadWrite;
                    }

                    break;

                case (int)UserGroups.BoardMedical:
                    if (CurrentStatusCode == (int)SpecCaseRWStatusCode.MedicalReview)
                    {
                        scAccessList[RWSectionNames.RW_BOARD_SG_REV.ToString()] = PageAccessType.ReadWrite;
                        scAccessList[RWSectionNames.RW_RLB.ToString()] = PageAccessType.ReadWrite;
                    }
                    break;

                case (int)UserGroups.SeniorMedicalReviewer:
                    if (CurrentStatusCode == (int)SpecCaseRWStatusCode.SeniorMedicalReview)
                    {
                        scAccessList[RWSectionNames.RW_SENIOR_MED_REV.ToString()] = PageAccessType.ReadWrite;
                        scAccessList[RWSectionNames.RW_RLB.ToString()] = PageAccessType.ReadWrite;
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
                        o.OptionVisible = (DetermineVisibilityForCanCompleteCaseRule((int)Finding.Qualify_RTD) || DetermineVisibilityForCanCompleteCaseRule((int)Finding.Admin_Qualified));
                        break;

                    case "cancompleteasdenied":
                        o.OptionVisible = (DetermineVisibilityForCanCompleteCaseRule((int)Finding.Disqualify) || DetermineVisibilityForCanCompleteCaseRule((int)Finding.Admin_Disqualified));
                        break;

                    case "adminlod":
                        o.OptionVisible = DetermineVisibilityForCanCompleteCaseRule((int)Finding.Admin_LOD);
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
                        string memoName = String.Empty;
                        memos = r.RuleValue.ToString().Split(',');
                        for (int i = 0; i < memos.Length; i++)
                        {
                            if (!String.IsNullOrEmpty(memos[i]))
                            {
                                memoName = ((MemoType)(Convert.ToByte(memos[i]))).ToString();
                                IList<Memorandum2> memolist = (from m in memoDao.GetByRefnModule(Id, moduleId)
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
                                if (AllDocuments.ContainsKey(docName) && Required.ContainsKey(docName))
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

                    case "validatequalifymemos":
                        canforward = ValidateQualifyMemosRule(memoDao, lookupDao);

                        if (canforward != true)
                            o.OptionValid = false;
                        else
                            o.OptionValid = true;

                        break;
                }
            }
        }

        protected bool DetermineVisibilityForCanCompleteCaseRule(int expectedFinding)
        {
            if (!med_off_approved.HasValue)
            {
                AddValidationItem(new ValidationItem("Board Medical Officer", "Decision", "Board Medical Officer must provide a finding in order to close out the RW case."));
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

        protected bool ValidateCanForwardCaseRule(ILookupDao lookupDao)
        {
            switch (CurrentStatusCode)
            {
                case (int)SpecCaseRWStatusCode.MedTechInitiateCase:
                case (int)SpecCaseRWStatusCode.HQAFRCTechInitiateCase:
                    return ValidateCanForwardCaseRuleForInitiateStep();

                case (int)SpecCaseRWStatusCode.MedicalReview:
                    return ValidateCanForwardCaseRuleForBoardMedicalOfficer();

                case (int)SpecCaseRWStatusCode.SeniorMedicalReview:
                    return ValidateCanForwardCaseRuleForSeniorMedicalReviewer();

                case (int)SpecCaseRWStatusCode.FinalReview:
                    return ValidateCanForwardCaseRuleForFinalReviewStep();

                default:
                    return true;
            }
        }

        protected bool ValidateCanForwardCaseRuleForBoardMedicalOfficer()
        {
            bool canForward = true;

            if (ICD9Code == null || ICD9Code == 0)
            {
                canForward = false;
                AddValidationItem(new ValidationItem(WORKFLOW_TITLE, "Med Off ICD 10 Code", "Must select an ICD 10 Code for the RW case."));
            }

            if (string.IsNullOrEmpty(ICD9Diagnosis))
            {
                canForward = false;
                AddValidationItem(new ValidationItem(WORKFLOW_TITLE, "Med Off ICD 10 Diagnosis", "Must enter a Diagnosis for the RW case."));
            }

            if (!med_off_approved.HasValue)
            {
                canForward = false;
                AddValidationItem(new ValidationItem(WORKFLOW_TITLE, "Decision", "Must enter a Decision for the RW case."));
            }
            else
            {
                if (DoesCaseNeedDecisionExplanation(med_off_approval_comment))
                {
                    canForward = false;
                    AddValidationItem(new ValidationItem(WORKFLOW_TITLE, "Decision Explanation", "Must enter a Decision Explanation for disqualified decision for the RW case."));
                }

                if (DoesCaseNeedDQParagraph(DQParagraph))
                {
                    canForward = false;
                    AddValidationItem(new ValidationItem(WORKFLOW_TITLE, "DQ Paragraph", "Must enter a DQ Paragraph for disqualified decision for the RW case."));
                }

                if (DoesCaseNeedProcessAsSelection(ProcessAs))
                {
                    canForward = false;
                    AddValidationItem(new ValidationItem(WORKFLOW_TITLE, "Med Off Process As", "Must select a Process As option for the RW case."));
                }

                if (DoesCaseNeedReturnToDutyDate(ReturnToDutyDate))
                {
                    canForward = false;
                    AddValidationItem(new ValidationItem(WORKFLOW_TITLE, "Med Off Return to Duty Date", "Must enter a Return to Duty Date for qualified decision for the RW case."));
                }

                if (!MemoTemplateID.HasValue)
                {
                    canForward = false;
                    AddValidationItem(new ValidationItem(WORKFLOW_TITLE, "Memo", "Must select a Memo/Letter for the RW case."));
                }
                else
                {
                    if (DoesCaseNeedAdminLODMemo())
                    {
                        canForward = false;
                        AddValidationItem(new ValidationItem(WORKFLOW_TITLE, "Memo", "Must select the Admin LOD AFPC Memo/Letter for the RW case."));
                    }
                    else if (DoesCaseNeedRTDQualifyMemo())
                    {
                        canForward = false;
                        AddValidationItem(new ValidationItem(WORKFLOW_TITLE, "Memo", "Must select a RTD/Qualify memo for the RW case."));
                    }
                    else if (DoesCaseNeedDisualifyMemo())
                    {
                        canForward = false;
                        AddValidationItem(new ValidationItem(WORKFLOW_TITLE, "Memo", "Must select a Disqualify memo for the RW case."));
                    }
                }
            }

            return canForward;
        }

        protected bool ValidateCanForwardCaseRuleForInitiateStep()
        {
            bool canForward = true;

            if (!RenewalDate.HasValue)
            {
                canForward = false;
                AddValidationItem(new ValidationItem(WORKFLOW_TITLE, "Renewal Date", "Must enter a Renewal Date"));
            }

            if (!MedGroupName.HasValue)
            {
                canForward = false;
                AddValidationItem(new ValidationItem(WORKFLOW_TITLE, "Med Group Name", "Must select a Med Group Name for the RW case."));
            }

            if (RMUName == null)
            {
                canForward = false;
                AddValidationItem(new ValidationItem(WORKFLOW_TITLE, "RMU Name", "Must select an RMU Name for the RW case."));
            }

            if (ICD9Code == null || ICD9Code == 0)
            {
                canForward = false;
                AddValidationItem(new ValidationItem(WORKFLOW_TITLE, "Med Tech ICD 10 Code", "Must select an ICD 10 Code for the RW case."));
            }

            if (string.IsNullOrEmpty(ICD9Diagnosis))
            {
                canForward = false;
                AddValidationItem(new ValidationItem(WORKFLOW_TITLE, "Med Tech ICD 10 Diagnosis", "Must enter a Diagnosis for the RW case."));
            }

            if (!MissedWorkDays.HasValue || MissedWorkDays.Value <= 0)
            {
                canForward = false;
                AddValidationItem(new ValidationItem(WORKFLOW_TITLE, "Missed Work Days", "Must select a Missed Work Day for the RW case."));
            }

            if (!RequiresSpecialistForMgmt.HasValue || RequiresSpecialistForMgmt.Value <= 0)
            {
                canForward = false;
                AddValidationItem(new ValidationItem(WORKFLOW_TITLE, "Require Specialist For Management", "Must select if Specialist Required for Management for the RW case."));
            }

            if (!HasHadERorUrgentCareVisits.HasValue)
            {
                canForward = false;
                AddValidationItem(new ValidationItem(WORKFLOW_TITLE, "ER or Urgent Care Visits", "Must select if ER or Urgent Care Visits has occured for the RW case."));
            }
            else if (HasHadERorUrgentCareVisits.Value && string.IsNullOrEmpty(ERorUrgentCareVisitList))
            {
                canForward = false;
                AddValidationItem(new ValidationItem(WORKFLOW_TITLE, "ER or Urgent Care Visits Details", "Must enter ER or Urgent Care Visits Details for the RW case."));
            }

            if (!HasBeenHospitalized.HasValue)
            {
                canForward = false;
                AddValidationItem(new ValidationItem(WORKFLOW_TITLE, "Hospitalization", "Must select if Hospitalization has occured for the RW case."));
            }
            else if (HasBeenHospitalized.Value && string.IsNullOrEmpty(HospitalizationList))
            {
                canForward = false;
                AddValidationItem(new ValidationItem(WORKFLOW_TITLE, "Hospitalization Details", "Must enter Hospitalization Details for the RW case."));
            }

            if (!RiskForSuddenIncapacitation.HasValue || RiskForSuddenIncapacitation.Value <= 0)
            {
                canForward = false;
                AddValidationItem(new ValidationItem(WORKFLOW_TITLE, "Risk for Sudden Incapacitation", "Must select a Risk for Sudden Incapacitation for the RW case."));
            }

            if (!RecommendedFollowUpInterval.HasValue || RecommendedFollowUpInterval.Value <= 0)
            {
                canForward = false;
                AddValidationItem(new ValidationItem(WORKFLOW_TITLE, "Recommeded Follow Up Interval", "Must select a Recommended Follow Up Interval for the RW case."));
            }

            if (string.IsNullOrEmpty(Prognosis))
            {
                canForward = false;
                AddValidationItem(new ValidationItem(WORKFLOW_TITLE, "Prognosis", "Must enter a Prognosis for the RW case."));
            }

            if (string.IsNullOrEmpty(Treatment))
            {
                canForward = false;
                AddValidationItem(new ValidationItem(WORKFLOW_TITLE, "Treatment", "Must enter a Treatment for the RW case."));
            }

            if (string.IsNullOrEmpty(MedicationsAndDosages))
            {
                canForward = false;
                AddValidationItem(new ValidationItem(WORKFLOW_TITLE, "Medications And Dosages", "Must enter a Medications/Dosage for the RW case."));
            }

            return canForward;
        }

        private bool DoesCaseNeedAdminLODMemo()
        {
            if (GetActiveBoardMedicalFinding().HasValue && GetActiveBoardMedicalFinding().Value == (int)Finding.Admin_LOD && !IsSelectedMemoTemplateTheAdminLODAFPCTemplate())
                return true;

            return false;
        }

        private bool DoesCaseNeedDecisionExplanation(string decisionExplanation)
        {
            if (GetActiveBoardMedicalFinding().HasValue && (GetActiveBoardMedicalFinding().Value == (int)Finding.Disqualify || GetActiveBoardMedicalFinding().Value == (int)Finding.Admin_Disqualified) && string.IsNullOrEmpty(decisionExplanation))
                return true;

            return false;
        }

        private bool DoesCaseNeedDisualifyMemo()
        {
            if (GetActiveBoardMedicalFinding().HasValue && (GetActiveBoardMedicalFinding().Value == (int)Finding.Disqualify || GetActiveBoardMedicalFinding().Value == (int)Finding.Admin_Disqualified) && !IsSelectedMemoTemplateADisqualifyTemplate())
                return true;

            return false;
        }

        private bool DoesCaseNeedDQParagraph(string dqParagraph)
        {
            if (GetActiveBoardMedicalFinding().HasValue && (GetActiveBoardMedicalFinding().Value == (int)Finding.Disqualify || GetActiveBoardMedicalFinding().Value == (int)Finding.Admin_Disqualified) && string.IsNullOrEmpty(dqParagraph))
                return true;

            return false;
        }

        private bool DoesCaseNeedProcessAsSelection(int? processAs)
        {
            if (GetActiveBoardMedicalFinding().HasValue && (GetActiveBoardMedicalFinding().Value == (int)Finding.Disqualify || GetActiveBoardMedicalFinding().Value == (int)Finding.Admin_Disqualified) && !processAs.HasValue)
                return true;

            return false;
        }

        private bool DoesCaseNeedReturnToDutyDate(DateTime? returnToDutyDate)
        {
            if (GetActiveBoardMedicalFinding().HasValue && (GetActiveBoardMedicalFinding().Value == (int)Finding.Qualify_RTD || GetActiveBoardMedicalFinding().Value == (int)Finding.Admin_Qualified) && !returnToDutyDate.HasValue)
                return true;

            return false;
        }

        private bool DoesCaseNeedRTDQualifyMemo()
        {
            if (GetActiveBoardMedicalFinding().HasValue && (GetActiveBoardMedicalFinding().Value == (int)Finding.Qualify_RTD || GetActiveBoardMedicalFinding().Value == (int)Finding.Admin_Qualified) && !IsSelectedMemoTemplateAQualifyTemplate())
                return true;

            return false;
        }

        private bool IsSelectedMemoTemplateADisqualifyTemplate()
        {
            IList<int> disqualifyTemplateIds = new List<int>()
            {
                (int)MemoType.RW_DQ_MEB,
                (int)MemoType.RW_DQ_WD,
                (int)MemoType.RW_DQ_Pending_LOD
            };

            if (!GetActiveMemoTemplateId().HasValue || !disqualifyTemplateIds.Contains(GetActiveMemoTemplateId().Value))
                return false;

            return true;
        }

        private bool IsSelectedMemoTemplateAQualifyTemplate()
        {
            IList<int> qualifyTemplateIds = new List<int>()
            {
                (int)MemoType.RW_4_Non_RW_C,
                (int)MemoType.RW_4_2_RTD,
                (int)MemoType.RW_HIV,
                (int)MemoType.RW_RTD_SAF,
                (int)MemoType.RW_RTD_SG,
            };

            if (!GetActiveMemoTemplateId().HasValue || !qualifyTemplateIds.Contains(GetActiveMemoTemplateId().Value))
                return false;

            return true;
        }

        private bool IsSelectedMemoTemplateTheAdminLODAFPCTemplate()
        {
            if (!GetActiveMemoTemplateId().HasValue || GetActiveMemoTemplateId().Value != (int)MemoType.RW_Admin_LOD_AFPC)
                return false;

            return true;
        }

        private bool ValidateCanForwardCaseRuleForFinalReviewStep()
        {
            bool canForward = true;

            if (!ValidateCanForwardCaseRuleForInitiateStep())
            {
                canForward = false;
                AddValidationItem(new ValidationItem(WORKFLOW_TITLE, "Med Tech Tab", "The above items in the Med Tech tab failed validation."));
            }

            if (ShouldUseSeniorMedicalReviewerFindings())
            {
                if (!ValidateCanForwardCaseRuleForSeniorMedicalReviewer())
                {
                    canForward = false;
                    AddValidationItem(new ValidationItem(WORKFLOW_TITLE, "Med Off Tab", "The above items in the Med Off tab for the Senior Medical Reviewer failed validation."));
                }
            }
            else
            {
                if (!ValidateCanForwardCaseRuleForBoardMedicalOfficer())
                {
                    canForward = false;
                    AddValidationItem(new ValidationItem(WORKFLOW_TITLE, "Med Off Tab", "The above items in the Med Off tab for the Board Medical Officer failed validation."));
                }
            }

            return canForward;
        }

        private bool ValidateCanForwardCaseRuleForSeniorMedicalReviewer()
        {
            bool canForward = true;

            if (string.IsNullOrEmpty(SeniorMedicalReviewerConcur))
            {
                canForward = false;
                AddValidationItem(new ValidationItem("Senior Medical Reviewer", "Decision", "Must provide a Concur/Non-Concur Decision for the RW case."));
            }
            else
            {
                if (SeniorMedicalReviewerConcur.Equals(DECISION_NONCONCUR))
                {
                    if (!ICD9Code.HasValue || ICD9Code.Value == 0)
                    {
                        canForward = false;
                        AddValidationItem(new ValidationItem("Senior Medical Reviewer", "Med Off ICD 10 Code", "Must select an ICD 10 Code for the RW case."));
                    }

                    if (string.IsNullOrEmpty(ICD9Diagnosis))
                    {
                        canForward = false;
                        AddValidationItem(new ValidationItem("Senior Medical Reviewer", "Med Off ICD 10 Diagnosis", "Must enter a Diagnosis for the RW case."));
                    }

                    if (!SeniorMedicalReviewerApproved.HasValue)
                    {
                        canForward = false;
                        AddValidationItem(new ValidationItem("Senior Medical Reviewer", "Determination", "Must provide a Decision for the RW case."));
                    }
                    else
                    {
                        if (DoesCaseNeedDecisionExplanation(SeniorMedicalReviewerComment))
                        {
                            canForward = false;
                            AddValidationItem(new ValidationItem("Senior Medical Reviewer", "Decision Explanation", "Must enter a Decision Explanation for disqualified decision for the RW case."));
                        }

                        if (DoesCaseNeedDQParagraph(AlternateDQParagraph))
                        {
                            canForward = false;
                            AddValidationItem(new ValidationItem("Senior Medical Reviewer", "DQ Paragraph", "Must enter a DQ Paragraph for disqualified decision for the RW case."));
                        }

                        if (DoesCaseNeedProcessAsSelection(AlternateProcessAs))
                        {
                            canForward = false;
                            AddValidationItem(new ValidationItem("Senior Medical Reviewer", "Med Off Process As", "Must select a Process As option for the RW case."));
                        }

                        if (DoesCaseNeedReturnToDutyDate(AlternateReturnToDutyDate))
                        {
                            canForward = false;
                            AddValidationItem(new ValidationItem("Senior Medical Reviewer", "Med Off Return to Duty Date", "Must enter a Return to Duty Date for qualified decision for the RW case."));
                        }

                        if (!AlternateMemoTemplateID.HasValue)
                        {
                            canForward = false;
                            AddValidationItem(new ValidationItem("Senior Medical Reviewer", "Memo", "Must select a Memo/Letter for the RW case."));
                        }
                        else
                        {
                            if (DoesCaseNeedAdminLODMemo())
                            {
                                canForward = false;
                                AddValidationItem(new ValidationItem("Senior Medical Reviewer", "Memo", "Must select the Admin LOD AFPC Memo/Letter for the RW case."));
                            }
                            else if (DoesCaseNeedRTDQualifyMemo())
                            {
                                canForward = false;
                                AddValidationItem(new ValidationItem("Senior Medical Reviewer", "Memo", "Must select a RTD/Qualify memo for the RW case."));
                            }
                            else if (DoesCaseNeedDisualifyMemo())
                            {
                                canForward = false;
                                AddValidationItem(new ValidationItem("Senior Medical Reviewer", "Memo", "Must select a Disqualify memo for the RW case."));
                            }
                        }
                    }
                }
            }

            return canForward;
        }

        private bool ValidateQualifyMemosRule(IMemoDao2 memoDao, ILookupDao lookupDao)
        {
            bool canForward = true;

            if (!GetActiveMemoTemplateId().HasValue)
            {
                canForward = false;
                AddValidationItem(new ValidationItem(WORKFLOW_TITLE, "Memo", "Must select a Memo/Letter for the RW case."));
            }
            else
            {
                IList<Memorandum2> memolist = (from m in memoDao.GetByRefnModule(Id, moduleId)
                                               where m.Deleted == false
                                               select m).ToList<Memorandum2>();

                if (memolist.Count < 1)
                {
                    canForward = false;
                    AddValidationItem(new ValidationItem(WORKFLOW_TITLE, "Memo", "No memo has been found for the RW case."));
                }
                else if (memolist.Where(x => x.Template.Id == GetActiveMemoTemplateId().Value).ToList().Count < 1)
                {
                    canForward = false;
                    AddValidationItem(new ValidationItem(WORKFLOW_TITLE, "Memo", "The selected memo has not been found for the RW case."));
                }
                else if (DoesCaseNeedAdminLODMemo())
                {
                    canForward = false;
                    AddValidationItem(new ValidationItem(WORKFLOW_TITLE, "Memo", "No Admin LOD AFPC memo has been found for the RW case."));
                }
                else if (DoesCaseNeedRTDQualifyMemo())
                {
                    canForward = false;
                    AddValidationItem(new ValidationItem(WORKFLOW_TITLE, "Memo", "No RTD memo has been found for the RW case."));
                }
                else if (DoesCaseNeedDisualifyMemo())
                {
                    canForward = false;
                    AddValidationItem(new ValidationItem(WORKFLOW_TITLE, "Memo", "No Disqualification memo has been found for the RW case."));
                }
            }

            return canForward;
        }
    }
}