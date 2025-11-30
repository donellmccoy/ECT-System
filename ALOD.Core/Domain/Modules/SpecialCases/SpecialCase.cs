using ALOD.Core.Domain.Common;
using ALOD.Core.Domain.Documents;
using ALOD.Core.Domain.Lookup;
using ALOD.Core.Domain.Modules.Common;
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
    public class SpecialCase : Entity
    {
        public const string DECISION_CONCUR = "Y";
        public const string DECISION_NONCONCUR = "N";
        private IDictionary<string, bool> _catAllDocs;
        private IDictionary<string, bool> _catRequired;
        private IList<WorkflowStatusOption> _ruleAppliedOptions;
        private IList<ValidationItem> _validations;
        public virtual int? AssociatedWWD { get; set; }
        public virtual DateTime? CaseCancelDate { get; set; }
        public virtual String CaseCancelExplanation { get; set; }
        public virtual int? CaseCancelReason { get; set; }
        public virtual String CaseComments { get; set; }
        public virtual string CaseId { get; set; }
        public virtual int CreatedBy { get; set; }
        public virtual DateTime? CreatedDate { get; set; }

        public virtual int CurrentStatusCode
        {
            get { return WorkflowStatus.StatusCodeType.Id; }
        }

        // Documents
        public virtual IDocumentDao DocumentDao { get; set; }

        public virtual IList<Document> Documents { get; set; }
        public virtual DateTime? ExpirationDate { get; set; }
        public virtual int? HasAdminLOD { get; set; }
        public virtual int? HasAdminSC { get; set; }
        public virtual int? hqt_approval1 { get; set; }
        public virtual String hqt_approval1_comment { get; set; }
        public virtual int? hqt_approval2 { get; set; }
        public virtual String hqt_approval2_comment { get; set; }

        // public virtual String sig_name_med_off { get; set; }
        public virtual String med_off_approval_comment { get; set; }

        public virtual int? med_off_approved { get; set; }
        public virtual String med_tech_approval_comment { get; set; }

        // Approvals
        public virtual int? med_tech_approved { get; set; }

        public virtual int MemberCompo { get; set; }
        public virtual DateTime? MemberDOB { get; set; }
        public virtual String MemberName { get; set; }
        public virtual UserRank MemberRank { get; set; }
        public virtual String MemberSSN { get; set; }
        public virtual String MemberUnit { get; set; }
        public virtual int MemberUnitId { get; set; }
        public virtual int ModifiedBy { get; set; }
        public virtual DateTime? ModifiedDate { get; set; }
        public virtual int moduleId { get; set; }
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
        public virtual int? SeniorMedicalReviewerApproved { get; set; }
        public virtual string SeniorMedicalReviewerComment { get; set; }

        // Signatures
        //public virtual SignatureEntry MedTechSignature { get; set; }
        //public virtual SignatureEntry HQTechSignature { get; set; }
        //public virtual SignatureEntry BoardSGSignature { get; set; }
        //public virtual SignatureEntry HQTechFinalSignature { get; set; }
        public virtual string SeniorMedicalReviewerConcur { get; set; }

        public virtual DateTime sig_date_med_off { get; set; }
        public virtual int Status { get; set; }
        public virtual int? SubWorkflowType { get; set; }

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

        public SpecialCase()
        {
            //MedTechSignature = new SignatureEntry();
            //HQTechSignature = new SignatureEntry();
            //BoardSGSignature = new SignatureEntry();
            //HQTechFinalSignature = new SignatureEntry();
        }

        #endregion Constructor

        #region IDocumentSource Members

        /// <summary>
        /// <para>
        /// Maintains a dictionary of all the document categories assoicated with the workflow.
        /// For each category there is a bool value which states if a category requires a document
        /// to be uploaded for it.
        /// </para>
        /// <para>KEY: Document Category Name</para>
        /// <para>VALUE: TRUE if document(s) are required for category; FALSE if document(s) not required for category.</para>
        /// </summary>
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

        public virtual int DocumentViewId  // override this property
        {
            get { return (int)DocumentViewType.LineOfDuty; }
        }

        /// <summary>
        /// <para>
        /// Maintains a dictionary of all required document categories and if valid document(s) has been uploaded
        /// for the category.
        /// </para>
        /// <para>KEY: Document Category Name</para>
        /// <para>VALUE: TRUE if valid document(s) uploaded; FALSE if valid document(s) has not been uploaded.</para>
        /// </summary>
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

        public virtual void AddSignature(IDaoFactory daoFactory, UserGroups groupId, string nameAndRank, string title, AppUser user)
        {
            ISignatueMetaDateDao sigDao = daoFactory.GetSigMetaDataDao();

            SignatureMetaData signature = new SignatureMetaData();
            signature.refId = Id;
            signature.workflowId = Workflow;
            signature.workStatus = Status;
            signature.userGroup = (int)groupId;
            signature.userId = user.Id;
            signature.date = DateTime.Now;
            signature.NameAndRank = nameAndRank;

            sigDao.AddForWorkStatus(signature);
        }

        public virtual void CopyBaseObjectInstance(SpecialCase baseInstance)
        {
            Id = baseInstance.Id;
            MemberName = baseInstance.MemberName;
            MemberSSN = baseInstance.MemberSSN;
            MemberUnit = baseInstance.MemberUnit;
            MemberUnitId = baseInstance.MemberUnitId;
            MemberCompo = baseInstance.MemberCompo;
            MemberDOB = baseInstance.MemberDOB;
            MemberRank = baseInstance.MemberRank;
            Status = baseInstance.Status;
            CreatedBy = baseInstance.CreatedBy;
            CreatedDate = baseInstance.CreatedDate;
            ModifiedBy = baseInstance.ModifiedBy;
            ModifiedDate = baseInstance.ModifiedDate;
            moduleId = baseInstance.moduleId;
            Workflow = baseInstance.Workflow;
            WorkflowStatus = baseInstance.WorkflowStatus;
            DocumentGroupId = baseInstance.DocumentGroupId;
            CaseId = baseInstance.CaseId;
            HasAdminLOD = baseInstance.HasAdminLOD;
            CaseComments = baseInstance.CaseComments;
            SubWorkflowType = baseInstance.SubWorkflowType;
            //CurrentStatusCode  baseInstance.CurrentStatusCode;

            // Signatures
            //MedTechSignature = baseInstance.MedTechSignature;
            //HQTechSignature = baseInstance.HQTechSignature;
            //BoardSGSignature = baseInstance.BoardSGSignature;
            //HQTechFinalSignature = baseInstance.HQTechFinalSignature;

            // Approval and Denial
            med_off_approved = baseInstance.med_off_approved;
            med_off_approval_comment = baseInstance.med_off_approval_comment;
            med_tech_approval_comment = baseInstance.med_tech_approval_comment;
            med_tech_approved = baseInstance.med_tech_approved;
            hqt_approval1 = baseInstance.hqt_approval1;
            hqt_approval1_comment = baseInstance.hqt_approval1_comment;
            hqt_approval2 = baseInstance.hqt_approval2;
            hqt_approval2_comment = baseInstance.hqt_approval2_comment;
            SeniorMedicalReviewerConcur = baseInstance.SeniorMedicalReviewerConcur;
            SeniorMedicalReviewerApproved = baseInstance.SeniorMedicalReviewerApproved;
            SeniorMedicalReviewerComment = baseInstance.SeniorMedicalReviewerComment;

            // Cancels and Returns
            Return_Comment = baseInstance.Return_Comment;
            RWOA_Date = baseInstance.RWOA_Date;
            RWOA_Explanation = baseInstance.RWOA_Explanation;
            RWOA_Reason = baseInstance.RWOA_Reason;
            CaseCancelDate = baseInstance.CaseCancelDate;
            CaseCancelReason = baseInstance.CaseCancelReason;
            CaseCancelExplanation = baseInstance.CaseCancelExplanation;
            ReturnByGroup = baseInstance.ReturnByGroup;
            ReturnToGroup = baseInstance.ReturnToGroup;
        }

        public virtual bool CreateDocumentGroup(IDocumentDao dao)
        {
            Check.Require(dao != null, "DocumentDao is required");

            DocumentGroupId = dao.CreateGroup();
            return DocumentGroupId > 0;
        }

        public virtual int? GetActiveBoardMedicalFinding()
        {
            if (ShouldUseSeniorMedicalReviewerFindings())
                return SeniorMedicalReviewerApproved;

            return med_off_approved.HasValue ? med_off_approved : hqt_approval1.HasValue ? hqt_approval1 : 0;
        }

        public virtual IEnumerable<WorkflowStatusOption> GetCurrentINCAPOptions(int lastStatus, IDaoFactory factory, SC_Incap_Findings incap)
        {
            Validations.Clear();
            Validate();
            ProcessINCAPDocuments(factory, incap);
            ProcessOption(lastStatus, factory);
            return RuleAppliedOptions;
        }

        public virtual IEnumerable<WorkflowStatusOption> GetCurrentOptions(int lastStatus, IDaoFactory factory)
        {
            Validations.Clear();
            Validate();
            ProcessDocuments(factory);
            ProcessOption(lastStatus, factory);
            return RuleAppliedOptions;
        }

        public virtual int GetMemoTemplateId(string title)
        {
            //IDaoFactory daoFactory;
            //IMemoDao2 memoDao = ;

            //return memoDao.GetMemoTemplateId(title);
            return 0;
        }

        /// <summary>
        /// This method performs a basic override of the special case. It checks if the case is being overriden to or from a
        /// cancel status and sets or clears the appropriate cancel related fields. It then simply assigns the new status ID to
        /// the Status property and assigns a new WorkStatus object to the WorkflowStatus property. If special considerations need
        /// to be taken into account for overriding a specific special case, then the class for that special case can override
        /// this method and provide its own implementation so that it can include code to handle those special considerations.
        /// </summary>
        public virtual void PerformOverride(IWorkStatusDao workStatusDao, IReminderEmailDao reminderEmailDao, int newStatusId, int oldStatusId)
        {
            // Only perform the override if the two statuses differ...
            if (newStatusId != oldStatusId)
            {
                WorkStatus newStatus = workStatusDao.GetById(newStatusId);
                WorkStatus oldStatus = workStatusDao.GetById(oldStatusId);

                // Check if overriding FROM a cancel status...
                if (oldStatus.StatusCodeType.IsCancel == true)
                {
                    CaseCancelReason = null;
                    CaseCancelExplanation = null;
                    CaseCancelDate = null;
                }

                // Check if overriding TO a cancel status...
                if (newStatus.StatusCodeType.IsCancel == true)
                {
                    CaseCancelReason = 8;   // The value 8 is currently arbitrary...it does not map to an enum value or a lookup table in the database.
                                            // Currently the NextAction code behind checks for the value 8. Unsure as to why 8 was chosen.
                    CaseCancelExplanation = "Case Cancelled by System Admin";
                    CaseCancelDate = DateTime.Now;
                }

                // Update the reminder email for this case
                reminderEmailDao.ReminderEmailUpdateStatusChange(oldStatusId, newStatusId, this.CaseId, "SC");

                // Set the new status for the workflow
                this.Status = newStatusId;
                this.WorkflowStatus = newStatus;
            }
        }

        public virtual void ProcessDocuments(IDaoFactory daoFactory)
        {
            IDocumentDao docDao = daoFactory.GetDocumentDao();

            if (DocumentGroupId != null)
            {
                Documents = docDao.GetDocumentsByGroupId(DocumentGroupId.Value);
            }

            List<DocumentCategory2> viewCats = ProcessDocumentCategories(daoFactory);

            //This code checks the  document rules for current status for various options
            //Active categories are updated to reflect the required documents.
            //Documents which are required by the rules are added to the present category and it is
            //also updated to reflect valid//invalid based on the uploaded  documents
            if (WorkflowStatus != null)
            {
                foreach (var opt in WorkflowStatus.WorkStatusOptionList)
                {
                    foreach (var rule in opt.RuleList)
                    {
                        if (rule.RuleTypes.Name.ToLower().Equals("document"))
                        {
                            ProcessDocumentsForRule(rule, daoFactory, viewCats);
                        }
                    }
                }
            }
        }

        public virtual void ProcessINCAPDocuments(IDaoFactory daoFactory, SC_Incap_Findings incap)
        {
            IDocumentDao docDao = daoFactory.GetDocumentDao();

            if (DocumentGroupId != null)
            {
                Documents = docDao.GetDocumentsByGroupId(DocumentGroupId.Value);
            }

            List<DocumentCategory2> viewCats = ProcessDocumentCategories(daoFactory);

            //This code checks the  document rules for current status for various options
            //Active categories are updated to reflect the required documents.
            //Documents which are required by the rules are added to the present category and it is
            //also updated to reflect valid//invalid based on the uploaded  documents
            if (WorkflowStatus != null)
            {
                foreach (var opt in WorkflowStatus.WorkStatusOptionList)
                {
                    foreach (var rule in opt.RuleList)
                    {
                        if (rule.RuleTypes.Name.ToLower().Equals("in_document_validations"))
                        {
                            ProcessINCAPDocumentsForRule(rule, daoFactory, viewCats, incap);
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

        public virtual bool ShouldUseBoardMedicalReviewerindings()
        {
            if (hqt_approval1.HasValue && hqt_approval1 == 1)
            {
                return true;
            }
            return false;
        }

        public virtual bool ShouldUseSeniorMedicalReviewerFindings()
        {
            if (!string.IsNullOrEmpty(SeniorMedicalReviewerConcur) && SeniorMedicalReviewerConcur.Equals(DECISION_NONCONCUR))
                return true;

            return false;
        }

        //public virtual void
        public virtual bool ShouldUseWingMedicalReviewerindings()
        {
            if (med_off_approved.HasValue && med_off_approved == 1)
            {
                return true;
            }
            return false;
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

        protected virtual void AddDocumentCategoryValidationItem(string docName, List<DocumentCategory2> viewCats, string additionalAppendedText = "")
        {
            string description = (from p in viewCats where p.DocCatId.ToString() == docName select p.CategoryDescription).Single();
            AddValidationItem(new ValidationItem("Documents", docName, description + "  document not found." + additionalAppendedText));
        }

        protected void AddReqdDocs(string section, bool isValid)
        {
            if (!Required.ContainsKey(section))
                Required.Add(section, isValid);
        }

        protected void AddValidationItem(IList<ValidationItem> itemsList)
        {
            foreach (var item in itemsList)
            {
                if (!Validations.Contains(item))
                    AddValidationItem(item);
            }
        }

        protected void AddValidationItem(ValidationItem item)
        {
            IList<ValidationItem> lst = (from p in Validations where p.Section == item.Section && p.Field == item.Field select p).ToList<ValidationItem>();
            //if (lst.Count == 0)
            //{
            Validations.Add(item);
            //}
        }

        protected virtual void ApplyRulesToOption(WorkflowStatusOption o, WorkflowOptionRule r, int lastStatus, IDaoFactory daoFactory)
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
                                IList<Memorandum2> memolist = (from m in memoDao.GetByRefnModule(Id, (int)ModuleType.LOD) where m.Deleted == false && m.Template.Id == Convert.ToByte(memos[i]) select m).ToList<Memorandum2>();
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
                                string docName = ((DocumentType)(Convert.ToByte(docs[i]))).ToString();
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

        protected virtual bool DetermineVisibilityForHasDecisionRuleForSeniorMedicalReviewer(UserGroupContainsDecisionRuleData ruleData)
        {
            if (!string.IsNullOrEmpty(SeniorMedicalReviewerConcur) && ruleData.Decisions.Contains(SeniorMedicalReviewerConcur))
                return true;

            return false;
        }

        protected virtual bool DetermineVisibilityForUserGroupContainsDecisionRule(UserGroupContainsDecisionRuleData ruleData)
        {
            if (!ruleData.IsValid)
                return true;

            if (ruleData.UserGroupName.Equals("Senior Medical Reviewer"))
                return DetermineVisibilityForHasDecisionRuleForSeniorMedicalReviewer(ruleData);

            return true;
        }

        protected bool DoDocumentsExistForCategory(string docName)
        {
            if (Documents != null)
            {
                IList<Document> doclist = (from p in Documents where p.DocType.ToString() == docName select p).ToList<Document>();

                if (doclist.Count > 0)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Performs additional processing to determine if the specified document category still required document(s) to be uploaded
        /// for it.
        /// </summary>
        /// <param name="docName">Document category name</param>
        /// <returns>TRUE if the document category is still required; FALSE if the document category is no longer required</returns>
        protected virtual bool IsDocumentCategoryStillRequired(bool docFound, string docName, IDaoFactory daoFactory)
        {
            return true;    // DEFAULT: document category is still required as dictated by the workstatus option rules
        }

        protected virtual bool IsDocumentCategoryValid(bool docFound, bool isDocCatRequired, IDaoFactory daoFactory)
        {
            if (!isDocCatRequired)
                return true; // Non required document categories are valid regarldess of if they have document(s) uploaded to the category or not
            else
                return docFound;
        }

        protected virtual void PerformAdditionalDocumentProcessing(string docName, bool docFound, IDaoFactory daoFactory)
        {
        }

        protected virtual List<DocumentCategory2> ProcessDocumentCategories(IDaoFactory factory)
        {
            IDocCategoryViewDao docViewDao = factory.GetDocCategoryViewDao();

            List<DocumentCategory2> viewCats = (List<DocumentCategory2>)docViewDao.GetCategoriesByDocumentViewId(DocumentViewId);

            foreach (DocumentCategory2 dc in viewCats)
            {
                if (!AllDocuments.ContainsKey(((DocumentType)(Convert.ToByte(dc.DocCatId))).ToString()))
                {
                    UpdateActiveCategories(((DocumentType)(Convert.ToByte(dc.DocCatId))).ToString(), false);
                }
            }

            return viewCats;
        }

        protected virtual void ProcessDocumentsForRule(WorkflowOptionRule rule, IDaoFactory daoFactory, List<DocumentCategory2> viewCats)
        {
            string[] docs = rule.RuleValue.ToString().Split(',');    // get a list of all specified required documents for this workstatus option
            bool isValid;
            bool isReqd;
            bool docFound = false;

            for (int i = 0; i < docs.Length; i++)
            {
                if (!String.IsNullOrEmpty(docs[i]))
                {
                    string docName = docs[i];

                    docFound = DoDocumentsExistForCategory(docName);

                    PerformAdditionalDocumentProcessing(docName, docFound, daoFactory);

                    isReqd = IsDocumentCategoryStillRequired(docFound, docName, daoFactory);
                    isValid = IsDocumentCategoryValid(docFound, isReqd, daoFactory);

                    AddReqdDocs(docName, isValid);
                    UpdateDocumentCategoryValidation(docName, isReqd, isValid, viewCats, daoFactory);
                }
            }

            UpdateDocumentCheckAllValidation(rule, docFound);
        }

        protected virtual void ProcessINCAPDocumentsForRule(WorkflowOptionRule rule, IDaoFactory daoFactory, List<DocumentCategory2> viewCats, SC_Incap_Findings incap)
        {
            ProcessINCAPInitiateDocumentValidations(Documents, daoFactory, viewCats, incap);
        }

        protected virtual void ProcessINCAPInitiateDocumentValidations(IList<ALOD.Core.Domain.Documents.Document> documents, IDaoFactory daoFactory, List<DocumentCategory2> viewCats, SC_Incap_Findings incap)
        {
            IList<DocumentCategory2> reqDocuments = new List<DocumentCategory2>();
            IList<DocumentCategory2> availableDocuments = new List<DocumentCategory2>();
            int wsId = (int)WorkflowStatus.Id;

            foreach (DocumentCategory2 category in viewCats)
            {
                if (wsId == (int)AFRCWorkflows.INInitiate)
                {
                    switch (category.CategoryDescription)
                    {
                        case "INCAP Pay PM (1971)":
                        //case "Interim or Final LOD and DD Form 261":
                        case "Member Signed Personnel Briefing":
                        case "Member Signed Financial Briefing":
                        case "Duty Status Proof/Orders":
                        case "AF Form 1768 (Staff Summary Sheet)":
                        case "Current Pay Statement (Loss of Income)":
                            reqDocuments.Add(category);
                            break;

                        case "Commander’s Letter of Delayed Submission":
                            if (incap.Init_LateSubmission.HasValue)
                            {
                                if ((bool)incap.Init_LateSubmission)
                                {
                                    reqDocuments.Add(category);
                                }
                            }
                            break;
                    }
                }
                else if (wsId == (int)AFRCWorkflows.INMedicalReview_WG)
                {
                    switch (category.CategoryDescription)
                    {
                        case "Medical (1971)":
                        case "Medical Evaluation (Medical)":
                        case "Current Medical Documentation (Medical)":
                        case "Medical Treatment Plan (Medical)":
                        case "Signed 2870 (Medical)":
                        case "Medical Entitlements (Medical)":
                        case "Medical Documentation":
                            reqDocuments.Add(category);
                            break;
                    }
                }
                else if (wsId == (int)AFRCWorkflows.INImmediateCommanderReview)
                {
                    if (category.CategoryDescription.Equals("Unit CC (1971)") || category.CategoryDescription.Equals("AF Form 469 (Commander)"))
                    {
                        reqDocuments.Add(category);
                    }
                }
                else if (wsId == (int)AFRCWorkflows.INWingJAReview)
                {
                    if (category.CategoryDescription.Equals("Legal (1971)"))
                    {
                        reqDocuments.Add(category);
                    }
                }
                else if (wsId == (int)AFRCWorkflows.INFinanceReview)
                {
                    switch (category.CategoryDescription)
                    {
                        case "Finance (1971)":
                        case "INCAP Pay Calculation Worksheet":
                            reqDocuments.Add(category);
                            break;

                        case "Employer/Employee Release Statement (Loss of Income and employed)":
                        case "Statement from Civilian Employment (Loss of Income and employed)":
                            if (!(bool)incap.Fin_SelfEmployed)
                            {
                                reqDocuments.Add(category);
                            }
                            break;

                        case "Self Employment Unemployed Statement (Loss of income and self employed)":
                        case "Self Employed Income Protection Statement (Loss of income and self employed)":
                            if ((bool)incap.Fin_SelfEmployed)
                            {
                                reqDocuments.Add(category);
                            }
                            break;
                    }
                }
                else if (wsId == (int)AFRCWorkflows.INWingCCAction)
                {
                    if (category.CategoryDescription.Equals("Wing CC (1971)"))
                    {
                        reqDocuments.Add(category);
                    }
                }
                else if (wsId == (int)AFRCWorkflows.INAppeal)
                {
                    if (category.CategoryDescription.Equals("Member Letter of Appeal"))
                    {
                        reqDocuments.Add(category);
                    }
                }
            }

            foreach (DocumentCategory2 reqDoc in reqDocuments)
            {
                bool available = false;
                foreach (ALOD.Core.Domain.Documents.Document doc in documents)
                {
                    if (doc.DocTypeName.Equals(reqDoc.CategoryDescription))
                    {
                        available = true;
                    }
                }
                if (!available)
                {
                    AddValidationItem(new ValidationItem("Documents", "document", "Document " + reqDoc.CategoryDescription + " needs to be uploaded."));
                }
            }
        }

        protected virtual void ProcessOption(int lastStatus, IDaoFactory daoFactory)
        {
            var options = from o in WorkflowStatus.WorkStatusOptionList select o;

            //first, apply the visibility rules
            //foreach (var opt in options)
            //    RuleAppliedOptions.Add(opt);

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

        protected void UpdateActiveCategories(string section, bool isReqd)
        {
            if (AllDocuments.ContainsKey(section))
            {
                AllDocuments[section] = isReqd;
            }
            else
            {
                AllDocuments.Add(section, isReqd);
            }
        }

        protected virtual void UpdateDocumentCategoryValidation(string docName, bool isRequired, bool isValid, List<DocumentCategory2> viewCats, IDaoFactory daoFactory)
        {
            if (AllDocuments.ContainsKey(docName))
            {
                if (!isValid)
                {
                    AddDocumentCategoryValidationItem(docName, viewCats);
                }

                AllDocuments[docName] = isRequired;
            }
        }

        protected virtual void UpdateDocumentCheckAllValidation(WorkflowOptionRule rule, bool docFound)
        {
            // If only one document type needs to be present and it has been found, clear out the validation errors for the other document types that aren't present
            if (rule.CheckAll.HasValue)
            {
                if (!rule.CheckAll.Value)
                {
                    if (docFound)
                    {
                        Validations.Clear();
                    }
                    else
                    {
                        AddValidationItem(new ValidationItem("Documents", "document", "At least one of the above document types must be uploaded."));
                    }
                }
            }
        }

        #region MyAccess

        public virtual Dictionary<String, PageAccessType> ReadSectionList(int role)  // override me
        {
            Dictionary<String, PageAccessType> scAccessList = new Dictionary<String, PageAccessType>();
            return scAccessList;
        }

        #endregion MyAccess
    }
}