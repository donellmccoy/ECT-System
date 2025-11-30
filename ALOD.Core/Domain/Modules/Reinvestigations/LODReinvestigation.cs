using ALOD.Core.Domain.Common;
using ALOD.Core.Domain.Documents;
using ALOD.Core.Domain.Lookup;
using ALOD.Core.Domain.Modules.Common;
using ALOD.Core.Domain.Modules.Lod;
using ALOD.Core.Domain.Users;
using ALOD.Core.Domain.Workflow;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Core.Utils;
using ALOD.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ALOD.Core.Domain.Modules.Reinvestigations
{
    [Serializable]
    public class LODReinvestigation : Entity
    {
        private IDictionary<string, bool> _catAllDocs;
        private IDictionary<string, bool> _catRequired;
        private IList<WorkflowStatusOption> _ruleAppliedOptions;
        private IList<ValidationItem> _validations;
        public virtual int? aa_final_approved { get; set; }
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
        public virtual IList<LODReinvestigationFindings> Findings { get; set; }
        public virtual int InitialLodId { get; set; }
        public virtual bool IsNonDBSignCase { get; set; }
        public virtual string MemberCompo { get; set; }

        public virtual string MemberGrade
        { get { return MemberRank.Title; } }

        public virtual String MemberName { get; set; }
        public virtual UserRank MemberRank { get; set; }
        public virtual String MemberSSN { get; set; }
        public virtual string MemberUnit { get; set; }
        public virtual int MemberUnitId { get; set; }
        public virtual int ModifiedBy { get; set; }
        public virtual DateTime? ModifiedDate { get; set; }

        public virtual int ModuleId
        {
            get { return (int)ModuleType.ReinvestigationRequest; }
        }

        public virtual int? ReinvestigationLodId { get; set; }
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

        // Legacy Approval fields...this are needed for legacy RR case so that DBSign validation will still function
        public virtual int? wing_cc_approved { get; set; }

        public virtual int Workflow { get; set; }
        public virtual WorkStatus WorkflowStatus { get; set; }
        // Signatures
        //public virtual SignatureEntry MPFSignature { get; set; }
        //public virtual SignatureEntry WingJASignature { get; set; }
        //public virtual SignatureEntry WingCCSignature { get; set; }
        //public virtual SignatureEntry BoardAdminSignature { get; set; }
        //public virtual SignatureEntry BoardMedicalSignature { get; set; }
        //public virtual SignatureEntry BoardLegalSignature { get; set; }
        //public virtual SignatureEntry ApprovingAuthoritySignature { get; set; }
        //public virtual SignatureEntry BoardTechFinalSignature { get; set; }
        //public virtual SignatureEntry LODProgramManagerSignature { get; set; }
        //public virtual SignatureEntry BoardA1Signature { get; protected set; }

        #region Constructor

        public LODReinvestigation()
        {
            //MPFSignature = new SignatureEntry();
            //WingJASignature = new SignatureEntry();
            //WingCCSignature = new SignatureEntry();
            //BoardAdminSignature = new SignatureEntry();
            //BoardLegalSignature = new SignatureEntry();
            //BoardMedicalSignature = new SignatureEntry();
            //ApprovingAuthoritySignature = new SignatureEntry();
            //BoardTechFinalSignature = new SignatureEntry();
            //LODProgramManagerSignature = new SignatureEntry();
            //BoardA1Signature = new SignatureEntry();
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

        public virtual Memorandum2 GetDeterminationMemorandum(RRDeterminationMemoArgs args)
        {
            if (!CanGenerateDeterminationMemorandum(args.PType))
                return null;

            MemoTemplate template = GetMemorandumTemplate(args.PType, args.DaoFactory.GetMemoDao2());

            if (template == null)
                return null;

            args.Template = template;

            if (!MemoHelpers.DoesUserHaveCreatePermissionForTemplate(args.UserGeneratingMemo, template))
                return null;

            return GenerateMemorandum(args);
        }

        public virtual void InitiateCase(PetitionWorkflowInitiateCaseArgs args)
        {
            RRInitiateCaseArgs castedArgs = (RRInitiateCaseArgs)args;
            InitialLodId = castedArgs.OriginalCaseRefId;
            CaseId = castedArgs.GetNextCaseId();
            Workflow = (int)AFRCWorkflows.ReinvestigationRequest;
            MemberSSN = castedArgs.Member.SSN;
            MemberName = castedArgs.Member.FullName.ToUpper();
            MemberUnit = castedArgs.Member.Unit.Name;
            MemberUnitId = castedArgs.Member.Unit.Id;
            MemberCompo = castedArgs.Member.Component;
            MemberRank.SetId(castedArgs.Member.Rank.Id);
            Status = (int)ReinvestigationRequestWorkStatus.InitiateRequest;
            CreatedBy = castedArgs.CreatedByUserId;
            CreatedDate = DateTime.Now;
            CreateDocumentGroup(castedArgs.DocumentDao);
        }

        #region Findings

        public virtual LODReinvestigationFindings CreateEmptyFindingForUser(AppUser user)
        {
            LODReinvestigationFindings cFinding = new LODReinvestigationFindings();

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

        public virtual bool DoesFindingExist(PersonnelTypes pType)
        {
            if (FindFindingByPersonnelType(pType) == null)
                return false;

            return true;
        }

        public virtual LODReinvestigationFindings FindFindingByPersonnelType(PersonnelTypes pType)
        {
            IEnumerable<LODReinvestigationFindings> lst = (from p in Findings where p.PType == (short)pType select p);

            if (lst.Count() > 0)
            {
                return lst.First();
            }

            return null;
        }

        public virtual bool HasAFinding(LODReinvestigationFindings finding)
        {
            if (finding == null)
                return false;

            if (!finding.Finding.HasValue || finding.Finding.Value == 0)
                return false;

            return true;
        }

        public virtual LODReinvestigationFindings SetFindingByType(LODReinvestigationFindings fnd)
        {
            int i = 0;

            if (Findings.Count > 0)
            {
                foreach (LODReinvestigationFindings item in Findings)
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

        public virtual LODReinvestigationFindings SetPersonalByType(LODReinvestigationFindings fnd)
        {
            int i = 0;

            if (Findings.Count > 0)
            {
                foreach (LODReinvestigationFindings item in Findings)
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
            get { return (int)DocumentViewType.ReinvestigationRequest; }
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
        /// This method performs a basic override of the rr case. It checks if the case is being overriden to or from a
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

                UpdateCancelProperties(oldStatus, newStatus);
                DeleteExistingDeterminationMemorandums(oldStatus, newStatus, daoFactory);

                reminderEmailDao.ReminderEmailUpdateStatusChange(oldStatusId, newStatusId, this.CaseId, "RR");

                this.Status = newStatusId;
                this.WorkflowStatus = newStatus;
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
            foreach (DocumentCategory2 dc in viewCats)
            {
                if (!AllDocuments.ContainsKey(((DocumentType)(Convert.ToByte(dc.DocCatId))).ToString()))
                {
                    UpdateActiveCategories(((DocumentType)(Convert.ToByte(dc.DocCatId))).ToString(), true);
                }
            }

            IList<Document> doclist;
            string[] docs;
            string description = "";
            //This code checks the  document rules for current lod status for various options
            //Active categories are updated to reflect the required documents.
            //Documents which are required by the rules are added to the present category and it is
            //also updated to reflect valid//invalid based on the uploaded  documents
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

        public virtual void RemoveSignature(IDaoFactory daoFactory, int statusOut)
        {
            ISignatueMetaDateDao sigDao = daoFactory.GetSigMetaDataDao();
            sigDao.DeleteForWorkStatus(Id, Workflow, statusOut);
        }

        protected virtual bool CanGenerateDeterminationMemorandum(PersonnelTypes pType)
        {
            if (CurrentStatusCode != (int)ReinvestigationRequestStatusCode.WingCCRequestReview && CurrentStatusCode != (int)ReinvestigationRequestStatusCode.ApprovingAuthorityRequestAction)
                return false;

            if (!HasAFinding(FindFindingByPersonnelType(pType)))
                return false;

            return true;
        }

        protected virtual bool CanGenerateSignatureBlock(RRDeterminationMemoArgs args)
        {
            if (args.SigService == null)
            {
                LogManager.LogError("Error in LODReinvestigation.CanGenerateSignatureBlock(): Could not initialize the Digital Signature Service for the " + GetPersonnelTypeName(args.PType) + " Findings for case " + CaseId + " (" + Id + ").");
                return false;
            }

            LODReinvestigationFindings cFindings = FindFindingByPersonnelType(args.PType);

            if (cFindings == null)
            {
                LogManager.LogError("Error in LODReinvestigation.CanGenerateSignatureBlock(): Could not find the " + GetPersonnelTypeName(args.PType) + " Findings for case " + CaseId + " (" + Id + ").");
                return false;
            }

            AppUser findingsUser = args.DaoFactory.GetUserDao().GetById(cFindings.ModifiedBy);

            if (findingsUser == null)
            {
                LogManager.LogError("Error in LODReinvestigation.CanGenerateSignatureBlock(): Could not find the " + GetPersonnelTypeName(args.PType) + " User for case " + CaseId + " (" + Id + ").");
                return false;
            }

            return true;
        }

        protected virtual void DeleteExistingDeterminationMemorandums(WorkStatus oldStatus, WorkStatus newStatus, IDaoFactory daoFactory)
        {
            if (oldStatus.StatusCodeType.IsFinal && !newStatus.StatusCodeType.IsFinal)
            {
                IMemoDao2 memoDao = daoFactory.GetMemoDao2();

                foreach (Memorandum2 m in GetExistingDeterminationMemorandums(memoDao))
                {
                    m.Deleted = true;
                    memoDao.SaveOrUpdate(m);
                }
            }
        }

        protected virtual Memorandum2 GenerateMemorandum(RRDeterminationMemoArgs args)
        {
            Memorandum2 memo = new Memorandum2();

            memo.Template = args.Template;
            memo.Letterhead = args.DaoFactory.GetMemoDao2().GetEffectiveLetterHead(args.UserGeneratingMemo.Component);
            memo.CreatedBy = args.UserGeneratingMemo;
            memo.CreatedDate = DateTime.Now;
            memo.Deleted = false;
            memo.ReferenceId = Id;
            memo.ModuleId = (int)ModuleType.ReinvestigationRequest;
            memo.AddContent(GenerateMemorandumContent(args));

            return memo;
        }

        protected virtual MemoContent2 GenerateMemorandumContent(RRDeterminationMemoArgs args)
        {
            MemoContent2 content = new MemoContent2();

            content.Body = args.Template.PopulatedBody(args.DaoFactory.GetMemoDao2().GetMemoData(Id, args.Template.DataSource));
            content.SignatureBlock = GetMemoSignatureBlock(args);
            content.CreatedBy = args.UserGeneratingMemo;
            content.CreatedDate = DateTime.Now;

            if (args.Template.AddDate)
                content.MemoDate = DateTime.Now.ToString(args.Template.DateFormat).Trim();
            else
                content.MemoDate = string.Empty;

            return content;
        }

        protected virtual IList<Memorandum2> GetExistingDeterminationMemorandums(IMemoDao2 memoDao)
        {
            return memoDao.GetByRefnModule(Id, (int)ModuleType.ReinvestigationRequest).Where(x => x.Template.Id == (int)MemoType.ReinvestigationRequestApproved || x.Template.Id == (int)MemoType.ReinvestigationRequestDenied).OrderByDescending(x => x.CreatedDate).ToList();
        }

        protected virtual MemoTemplate GetMemorandumTemplate(PersonnelTypes pType, IMemoDao2 memoDao)
        {
            LODReinvestigationFindings finding = FindFindingByPersonnelType(pType);

            if (!HasAFinding(finding))
                return null;

            if (finding.Finding == (short)Finding.Approve)
                return memoDao.GetTemplateById((int)MemoType.ReinvestigationRequestApproved);

            if (finding.Finding == (short)Finding.Disapprove)
                return memoDao.GetTemplateById((int)MemoType.ReinvestigationRequestDenied);

            return null;
        }

        protected virtual string GetMemoSignatureBlock(RRDeterminationMemoArgs args)
        {
            if (!CanGenerateSignatureBlock(args))
            {
                return "ERROR GENERATING SIGNATURE BLOCK" + Environment.NewLine + "Please contact the ECT Help Desk for assistance";
            }

            LODReinvestigationFindings cFindings = FindFindingByPersonnelType(args.PType);
            AppUser findingsUser = args.DaoFactory.GetUserDao().GetById(cFindings.ModifiedBy);

            string memoSignature = MemoHelpers.GetMemoFormattedSignature(args.SigService);

            memoSignature += Environment.NewLine + Environment.NewLine;
            memoSignature += findingsUser.AlternateSignatureName + ", ";
            memoSignature += GetPersonnelCompoAbbreviation(args.PType) + Environment.NewLine;
            memoSignature += findingsUser.CurrentRoleName;

            return memoSignature;
        }

        protected virtual string GetPersonnelCompoAbbreviation(PersonnelTypes pType)
        {
            switch (pType)
            {
                case PersonnelTypes.BOARD_AA:
                    return "USAF";

                case PersonnelTypes.APPOINT_AUTH:
                    return "USAF";

                default:
                    return "UNKNOWN";
            }
        }

        protected virtual string GetPersonnelTypeName(PersonnelTypes pType)
        {
            switch (pType)
            {
                case PersonnelTypes.BOARD_AA:
                    return "Approving Authority";

                case PersonnelTypes.APPOINT_AUTH:
                    return "Appointing Authority";

                default:
                    return "UNKNOWN";
            }
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
                    //Example-if coming from app_auth or review board fwd to med off should not be visible (med tech section)
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

                    case "wingjasignature":
                        bool isSigned = false;
                        ISignatueMetaDateDao sigDao = daoFactory.GetSigMetaDataDao();
                        if (sigDao.GetByWorkStatus(Id, Workflow, (int)ReinvestigationRequestWorkStatus.WingJARequestReview) != null)
                            isSigned = true;
                        if ((Convert.ToBoolean(r.RuleValue.ToString().ToLower()) == true) && (isSigned != true))
                        {
                            o.OptionVisible = false;
                        }
                        break;

                    case "wingccapproverr":
                        o.OptionVisible = Visibility_IsDesiredFindingForPersonnelType(PersonnelTypes.APPOINT_AUTH, Finding.Approve);
                        break;

                    case "wingccdenyrr":
                        o.OptionVisible = Visibility_IsDesiredFindingForPersonnelType(PersonnelTypes.APPOINT_AUTH, Finding.Disapprove);
                        break;

                    case "reinvestigationrequestapproved":
                        o.OptionVisible = Visibility_IsDesiredFindingForPersonnelType(PersonnelTypes.BOARD_AA, Finding.Approve);
                        break;

                    case "reinvestigationrequestdenied":
                        o.OptionVisible = Visibility_IsDesiredFindingForPersonnelType(PersonnelTypes.BOARD_AA, Finding.Disapprove);
                        break;
                }
            }

            if (r.RuleTypes.ruleTypeId == (int)RuleKind.Validation)
            {
                bool ruleValue;

                //Validation Rule
                switch (r.RuleTypes.Name.ToLower())
                {
                    case "wingjaactionrr":
                        o.OptionValid = Validation_CanForward_HasPersonnelMadeDecision(r, PersonnelTypes.WING_JA, "Wing JA");
                        break;

                    case "wingccactionrr":
                        o.OptionValid = Validation_CanForward_HasPersonnelMadeDecision(r, PersonnelTypes.APPOINT_AUTH, "Wing CC");
                        break;

                    case "wingccapproverr":
                        o.OptionValid = Validation_CanForward_HasPersonnelMadeDesiredDecision(r, PersonnelTypes.APPOINT_AUTH, "Wing CC", Finding.Approve, "Complete");
                        break;

                    case "wingccdenyrr":
                        o.OptionValid = Validation_CanForward_HasPersonnelMadeDesiredDecision(r, PersonnelTypes.APPOINT_AUTH, "Wing CC", Finding.Disapprove, "Forward");
                        break;

                    case "boardtechactionrr":
                        o.OptionValid = Validation_CanForward_HasPersonnelMadeDecision(r, PersonnelTypes.BOARD, "Board Tech");
                        break;

                    case "boardmedicalactionrr":
                        o.OptionValid = Validation_CanForward_HasPersonnelMadeDecision(r, PersonnelTypes.BOARD_SG, "Board Medical");
                        break;

                    case "seniormedicalactionrr":
                        o.OptionValid = Validation_CanForward_HasPersonnelMadeDecision(r, PersonnelTypes.SENIOR_MEDICAL_REVIEWER, "Senior Medical Reviewer");
                        break;

                    case "boarda1actionrr":
                        o.OptionValid = Validation_CanForward_HasPersonnelMadeDecision(r, PersonnelTypes.BOARD_A1, "Board A1");
                        break;

                    case "boardlegalactionrr":
                        o.OptionValid = Validation_CanForward_HasPersonnelMadeDecision(r, PersonnelTypes.BOARD_JA, "Board Legal");
                        break;

                    case "approvalactionrr":
                        o.OptionValid = Validation_CanForward_HasPersonnelMadeDecision(r, PersonnelTypes.BOARD_AA, "Approving Authority");
                        break;

                    case "wingjasignature":
                        bool isSigned = false;
                        ISignatueMetaDateDao sigDao = daoFactory.GetSigMetaDataDao();
                        if (sigDao.GetByWorkStatus(Id, Workflow, (int)ReinvestigationRequestWorkStatus.WingJARequestReview) != null)
                            isSigned = true;
                        if (r.RuleValue == null || r.RuleValue == String.Empty)
                        {
                            ruleValue = true;
                        }
                        else
                        {
                            ruleValue = Convert.ToBoolean(r.RuleValue.ToString().ToLower());
                        }
                        if ((ruleValue == true) && (isSigned == false))
                        {
                            o.OptionValid = false;
                            AddValidationItem(new ValidationItem("Signatures", "Wing JA",
                                "Can not return to Wing JA.  Wing JA Signature not present", true));
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
                                IList<Memorandum2> memolist = (from m in memoDao.GetByRefnModule(Id, (int)ModuleType.ReinvestigationRequest) where m.Deleted == false && m.Template.Id == Convert.ToByte(memos[i]) select m).ToList<Memorandum2>();
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

        protected bool Validation_CanForward_HasPersonnelMadeDecision(WorkflowOptionRule optionRule, PersonnelTypes pType, string userGroupName)
        {
            bool hasActed = HasAFinding(FindFindingByPersonnelType(pType));
            bool hasToAct = true;

            if (!string.IsNullOrEmpty(optionRule.RuleValue))
                hasToAct = Convert.ToBoolean(optionRule.RuleValue.ToString().ToLower());

            if (hasToAct == true && hasActed == false)
            {
                AddValidationItem(new ValidationItem("Actions", userGroupName, "Can not Forward Reinvestigation Request.  You must take an Action", true));
                return false;
            }

            return true;
        }

        protected bool Validation_CanForward_HasPersonnelMadeDesiredDecision(WorkflowOptionRule optionRule, PersonnelTypes pType, string userGroupName, Finding desiredFinding, string routingType)
        {
            bool hasActed = false;
            bool hasToAct = true;

            if (HasAFinding(FindFindingByPersonnelType(pType)) && FindFindingByPersonnelType(pType).Finding.Value == (short)desiredFinding)
                hasActed = true;

            if (!string.IsNullOrEmpty(optionRule.RuleValue))
                hasToAct = Convert.ToBoolean(optionRule.RuleValue.ToString().ToLower());

            if (hasToAct == true && hasActed == false)
            {
                AddValidationItem(new ValidationItem("Actions", userGroupName, "Can not " + routingType + " Reinvestigation Request.  You must take an Action", true));
                return false;
            }

            return true;
        }

        protected bool Visibility_IsDesiredFindingForPersonnelType(PersonnelTypes pType, Finding desiredFinding)
        {
            LODReinvestigationFindings cFindings = FindFindingByPersonnelType(pType);

            if (!HasAFinding(cFindings) || cFindings.Finding.Value != (short)desiredFinding)
            {
                return false;
            }

            return true;
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
            if ((Status == (int)ReinvestigationRequestWorkStatus.RequestApproved) || (Status == (int)ReinvestigationRequestWorkStatus.RequestDenied))
            {
                access = PageAccessType.ReadOnly;
            }

            //'Add all pages as readonly

            scAccessList.Add(RRSectionNames.RR_INIT.ToString(), access);
            scAccessList.Add(RRSectionNames.RR_WING_JA_REV.ToString(), access);
            scAccessList.Add(RRSectionNames.RR_WING_CC_REV.ToString(), access);
            scAccessList.Add(RRSectionNames.RR_BOARD_TECH_REV.ToString(), access);
            scAccessList.Add(RRSectionNames.RR_BOARD_MED_REV.ToString(), access);
            scAccessList.Add(RRSectionNames.RR_SENIOR_MED_REV.ToString(), access);
            scAccessList.Add(RRSectionNames.RR_BOARD_LEGAL_REV.ToString(), access);
            scAccessList.Add(RRSectionNames.RR_BOARD_APPROVING_AUTH_REV.ToString(), access);
            scAccessList.Add(RRSectionNames.RR_BOARD_TECH_FINAL.ToString(), access);
            scAccessList.Add(RRSectionNames.RR_RLB.ToString(), access);
            scAccessList.Add(RRSectionNames.RR_BOARD_A1_REV.ToString(), access);

            //Modify access with user role
            switch (role)
            {
                case (int)UserGroups.MPF:
                    if (Status == (int)ReinvestigationRequestWorkStatus.InitiateRequest)
                    {
                        scAccessList[RRSectionNames.RR_INIT.ToString()] = PageAccessType.ReadWrite;
                    }
                    break;

                case (int)UserGroups.LOD_PM:
                    if (Status == (int)ReinvestigationRequestWorkStatus.InitiateRequest)
                    {
                        scAccessList[RRSectionNames.RR_INIT.ToString()] = PageAccessType.ReadWrite;
                        scAccessList[RRSectionNames.RR_RLB.ToString()] = PageAccessType.ReadWrite;
                    }
                    break;

                case (int)UserGroups.WingJudgeAdvocate:
                    if (Status == (int)ReinvestigationRequestWorkStatus.WingJARequestReview)
                    {
                        scAccessList[RRSectionNames.RR_WING_JA_REV.ToString()] = PageAccessType.ReadWrite;
                        scAccessList[RRSectionNames.RR_RLB.ToString()] = PageAccessType.ReadWrite;
                    }
                    break;

                case (int)UserGroups.WingCommander:
                    if (Status == (int)ReinvestigationRequestWorkStatus.WingCCRequestReview)
                    {
                        scAccessList[RRSectionNames.RR_WING_CC_REV.ToString()] = PageAccessType.ReadWrite;
                        scAccessList[RRSectionNames.RR_RLB.ToString()] = PageAccessType.ReadWrite;
                    }
                    break;

                case (int)UserGroups.BoardTechnician:
                    if (Status == (int)ReinvestigationRequestWorkStatus.BoardTechRequestReview)
                    {
                        scAccessList[RRSectionNames.RR_BOARD_TECH_REV.ToString()] = PageAccessType.ReadWrite;
                        scAccessList[RRSectionNames.RR_RLB.ToString()] = PageAccessType.ReadOnly;
                    }
                    if (Status == (int)ReinvestigationRequestWorkStatus.BoardTechRequestFinalReview)
                    {
                        scAccessList[RRSectionNames.RR_BOARD_TECH_FINAL.ToString()] = PageAccessType.ReadWrite;
                    }
                    break;

                case (int)UserGroups.BoardMedical:
                    if (Status == (int)ReinvestigationRequestWorkStatus.BoardMedicalRequestReview)
                    {
                        scAccessList[RRSectionNames.RR_BOARD_MED_REV.ToString()] = PageAccessType.ReadWrite;
                        scAccessList[RRSectionNames.RR_RLB.ToString()] = PageAccessType.ReadOnly;
                    }
                    break;

                case (int)UserGroups.SeniorMedicalReviewer:
                    if (Status == (int)ReinvestigationRequestWorkStatus.SeniorMedicalReview)
                    {
                        scAccessList[RRSectionNames.RR_SENIOR_MED_REV.ToString()] = PageAccessType.ReadWrite;
                        scAccessList[RRSectionNames.RR_RLB.ToString()] = PageAccessType.ReadOnly;
                    }
                    break;

                case (int)UserGroups.BoardLegal:
                    if (Status == (int)ReinvestigationRequestWorkStatus.BoardLegalRequestReview)
                    {
                        scAccessList[RRSectionNames.RR_BOARD_LEGAL_REV.ToString()] = PageAccessType.ReadWrite;
                        scAccessList[RRSectionNames.RR_RLB.ToString()] = PageAccessType.ReadOnly;
                    }
                    break;

                case (int)UserGroups.BoardApprovalAuthority:
                    if (Status == (int)ReinvestigationRequestWorkStatus.ApprovingAuthorityRequestAction)
                    {
                        scAccessList[RRSectionNames.RR_BOARD_APPROVING_AUTH_REV.ToString()] = PageAccessType.ReadWrite;
                        scAccessList[RRSectionNames.RR_RLB.ToString()] = PageAccessType.ReadOnly;
                    }
                    break;

                case (int)UserGroups.BoardAdministrator:
                    if (Status == (int)ReinvestigationRequestWorkStatus.BoardA1RequestReview)
                    {
                        scAccessList[RRSectionNames.RR_BOARD_A1_REV.ToString()] = PageAccessType.ReadWrite;
                        scAccessList[RRSectionNames.RR_RLB.ToString()] = PageAccessType.ReadOnly;
                    }
                    break;

                default:
                    break;
            }
            return scAccessList;
        }

        #endregion MyAccess
    }
}