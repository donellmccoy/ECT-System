using ALOD.Core.Domain.Common;
using ALOD.Core.Domain.Documents;
using ALOD.Core.Domain.Lookup;
using ALOD.Core.Domain.Users;
using ALOD.Core.Domain.Workflow;
using ALOD.Core.Interfaces;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ALOD.Core.Domain.Modules.Lod
{
    [Serializable]
    public class ANGLineOfDuty : Entity, ITrackingSource
    {
        protected IDictionary<string, bool> _catActive;

        protected IDictionary<string, bool> _catRequired;

        protected IDictionary<string, bool> _moduleStatus;

        protected IList<WorkflowStatusOption> _ruleAppliedOptions;

        protected IList<ValidationItem> _validations;

        public virtual int? AAUserId { get; set; }

        public virtual AppUser AppointedIO { get; set; }

        public virtual string AppointingAuthorityPoc { get; set; }

        public virtual string AppointingCancelExplanation { get; set; }

        public virtual int? AppointingCancelReasonId { get; set; }

        public virtual int? ApprovingAuthorityUserId { get; set; }

        public virtual string ApprovingCancelExplanation { get; set; }

        // Approving Authority Cancel properties
        public virtual int? ApprovingCancelReasonId { get; set; }

        public virtual IList<LODComment> BoardComments { get; set; }

        public virtual string BoardForGeneral { get; set; }

        public virtual string BoardTechComments { get; set; }

        //case data
        public virtual string CaseId { get; set; }

        public virtual int? Completed_By_Unit { get; set; }

        public virtual int CreatedBy { get; set; }

        public virtual DateTime CreatedDate { get; set; }

        public virtual int CurrentStatusCode
        {
            get { return WorkflowStatus.StatusCodeType.Id; }
        }

        public virtual IDocumentDao DocumentDao { get; set; }

        public virtual IList<Document> Documents { get; set; }

        public virtual string FinalDecision { get; set; }

        public virtual short? FinalFindings { get; set; }

        public virtual bool Formal { get; set; }

        public virtual string FromUnit { get; set; }

        // Board Personnel properties
        public virtual bool? HasCredibleService { get; set; }

        public virtual string InstructionsToInvestigator { get; set; }

        public virtual DateTime? IoCompletionDate { get; set; }

        //investigation
        public virtual string IoSsn { get; set; }

        // Attached PASSCODE Fields
        public virtual bool isAttachPas { get; set; }

        public virtual bool IsDeleted { get; set; }

        // Appointing Authority Cancel properties
        public virtual bool IsFormalCancelRecommended { get; set; }

        public virtual bool IsPostProcessingComplete { get; set; }

        public virtual bool IsRestricted { get; set; }

        public virtual IList<LineOfDutyFindings> LodFindings { get; set; }

        public virtual LineOfDutyInvestigation LODInvestigation { get; set; }

        public virtual LineOfDutyMedical LODMedical { get; set; }

        public virtual int LODPM { get; set; }

        public virtual LineOfDutyUnit LODUnit { get; set; }

        public virtual string MedTechComments { get; set; }

        public virtual int? MemberAttachedUnitId { get; set; }

        public virtual string MemberCompo { get; set; }

        public virtual DateTime? MemberDob { get; set; }

        public virtual string MemberGrade
        { get { return MemberRank.Title; } }

        //Member data
        public virtual string MemberName { get; set; }

        public virtual bool memberNotified { get; set; }

        public virtual UserRank MemberRank { get; set; }

        //Selected by WIng CC
        public virtual string MemberSSN { get; set; }

        public virtual string MemberUnit { get; set; }

        public virtual int MemberUnitId { get; set; }

        public virtual int ModifiedBy { get; set; }

        public virtual DateTime ModifiedDate { get; set; }

        //parent case (used if this case is a reinvestigation of a prior case)
        public virtual int? ParentId { get; set; }

        public virtual int? ReturnByGroup { get; set; }

        public virtual string ReturnComment { get; set; }

        public virtual int? ReturnToGroup { get; set; }

        public virtual DateTime? RwoaDate { get; set; }

        public virtual string RwoaExplanation { get; set; }

        //signatures
        //public virtual SignatureEntry UnitCommanderSignature { get; protected set; }
        //public virtual SignatureEntry MedicalOfficerSignature { get; protected set; }
        //public virtual SignatureEntry JudgeAdvocateSignature { get; protected set; }
        //public virtual SignatureEntry AppointingAuthoritySignature { get; protected set; }
        //public virtual SignatureEntry BoardMedicalSignature { get; protected set; }
        //public virtual SignatureEntry BoardLegalSignature { get; protected set; }
        //public virtual SignatureEntry BoardAdminSignature { get; protected set; }   // Board Technician's signature
        //public virtual SignatureEntry ApprovalAuthoritySignature { get; protected set; }
        //public virtual SignatureEntry FormalApprovalAuthoritySignature { get; protected set; }
        //public virtual SignatureEntry MPFSignature { get; protected set; }
        //public virtual SignatureEntry LODProgramManagerSignature { get; protected set; }
        //public virtual SignatureEntry BoardA1Signature { get; protected set; }  // Board Admin's signatures
        public virtual short? RwoaReason { get; set; }

        public virtual bool SarcCase { get; set; }

        /// <inheritdoc/>
        public virtual int Status { get; set; }

        //check the timely manner the member notified the RMU
        public virtual bool? TimelyMemberNotify { get; set; }

        public virtual bool? TimelyMemberSubmit { get; set; }
        public virtual string ToUnit { get; set; }
        public virtual bool? WasMemberOnOrders { get; set; }
        public virtual int Workflow { get; set; }
        public virtual WorkStatus WorkflowStatus { get; set; }

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
            signature.NameAndRank = user.SignatureName;
            signature.Title = title;

            sigDao.AddForWorkStatus(signature);

            if (CurrentStatusCode == (int)LodStatusCode.FormalActionByAppointingAuthority && LODInvestigation != null)
            {
                if (Formal && LODInvestigation != null)
                {
                    LODInvestigation.DateSignedAppointing = DateTime.Now;
                    PersonnelData perapp = new PersonnelData();
                    perapp.Name = user.FormName;
                    perapp.Grade = user.Rank.Rank;
                    perapp.SSN = user.SSN;
                    perapp.PasCodeDescription = user.Unit.Name;
                    perapp.Branch = "AFRC";
                    LODInvestigation.SignatureInfoAppointing = perapp;
                }
            }

            if (CurrentStatusCode == (int)LodStatusCode.FormalInvestigation && LODInvestigation != null)
            {
                LODInvestigation.DateSignedIO = DateTime.Now;
                PersonnelData perio = new PersonnelData();

                perio.Name = user.FormName;
                perio.Grade = user.Rank.Rank;
                perio.SSN = user.SSN;
                perio.PasCodeDescription = user.Unit.Name;
                perio.Branch = "AFRC";
                LODInvestigation.SignatureInfoIO = perio;
            }
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

            if (memberNotified && HasDeterminationMemo(daoFactory) && HasPostCompletionDigitalSignatureBeenGenerated(daoFactory))
                return true;

            return false;
        }

        public virtual string IOSelectionString()
        {
            String selectionValue = String.Empty;

            //Appointed IO
            if (AppointedIO != null)
            {
                selectionValue = "uid:" + AppointedIO.Id.ToString();
            }
            else
            {
                if ((IoSsn != null) && !(String.IsNullOrEmpty(IoSsn)))
                {
                    selectionValue = "ssn:" + IoSsn;
                }
            }
            return selectionValue;
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
            // Only perform the override if the two statuses differ...
            if (newStatusId != oldStatusId)
            {
                IWorkStatusDao workStatusDao = daoFactory.GetWorkStatusDao();
                IReminderEmailDao reminderEmailDao = daoFactory.GetReminderEmailDao();

                WorkStatus newStatus = workStatusDao.GetById(newStatusId);
                WorkStatus oldStatus = workStatusDao.GetById(oldStatusId);

                // Check if overriding FROM a cancel status...
                if (oldStatus.StatusCodeType.IsCancel == true)
                {
                    if (LODMedical != null)
                    {
                        LODMedical.PhysicianCancelReason = 0;
                        LODMedical.PhysicianCancelExplanation = null;
                    }

                    IsFormalCancelRecommended = false;
                    AppointingCancelReasonId = null;
                    AppointingCancelExplanation = null;
                    ApprovingCancelReasonId = null;
                    ApprovingCancelExplanation = null;
                }

                // Check if overriding TO a cancel status...
                if (newStatus.StatusCodeType.IsCancel == true)
                {
                    if (this.Formal)
                    {
                        // The value 8 is currently arbitrary...it does not map to an enum value or a lookup table in the database...the NextAction code behind checks for the value 8. Unsure as to why 8 was chosen.
                        IsFormalCancelRecommended = true;
                        AppointingCancelReasonId = 8;
                        AppointingCancelExplanation = "Case Cancelled by System Admin";
                        ApprovingCancelReasonId = 8;
                        ApprovingCancelExplanation = "Case Cancelled by System Admin";
                    }
                    else
                    {
                        if (LODMedical != null)
                        {
                            // The value 8 is currently arbitrary...it does not map to an enum value or a lookup table in the database...the NextAction code behind checks for the value 8. Unsure as to why 8 was chosen.
                            LODMedical.PhysicianCancelReason = 8;
                            LODMedical.PhysicianCancelExplanation = "Case Cancelled by System Admin";
                        }
                    }
                }

                // Update the formal flag based on the status the case is being overridden to, but only when overriding to a non final status...
                if (newStatus.StatusCodeType.IsFinal == false)
                {
                    if (newStatus.StatusCodeType.IsFormal == true)
                    {
                        this.Formal = true;
                    }
                    else
                    {
                        this.Formal = false;
                    }
                }

                // Update the reminder email for this case
                reminderEmailDao.ReminderEmailUpdateStatusChange(oldStatusId, newStatusId, this.CaseId, "LOD");

                // Set the new status for the workflow
                this.Status = newStatusId;
                this.WorkflowStatus = newStatus;

                UpdateIsPostProcessingComplete(daoFactory);
            }
        }

        public virtual void RemoveSignature(IDaoFactory daoFactory, int statusOut, int statusCodeOut)
        {
            ISignatueMetaDateDao sigDao = daoFactory.GetSigMetaDataDao();
            sigDao.DeleteForWorkStatus(Id, Workflow, statusOut);

            if (sigDao != null)
                if ((int)LodStatusCode.FormalActionByAppointingAuthority == statusCodeOut && LODInvestigation != null)
                {
                    LODInvestigation.DateSignedAppointing = null;
                    LODInvestigation.SignatureInfoAppointing = null;
                }

            if ((int)LodStatusCode.FormalInvestigation == statusCodeOut && LODInvestigation != null)
            {
                LODInvestigation.DateSignedIO = null;
                LODInvestigation.SignatureInfoIO = null;
            }
        }

        #region ITrackingSource Members

        /// <inheritdoc/>
        public virtual byte ModuleType
        {
            get { return 2; }
        }

        /// <inheritdoc/>
        public virtual int TrackingId
        {
            get { return Id; }
        }

        #endregion ITrackingSource Members

        #region IDocumentSource Members

        public virtual IDictionary<string, bool> Active
        {
            get
            {
                if (_catActive == null)
                    _catActive = new Dictionary<string, bool>();

                return _catActive;
            }

            set { _catActive = value; }
        }

        public virtual string DocumentEntityId
        {
            get { return MemberSSN; }
        }

        public virtual long? DocumentGroupId { get; set; }

        public virtual int DocumentViewId
        {
            get { return (int)DocumentViewType.LineOfDuty; }
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

        #region Constructor

        public ANGLineOfDuty()
        {
            Active.Add(DocumentType.LODSupportingDocument.ToString(), false);
            Active.Add(DocumentType.MilitaryMedicalDocumentation.ToString(), false);
            Active.Add(DocumentType.CivilianMedicalDocumentation.ToString(), false);
            Active.Add(DocumentType.Labs.ToString(), false);
            Active.Add(DocumentType.RadiologyAndImaging.ToString(), false);
            Active.Add(DocumentType.Studies.ToString(), false);
            Active.Add(DocumentType.SpecialtyConsults.ToString(), false);
            Active.Add(DocumentType.ProofOfMilitaryStatus.ToString(), false);
            Active.Add(DocumentType.AutopsyReportDeathCertificate.ToString(), false);
            Active.Add(DocumentType.Maps.ToString(), false);
            Active.Add(DocumentType.AccidentReport.ToString(), false);
            Active.Add(DocumentType.MembersStatement.ToString(), false);
            Active.Add(DocumentType.Miscellaneous.ToString(), false);
            Active.Add(DocumentType.UntimelySubmissionOfIncidentReport.ToString(), false);
            Active.Add(DocumentType.SignedNotificationMemo.ToString(), false);
            Active.Add(DocumentType.AmendedDocuments.ToString(), false);

            //MedicalOfficerSignature = new SignatureEntry();
            //UnitCommanderSignature = new SignatureEntry();
            //JudgeAdvocateSignature = new SignatureEntry();
            //AppointingAuthoritySignature = new SignatureEntry();
            //BoardMedicalSignature = new SignatureEntry();
            //BoardLegalSignature = new SignatureEntry();
            //BoardAdminSignature = new SignatureEntry();
            //ApprovalAuthoritySignature = new SignatureEntry();
            //LODProgramManagerSignature = new SignatureEntry();

            MemberRank = new ALOD.Core.Domain.Lookup.UserRank();
        }

        #endregion Constructor

        #region AddFindings

        public virtual LineOfDutyFindings FindByType(short pType)
        {
            IEnumerable<LineOfDutyFindings> lst = (from p in LodFindings where p.PType == pType select p);
            if (lst.Count() > 0)
            {
                LineOfDutyFindings old = lst.First();
                return old;
            }
            return null;
        }

        public virtual bool HasAFinding(LineOfDutyFindings finding)
        {
            if (finding == null)
                return false;

            if (!finding.Finding.HasValue || finding.Finding.Value == 0)
                return false;

            return true;
        }

        public virtual LineOfDutyFindings SetFindingByType(LineOfDutyFindings fnd)
        {
            int i = 0;

            if (LodFindings.Count > 0) // Correct
            {
                foreach (LineOfDutyFindings item in LodFindings)
                {
                    if (item.PType == fnd.PType)
                    {
                        LodFindings.ElementAt(i).Modify(fnd);
                        return LodFindings.ElementAt(i);
                    }
                    i = i + 1;
                }
            }
            LodFindings.Add(fnd);
            return LodFindings.ElementAt(i);
        }

        public virtual LineOfDutyFindings SetPersonalByType(LineOfDutyFindings fnd)
        {
            int i = 0;

            if (LodFindings.Count > 0) // Correct
            {
                foreach (LineOfDutyFindings item in LodFindings)
                {
                    if (item.PType == fnd.PType)
                    {
                        LodFindings.ElementAt(i).ModifyPersonal(fnd);
                        return LodFindings.ElementAt(i);
                    }
                    i = i + 1;
                }
            }
            LodFindings.Add(fnd);
            return LodFindings.ElementAt(i);
        }

        #endregion AddFindings

        #region "MemberMemoCreated"

        public virtual bool NotificationMemoCreated(IMemoDao memoDao)
        {
            if (!WorkflowStatus.StatusCodeType.IsFinal)
            {
                return false;
            }
            short? notificationType;
            //Select memo type to look for
            if ((FinalFindings == (byte)ALOD.Core.Utils.Finding.In_Line_Of_Duty) || (FinalFindings == (byte)ALOD.Core.Utils.Finding.Epts_Service_Aggravated))
            {
                if (LODMedical.DeathInvolved == "Yes")
                {
                    notificationType = (byte)MemoType.LodFindingsILODDeath;
                }
                else
                {
                    notificationType = (byte)MemoType.LodFindingsILOD;
                }
            }
            else
            {
                if (LODMedical.DeathInvolved == "Yes")
                {
                    notificationType = (byte)MemoType.LodFindingsNLODDeath;
                }
                else
                {
                    notificationType = (byte)MemoType.LodFindingsNLOD;
                }
            }
            if (!notificationType.HasValue)
            {
                return false;
            }

            IList<Memorandum> memolist = (from m in memoDao.GetByRefId(Id) where m.Deleted == false && m.Template.Id == notificationType select m).ToList<Memorandum>();
            if (memolist.Count > 0)
            {
                return true;
            }

            return false;
        }

        #endregion "MemberMemoCreated"

        #region workflow options and rules

        public virtual bool FormalInvestigationRecommended
        {
            get
            {
                LineOfDutyFindings wingCcFindings = FindByType((short)PersonnelTypes.APPOINT_AUTH);
                bool recommended = false;

                if (wingCcFindings != null && wingCcFindings.Finding.HasValue)
                {
                    if (wingCcFindings.Finding.Value == (short)LodFinding.RecommendFormalInvestigation ||
                        wingCcFindings.Finding.Value == (short)LodFinding.NotInLineOfDutyDueToOwnMisconduct ||
                        wingCcFindings.Finding.Value == (short)LodFinding.NotInLineOfDutyNotDueToOwnMisconduct)
                    {
                        recommended = true;
                    }
                }

                return recommended;
            }
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

        public virtual IEnumerable<WorkflowStatusOption> GetCurrentOptions(int lastStatus, IDaoFactory daoFactory, int userId)
        {
            Validations.Clear();
            Validate(userId);
            ProcessDocuments(daoFactory);
            ProcessOption(lastStatus, daoFactory);
            return RuleAppliedOptions;
        }

        public virtual bool IsReconductFormalInvestigationRequested(ILookupDao lookupDao)
        {
            // Currently only formal LOD cases which are reinvestigations of prior LOD cases can have their formal investigations reconducted...
            if (this.Formal == false)
                return false;

            if (lookupDao.GetIsReinvestigationLod(this.Id) == false)
                return false;

            LineOfDutyFindings wingCCFindings = FindByType((short)PersonnelTypes.FORMAL_APP_AUTH);

            if (wingCCFindings == null || wingCCFindings.Finding.HasValue == false)
                return false;

            if (wingCCFindings.Finding.Value != (short)LodFinding.RecommendFormalInvestigation)
                return false;

            return true;
        }

        public virtual void ProcessDocuments(IDaoFactory daoFactory)
        {
            IDocumentDao docDao = daoFactory.GetDocumentDao();
            IDocCategoryViewDao docViewDao = daoFactory.GetDocCategoryViewDao();

            if (DocumentGroupId != null)
            {
                Documents = docDao.GetDocumentsByGroupId(DocumentGroupId.Value);

                List<DocumentCategory2> viewCats = (List<DocumentCategory2>)docViewDao.GetCategoriesByDocumentViewId(DocumentViewId);

                //Add addiotnal documents to currently active categories based on the lod status
                //    AddAddtionaCategories();
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
                                        if (docName == DocumentType.AutopsyReportDeathCertificate.ToString())
                                            if (LODMedical.DeathInvolved != "Yes")
                                            {
                                                isValid = true;
                                                isReqd = false;
                                            }
                                        if (docName == DocumentType.Maps.ToString() || docName == DocumentType.AccidentReport.ToString())
                                            if (LODMedical.MvaInvolved != "Yes")
                                            {
                                                isValid = true;
                                                isReqd = false;
                                            }

                                        //if the member did notify the RMU in a timely manner then "Untimely Submission of Incident" is not required
                                        if (docName == DocumentType.UntimelySubmissionOfIncidentReport.ToString())
                                        {
                                            if (TimelyMemberNotify.HasValue)
                                            {
                                                if (TimelyMemberNotify == true)
                                                {
                                                    isValid = true;
                                                    isReqd = false;
                                                }
                                            }
                                            else
                                            {
                                                isValid = true;
                                                isReqd = false;
                                            }
                                        }

                                        AddReqdDocs(docName, isValid);
                                        if (Active.ContainsKey(docName))
                                        {
                                            if (!isValid)
                                            {
                                                //DocType    //Description
                                                description = (from p in viewCats where p.DocCatId.ToString() == docName select p.CategoryDescription).Single();
                                                AddValidationItem(new ValidationItem("Documents", docName, description + "  document not found."));
                                            }
                                            Active[docName] = isReqd;
                                        }
                                    }
                                }
                                break;
                        }
                    }
                }
            }
        }

        public virtual void Validate(int userid)
        {
            LineOfDutyFindings IOFinding = FindByType((short)PersonnelTypes.IO);
            LineOfDutyFindings UnitFinding = FindByType((short)PersonnelTypes.UNIT_CMDR);
            //Since findings are stored in a seperate table
            foreach (var opt in WorkflowStatus.WorkStatusOptionList)
            {
                foreach (var rule in opt.RuleList)
                {
                    {
                        switch (rule.RuleTypes.Name.ToLower())
                        {
                            case LODMOduleType.Medical:
                                ValidateModule(LODMOduleType.Medical, LODMedical, userid);
                                break;

                            case LODMOduleType.Unit:
                                LODUnit.UnitFinding = UnitFinding;
                                ValidateModule(LODMOduleType.Unit, LODUnit, userid);
                                break;

                            case LODMOduleType.Investigation:
                                LODInvestigation.IOFinding = IOFinding;
                                ValidateModule(LODMOduleType.Investigation, LODInvestigation, userid);
                                break;
                        }
                    }
                }
            }
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
            IMemoDao memoDao = daoFactory.GetMemoDao();
            ILookupDao lookupDao = daoFactory.GetLookupDao();

            //last status should be the curren
            string[] statuses;
            IList<WorkStatusTracking> trackingData;

            bool allExist;
            bool oneExist;
            bool isValid;
            bool isVisible;

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
                        //If the last status was either of these status codes then the option should not be visible
                        //Example-if coming from app_auth or review board fwd to med off should not be visible (med tech section)
                        statuses = r.RuleValue.ToString().Split(',');
                        if (statuses.Contains(lastStatus.ToString()))
                        {
                            o.OptionVisible = false;
                        }

                        break;

                    case "wasinstatus":
                        isVisible = false;
                        statuses = r.RuleValue.ToString().Split(',');

                        trackingData = lookupDao.GetStatusTracking(this.Id, this.ModuleType);

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

                        trackingData = lookupDao.GetStatusTracking(this.Id, this.ModuleType);

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

                    case "formal":
                        if ((Convert.ToBoolean(r.RuleValue.ToString().ToLower()) == true))
                        {
                            o.OptionVisible = false;
                        }

                        break;

                    case "sarcrestricted":
                        bool restricted = Boolean.Parse(r.RuleValue);
                        o.OptionVisible = (restricted == IsRestricted);
                        break;

                    case "injurynonmvanonepts":

                        o.OptionVisible = false;

                        if (!string.IsNullOrEmpty(LODMedical.MvaInvolved) && LODMedical.MvaInvolved == "No")
                        {
                            LineOfDutyFindings mvaFindings = FindByType((short)PersonnelTypes.APPOINT_AUTH);
                            o.OptionVisible = false;

                            if (mvaFindings != null && mvaFindings.Finding.HasValue)
                            {
                                //                                o.OptionVisible = (mvaFindings.Finding.Value == (int)LodFinding.InLineOfDuty);
                                if ((mvaFindings.Finding.Value == (int)LodFinding.EptsServiceAgravated) ||  // service aggravated is considered ILOD
                                    (mvaFindings.Finding.Value == (int)LodFinding.InLineOfDuty))
                                {
                                    o.OptionVisible = true;
                                }
                            }
                        }

                        break;

                    case "formalrecommended":

                        LineOfDutyFindings wingCcFindings = FindByType((short)PersonnelTypes.APPOINT_AUTH);
                        bool rule = Boolean.Parse(r.RuleValue);
                        bool recommended = false;

                        if (wingCcFindings != null && wingCcFindings.Finding.HasValue)
                        {
                            if (wingCcFindings.Finding.Value == (short)LodFinding.RecommendFormalInvestigation ||
                                wingCcFindings.Finding.Value == (short)LodFinding.NotInLineOfDutyDueToOwnMisconduct ||
                                wingCcFindings.Finding.Value == (short)LodFinding.NotInLineOfDutyNotDueToOwnMisconduct)
                            {
                                recommended = true;
                            }
                        }

                        o.OptionVisible = (rule == recommended);
                        break;

                    case "icd9codenotpresent":

                        //if the selected diagnosis is a disease, this rule fails
                        bool icd9Present = false;

                        if (LODMedical.ICD9Id.HasValue)
                        {
                            ICD9Code code = lookupDao.GetIcd9ById(LODMedical.ICD9Id.Value);

                            if (code != null && code.IsDisease)
                            {
                                icd9Present = true;
                            }
                        }

                        o.OptionVisible = !icd9Present;
                        break;

                    case "cancelformalrecommended":
                        o.OptionVisible = IsFormalCancelRecommended;
                        break;

                    case "isreinvestigationlod":
                        bool isRLod = lookupDao.GetIsReinvestigationLod(this.Id);

                        if (isRLod == Convert.ToBoolean(r.RuleValue.ToString().ToLower()))
                        {
                            o.OptionVisible = true;
                        }
                        else
                        {
                            o.OptionVisible = false;
                        }

                        break;

                    case "ispersonnelfindingpresent":
                        // Rule Value Format: personnelType,LodFinding
                        LineOfDutyFindings lodFindings = null;
                        string[] ruleData = r.RuleValue.Split(',');

                        if (ruleData.Length != 2)
                        {
                            o.OptionVisible = false;
                            break;
                        }

                        lodFindings = FindByType(short.Parse(ruleData[0]));

                        if (lodFindings == null || lodFindings.Finding.HasValue == false)
                        {
                            o.OptionVisible = false;
                            break;
                        }

                        if (lodFindings.Finding.Value != short.Parse(ruleData[1]))
                        {
                            o.OptionVisible = false;
                            break;
                        }

                        o.OptionVisible = true;
                        break;

                    case "unitccquestions":
                        if (HasCredibleService.HasValue && WasMemberOnOrders.HasValue)
                            o.OptionVisible = true;
                        else
                            o.OptionVisible = false;

                        break;
                }
            }

            if (r.RuleTypes.ruleTypeId == (int)RuleKind.Validation)
            {
                //Validation Rule
                switch (r.RuleTypes.Name.ToLower())
                {
                    case LODMOduleType.Unit:

                        if (!(bool)ModuleStatus[r.RuleTypes.Name.ToLower()])
                        {
                            o.OptionValid = (bool)ModuleStatus[r.RuleTypes.Name.ToLower()];
                        }

                        break;

                    case LODMOduleType.Medical:
                        if (!(bool)ModuleStatus[r.RuleTypes.Name.ToLower()])
                        {
                            o.OptionValid = (bool)ModuleStatus[r.RuleTypes.Name.ToLower()];
                        }

                        break;

                    case LODMOduleType.Investigation:
                        if (!(bool)ModuleStatus[r.RuleTypes.Name.ToLower()])
                        {
                            o.OptionValid = (bool)ModuleStatus[r.RuleTypes.Name.ToLower()];
                        }

                        break;

                    case "wingccioassigned":

                        bool passed = true;

                        if (string.IsNullOrEmpty(IoSsn) && AppointedIO == null)
                        {
                            passed = false;
                            AddValidationItem(new ValidationItem("Formal Investigation", "Investigating Officer", "Investigating Officer must be selected"));
                        }

                        if (!IoCompletionDate.HasValue)
                        {
                            passed = false;
                            AddValidationItem(new ValidationItem("Formal Investigation", "Investigation Completion Date", "Date investigation to be completed by must be entered"));
                        }

                        if (IoCompletionDate.HasValue)
                        {
                            if (IoCompletionDate.Value < DateTime.Now)
                            {
                                passed = false;
                                AddValidationItem(new ValidationItem("Formal Investigation", "Investigation Completion Date", "Date Investigation to be completed by can not be a past date."));
                            }
                        }
                        o.OptionValid = passed;

                        break;

                    case "wingccfindings":
                        LineOfDutyFindings wingCcFindings = FindByType((short)PersonnelTypes.APPOINT_AUTH);

                        if (wingCcFindings != null && wingCcFindings.Finding.HasValue)
                        {
                            o.OptionValid = true;
                        }
                        else
                        {
                            o.OptionValid = false;
                            AddValidationItem(new ValidationItem("Findings", "Appointing Authority", "Appointing Authority Findings not present", true));
                        }

                        break;

                    case "wingccfindingsforcomplete":
                        LineOfDutyFindings wcfnd = FindByType((short)PersonnelTypes.APPOINT_AUTH);

                        if (wcfnd != null && wcfnd.Finding.HasValue && wcfnd.Finding == (short)LodFinding.InLineOfDuty)
                        {
                            o.OptionValid = true;
                        }
                        else
                        {
                            o.OptionValid = false;
                            AddValidationItem(new ValidationItem("Findings", "Appointing Authority Complete", "Appointing Authority Findings not enough to allow closing.", true));
                        }

                        break;

                    case "wingccformalaction":

                        LineOfDutyFindings wingCcFormalFindings = FindByType((short)PersonnelTypes.FORMAL_APP_AUTH);

                        if (wingCcFormalFindings != null)
                        {
                            if (wingCcFormalFindings.DecisionYN == "Y")
                            {
                                o.OptionValid = true;
                            }
                            else if (wingCcFormalFindings.DecisionYN == "N")
                            {
                                if (wingCcFormalFindings.Finding.HasValue)
                                {
                                    if (wingCcFormalFindings.Finding.Value != (short)LodFinding.RecommendFormalInvestigation)
                                    {
                                        o.OptionValid = true;
                                    }
                                    else
                                    {
                                        o.OptionValid = false;
                                        AddValidationItem(new ValidationItem("Findings", "Formal Appointing Authority", "Formal Investigation has been directed.", true));
                                    }
                                }
                                else
                                {
                                    o.OptionValid = false;
                                    AddValidationItem(new ValidationItem("Findings", "Formal Appointing Authority", "Formal Action by Appointing Authority not present", true));
                                }
                            }
                            else
                            {
                                o.OptionValid = false;
                                AddValidationItem(new ValidationItem("Findings", "Formal Appointing Authority", "Formal Action by Appointing Authority not present", true));
                            }
                        }
                        else
                        {
                            o.OptionValid = false;
                            AddValidationItem(new ValidationItem("Findings", "Formal Appointing Authority", "Formal Action by Appointing Authority not present", true));
                        }

                        break;

                    case "wingjafindings":
                        //Needed for completing the lod from board
                        LineOfDutyFindings wingJaFindings = FindByType((short)PersonnelTypes.WING_JA);

                        if (wingJaFindings != null)
                        {
                            if (wingJaFindings.DecisionYN == "Y")
                            {
                                o.OptionValid = true;
                            }
                            else if (wingJaFindings.DecisionYN == "N")
                            {
                                if (wingJaFindings.Finding.HasValue)
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
                                o.OptionValid = false;
                            }
                        }
                        else
                        {
                            o.OptionValid = false;
                        }

                        if (!o.OptionValid)
                        {
                            AddValidationItem(new ValidationItem("Findings", "Wing JA", "Wing Judge Advocate Findings not present", true));
                        }

                        break;

                    case "wingjaformalfindings":
                        //Needed for completing the lod from board
                        LineOfDutyFindings wingJaFormalFindings = FindByType((short)PersonnelTypes.FORMAL_WING_JA);

                        if (wingJaFormalFindings != null)
                        {
                            if (wingJaFormalFindings.DecisionYN == "Y")
                            {
                                o.OptionValid = true;
                            }
                            else if (wingJaFormalFindings.DecisionYN == "N")
                            {
                                if (wingJaFormalFindings.Finding.HasValue)
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
                                o.OptionValid = false;
                            }
                        }
                        else
                        {
                            o.OptionValid = false;
                        }

                        if (!o.OptionValid)
                        {
                            AddValidationItem(new ValidationItem("Findings", "Formal Wing JA", "Wing Judge Advocate Formal Action not present", true));
                        }

                        break;

                    case "boardfindingspresent":
                        //Needed for completing the lod from board
                        LineOfDutyFindings hqaaFinding = FindByType((short)PersonnelTypes.BOARD_AA);
                        LineOfDutyFindings hqbdFinding = FindByType((short)PersonnelTypes.BOARD);

                        if (hqaaFinding != null && hqaaFinding.Finding.HasValue)
                        {
                            o.OptionValid = true;
                        }
                        else if (hqbdFinding != null && hqbdFinding.Finding.HasValue)
                        {
                            if (ApprovingAuthorityUserId.HasValue)
                            {
                                o.OptionValid = true;
                            }
                            else
                            {
                                AddValidationItem(new ValidationItem("Findings", "Approving Authority", "Board Technician must select an Approving Authority on the LOD Board Page in order to complete the case without going throught the Approving Authority Action step."));
                                o.OptionValid = false;
                            }
                        }
                        else
                        {
                            AddValidationItem(new ValidationItem("Findings", "Approving Authority", "Approving Authority Findings not found."));
                            o.OptionValid = false;
                        }

                        break;

                    case "boarda1findingspresent":
                        isValid = true;

                        // Needed for returning the LOD from Board A1 to Board Tech with findings
                        LineOfDutyFindings findings = FindByType((short)PersonnelTypes.BOARD_A1);

                        // Either they have there findings or they concur
                        if (!(findings != null && (findings.Finding.HasValue || findings.DecisionYN == "Y")))
                        {
                            AddValidationItem(new ValidationItem("Findings", "Board Admin", "Board Admin findings not found."));
                            isValid = false;
                        }

                        if (!ValidateBoardA1Properties(true))
                        {
                            isValid = false;
                        }

                        o.OptionValid = isValid;

                        break;

                    case "boardmedicalfindingspresent":
                        //Needed for returning  the lod  from medical board to board with findings
                        LineOfDutyFindings hqsgFinding = FindByType((short)PersonnelTypes.BOARD_SG);

                        //Either they have there findings or they concur
                        if ((hqsgFinding != null && (hqsgFinding.Finding.HasValue || hqsgFinding.DecisionYN == "Y")))
                        {
                            o.OptionValid = true;
                        }
                        else
                        {
                            AddValidationItem(new ValidationItem("Findings", "Board Surgeon", "Board Surgeon findings not found."));
                            o.OptionValid = false;
                        }

                        break;

                    case "boardlegalfindingspresent":
                        //Needed for returning  the lod  from legal board to board with findings
                        LineOfDutyFindings hqjaFinding = FindByType((short)PersonnelTypes.BOARD_JA);
                        //Either they have there findings or tehy concur
                        if ((hqjaFinding != null && (hqjaFinding.Finding.HasValue || hqjaFinding.DecisionYN == "Y")))
                        {
                            o.OptionValid = true;
                        }
                        else
                        {
                            AddValidationItem(new ValidationItem("Findings", "Board Legal", "Board Legal findings not found."));
                            o.OptionValid = false;
                        }

                        break;

                    case "boardsrfindingspresent":
                        //Needed for returning  the lod  from senior reviewer to board with findings
                        LineOfDutyFindings hqsrFinding = FindByType((short)PersonnelTypes.BOARD_SR);
                        //Either they have there findings or tehy concur
                        if ((hqsrFinding != null && (hqsrFinding.Finding.HasValue || hqsrFinding.DecisionYN == "Y")))
                        {
                            o.OptionValid = true;
                        }
                        else
                        {
                            AddValidationItem(new ValidationItem("Findings", "Board Senior Reviewer", "Board Senior Reviewer findings not found."));
                            o.OptionValid = false;
                        }

                        break;

                    case "boardaafindingspresent":
                        //Needed for returning  the lod  from approving authority to board with findings
                        LineOfDutyFindings hqaauthFinding = FindByType((short)PersonnelTypes.BOARD_AA);
                        //Either they have there findings or tehy concur
                        if ((hqaauthFinding != null && (hqaauthFinding.Finding.HasValue)))
                        {
                            o.OptionValid = true;
                        }
                        else
                        {
                            AddValidationItem(new ValidationItem("Findings", "Approving Authority", "Approving Authority findings not found."));
                            o.OptionValid = false;
                        }

                        break;

                    case "formalboardfindingspresent":
                        //Needed for completing lod from formal board
                        LineOfDutyFindings hqforamlaaFinding = FindByType((short)PersonnelTypes.FORMAL_BOARD_AA);
                        LineOfDutyFindings hqformalraFinding = FindByType((short)PersonnelTypes.FORMAL_BOARD_RA);

                        if (hqforamlaaFinding != null && hqforamlaaFinding.Finding.HasValue)
                        {
                            o.OptionValid = true;
                        }
                        else if (hqformalraFinding != null && hqformalraFinding.Finding.HasValue)
                        {
                            if (ApprovingAuthorityUserId.HasValue)
                            {
                                o.OptionValid = true;
                            }
                            else
                            {
                                AddValidationItem(new ValidationItem("Findings", "Formal Approving Authority", "Board Technician must select an Approving Authority on the LOD Board Page in order to complete the case without going throught the Formal Approving Authority Action step."));
                                o.OptionValid = false;
                            }
                        }
                        else
                        {
                            AddValidationItem(new ValidationItem("Findings", "Formal Approving Authority", "Formal Approving Authority Findings not found."));
                            o.OptionValid = false;
                        }

                        break;

                    case "frmlboarda1findingspresent":
                        isValid = true;

                        LineOfDutyFindings formalFindings = FindByType((short)PersonnelTypes.FORMAL_BOARD_A1);

                        // Either they have there own findings or they concur
                        if (!(formalFindings != null && formalFindings.DecisionYN != null && formalFindings.DecisionYN != ""))
                        {
                            AddValidationItem(new ValidationItem("Findings", "Formal Board Admin", "Formal Board Admin findings not found."));

                            isValid = false;
                        }

                        if (!ValidateBoardA1Properties(true))
                        {
                            isValid = false;
                        }

                        o.OptionValid = isValid;

                        break;

                    case "frmlboardmedicalfindingspresent":
                        LineOfDutyFindings hqfrmlsgFinding = FindByType((short)PersonnelTypes.FORMAL_BOARD_SG);
                        //Either they have there own findings or they concur
                        if ((hqfrmlsgFinding != null && hqfrmlsgFinding.DecisionYN != null && hqfrmlsgFinding.DecisionYN != ""))
                        {
                            o.OptionValid = true;
                        }
                        else
                        {
                            AddValidationItem(new ValidationItem("Findings", "Formal Board Surgeon", "Formal Board Surgeon findings not found."));

                            o.OptionValid = false;
                        }

                        break;

                    case "frmlboardlegalfindingspresent":
                        LineOfDutyFindings hqfrmljaFinding = FindByType((short)PersonnelTypes.FORMAL_BOARD_JA);
                        //Either they have there own findings or they concur
                        if ((hqfrmljaFinding != null && hqfrmljaFinding.DecisionYN != null && hqfrmljaFinding.DecisionYN != ""))
                        {
                            o.OptionValid = true;
                        }
                        else
                        {
                            AddValidationItem(new ValidationItem("Findings", "Formal Board Legal", "Formal Board Legal findings not found."));
                            o.OptionValid = false;
                        }

                        break;

                    case "frmlboardsrfindingspresent":
                        LineOfDutyFindings hqfrmlsrFinding = FindByType((short)PersonnelTypes.FORMAL_BOARD_SR);
                        //Either they have there own findings or they concur

                        if ((hqfrmlsrFinding != null && hqfrmlsrFinding.DecisionYN != null && hqfrmlsrFinding.DecisionYN != ""))
                        {
                            o.OptionValid = true;
                        }
                        else
                        {
                            AddValidationItem(new ValidationItem("Findings", "Formal Board Senior", "Formal Board Senior Reviewer findings not found."));

                            o.OptionValid = false;
                        }

                        break;

                    case "frmlboardaafindingspresent":
                        LineOfDutyFindings hqfrmlaaFinding = FindByType((short)PersonnelTypes.FORMAL_BOARD_AA);
                        //Either they have there own findings or they concur
                        if (hqfrmlaaFinding != null && hqfrmlaaFinding.Finding.HasValue)
                        {
                            o.OptionValid = true;
                        }
                        else
                        {
                            AddValidationItem(new ValidationItem("Findings", "Formal Approving Authority", "Formal Approving Authority findings not found."));

                            o.OptionValid = false;
                        }

                        break;

                    case "boarda1questions":
                        if (!ValidateBoardA1Properties(false))
                        {
                            o.OptionValid = false;
                        }
                        else
                        {
                            o.OptionValid = true;
                        }
                        break;

                    case "appointedioverified":
                        isValid = true;

                        if (AppointedIO == null)
                        {
                            AddValidationItem(new ValidationItem("Formal Investigation", "Investigating Officer", "A valid Investigating Officer has not been appointed!"));
                            isValid = false;
                        }
                        else if (string.IsNullOrEmpty(AppointedIO.EDIPIN))
                        {
                            AddValidationItem(new ValidationItem("Formal Investigation", "Investigating Officer", "The appointed Investigating Officer does not have an EDIPI!"));
                            isValid = false;
                        }
                        else if (AppointedIO.AccountExpiration == null)
                        {
                            AddValidationItem(new ValidationItem("Formal Investigation", "Investigating Officer", "The appointed Investigating Officer does not have an IA Training Date!"));
                            isValid = false;
                        }
                        else if (AppointedIO.AccountExpiration.Value.Date <= DateTime.Now.Date)
                        {
                            AddValidationItem(new ValidationItem("Formal Investigation", "Investigating Officer", "The appointed Investigating Officer has an expired IA Training Date!"));
                            isValid = false;
                        }
                        else if (AppointedIO.Unit == null)
                        {
                            AddValidationItem(new ValidationItem("Formal Investigation", "Investigating Officer", "The appointed Investigating Officer does not belong to a unit!"));
                            isValid = false;
                        }

                        o.OptionValid = isValid;

                        break;

                    case "isnonmvainjuryandeptssa":
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
                                IList<Memorandum> memolist = (from m in memoDao.GetByRefId(Id) where m.Deleted == false && m.Template.Id == Convert.ToByte(memos[i]) select m).ToList<Memorandum>();

                                // Special Case: LOD is a reinvesitgation case & a new IO invesitgation is requested...need to make sure a new IO appointment memo is generated
                                //               instead of using the old one from the original LOD case...
                                if (lookupDao.GetIsReinvestigationLod(this.Id) && memolist.Count > 0 && (MemoType)Convert.ToByte(memos[i]) == MemoType.LodAppointIo)
                                {
                                    DateTime maxMemoCreatedDate = DateTime.MinValue;

                                    // Find most recent memo created date...
                                    foreach (Memorandum m in memolist)
                                    {
                                        if (m.Contents[0] != null && m.Contents[0].CreatedDate > maxMemoCreatedDate)
                                        {
                                            maxMemoCreatedDate = m.Contents[0].CreatedDate;
                                        }
                                    }

                                    // Check if most recent memo creation date precedes creation date of LOD record...
                                    if (maxMemoCreatedDate < this.CreatedDate)
                                    {
                                        // Only the LOD appointment memo for the original case exists, and not a new one for the reinvestigation LOD case...
                                        allExist = false;

                                        string description = (from m in memoDao.GetAllTemplates() where m.Id == Convert.ToByte(memos[i]) select m.Title).Single();
                                        AddValidationItem(new ValidationItem("Memos", "Memo", "New " + description + "  Memo  not found."));
                                    }
                                    else
                                    {
                                        oneExist = true;
                                    }
                                }
                                else if (memolist.Count > 0)
                                {
                                    oneExist = true;
                                }
                                else
                                {
                                    allExist = false;

                                    if (r.CheckAll.Value == true)
                                    {
                                        string description = (from m in memoDao.GetAllTemplates() where m.Id == Convert.ToByte(memos[i]) select m.Title).Single();
                                        AddValidationItem(new ValidationItem("Memos", "Memo", description + "  Memo  not found."));
                                    }
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
                        isValid = false;
                        allExist = true;
                        oneExist = false;

                        //This block is to make sure if there are more then one docs then isvalid is set after checking all the docs are present
                        for (int i = 0; i < docs.Length; i++)
                        {
                            if (!String.IsNullOrEmpty(docs[i]))
                            {
                                string docName = docs[i];
                                isValid = false;
                                if (Active.ContainsKey(docName))
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

        private void UpdateActiveCategories(string section, bool isReqd)
        {
            if (Active.ContainsKey(section))
                Active[section] = isReqd;
            if (!Active.ContainsKey(section))
                Active.Add(section, isReqd);
        }

        private bool ValidateBoardA1Properties(bool forCompletion)
        {
            bool valid = true;
            string preValidationText = String.Empty;

            if (forCompletion)
            {
                preValidationText = "Board Admin must ";
            }
            else
            {
                preValidationText = "Must ";
            }

            if (!HasCredibleService.HasValue)
            {
                AddValidationItem(new ValidationItem("Findings", "HasCredibleService", preValidationText + "select Yes or No for if the member has 8 years of credible service."));
                //AddValidationItem(new ValidationItem("Unit", "HasCredibleService", preValidationText + "select Yes or No for if the member has 8 years of credible service."));
                valid = false;
            }

            if (!WasMemberOnOrders.HasValue)
            {
                AddValidationItem(new ValidationItem("Findings", "WasMemberOnOrders", preValidationText + "select Yes or No for if the member was on orders of 31 days or more."));
                //AddValidationItem(new ValidationItem("Unit", "WasMemberOnOrders", preValidationText + "select Yes or No for if the member was on orders of 31 days or more."));
                valid = false;
            }

            return valid;
        }

        private void ValidateModule(string section, IValidatable item, int userid)
        {
            bool isValid = true;
            isValid = item.Validate(userid);
            AddValidationItem(item.ValidationItems);
            AddModuleStatus(section, isValid);
        }

        #endregion workflow options and rules

        #region MyAccess

        public virtual Dictionary<String, PageAccessType> ReadSectionList(int role)
        {
            PageAccessType access;
            access = PageAccessType.None;

            Dictionary<String, PageAccessType> scAccessList = new Dictionary<String, PageAccessType>();
            if (CurrentStatusCode == (int)LodStatusCode.Complete)
            {
                access = PageAccessType.ReadOnly;
            }

            //'Add all pages as readonly

            scAccessList.Add(SectionNames.MED_TECH_REV.ToString(), access); ;
            scAccessList.Add(SectionNames.MED_OFF_REV.ToString(), access);
            scAccessList.Add(SectionNames.UNIT_CMD_REV.ToString(), access);
            scAccessList.Add(SectionNames.APP_AUTH_REV.ToString(), access);
            scAccessList.Add(SectionNames.APPOINT_IO.ToString(), access);
            scAccessList.Add(SectionNames.NOTIFY_IO.ToString(), access);
            scAccessList.Add(SectionNames.IO.ToString(), access);
            scAccessList.Add(SectionNames.WING_JA_REV.ToString(), access);
            scAccessList.Add(SectionNames.FORMAL_ACTION_APP_AUTH.ToString(), access);
            scAccessList.Add(SectionNames.FORMAL_ACTION_WING_JA.ToString(), access);

            access = PageAccessType.ReadOnly;
            scAccessList.Add(SectionNames.BOARD_REV.ToString(), access);
            scAccessList.Add(SectionNames.BOARD_MED_REV.ToString(), access);
            scAccessList.Add(SectionNames.BOARD_LEGAL_REV.ToString(), access);
            scAccessList.Add(SectionNames.BOARD_APPROVING_AUTH_REV.ToString(), access);
            scAccessList.Add(SectionNames.BOARD_SENIOR_REV.ToString(), access);
            scAccessList.Add(SectionNames.BOARD_PERSONNEL_REV.ToString(), access);

            if (!Formal)
            {
                access = PageAccessType.None;
            }
            scAccessList.Add(SectionNames.FORMAL_BOARD_REV.ToString(), access);
            scAccessList.Add(SectionNames.FORMAL_BOARD_MED_REV.ToString(), access);
            scAccessList.Add(SectionNames.FORMAL_BOARD_LEGAL_REV.ToString(), access);
            scAccessList.Add(SectionNames.FORMAL_BOARD_APPROVING_AUTH_REV.ToString(), access);
            scAccessList.Add(SectionNames.FORMAL_BOARD_SENIOR_REV.ToString(), access);
            scAccessList.Add(SectionNames.FORMAL_BOARD_PERSONNEL_REV.ToString(), access);

            access = PageAccessType.ReadOnly;

            scAccessList.Add(SectionNames.RLB.ToString(), access); ;

            //Modify access with user role
            switch (role)
            {
                case (int)UserGroups.MedicalTechnician:
                    if (CurrentStatusCode == (int)LodStatusCode.MedTechReview)
                    {
                        scAccessList[SectionNames.MED_TECH_REV.ToString()] = PageAccessType.ReadWrite;
                        scAccessList[SectionNames.RLB.ToString()] = PageAccessType.ReadWrite;
                    }
                    break;

                case (int)UserGroups.MedicalOfficer:
                    if (CurrentStatusCode == (int)LodStatusCode.MedicalOfficerReview)
                    {
                        scAccessList[SectionNames.MED_OFF_REV.ToString()] = PageAccessType.ReadWrite;
                        scAccessList[SectionNames.RLB.ToString()] = PageAccessType.ReadWrite;
                    }
                    break;

                case (int)UserGroups.UnitCommander:
                    if (CurrentStatusCode == (int)LodStatusCode.UnitCommanderReview)
                    {
                        scAccessList[SectionNames.UNIT_CMD_REV.ToString()] = PageAccessType.ReadWrite;
                        scAccessList[SectionNames.RLB.ToString()] = PageAccessType.ReadWrite;
                    }
                    break;

                case (int)UserGroups.WingCommander:
                    if (CurrentStatusCode == (int)LodStatusCode.AppointingAutorityReview)
                    {
                        scAccessList[SectionNames.APP_AUTH_REV.ToString()] = PageAccessType.ReadWrite;
                        scAccessList[SectionNames.RLB.ToString()] = PageAccessType.ReadWrite;
                        scAccessList[SectionNames.APPOINT_IO.ToString()] = PageAccessType.ReadWrite;
                    }

                    if (CurrentStatusCode == (int)LodStatusCode.FormalActionByAppointingAuthority)
                    {
                        scAccessList[SectionNames.FORMAL_ACTION_APP_AUTH.ToString()] = PageAccessType.ReadWrite;
                    }
                    break;

                case (int)UserGroups.InvestigatingOfficer:
                    if (CurrentStatusCode == (int)LodStatusCode.FormalInvestigation)
                    {
                        scAccessList[SectionNames.IO.ToString()] = PageAccessType.ReadWrite;
                    }
                    break;

                case (int)UserGroups.WingJudgeAdvocate:
                    if (CurrentStatusCode == (int)LodStatusCode.WingJAReview)
                    {
                        scAccessList[SectionNames.WING_JA_REV.ToString()] = PageAccessType.ReadWrite;
                        scAccessList[SectionNames.RLB.ToString()] = PageAccessType.ReadWrite;
                    }
                    if (CurrentStatusCode == (int)LodStatusCode.FormalActionByWingJA)
                    {
                        scAccessList[SectionNames.FORMAL_ACTION_WING_JA.ToString()] = PageAccessType.ReadWrite;
                    }
                    break;

                case (int)UserGroups.BoardTechnician:
                    if (CurrentStatusCode == (int)LodStatusCode.BoardReview)
                    {
                        scAccessList[SectionNames.BOARD_REV.ToString()] = PageAccessType.ReadWrite;
                    }
                    if (CurrentStatusCode == (int)LodStatusCode.FormalBoardReview)
                    {
                        scAccessList[SectionNames.FORMAL_BOARD_REV.ToString()] = PageAccessType.ReadWrite;
                        scAccessList[SectionNames.RLB.ToString()] = PageAccessType.ReadWrite;
                    }
                    break;

                case (int)UserGroups.BoardMedical:
                    if (CurrentStatusCode == (int)LodStatusCode.BoardMedicalReview)
                    {
                        scAccessList[SectionNames.BOARD_MED_REV.ToString()] = PageAccessType.ReadWrite;
                    }
                    if (CurrentStatusCode == (int)LodStatusCode.FormalBoardMedicalReview)
                    {
                        scAccessList[SectionNames.FORMAL_BOARD_MED_REV.ToString()] = PageAccessType.ReadWrite;
                    }
                    break;

                case (int)UserGroups.BoardLegal:
                    if (CurrentStatusCode == (int)LodStatusCode.BoardLegalReview)
                    {
                        scAccessList[SectionNames.BOARD_LEGAL_REV.ToString()] = PageAccessType.ReadWrite;
                    }
                    if (CurrentStatusCode == (int)LodStatusCode.FormalBoardLegalReview)
                    {
                        scAccessList[SectionNames.FORMAL_BOARD_LEGAL_REV.ToString()] = PageAccessType.ReadWrite;
                    }
                    break;

                case (int)UserGroups.BoardApprovalAuthority:
                    if (CurrentStatusCode == (int)LodStatusCode.ApprovingAuthorityAction)
                    {
                        scAccessList[SectionNames.BOARD_APPROVING_AUTH_REV.ToString()] = PageAccessType.ReadWrite;
                    }
                    if (CurrentStatusCode == (int)LodStatusCode.FormalApprovingAuthorityAction)
                    {
                        scAccessList[SectionNames.FORMAL_BOARD_APPROVING_AUTH_REV.ToString()] = PageAccessType.ReadWrite;
                    }
                    break;

                case (int)UserGroups.MPF:
                    if (CurrentStatusCode == (int)LodStatusCode.NotifyFormalInvestigator)
                    {
                        scAccessList[SectionNames.NOTIFY_IO.ToString()] = PageAccessType.ReadWrite;
                    }
                    break;

                case (int)UserGroups.LOD_PM:
                    if (CurrentStatusCode == (int)LodStatusCode.NotifyFormalInvestigator)
                    {
                        scAccessList[SectionNames.NOTIFY_IO.ToString()] = PageAccessType.ReadWrite;
                    }
                    break;

                case (int)UserGroups.LOD_MFP:
                    if (CurrentStatusCode == (int)LodStatusCode.MedTechReview)
                    {
                        scAccessList[SectionNames.MED_TECH_REV.ToString()] = PageAccessType.ReadWrite;
                        scAccessList[SectionNames.RLB.ToString()] = PageAccessType.ReadWrite;
                    }
                    break;

                case (int)UserGroups.BoardAdministrator:
                    if (CurrentStatusCode == (int)LodStatusCode.BoardPersonnelReview)
                    {
                        scAccessList[SectionNames.BOARD_PERSONNEL_REV.ToString()] = PageAccessType.ReadWrite;
                    }
                    if (CurrentStatusCode == (int)LodStatusCode.FormalBoardPersonnelReview)
                    {
                        scAccessList[SectionNames.FORMAL_BOARD_PERSONNEL_REV.ToString()] = PageAccessType.ReadWrite;
                    }
                    break;

                default:
                    break;
            }
            return scAccessList;
        }

        #endregion MyAccess

        public virtual void UpdateIsPostProcessingComplete(IDaoFactory daoFactory)
        {
            IsPostProcessingComplete = DetermineIfPostProcessingIsComplete(daoFactory);
        }

        protected virtual bool HasDeterminationMemo(IDaoFactory daoFactory)
        {
            IMemoDao memoDao = daoFactory.GetMemoDao();
            int count = 0;

            if (FinalFindings == (short)Finding.In_Line_Of_Duty || FinalFindings == (short)Finding.Epts_Service_Aggravated)
            {
                // Both ILOD memos are checked without checking if a death is involved becuase the ILOD Death memo was added to the system
                // much later than the normal ILOD memo, which means that there exists legacy LOD cases that are ILOD with death involved
                // that had the normal ILOD memo generated.
                count = memoDao.GetByRefId(Id).Where(x => x.Deleted == false && (x.Template.Id == (int)MemoType.LodFindingsILOD || x.Template.Id == (int)MemoType.LodFindingsILODDeath)).ToList().Count;
            }
            else
            {
                if (LODMedical.DeathInvolved.Equals("Yes"))
                    count = memoDao.GetByRefId(Id).Where(x => x.Deleted == false && x.Template.Id == (int)MemoType.LodFindingsNLODDeath).ToList().Count;
                else
                    count = memoDao.GetByRefId(Id).Where(x => x.Deleted == false && x.Template.Id == (int)MemoType.LodFindingsNLOD).ToList().Count;
            }

            if (count > 0)
                return true;
            else
                return false;
        }

        protected virtual bool HasPostCompletionDigitalSignatureBeenGenerated(IDaoFactory daoFactory)
        {
            ISignatueMetaDateDao sigMetaDataDao = daoFactory.GetSigMetaDataDao();

            if (sigMetaDataDao.GetByWorkStatus(Id, Workflow, (int)LodWorkStatus.Complete) == null)
                return false;

            return true;
        }
    }
}