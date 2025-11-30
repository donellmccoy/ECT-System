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
    public class SC_MO : SpecialCase
    {
        public SC_MO()
        {
        }

        public virtual int? AssociatedSC { get; set; }

        public virtual int? CaseType { get; set; }

        /// <inheritdoc/>
        public override int DocumentViewId
        {
            get { return (int)DocumentViewType.MO; }
        }

        /// <inheritdoc/>
        public override int? HasAdminLOD { get; set; }

        public virtual DateTime? HighTenureDate { get; set; }

        public virtual string ICD7thCharacter { get; set; }

        // Medical Tech / HQ AFRC Tech
        public virtual int? ICD9Code { get; set; }

        public virtual string ICD9Description { get; set; }
        public virtual string ICD9Diagnosis { get; set; }

        // Special Case Type (matches module type) - That drop down is included so when they have to run reports on how many modifications were for WWD, for instance, we have captured that information.
        public virtual string Justification { get; set; }

        // HYTD/MSD

        public virtual int? MedGroupName { get; set; }
        public virtual string PocEmail { get; set; }
        public virtual string PocPhoneDSN { get; set; }
        public virtual string PocRankAndName { get; set; }
        public virtual int? RMUName { get; set; }

        /// <inheritdoc/>
        public override Dictionary<String, PageAccessType> ReadSectionList(int role)
        {
            PageAccessType access;
            access = PageAccessType.None;

            Dictionary<String, PageAccessType> scAccessList = new Dictionary<String, PageAccessType>();
            if ((CurrentStatusCode == (int)SpecCaseMOStatusCode.Approved) || (CurrentStatusCode == (int)SpecCaseMOStatusCode.Denied))
            {
                access = PageAccessType.ReadOnly;
            }

            //'Add all pages as readonly
            scAccessList.Add(MOSectionNames.MO_RLB.ToString(), access);
            scAccessList.Add(MOSectionNames.MO_MED_TECH_INIT.ToString(), access);
            scAccessList.Add(MOSectionNames.MO_HQT_INIT.ToString(), access);
            scAccessList.Add(MOSectionNames.MO_HQT_REV.ToString(), access);
            scAccessList.Add(MOSectionNames.MO_BOARD_SG_REV.ToString(), access);
            scAccessList.Add(MOSectionNames.MO_HQT_FINAL_REV.ToString(), access);
            scAccessList.Add(MOSectionNames.MO_APPROVED.ToString(), access);
            scAccessList.Add(MOSectionNames.MO_DENIED.ToString(), access);

            //Modify access with user role
            switch (role)
            {
                case (int)UserGroups.AFRCHQTechnician:
                    if (CurrentStatusCode == (int)SpecCaseMOStatusCode.HQAFRCTechInitiateCase)
                    {
                        scAccessList[MOSectionNames.MO_HQT_INIT.ToString()] = PageAccessType.ReadWrite;
                    }
                    if (CurrentStatusCode == (int)SpecCaseMOStatusCode.PackageReview)
                    {
                        scAccessList[MOSectionNames.MO_HQT_REV.ToString()] = PageAccessType.ReadWrite;
                        scAccessList[MOSectionNames.MO_RLB.ToString()] = PageAccessType.ReadOnly;
                    }
                    if (CurrentStatusCode == (int)SpecCaseMOStatusCode.FinalReview)
                    {
                        scAccessList[MOSectionNames.MO_HQT_FINAL_REV.ToString()] = PageAccessType.ReadWrite;
                        scAccessList[MOSectionNames.MO_RLB.ToString()] = PageAccessType.ReadOnly;
                    }

                    break;

                case (int)UserGroups.BoardMedical:
                    if (CurrentStatusCode == (int)SpecCaseMOStatusCode.MedicalReview)
                    {
                        scAccessList[MOSectionNames.MO_BOARD_SG_REV.ToString()] = PageAccessType.ReadWrite;
                    }
                    break;

                case (int)UserGroups.MedicalTechnician:
                    if (CurrentStatusCode == (int)SpecCaseMOStatusCode.MedTechInitiateCase)
                    {
                        scAccessList[MOSectionNames.MO_MED_TECH_INIT.ToString()] = PageAccessType.ReadWrite;
                        scAccessList[MOSectionNames.MO_RLB.ToString()] = PageAccessType.ReadWrite;
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
                                IList<Memorandum2> memolist = (from m in memoDao.GetByRefnModule(Id, (int)ModuleType.SpecCaseMO)
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
                }
            }
        }

        protected bool DetermineVisibilityForCanCompleteCaseRule(int expectedFinding)
        {
            if (!med_off_approved.HasValue)
            {
                AddValidationItem(new ValidationItem("Board Medical Officer", "Decision", "Board Medical Officer must provide a finding in order to close out the MO case."));
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
            if (CurrentStatusCode == (int)SpecCaseMOStatusCode.MedTechInitiateCase)
                return ValidateCanForwardCaseRuleForInitiateStatus("Medical Technician");

            if (CurrentStatusCode == (int)SpecCaseMOStatusCode.HQAFRCTechInitiateCase)
                return ValidateCanForwardCaseRuleForInitiateStatus("HQ AFRC Technician");

            if (CurrentStatusCode == (int)SpecCaseMOStatusCode.MedicalReview)
                return ValidateCanForwardCaseRuleForBoardMedicalOfficer();

            if (CurrentStatusCode == (int)SpecCaseMOStatusCode.SeniorMedicalReview)
                return ValidateCanForwardCaseRuleForSeniorMedicalReviewer();

            return true;
        }

        private bool ValidateCanForwardCaseRuleForBoardMedicalOfficer()
        {
            if (!med_off_approved.HasValue)
            {
                AddValidationItem(new ValidationItem("Board Medical Officer", "Decision", "Must provide a Decision for the MO case."));
                return false;
            }

            return true;
        }

        private bool ValidateCanForwardCaseRuleForInitiateStatus(string userGroupNameBeingValidated)
        {
            bool canForward = true;

            if (!ICD9Code.HasValue || ICD9Code.Value == 0)
            {
                canForward = false;
                AddValidationItem(new ValidationItem(userGroupNameBeingValidated, "ICD9", "Must select a Diagnosis (ICD) for the MO case."));
            }

            if (string.IsNullOrEmpty(ICD9Diagnosis))
            {
                canForward = false;
                AddValidationItem(new ValidationItem(userGroupNameBeingValidated, "ICD9 Description", "Must enter a Diagnosis Description for the MO case."));
            }

            if (string.IsNullOrEmpty(Justification))
            {
                canForward = false;
                AddValidationItem(new ValidationItem(userGroupNameBeingValidated, "Justification", "Must enter a Justification for the MO case."));
            }

            return canForward;
        }

        private bool ValidateCanForwardCaseRuleForSeniorMedicalReviewer()
        {
            bool canForward = true;

            if (string.IsNullOrEmpty(SeniorMedicalReviewerConcur))
            {
                canForward = false;
                AddValidationItem(new ValidationItem("Senior Medical Reviewer", "Decision", "Must provide a Concur/Non-Concur Decision for the MO case."));
            }
            else
            {
                if (SeniorMedicalReviewerConcur.Equals(DECISION_NONCONCUR) && !SeniorMedicalReviewerApproved.HasValue)
                {
                    canForward = false;
                    AddValidationItem(new ValidationItem("Senior Medical Reviewer", "Determination", "Must provide a Decision for the MO case."));
                }
            }

            if (string.IsNullOrEmpty(SeniorMedicalReviewerComment))
            {
                AddValidationItem(new ValidationItem("Senior Medical Reviewer", "Decision Explanation", "Must provide a Decision Explanation for the MO case."));
                canForward = false;
            }

            return canForward;
        }
    }
}