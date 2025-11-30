using ALOD.Core.Domain.Common;
using ALOD.Core.Domain.Documents;
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
    public class SC_PH : SpecialCase
    {
        public SC_PH()
                    : base()
        {
            UnitPHSignature = new SignatureEntry();
            HQAFRCDPHSignature = new SignatureEntry();
        }

        public virtual int? CaseUnitId { get; set; }

        /// <inheritdoc/>
        public override string DocumentEntityId
        {
            get
            {
                if (DPHUser == null)
                    return string.Empty;

                if (!string.IsNullOrEmpty(DPHUser.EDIPIN))
                    return DPHUser.EDIPIN;

                return DPHUser.SSN;
            }
        }

        /// <inheritdoc/>
        public override int DocumentViewId
        {
            get { return (int)DocumentViewType.PH; }
        }

        // Start Page Properties
        public virtual AppUser DPHUser { get; set; }     // Unit PH user who initated the case

        public virtual DateTime? FormLastModified { get; set; }
        public virtual SignatureEntry HQAFRCDPHSignature { get; set; }

        // Misc Properties
        public virtual bool? IsDelinquent { get; set; }

        public virtual DateTime? ReportingPeriod { get; set; }

        // Signatures
        public virtual SignatureEntry UnitPHSignature { get; set; }

        /// <inheritdoc/>
        public override void PerformOverride(IWorkStatusDao workStatusDao, IReminderEmailDao reminderEmailDao, int newStatusId, int oldStatusId)
        {
            base.PerformOverride(workStatusDao, reminderEmailDao, newStatusId, oldStatusId);

            if (newStatusId != oldStatusId)
            {
                if (newStatusId == (int)SpecCasePHWorkStatus.InitiateCase || newStatusId == (int)SpecCasePHWorkStatus.AFRCReview)
                {
                    IsDelinquent = false;
                }

                if (newStatusId == (int)SpecCasePHWorkStatus.Delinquent || newStatusId == (int)SpecCasePHWorkStatus.DelinquentAFRCReview)
                {
                    IsDelinquent = true;
                }
            }
        }

        /// <inheritdoc/>
        public override Dictionary<String, PageAccessType> ReadSectionList(int role)  // override
        {
            PageAccessType access;
            access = PageAccessType.None;

            Dictionary<String, PageAccessType> scAccessList = new Dictionary<String, PageAccessType>();
            if (CurrentStatusCode == (int)SpecCasePHStatusCode.Complete || CurrentStatusCode == (int)SpecCasePHStatusCode.Cancelled)
            {
                access = PageAccessType.ReadOnly;
            }

            //'Add all pages as readonly
            scAccessList.Add(PHSectionNames.PH_RLB.ToString(), access);
            scAccessList.Add(PHSectionNames.PH_HQDPH_REV.ToString(), access);
            scAccessList.Add(PHSectionNames.PH_UNIT_INIT.ToString(), access);
            scAccessList.Add(PHSectionNames.PH_COMPLETE.ToString(), access);

            //Modify access with user role
            switch (role)
            {
                case (int)UserGroups.UnitPH:
                    if (CurrentStatusCode == (int)SpecCasePHStatusCode.InitiateCase || CurrentStatusCode == (int)SpecCasePHStatusCode.Delinquent)
                    {
                        scAccessList[PHSectionNames.PH_UNIT_INIT.ToString()] = PageAccessType.ReadWrite;
                        scAccessList[PHSectionNames.PH_RLB.ToString()] = PageAccessType.ReadWrite;
                    }
                    break;

                case (int)UserGroups.HQAFRCDPH:
                    if (CurrentStatusCode == (int)SpecCasePHStatusCode.AFRCReview || CurrentStatusCode == (int)SpecCasePHStatusCode.DelinquentAFRCReview)
                    {
                        scAccessList[PHSectionNames.PH_HQDPH_REV.ToString()] = PageAccessType.ReadWrite;
                        scAccessList[PHSectionNames.PH_RLB.ToString()] = PageAccessType.ReadOnly;
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
                                IList<Memorandum2> memolist = (from m in memoDao.GetByRefnModule(Id, (int)ModuleType.SpecCasePH)
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
                        bool canforward = true;

                        if (WorkflowStatus.Id == (int)SpecCasePHWorkStatus.InitiateCase || WorkflowStatus.Id == (int)SpecCasePHWorkStatus.Delinquent)
                        {
                            string user = string.Empty;
                            user = "HQ AFRC Tech";
                        }
                        else if (WorkflowStatus.Id == (int)SpecCasePHWorkStatus.AFRCReview || WorkflowStatus.Id == (int)SpecCasePHWorkStatus.DelinquentAFRCReview)
                        {
                        }

                        if (canforward != true)
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
    }
}