using ALOD.Core.Domain.Common;
using ALOD.Core.Domain.Documents;
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
    public class SC_CMAS : SpecialCase
    {
        public SC_CMAS()
        {
            ;
        }

        /// <inheritdoc/>
        public override int DocumentViewId
        {
            get { return (int)DocumentViewType.CMAS; }
        }

        public virtual IList<LineOfDutyFindings> LodFindings { get; set; }
        public virtual DateTime? SystemOutDate { get; set; }
        public virtual DateTime? SystemReceiveDate { get; set; }

        /// <inheritdoc/>
        public override IEnumerable<WorkflowStatusOption> GetCurrentOptions(int lastStatus, IDaoFactory factory)
        {
            Validations.Clear();
            Validate();
            ProcessDocuments(factory);
            ProcessOption(lastStatus, factory);
            return RuleAppliedOptions;
        }

        /// <inheritdoc/>
        public override Dictionary<String, PageAccessType> ReadSectionList(int role)
        {
            PageAccessType access;
            access = PageAccessType.None;

            Dictionary<String, PageAccessType> scAccessList = new Dictionary<String, PageAccessType>();
            if ((CurrentStatusCode == (int)SpecCaseCMASStatusCode.Complete))
            {
                access = PageAccessType.ReadOnly;
            }

            //'Add all pages as readonly

            scAccessList.Add(CMSectionNames.CM_HQT_INIT.ToString(), access); ;
            scAccessList.Add(CMSectionNames.CM_COMPLETE.ToString(), access);
            scAccessList.Add(CMSectionNames.CM_RLB.ToString(), access);

            //Modify access with user role
            switch (role)
            {
                case (int)UserGroups.AFRCHQTechnician:
                    if (CurrentStatusCode == (int)SpecCaseCMASStatusCode.InitiateCase)
                    {
                        scAccessList[CMSectionNames.CM_HQT_INIT.ToString()] = PageAccessType.ReadWrite;
                    }
                    else
                    {
                        scAccessList[CMSectionNames.CM_COMPLETE.ToString()] = PageAccessType.ReadWrite;
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

            if (Status == (int)SpecCaseCMASWorkStatus.InitiateCase)
            {
                section = "HQ Tech - CMAS Information";

                //Required Data
                if (!(SystemReceiveDate.HasValue))
                {
                    AddValidationItem(new ValidationItem(section, "DateInTB", "Date In is required."));
                }

                if (!(SystemOutDate.HasValue))
                {
                    AddValidationItem(new ValidationItem(section, "DateOutTB", "Date Out is required."));
                }

                //Make sure Date IN is less than Date Out
                if ((SystemReceiveDate.HasValue) && (SystemOutDate.HasValue))
                {
                    if (SystemOutDate < SystemReceiveDate)
                    {
                        AddValidationItem(new ValidationItem(section, "DateInTB,DateOutTB", "Date In must be earlier than Date Out."));
                    }
                }
            }
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
                                IList<Memorandum2> memolist = (from m in memoDao.GetByRefnModule(Id, (int)ModuleType.SpecCaseCMAS) where m.Deleted == false && m.Template.Id == Convert.ToByte(memos[i]) select m).ToList<Memorandum2>();
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
                                //string docName = ((DocumentType)(Convert.ToByte(docs[i]))).ToString();
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

                    case "hqtechvalidate":
                        //Required Data
                        if ((SystemOutDate == null) || (SystemReceiveDate == null))
                        {
                            o.OptionValid = false;
                        }

                        //Make sure Date IN is less than Date Out
                        if ((SystemOutDate != null) && (SystemReceiveDate != null))
                        {
                            if (SystemOutDate < SystemReceiveDate)
                            {
                                o.OptionValid = false;
                            }
                        }

                        break;
                }
            }
        }
    }
}