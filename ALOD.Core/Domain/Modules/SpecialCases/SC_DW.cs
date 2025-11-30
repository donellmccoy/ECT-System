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
    public class SC_DW : SpecialCase
    {
        public SC_DW()
        {
            //Testing Required Documents
            AllDocuments.Add(DocumentType.AF422AF469.ToString(), false);
            AllDocuments.Add(DocumentType.NarrativeSummary.ToString(), false);
            AllDocuments.Add(DocumentType.RTDLetter.ToString(), false);
            AllDocuments.Add(DocumentType.DWSupportingMedicalDocs.ToString(), false);
        }

        public virtual DateTime? ApprovalDate { get; set; }
        public virtual DateTime? deploy_end_date { get; set; }
        public virtual string deploy_location { get; set; }
        public virtual DateTime? deploy_start_date { get; set; }

        /// <inheritdoc/>
        public override int DocumentViewId
        {
            get { return (int)DocumentViewType.DW; }
        }

        public virtual int? DWaiverCategory { get; set; }
        public virtual string line_number { get; set; }
        public virtual string line_remarks { get; set; }
        public virtual int? MAJCOM { get; set; }
        public virtual int? MedGroupName { get; set; }
        public virtual string PocEmail { get; set; }
        public virtual string PocPhoneDSN { get; set; }
        public virtual string PocRankAndName { get; set; }
        public virtual int? RMUName { get; set; }
        public virtual int? Sim_Deployment { get; set; }

        /// <inheritdoc/>
        public override Dictionary<String, PageAccessType> ReadSectionList(int role)
        {
            PageAccessType access;
            access = PageAccessType.None;

            Dictionary<String, PageAccessType> scAccessList = new Dictionary<String, PageAccessType>();
            if ((CurrentStatusCode == (int)SpecCaseDWStatusCode.Complete))
            {
                access = PageAccessType.ReadOnly;
            }

            //'Add all pages as readonly

            scAccessList.Add(DWSectionNames.DW_MED_TECH_INIT.ToString(), access);
            scAccessList.Add(DWSectionNames.DW_HQT_REV.ToString(), access);
            scAccessList.Add(DWSectionNames.DW_BOARD_SG_REV.ToString(), access);
            scAccessList.Add(DWSectionNames.DW_SENIOR_MED_REV.ToString(), access);
            scAccessList.Add(DWSectionNames.DW_HQT_FINAL_REV.ToString(), access);
            scAccessList.Add(DWSectionNames.DW_APPROVED.ToString(), access);
            scAccessList.Add(DWSectionNames.DW_DENIED.ToString(), access);
            scAccessList.Add(DWSectionNames.DW_RLB.ToString(), access);

            //Modify access with user role
            switch (role)
            {
                case (int)UserGroups.MedicalTechnician:
                    if (CurrentStatusCode == (int)SpecCaseDWStatusCode.InitiateCase)
                    {
                        scAccessList[DWSectionNames.DW_MED_TECH_INIT.ToString()] = PageAccessType.ReadWrite;
                        scAccessList[DWSectionNames.DW_RLB.ToString()] = PageAccessType.ReadWrite;
                    }
                    break;

                case (int)UserGroups.AFRCHQTechnician:
                    if (CurrentStatusCode == (int)SpecCaseDWStatusCode.PackageReview)
                    {
                        scAccessList[DWSectionNames.DW_HQT_REV.ToString()] = PageAccessType.ReadWrite;
                    }
                    if (CurrentStatusCode == (int)SpecCaseDWStatusCode.FinalReview)
                    {
                        scAccessList[DWSectionNames.DW_HQT_FINAL_REV.ToString()] = PageAccessType.ReadWrite;
                    }
                    if (CurrentStatusCode == (int)SpecCaseDWStatusCode.FinalReviewPending)
                    {
                        scAccessList[DWSectionNames.DW_HQT_FINAL_REV.ToString()] = PageAccessType.ReadWrite;
                    }

                    scAccessList[DWSectionNames.DW_RLB.ToString()] = PageAccessType.ReadOnly;
                    break;

                case (int)UserGroups.BoardMedical:
                    if (CurrentStatusCode == (int)SpecCaseDWStatusCode.MedicalReview)
                    {
                        scAccessList[DWSectionNames.DW_BOARD_SG_REV.ToString()] = PageAccessType.ReadWrite;
                    }
                    break;

                case (int)UserGroups.SeniorMedicalReviewer:
                    if (CurrentStatusCode == (int)SpecCaseDWStatusCode.SeniorMedicalReview)
                    {
                        scAccessList[DWSectionNames.DW_SENIOR_MED_REV.ToString()] = PageAccessType.ReadWrite;
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
                switch (r.RuleTypes.Name.ToLower())
                {
                    case "canforwardcase":
                        bool canForward = ValidateCanForwardCaseRule();

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

        protected bool DetermineVisibilityForCanCompleteCaseRule(int expectedFinding)
        {
            if (!med_off_approved.HasValue)
            {
                AddValidationItem(new ValidationItem("Board Medical Officer", "Decision", "Board Medical Officer must provide a finding in order to close out the DW case."));
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
        protected override bool IsDocumentCategoryStillRequired(bool docFound, string docName, IDaoFactory daoFactory)
        {
            if (docFound)
            {
                return false;
            }
            else
            {
                if (docName == DocumentType.MAJCOMDisposition.ToString())
                {
                    if (MAJCOM.HasValue)
                    {
                        return true;
                    }
                }
                else if (docName == DocumentType.Checklist.ToString())
                {
                    // Required if MAJCOM selected is CENTCOM or SOUTHCOM
                    if (MAJCOM.HasValue && MAJCOM.Value != 2 && MAJCOM.Value != 6)
                    {
                        return false;
                    }
                }
                else if (docName == DocumentType.MedicalSpecialInstructions.ToString())
                {
                    // Medical Special Instructions (SpINs) (required only if Simulated Deployment question answered yes
                    if (Sim_Deployment.HasValue && Sim_Deployment != 1)
                    {
                        return false;
                    }
                }

                return base.IsDocumentCategoryStillRequired(docFound, docName, daoFactory);
            }
        }

        protected bool ValidateCanForwardCaseRule()
        {
            if (CurrentStatusCode == (int)SpecCaseDWStatusCode.InitiateCase)
                return ValidateCanForwardCaseRuleForMedicalTechnician();

            if (CurrentStatusCode == (int)SpecCaseDWStatusCode.MedicalReview)
                return ValidateCanForwardCaseRuleForBoardMedicalOfficer();

            if (CurrentStatusCode == (int)SpecCaseDWStatusCode.SeniorMedicalReview)
                return ValidateCanForwardCaseRuleForSeniorMedicalReviewer();

            return true;
        }

        private bool IsDeployEndDateAfterDeployStartDate()
        {
            if (!deploy_start_date.HasValue || !deploy_end_date.HasValue)
                return false;

            if (DateTime.Compare(deploy_end_date.Value, deploy_start_date.Value) <= 0)
                return false;

            return true;
        }

        private bool ValidateCanForwardCaseRuleForBoardMedicalOfficer()
        {
            bool canForward = true;

            if (!med_off_approved.HasValue)
            {
                canForward = false;
                AddValidationItem(new ValidationItem("Board Medical Officer", "Determination", "Must provide a Determination for the DW case."));
            }

            if (string.IsNullOrEmpty(med_off_approval_comment))
            {
                canForward = false;
                AddValidationItem(new ValidationItem("Board Medical Officer", "Decision Explanation", "Must provide a Decision Explanation for the DW case."));
            }

            return canForward;
        }

        private bool ValidateCanForwardCaseRuleForMedicalTechnician()
        {
            bool canForward = true;

            if (!deploy_start_date.HasValue)
            {
                canForward = false;
                AddValidationItem(new ValidationItem("Medical Technician", "Deploy Start", "Please enter the Deployment Start Date."));
            }

            if (!deploy_end_date.HasValue)
            {
                canForward = false;
                AddValidationItem(new ValidationItem("Medical Technician", "Deploy End", "Please enter the Deployment End Date."));
            }

            if (!IsDeployEndDateAfterDeployStartDate())
            {
                canForward = false;
                AddValidationItem(new ValidationItem("Medical Technician", "Deploy Compare", "Deployment End Date must be after Start Date."));
            }

            if (string.IsNullOrEmpty(deploy_location))
            {
                canForward = false;
                AddValidationItem(new ValidationItem("Medical Technician", "Deploy Location", "Please enter the Deployment Location."));
            }

            return canForward;
        }

        private bool ValidateCanForwardCaseRuleForSeniorMedicalReviewer()
        {
            bool canForward = true;

            if (string.IsNullOrEmpty(SeniorMedicalReviewerConcur))
            {
                canForward = false;
                AddValidationItem(new ValidationItem("Senior Medical Reviewer", "Decision", "Must provide a Concur/Non-Concur Decision for the DW case."));
            }
            else
            {
                if (SeniorMedicalReviewerConcur.Equals(DECISION_NONCONCUR) && !SeniorMedicalReviewerApproved.HasValue)
                {
                    canForward = false;
                    AddValidationItem(new ValidationItem("Senior Medical Reviewer", "Determination", "Must provide a Decision for the DW case."));
                }
            }

            if (string.IsNullOrEmpty(SeniorMedicalReviewerComment))
            {
                AddValidationItem(new ValidationItem("Senior Medical Reviewer", "Decision Explanation", "Must provide a Decision Explanation for the DW case."));
                canForward = false;
            }

            return canForward;
        }
    }
}