using ALOD.Core.Domain.Common;
using ALOD.Core.Domain.Documents;
using ALOD.Core.Domain.Lookup;
using ALOD.Core.Domain.Modules.Lod;
using ALOD.Core.Domain.Users;
using ALOD.Core.Domain.Workflow;
using ALOD.Core.Interfaces;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ALOD.Core.Domain.Modules.SARC
{
    [Serializable]
    public class RestrictedSARC : Entity
    {
        public const int ROTC_CADET_MEMBER_STATUS = 5;
        private IDictionary<string, bool> _catAllDocs;

        private IDictionary<string, bool> _catRequired;

        private IDictionary<string, bool> _moduleStatus;

        private IList<WorkflowStatusOption> _ruleAppliedOptions;

        private IList<ValidationItem> _validations;

        public RestrictedSARC()
        {
            AllDocuments.Add(DocumentType.UnredactedSupportingDocumnets.ToString(), false);
            AllDocuments.Add(DocumentType.RedactedSupportingDocuments.ToString(), false);
            AllDocuments.Add(DocumentType.Miscellaneous.ToString(), false);

            //RSLWingSignature = new SignatureEntry();
            //SARCAdminSignature = new SignatureEntry();
            //BoardMedicalSignature = new SignatureEntry();
            //BoardJASignature = new SignatureEntry();
            //ApprovingSignature = new SignatureEntry();

            MemberRank = new ALOD.Core.Domain.Lookup.UserRank();
        }

        public virtual DateTime? Cancel_Date { get; set; }

        public virtual String Cancel_Explanation { get; set; }

        public virtual int? Cancel_Reason { get; set; }

        //case data
        public virtual string CaseId { get; set; }

        public virtual byte? ConsultationFromUserGroupId { get; set; }
        public virtual int CreatedBy { get; set; }
        public virtual DateTime? CreatedDate { get; set; }

        public virtual int CurrentStatusCode
        {
            get { return WorkflowStatus.StatusCodeType.Id; }
        }

        public virtual string DefenseSexualAssaultDBCaseNumber { get; set; }
        public virtual IDocumentDao DocumentDao { get; set; }

        // Doucments
        public virtual IList<Document> Documents { get; set; }

        public virtual DateTime? DurationEnd { get; set; }
        public virtual DateTime? DurationStart { get; set; }
        public virtual int? DutyStatus { get; set; }
        public virtual bool? ICDE968 { get; set; }
        public virtual bool? ICDE969 { get; set; }
        public virtual IList<RestrictedSARCOtherICDCode> ICDList { get; set; }
        public virtual bool? ICDOther { get; set; }
        public virtual DateTime? IncidentDate { get; set; }
        public virtual bool? InDutyStatus { get; set; }

        public virtual bool IsInPostCompletionProcessing
        {
            get
            {
                if (WorkflowStatus.StatusCodeType.IsFinal && !WorkflowStatus.StatusCodeType.IsCancel)
                    return true;

                return false;
            }
        }

        public virtual bool IsPostProcessingComplete { get; set; }
        public virtual String MemberCompo { get; set; }
        public virtual DateTime? MemberDOB { get; set; }
        public virtual String MemberName { get; set; }
        public virtual UserRank MemberRank { get; set; }
        public virtual String MemberSSN { get; set; }

        //    public virtual int? MemberGrade { get; set; }
        public virtual String MemberUnit { get; set; }

        public virtual int? MemberUnitId { get; set; }
        public virtual int ModifiedBy { get; set; }
        public virtual DateTime? ModifiedDate { get; set; }

        public virtual int ModuleId
        {
            get { return (int)ModuleType.SARC; }
        }

        public virtual IDictionary<string, bool> ModuleStatus
        {
            get
            {
                if (_moduleStatus == null)
                    _moduleStatus = new Dictionary<string, bool>();

                return _moduleStatus;
            }

            set { _moduleStatus = value; }
        }

        public virtual String Return_Comment { get; set; }
        public virtual int? ReturnByGroup { get; set; }
        public virtual int? ReturnToGroup { get; set; }

        public virtual IList<WorkflowStatusOption> RuleAppliedOptions
        {
            get
            {
                if (_ruleAppliedOptions == null)
                    _ruleAppliedOptions = new List<WorkflowStatusOption>();

                return _ruleAppliedOptions;
            }

            set { _ruleAppliedOptions = value; }
        }

        public virtual DateTime? RWOA_Date { get; set; }
        public virtual String RWOA_Explanation { get; set; }
        public virtual int? RWOA_Reason { get; set; }
        public virtual IList<RestrictedSARCFindings> SARCFindings { get; set; }
        public virtual int Status { get; set; }

        public virtual IList<ValidationItem> Validations
        {
            get
            {
                if (_validations == null)
                    _validations = new List<ValidationItem>();

                return _validations;
            }

            set { _validations = value; }
        }

        public virtual int Workflow { get; set; }
        public virtual WorkStatus WorkflowStatus { get; set; }
        //Signatures
        //public virtual SignatureEntry RSLWingSignature { get; set; }  //initiate step
        //public virtual SignatureEntry SARCAdminSignature { get; set; }
        //public virtual SignatureEntry BoardMedicalSignature { get; set; }
        //public virtual SignatureEntry BoardJASignature { get; set; }
        //public virtual SignatureEntry ApprovingSignature { get; set; }
        //public virtual SignatureEntry BoardAdminSignature { get; set; }

        #region IDocumentSource Members

        public virtual IDictionary<string, bool> AllDocuments
        {
            get
            {
                if (_catAllDocs == null)
                    _catAllDocs = new Dictionary<string, bool>();

                return _catAllDocs;
            }

            set { _catAllDocs = value; }
        }

        public virtual string DocumentEntityId
        {
            get { return MemberSSN; }
        }

        public virtual long? DocumentGroupId { get; set; }

        public virtual int DocumentViewId
        {
            get { return (int)DocumentViewType.SARC; }
        }

        public virtual IDictionary<string, bool> Required
        {
            get
            {
                if (_catRequired == null)
                    _catRequired = new Dictionary<string, bool>();

                return _catRequired;
            }

            set { _catRequired = value; }
        }

        public virtual bool CreateDocumentGroup(IDocumentDao dao)
        {
            Check.Require(dao != null, "DocumentDao is required");

            DocumentGroupId = dao.CreateGroup();
            return DocumentGroupId > 0;
        }

        #endregion IDocumentSource Members

        #region Findings

        public virtual short FinalFindings
        {
            get
            {
                if (!(WorkflowStatus.StatusCodeType.IsFinal && !WorkflowStatus.StatusCodeType.IsCancel))
                    return 0;

                RestrictedSARCFindings finalFinding = FindByType((short)PersonnelTypes.BOARD_AA);

                if (finalFinding == null)
                    return 0;

                return finalFinding.Finding.Value;
            }
        }

        public virtual bool DoesFindingExist(short pType)
        {
            if (FindByType(pType) == null)
                return false;

            return true;
        }

        public virtual RestrictedSARCFindings FindByType(short pType)
        {
            IEnumerable<RestrictedSARCFindings> lst = (from p in SARCFindings where p.PType == pType select p);
            if (lst.Count() > 0)
            {
                RestrictedSARCFindings old = lst.First();
                return old;
            }
            return null;
        }

        public virtual bool HasAFinding(RestrictedSARCFindings finding)
        {
            if (finding == null)
                return false;

            if (!finding.Finding.HasValue || finding.Finding.Value == 0)
                return false;

            return true;
        }

        public virtual RestrictedSARCFindings SetFindingByType(RestrictedSARCFindings fnd)
        {
            int i = 0;

            if (SARCFindings.Count > 0) // Correct
            {
                foreach (RestrictedSARCFindings item in SARCFindings)
                {
                    if (item.PType == fnd.PType)
                    {
                        SARCFindings.ElementAt(i).Modify(fnd);
                        return SARCFindings.ElementAt(i);
                    }
                    i = i + 1;
                }
            }

            SARCFindings.Add(fnd);

            return SARCFindings.ElementAt(i);
        }

        public virtual RestrictedSARCFindings SetPersonalByType(RestrictedSARCFindings fnd)
        {
            int i = 0;

            if (SARCFindings.Count > 0) // Correct
            {
                foreach (RestrictedSARCFindings item in SARCFindings)
                {
                    if (item.PType == fnd.PType)
                    {
                        SARCFindings.ElementAt(i).ModifyPersonal(fnd);
                        return SARCFindings.ElementAt(i);
                    }
                    i = i + 1;
                }
            }

            SARCFindings.Add(fnd);

            return SARCFindings.ElementAt(i);
        }

        #endregion Findings

        public virtual void AddOtherICDCode(ICD9Code newOtherICDCode, string newICD7thCharacter)
        {
            if (newOtherICDCode == null)
                return;

            foreach (RestrictedSARCOtherICDCode c in ICDList)
            {
                if (c.ICDCode.Id == newOtherICDCode.Id)
                    return;
            }

            ICDList.Add(new RestrictedSARCOtherICDCode(this.Id, newOtherICDCode, newICD7thCharacter));
        }

        public virtual void AddSignature(IDaoFactory daoFactory, UserGroups groupId, string title, AppUser user)
        {
            ISignatueMetaDateDao sigDao = daoFactory.GetSigMetaDataDao();

            SignatureMetaData signature = new SignatureMetaData();
            signature.refId = Id;
            signature.workflowId = Workflow;
            signature.workStatus = Status;
            signature.userGroup = (int)groupId;
            signature.userId = user.Id;
            signature.date = DateTime.Now;
            signature.NameAndRank = user.AlternateSignatureName;
            signature.Title = title;

            sigDao.AddForWorkStatus(signature);
        }

        public virtual bool DetermineIfPostProcessingIsComplete(IDaoFactory daoFactory)
        {
            if (!WorkflowStatus.StatusCodeType.IsFinal)
                return false;

            if (WorkflowStatus.StatusCodeType.IsCancel)
                return true;

            if (HasMemberBeenNotified(daoFactory) && HasDeterminationMemoBeenGenerated(daoFactory) && HasPostCompletionDigitalSignatureBeenGenerated(daoFactory))
                return true;

            return false;
        }

        public virtual IEnumerable<WorkflowStatusOption> GetCurrentOptions(int lastStatus, IDaoFactory daoFactory)
        {
            Validations.Clear();
            Validate();
            ProcessDocuments(daoFactory);
            ProcessOption(lastStatus, daoFactory);
            return RuleAppliedOptions;
        }

        public virtual bool NotificationMemoCreated(IMemoDao2 memoDao)
        {
            if (!IsInPostCompletionProcessing)
            {
                return false;
            }

            byte notificationType = 0;

            if (FinalFindings == (byte)ALOD.Core.Utils.Finding.In_Line_Of_Duty)
            {
                if (MemberCompo == "6") notificationType = (byte)MemoType.SARC_Determination_ILOD;
                else if (MemberCompo == "5") notificationType = (byte)MemoType.ANGSARC_Determination_ILOD;
            }
            else if (FinalFindings == (byte)ALOD.Core.Utils.Finding.Nlod_Not_Due_To_OwnMisconduct)
            {
                if (MemberCompo == "6") notificationType = (byte)MemoType.SARC_Determination_NILOD;
                else if (MemberCompo == "5") notificationType = (byte)MemoType.ANGSARC_Determination_NILOD;
            }

            if (notificationType == 0)
                return false;

            IList<Memorandum2> memolist = (from m in memoDao.GetByRefnModule(Id, (short)ModuleType.SARC) where m.Deleted == false && m.Template.Id == notificationType select m).ToList<Memorandum2>();
            if (memolist.Count > 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// This method performs a basic override of the special case. It checks if the case is being overriden to or from a
        /// cancel status and sets or clears the appropriate cancel related fields. It then simply assigns the new status ID to
        /// the Status property and assigns a new WorkStatus object to the WorkflowStatus property. If special considerations need
        /// to be taken into account for overriding a specific special case, then the class for that special case can override
        /// this method and provide its own implementation so that it can include code to handle those special considerations.
        /// </summary>
        public virtual void PerformOverride(IDaoFactory daoFactory, int newStatusId, int oldStatusId)
        {
            if (newStatusId != oldStatusId)
            {
                IWorkStatusDao workStatusDao = daoFactory.GetWorkStatusDao();
                IReminderEmailDao reminderEmailDao = daoFactory.GetReminderEmailDao();

                WorkStatus newStatus = workStatusDao.GetById(newStatusId);
                WorkStatus oldStatus = workStatusDao.GetById(oldStatusId);

                // Check if overriding FROM a cancel status...
                if (oldStatus.StatusCodeType.IsCancel == true)
                {
                    Cancel_Reason = null;
                    Cancel_Explanation = null;
                    Cancel_Date = null;
                }

                // Check if overriding TO a cancel status...
                if (newStatus.StatusCodeType.IsCancel == true)
                {
                    Cancel_Reason = 8;
                    Cancel_Explanation = "Case Cancelled by System Admin";
                    Cancel_Date = DateTime.Now;
                }

                reminderEmailDao.ReminderEmailUpdateStatusChange(oldStatusId, newStatusId, this.CaseId, "SARC");

                this.Status = newStatusId;
                this.WorkflowStatus = newStatus;

                UpdateIsPostProcessingComplete(daoFactory);
            }
        }

        public virtual void ProcessDocuments(IDaoFactory daoFactory)
        {
            IDocumentDao docDao = daoFactory.GetDocumentDao();
            IDocCategoryViewDao docViewDao = daoFactory.GetDocCategoryViewDao();

            if (DocumentGroupId != null)
            {
                Documents = docDao.GetDocumentsByGroupId(DocumentGroupId.Value);
            }

            List<DocumentCategory2> viewCats = (List<DocumentCategory2>)docViewDao.GetCategoriesByDocumentViewId(DocumentViewId);
            //            DocCategoryView view = docViewDao.GetById(DocumentViewId, false);
            //            foreach (DocumentCategory2 dc in view)
            //foreach (DocumentCategory2 dc in viewCats)
            //{
            //    if (!AllDocuments.ContainsKey(((DocumentType)(Convert.ToByte(dc.DocCatId))).ToString()))
            //    {
            //        UpdateActiveCategories(((DocumentType)(Convert.ToByte(dc.DocCatId))).ToString(), true);
            //    }
            //}

            IList<Document> doclist;
            string[] docs;
            string description = "";

            //This code checks the  document rules for current lod status for various options
            //Active categories are updated to reflect the required documents.
            //Documents which are required by the rules are added to the present category and it is
            //also updated to reflect valid//invalid based on the uploaded  documents
            if (WorkflowStatus != null)
            {
                foreach (var opt in WorkflowStatus.WorkStatusOptionList)
                {
                    foreach (var rule in opt.RuleList)
                    {
                        {
                            switch (rule.RuleTypes.Name.ToLower())
                            {
                                case "document":
                                    docs = rule.RuleValue.ToString().Split(',');
                                    bool isValid;
                                    bool isReqd;
                                    for (int i = 0; i < docs.Length; i++)
                                    {
                                        if (!String.IsNullOrEmpty(docs[i]))
                                        {
                                            string docName = docs[i];
                                            isReqd = true;
                                            isValid = false;
                                            if (Documents != null)
                                            {
                                                doclist = (from p in Documents where p.DocType.ToString() == docName select p).ToList<Document>();
                                                if (doclist.Count > 0)
                                                {
                                                    isValid = true;
                                                }
                                            }

                                            AddReqdDocs(docName, isValid);
                                            if (AllDocuments.ContainsKey(docName))
                                            {
                                                if (!isValid)
                                                {
                                                    //                                                    description = (from p in view.Categories where p.DocCatId.ToString() == docName select p.CategoryDescription).Single();
                                                    description = (from p in viewCats where p.DocCatId.ToString() == docName select p.CategoryDescription).Single();
                                                    AddValidationItem(new ValidationItem("Documents", docName, description + "  document not found."));
                                                }
                                                AllDocuments[docName] = isReqd;
                                            }
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
        }

        public virtual void RemoveAllOtherICDCodes()
        {
            ICDList.Clear();
        }

        public virtual void RemoveOtherICDCode(int icdCodeId)
        {
            RestrictedSARCOtherICDCode otherICDCodeToRemove = null;

            foreach (RestrictedSARCOtherICDCode c in ICDList)
            {
                if (c.ICDCode.Id == icdCodeId)
                {
                    otherICDCodeToRemove = c;
                    break;
                }
            }

            if (otherICDCodeToRemove == null)
                return;

            ICDList.Remove(otherICDCodeToRemove);
        }

        public virtual void RemoveSignature(IDaoFactory daoFactory, int statusOut)
        {
            ISignatueMetaDateDao sigDao = daoFactory.GetSigMetaDataDao();
            sigDao.DeleteForWorkStatus(Id, Workflow, statusOut);
        }

        public virtual void UpdateIsPostProcessingComplete(IDaoFactory daoFactory)
        {
            IsPostProcessingComplete = DetermineIfPostProcessingIsComplete(daoFactory);
        }

        public virtual void Validate()
        {
            //Since findings are stored in a seperate table
            foreach (var opt in WorkflowStatus.WorkStatusOptionList)
            {
                foreach (var rule in opt.RuleList)
                {
                    ;  // No sub modules to validate
                }
            }
        }

        protected virtual bool HasDeterminationMemoBeenGenerated(IDaoFactory daoFactory)
        {
            IMemoDao2 memoDao = daoFactory.GetMemoDao2();
            int count = 0;

            if (FinalFindings == (short)Finding.In_Line_Of_Duty)
            {
                count = memoDao.GetByRefnModule(Id, (short)ModuleType.SARC).Where(x => x.Deleted == false && x.Template.Id == (int)MemoType.SARC_Determination_ILOD).ToList().Count;
            }
            else if (FinalFindings == (byte)ALOD.Core.Utils.Finding.Nlod_Not_Due_To_OwnMisconduct)
            {
                count = memoDao.GetByRefnModule(Id, (short)ModuleType.SARC).Where(x => x.Deleted == false && x.Template.Id == (int)MemoType.SARC_Determination_NILOD).ToList().Count;
            }

            if (count > 0)
                return true;
            else
                return false;
        }

        protected virtual bool HasMemberBeenNotified(IDaoFactory daoFactory)
        {
            ISARCPostProcessingDao postProcessingDao = daoFactory.GetSARCPostProcessingDao();
            RestrictedSARCPostProcessing postProcessing = postProcessingDao.GetById(Id);

            if (postProcessing != null && postProcessing.MemberNotified.HasValue)
                return postProcessing.MemberNotified.Value;

            return false;
        }

        protected virtual bool HasPostCompletionDigitalSignatureBeenGenerated(IDaoFactory daoFactory)
        {
            ISignatueMetaDateDao sigMetaDataDao = daoFactory.GetSigMetaDataDao();

            if (sigMetaDataDao.GetByWorkStatus(Id, Workflow, (int)SARCRestrictedWorkStatus.SARCComplete) == null)
                return false;

            return true;
        }

        private void AddModuleStatus(string section, bool isValid)
        {
            if (!ModuleStatus.ContainsKey(section))
                ModuleStatus.Add(section, isValid);
        }

        private void AddReqdDocs(string section, bool isValid)
        {
            if (!Required.ContainsKey(section))
                Required.Add(section, isValid);
        }

        private void AddValidationItem(IList<ValidationItem> itemsList)
        {
            foreach (var item in itemsList)
            {
                if (!Validations.Contains(item))
                    AddValidationItem(item);
            }
        }

        private void AddValidationItem(ValidationItem item)
        {
            IList<ValidationItem> lst = (from p in Validations where p.Section == item.Section && p.Field == item.Field select p).ToList<ValidationItem>();
            if (lst.Count == 0)
            {
                Validations.Add(item);
            }
        }

        private void ApplyRulesToOption(WorkflowStatusOption o, WorkflowOptionRule r, int lastStatus, IDaoFactory daoFactory)
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

                    case "isrequestforconsultto":
                        o.OptionVisible = Visbility_IsRequestForConsultTo(r.RuleValue);
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
                                IList<Memorandum2> memolist = (from m in memoDao.GetByRefnModule(Id, (short)ModuleType.SARC) where m.Deleted == false && m.Template.Id == Convert.ToByte(memos[i]) select m).ToList<Memorandum2>();
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

                    case "canforwardcase":
                        bool canforward = Validate_CanForwardCase(lookupDao);

                        if (canforward != true)
                        {
                            o.OptionValid = false;
                        }
                        else
                        {
                            // o.OptionValid = true && DutyStatus.Value != 5;
                            o.OptionValid = true;
                        }

                        break;

                    case "cancomplete":
                        bool cancomplete = Validate_CanCompleteCase(lookupDao);

                        if (cancomplete != true)
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

        private void ProcessOption(int lastStatus, IDaoFactory daoFactory)
        {
            var options = from o in WorkflowStatus.WorkStatusOptionList select o;

            //first, apply the visibility rules
            foreach (var opt in options)
            {
                bool optVisible = true;

                var rules = from r in opt.RuleList
                            where r.RuleTypes.ruleTypeId == (int)RuleKind.Visibility
                            select r;

                foreach (var visible in rules)
                {
                    ApplyRulesToOption(opt, visible, lastStatus, daoFactory);

                    if (!opt.OptionVisible)
                    {
                        optVisible = false;
                    }
                }

                if (optVisible)
                {
                    RuleAppliedOptions.Add(opt);
                }
            }

            //now that we know which options are still visible, apply the validations to them
            options = from o in RuleAppliedOptions select o;

            foreach (var opt in options)
            {
                bool optValid = true;

                //now apply the validation rules
                var rules = from r in opt.RuleList
                            where r.RuleTypes.ruleTypeId == (int)RuleKind.Validation
                            select r;

                foreach (var validation in rules)
                {
                    ApplyRulesToOption(opt, validation, lastStatus, daoFactory);

                    if (!opt.OptionValid)
                    {
                        optValid = false;
                    }
                }

                opt.OptionValid = optValid;
            }
        }

        #region Routing Validation

        protected bool Validate_CanCompleteCase(ILookupDao lookupDao)
        {
            bool canComplete = true;

            if (WorkflowStatus.Id == (int)SARCRestrictedWorkStatus.SARCAdminReview)
            {
                RestrictedSARCFindings approvingAuthFindings = FindByType((short)PersonnelTypes.BOARD_AA);

                if (approvingAuthFindings == null || !approvingAuthFindings.Finding.HasValue || approvingAuthFindings.Finding.Value == (short)Finding.Request_Consultation)
                {
                    canComplete = false;
                    AddValidationItem(new ValidationItem("SARC Administrator", "Approving Authority Findings", "The Approval Authority must make a final determination before the case can be closed."));
                }

                if (!DoesFindingExist((short)PersonnelTypes.SARC_ADMIN) || String.IsNullOrEmpty(FindByType((short)PersonnelTypes.SARC_ADMIN).Remarks))
                {
                    canComplete = false;
                    AddValidationItem(new ValidationItem("SARC Administrator", "SARC Admin Remarks", "Remarks are required to close this case."));
                }
            }
            else if (WorkflowStatus.Id == (int)SARCRestrictedWorkStatus.SARCInitiate)
            {
                canComplete = Validate_WingSARC_Properties();

                if (DutyStatus.HasValue && DutyStatus.Value != ROTC_CADET_MEMBER_STATUS)
                {
                    canComplete = false;
                }
            }

            return canComplete;
        }

        protected bool Validate_CanForwardCase(ILookupDao lookupDao)
        {
            bool canForward = true;

            if (WorkflowStatus.Id == (int)SARCRestrictedWorkStatus.SARCInitiate)
            {
                canForward = Validate_WingSARC_Properties();

                if (DutyStatus.HasValue && DutyStatus.Value == ROTC_CADET_MEMBER_STATUS)
                {
                    canForward = false;
                }
            }
            else if (WorkflowStatus.Id == (int)SARCRestrictedWorkStatus.SARCBoardJAReview)
            {
                if (!DoesFindingExist((short)PersonnelTypes.BOARD_JA) || String.IsNullOrEmpty(FindByType((short)PersonnelTypes.BOARD_JA).Remarks))
                {
                    canForward = false;
                    AddValidationItem(new ValidationItem("Board JA Review", "Board JA Remarks", "Please include your remarks for this case."));
                }
            }
            else if (WorkflowStatus.Id == (int)SARCRestrictedWorkStatus.SARCBoardMedicalReview)
            {
                if (!DoesFindingExist((short)PersonnelTypes.BOARD_SG) || String.IsNullOrEmpty(FindByType((short)PersonnelTypes.BOARD_SG).Remarks))
                {
                    canForward = false;
                    AddValidationItem(new ValidationItem("Board Medical", "Borad Medical Remarks", "Please include your remarks for this case."));
                }
            }
            else if (WorkflowStatus.Id == (int)SARCRestrictedWorkStatus.SARCSeniorMedicalReview)
            {
                if (!DoesFindingExist((short)PersonnelTypes.SENIOR_MEDICAL_REVIEWER) || String.IsNullOrEmpty(FindByType((short)PersonnelTypes.SENIOR_MEDICAL_REVIEWER).Remarks))
                {
                    canForward = false;
                    AddValidationItem(new ValidationItem("Senior Medical", "Senior Medical Remarks", "Please include your remarks for this case."));
                }
            }
            else if (WorkflowStatus.Id == (int)SARCRestrictedWorkStatus.SARCBoardAdminReview)
            {
                if (!DoesFindingExist((short)PersonnelTypes.BOARD_A1) || String.IsNullOrEmpty(FindByType((short)PersonnelTypes.BOARD_A1).Remarks))
                {
                    canForward = false;
                    AddValidationItem(new ValidationItem("Board Medical", "Borad Administrator Remarks", "Please include your remarks for this case."));
                }
            }
            else if (WorkflowStatus.Id == (int)SARCRestrictedWorkStatus.SARCApprovingAuthorityReview)
            {
                if (!DoesFindingExist((short)PersonnelTypes.BOARD_AA) || String.IsNullOrEmpty(FindByType((short)PersonnelTypes.BOARD_AA).Remarks))
                {
                    canForward = false;
                    AddValidationItem(new ValidationItem("Approving Authority", "Approving Authority Remarks", "Please include your remarks for this case."));
                }
            }
            else if (WorkflowStatus.Id == (int)SARCRestrictedWorkStatus.SARCAdminReview)
            {
                if (!DoesFindingExist((short)PersonnelTypes.SARC_ADMIN) || String.IsNullOrEmpty(FindByType((short)PersonnelTypes.SARC_ADMIN).Remarks))
                {
                    canForward = false;
                    AddValidationItem(new ValidationItem("SARC Administrator", "SARC Administrator Remarks", "Remarks are required to forward this case."));
                }
            }

            return canForward;
        }

        protected bool Validate_WingSARC_Properties()
        {
            bool allPropertiesValid = true;

            if (IncidentDate == null)
            {
                allPropertiesValid = false;
                AddValidationItem(new ValidationItem("Wing SARC", "Date of Incident", "Must enter the Date of Incident for the SARC case."));
            }

            if (String.IsNullOrEmpty(DefenseSexualAssaultDBCaseNumber))
            {
                allPropertiesValid = false;
                AddValidationItem(new ValidationItem("Wing SARC", "Case Number", "Must enter Database Case Number."));
            }

            if (!DutyStatus.HasValue)
            {
                allPropertiesValid = false;
                AddValidationItem(new ValidationItem("Wing SARC", "Duty Status", "Please select Memebership and Duty Status"));
            }

            if (DurationStart == null)
            {
                allPropertiesValid = false;
                AddValidationItem(new ValidationItem("Wing SARC", "IDT Start", "Must enter Duration of Order or IDT Start Date"));
            }

            if (DurationEnd == null)
            {
                allPropertiesValid = false;
                AddValidationItem(new ValidationItem("Wing SARC", "IDT End", "Must enter Duration of Order or IDT End Date"));
            }

            if (ICDE968.Value == false && ICDE969.Value == false && ICDOther.Value == false)
            {
                allPropertiesValid = false;
                AddValidationItem(new ValidationItem("Wing SARC", "ICD Code", "Please select/add at least one ICD Code"));
            }

            if (ICDOther.Value == true && (ICDList == null || ICDList.Count == 0))
            {
                allPropertiesValid = false;
                AddValidationItem(new ValidationItem("Wing SARC", "ICD Code", "Please select/add at least one Other ICD Code"));
            }

            if (!InDutyStatus.HasValue)
            {
                allPropertiesValid = false;
                AddValidationItem(new ValidationItem("Wing SARC", "Duty Status Dermination", "Please indicate Duty Status Dermination"));
            }

            return allPropertiesValid;
        }

        protected bool Visbility_IsRequestForConsultTo(string ruleData)
        {
            if (!ConsultationFromUserGroupId.HasValue || ConsultationFromUserGroupId.Value <= 0)
                return false;

            int targetGroupId = 0;

            if (int.TryParse(ruleData, out targetGroupId) && ConsultationFromUserGroupId.Value == targetGroupId)
                return true;

            return false;
        }

        #endregion Routing Validation

        private void UpdateActiveCategories(string section, bool isReqd)
        {
            if (AllDocuments.ContainsKey(section))
                AllDocuments[section] = isReqd;
            if (!AllDocuments.ContainsKey(section))
                AllDocuments.Add(section, isReqd);
        }

        private void ValidateModule(string section, IValidatable item, int userid)
        {
            bool isValid = true;
            isValid = item.Validate(userid);
            AddValidationItem(item.ValidationItems);
            AddModuleStatus(section, isValid);
        }

        #region MyAccess

        public virtual Dictionary<String, PageAccessType> ReadSectionList(int role)
        {
            PageAccessType access = PageAccessType.None;

            Dictionary<String, PageAccessType> scAccessList = new Dictionary<String, PageAccessType>();

            if ((Status == (int)SARCRestrictedWorkStatus.SARCComplete) || (Status == (int)SARCRestrictedWorkStatus.SARCCancelled))
            {
                access = PageAccessType.ReadOnly;
            }

            scAccessList.Add(SARCSectionNames.SARC_INIT.ToString(), access);
            scAccessList.Add(SARCSectionNames.SARC_ADMIN.ToString(), access);
            scAccessList.Add(SARCSectionNames.SARC_BOARD_MED_REV.ToString(), access);
            scAccessList.Add(SARCSectionNames.SARC_SENIOR_MED_REV.ToString(), access);
            scAccessList.Add(SARCSectionNames.SARC_BOARD_JA_REV.ToString(), access);
            scAccessList.Add(SARCSectionNames.SARC_BOARD_APPROVING_AUTH_REV.ToString(), access);
            scAccessList.Add(SARCSectionNames.SARC_BOARD_ADMIN_REV.ToString(), access);
            scAccessList.Add(SARCSectionNames.SARC_COMPELTED.ToString(), access);
            scAccessList.Add(SARCSectionNames.SARC_CANCELLED.ToString(), access);
            scAccessList.Add(SARCSectionNames.SARC_RLB.ToString(), access);

            switch (role)
            {
                case (int)UserGroups.WingSarc:
                    if (Status == (int)SARCRestrictedWorkStatus.SARCInitiate)
                    {
                        scAccessList[SARCSectionNames.SARC_INIT.ToString()] = PageAccessType.ReadWrite;
                        scAccessList[SARCSectionNames.SARC_RLB.ToString()] = PageAccessType.ReadWrite;
                    }
                    break;

                case (int)UserGroups.RSL:
                    if (Status == (int)SARCRestrictedWorkStatus.SARCInitiate)
                    {
                        scAccessList[SARCSectionNames.SARC_INIT.ToString()] = PageAccessType.ReadWrite;
                    }

                    break;

                case (int)UserGroups.SARCAdmin:
                    if (Status == (int)SARCRestrictedWorkStatus.SARCAdminReview)
                    {
                        scAccessList[SARCSectionNames.SARC_ADMIN.ToString()] = PageAccessType.ReadWrite;
                        scAccessList[SARCSectionNames.SARC_RLB.ToString()] = PageAccessType.ReadOnly;
                    }
                    break;

                case (int)UserGroups.BoardMedical:
                    if (Status == (int)SARCRestrictedWorkStatus.SARCBoardMedicalReview)
                    {
                        scAccessList[SARCSectionNames.SARC_BOARD_MED_REV.ToString()] = PageAccessType.ReadWrite;
                    }
                    break;

                case (int)UserGroups.SeniorMedicalReviewer:
                    if (Status == (int)SARCRestrictedWorkStatus.SARCSeniorMedicalReview)
                    {
                        scAccessList[SARCSectionNames.SARC_SENIOR_MED_REV.ToString()] = PageAccessType.ReadWrite;
                    }
                    break;

                case (int)UserGroups.BoardLegal:
                    if (Status == (int)SARCRestrictedWorkStatus.SARCBoardJAReview)
                    {
                        scAccessList[SARCSectionNames.SARC_BOARD_JA_REV.ToString()] = PageAccessType.ReadWrite;
                    }
                    break;

                case (int)UserGroups.BoardApprovalAuthority:
                    if (Status == (int)SARCRestrictedWorkStatus.SARCApprovingAuthorityReview)
                    {
                        scAccessList[SARCSectionNames.SARC_BOARD_APPROVING_AUTH_REV.ToString()] = PageAccessType.ReadWrite;
                    }
                    break;

                case (int)UserGroups.BoardAdministrator:
                    if (Status == (int)SARCRestrictedWorkStatus.SARCBoardAdminReview)
                    {
                        scAccessList[SARCSectionNames.SARC_BOARD_ADMIN_REV.ToString()] = PageAccessType.ReadWrite;
                    }
                    break;
            }

            return scAccessList;
        }

        #endregion MyAccess
    }
}