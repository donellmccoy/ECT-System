using ALOD.Core.Domain.Common;
using ALOD.Core.Domain.Documents;
using ALOD.Core.Domain.Lookup;
using ALOD.Core.Domain.Modules.Common;
using ALOD.Core.Domain.Modules.Lod;
using ALOD.Core.Domain.Users;
using ALOD.Core.Domain.Workflow;
using ALOD.Core.Interfaces;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Core.Utils;
using ALOD.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ALOD.Core.Domain.Modules.Appeals
{
    [Serializable]
    public class LODAppeal : Entity
    {
        private IDictionary<string, bool> _catAllDocs;
        private IDictionary<string, bool> _catRequired;
        private IList<WorkflowStatusOption> _ruleAppliedOptions;
        private IList<ValidationItem> _validations;

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

        public virtual DateTime? Cancel_Date { get; set; }
        public virtual String Cancel_Explanation { get; set; }
        public virtual int? Cancel_Reason { get; set; }

        //case data
        public virtual string CaseId { get; set; }

        public virtual int CreatedBy { get; set; }
        public virtual DateTime? CreatedDate { get; set; }

        public virtual int CurrentStatusCode
        {
            get { return WorkflowStatus.StatusCodeType.Id; }
        }

        // Doucments
        public virtual IDocumentDao DocumentDao { get; set; }

        public virtual IList<Document> Documents { get; set; }
        public virtual IList<LODAppealFindings> Findings { get; set; }
        public virtual int InitialLodId { get; set; }
        public virtual bool IsNonDBSignCase { get; set; }
        public virtual bool IsPostProcessingComplete { get; set; }
        public virtual string MemberCompo { get; set; }

        public virtual string MemberGrade
        { get { return MemberRank.Title; } }

        public virtual String MemberName { get; set; }
        public virtual bool MemberNotified { get; set; }
        public virtual UserRank MemberRank { get; set; }
        public virtual String MemberSSN { get; set; }
        public virtual string MemberUnit { get; set; }
        public virtual int MemberUnitId { get; set; }
        public virtual int ModifiedBy { get; set; }
        public virtual DateTime? ModifiedDate { get; set; }

        public virtual byte ModuleId
        {
            get { return (byte)ModuleType.AppealRequest; }
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

        //Signatures
        //public virtual SignatureEntry LODProgramManagerSignature { get; set; }
        //public virtual SignatureEntry BoardTechSignature { get; set; }
        //public virtual SignatureEntry BoardMedicalSignature { get; set; }
        //public virtual SignatureEntry BoardLegalSignature { get; set; }
        //public virtual SignatureEntry BoardAdminSignature { get; set; }
        //public virtual SignatureEntry ApprovingAuthoritySignature { get; set; }
        //public virtual SignatureEntry AppellateAuthoritySignature { get; set; }
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
        public virtual Byte? RWOA_Reason { get; set; }
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

        #region Constructor

        public LODAppeal()
        {
            AllDocuments.Add(DocumentType.SupportingAppealDocuments.ToString(), false);
            AllDocuments.Add(DocumentType.Miscellaneous.ToString(), false);

            //LODProgramManagerSignature = new SignatureEntry();
            //BoardTechSignature = new SignatureEntry();
            //BoardMedicalSignature = new SignatureEntry();
            //BoardLegalSignature = new SignatureEntry();
            //BoardAdminSignature = new SignatureEntry();
            //ApprovingAuthoritySignature = new SignatureEntry();
            //AppellateAuthoritySignature = new SignatureEntry();
            MemberRank = new UserRank();
        }

        #endregion Constructor

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

        public virtual bool CreateDocumentGroup(IDocumentDao dao)
        {
            Check.Require(dao != null, "DocumentDao is required");

            DocumentGroupId = dao.CreateGroup();
            return DocumentGroupId > 0;
        }

        public virtual IEnumerable<WorkflowStatusOption> GetCurrentOptions(int lastStatus, IDaoFactory daoFactory)
        {
            Validations.Clear();
            ProcessDocuments(daoFactory);
            ProcessOption(lastStatus, daoFactory);
            return RuleAppliedOptions;
        }

        public virtual Memorandum GetDeterminationMemorandum(AppUser appellateAuthorityUser, IDigitalSignatureService sigService, IDaoFactory daoFactory)
        {
            if (!CanGenerateDeterminationMemorandum())
                return null;

            MemoTemplate template = GetMemorandumTemplate(daoFactory.GetMemoDao());

            if (template == null)
                return null;

            if (!MemoHelpers.DoesUserHaveCreatePermissionForTemplate(appellateAuthorityUser, template))
                return null;

            return GenerateMemorandum(template, appellateAuthorityUser, sigService, daoFactory);
        }

        public virtual void InitiateCase(PetitionWorkflowInitiateCaseArgs args)
        {
            AppealInitiateCaseArgs appealArgs = (AppealInitiateCaseArgs)args;
            InitialLodId = appealArgs.OriginalCaseRefId;
            CaseId = appealArgs.GetNextCaseId();
            Workflow = (int)AFRCWorkflows.AppealRequest;
            MemberSSN = appealArgs.Member.SSN;
            MemberName = appealArgs.Member.FullName.ToUpper();
            MemberUnit = appealArgs.Member.Unit.Name;
            MemberUnitId = appealArgs.Member.Unit.Id;
            MemberCompo = appealArgs.Member.Component;
            MemberRank.SetId(appealArgs.Member.Rank.Id);
            Status = appealArgs.InitialWorkStatus;
            CreatedBy = appealArgs.CreatedByUserId;
            CreatedDate = DateTime.Now;
            CreateDocumentGroup(appealArgs.DocumentDao);
        }

        #region Findings

        public virtual short FinalFinding
        {
            get
            {
                if (!(WorkflowStatus.StatusCodeType.IsFinal && !WorkflowStatus.StatusCodeType.IsCancel))
                    return 0;

                LODAppealFindings finalFinding = FindFindingByPersonnelType((short)PersonnelTypes.APPELLATE_AUTH);

                if (finalFinding == null)
                    return 0;

                return finalFinding.Finding.Value;
            }
        }

        public virtual LODAppealFindings CreateEmptyFindingForUser(AppUser user)
        {
            LODAppealFindings cFinding = new LODAppealFindings();

            cFinding.RefId = Id;
            cFinding.Compo = user.Component;
            cFinding.PasCode = user.Unit.PasCode;
            cFinding.Rank = user.Rank.Rank;
            cFinding.Grade = user.Rank.Grade;
            cFinding.LastName = user.LastName;
            cFinding.FirstName = user.FirstName;
            cFinding.MiddleName = user.MiddleName;
            cFinding.IsLegacyFinding = false;
            cFinding.ModifiedBy = user.Id;
            cFinding.ModifiedDate = DateTime.Now;
            cFinding.CreatedBy = user.Id;
            cFinding.CreatedDate = DateTime.Now;

            return cFinding;
        }

        public virtual bool DoesFindingExist(short pType)
        {
            if (FindFindingByPersonnelType(pType) == null)
                return false;

            return true;
        }

        public virtual LODAppealFindings FindFindingByPersonnelType(short pType)
        {
            IEnumerable<LODAppealFindings> lst = (from p in Findings where p.PType == pType select p);

            if (lst.Count() > 0)
            {
                return lst.First();
            }

            return null;
        }

        public virtual bool HasAFinding(LODAppealFindings finding)
        {
            if (finding == null)
                return false;

            if (!finding.Finding.HasValue || finding.Finding.Value == 0)
                return false;

            return true;
        }

        public virtual LODAppealFindings SetFindingByType(LODAppealFindings fnd)
        {
            int i = 0;

            if (Findings.Count > 0)
            {
                foreach (LODAppealFindings item in Findings)
                {
                    if (item.PType == fnd.PType)
                    {
                        Findings.ElementAt(i).Modify(fnd);
                        return Findings.ElementAt(i);
                    }
                    i = i + 1;
                }
            }

            Findings.Add(fnd);

            return Findings.ElementAt(i);
        }

        public virtual LODAppealFindings SetPersonalByType(LODAppealFindings fnd)
        {
            int i = 0;

            if (Findings.Count > 0)
            {
                foreach (LODAppealFindings item in Findings)
                {
                    if (item.PType == fnd.PType)
                    {
                        Findings.ElementAt(i).ModifyPersonnel(fnd);
                        return Findings.ElementAt(i);
                    }
                    i = i + 1;
                }
            }

            Findings.Add(fnd);

            return Findings.ElementAt(i);
        }

        #endregion Findings

        #region IDocumentSource Members

        public virtual string DocumentEntityId
        {
            get { return MemberSSN; }
        }

        public virtual long? DocumentGroupId { get; set; }

        public virtual int DocumentViewId
        {
            get { return (int)DocumentViewType.AppealRequest; }
        }

        #endregion IDocumentSource Members

        /// <summary>
        /// This method performs a basic override of the appeal case. It checks if the case is being overriden to or from a
        /// cancel status and sets or clears the appropriate cancel related fields. It then simply assigns the new status ID to
        /// the Status property and assigns a new WorkStatus object to the WorkflowStatus property. If special considerations need
        /// to be taken into account for overriding a specific appeal case, then the class for that appeal case can override
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

                UpdateCancelProperties(oldStatus, newStatus);
                DeleteExistingDeterminationMemorandums(oldStatus, newStatus, daoFactory);

                reminderEmailDao.ReminderEmailUpdateStatusChange(oldStatusId, newStatusId, this.CaseId, "AP");

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

        public virtual void RemoveSignature(IDaoFactory daoFactory, int statusOut)
        {
            ISignatueMetaDateDao sigDao = daoFactory.GetSigMetaDataDao();
            sigDao.DeleteForWorkStatus(Id, Workflow, statusOut);
        }

        public virtual void UpdateIsPostProcessingComplete(IDaoFactory daoFactory)
        {
            IsPostProcessingComplete = DetermineIfPostProcessingIsComplete(daoFactory);
        }

        protected virtual bool CanGenerateDeterminationMemorandum()
        {
            if (CurrentStatusCode != (int)AppealStatusCode.AppealApproved && CurrentStatusCode != (int)AppealStatusCode.AppealDenied)
                return false;

            if (!DoesFindingExist((short)PersonnelTypes.APPELLATE_AUTH))
                return false;

            return true;
        }

        protected virtual bool CanGenerateSignatureBlock(IDaoFactory daoFactory)
        {
            LODAppealFindings appellateAuthorityFindings = FindFindingByPersonnelType((short)PersonnelTypes.APPELLATE_AUTH);

            if (appellateAuthorityFindings == null)
            {
                LogManager.LogError("Error in LODAppeal.CanGenerateSignatureBlock(): Could not find Appellate Authority Findings for case " + CaseId + " (" + Id + ").");
                return false;
            }

            return true;
        }

        protected virtual void DeleteExistingDeterminationMemorandums(WorkStatus oldStatus, WorkStatus newStatus, IDaoFactory daoFactory)
        {
            if (oldStatus.StatusCodeType.IsFinal == true && !newStatus.StatusCodeType.IsFinal)
            {
                IMemoDao memoDao = daoFactory.GetMemoDao();

                foreach (Memorandum m in GetExistingDeterminationMemorandums(memoDao))
                {
                    m.Deleted = true;
                    memoDao.SaveOrUpdate(m);
                }
            }
        }

        protected virtual bool DetermineIfPostProcessingIsComplete(IDaoFactory daoFactory)
        {
            if (!WorkflowStatus.StatusCodeType.IsFinal)
                return false;

            if (WorkflowStatus.StatusCodeType.IsCancel)
                return true;

            if (MemberNotified && HasPostCompletionDigitalSignatureBeenGenerated(daoFactory))
                return true;

            return false;
        }

        protected virtual Memorandum GenerateMemorandum(MemoTemplate template, AppUser appellateAuthorityUser, IDigitalSignatureService sigService, IDaoFactory daoFactory)
        {
            Memorandum memo = new Memorandum();

            memo.Template = template;
            memo.Letterhead = daoFactory.GetMemoDao().GetEffectiveLetterHead(appellateAuthorityUser.Component);
            memo.CreatedBy = appellateAuthorityUser;
            memo.CreatedDate = DateTime.Now;
            memo.Deleted = false;
            memo.ReferenceId = InitialLodId;
            memo.AddContent(GenerateMemorandumContent(template, appellateAuthorityUser, sigService, daoFactory));

            return memo;
        }

        protected virtual MemoContent GenerateMemorandumContent(MemoTemplate template, AppUser appellateAuthorityUser, IDigitalSignatureService sigService, IDaoFactory daoFactory)
        {
            MemoContent content = new MemoContent();

            content.Body = template.PopulatedBody(daoFactory.GetMemoDao().GetMemoData(Id, template.DataSource));
            content.SignatureBlock = GetMemoSignatureBlock(appellateAuthorityUser, sigService, daoFactory);
            content.CreatedBy = appellateAuthorityUser;
            content.CreatedDate = DateTime.Now;

            if (template.AddDate)
                content.MemoDate = DateTime.Now.ToString(template.DateFormat).Trim();
            else
                content.MemoDate = string.Empty;

            return content;
        }

        protected virtual IList<Memorandum> GetExistingDeterminationMemorandums(IMemoDao memoDao)
        {
            return memoDao.GetByRefId(InitialLodId).Where(x => x.Template.Id == (int)MemoType.ApprovalAppeal || x.Template.Id == (int)MemoType.DisapprovalAppeal).OrderByDescending(x => x.CreatedDate).ToList();
        }

        protected virtual MemoTemplate GetMemorandumTemplate(IMemoDao memoDao)
        {
            if (!DoesFindingExist((short)PersonnelTypes.APPELLATE_AUTH))
                return null;

            if (FinalFinding == (short)Finding.Approve)
                return memoDao.GetTemplateById((int)MemoType.ApprovalAppeal);

            if (FinalFinding == (short)Finding.Disapprove)
                return memoDao.GetTemplateById((int)MemoType.DisapprovalAppeal);

            return null;
        }

        protected virtual string GetMemoSignatureBlock(AppUser appellateAuthorityUser, IDigitalSignatureService sigService, IDaoFactory daoFactory)
        {
            if (!CanGenerateSignatureBlock(daoFactory))
            {
                return "ERROR GENERATING SIGNATURE BLOCK" + Environment.NewLine + "Please contact the ECT Help Desk for assistance";
            }

            string memoSignature = MemoHelpers.GetMemoFormattedSignature(sigService) + Environment.NewLine + Environment.NewLine;
            memoSignature += appellateAuthorityUser.AlternateSignatureName + ", USAF" + Environment.NewLine + "Appellate Authority";

            return memoSignature;
        }

        protected virtual bool HasPostCompletionDigitalSignatureBeenGenerated(IDaoFactory daoFactory)
        {
            ISignatueMetaDateDao sigMetaDataDao = daoFactory.GetSigMetaDataDao();

            if (sigMetaDataDao.GetByWorkStatus(Id, Workflow, (int)AppealWorkStatus.AppealApproved) == null &&
                sigMetaDataDao.GetByWorkStatus(Id, Workflow, (int)AppealWorkStatus.AppealDenied) == null)
                return false;

            return true;
        }

        protected virtual void UpdateCancelProperties(WorkStatus oldStatus, WorkStatus newStatus)
        {
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
                Cancel_Reason = 8;   // The value 8 is currently arbitrary...it does not map to an enum value or a lookup table in the database.
                                     // Currently the NextAction code behind checks for the value 8. Unsure as to why 8 was chosen.
                Cancel_Explanation = "Case Cancelled by System Admin";
                Cancel_Date = DateTime.Now;
            }
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
            IMemoDao memoDao = daoFactory.GetMemoDao();
            ILookupDao lookupDao = daoFactory.GetLookupDao();

            //last status should be the current
            string[] statuses;

            bool allExist;
            bool oneExist;
            bool isVisible;
            IList<WorkStatusTracking> trackingData;

            string decision;

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
                        statuses = r.RuleValue.ToString().Split(',');
                        if (statuses.Contains(lastStatus.ToString()))
                        {
                            o.OptionVisible = false;
                        }
                        break;

                    case "appellatedecision":

                        decision = o.DisplayText;

                        if (!DoesFindingExist((short)PersonnelTypes.APPELLATE_AUTH))
                        {
                            o.OptionVisible = true;
                        }
                        else
                        {
                            if (String.Equals(o.DisplayText, "Approved") && FinalFinding == (short)Finding.Disapprove)
                            {
                                o.OptionVisible = false;
                            }
                            if (String.Equals(o.DisplayText, "Denied") && FinalFinding == (short)Finding.Approve)
                            {
                                o.OptionVisible = false;
                            }
                        }

                        break;

                    case "wasinstatus":
                        isVisible = false;
                        statuses = r.RuleValue.ToString().Split(',');

                        trackingData = lookupDao.GetStatusTracking(this.Id, this.ModuleId);

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

                    case "wasnotinstatus":
                        isVisible = false;
                        statuses = r.RuleValue.ToString().Split(',');

                        trackingData = lookupDao.GetStatusTracking(this.Id, this.ModuleId);

                        if (trackingData == null)
                        {
                            o.OptionVisible = false;
                            break; // Breaks out of the switch statement
                        }

                        if (trackingData.Count == 0)
                        {
                            o.OptionVisible = true;
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

                        o.OptionVisible = !isVisible;

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
                                IList<Memorandum> memolist = (from m in memoDao.GetByRefId(Id) where m.Deleted == false && m.Template.Id == Convert.ToByte(memos[i]) select m).ToList<Memorandum>();
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
                            o.OptionValid = true;
                        }

                        break;

                    case "appellateauthority":
                        bool canAppeal = Validate_AppellateAuthority(lookupDao, true);

                        if (canAppeal != true)
                        {
                            o.OptionValid = false;
                        }
                        else
                        {
                            o.OptionValid = true;
                        }

                        break;

                    case "boardcheck":
                        canAppeal = Validate_AppellateAuthority(lookupDao, false);

                        break;
                }
            }
        }

        private LODAppealFindings FindByType(short bOARD_SG)
        {
            throw new NotImplementedException();
        }

        private void ProcessOption(int lastStatus, IDaoFactory daoFactory)
        {
            IMemoDao memoDao = daoFactory.GetMemoDao();
            ILookupDao lookupDao = daoFactory.GetLookupDao();

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

        protected bool Validate_AppellateAuthority(ILookupDao lookupDao, Boolean validateAppellateAuthority)
        {
            bool canForward = true;

            if (!Validate_FindingsForPersonnelType(PersonnelTypes.BOARD_SG, "Board Medical"))
            {
                canForward = false;
            }

            if (!Validate_FindingsForPersonnelType(PersonnelTypes.BOARD_JA, "Board Legal"))
            {
                canForward = false;
            }

            if (!Validate_FindingsForPersonnelType(PersonnelTypes.BOARD_A1, "Board Administrator"))
            {
                canForward = false;
            }

            if (!Validate_FindingsForPersonnelType(PersonnelTypes.BOARD_AA, "Approving Authority"))
            {
                canForward = false;
            }

            if (!validateAppellateAuthority)
            {
                if (!Validate_FindingsForPersonnelType(PersonnelTypes.APPELLATE_AUTH, "Appellate Authority"))
                {
                    canForward = false;
                }
            }

            return canForward;
        }

        protected bool Validate_CanForwardCase(ILookupDao lookupDao)
        {
            switch (CurrentStatusCode)
            {
                case (int)AppealStatusCode.AppealBoardMedicalReview:
                    return Validate_FindingsForPersonnelType(PersonnelTypes.BOARD_SG, "Board Medical");

                case (int)AppealStatusCode.AppealBoardLegalReview:
                    return Validate_FindingsForPersonnelType(PersonnelTypes.BOARD_JA, "Board Legal");

                case (int)AppealStatusCode.AppealBoardAdminReview:
                    return Validate_FindingsForPersonnelType(PersonnelTypes.BOARD_A1, "Board Administrator");

                case (int)AppealStatusCode.AppealApprovingAuthorityReview:
                    return Validate_FindingsForPersonnelType(PersonnelTypes.BOARD_AA, "Approving Authority");

                case (int)AppealStatusCode.AppealAppellateAuthorityReview:
                    return Validate_FindingsForPersonnelType(PersonnelTypes.APPELLATE_AUTH, "Appellate Authority");

                default:
                    return true;
            }
        }

        protected bool Validate_FindingsForPersonnelType(PersonnelTypes pType, string userGroupName)
        {
            bool isValid = true;
            LODAppealFindings cFindings = FindFindingByPersonnelType((short)pType);

            if (pType == PersonnelTypes.SENIOR_MEDICAL_REVIEWER)
            {
                LODAppealFindings BoardMedicalFindings = FindFindingByPersonnelType((short)PersonnelTypes.BOARD_SG);

                if (BoardMedicalFindings == null || !BoardMedicalFindings.Finding.HasValue)
                {
                    if (cFindings == null || !cFindings.Finding.HasValue || cFindings.Finding == 0)
                    {
                        isValid = false;
                        AddValidationItem(new ValidationItem(userGroupName, "Determination", "Must provide a determination for the AP case."));
                    }

                    if (cFindings == null || String.IsNullOrEmpty(cFindings.Explanation))
                    {
                        isValid = false;
                        AddValidationItem(new ValidationItem(userGroupName, "Decision Explanation", "Must provide a decision explanation for the AP case."));
                    }
                }
                else
                {
                    if (String.IsNullOrEmpty(cFindings.Concur))
                    {
                        isValid = false;
                        AddValidationItem(new ValidationItem(userGroupName, "Determination", "Must Concur or Non-Concur with the Board Meadical"));
                    }
                    else if (cFindings.Concur.Equals("N"))
                    {
                        if (cFindings == null || !cFindings.Finding.HasValue || cFindings.Finding == 0)
                        {
                            isValid = false;
                            AddValidationItem(new ValidationItem(userGroupName, "Determination", "Must provide a determination for the AP case."));
                        }
                    }

                    if (cFindings == null || String.IsNullOrEmpty(cFindings.Explanation))
                    {
                        isValid = false;
                        AddValidationItem(new ValidationItem(userGroupName, "Decision Explanation", "Must provide a decision explanation for the AP case."));
                    }
                }
            }
            else
            {
                if (cFindings == null || !cFindings.Finding.HasValue)
                {
                    isValid = false;
                    AddValidationItem(new ValidationItem(userGroupName, "Determination", "Must provide a determination for the AP case."));
                }

                if (cFindings == null || String.IsNullOrEmpty(cFindings.Explanation))
                {
                    isValid = false;
                    AddValidationItem(new ValidationItem(userGroupName, "Decision Explanation", "Must provide a decision explanation for the AP case."));
                }
            }

            return isValid;
        }

        #endregion Routing Validation

        private void UpdateActiveCategories(string section, bool isReqd)
        {
            if (AllDocuments.ContainsKey(section))
                AllDocuments[section] = isReqd;
            if (!AllDocuments.ContainsKey(section))
                AllDocuments.Add(section, isReqd);
        }

        #region MyAccess

        public virtual Dictionary<String, PageAccessType> ReadSectionList(int role)
        {
            PageAccessType access;
            access = PageAccessType.None;

            Dictionary<String, PageAccessType> scAccessList = new Dictionary<String, PageAccessType>();
            if ((CurrentStatusCode == (int)AppealStatusCode.AppealApproved) || (CurrentStatusCode == (int)AppealStatusCode.AppealDenied))
            {
                access = PageAccessType.ReadOnly;
            }

            //Add all pages as readonly
            scAccessList.Add(APSectionNames.AP_INIT.ToString(), access);
            scAccessList.Add(APSectionNames.AP_BOARD_TECH_REV.ToString(), access);
            scAccessList.Add(APSectionNames.AP_BOARD_MED_REV.ToString(), access);
            scAccessList.Add(APSectionNames.AP_BOARD_SENIOR_MED_REV.ToString(), access);
            scAccessList.Add(APSectionNames.AP_BOARD_LEGAL_REV.ToString(), access);
            scAccessList.Add(APSectionNames.AP_BOARD_ADMIN_REV.ToString(), access);
            scAccessList.Add(APSectionNames.AP_APPROVING_AUTH_REV.ToString(), access);
            scAccessList.Add(APSectionNames.AP_APPEALLATE_AUTH_REV.ToString(), access);
            scAccessList.Add(APSectionNames.AP_APPROVED.ToString(), access);
            scAccessList.Add(APSectionNames.AP_DENIED.ToString(), access);
            scAccessList.Add(APSectionNames.AP_CANCLED.ToString(), access);

            access = PageAccessType.ReadOnly;
            scAccessList.Add(APSectionNames.AP_RLB.ToString(), access);

            switch (role)
            {
                case (int)UserGroups.LOD_PM:
                    if (CurrentStatusCode == (int)AppealStatusCode.AppealInitiation)
                    {
                        scAccessList[APSectionNames.AP_INIT.ToString()] = PageAccessType.ReadWrite;
                        scAccessList[APSectionNames.AP_RLB.ToString()] = PageAccessType.ReadWrite;
                    }
                    break;

                case (int)UserGroups.BoardTechnician:
                    if (CurrentStatusCode == (int)AppealStatusCode.AppealBoardTechReview)
                    {
                        scAccessList[APSectionNames.AP_BOARD_TECH_REV.ToString()] = PageAccessType.ReadWrite;
                    }
                    break;

                case (int)UserGroups.BoardMedical:
                    if (CurrentStatusCode == (int)AppealStatusCode.AppealBoardMedicalReview)
                    {
                        scAccessList[APSectionNames.AP_BOARD_MED_REV.ToString()] = PageAccessType.ReadWrite;
                    }
                    break;

                case (int)UserGroups.SeniorMedicalReviewer:
                    if (CurrentStatusCode == (int)AppealStatusCode.AppealSeniorMedicalReview)
                    {
                        scAccessList[APSectionNames.AP_BOARD_SENIOR_MED_REV.ToString()] = PageAccessType.ReadWrite;
                    }
                    break;

                case (int)UserGroups.BoardLegal:
                    if (CurrentStatusCode == (int)AppealStatusCode.AppealBoardLegalReview)
                    {
                        scAccessList[APSectionNames.AP_BOARD_LEGAL_REV.ToString()] = PageAccessType.ReadWrite;
                    }
                    break;

                case (int)UserGroups.BoardAdministrator:
                    if (CurrentStatusCode == (int)AppealStatusCode.AppealBoardAdminReview)
                    {
                        scAccessList[APSectionNames.AP_BOARD_ADMIN_REV.ToString()] = PageAccessType.ReadWrite;
                    }
                    break;

                case (int)UserGroups.BoardApprovalAuthority:
                    if (CurrentStatusCode == (int)AppealStatusCode.AppealApprovingAuthorityReview)
                    {
                        scAccessList[APSectionNames.AP_APPROVING_AUTH_REV.ToString()] = PageAccessType.ReadWrite;
                    }
                    break;

                case (int)UserGroups.AppellateAuthority:
                    if (CurrentStatusCode == (int)AppealStatusCode.AppealAppellateAuthorityReview)
                    {
                        scAccessList[APSectionNames.AP_APPEALLATE_AUTH_REV.ToString()] = PageAccessType.ReadWrite;
                    }
                    break;
            }

            return scAccessList;
        }

        #endregion MyAccess
    }
}