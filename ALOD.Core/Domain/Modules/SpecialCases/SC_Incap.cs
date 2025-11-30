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
    public class SC_Incap : SpecialCase
    {
        public SC_Incap()
        {
        }

        //public virtual SC_Incap_Findings INCAPResults { get; set; }
        /// <inheritdoc/>
        public override int DocumentViewId
        {
            get { return (int)DocumentViewType.Incap; }
        }

        public virtual IList<SC_IncapAppeal_Findings> INCAPAppealFindings { get; set; }
        public virtual IList<SC_IncapExt_Findings> INCAPExtFindings { get; set; }
        public virtual IList<SC_Incap_Findings> INCAPFindings { get; set; }
        public virtual DateTime? SuspenseDate { get; set; }
        public virtual string TMTNumber { get; set; }

        public virtual DateTime? TMTReceiveDate { get; set; }

        /// <inheritdoc/>
        public override Dictionary<String, PageAccessType> ReadSectionList(int role)
        {
            PageAccessType access;
            access = PageAccessType.None;

            Dictionary<String, PageAccessType> scAccessList = new Dictionary<String, PageAccessType>();
            if ((CurrentStatusCode == (int)SpecCaseIncapStatusCode.IN_Complete))
            {
                access = PageAccessType.ReadOnly;
            }

            //'Add all pages as readonly

            scAccessList.Add(INSectionNames.IN_HQT_INIT.ToString(), access);
            scAccessList.Add(INSectionNames.IN_BOARD_SG_REV.ToString(), access);
            scAccessList.Add(INSectionNames.IN_HQT_FINAL_REV.ToString(), access);
            scAccessList.Add(INSectionNames.IN_SENIOR_MED_REV.ToString(), access);
            scAccessList.Add(INSectionNames.IN_COMPLETE.ToString(), access);
            scAccessList.Add(INSectionNames.IN_RLB.ToString(), access);

            //Modify access with user role
            switch (role)
            {
                case (int)UserGroups.AFRCHQTechnician:
                    if (CurrentStatusCode == (int)SpecCaseIncapStatusCode.IN_Initiate)
                    {
                        scAccessList[INSectionNames.IN_HQT_INIT.ToString()] = PageAccessType.ReadWrite;
                    }
                    if (CurrentStatusCode == (int)SpecCaseIncapStatusCode.IN_WingCCAction)
                    {
                        scAccessList[INSectionNames.IN_HQT_FINAL_REV.ToString()] = PageAccessType.ReadWrite;
                    }

                    break;

                case (int)UserGroups.BoardMedical:
                    if (CurrentStatusCode == (int)SpecCaseIncapStatusCode.IN_MedicalReview_WG)
                    {
                        scAccessList[INSectionNames.IN_BOARD_SG_REV.ToString()] = PageAccessType.ReadWrite;
                    }
                    break;

                //case (int)UserGroups.SeniorMedicalReviewer:
                //    if (CurrentStatusCode == (int)SpecCaseIncapStatusCode.SeniorMedicalReview)
                //    {
                //        scAccessList[INSectionNames.IN_SENIOR_MED_REV.ToString()] = PageAccessType.ReadWrite;
                //    }
                //    break;
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

                    case "cancompleteasapproved":
                        o.OptionVisible = DetermineVisibilityForCanCompleteCaseRule(1);
                        break;

                    case "cancompleteasdenied":
                        o.OptionVisible = DetermineVisibilityForCanCompleteCaseRule(0);
                        break;

                    case "usergroupcontainsdecision":
                        o.OptionVisible = DetermineVisibilityForUserGroupContainsDecisionRule(new UserGroupContainsDecisionRuleData(r));
                        break;

                    case "in_approvalvisiblity":
                        o.OptionVisible = IncapVisibiltyByWorkflowType(o.DisplayText);
                        break;
                }
            }

            if (r.RuleTypes.ruleTypeId == (int)RuleKind.Validation)
            {
                switch (r.RuleTypes.Name.ToLower())
                {
                    case "canforwardcase":
                        bool canforward = ValidateCanForwardCaseRule(lookupDao);

                        if (canforward != true)
                        {
                            o.OptionValid = false;
                        }
                        else
                        {
                            o.OptionValid = true;
                        }

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
                                IList<Memorandum2> memolist = (from m in memoDao.GetByRefnModule(Id, (int)ModuleType.SpecCaseIncap) where m.Deleted == false && m.Template.Id == Convert.ToByte(memos[i]) select m).ToList<Memorandum2>();
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
                    //INCAP
                    case "in_inpm_validations":
                    case "in_medtech_validations":
                    case "in_unitcc_validations":
                    case "in_wingja_validations":
                    case "in_fmtech_validations":
                    case "in_wingcc_validations":
                    case "in_hropr_validations":
                    case "in_hrocr_validations":
                    case "in_afrdos_validations":
                    case "in_afrcc_validations":
                    case "in_afrvcr_validations":
                    case "in_afrdop_validations":
                    case "in_cafr_validations":
                        o.OptionValid = IncapValidationsByWorkflowType(r.RuleTypes.Name.ToLower());
                        if (Validations.Count > 0) //taking all validations including Incap documents into consideration
                        {
                            o.OptionValid = false;
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
                AddValidationItem(new ValidationItem("Board Medical Officer", "Decision", "Board Medical Officer must provide a finding in order to close out the IN case."));
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
            if (CurrentStatusCode == (int)SpecCaseIncapStatusCode.IN_Initiate)
                return ValidateCanForwardCaseRuleForHqAfrcTechnician();

            if (CurrentStatusCode == (int)SpecCaseIncapStatusCode.IN_MedicalReview_WG)
                return ValidateCanForwardCaseRuleForBoardMedicalOfficer();

            //if (CurrentStatusCode == (int)SpecCaseIncapStatusCode.SeniorMedicalReview)
            //    return ValidateCanForwardCaseRuleForSeniorMedicalReviewer();

            return true;
        }

        private bool AppealDataValidation(SC_IncapAppeal_Findings fnd, SC_Incap_Findings init_fnd, string validation)
        {
            bool valid = true;
            //Wing
            if (validation.Equals("in_inpm_validations"))
            {
                if (!init_fnd.Init_AppealOrComplete.HasValue)
                {
                    AddValidationItem(new ValidationItem("IN Wing Int", "Initial", "Must appeal or complete case"));
                    valid = false;
                }
            }
            if (validation.Equals("in_wingcc_validations"))
            {
                if (!fnd.WCC_AppealApproval.HasValue)
                {
                    AddValidationItem(new ValidationItem("IN Wing Int", "Initial", "Must provide recommendation for appeal"));
                    valid = false;
                }
            }
            //HQ
            if (validation.Equals("in_hropr_validations"))
            {
                if (!fnd.OPR_AppealApproval.HasValue)
                {
                    AddValidationItem(new ValidationItem("IN HQ EXT/ App", "Initial", "Must provide recommendation for appeal"));
                    valid = false;
                }
            }
            if (validation.Equals("in_hrocr_validations"))
            {
                if (fnd.OCR_AppealApproval == null)
                {
                    AddValidationItem(new ValidationItem("IN HQ EXT/ App", "Initial", "Must provide recommendation for appeal"));
                    valid = false;
                }
            }
            if (validation.Equals("in_afrdos_validations"))
            {
                if (!fnd.DOS_AppealApproval.HasValue)
                {
                    AddValidationItem(new ValidationItem("IN HQ EXT/ App", "Initial", "Must provide Immediate Commander Recommendation"));
                    valid = false;
                }
            }
            if (validation.Equals("in_afrcc_validations"))
            {
                if (!fnd.CCR_AppealApproval.HasValue)
                {
                    AddValidationItem(new ValidationItem("IN HQ EXT/ App", "Initial", "Must provide recommendation for appeal"));
                    valid = false;
                }
            }
            if (validation.Equals("in_afrvcr_validations"))
            {
                if (!fnd.VCR_AppealApproval.HasValue)
                {
                    AddValidationItem(new ValidationItem("IN HQ EXT/ App", "Initial", "Must provide recommendation for appeal"));
                    valid = false;
                }
            }
            if (validation.Equals("in_afrdop_validations"))
            {
                if (!fnd.DOP_AppealApproval.HasValue)
                {
                    AddValidationItem(new ValidationItem("IN HQ EXT/ App", "Initial", "Must provide recommendation for appeal"));
                    valid = false;
                }
            }
            if (validation.Equals("in_cafr_validations"))
            {
                if (!fnd.CAFR_AppealApproval.HasValue)
                {
                    AddValidationItem(new ValidationItem("IN HQ EXT/ App", "Initial", "Must provide appeal approval answer"));
                    valid = false;
                }
            }
            //if (validation.Equals("in_approvalvisiblity"))
            //{
            //    if (!fnd.WCC_ExtApproval.HasValue)
            //    {
            //        AddValidationItem(new ValidationItem("IN Wing Int", "Initial", "Must approve or disapprove INCAP"));
            //        valid = false;
            //    }
            //}

            return valid;
        }

        private bool ExtDataValidation(SC_IncapExt_Findings fnd, string validation)
        {
            bool valid = true;
            //Wing
            if (validation.Equals("in_inpm_validations"))
            {
                if (!fnd.EXT_StartDate.HasValue)
                {
                    AddValidationItem(new ValidationItem("IN Wing Int", "Initial", "Must provide a extension start date"));
                    valid = false;
                }
                if (!fnd.EXT_EndDate.HasValue)
                {
                    AddValidationItem(new ValidationItem("IN Wing Int", "Initial", "Must provide a extension end date"));
                    valid = false;
                }
            }
            if (validation.Equals("in_medtech_validations"))
            {
                if (fnd.MED_ExtRecommendation == null)
                {
                    AddValidationItem(new ValidationItem("IN Wing Int", "Initial", "Must provide Med Tech Recommendation"));
                    valid = false;
                }
                if (!fnd.MED_AMROStartDate.HasValue)
                {
                    AddValidationItem(new ValidationItem("IN Wing Int", "Initial", "Must provide a AMRO start date"));
                    valid = false;
                }
                if (!fnd.MED_AMROEndDate.HasValue)
                {
                    AddValidationItem(new ValidationItem("IN Wing Int", "Initial", "Must provide a AMRO end date"));
                    valid = false;
                }
                if (!fnd.MED_NextAMROStartDate.HasValue)
                {
                    AddValidationItem(new ValidationItem("IN Wing Int", "Initial", "Must provide a next AMRO start date"));
                    valid = false;
                }
                if (!fnd.MED_NextAMROEndDate.HasValue)
                {
                    AddValidationItem(new ValidationItem("IN Wing Int", "Initial", "Must provide a next AMRO end date"));
                    valid = false;
                }
                if (fnd.med_AMRODisposition.Equals(""))
                {
                    AddValidationItem(new ValidationItem("IN Wing Int", "Initial", "Must provide a next AMRO disposition"));
                    valid = false;
                }
                if (fnd.MED_IRILOStatus.Equals(""))
                {
                    AddValidationItem(new ValidationItem("IN Wing Int", "Initial", "Must provide a IRILO status"));
                    valid = false;
                }
            }
            if (validation.Equals("in_unitcc_validations"))
            {
                if (!fnd.IC_ExtRecommendation.HasValue)
                {
                    AddValidationItem(new ValidationItem("IN Wing Int", "Initial", "Must provide Immediate Commander Recommendation"));
                    valid = false;
                }
            }
            if (validation.Equals("in_wingja_validations"))
            {
                if (!fnd.WJA_ConcurWithIC.HasValue)
                {
                    AddValidationItem(new ValidationItem("IN Wing Int", "Initial", "Must concur or nonconcur with Immediate Commander"));
                    valid = false;
                }
            }
            if (validation.Equals("in_fmtech_validations"))
            {
                if (!fnd.FIN_ExtIncomeLost.HasValue)
                {
                    AddValidationItem(new ValidationItem("IN Wing Int", "Initial", "Must provide loss of earned income answer for extension"));
                    valid = false;
                }
            }
            if (validation.Equals("in_wingcc_validations"))
            {
                if (!fnd.WCC_ExtApproval.HasValue)
                {
                    AddValidationItem(new ValidationItem("IN Wing Int", "Initial", "Must approve or disapprove extension"));
                    valid = false;
                }
            }
            //HQ
            if (validation.Equals("in_hropr_validations"))
            {
                if (!fnd.OPR_ExtApproval.HasValue)
                {
                    AddValidationItem(new ValidationItem("IN HQ EXT/ App", "Initial", "Must provide recommendation for extension"));
                    valid = false;
                }
            }
            if (validation.Equals("in_hrocr_validations"))
            {
                if (fnd.OCR_ExtApproval == null)
                {
                    AddValidationItem(new ValidationItem("IN HQ EXT/ App", "Initial", "Must provide recommendation for extension"));
                    valid = false;
                }
            }
            if (validation.Equals("in_afrdos_validations"))
            {
                if (!fnd.DOS_ExtApproval.HasValue)
                {
                    AddValidationItem(new ValidationItem("IN HQ EXT/ App", "Initial", "Must provide Immediate Commander Recommendation"));
                    valid = false;
                }
            }
            if (validation.Equals("in_afrcc_validations"))
            {
                if (!fnd.CCR_ExtApproval.HasValue)
                {
                    AddValidationItem(new ValidationItem("IN HQ EXT/ App", "Initial", "Must provide recommendation for extension"));
                    valid = false;
                }
            }
            if (validation.Equals("in_afrvcr_validations"))
            {
                if (!fnd.VCR_ExtApproval.HasValue)
                {
                    AddValidationItem(new ValidationItem("IN HQ EXT/ App", "Initial", "Must provide recommendation for extension"));
                    valid = false;
                }
            }
            if (validation.Equals("in_afrdop_validations"))
            {
                if (!fnd.DOP_ExtApproval.HasValue)
                {
                    AddValidationItem(new ValidationItem("IN HQ EXT/ App", "Initial", "Must provide recommendation for extension"));
                    valid = false;
                }
            }
            if (validation.Equals("in_cafr_validations"))
            {
                if (!fnd.CAFR_ExtApproval.HasValue)
                {
                    AddValidationItem(new ValidationItem("IN HQ EXT/ App", "Initial", "Must provide extension approval answer"));
                    valid = false;
                }
            }
            //if (validation.Equals("in_approvalvisiblity"))
            //{
            //    if (!fnd.WCC_ExtApproval.HasValue)
            //    {
            //        AddValidationItem(new ValidationItem("IN Wing Int", "Initial", "Must approve or disapprove INCAP"));
            //        valid = false;
            //    }
            //}
            return valid;
        }

        private string GetIncapWorkflowType(int statusId)
        {
            string type = "";
            switch (statusId)
            {
                case (int)SpecCaseIncapStatusCode.IN_Approved:
                case (int)SpecCaseIncapStatusCode.IN_Initiate:
                case (int)SpecCaseIncapStatusCode.IN_MedicalReview_WG:
                case (int)SpecCaseIncapStatusCode.IN_ImmediateCommanderReview:
                case (int)SpecCaseIncapStatusCode.IN_WingJAReview:
                case (int)SpecCaseIncapStatusCode.IN_FinanceReview:
                case (int)SpecCaseIncapStatusCode.IN_WingCCAction:
                    type = "Initiate";
                    break;

                case (int)SpecCaseIncapStatusCode.IN_Extension:
                case (int)SpecCaseIncapStatusCode.IN_MedicalReview_WG_Extension:
                case (int)SpecCaseIncapStatusCode.IN_ImmediateCommanderReview_Extension:
                case (int)SpecCaseIncapStatusCode.IN_WingJAReview_Extension:
                case (int)SpecCaseIncapStatusCode.IN_FinanceReview_Extension:
                case (int)SpecCaseIncapStatusCode.IN_WingCommanderRecommendation_Extension:
                case (int)SpecCaseIncapStatusCode.IN_OCRExtension_HRReview:
                case (int)SpecCaseIncapStatusCode.IN_OPRExtension_HRReview:
                case (int)SpecCaseIncapStatusCode.IN_DirectorOfStaffReview:
                case (int)SpecCaseIncapStatusCode.IN_DirectorOfPersonnelReview:
                case (int)SpecCaseIncapStatusCode.IN_CommandChiefReview:
                case (int)SpecCaseIncapStatusCode.IN_ViceCommanderReview:
                case (int)SpecCaseIncapStatusCode.IN_CAFRAction:
                    type = "Ext";
                    break;

                case (int)SpecCaseIncapStatusCode.IN_WingCommanderRecommendation_Appeal:
                case (int)SpecCaseIncapStatusCode.IN_Appeal:
                case (int)SpecCaseIncapStatusCode.IN_OPRAppeal_HRReview:
                case (int)SpecCaseIncapStatusCode.IN_OCRAppeal_HRReview:
                case (int)SpecCaseIncapStatusCode.IN_ViceCommanderReview_Appeal:
                case (int)SpecCaseIncapStatusCode.IN_DirectorOfPersonnelReview_Appeal:
                case (int)SpecCaseIncapStatusCode.IN_DirectorOfStaffReview_Appeal:
                case (int)SpecCaseIncapStatusCode.IN_CommandChiefReview_Appeal:
                case (int)SpecCaseIncapStatusCode.IN_CAFRAction_Appeal:
                    type = "Appeal";
                    break;
            }
            return type;
        }

        private bool IncapValidationsByWorkflowType(string validation)
        {
            bool valid = false;
            SC_Incap_Findings incap_Findings = INCAPFindings.First();
            string workflowType = GetIncapWorkflowType(CurrentStatusCode);

            switch (workflowType)
            {
                case "Initiate":
                    valid = InitialDataValidation(incap_Findings, validation);
                    break;

                case "Ext":
                    SC_IncapExt_Findings ext_Findings = INCAPExtFindings.Last();
                    valid = ExtDataValidation(ext_Findings, validation);
                    break;

                case "Appeal":
                    SC_IncapAppeal_Findings appeal_Findings = INCAPAppealFindings.Last();
                    valid = AppealDataValidation(appeal_Findings, incap_Findings, validation);
                    break;
            }
            return valid;
        }

        private bool IncapVisibiltyByWorkflowType(string option)
        {
            bool valid = true;
            SC_Incap_Findings incap_Findings = INCAPFindings.First();
            string workflowType = GetIncapWorkflowType(CurrentStatusCode);

            switch (workflowType)
            {
                case "Initiate":
                    if (incap_Findings.WCC_InitApproval == false && option.Contains("Approved"))
                    {
                        valid = false;
                    }
                    if (incap_Findings.WCC_InitApproval == true && option.Contains("Denied"))
                    {
                        valid = false;
                    }
                    break;

                case "Ext":
                    SC_IncapExt_Findings ext_Findings = INCAPExtFindings.Last();
                    if (ext_Findings.CAFR_ExtApproval == false && option.Contains("Approved"))
                    {
                        valid = false;
                    }
                    if (ext_Findings.CAFR_ExtApproval == true && option.Contains("Disapproved"))
                    {
                        valid = false;
                    }
                    break;

                case "Appeal":
                    SC_IncapAppeal_Findings appeal_Findings = INCAPAppealFindings.Last();
                    if (incap_Findings.CAFR_AppealApproval == false && option.Contains("Approved"))
                    {
                        valid = false;
                    }
                    if (incap_Findings.CAFR_AppealApproval == true && option.Contains("Disapproved"))
                    {
                        valid = false;
                    }
                    break;
            }
            return valid;
        }

        private bool InitialDataValidation(SC_Incap_Findings fnd, string validation)
        {
            bool valid = true;
            //INCAP PM
            if (validation.Equals("in_inpm_validations"))
            {
                if (!fnd.Init_StartDate.HasValue)
                {
                    AddValidationItem(new ValidationItem("IN Wing Int", "Initial", "Must provide a Initial start date"));
                    valid = false;
                }
                if (!fnd.Init_EndDate.HasValue)
                {
                    AddValidationItem(new ValidationItem("IN Wing Int", "Initial", "Must provide a Initial end date"));
                    valid = false;
                }
                if (!fnd.Init_ExtOrComplete.HasValue && CurrentStatusCode == (int)SpecCaseIncapStatusCode.IN_Approved)
                {
                    AddValidationItem(new ValidationItem("IN Wing Int", "Initial", "Must extend or complete case"));
                    valid = false;
                }
                if (!fnd.Init_ExtOrComplete.HasValue && CurrentStatusCode == (int)SpecCaseIncapStatusCode.IN_Approved)
                {
                    AddValidationItem(new ValidationItem("IN Wing Int", "Initial", "Must extend or complete case"));
                    valid = false;
                }
                if (!fnd.Init_LateSubmission.HasValue)
                {
                    AddValidationItem(new ValidationItem("IN Wing Int", "Initial", "Is this case a late submission"));
                    valid = false;
                }
            }
            if (validation.Equals("in_medtech_validations"))
            {
                if (fnd.Med_ReportType == null)
                {
                    AddValidationItem(new ValidationItem("IN Wing Int", "Initial", "Must provide duty limiting condition report type"));
                    valid = false;
                }
                if (!fnd.Med_AbilityToPreform.HasValue)
                {
                    AddValidationItem(new ValidationItem("IN Wing Int", "Initial", "Must provide the ability to preform duties answer"));
                    valid = false;
                }
            }
            if (validation.Equals("in_unitcc_validations"))
            {
                if (!fnd.IC_Recommendation.HasValue)
                {
                    AddValidationItem(new ValidationItem("IN Wing Int", "Initial", "Must provide Immediate Commander Recommendation"));
                    valid = false;
                }
            }
            if (validation.Equals("in_wingja_validations"))
            {
                if (!fnd.Wing_Ja_Concur.HasValue)
                {
                    AddValidationItem(new ValidationItem("IN Wing Int", "Initial", "Must concur or nonconcur with Immediate Commander"));
                    valid = false;
                }
            }
            if (validation.Equals("in_fmtech_validations"))
            {
                if (!fnd.Fin_IncomeLost.HasValue)
                {
                    AddValidationItem(new ValidationItem("IN Wing Int", "Initial", "Must provide loss of earned income answer"));
                    valid = false;
                }
                if (!fnd.Fin_SelfEmployed.HasValue)
                {
                    AddValidationItem(new ValidationItem("IN Wing Int", "Initial", "Is the member self employed"));
                    valid = false;
                }
            }
            if (validation.Equals("in_wingcc_validations"))
            {
                if (!fnd.WCC_InitApproval.HasValue)
                {
                    AddValidationItem(new ValidationItem("IN Wing Int", "Initial", "Must approve or disapprove INCAP"));
                    valid = false;
                }
            }
            if (validation.Equals("in_approvalvisiblity"))
            {
                if (fnd.WCC_InitApproval == false)
                {
                    valid = false;
                }
            }
            return valid;
        }

        private bool ValidateCanForwardCaseRuleForBoardMedicalOfficer()
        {
            bool canForward = true;

            if (!med_off_approved.HasValue)
            {
                canForward = false;
                AddValidationItem(new ValidationItem("Board Medical Officer", "Decision", "Must provide a Decision for the IN case."));
            }
            else if (med_off_approved.Value == 2)   // TODO: Determine if this check is necessary...at the moment there is no Decision option with a value of 2...might be here for legacy cases.
            {
                canForward = false;
                AddValidationItem(new ValidationItem("Board Medical Officer", "Decision", "Must select either the Approve or Deny Decision for the IN case."));
            }

            return canForward;
        }

        private bool ValidateCanForwardCaseRuleForHqAfrcTechnician()
        {
            bool canForward = true;

            //before task 168
            //if (string.IsNullOrEmpty(TMTNumber))
            //{
            //    canForward = false;
            //    AddValidationItem(new ValidationItem("HQ AFRC Technician", "TMT Number", "Must provide a TMT Number for the IN case."));
            //}

            //if (!TMTReceiveDate.HasValue)
            //{
            //    canForward = false;
            //    AddValidationItem(new ValidationItem("HQ AFRC Technician", "TMT Receive Date", "Must provide a TMT Receive Date for the IN case."));
            //}

            return canForward;
        }

        private bool ValidateCanForwardCaseRuleForSeniorMedicalReviewer()
        {
            bool canForward = true;

            if (string.IsNullOrEmpty(SeniorMedicalReviewerConcur))
            {
                canForward = false;
                AddValidationItem(new ValidationItem("Senior Medical Reviewer", "Decision", "Must provide a Concur/Non-Concur Decision for the IN case."));
            }
            else
            {
                if (SeniorMedicalReviewerConcur.Equals(DECISION_NONCONCUR) && !SeniorMedicalReviewerApproved.HasValue)
                {
                    canForward = false;
                    AddValidationItem(new ValidationItem("Senior Medical Reviewer", "Determination", "Must provide a Decision for the IN case."));
                }
            }

            if (string.IsNullOrEmpty(SeniorMedicalReviewerComment))
            {
                AddValidationItem(new ValidationItem("Senior Medical Reviewer", "Decision Explanation", "Must provide a Decision Explanation for the IN case."));
                canForward = false;
            }

            return canForward;
        }
    }
}