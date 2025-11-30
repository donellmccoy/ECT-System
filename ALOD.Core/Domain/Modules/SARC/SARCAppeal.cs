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

namespace ALOD.Core.Domain.Modules.SARC
{
    [Serializable]
    public class SARCAppeal : Entity
    {
        private IDictionary<string, bool> _catAllDocs;
        private IDictionary<string, bool> _catRequired;
        private IList<WorkflowStatusOption> _ruleAppliedOptions;
        private IList<ValidationItem> _validations;
        public virtual IList<SARCAppealFindings> AppealFindings { get; set; }
        public virtual DateTime? Cancel_Date { get; set; }
        public virtual String Cancel_Explanation { get; set; }
        public virtual int? Cancel_Reason { get; set; }
        public virtual string CaseId { get; set; }
        public virtual byte? ConsultationFromUserGroupId { get; set; }
        public virtual int CreatedBy { get; set; }
        public virtual DateTime? CreatedDate { get; set; }

        public virtual int CurrentStatusCode
        {
            get { return WorkflowStatus.StatusCodeType.Id; }
        }

        public virtual IDocumentDao DocumentDao { get; set; }
        public virtual IList<Document> Documents { get; set; }
        public virtual int InitialId { get; set; }
        public virtual int InitialWorkflow { get; set; }
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

        public virtual int ModuleId
        {
            get { return (int)ModuleType.SARCAppeal; }
        }

        public virtual String Return_comment { get; set; }
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
        public virtual String RWOA_Reply { get; set; }
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

        //Signatures
        //public virtual SignatureEntry WingSARCSignature { get; set; }
        //public virtual SignatureEntry SARCAdminSignature { get; set; }
        //public virtual SignatureEntry BoardMedicalSignature { get; set; }
        //public virtual SignatureEntry BoardLegalSignature { get; set; }
        //public virtual SignatureEntry BoardAdminSignature { get; set; }
        //public virtual SignatureEntry AppellateAuthoritySignature { get; set; }
        public virtual WorkStatus WorkflowStatus { get; set; }

        #region Constructor

        public SARCAppeal()
        {
            AllDocuments.Add(DocumentType.SupportingSARCAppealDocuments.ToString(), false);
            AllDocuments.Add(DocumentType.MemberAppealRequest.ToString(), false);

            //WingSARCSignature = new SignatureEntry();
            //SARCAdminSignature = new SignatureEntry();
            //BoardMedicalSignature = new SignatureEntry();
            //BoardLegalSignature = new SignatureEntry();
            //BoardAdminSignature = new SignatureEntry();
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

        public virtual bool DetermineIfPostProcessingIsComplete(IDaoFactory daoFactory)
        {
            if (!WorkflowStatus.StatusCodeType.IsFinal)
                return false;

            if (WorkflowStatus.StatusCodeType.IsCancel)
                return true;

            if (MemberNotified && HasPostCompletionDigitalSignatureBeenGenerated(daoFactory))
                return true;

            return false;
        }

        public virtual SARCAppealFindings FindFindingByPersonnelType(short pType)
        {
            IEnumerable<SARCAppealFindings> lst = (from p in AppealFindings where p.PType == pType select p);
            if (lst.Count() > 0)
            {
                SARCAppealFindings old = lst.First();
                return old;
            }
            return null;
        }

        public virtual IEnumerable<WorkflowStatusOption> GetCurrentOptions(int lastStatus, IDaoFactory daoFactory)
        {
            Validations.Clear();
            ProcessDocuments(daoFactory);
            ProcessOption(lastStatus, daoFactory);
            return RuleAppliedOptions;
        }

        public virtual Memorandum GetDeterminationMemorandumFromLOD(AppUser user, IDigitalSignatureService sigService, IDaoFactory daoFactory)
        {
            if (!CanGenerateDeterminationMemorandum())
                return null;

            MemoTemplate template = GetMemorandumTemplate(daoFactory.GetMemoDao());

            if (template == null)
                return null;

            if (!MemoHelpers.DoesUserHaveCreatePermissionForTemplate(user, template))
                return null;

            return GenerateMemorandumForLOD(template, user, sigService, daoFactory);
        }

        public virtual Memorandum2 GetDeterminationMemorandumFromSARC(AppUser user, IDigitalSignatureService sigService, IDaoFactory daoFactory)
        {
            if (!CanGenerateDeterminationMemorandum())
                return null;

            MemoTemplate template = GetMemorandumTemplate(daoFactory.GetMemoDao());

            if (template == null)
                return null;

            if (!MemoHelpers.DoesUserHaveCreatePermissionForTemplate(user, template))
                return null;

            return GenerateMemorandumForSARC(template, user, sigService, daoFactory);
        }

        public virtual bool HasAFinding(SARCAppealFindings finding)
        {
            if (finding == null)
                return false;

            if (!finding.Finding.HasValue || finding.Finding.Value == 0)
                return false;

            return true;
        }

        public virtual void InitiateCase(PetitionWorkflowInitiateCaseArgs args)
        {
            SARCAppealInitiateCaseArgs castedArgs = (SARCAppealInitiateCaseArgs)args;
            InitialId = castedArgs.OriginalCaseRefId;
            InitialWorkflow = castedArgs.OriginalCaseWorkflowId;
            CaseId = castedArgs.GetNextCaseId();
            Workflow = (int)AFRCWorkflows.SARCRestrictedAppeal;
            MemberSSN = castedArgs.Member.SSN;
            MemberName = castedArgs.Member.FullName.ToUpper();
            MemberUnit = castedArgs.Member.Unit.Name;
            MemberUnitId = castedArgs.Member.Unit.Id;
            MemberCompo = castedArgs.Member.Component;
            MemberRank.SetId(castedArgs.Member.Rank.Id);
            Status = (int)SARCAppealWorkStatus.Initiate;
            CreatedBy = castedArgs.CreatedByUserId;
            CreatedDate = DateTime.Now;
            CreateDocumentGroup(castedArgs.DocumentDao);
        }

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
            get { return (int)DocumentViewType.Appeal_SARC; }
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

        #endregion IDocumentSource Members

        /// <summary>
        /// This method performs a basic override of the sarc appeal case. It checks if the case is being overriden to or from a
        /// cancel status and sets or clears the appropriate cancel related fields. It then simply assigns the new status ID to
        /// the Status property and assigns a new WorkStatus object to the WorkflowStatus property. If special considerations need
        /// to be taken into account for overriding a specific sarc appeal case, then the class for that sarc appeal case can override
        /// this method and provide its own implementation so that it can include code to handle those special considerations.
        /// </summary>
        public virtual void PerformOverride(IDaoFactory daoFactory, int newStatusId, int oldStatusId)
        {
            // Only perform the override if the two statuses differ...
            if (newStatusId != oldStatusId)
            {
                IWorkStatusDao workStatusDao = daoFactory.GetWorkStatusDao();
                IReminderEmailDao reminderEmailDao = daoFactory.GetReminderEmailDao();

                WorkStatus newStatus = workStatusDao.GetById(newStatusId);
                WorkStatus oldStatus = workStatusDao.GetById(oldStatusId);

                UpdateCancelProperties(oldStatus, newStatus);
                DeleteExistingDeterminationMemorandums(oldStatus, newStatus, daoFactory);

                reminderEmailDao.ReminderEmailUpdateStatusChange(oldStatusId, newStatusId, this.CaseId, "APSA");

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

        public virtual SARCAppealFindings SetFindingByType(SARCAppealFindings fnd)
        {
            int i = 0;

            if (AppealFindings.Count > 0)
            {
                foreach (SARCAppealFindings item in AppealFindings)
                {
                    if (item.PType == fnd.PType)
                    {
                        AppealFindings.ElementAt(i).Modify(fnd);
                        return AppealFindings.ElementAt(i);
                    }
                    i = i + 1;
                }
            }

            AppealFindings.Add(fnd);

            return AppealFindings.ElementAt(i);
        }

        public virtual void UpdateIsPostProcessingComplete(IDaoFactory daoFactory)
        {
            IsPostProcessingComplete = DetermineIfPostProcessingIsComplete(daoFactory);
        }

        protected virtual bool CanGenerateDeterminationMemorandum()
        {
            if (CurrentStatusCode != (int)SARCAppealStatusCode.Approved && CurrentStatusCode != (int)SARCAppealStatusCode.Denied)
                return false;

            SARCAppealFindings cFindings = FindFindingByPersonnelType((short)PersonnelTypes.APPELLATE_AUTH);

            if (cFindings == null)
                return false;

            if (!cFindings.Finding.HasValue)
                return false;

            return true;
        }

        protected virtual bool CanGenerateSignatureBlock(IDaoFactory daoFactory)
        {
            SARCAppealFindings appellateAuthorityFindings = FindFindingByPersonnelType((short)PersonnelTypes.APPELLATE_AUTH);

            if (appellateAuthorityFindings == null)
            {
                LogManager.LogError("Error in SARCAppeal.CanGenerateSignatureBlock(): Could not find Appellate Authority Findings for case " + CaseId + " (" + Id + ").");
                return false;
            }

            AppUser appellateAuthorityUser = daoFactory.GetUserDao().GetById(appellateAuthorityFindings.ModifiedBy);

            if (appellateAuthorityUser == null)
            {
                LogManager.LogError("Error in SARCAppeal.CanGenerateSignatureBlock(): Could not find Appellate Authority User for case " + CaseId + " (" + Id + ").");
                return false;
            }

            return true;
        }

        protected virtual void DeleteExistingDeterminationMemorandums(WorkStatus oldStatus, WorkStatus newStatus, IDaoFactory daoFactory)
        {
            if (oldStatus.StatusCodeType.IsFinal == true && !newStatus.StatusCodeType.IsFinal)
            {
                if (InitialWorkflow == (int)AFRCWorkflows.SARCRestricted)
                {
                    DeleteExistingSARCDeterminationMemorandums(daoFactory.GetMemoDao2());
                }
                else
                {
                    DeleteExistingLODDeterminationMemorandums(daoFactory.GetMemoDao());
                }
            }
        }

        protected virtual void DeleteExistingLODDeterminationMemorandums(IMemoDao memoDao)
        {
            foreach (Memorandum m in GetExistingLODDeterminationMemorandums(memoDao))
            {
                m.Deleted = true;
                memoDao.SaveOrUpdate(m);
            }
        }

        protected virtual void DeleteExistingSARCDeterminationMemorandums(IMemoDao2 memoDao)
        {
            foreach (Memorandum2 m in GetExistingSARCDeterminationMemorandums(memoDao))
            {
                m.Deleted = true;
                memoDao.SaveOrUpdate(m);
            }
        }

        protected virtual MemoContent GenerateMemorandumContentForLOD(MemoTemplate template, AppUser user, IDigitalSignatureService sigService, IDaoFactory daoFactory)
        {
            MemoContent content = new MemoContent();

            content.Body = template.PopulatedBody(daoFactory.GetMemoDao().GetMemoData(Id, template.DataSource));
            content.SignatureBlock = GetMemoSignatureBlock(sigService, daoFactory);
            content.CreatedBy = user;
            content.CreatedDate = DateTime.Now;

            if (template.AddDate)
                content.MemoDate = DateTime.Now.ToString(template.DateFormat).Trim();
            else
                content.MemoDate = string.Empty;

            return content;
        }

        protected virtual MemoContent2 GenerateMemorandumContentForSARC(MemoTemplate template, AppUser user, IDigitalSignatureService sigService, IDaoFactory daoFactory)
        {
            MemoContent2 content = new MemoContent2();

            content.Body = template.PopulatedBody(daoFactory.GetMemoDao2().GetMemoData(Id, template.DataSource));
            content.SignatureBlock = GetMemoSignatureBlock(sigService, daoFactory);
            content.CreatedBy = user;
            content.CreatedDate = DateTime.Now;

            if (template.AddDate)
                content.MemoDate = DateTime.Now.ToString(template.DateFormat).Trim();
            else
                content.MemoDate = string.Empty;

            return content;
        }

        protected virtual Memorandum GenerateMemorandumForLOD(MemoTemplate template, AppUser user, IDigitalSignatureService sigService, IDaoFactory daoFactory)
        {
            Memorandum memo = new Memorandum();

            memo.Template = template;
            memo.Letterhead = daoFactory.GetMemoDao().GetEffectiveLetterHead(user.Component);
            memo.CreatedBy = user;
            memo.CreatedDate = DateTime.Now;
            memo.Deleted = false;
            memo.ReferenceId = InitialId;
            memo.AddContent(GenerateMemorandumContentForLOD(template, user, sigService, daoFactory));

            return memo;
        }

        protected virtual Memorandum2 GenerateMemorandumForSARC(MemoTemplate template, AppUser user, IDigitalSignatureService sigService, IDaoFactory daoFactory)
        {
            Memorandum2 memo = new Memorandum2();

            memo.Template = template;
            memo.Letterhead = daoFactory.GetMemoDao2().GetEffectiveLetterHead(user.Component);
            memo.CreatedBy = user;
            memo.CreatedDate = DateTime.Now;
            memo.Deleted = false;
            memo.ReferenceId = InitialId;
            memo.ModuleId = (int)ModuleType.SARC;
            memo.AddContent(GenerateMemorandumContentForSARC(template, user, sigService, daoFactory));

            return memo;
        }

        protected virtual IList<Memorandum> GetExistingLODDeterminationMemorandums(IMemoDao memoDao)
        {
            return memoDao.GetByRefId(InitialId).Where(x => x.Template.Id == (int)MemoType.SARC_APPEAL_APPROVED || x.Template.Id == (int)MemoType.SARC_APPEAL_DISAPPROVAL).OrderByDescending(x => x.CreatedDate).ToList();
        }

        protected virtual IList<Memorandum2> GetExistingSARCDeterminationMemorandums(IMemoDao2 memoDao)
        {
            return memoDao.GetByRefnModule(InitialId, (int)ModuleType.SARC).Where(x => x.Template.Id == (int)MemoType.SARC_APPEAL_APPROVED || x.Template.Id == (int)MemoType.SARC_APPEAL_DISAPPROVAL).OrderByDescending(x => x.CreatedDate).ToList();
        }

        protected virtual MemoTemplate GetMemorandumTemplate(IMemoDao memoDao)
        {
            SARCAppealFindings cFindings = FindFindingByPersonnelType((short)PersonnelTypes.APPELLATE_AUTH);

            if (cFindings == null)
                return null;

            if (!cFindings.Finding.HasValue)
                return null;

            if (cFindings.Finding.Value == (int)Finding.Approve)
            {
                return memoDao.GetTemplateById((int)MemoType.SARC_APPEAL_APPROVED);
            }
            else if (cFindings.Finding.Value == (int)Finding.Disapprove)
            {
                return memoDao.GetTemplateById((int)MemoType.SARC_APPEAL_DISAPPROVAL);
            }
            else
            {
                return null;
            }
        }

        protected virtual string GetMemoSignatureBlock(IDigitalSignatureService sigService, IDaoFactory daoFactory)
        {
            if (!CanGenerateSignatureBlock(daoFactory))
            {
                return "ERROR GENERATING SIGNATURE BLOCK" + Environment.NewLine + "Please contact the ECT Help Desk for assistance";
            }

            SARCAppealFindings appellateAuthorityFindings = FindFindingByPersonnelType((short)PersonnelTypes.APPELLATE_AUTH);
            AppUser appellateAuthorityUser = daoFactory.GetUserDao().GetById(appellateAuthorityFindings.ModifiedBy);
            string memoSignature = Environment.NewLine;

            if (appellateAuthorityFindings.IsLegacyFinding)
            {
                MemoSignature signer = daoFactory.GetMemoSignatureDao().GetSignature(Id, Workflow, (int)PersonnelTypes.APPELLATE_AUTH);

                if (signer != null)
                    memoSignature = "Digitally signed by " + signer.Signature + Environment.NewLine + "Date: " + signer.sig_date;
            }
            else
            {
                memoSignature = MemoHelpers.GetMemoFormattedSignature(sigService);
            }

            memoSignature = memoSignature + Environment.NewLine + Environment.NewLine + appellateAuthorityUser.AlternateSignatureName + ", USAF" + Environment.NewLine + "Appellate Authority";

            return memoSignature;
        }

        protected virtual bool HasPostCompletionDigitalSignatureBeenGenerated(IDaoFactory daoFactory)
        {
            ISignatueMetaDateDao sigMetaDataDao = daoFactory.GetSigMetaDataDao();

            if (sigMetaDataDao.GetByWorkStatus(Id, Workflow, (int)SARCAppealWorkStatus.Approved) == null &&
                sigMetaDataDao.GetByWorkStatus(Id, Workflow, (int)SARCAppealWorkStatus.Denied) == null)
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

        protected bool Validate_CanForwardCase(ILookupDao lookupDao)
        {
            switch (CurrentStatusCode)
            {
                case (int)SARCAppealStatusCode.SARCAdminReview:
                    return Validate_FindingsForPersonnelType(PersonnelTypes.SARC_ADMIN, "SARC Admin");

                case (int)SARCAppealStatusCode.BoardMedicalReview:
                    return Validate_FindingsForPersonnelType(PersonnelTypes.BOARD_SG, "Board Medical");

                case (int)SARCAppealStatusCode.SeniorMedicalReview:
                    return Validate_FindingsForPersonnelType(PersonnelTypes.SENIOR_MEDICAL_REVIEWER, "Senior Medical Reviewer");

                case (int)SARCAppealStatusCode.BoardLegalReview:
                    return Validate_FindingsForPersonnelType(PersonnelTypes.BOARD_JA, "Board Legal");

                case (int)SARCAppealStatusCode.BoardAdminReview:
                    return Validate_FindingsForPersonnelType(PersonnelTypes.BOARD_A1, "Board Administrator");

                case (int)SARCAppealStatusCode.AppellateAuthorityReview:
                    return Validate_FindingsForPersonnelType(PersonnelTypes.APPELLATE_AUTH, "Appellate Authority");

                default:
                    return true;
            }
        }

        protected bool Validate_FindingsForPersonnelType(PersonnelTypes pType, string userGroupName)
        {
            bool isValid = true;
            SARCAppealFindings cFindings = FindFindingByPersonnelType((short)pType);

            if (cFindings == null || String.IsNullOrEmpty(cFindings.Remarks))
            {
                isValid = false;
                AddValidationItem(new ValidationItem(userGroupName, "Decision Explanation", "Must provide a decision explanation for the SARC AP case."));
            }

            return isValid;
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

        private void ApplyRulesToOption(WorkflowStatusOption o, WorkflowOptionRule r, int lastStatus, IMemoDao2 memoDao, ILookupDao lookupDao)
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
                                IList<Memorandum2> memolist = (from m in memoDao.GetByRefnModule(Id, (int)ModuleType.SARCAppeal) where m.Deleted == false && m.Template.Id == Convert.ToByte(memos[i]) select m).ToList<Memorandum2>();
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

                    case "cancomplete":
                        string approve = r.RuleValue.ToString();

                        SARCAppealFindings AppellateAuthFindings = FindFindingByPersonnelType((short)PersonnelTypes.APPELLATE_AUTH);

                        if (AppellateAuthFindings == null || !AppellateAuthFindings.Finding.HasValue || AppellateAuthFindings.Finding.Value == (short)Finding.Request_Consultation)
                        {
                            o.OptionValid = false;
                        }
                        else
                        {
                            if (approve.Equals("True"))
                            {
                                if (AppellateAuthFindings.Finding.Value == (short)Finding.Approve)
                                {
                                    o.OptionValid = true;
                                }
                                else
                                {
                                    o.OptionValid = false;
                                }
                            }
                            else
                            {
                                if (AppellateAuthFindings.Finding.Value == (short)Finding.Disapprove)
                                {
                                    o.OptionValid = true;
                                }
                                else
                                {
                                    o.OptionValid = false;
                                }
                            }
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

                        bool appellateforward = true;
                        SARCAppealFindings AppellateFindings = FindFindingByPersonnelType((short)PersonnelTypes.APPELLATE_AUTH);

                        if (AppellateFindings != null)
                        {
                            if (!AppellateFindings.Finding.HasValue)
                            {
                                appellateforward = false;
                                AddValidationItem(new ValidationItem("Appellate Authority", "Determination", "Must provide a determination for the AP case."));
                            }

                            if (String.IsNullOrEmpty(AppellateFindings.Remarks))
                            {
                                appellateforward = false;
                                AddValidationItem(new ValidationItem("Appellate Authority", "Decision Explanation", "Must provide a decision explanation for the AP case."));
                            }
                        }
                        else
                        {
                            appellateforward = false;
                            AddValidationItem(new ValidationItem("Appellate Authority", "Determination", "Must provide a determination for the AP case."));
                            AddValidationItem(new ValidationItem("Appellate Authority", "Decision Explanation", "Must provide a decision explanation for the AP case."));
                        }

                        o.OptionValid = appellateforward;

                        break;
                }
            }
        }

        private void ProcessOption(int lastStatus, IDaoFactory daoFactory)
        {
            IMemoDao2 memoDao = daoFactory.GetMemoDao2();
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
                    ApplyRulesToOption(opt, visible, lastStatus, memoDao, lookupDao);

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
                    ApplyRulesToOption(opt, validation, lastStatus, memoDao, lookupDao);

                    if (!opt.OptionValid)
                    {
                        optValid = false;
                    }
                }

                opt.OptionValid = optValid;
            }
        }

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
            if ((CurrentStatusCode == (int)SARCAppealStatusCode.Approved) || (CurrentStatusCode == (int)SARCAppealStatusCode.Denied))
            {
                access = PageAccessType.ReadOnly;
            }

            //Add all pages as readonly
            scAccessList.Add(APSASectionNames.APSA_INIT.ToString(), access);
            scAccessList.Add(APSASectionNames.APSA_SARC_ADMIN.ToString(), access);
            scAccessList.Add(APSASectionNames.APSA_BOARD_MED_REV.ToString(), access);
            scAccessList.Add(APSASectionNames.APSA_SENIOR_MED_REV.ToString(), access);
            scAccessList.Add(APSASectionNames.APSA_BOARD_LEGAL_REV.ToString(), access);
            scAccessList.Add(APSASectionNames.APSA_BOARD_ADMIN_REV.ToString(), access);
            scAccessList.Add(APSASectionNames.APSA_APPELLATE_AUTH_REV.ToString(), access);
            scAccessList.Add(APSASectionNames.APSA_APPROVED.ToString(), access);
            scAccessList.Add(APSASectionNames.APSA_DENIED.ToString(), access);
            scAccessList.Add(APSASectionNames.APSA_CANCELED.ToString(), access);
            scAccessList.Add(APSASectionNames.APSA_RLB.ToString(), access);

            switch (role)
            {
                case (int)UserGroups.WingSarc:
                    if (CurrentStatusCode == (int)SARCAppealStatusCode.Initiate)
                    {
                        scAccessList[APSASectionNames.APSA_INIT.ToString()] = PageAccessType.ReadWrite;
                        scAccessList[APSASectionNames.APSA_RLB.ToString()] = PageAccessType.ReadWrite;
                    }
                    break;

                case (int)UserGroups.SARCAdmin:
                    if (CurrentStatusCode == (int)SARCAppealStatusCode.SARCAdminReview)
                    {
                        scAccessList[APSASectionNames.APSA_SARC_ADMIN.ToString()] = PageAccessType.ReadWrite;
                        scAccessList[APSASectionNames.APSA_RLB.ToString()] = PageAccessType.ReadOnly;
                        scAccessList[APSASectionNames.APSA_BOARD_MED_REV.ToString()] = PageAccessType.ReadOnly;
                        scAccessList[APSASectionNames.APSA_BOARD_LEGAL_REV.ToString()] = PageAccessType.ReadOnly;
                        scAccessList[APSASectionNames.APSA_BOARD_ADMIN_REV.ToString()] = PageAccessType.ReadOnly;
                        scAccessList[APSASectionNames.APSA_APPELLATE_AUTH_REV.ToString()] = PageAccessType.ReadOnly;
                    }
                    break;

                case (int)UserGroups.BoardMedical:
                    if (CurrentStatusCode == (int)SARCAppealStatusCode.BoardMedicalReview)
                    {
                        scAccessList[APSASectionNames.APSA_BOARD_MED_REV.ToString()] = PageAccessType.ReadWrite;
                        scAccessList[APSASectionNames.APSA_RLB.ToString()] = PageAccessType.ReadOnly;
                    }
                    break;

                case (int)UserGroups.SeniorMedicalReviewer:
                    if (CurrentStatusCode == (int)SARCAppealStatusCode.SeniorMedicalReview)
                    {
                        scAccessList[APSASectionNames.APSA_SENIOR_MED_REV.ToString()] = PageAccessType.ReadWrite;
                        scAccessList[APSASectionNames.APSA_RLB.ToString()] = PageAccessType.ReadOnly;
                    }
                    break;

                case (int)UserGroups.BoardLegal:
                    if (CurrentStatusCode == (int)SARCAppealStatusCode.BoardLegalReview)
                    {
                        scAccessList[APSASectionNames.APSA_BOARD_LEGAL_REV.ToString()] = PageAccessType.ReadWrite;
                        scAccessList[APSASectionNames.APSA_RLB.ToString()] = PageAccessType.ReadOnly;
                    }
                    break;

                case (int)UserGroups.BoardAdministrator:
                    if (CurrentStatusCode == (int)SARCAppealStatusCode.BoardAdminReview)
                    {
                        scAccessList[APSASectionNames.APSA_BOARD_ADMIN_REV.ToString()] = PageAccessType.ReadWrite;
                        scAccessList[APSASectionNames.APSA_RLB.ToString()] = PageAccessType.ReadOnly;
                    }
                    break;

                case (int)UserGroups.AppellateAuthority:
                    if (CurrentStatusCode == (int)SARCAppealStatusCode.AppellateAuthorityReview)
                    {
                        scAccessList[APSASectionNames.APSA_APPELLATE_AUTH_REV.ToString()] = PageAccessType.ReadWrite;
                        scAccessList[APSASectionNames.APSA_RLB.ToString()] = PageAccessType.ReadOnly;
                    }
                    break;
            }

            return scAccessList;
        }

        #endregion MyAccess
    }
}