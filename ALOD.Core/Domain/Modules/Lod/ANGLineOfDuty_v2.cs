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
    public class ANGLineOfDuty_v2 : ANGLineOfDuty
    {
        public virtual string AppointingUnit { get; set; }
        public virtual Boolean DirectReply { get; set; }

        /// <inheritdoc/>
        public override int DocumentViewId
        {
            get { return (int)DocumentViewType.LineOfDuty_v2; }
        }

        public virtual LineOfDutyMedical_v2 LODMedical_v2
        {
            get
            {
                return (LineOfDutyMedical_v2)LODMedical;
            }
        }

        public virtual LineOfDutyUnit_v2 LODUnit_v2
        {
            get
            {
                return (LineOfDutyUnit_v2)LODUnit;
            }
        }

        public virtual int? NILODsubFinding { get; set; }

        #region workflow options and rules

        /// <inheritdoc/>
        public override bool FormalInvestigationRecommended
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

        /// <inheritdoc/>
        public override void AddSignature(IDaoFactory daoFactory, UserGroups groupId, string title, AppUser user)
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

            if (CurrentStatusCode == (int)LodStatusCode.ANGFormalActionByAppointingAuthority && LODInvestigation != null)
            {
                LODInvestigation.DateSignedAppointing = DateTime.Now;
                PersonnelData perapp = new PersonnelData();
                perapp.Name = user.FormName;
                perapp.Grade = user.Rank.Rank;
                perapp.SSN = user.SSN;
                perapp.PasCodeDescription = user.Unit.Name;
                perapp.Branch = "ANG";
                LODInvestigation.SignatureInfoAppointing = perapp;
            }

            if (CurrentStatusCode == (int)LodStatusCode.ANGFormalInvestigation && LODInvestigation != null)
            {
                LODInvestigation.DateSignedIO = DateTime.Now;
                PersonnelData perio = new PersonnelData();

                perio.Name = user.FormName;
                perio.Grade = user.Rank.Rank;
                perio.SSN = user.SSN;
                perio.PasCodeDescription = user.Unit.Name;
                perio.Branch = "ANG";
                LODInvestigation.SignatureInfoIO = perio;
            }
        }

        /// <inheritdoc/>
        public override IEnumerable<WorkflowStatusOption> GetCurrentOptions(int lastStatus, IDaoFactory daoFactory, int userId)
        {
            Validations.Clear();
            Validate(userId);
            ProcessDocuments(daoFactory);
            ProcessOption(lastStatus, daoFactory);
            return RuleAppliedOptions;
        }

        /// <inheritdoc/>
        public override bool IsReconductFormalInvestigationRequested(ILookupDao lookupDao)
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

        /// <inheritdoc/>
        public override void ProcessDocuments(IDaoFactory daoFactory)
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

        /// <inheritdoc/>
        public override void RemoveSignature(IDaoFactory daoFactory, int deleteWorkStatus, int newStatusCodeOut)
        {
            ISignatueMetaDateDao sigDao = daoFactory.GetSigMetaDataDao();
            sigDao.DeleteForWorkStatus(Id, Workflow, deleteWorkStatus);

            if ((int)LodStatusCode.ANGFormalActionByAppointingAuthority == newStatusCodeOut && LODInvestigation != null)
            {
                LODInvestigation.DateSignedAppointing = null;
                LODInvestigation.SignatureInfoAppointing = null;
            }

            if ((int)LodStatusCode.ANGFormalInvestigation == newStatusCodeOut && LODInvestigation != null)
            {
                LODInvestigation.DateSignedIO = null;
                LODInvestigation.SignatureInfoIO = null;
            }
        }

        /// <inheritdoc/>
        public override void Validate(int userid)
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
                                ValidateModule(LODMOduleType.Medical, LODMedical_v2, userid);
                                break;

                            case LODMOduleType.Unit:
                                LODUnit.UnitFinding = UnitFinding;
                                ValidateModule(LODMOduleType.Unit, LODUnit_v2, userid);
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
                        if ((Convert.ToBoolean(r.RuleValue.ToString().ToLower()) != Formal))
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
                                else if (mvaFindings.Finding.Value == (int)LodFinding.NotInLineOfDutyNotDueToOwnMisconduct)
                                {
                                    if (NILODsubFinding.HasValue && NILODsubFinding.Value == (int)NILODSubFindings.EPTS_NSA)
                                    {
                                        o.OptionVisible = true;
                                    }
                                }
                            }
                        }

                        break;

                    case "injury":
                        if (LODMedical.NatureOfIncidentDescription == "Injury (non MVA)") o.OptionVisible = true;
                        else o.OptionVisible = false;
                        break;

                    case "nonmva":
                        if (!string.IsNullOrEmpty(LODMedical.MvaInvolved) && LODMedical.MvaInvolved == "No") o.OptionVisible = true;
                        else o.OptionVisible = false;
                        break;

                    case "formalrecommended":

                        LineOfDutyFindings wingCcFindings = FindByType((short)PersonnelTypes.APPOINT_AUTH);
                        bool rule = Boolean.Parse(r.RuleValue);
                        bool recommended = false;

                        if (wingCcFindings != null && wingCcFindings.Finding.HasValue)
                        {
                            if (wingCcFindings.Finding.Value == (short)LodFinding.RecommendFormalInvestigation ||
                                wingCcFindings.Finding.Value == (short)LodFinding.NotInLineOfDutyDueToOwnMisconduct)
                            {
                                recommended = true;
                            }
                            else if (wingCcFindings.Finding.Value == (short)LodFinding.NotInLineOfDutyNotDueToOwnMisconduct)
                            {
                                if (NILODsubFinding == (int)NILODSubFindings.AbsentwithoutAuthority)
                                {
                                    recommended = true;
                                }
                            }
                        }

                        o.OptionVisible = (rule == recommended);
                        break;

                    case "icd9codenotpresent":
                        if (LODMedical_v2.IsSelectedICDCodeADisease(lookupDao))
                            o.OptionVisible = false;
                        else
                            o.OptionVisible = true;
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
                        if (!string.IsNullOrEmpty(LODUnit_v2.MemberCredible) && !string.IsNullOrEmpty(LODUnit_v2.MemberOnOrders))
                            o.OptionVisible = true;
                        else
                            o.OptionVisible = false;

                        break;

                    case "boardfinalization":

                        if (String.IsNullOrEmpty(LODMedical_v2.BoardFinalization) || LODMedical_v2.BoardFinalization.Equals("Yes"))
                        {
                            o.OptionVisible = false;
                            break;
                        }

                        o.OptionVisible = true;
                        break;

                    case "issarc":

                        if (SarcCase && r.RuleValue.ToString().Equals("True"))
                        {
                            o.OptionVisible = true;
                            break;
                        }

                        o.OptionVisible = false;
                        break;

                    case "directreply":

                        if (DirectReply && r.RuleValue.ToString().Equals("True"))
                        {
                            o.OptionVisible = true;
                            break;
                        }
                        else if (!DirectReply && r.RuleValue.ToString().Equals("False"))
                        {
                            o.OptionVisible = true;
                            break;
                        }

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
                        o.OptionValid = ValidateWingCcFindingsForCompleteRule(lookupDao);

                        //LineOfDutyFindings wcfnd = FindByType((short)PersonnelTypes.APPOINT_AUTH);

                        //if (wcfnd != null && wcfnd.Finding.HasValue && wcfnd.Finding == (short)LodFinding.InLineOfDuty)
                        //{
                        //    o.OptionValid = true;
                        //}
                        //else if (wcfnd != null && wcfnd.Finding.HasValue && wcfnd.Finding == (short)LodFinding.NotInLineOfDutyNotDueToOwnMisconduct)
                        //{
                        //    if (NILODsubFinding.HasValue)
                        //    {
                        //        if (NILODsubFinding.Value == (int)NILODSubFindings.EPTS_NSA && !LODMedical_v2.IsSelectedICDCodeADisease(lookupDao))
                        //        {
                        //            o.OptionValid = true;
                        //        }
                        //        else
                        //        {
                        //            o.OptionValid = false;
                        //        }
                        //    }
                        //    else
                        //    {
                        //        o.OptionValid = false;
                        //        AddValidationItem(new ValidationItem("Findings", "Appointing Authority Complete", "Appointing Authority NILOD finding needs reason.", true));
                        //    }
                        //}
                        //else
                        //{
                        //    o.OptionValid = false;
                        //    AddValidationItem(new ValidationItem("Findings", "Appointing Authority Complete", "Appointing Authority Findings not enough to allow closing.", true));
                        //}

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
                            if (wingJaFormalFindings.DecisionYN == "Y" && r.RuleValue.Equals("True"))
                            {
                                o.OptionValid = true;
                            }
                            else if (wingJaFormalFindings.DecisionYN == "N" && r.RuleValue.Equals("False"))
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
                            AddValidationItem(new ValidationItem("Findings", "Formal Wing JA", "Wing Judge Advocate Formal Action not present", true));
                        }

                        //if (!o.OptionValid)
                        //{
                        //    AddValidationItem(new ValidationItem("Findings", "Formal Wing JA", "Wing Judge Advocate Formal Action not present", true));
                        //}

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

                    case "canrwoa":

                        LineOfDutyFindings bdfindings = FindByType((short)PersonnelTypes.BOARD);

                        if (bdfindings != null && bdfindings.Finding.HasValue)
                        {
                            o.OptionValid = false;
                            AddValidationItem(new ValidationItem("Board Review", "Board Review", "To return to wing level no finding can be present"));
                        }
                        else
                        {
                            o.OptionValid = true;
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

                    case "canrwoaformal":

                        LineOfDutyFindings bdformalfindings = FindByType((short)PersonnelTypes.FORMAL_BOARD_RA);

                        if (bdformalfindings != null && bdformalfindings.Finding.HasValue)
                        {
                            o.OptionValid = false;
                            AddValidationItem(new ValidationItem("Formal Board Review", "Formal Board Review", "To return to wing level no finding can be present"));
                        }
                        else
                        {
                            o.OptionValid = true;
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
                                if (lookupDao.GetIsReinvestigationLod(this.Id) && memolist.Count > 0 && (MemoType)Convert.ToByte(memos[i]) == MemoType.ANGLodAppointIo)
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

        private void ProcessOption(int lastStatus, IDaoFactory daoFactor)
        {
            var options = from o in WorkflowStatus.WorkStatusOptionList
                          where o.Compo == MemberCompo || String.Equals(o.Compo, MemberCompo) || String.IsNullOrEmpty(o.Compo) || o.Compo == "0"

                          select o;

            //first, apply the visibility rules
            foreach (var opt in options)
            {
                bool optVisible = true;

                var rules = from r in opt.RuleList
                            where r.RuleTypes.ruleTypeId == (int)RuleKind.Visibility
                            select r;

                foreach (var visible in rules)
                {
                    ApplyRulesToOption(opt, visible, lastStatus, daoFactor);

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
                    ApplyRulesToOption(opt, validation, lastStatus, daoFactor);

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

        private void ValidateModule(string section, IValidatable item, int userid)
        {
            bool isValid = true;
            isValid = item.Validate(userid);
            AddValidationItem(item.ValidationItems);
            AddModuleStatus(section, isValid);
        }

        private bool ValidateWingCcFindingsForCompleteRule(ILookupDao lookupDao)
        {
            // Appointing Authority should be able to complete case even when disease is selected for ANG
            /*
            if (LODMedical_v2.IsSelectedICDCodeADisease(lookupDao))
            {
                AddValidationItem(new ValidationItem("Findings", "Appointing Authority Complete", "Appointing Authority cannot close out a disease case.", true));
                return false;
            }
            */

            bool canForward = false;
            LineOfDutyFindings wcfnd = FindByType((short)PersonnelTypes.APPOINT_AUTH);

            if (wcfnd != null && wcfnd.Finding.HasValue)
            {
                if (wcfnd.Finding == (short)LodFinding.InLineOfDuty)
                    canForward = true;

                if (wcfnd.Finding == (short)LodFinding.NotInLineOfDutyNotDueToOwnMisconduct && NILODsubFinding.HasValue && NILODsubFinding.Value == (int)NILODSubFindings.EPTS_NSA)
                    canForward = true;
            }

            if (!canForward)
            {
                AddValidationItem(new ValidationItem("Findings", "Appointing Authority Complete", "Appointing Authority Findings not enough to allow closing.", true));

                if (wcfnd != null && wcfnd.Finding.HasValue && wcfnd.Finding == (short)LodFinding.NotInLineOfDutyNotDueToOwnMisconduct && !NILODsubFinding.HasValue)
                    AddValidationItem(new ValidationItem("Findings", "Appointing Authority Complete", "Appointing Authority NILOD finding needs reason.", true));
            }

            return canForward;
        }

        #endregion workflow options and rules

        #region Constructor

        public ANGLineOfDuty_v2() : base()
        {
        }

        #endregion Constructor

        #region MyAccess

        /// <inheritdoc/>
        public override Dictionary<String, PageAccessType> ReadSectionList(int role)
        {
            PageAccessType access;
            access = PageAccessType.None;

            Dictionary<String, PageAccessType> scAccessList = new Dictionary<String, PageAccessType>();
            if (CurrentStatusCode == (int)LodStatusCode.ANGComplete)
            {
                access = PageAccessType.ReadOnly;
            }

            //'Add all pages as readonly

            scAccessList.Add(SectionNames.MED_TECH_REV.ToString(), access);
            scAccessList.Add(SectionNames.RLB.ToString(), access);
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
            scAccessList.Add(SectionNames.SENIOR_MED_REV.ToString(), access);
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
            scAccessList.Add(SectionNames.FORMAL_SENIOR_MED_REV.ToString(), access);
            scAccessList.Add(SectionNames.FORMAL_BOARD_LEGAL_REV.ToString(), access);
            scAccessList.Add(SectionNames.FORMAL_BOARD_APPROVING_AUTH_REV.ToString(), access);
            scAccessList.Add(SectionNames.FORMAL_BOARD_SENIOR_REV.ToString(), access);
            scAccessList.Add(SectionNames.FORMAL_BOARD_PERSONNEL_REV.ToString(), access);

            //Modify access with user role
            switch (role)
            {
                case (int)UserGroups.ANGMedicalTechnician:
                    if (CurrentStatusCode == (int)LodStatusCode.ANGMedTechReview)
                    {
                        scAccessList[SectionNames.MED_TECH_REV.ToString()] = PageAccessType.ReadWrite;
                        scAccessList[SectionNames.RLB.ToString()] = PageAccessType.ReadWrite;
                    }
                    break;

                default:
                    break;
            }
            return scAccessList;
        }

        #endregion MyAccess

        public virtual string GetMemberRankAndGradeForForm(ILookupDao lookupDao)
        {
            string rankAbbreviation = lookupDao.GetRankAbbreviationByType(MemberRank, "Form 348 Member");

            if (string.IsNullOrEmpty(rankAbbreviation))
                return MemberRank.FormattedGrade;

            return (rankAbbreviation);
        }

        /// <inheritdoc/>
        protected override bool HasPostCompletionDigitalSignatureBeenGenerated(IDaoFactory daoFactory)
        {
            ISignatueMetaDateDao sigMetaDataDao = daoFactory.GetSigMetaDataDao();

            if (sigMetaDataDao.GetByWorkStatus(Id, Workflow, (int)LodWorkStatus_v2.ANGComplete) == null)
                return false;

            return true;
        }
    }
}