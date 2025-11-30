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
    public class SC_WWD : SpecialCase
    {
        public const int ADMINLOD_FINDING_VALUE = 2;
        public const int DISQUALIFY_FINDING_VALUE = 0;
        public const int QUALIFY_FINDING_VALUE = 1;
        public const int RFA_FINDING_VALUE = 3;

        private int CoverLetterCount = 0;
        private bool isps3811Valid;

        public SC_WWD()
        {
            PointOfContact = new SignatureEntry();
            SAFLetterUploaded = 0;
            MUQUploaded = 0;
        }

        public virtual int? AFForm469Uploaded { get; set; }
        public virtual int? ALCLetterType { get; set; }
        public virtual int? AlternateALCLetterType { get; set; }
        public virtual DateTime? AlternateDQCompletionDate { get; set; }
        public virtual string AlternateDQParagraph { get; set; }
        public virtual DateTime? AlternateExpirationDate { get; set; }
        public virtual int? AlternateMemoTemplateID { get; set; }
        public virtual DateTime? AlternateReturnToDutyDate { get; set; }

        /// <inheritdoc/>
        public override int? AssociatedWWD { get; set; }

        public virtual DateTime? Code37InitDate { get; set; }
        public virtual int? CoverLetterUploaded { get; set; }
        public virtual Boolean? CoverLtrIncContactAttemptDetails { get; set; }
        public virtual Boolean? CoverLtrIncMemberStatement { get; set; }

        /// <inheritdoc/>
        public override int DocumentViewId
        {
            get { return (int)DocumentViewType.WWD; }
        }

        public virtual DateTime? DQCompletionDate { get; set; }
        public virtual string DQParagraph { get; set; }
        public virtual DateTime? FirstClassMailDate { get; set; }
        public virtual int? HQTechDisposition { get; set; }
        public virtual string ICD7thCharacter { get; set; }

        // Qualified, Disqualified, or RWOA
        public virtual int? ICD9Code { get; set; }

        public virtual String ICD9Description { get; set; }
        public virtual String ICD9Diagnosis { get; set; }
        public virtual int? IPEBElection { get; set; }
        public virtual int? IPEBRefusal { get; set; }
        public virtual DateTime? IPEBSignatureDate { get; set; }
        public virtual DateTime? MedEvalFactSheetSignDate { get; set; }
        public virtual DateTime? MedEvalFSWaiverSignDate { get; set; }
        public virtual int? MedGroupName { get; set; }
        public virtual int? MemberLetterUploaded { get; set; }
        public virtual int? MemoTemplateID { get; set; }
        public virtual bool? MUQ_Valid { get; set; }
        public virtual DateTime? MUQRequestDate { get; set; }
        public virtual DateTime? MUQUploadDate { get; set; }
        public virtual int MUQUploaded { get; set; }
        public virtual int? NarrativeSummaryUploaded { get; set; }
        public virtual string PocEmail { get; set; }
        public virtual string PocPhoneDSN { get; set; }
        public virtual string PocUnit { get; set; }  // RMU
        public virtual SignatureEntry PointOfContact { get; set; }
        public virtual int? PrivatePhysicianDocsUploaded { get; set; }
        public virtual DateTime? PS3811RequestDate { get; set; }

        // Certified Mail Request Signature
        public virtual DateTime? PS3811SignDate { get; set; }

        // Certified Mail Receipt Signature
        public virtual int? PS3811Uploaded { get; set; }

        public virtual DateTime? ReturnToDutyDate { get; set; }
        public virtual int? RMUName { get; set; }
        public virtual DateTime? SAFLetterUploadDate { get; set; }
        public virtual int SAFLetterUploaded { get; set; }

        public virtual int? UnitCmdrMemoUploaded { get; set; }

        /// <inheritdoc/>
        public override WorkStatus WorkflowStatus { get; set; }

        //case data
        public virtual int? WWDDocsAttached { get; set; }

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
            if ((CurrentStatusCode == (int)SpecCaseWWDStatusCode.Approved) || (CurrentStatusCode == (int)SpecCaseWWDStatusCode.Denied))
            {
                access = PageAccessType.ReadOnly;
            }

            //'Add all pages as readonly

            scAccessList.Add(WDSectionNames.WD_MED_TECH_INIT.ToString(), access);
            scAccessList.Add(WDSectionNames.WD_HQT_REV.ToString(), access);
            scAccessList.Add(WDSectionNames.WD_BOARD_SG_REV.ToString(), access);
            scAccessList.Add(WDSectionNames.WD_HQT_FINAL_REV.ToString(), access);
            scAccessList.Add(WDSectionNames.WD_APPROVED.ToString(), access);
            scAccessList.Add(WDSectionNames.WD_DENIED.ToString(), access);
            scAccessList.Add(WDSectionNames.WD_RLB.ToString(), access);

            //Modify access with user role
            switch (role)
            {
                case (int)UserGroups.AFRCHQTechnician:
                    if (CurrentStatusCode == (int)SpecCaseWWDStatusCode.PackageReview)
                    {
                        scAccessList[WDSectionNames.WD_HQT_REV.ToString()] = PageAccessType.ReadWrite;
                        scAccessList[WDSectionNames.WD_RLB.ToString()] = PageAccessType.ReadOnly;
                    }
                    if (CurrentStatusCode == (int)SpecCaseWWDStatusCode.FinalReview || CurrentStatusCode == (int)SpecCaseWWDStatusCode.FinalReviewSAF || CurrentStatusCode == (int)SpecCaseWWDStatusCode.FinalReviewIPEB || CurrentStatusCode == (int)SpecCaseWWDStatusCode.FinalReviewFPEB)
                    {
                        scAccessList[WDSectionNames.WD_HQT_FINAL_REV.ToString()] = PageAccessType.ReadWrite;
                    }
                    if (CurrentStatusCode == (int)SpecCaseWWDStatusCode.FinalReview)
                    {
                        scAccessList[WDSectionNames.WD_RLB.ToString()] = PageAccessType.ReadOnly;
                    }
                    break;

                case (int)UserGroups.BoardMedical:
                    if (CurrentStatusCode == (int)SpecCaseWWDStatusCode.MedicalReview)
                    {
                        scAccessList[WDSectionNames.WD_BOARD_SG_REV.ToString()] = PageAccessType.ReadWrite;
                    }
                    break;

                case (int)UserGroups.MedicalTechnician:
                    if (CurrentStatusCode == (int)SpecCaseWWDStatusCode.InitiateCase)
                    {
                        scAccessList[WDSectionNames.WD_MED_TECH_INIT.ToString()] = PageAccessType.ReadWrite;
                        scAccessList[WDSectionNames.WD_RLB.ToString()] = PageAccessType.ReadWrite;
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

                    case "hqtechapprove":
                        o.OptionVisible = DetermineVisibilityForHQTechCanCompleteCaseRule(QUALIFY_FINDING_VALUE);
                        break;

                    case "hqtechdeny":
                        o.OptionVisible = DetermineVisibilityForHQTechCanCompleteCaseRule(DISQUALIFY_FINDING_VALUE);
                        break;

                    case "adminlod":
                        o.OptionVisible = DetermineVisibilityForAdminLodRule(o.Id);
                        break;

                    case "adminlodonly":
                        o.OptionVisible = DetermineVisibilityForAdminLodOnlyRule();
                        break;

                    case "cancompleteasapproved":
                        o.OptionVisible = (DetermineVisibilityForCanCompleteCaseRule(QUALIFY_FINDING_VALUE) || DetermineVisibilityForCanCompleteCaseRule(ADMINLOD_FINDING_VALUE));
                        break;

                    case "cancompleteasdenied":
                        o.OptionVisible = DetermineVisibilityForCanCompleteCaseRule(DISQUALIFY_FINDING_VALUE);
                        break;

                    case "usergroupcontainsdecision":
                        o.OptionVisible = DetermineVisibilityForUserGroupContainsDecisionRule(new UserGroupContainsDecisionRuleData(r));
                        break;

                    case "rfa":
                        o.OptionVisible = DetermineVisibiltyForRFARule(RFA_FINDING_VALUE);
                        break;
                }
            }

            if (r.RuleTypes.ruleTypeId == (int)RuleKind.Validation)
            {
                bool canForward = true;

                //Validation Rule
                switch (r.RuleTypes.Name.ToLower())
                {
                    case "hqtechvalidate":
                        canForward = ValidateHQTechValidateRule();

                        if (Convert.ToBoolean(r.RuleValue.ToLower()) == true && canForward != true)
                            o.OptionValid = false;
                        else
                            o.OptionValid = true;
                        break;

                    case "medical":
                        if (CurrentStatusCode == (int)SpecCaseWWDStatusCode.MedicalReview ||
                            CurrentStatusCode == (int)SpecCaseWWDStatusCode.MedicalReviewFPEB ||
                            CurrentStatusCode == (int)SpecCaseWWDStatusCode.MedicalReviewSAF ||
                            CurrentStatusCode == (int)SpecCaseWWDStatusCode.MedicalReviewIPEB)
                        {
                            canForward = ValidateMedicalRuleForBoardMedicalOfficer();
                        }

                        if (CurrentStatusCode == (int)SpecCaseWWDStatusCode.SeniorMedicalReview ||
                            CurrentStatusCode == (int)SpecCaseWWDStatusCode.SeniorMedicalReviewSAF ||
                            CurrentStatusCode == (int)SpecCaseWWDStatusCode.SeniorMedicalReviewIPEB ||
                            CurrentStatusCode == (int)SpecCaseWWDStatusCode.SeniorMedicalReviewFPEB)
                        {
                            canForward = ValidateMedicalRuleForSeniorMedicalReviewer();
                        }

                        if (canForward != true)
                            o.OptionValid = false;
                        else
                            o.OptionValid = true;
                        break;

                    case "canforwardcase":
                        canForward = ValidateCanForwardCaseRule();

                        if (Convert.ToBoolean(r.RuleValue.ToLower()) == true && canForward != true)
                            o.OptionValid = false;
                        else
                            o.OptionValid = true;
                        break;

                    case "validatequalifymemos":
                        canForward = ValidateQualifyMemosRule(memoDao);

                        if (canForward != true)
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

        /// <inheritdoc/>
        protected override bool IsDocumentCategoryStillRequired(bool docFound, string docName, IDaoFactory daoFactory)
        {
            bool isRequired = true;

            if (!docFound)
            {
                if (docName == DocumentType.WWDCoverLetter.ToString())
                {
                    CoverLetterCount = 0;
                }
                if (WWDDocsAttached == 1)
                {
                    if (docName == DocumentType.AFForm469.ToString() && AFForm469Uploaded == 1)
                        isRequired = true;

                    if (docName == DocumentType.NarrativeSummary.ToString() && NarrativeSummaryUploaded == 1)
                        isRequired = true;

                    if (docName == DocumentType.IPEBElection.ToString() && IPEBRefusal == 1 && IPEBSignatureDate != null)
                        isRequired = false;  // Election Waived

                    if (docName == DocumentType.UnitCmdrMemo.ToString() && UnitCmdrMemoUploaded == 1)
                        isRequired = true;

                    if (docName == DocumentType.MedicalEvaluationFactSheet.ToString() && MedEvalFSWaiverSignDate == null)
                        isRequired = true;  // Required unless the waiver date was entered

                    if (docName == DocumentType.MedicalEvaluationFactSheet.ToString() && MedEvalFSWaiverSignDate != null)
                        isRequired = false;  // Not Required since the waiver date was entered

                    if (docName == DocumentType.PrivatePhysicianDocs.ToString() && PrivatePhysicianDocsUploaded == 1)
                        isRequired = true;

                    if (docName == DocumentType.PS3811.ToString())
                        isRequired = false;
                }
                if (WWDDocsAttached == 0)
                {
                    if (docName == DocumentType.PS3811.ToString())
                    {
                        if (isps3811Valid == false)
                        {
                            isRequired = true;
                        }
                        else
                        {
                            isRequired = false;
                        }
                    }
                    if (docName == DocumentType.LetterToMemberRequestingDocumentation.ToString() && MemberLetterUploaded == 1)
                        isRequired = true;
                }
            }

            if ((GetActiveBoardMedicalFinding() == DISQUALIFY_FINDING_VALUE) & (SAFLetterUploaded == 0) && ((Status == (int)SpecCaseWWDWorkStatus.AwaitingSAF) || (Status == (int)SpecCaseWWDWorkStatus.IPEBDisposition) || (Status == (int)SpecCaseWWDWorkStatus.FPEBDisposition)))
            {
                isRequired = true;
            }

            if (WWDDocsAttached == 1)
            {
                // Can't do switch with enums - expects Constant values
                // So we have to test for each doc type 1 at a time
                // ... and make them not required
                if (docName == DocumentType.PS3811.ToString())
                {
                    isRequired = false;
                }

                if (docName == DocumentType.LetterToMemberRequestingDocumentation.ToString())
                {
                    isRequired = false;
                }

                // validation okay
                if (docName == DocumentType.IPEBElection.ToString() && IPEBSignatureDate != null)
                {
                    if ((IPEBElection == 1) || (IPEBElection == 0))  // IPEB Election Made (Yes/No)
                    {
                        isRequired = false;
                    }
                    if ((IPEBRefusal == 1))  // IPEB Refusal Made (Yes/No)
                    {
                        isRequired = false;
                    }
                }

                if (docName == DocumentType.MemberUtilizationQuestionnaire.ToString() && MUQUploadDate == null)
                {
                    isRequired = false;
                }

                if (docName == DocumentType.MemberUtilizationQuestionnaire.ToString() && MUQUploadDate != null && MUQUploaded != 1)
                {
                    isRequired = true;
                }

                if (docName == DocumentType.MemberUtilizationQuestionnaire.ToString() && MUQUploadDate != null && MUQUploaded == 1)
                {
                    isRequired = true;
                }

                if (docName == DocumentType.MedicalEvaluationFactSheet.ToString() && MedEvalFSWaiverSignDate != null)  // Waiver signed
                {
                    isRequired = false;
                }

                if (docName == DocumentType.WWDCoverLetter.ToString())
                {
                    if (CoverLetterCount == 0)
                    {  // doc not uploaded
                        if (CoverLetterUploaded == 0)
                        {  // user did not indicate upload
                            isRequired = false;
                        }
                        else
                        {  // user indicated upload
                            isRequired = true;
                        }
                    }
                    else
                    {
                        // cover letter uploaded
                        isRequired = true;
                    }
                }
            }

            if (WWDDocsAttached == 0)
            {
                // Can't do switch with enums - expects Constant values
                // So we have to test for each doc type 1 at a time
                // ... and make them not required
                if (docName == DocumentType.AFForm469.ToString())
                {
                    isRequired = false;
                }

                if (docName == DocumentType.NarrativeSummary.ToString())
                {
                    isRequired = false;
                }

                if (docName == DocumentType.IPEBElection.ToString())
                {
                    isRequired = false;
                }

                if (docName == DocumentType.UnitCmdrMemo.ToString())
                {
                    isRequired = false;
                }

                if (docName == DocumentType.MedicalEvaluationFactSheet.ToString())
                {
                    isRequired = false;
                }

                if (docName == DocumentType.PrivatePhysicianDocs.ToString())
                {
                    isRequired = false;
                }

                if (docName == DocumentType.MemberUtilizationQuestionnaire.ToString())
                {
                    isRequired = false;
                }

                // validation okay
                if (docName == DocumentType.PS3811.ToString() && FirstClassMailDate != null)
                {
                    isRequired = false;
                }
            }

            if (WWDDocsAttached == null && docName != DocumentType.WWDCoverLetter.ToString())
            {
                // Don't validate if no selection has been made - as you don't know which ones to validate
                isRequired = false;
            }

            return isRequired;
        }

        /// <inheritdoc/>
        protected override void PerformAdditionalDocumentProcessing(string docName, bool docFound, IDaoFactory daoFactory)
        {
            if (docFound)
            {
                if (docName == DocumentType.WWDCoverLetter.ToString())
                {
                    CoverLetterCount = 1;
                    CoverLetterUploaded = 1;
                }

                if (docName == DocumentType.AFForm469.ToString())
                    AFForm469Uploaded = 1;

                if (docName == DocumentType.NarrativeSummary.ToString())
                    NarrativeSummaryUploaded = 1;

                if (docName == DocumentType.WWD_Disposition.ToString())
                    SAFLetterUploaded = 1;

                if (docName == DocumentType.MemberUtilizationQuestionnaire.ToString())
                    MUQUploaded = 1;

                if (docName == DocumentType.MemberUtilizationQuestionnaire.ToString())
                {
                    IList<Document> doclist = (from p in Documents where p.DocType.ToString() == docName select p).ToList<Document>();

                    if (MUQUploadDate == null)  // assume only 1 MUQ
                        MUQUploadDate = doclist[0].DocDate;

                    if (MUQUploadDate == null)  // default if the user didn't give the upload a date
                        MUQUploadDate = doclist[0].DateAdded;

                    if (MUQRequestDate == null)
                    {
                        MUQ_Valid = false;
                    }
                    else
                    {
                        DateTime temp1 = (DateTime)MUQUploadDate;
                        DateTime temp2 = (DateTime)MUQRequestDate;
                        TimeSpan diff = temp1.Subtract(temp2);

                        if (diff.Days > 60)
                            MUQ_Valid = false;
                        else
                            MUQ_Valid = true;
                    }
                }

                if (docName == DocumentType.UnitCmdrMemo.ToString())
                    UnitCmdrMemoUploaded = 1;

                if (docName == DocumentType.PrivatePhysicianDocs.ToString())
                    PrivatePhysicianDocsUploaded = 1;

                if (docName == DocumentType.PS3811.ToString())
                {
                    PS3811Uploaded = 1;
                    isps3811Valid = true;
                }

                if (docName == DocumentType.LetterToMemberRequestingDocumentation.ToString())
                    MemberLetterUploaded = 1;
            }
            else
            {
                if (AssociatedWWD == null && docName == DocumentType.WWDCoverLetter.ToString())
                {
                    CoverLetterCount = 0;
                }
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
                    if (docName == DocumentType.WWD_Disposition.ToString())
                    {
                        if (GetActiveBoardMedicalFinding().HasValue && GetActiveBoardMedicalFinding().Value == DISQUALIFY_FINDING_VALUE)
                        {
                            //SelectionType = "Disqualified by Medical Officer";
                            AddDocumentCategoryValidationItem(docName, viewCats);
                        }
                        else
                        {
                            //Is not required if Board Medical Decision is Admin LOD or Qualified.
                            Required.Remove(docName);
                        }
                    }
                    else
                    {
                        AddDocumentCategoryValidationItem(docName, viewCats, "  Upload or uncheck.");
                    }
                }

                AllDocuments[docName] = newIsRequired;
            }
        }

        private bool DetermineVisibilityForAdminLodOnlyRule()
        {
            if (WorkflowStatus.Id == (int)SpecCaseWWDWorkStatus.MedicalReview && med_off_approved.HasValue && med_off_approved.Value != ADMINLOD_FINDING_VALUE)
                return false;

            return true;
        }

        private bool DetermineVisibilityForAdminLodRule(int workStatusOptionId)
        {
            if (WorkflowStatus.Id == (int)SpecCaseWWDWorkStatus.FinalReview)
            {
                if (!GetActiveBoardMedicalFinding().HasValue)
                    return true;

                if (GetActiveBoardMedicalFinding().Value == ADMINLOD_FINDING_VALUE || GetActiveBoardMedicalFinding().Value == QUALIFY_FINDING_VALUE)
                {
                    // This is the workstatus option ID for Close case Admin LOD
                    if (workStatusOptionId == 196 || workStatusOptionId == 160)
                        return false;
                }
                else if (GetActiveBoardMedicalFinding().Value == DISQUALIFY_FINDING_VALUE)
                {
                    //  This is the workstatus option ID for Close case Admin LOD
                    if (workStatusOptionId == 196)
                        return false;
                }
            }

            return true;
        }

        private bool DetermineVisibilityForCanCompleteCaseRule(int expectedFinding)
        {
            if (!med_off_approved.HasValue)
            {
                AddValidationItem(new ValidationItem("Board Medical Officer", "Decision", "Board Medical Officer must provide a finding in order to close out the WWD case."));
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

        private bool DetermineVisibilityForHQTechCanCompleteCaseRule(int expectedFinding)
        {
            if (!hqt_approval1.HasValue)
                return false;

            if (hqt_approval1.Value != expectedFinding)
                return false;

            return true;
        }

        private bool DetermineVisibiltyForRFARule(int expectedFinding)
        {
            if (GetActiveBoardMedicalFinding() != null && GetActiveBoardMedicalFinding() == expectedFinding)
                return false;
            else
                return true;
        }

        private bool ValidateCanForwardCaseRule()
        {
            bool canForward = true;
            DateTime tempMUQDate;
            TimeSpan MUQSpan;
            DateTime tempMEFactSheetDate;
            TimeSpan MEFactSheetSpan;
            DateTime TempBadTime = Convert.ToDateTime("11/11/1911");
            string valError = "";

            if (MedGroupName == null)
            {
                canForward = false;
                AddValidationItem(new ValidationItem("World Wide Duty", "Med Group Name", "Must select a Med Group Name for the WWD case."));
            }

            if (RMUName == null)
            {
                canForward = false;
                AddValidationItem(new ValidationItem("World Wide Duty", "RMU Name", "Must select an RMU Name for the WWD case."));
            }

            if (ICD9Code == null || ICD9Code == 0)
            {
                canForward = false;
                AddValidationItem(new ValidationItem("World Wide Duty", "ICD 9 Code", "Must select an ICD 10 Code for the WWD case."));
            }

            if (MUQRequestDate != null)
            {
                tempMUQDate = (DateTime)MUQRequestDate;
                MUQSpan = DateTime.Now - tempMUQDate;
            }
            else
            {
                MUQSpan = DateTime.Now - TempBadTime;
            }
            if (MedEvalFactSheetSignDate != null)
            {
                tempMEFactSheetDate = (DateTime)MedEvalFactSheetSignDate;
                MEFactSheetSpan = DateTime.Now - tempMEFactSheetDate;
            }
            else
            {
                MEFactSheetSpan = DateTime.Now - TempBadTime;
            }

            if (WWDDocsAttached == null)
            {
                canForward = false;
                valError = "Must select whether or not Documents are included with WWD case.";
                AddValidationItem(new ValidationItem("World Wide Duty", "Docs Attached Selection", valError, true));
            }

            if (WWDDocsAttached == 1)
            {
                if (Code37InitDate == null)
                {
                    canForward = false;
                    valError = "Code 37 Initiation Date must be entered.";
                    AddValidationItem(new ValidationItem("World Wide Duty", "Code 37 Date", valError, true));
                }

                if ((IPEBElection == null) && (IPEBRefusal == null))
                {
                    canForward = false;
                    valError = "Need either an IPEB Election or a Refusal of such election.";
                    AddValidationItem(new ValidationItem("World Wide Duty", "IPEB Election", valError, true));
                }
                if ((CoverLtrIncMemberStatement != true) && (MUQ_Valid != true))
                {
                    canForward = false;
                    valError = "MUQ must be valid or cover letter should include Member Statement.";
                    AddValidationItem(new ValidationItem("World Wide Duty", "MUQ Validity", valError, true));
                }
                if (MUQRequestDate == null)
                {
                    canForward = false;
                    valError = "MUQ Request Date must be entered.";
                    AddValidationItem(new ValidationItem("World Wide Duty", "MUQ Request Date", valError, true));
                }
                if ((MUQSpan.TotalDays < 60) && (MUQUploadDate == null))
                {
                    canForward = false;
                    valError = "MUQ Request Date must at least 60 days old if there is no MUQ upload Date for the WWD case package.";
                    AddValidationItem(new ValidationItem("World Wide Duty", "HQ Tech MUQ Request Date", valError));
                }
                if ((MUQSpan.TotalDays > 60) && (MUQUploadDate == null) && ((CoverLtrIncMemberStatement == false) || (CoverLtrIncMemberStatement == null)))
                {
                    canForward = false;
                    valError = "Member statement included in cover letter must be marked YES if the MUQ Request Date is over 60 days old and there is no MUQ upload date for the WWD case package.";
                    AddValidationItem(new ValidationItem("World Wide Duty", "HQ Tech MUQ Request Date", valError));
                }

                if ((MEFactSheetSpan.TotalDays < 60) && (MedEvalFSWaiverSignDate == null))
                {
                    canForward = false;
                    valError = "Medical Evaluation Fact Sheet Signature Date must be greater than 60 days or the ME Fact Sheet Waiver – Signature Date must be present.";
                    AddValidationItem(new ValidationItem("World Wide Duty", "Medical Evaluation Fact Sheet", valError, true));
                }
                if ((CoverLetterUploaded == 0) && ((CoverLtrIncContactAttemptDetails == true) || (CoverLtrIncMemberStatement == true)))
                {
                    canForward = false;
                    valError = "No Cover Letter provided when selections indicate that sections [contact attempts/member statement] are included within said cover letter.";
                    AddValidationItem(new ValidationItem("World Wide Duty", "Conver Letter Inclusions", valError, true));
                }
            }

            if (WWDDocsAttached == 0)
            {
                if ((PS3811RequestDate == null))
                {
                    canForward = false;
                    valError = "Need a Certified Mail (PS3811) Request/Send Date.";
                    AddValidationItem(new ValidationItem("World Wide Duty", "PS3811 Request", valError, true));
                }
                if ((PS3811Uploaded == 0) || (PS3811Uploaded == null))
                {
                    canForward = false;
                    valError = "PS3811(uploaded) must be marked yes in order to move case forward.";
                    AddValidationItem(new ValidationItem("World Wide Duty", "PS3811 Uploaded", valError, true));
                }
                if ((MemberLetterUploaded == 0) || (MemberLetterUploaded == null))
                {
                    canForward = false;
                    valError = "Copy of Letter to Member & Attachment List(uploaded) must be marked yes in order to move case forward.";
                    AddValidationItem(new ValidationItem("World Wide Duty", "Member Letter Uploaded", valError, true));
                }
                if ((PS3811SignDate != null) && (PS3811RequestDate != null))
                {
                    DateTime temp1 = (DateTime)PS3811RequestDate;
                    DateTime temp2 = (DateTime)PS3811SignDate;
                    TimeSpan diff = temp2.Subtract(temp1);
                    if (diff.Days < 1)
                    {
                        canForward = false;
                        valError = "PS3811 Signature date has to be after PS3811 Request date.";
                        AddValidationItem(new ValidationItem("World Wide Duty", "PS3811 Invalid Signature Date", valError, true));
                    }
                }
                if ((PS3811SignDate == null) && (FirstClassMailDate == null))
                {
                    canForward = false;
                    valError = "Need either a PS3811 signature date or a First Class postal mail date.";
                    AddValidationItem(new ValidationItem("World Wide Duty", "No Date (PS3811 Signature or First Class Mail)", valError, true));
                }
            }

            return canForward;
        }

        private bool ValidateHQTechValidateRule()
        {
            bool canForward = true;

            if (SAFLetterUploaded != 1 && GetActiveBoardMedicalFinding() == DISQUALIFY_FINDING_VALUE && (Status == (int)SpecCaseWWDWorkStatus.AwaitingSAF || Status == (int)SpecCaseWWDWorkStatus.IPEBDisposition || Status == (int)SpecCaseWWDWorkStatus.FPEBDisposition))
            {
                canForward = false;
                AddValidationItem(new ValidationItem("World Wide Duty", "SAF Orders/Letter document", "Must upload a SAF Orders/Letter document for the Disqualified WWD case."));
            }

            return canForward;
        }

        private bool ValidateMedicalRuleForBoardMedicalOfficer()
        {
            bool canForward = true;

            if (ICD9Code == null || ICD9Code == 0)
            {
                canForward = false;
                AddValidationItem(new ValidationItem("World Wide Duty", "Med Off ICD 9 Code", "Must select an ICD 10 Code for the WWD case."));
            }

            if (med_off_approved == null)
            {
                canForward = false;
                AddValidationItem(new ValidationItem("World Wide Duty", "Med Off Qualification", "Must decide whether or not to Qualify the WWD case."));
            }

            if (med_off_approved != null && !MemoTemplateID.HasValue)
            {
                canForward = false;
                AddValidationItem(new ValidationItem("Memos", "Med Off Select Memo", "Must select a Memo/Letter corresponding to Return to Duty decision."));
            }

            if (med_off_approved == ADMINLOD_FINDING_VALUE && MemoTemplateID != (int)MemoType.WWD_Admin_LOD_AFPC)
            {
                canForward = false;
                AddValidationItem(new ValidationItem("Memos", "Admin LOD Memo", "Please select WWD Admin LOD AFPC Letter."));
            }

            if (med_off_approved == DISQUALIFY_FINDING_VALUE && MemoTemplateID != (int)MemoType.WWD_Disqualification_Letter)
            {
                canForward = false;
                AddValidationItem(new ValidationItem("Memos", "Disqul Memo", "Please select the WWD Disqualification Letter."));
            }

            if (ReturnToDutyDate == null && med_off_approved == QUALIFY_FINDING_VALUE)
            {
                canForward = false;
                AddValidationItem(new ValidationItem("World Wide Duty", "Return To Duty Date", "Must choose a Return To Duty Date for Qualified WWD Case."));
            }
            if ((DQParagraph == null || DQParagraph == "") && med_off_approved == DISQUALIFY_FINDING_VALUE)
            {
                canForward = false;
                AddValidationItem(new ValidationItem("World Wide Duty", "Return To Duty Date", "Must enter a DQ paragraph if you choose Disqualify for the WWD Case."));
            }

            return canForward;
        }

        private bool ValidateMedicalRuleForSeniorMedicalReviewer()
        {
            bool canForward = true;

            if (string.IsNullOrEmpty(SeniorMedicalReviewerConcur))
            {
                canForward = false;
                AddValidationItem(new ValidationItem("Senior Medical Reviewer", "Decision", "Must provide a Concur/Non-Concur Decision for the WWD case."));
            }
            else
            {
                if (SeniorMedicalReviewerConcur.Equals(DECISION_NONCONCUR))
                {
                    if (!ICD9Code.HasValue || ICD9Code.Value == 0)
                    {
                        canForward = false;
                        AddValidationItem(new ValidationItem("Senior Medical Reviewer", "Med Off ICD 10 Code", "Must select an ICD 10 Code for the WWD case."));
                    }

                    if (string.IsNullOrEmpty(ICD9Diagnosis))
                    {
                        canForward = false;
                        AddValidationItem(new ValidationItem("Senior Medical Reviewer", "Med Off ICD 10 Diagnosis", "Must enter a Diagnosis for the WWD case."));
                    }

                    if (!SeniorMedicalReviewerApproved.HasValue)
                    {
                        canForward = false;
                        AddValidationItem(new ValidationItem("Senior Medical Reviewer", "Determination", "Must provide a Decision for the WWD case."));
                    }
                    else
                    {
                        if (!AlternateMemoTemplateID.HasValue)
                        {
                            canForward = false;
                            AddValidationItem(new ValidationItem("Senior Medical Reviewer", "Med Off Select Memo", "Must select a Memo/Letter corresponding to Return to Duty decision."));
                        }
                        else
                        {
                            if (SeniorMedicalReviewerApproved.Value == ADMINLOD_FINDING_VALUE && AlternateMemoTemplateID.Value != (int)MemoType.WWD_Admin_LOD_AFPC)
                            {
                                canForward = false;
                                AddValidationItem(new ValidationItem("Senior Medical Reviewer", "Admin LOD Memo", "Please select WWD Admin LOD AFPC Letter."));
                            }

                            if (SeniorMedicalReviewerApproved.Value == DISQUALIFY_FINDING_VALUE && AlternateMemoTemplateID.Value != (int)MemoType.WWD_Disqualification_Letter)
                            {
                                canForward = false;
                                AddValidationItem(new ValidationItem("Senior Medical Reviewer", "Disqul Memo", "Please select the WWD Disqualification Letter."));
                            }
                        }

                        if (!AlternateReturnToDutyDate.HasValue && SeniorMedicalReviewerApproved.Value == QUALIFY_FINDING_VALUE)
                        {
                            canForward = false;
                            AddValidationItem(new ValidationItem("Senior Medical Reviewer", "Return To Duty Date", "Must choose a Return To Duty Date for Qualified WWD Case."));
                        }

                        if (string.IsNullOrEmpty(AlternateDQParagraph) && SeniorMedicalReviewerApproved.Value == DISQUALIFY_FINDING_VALUE)
                        {
                            canForward = false;
                            AddValidationItem(new ValidationItem("Senior Medical Reviewer", "DQ Paragraph", "Must enter a DQ paragraph if you choose Disqualify for the WWD Case."));
                        }
                    }
                }
            }

            return canForward;
        }

        private bool ValidateQualifyMemosRule(IMemoDao2 memoDao)
        {
            bool canForward = true;

            // For HQ Tech: validates one memo from a group associated with a step in WWD workflow.
            if ((Status == (int)SpecCaseWWDWorkStatus.FinalReview || Status == (int)SpecCaseWWDWorkStatus.AdminLOD) ||
                (Status == (int)SpecCaseWWDWorkStatus.PackageReview && (GetActiveBoardMedicalFinding() == ADMINLOD_FINDING_VALUE)))
            {
                IList<Memorandum2> memolist = (from m in memoDao.GetByRefnModule(Id, (int)ModuleType.SpecCaseWWD)
                                               where m.Deleted == false && m.Template.Id == Convert.ToByte(GetActiveMemoTemplateId())
                                               select m).ToList<Memorandum2>();

                if (memolist.Count < 1 && GetActiveMemoTemplateId().HasValue)
                {
                    canForward = false;
                    string memoName = (from m in memoDao.GetAllTemplates() where m.Id == Convert.ToByte(GetActiveMemoTemplateId()) select m.Title).SingleOrDefault();
                    AddValidationItem(new ValidationItem("Memos", "Memo", memoName + "  Memo  not found.", true));
                }

                // check for MEB Disqual Letter from Board Medical step if Admin LOD is selected
                if (GetActiveBoardMedicalFinding() == ADMINLOD_FINDING_VALUE)
                {
                    IList<Memorandum2> memolist2 = (from m in memoDao.GetByRefnModule(Id, (int)ModuleType.SpecCaseWWD)
                                                    where m.Deleted == false && m.Template.Id == Convert.ToByte(MemoType.MEB_Disqualification_Letter)
                                                    select m).ToList<Memorandum2>();

                    if (memolist2.Count < 1)
                    {
                        canForward = false;
                        string memoName2 = (from m in memoDao.GetAllTemplates() where m.Id == Convert.ToByte(MemoType.MEB_Disqualification_Letter) select m.Title).Single();
                        AddValidationItem(new ValidationItem("Memos", "MEB Memo", memoName2 + "  Memo  not found.", true));
                    }
                }

                // check for Unit Disqual Letter from Board Medical step if Disqualify is selected
                if (GetActiveBoardMedicalFinding() == DISQUALIFY_FINDING_VALUE)
                {
                    IList<Memorandum2> memolist2 = (from m in memoDao.GetByRefnModule(Id, (int)ModuleType.SpecCaseWWD)
                                                    where m.Deleted == false && m.Template.Id == Convert.ToByte(MemoType.WWD_Unit_Disqualification_Letter)
                                                    select m).ToList<Memorandum2>();
                    if (memolist2.Count < 1)
                    {
                        canForward = false;
                        string memoName2 = (from m in memoDao.GetAllTemplates() where m.Id == Convert.ToByte(MemoType.WWD_Unit_Disqualification_Letter) select m.Title).Single();
                        AddValidationItem(new ValidationItem("Memos", "WWW Memo", memoName2 + "  Memo not found.", true));
                    }
                }
            }

            // SAF Disposition: sent back to Board Medical
            if (Status == (int)SpecCaseWWDWorkStatus.MedicalReviewSAF || Status == (int)SpecCaseWWDWorkStatus.MedicalReviewIPEB || Status == (int)SpecCaseWWDWorkStatus.MedicalReviewFPEB)
            {
                if (GetActiveMemoTemplateId() != (int)MemoType.WWD_4_2_RTD &&
                    GetActiveMemoTemplateId() != (int)MemoType.WWD_4_Non_ALC_C &&
                    GetActiveMemoTemplateId() != (int)MemoType.WWD_RTD_SAF &&
                    GetActiveMemoTemplateId() != (int)MemoType.WWD_RTD_SG)
                {
                    canForward = false;
                    AddValidationItem(new ValidationItem("Memos", "SAF RTD Memo", "Please select a Return to Duty memo.", true));
                }
            }

            // SAF Disposition: HQ Tech Final Review
            if (Status == (int)SpecCaseWWDWorkStatus.FinalReviewSAF || Status == (int)SpecCaseWWDWorkStatus.FinalReviewIPEB || Status == (int)SpecCaseWWDWorkStatus.FinalReviewFPEB)
            {
                IList<Memorandum2> memolist = (from m in memoDao.GetByRefnModule(Id, (int)ModuleType.SpecCaseWWD)
                                               where m.Deleted == false && (m.Template.Id == (int)MemoType.WWD_4_2_RTD ||
                                                                            m.Template.Id == (int)MemoType.WWD_4_Non_ALC_C ||
                                                                            m.Template.Id == (int)MemoType.WWD_RTD_SAF ||
                                                                            m.Template.Id == (int)MemoType.WWD_RTD_SG)
                                               select m).ToList<Memorandum2>();

                if (memolist.Count < 1 && GetActiveMemoTemplateId().HasValue)
                {
                    canForward = false;
                    string memoName = (from m in memoDao.GetAllTemplates() where m.Id == Convert.ToByte(GetActiveMemoTemplateId()) select m.Title).Single();
                    AddValidationItem(new ValidationItem("Memos", "Memo", memoName + "  Memo  not found.", true));
                }
            }

            //// If Admin LOD and sent back to HQ Tech or Disqualified
            if ((Status == (int)SpecCaseWWDWorkStatus.MedicalReview) || (Status == (int)SpecCaseWWDWorkStatus.FinalReview))
            {
                if (GetActiveBoardMedicalFinding() == ADMINLOD_FINDING_VALUE)
                {
                    if (GetActiveMemoTemplateId() != (int)MemoType.WWD_Admin_LOD_AFPC)
                    {
                        canForward = false;
                        AddValidationItem(new ValidationItem("Memos", "Admin LOD Memo", "Please select WWD Admin LOD AFPC Letter.", true));
                    }
                }

                if (GetActiveBoardMedicalFinding() == DISQUALIFY_FINDING_VALUE)
                {
                    if (GetActiveMemoTemplateId() != (int)MemoType.WWD_Disqualification_Letter)
                    {
                        canForward = false;
                        AddValidationItem(new ValidationItem("Memos", "Disqal Memo", "Please select the WWD Disqualification Letter."));
                    }
                }
            }

            return canForward;
        }
    }
}