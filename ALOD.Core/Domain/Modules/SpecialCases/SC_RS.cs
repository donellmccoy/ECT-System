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
    public class SC_RS : SpecialCase
    {
        public SC_RS()
        {
        }

        public virtual int? ALCLetterType { get; set; }

        public virtual int? AlternateALCLetterType { get; set; }

        public virtual int? AlternateAlternateALCLetterType { get; set; }

        public virtual DateTime? AlternateExpirationDate { get; set; }

        // Start Page Properties
        public virtual int? CaseType { get; set; }

        public virtual string CaseTypeName { get; set; }
        public virtual int? CertificationStamp { get; set; }
        public virtual int? CompletedByUnit { get; set; }
        public virtual string CompletedByUnitName { get; set; }
        public virtual DateTime? DateReceived { get; set; }
        public virtual int? Disposition { get; set; }

        /// <inheritdoc/>
        public override int DocumentViewId
        {
            get { return (int)DocumentViewType.RS; }
        }

        public virtual string FreeText { get; set; }
        public virtual string ICD7thCharacter { get; set; }

        // HQ AFRC Tech tab properties
        public virtual int? ICD9Code { get; set; }

        public virtual string ICD9Description { get; set; }
        public virtual string ICD9Diagnosis { get; set; }
        public virtual int? MedGroupName { get; set; }
        public virtual int? MemoTemplateID { get; set; }
        public virtual string PocEmail { get; set; }
        public virtual string PocPhoneDSN { get; set; }
        public virtual string PocRankAndName { get; set; }

        // Board Med Tab properties
        public virtual int? PreviousDisposition { get; set; }

        public virtual int? Rating { get; set; }
        public virtual string RatingName { get; set; }
        public virtual int? RMUName { get; set; }
        public virtual int? SecondaryCertificationStamp { get; set; }
        public virtual string SecondaryFreeText { get; set; }

        // Document tab properties
        public virtual long? StampedDocId { get; set; }

        public virtual int? SubCaseType { get; set; }
        public virtual string SubCaseTypeName { get; set; }
        public virtual string TypeName { get; set; }

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

            if (ALCLetterType.HasValue)
                return ALCLetterType.Value;

            return 0; // No Limitations
        }

        public virtual int GetActiveDispositionValue(ILookupDispositionDao dispositionDao)
        {
            if (GetActiveBoardMedicalFinding().HasValue)
                return GetActiveBoardMedicalFinding().Value;

            if (Disposition.HasValue)
                return NormalizeHQTechDisposition(dispositionDao);

            return -1;
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
            if (CurrentStatusCode == (int)SpecCaseRSStatusCode.Qualified || CurrentStatusCode == (int)SpecCaseRSStatusCode.Disqualified)
            {
                access = PageAccessType.ReadOnly;
            }

            //'Add all pages as readonly
            scAccessList.Add(RSSectionNames.RS_RLB.ToString(), access);
            scAccessList.Add(RSSectionNames.RS_HQT_INIT.ToString(), access);
            scAccessList.Add(RSSectionNames.RS_DISQUALIFIED.ToString(), access);
            scAccessList.Add(RSSectionNames.RS_QUALIFIED.ToString(), access);
            scAccessList.Add(RSSectionNames.RS_BOARD_SG_REV.ToString(), access);
            scAccessList.Add(RSSectionNames.RS_SENIOR_MED_REV.ToString(), access);
            scAccessList.Add(RSSectionNames.RS_HQT_FINAL_REV.ToString(), access);

            //Modify access with user role
            switch (role)
            {
                case (int)UserGroups.AFRCHQTechnician:
                    if (CurrentStatusCode == (int)SpecCaseRSStatusCode.InitiateCase)
                    {
                        scAccessList[RSSectionNames.RS_HQT_INIT.ToString()] = PageAccessType.ReadWrite;
                    }

                    if (CurrentStatusCode == (int)SpecCaseRSStatusCode.FinalReview)
                    {
                        scAccessList[RSSectionNames.RS_HQT_FINAL_REV.ToString()] = PageAccessType.ReadWrite;
                    }

                    if (CurrentStatusCode == (int)SpecCaseRSStatusCode.RecruiterComments)
                    {
                        scAccessList[RSSectionNames.RS_RLB.ToString()] = PageAccessType.ReadWrite;
                    }

                    break;

                case (int)UserGroups.BoardMedical:
                    if (CurrentStatusCode == (int)SpecCaseRSStatusCode.MedicalReview)
                    {
                        scAccessList[RSSectionNames.RS_BOARD_SG_REV.ToString()] = PageAccessType.ReadWrite;
                    }
                    break;

                case (int)UserGroups.SeniorMedicalReviewer:
                    if (CurrentStatusCode == (int)SpecCaseRSStatusCode.SeniorMedicalReview)
                    {
                        scAccessList[RSSectionNames.RS_SENIOR_MED_REV.ToString()] = PageAccessType.ReadWrite;
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
                                IList<Memorandum2> memolist = (from m in memoDao.GetByRefnModule(Id, (int)ModuleType.SpecCaseRS)
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

        /// <inheritdoc/>
        protected override bool IsDocumentCategoryStillRequired(bool docFound, string docName, IDaoFactory daoFactory)
        {
            if (docName == DocumentType.Form2808.ToString())
            {
                ILookupDao lookupDao = daoFactory.GetLookupDao();

                if (!IsPalaceChaseFrontCaseType(lookupDao) && !IsMEPSRequestCaseType(lookupDao))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        protected bool IsMEPSRequestCaseType(ILookupDao lookupDao)
        {
            if (!CaseType.HasValue || CaseType.Value <= 0)
                return false;

            return lookupDao.GetCaseTypeName(CaseType.Value).Equals(ALOD.Core.Domain.Lookup.CaseType.MEPSRequestName);
        }

        protected bool IsPalaceChaseFrontCaseType(ILookupDao lookupDao)
        {
            if (!CaseType.HasValue || CaseType.Value <= 0)
                return false;

            return (lookupDao.GetCaseTypeName(CaseType.Value).Equals(ALOD.Core.Domain.Lookup.CaseType.PalaceChaseName) || lookupDao.GetCaseTypeName(CaseType.Value).Equals(ALOD.Core.Domain.Lookup.CaseType.PalaceFrontName));
        }

        private int? GetActiveBoardMedicalALC()
        {
            if (ShouldUseSeniorMedicalReviewerFindings())
                return AlternateAlternateALCLetterType;

            return AlternateALCLetterType;
        }

        private bool ShouldValidateMemos(ILookupDao lookupDao)
        {
            if (CaseType.HasValue && CaseType.Value > 0 && (lookupDao.GetCaseTypeName(CaseType.Value).Equals(ALOD.Core.Domain.Lookup.CaseType.PalaceChaseName) || lookupDao.GetCaseTypeName(CaseType.Value).Equals(ALOD.Core.Domain.Lookup.CaseType.PalaceFrontName)))
                return true;

            return false;
        }

        private bool ShouldValidatePalaceChaseMemos()
        {
            if (MemoTemplateID.HasValue && MemoTemplateID.Value > 0)
                return false;

            return true;
        }

        private bool ValidateCanForwardCaseRule(ILookupDao lookupDao)
        {
            bool canForward = true;

            if (CurrentStatusCode == (int)SpecCaseRSStatusCode.InitiateCase ||
                CurrentStatusCode == (int)SpecCaseRSStatusCode.FinalReview ||
                CurrentStatusCode == (int)SpecCaseRSStatusCode.RecruiterComments)
            {
                canForward = ValidateCanForwardCaseRuleForHQAFRCTechnician(lookupDao);
            }
            else if (CurrentStatusCode == (int)SpecCaseRSStatusCode.MedicalReview)
            {
                canForward = ValidateCanForwardCaseRuleForBoardMedicalOfficer();
            }
            else if (CurrentStatusCode == (int)SpecCaseRSStatusCode.SeniorMedicalReview)
            {
                canForward = ValidateCanForwardCaseRuleForSeniorMedicalReviewer();
            }

            return canForward;
        }

        private bool ValidateCanForwardCaseRuleForBoardMedicalOfficer()
        {
            bool canForward = true;

            if (!med_off_approved.HasValue)
            {
                canForward = false;
                AddValidationItem(new ValidationItem("Board Medical", "Determination", "Must provide a Determination for the RS case."));
            }

            if (String.IsNullOrEmpty(med_off_approval_comment))
            {
                canForward = false;
                AddValidationItem(new ValidationItem("Board Medical", "Decision Explanation", "Must provide a Decision Explanation for the RS case."));
            }

            return canForward;
        }

        private bool ValidateCanForwardCaseRuleForHQAFRCTechnician(ILookupDao lookupDao)
        {
            bool canForward = true;

            string user = string.Empty;
            user = "HQ AFRC Tech";

            if (CaseType.HasValue == false || CaseType.Value == 0)
            {
                canForward = false;
                AddValidationItem(new ValidationItem(user, "Case Type", "Must select a Specialty Case Type for the RS case."));
            }
            else
            {
                if (lookupDao.GetCaseTypeName(CaseType.Value).ToLower().Equals("other") && string.IsNullOrEmpty(CaseTypeName))
                {
                    canForward = false;
                    AddValidationItem(new ValidationItem(user, "Case Type Name", "Must enter the Specialty Case Type Name for the RS case."));
                }

                if (lookupDao.GetHasSubCaseTypes(CaseType.Value))
                {
                    if (SubCaseType.HasValue == false || SubCaseType.Value == 0)
                    {
                        canForward = false;
                        AddValidationItem(new ValidationItem(user, "Sub Case Type", "Must select a Case Type for the RS case."));
                    }
                    else
                    {
                        if (lookupDao.GetSubCaseTypeName(SubCaseType.Value).ToLower().Equals("other") && string.IsNullOrEmpty(SubCaseTypeName))
                        {
                            canForward = false;
                            AddValidationItem(new ValidationItem(user, "Sub Case Type Name", "Must enter the Case Type Name for the RS case."));
                        }
                    }
                }
            }

            IList<LookUpItem> types = lookupDao.GetSpecialCasePEPPTypes(this.Id);

            if (types == null || types.Count == 0)
            {
                canForward = false;
                AddValidationItem(new ValidationItem(user, "Type", "Must select a Type for the RS case."));
            }
            else
            {
                if (types.Where(x => x.Name.ToLower().Equals("other")).ToList().Count != 0 && (TypeName == null || TypeName == String.Empty))
                {
                    canForward = false;
                    AddValidationItem(new ValidationItem(user, "Type Name", "Must enter the Type Name for the RS case."));
                }
            }

            if ((Rating == null) || (Rating == 0))
            {
                canForward = false;
                AddValidationItem(new ValidationItem(user, "Rating", "Must enter the Rating for the RS case."));
            }
            else
            {
                DataSet ds = lookupDao.GetPEPPRatings((int)Rating, 0);
                string ratingName = ds.Tables[0].Rows[0]["Name"].ToString();

                // If the "Other" rating is selected, then make sure the user actually entered a rating name in the provided textbox.
                if (ratingName.ToLower().Equals("other") == true && (RatingName == null || RatingName == String.Empty))
                {
                    canForward = false;
                    AddValidationItem(new ValidationItem(user, "Rating Name", "Must enter the Rating Name for the RS case."));
                }
            }

            if ((Disposition == null) || (Disposition == 0))
            {
                canForward = false;
                AddValidationItem(new ValidationItem(user, "Disposition", "Must select a Disposition for the RS case."));
            }

            if ((CompletedByUnit == null) || (CompletedByUnit == 0))
            {
                canForward = false;
                AddValidationItem(new ValidationItem(user, "Completed By", "Must enter the Completed By for the RS case."));
            }
            else
            {
                string cbgName = lookupDao.GetCompletedByGroupName(CompletedByUnit.Value);

                if (!string.IsNullOrEmpty(cbgName) && cbgName.ToLower().Equals("other") && CompletedByUnitName == string.Empty)
                {
                    canForward = false;
                    AddValidationItem(new ValidationItem(user, "Completed By Name", "Must enter the Completed By Name for the RS case."));
                }
            }

            if (DateReceived == null)
            {
                canForward = false;
                AddValidationItem(new ValidationItem(user, "Date Received", "Must enter the Date Received for the RS case."));
            }

            if (CertificationStamp == null || CertificationStamp == 0)
            {
                if (CaseType.HasValue && CaseType.Value > 0 && !(lookupDao.GetCaseTypeName(CaseType.Value).Equals(ALOD.Core.Domain.Lookup.CaseType.PalaceChaseName) || lookupDao.GetCaseTypeName(CaseType.Value).Equals(ALOD.Core.Domain.Lookup.CaseType.PalaceFrontName)) && !lookupDao.GetCaseTypeName(CaseType.Value).Equals(ALOD.Core.Domain.Lookup.CaseType.MEPSRequestName))
                {
                    canForward = false;
                    AddValidationItem(new ValidationItem(user, "Certification Stamp", "Must select a Certification Stamp for the RS case."));
                }
            }

            if ((ICD9Code == null) || (ICD9Code == 0))
            {
                canForward = false;
                AddValidationItem(new ValidationItem(user, "ICD9", "Must select a Diagnosis (ICD) for the RS case."));
            }

            if ((ICD9Diagnosis == null || ICD9Diagnosis == ""))
            {
                canForward = false;
                AddValidationItem(new ValidationItem(user, "ICD9 Description", "Must enter a Diagnosis Description for the RS case."));
            }

            if ((StampedDocId == null || StampedDocId == 0) && WorkflowStatus.Id == (int)SpecCaseRSWorkStatus.FinalReview)
            {
                if (CaseType.HasValue && CaseType.Value > 0 && !(lookupDao.GetCaseTypeName(CaseType.Value).Equals(ALOD.Core.Domain.Lookup.CaseType.PalaceChaseName) || lookupDao.GetCaseTypeName(CaseType.Value).Equals(ALOD.Core.Domain.Lookup.CaseType.PalaceFrontName)) && !lookupDao.GetCaseTypeName(CaseType.Value).Equals(ALOD.Core.Domain.Lookup.CaseType.MEPSRequestName))
                {
                    canForward = false;
                    AddValidationItem(new ValidationItem(user, "Stamped Doc", "Must upload a DD Form 2808 document to be stamped."));
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
                AddValidationItem(new ValidationItem("Senior Medical Reviewer", "Decision", "Must provide a Concur/Non-Concur Decision for the RS case."));
            }
            else
            {
                if (SeniorMedicalReviewerConcur.Equals(DECISION_NONCONCUR) && !SeniorMedicalReviewerApproved.HasValue)
                {
                    canForward = false;
                    AddValidationItem(new ValidationItem("Senior Medical Reviewer", "Determination", "Must provide a Decision for the RS case."));
                }
            }

            if (string.IsNullOrEmpty(SeniorMedicalReviewerComment))
            {
                AddValidationItem(new ValidationItem("Senior Medical Reviewer", "Decision Explanation", "Must provide a Decision Explanation for the RS case."));
                canForward = false;
            }

            return canForward;
        }

        private bool ValidateQualifyMemosRule(IMemoDao2 memoDao, ILookupDao lookupDao)
        {
            if (!ShouldValidateMemos(lookupDao))
                return true;

            bool canForward = true;
            string memoTemplateName = string.Empty;
            string preText = string.Empty;
            IList<Memorandum2> rsMemos = null;

            if (lookupDao.GetIsReassessmentSpecialCase(this.Id))
                preText = "A new ";

            if (!MemoTemplateID.HasValue || MemoTemplateID.Value == 0)
            {
                AddValidationItem(new ValidationItem("Memos", "RS Memo", "Must select " + preText.ToLower() + ALOD.Core.Domain.Lookup.CaseType.PalaceChaseFrontName + " memo(s) to generate.", true));
                canForward = false;
            }
            else
            {
                if (ShouldValidatePalaceChaseMemos())
                {
                    rsMemos = (from m in memoDao.GetByRefnModule(Id, (int)ModuleType.SpecCaseRS)
                               where m.Deleted == false && (m.Template.Id == Convert.ToByte(MemoType.PalaceChaseAFI) || m.Template.Id == Convert.ToByte(MemoType.PalaceChaseMSD)) && m.CreatedDate >= CreatedDate
                               select m).ToList<Memorandum2>();

                    if (rsMemos == null || rsMemos.Count < 2)
                    {
                        List<int> templateIds = new List<int>();

                        if (rsMemos != null)
                        {
                            foreach (Memorandum2 m in rsMemos)
                            {
                                templateIds.Add(m.Template.Id);
                            }
                        }

                        if (!templateIds.Contains(Convert.ToByte(MemoType.PalaceChaseMSD)))
                        {
                            memoTemplateName = (from m in memoDao.GetAllTemplates() where m.Id == Convert.ToByte(MemoType.PalaceChaseMSD) select m.Title).Single();
                            AddValidationItem(new ValidationItem("Memos", "RS MSD Memo", preText + memoTemplateName + " Memo not found.", true));
                        }

                        if (!templateIds.Contains(Convert.ToByte(MemoType.PalaceChaseAFI)))
                        {
                            memoTemplateName = (from m in memoDao.GetAllTemplates() where m.Id == Convert.ToByte(MemoType.PalaceChaseAFI) select m.Title).Single();
                            AddValidationItem(new ValidationItem("Memos", "RS AFI Memo", preText + memoTemplateName + " Memo not found.", true));
                        }

                        canForward = false;
                    }
                }
                else
                {
                    rsMemos = (from m in memoDao.GetByRefnModule(Id, (int)ModuleType.SpecCaseRS)
                               where m.Deleted == false && m.Template.Id == Convert.ToByte(MemoTemplateID) && m.CreatedDate >= CreatedDate
                               select m).ToList<Memorandum2>();

                    if (rsMemos == null || rsMemos.Count == 0)
                    {
                        memoTemplateName = (from m in memoDao.GetAllTemplates() where m.Id == Convert.ToByte(MemoTemplateID) select m.Title).Single();
                        AddValidationItem(new ValidationItem("Memos", "RS Memo", preText + memoTemplateName + " Memo not found.", true));
                        canForward = false;
                    }
                }
            }

            return canForward;
        }
    }
}