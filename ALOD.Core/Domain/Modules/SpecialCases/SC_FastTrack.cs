using ALOD.Core.Domain.Common;
using ALOD.Core.Domain.Documents;
using ALOD.Core.Domain.Lookup;
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
    public class SC_FastTrack : SpecialCase
    {
        private int LabsRequired = 0;
        private int LabsUploaded = 0;

        public SC_FastTrack()
        {
            DAWGSignature = new SignatureEntry();
        }

        public virtual int? ALCLetterType { get; set; }
        public virtual int? AlternateALCLetterType { get; set; }
        public virtual DateTime? AlternateDQCompletionDate { get; set; }
        public virtual string AlternateDQParagraph { get; set; }
        public virtual DateTime? AlternateExpirationDate { get; set; }
        public virtual int? AlternateMemoTemplateID { get; set; }
        public virtual int? AlternateProcessAs { get; set; }
        public virtual DateTime? AlternateReturnToDutyDate { get; set; }
        public virtual string ApneaEpisodeDescription { get; set; }
        public virtual int? BIPAPRequired { get; set; }
        public virtual decimal? BodyMassIndex { get; set; }
        public virtual int? ControlledWithOralAgents { get; set; }
        public virtual int? CPAPRequired { get; set; }
        public virtual int? CurrentOptometryExam { get; set; }
        public virtual string DAFSC { get; set; }
        public virtual decimal? DailySteroidDosage { get; set; }
        public virtual int? DAWGRecommendation { get; set; }
        public virtual SignatureEntry DAWGSignature { get; set; }
        public virtual string DaySleepDescription { get; set; }

        // Obstructive Sleep Apnea specific questions
        public virtual int? DaytimeSomnolence { get; set; }

        /// <inheritdoc/>
        public override int DocumentViewId
        {
            get { return (int)DocumentViewType.FastTrack; }
        }

        public virtual DateTime? DQCompletionDate { get; set; }
        public virtual string DQParagraph { get; set; }
        public virtual int? DxInterferenceWithDuties { get; set; }
        public virtual string ERorUrgentCareVisitList { get; set; }
        public virtual string ExacerbatedSymptomsOralSteroidsDosage { get; set; }
        public virtual int? ExacerbatedSymptomsRequireOralSteroids { get; set; }
        public virtual string ExerciseColdSymptomDescription { get; set; }

        // Type 2 Diabetes specific questions
        public virtual decimal? FastingBloodSugar { get; set; }

        // sub-class specific properties  - none as this is a base class too
        public virtual int? FastTrackType { get; set; }

        // Medical Evaluation Board specific questions
        public virtual string FTDiagnosis { get; set; }

        public virtual string FTMedicationsAndDosages { get; set; }
        public virtual string FTPrognosis { get; set; }
        public virtual string FTTreatment { get; set; }
        public virtual int? HasApneaEpisodes { get; set; }
        public virtual int? HasBeenHospitalized { get; set; }
        public virtual int? HasHadERorUrgentCareVisits { get; set; }
        public virtual int? HasOtherSignificantConditions { get; set; }
        public virtual decimal? HgbA1C { get; set; }
        public virtual int? HOIntubation { get; set; }
        public virtual string HospitalizationList { get; set; }
        public virtual string ICD7thCharacter { get; set; }
        public virtual int? ICD9Code { get; set; }
        public virtual string ICD9Description { get; set; }
        public virtual string InsulinDosageRegime { get; set; }
        public virtual string MedGroupName { get; set; }
        public virtual int? MemoTemplateID { get; set; }
        public virtual int? MethacholineChallenge { get; set; }
        public virtual int? MissedWorkDays { get; set; }
        public virtual int? NormalPFTwithTreatment { get; set; }
        public virtual string OralAgentsList { get; set; }
        public virtual int? OralDevicesUsed { get; set; }
        public virtual string OtherSignificantConditionsList { get; set; }
        public virtual string PocEmail { get; set; }
        public virtual string PocPhoneDSN { get; set; }
        public virtual string PocRankAndName { get; set; }
        public virtual int? Process { get; set; }

        // Asthma specific questions
        public virtual int? PulmonaryFunctionTest { get; set; }

        public virtual int? RecommendedFollowUpInterval { get; set; }
        public virtual int? RequiresDailySteroids { get; set; }
        public virtual int? RequiresInsulin { get; set; }
        public virtual int? RequiresNonInsulinMed { get; set; }
        public virtual int? RequiresSpecialistForMgmt { get; set; }
        public virtual int? RescueInhalerUsageFrequency { get; set; }
        public virtual int? ResponseToDevices { get; set; }
        public virtual DateTime? ReturnToDutyDate { get; set; }
        public virtual int? RiskForSuddenIncapacitation { get; set; }
        public virtual int? RMUName { get; set; }
        public virtual int? SleepStudyResults { get; set; }
        public virtual int? SuitableDAFSC { get; set; }
        public virtual int? SymptomsExacerbatedByColdExercise { get; set; }
        public virtual DateTime? TMTReceiveDate { get; set; }  // Fast Track Date
        public virtual int? YearsSatisfactoryService { get; set; }

        /// <inheritdoc/>
        public override bool CreateDocumentGroup(IDocumentDao dao)
        {
            Check.Require(dao != null, "DocumentDao is required");

            DocumentGroupId = dao.CreateGroup();
            return DocumentGroupId > 0;
        }

        public virtual int? GetActiveMemoTemplateId()
        {
            if (ShouldUseSeniorMedicalReviewerFindings())
                return AlternateMemoTemplateID;

            return MemoTemplateID;
        }

        public virtual bool IsICDCodeOfType(ILookupDao lookupDao, string typeName)
        {
            IList<int> ids = lookupDao.GetIRILOTypeICDCodeIds(typeName);

            if (ids == null)
                return false;

            if (!ICD9Code.HasValue)
                return false;

            if (ids.Contains(ICD9Code.Value))
                return true;

            return false;
        }

        /// <inheritdoc/>
        public override Dictionary<String, PageAccessType> ReadSectionList(int role)
        {
            PageAccessType access;
            access = PageAccessType.None;

            Dictionary<String, PageAccessType> scAccessList = new Dictionary<String, PageAccessType>();
            if ((CurrentStatusCode == (int)SpecCaseFTStatusCode.RTD) || (CurrentStatusCode == (int)SpecCaseFTStatusCode.Disqualified))
            {
                access = PageAccessType.ReadOnly;
            }

            //'Add all pages as readonly

            scAccessList.Add(FTSectionNames.FT_HQT_INIT.ToString(), access); ;
            scAccessList.Add(FTSectionNames.FT_RTD.ToString(), access);
            scAccessList.Add(FTSectionNames.FT_DISQUALIFIED.ToString(), access);
            scAccessList.Add(FTSectionNames.FT_RLB.ToString(), access);
            scAccessList.Add(FTSectionNames.FT_SENIOR_MED_REV.ToString(), access);

            //Modify access with user role
            switch (role)
            {
                case (int)UserGroups.AFRCHQTechnician:
                    if (CurrentStatusCode == (int)SpecCaseFTStatusCode.InitiateCase)
                    {
                        scAccessList[FTSectionNames.FT_HQT_INIT.ToString()] = PageAccessType.ReadWrite;
                    }
                    else
                    {
                        scAccessList[FTSectionNames.FT_RTD.ToString()] = PageAccessType.ReadWrite;
                        scAccessList[FTSectionNames.FT_DISQUALIFIED.ToString()] = PageAccessType.ReadWrite;
                    }

                    scAccessList[FTSectionNames.FT_RLB.ToString()] = PageAccessType.ReadOnly;

                    break;

                case (int)UserGroups.MedicalTechnician:
                    if (CurrentStatusCode == (int)SpecCaseFTStatusCode.InitiateCase)
                    {
                        scAccessList[FTSectionNames.FT_RLB.ToString()] = PageAccessType.ReadWrite;
                    }
                    break;

                case (int)UserGroups.SeniorMedicalReviewer:
                    if (CurrentStatusCode == (int)SpecCaseFTStatusCode.SeniorMedicalReview)
                    {
                        scAccessList[FTSectionNames.FT_SENIOR_MED_REV.ToString()] = PageAccessType.ReadWrite;
                        scAccessList[FTSectionNames.FT_RLB.ToString()] = PageAccessType.ReadWrite;
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

                    case "wasinstatus":
                        bool isVisible = false;
                        statuses = r.RuleValue.ToString().Split(',');

                        IList<WorkStatusTracking> trackingData = lookupDao.GetStatusTracking(this.Id, (byte)ModuleType.SpecCaseFT);

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

                    case "cancompleteasapproved":
                        o.OptionVisible = (DetermineVisibilityForCanCompleteCaseRule((int)Finding.Qualify_RTD) || DetermineVisibilityForCanCompleteCaseRule((int)Finding.Admin_Qualified));
                        break;

                    case "cancompleteasdenied":
                        o.OptionVisible = (DetermineVisibilityForCanCompleteCaseRule((int)Finding.Disqualify) || DetermineVisibilityForCanCompleteCaseRule((int)Finding.Admin_Disqualified));
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
                        bool canforward = ValidateCanForwardCaseRule(lookupDao);

                        if (canforward != true)
                            o.OptionValid = false;
                        else
                            o.OptionValid = true;

                        break;

                    case "validatequalifymemos":
                        canforward = ValidateQualifyMemosRule(memoDao);

                        if (canforward != true)
                            o.OptionValid = false;
                        else
                            o.OptionValid = true;

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
                AddValidationItem(new ValidationItem("Board Medical Officer", "Decision", "Board Medical Officer must provide a finding in order to close out the IRILO case."));
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

        /// <inheritdoc/>
        protected override bool IsDocumentCategoryStillRequired(bool docFound, string docName, IDaoFactory daoFactory)
        {
            ILookupDao lookupDao = daoFactory.GetLookupDao();
            bool isRequired = true;

            if (!docFound)
            {
                // Set checklist values to false for docs that have NOT been uploaded
                // If checklist value is true - then document MUST be uploaded, so no changes for validation sake
                if (docName == DocumentType.OptometryExam.ToString() && CurrentOptometryExam == 1)
                    isRequired = true;
            }

            if (docName == DocumentType.SleepStudy.ToString())
            {
                if (!IsICDCodeOfType(lookupDao, "Sleep"))  // Sleep Apnea = 2
                {
                    isRequired = false;
                }
            }

            if (docName == DocumentType.PFTResults.ToString())
            {
                if (!IsICDCodeOfType(lookupDao, "Asthma")) // Asthma = 4
                {
                    isRequired = false;
                }
            }

            if (docName == DocumentType.OptometryExam.ToString())
            {
                if ((!IsICDCodeOfType(lookupDao, "Diabetes")) || (CurrentOptometryExam == 0))  // Diabetes = 3
                {
                    isRequired = false;
                }
            }

            if (docName == DocumentType.Labs.ToString())
            {
                if ((LabsRequired == 0))
                {
                    isRequired = false;
                }

                if ((LabsRequired == 1) && (LabsUploaded == 1))
                {
                    isRequired = true;
                }

                if ((LabsRequired == 1) && (LabsUploaded == 0))
                {
                    isRequired = true;
                }
            }

            return isRequired;
        }

        /// <inheritdoc/>
        protected override void PerformAdditionalDocumentProcessing(string docName, bool docFound, IDaoFactory daoFactory)
        {
            if (docFound)
            {
                // Set checklist values to true for docs that have been uploaded
                if (docName == DocumentType.OptometryExam.ToString())
                    CurrentOptometryExam = 1;

                if (docName == DocumentType.Labs.ToString())
                    LabsUploaded = 1;
            }

            if (docName == DocumentType.Labs.ToString())
            {
                if (((HgbA1C != null) && (HgbA1C > 0)) || ((FastingBloodSugar != null) && (FastingBloodSugar > 0)) || (ICD9Code == 251))
                {
                    LabsRequired = 1;
                }
            }
        }

        protected bool ValidateCanForwardCaseRule(ILookupDao lookupDao)
        {
            switch (CurrentStatusCode)
            {
                case (int)SpecCaseFTStatusCode.MedicalReview:
                    return ValidateCanForwardCaseRuleForBoardMedicalOfficer();

                case (int)SpecCaseFTStatusCode.SeniorMedicalReview:
                    return ValidateCanForwardCaseRuleForSeniorMedicalReviewer();

                default:
                    return ValidateCanForwardCaseRuleForMedicalTechnicianTabFields(lookupDao);
            }
        }

        private bool ValidateCanForwardCaseRuleForBoardMedicalOfficer()
        {
            bool canForward = true;

            if (!ICD9Code.HasValue || ICD9Code.Value == 0)
            {
                canForward = false;
                AddValidationItem(new ValidationItem("IRILO", "Med Off ICD 10 Code", "Must select an ICD 10 Code for the IRILO case."));
            }

            if (!med_off_approved.HasValue)
            {
                canForward = false;
                AddValidationItem(new ValidationItem("IRILO", "Med Off Qualification", "Must decide whether or not to Qualify the IRILO case."));
            }
            else
            {
                if ((med_off_approved.Value == (int)Finding.Disqualify || med_off_approved.Value == (int)Finding.Admin_Disqualified) && (!Process.HasValue || Process.Value == 0))
                {
                    canForward = false;
                    AddValidationItem(new ValidationItem("IRILO", "Med Off Process", "Please select a Process As option."));
                }

                if ((med_off_approved.Value == (int)Finding.Disqualify || med_off_approved.Value == (int)Finding.Admin_Disqualified) && !MemoTemplateID.HasValue)
                {
                    canForward = false;
                    AddValidationItem(new ValidationItem("Memos", "Med Off Select Memo", "Must select a Memo/Letter for disqualify"));
                }

                if ((med_off_approved.Value == (int)Finding.Qualify_RTD || med_off_approved.Value == (int)Finding.Admin_Qualified) && !MemoTemplateID.HasValue)
                {
                    canForward = false;
                    AddValidationItem(new ValidationItem("Memos", "Med Off Select Memo", "Must select a Memo/Letter corresponding to Return to Duty decision."));
                }

                if (med_off_approved.Value == (int)Finding.Admin_LOD && (!MemoTemplateID.HasValue || MemoTemplateID.Value != (int)MemoType.IRILO_Admin_LOD_AFPC))
                {
                    canForward = false;
                    AddValidationItem(new ValidationItem("Memos", "Admin LOD Memo", "Please select IRILO Admin LOD AFPC Letter.", true));
                }

                if (!ReturnToDutyDate.HasValue && (med_off_approved.Value == (int)Finding.Qualify_RTD || med_off_approved.Value == (int)Finding.Admin_Qualified))
                {
                    canForward = false;
                    AddValidationItem(new ValidationItem("IRILO", "Return To Duty Date", "Must choose a Return To Duty Date for Qualified IRILO Case."));
                }

                if (string.IsNullOrEmpty(DQParagraph) && (med_off_approved.Value == (int)Finding.Disqualify || med_off_approved.Value == (int)Finding.Admin_Disqualified))
                {
                    canForward = false;
                    AddValidationItem(new ValidationItem("IRILO", "Return To Duty Date", "Must enter a DQ paragraph if you choose Disqualify for the IRILO Case."));
                }
            }

            return canForward;
        }

        private bool ValidateCanForwardCaseRuleForMedicalTechnicianTabFields(ILookupDao lookupDao)
        {
            bool canForward = true;
            string messageProblem = string.Empty;
            string sectionProblem = string.Empty;

            // Required Patient/Case Fields
            if (TMTReceiveDate == null)
            {
                canForward = false;
                sectionProblem = "Fast Track Date";
                messageProblem = "Can Not Forward Case.  Must Enter a IRILO Date";
                AddValidationItem(new ValidationItem("IRILO", sectionProblem, messageProblem, true));
            }

            if (MedGroupName == null)
            {
                canForward = false;
                AddValidationItem(new ValidationItem("IRILO", "Med Group Name", "Must select a Med Group Name for the IRILO case."));
            }

            if (RMUName == null)
            {
                canForward = false;
                AddValidationItem(new ValidationItem("IRILO", "RMU Name", "Must select an RMU Name for the IRILO case."));
            }

            if (ICD9Code == null || ICD9Code == 0)
            {
                canForward = false;
                AddValidationItem(new ValidationItem("IRILO", "Med Tech ICD 10 Code", "Must select an ICD 10 Code for the WWD case."));
            }

            if (MissedWorkDays == null || MissedWorkDays == 0) // No Years Selection
            {
                canForward = false;
                sectionProblem = "Missed Work Days";
                messageProblem = "Can Not Forward Case.  Must Select Number of Days Missed from Work";
                AddValidationItem(new ValidationItem("IRILO", sectionProblem, messageProblem, true));
            }
            if ((HasHadERorUrgentCareVisits == null))
            {
                canForward = false;
                sectionProblem = "ER or Urgent Care Visits";
                messageProblem = "Can Not Forward Case.  Must Pick existence of ER or Urgent Care Visits";
                AddValidationItem(new ValidationItem("IRILO", sectionProblem, messageProblem, true));
            }
            if ((HasHadERorUrgentCareVisits == 1) && (ERorUrgentCareVisitList == null))
            {
                canForward = false;
                sectionProblem = "ER or Urgent Care Visit Details";
                messageProblem = "Can Not Forward Case.  Must provide Details about ER or Urgent Care Visits";
                AddValidationItem(new ValidationItem("IRILO", sectionProblem, messageProblem, true));
            }
            if ((RequiresSpecialistForMgmt == null) || (RequiresSpecialistForMgmt == 0))
            {
                canForward = false;
                sectionProblem = "Requires Specialist for Management";
                messageProblem = "Can Not Forward Case.  Must Select Frequency of Specialist Visits for Management";
                AddValidationItem(new ValidationItem("IRILO", sectionProblem, messageProblem, true));
            }
            if ((HasBeenHospitalized == null))
            {
                canForward = false;
                sectionProblem = "Hospitalizations";
                messageProblem = "Can Not Forward Case.  Must Pick existence of hospitalizations";
                AddValidationItem(new ValidationItem("IRILO", sectionProblem, messageProblem, true));
            }
            if ((HasBeenHospitalized == 1) && (HospitalizationList == null))
            {
                canForward = false;
                sectionProblem = "Hospitalization List";
                messageProblem = "Can Not Forward Case.  Must provide Details about Hospitalizations";
                AddValidationItem(new ValidationItem("IRILO", sectionProblem, messageProblem, true));
            }
            if ((RiskForSuddenIncapacitation == null) || (RiskForSuddenIncapacitation == 0))
            {
                canForward = false;
                sectionProblem = "Risk for Sudden Incapacitation";
                messageProblem = "Can Not Forward Case.  Must Select Chance of Sudden Incapacitation";
                AddValidationItem(new ValidationItem("IRILO", sectionProblem, messageProblem, true));
            }
            if ((RecommendedFollowUpInterval == null) || (RecommendedFollowUpInterval == 0))
            {
                canForward = false;
                sectionProblem = "Recommended Follow Up Interval";
                messageProblem = "Can Not Forward Case.  Must Select Follow Up Interval";
                AddValidationItem(new ValidationItem("IRILO", sectionProblem, messageProblem, true));
            }
            if ((DAWGRecommendation == null) || (DAWGRecommendation == 0))
            {
                canForward = false;
                sectionProblem = "DAWG Recommendation";
                messageProblem = "Can Not Forward Case.  Must Select a DAWG Recommendation";
                AddValidationItem(new ValidationItem("IRILO", sectionProblem, messageProblem, true));
            }

            // MEB Fields
            if ((ICD9Code != null) && (!IsICDCodeOfType(lookupDao, "Asthma")) &&
                (!IsICDCodeOfType(lookupDao, "Diabetes")) && (!IsICDCodeOfType(lookupDao, "Sleep"))
                && (ICD9Code != 0))
            {
                if (FTDiagnosis == null)
                {
                    canForward = false;
                    sectionProblem = "Diagnosis";
                    messageProblem = "Can Not Forward Case.  Must Enter a Diagnosis";
                    AddValidationItem(new ValidationItem("IRILO", sectionProblem, messageProblem, true));
                }

                if (FTPrognosis == null)
                {
                    canForward = false;
                    sectionProblem = "Prognosis";
                    messageProblem = "Can Not Forward Case.  Must Enter a Prognosis";
                    AddValidationItem(new ValidationItem("IRILO", sectionProblem, messageProblem, true));
                }
                if (FTTreatment == null)
                {
                    canForward = false;
                    sectionProblem = "Treatment";
                    messageProblem = "Can Not Forward Case.  Must Enter a Treatment";
                    AddValidationItem(new ValidationItem("IRILO", sectionProblem, messageProblem, true));
                }
                if (FTMedicationsAndDosages == null)
                {
                    canForward = false;
                    sectionProblem = "Medications and Dosages";
                    messageProblem = "Can Not Forward Case.  Must Enter Medications and Dosages";
                    AddValidationItem(new ValidationItem("IRILO", sectionProblem, messageProblem, true));
                }
            }

            // Sleep Apnea
            if (IsICDCodeOfType(lookupDao, "Sleep"))
            {
                if (BIPAPRequired == 1) // bipap required;
                {
                    canForward = false;
                    sectionProblem = "BIPAP Required";
                    messageProblem = "<u>BIPAP REQUIRED. CANCEL IRILO. INITIATE WWD.</u>";
                    AddValidationItem(new ValidationItem("IRILO", sectionProblem, messageProblem, true));
                }

                if (DaytimeSomnolence == null)
                {
                    canForward = false;
                    sectionProblem = "Daytime Somnolence";
                    messageProblem = "Can Not Forward Case.  Must Pick whether Daytime Somnolence occurs";
                    AddValidationItem(new ValidationItem("IRILO", sectionProblem, messageProblem, true));
                }
                if (DaytimeSomnolence == 1 && DaySleepDescription == null)
                {
                    canForward = false;
                    sectionProblem = "Daytime Somnolence (details)";
                    messageProblem = "Can Not Forward Case.  Must Provide Daytime Somnolence details";
                    AddValidationItem(new ValidationItem("IRILO", sectionProblem, messageProblem, true));
                }
                if (HasApneaEpisodes == null)
                {
                    canForward = false;
                    sectionProblem = "Sleep Apnea Episodes";
                    messageProblem = "Can Not Forward Case.  Must Pick whether Apnea Episodes occurs";
                    AddValidationItem(new ValidationItem("IRILO", sectionProblem, messageProblem, true));
                }
                if (HasApneaEpisodes == 1 && ApneaEpisodeDescription == null)
                {
                    canForward = false;
                    sectionProblem = "Apnea Episodes (details)";
                    messageProblem = "Can Not Forward Case.  Must Provide Apnea Episodes details";
                    AddValidationItem(new ValidationItem("IRILO", sectionProblem, messageProblem, true));
                }
                if (SleepStudyResults == null)
                {
                    canForward = false;
                    sectionProblem = "Sleep Study Results";
                    messageProblem = "Can Not Forward Case.  Must Pick whether Sleep Study Results are consistent with Sleep Apnea";
                    AddValidationItem(new ValidationItem("IRILO", sectionProblem, messageProblem, true));
                }
                if (OralDevicesUsed == null)
                {
                    canForward = false;
                    sectionProblem = "Oral Devices Used";
                    messageProblem = "Can Not Forward Case.  Must Pick whether Oral Devices are used";
                    AddValidationItem(new ValidationItem("IRILO", sectionProblem, messageProblem, true));
                }
                if (CPAPRequired == null)
                {
                    canForward = false;
                    sectionProblem = "CPAP Required";
                    messageProblem = "Can Not Forward Case.  Must Pick whether CPAP is required";
                    AddValidationItem(new ValidationItem("IRILO", sectionProblem, messageProblem, true));
                }
                if (BIPAPRequired == null)
                {
                    canForward = false;
                    sectionProblem = "BIPAP Required";
                    messageProblem = "Can Not Forward Case.  Must Pick whether BIPAP is required";
                    AddValidationItem(new ValidationItem("IRILO", sectionProblem, messageProblem, true));
                }

                if (ResponseToDevices == null)
                {
                    canForward = false;
                    sectionProblem = "Symptom Response to Oral Devices";
                    messageProblem = "Can Not Forward Case.  Must Pick whether Symptom Response to Oral Devices is controlled";
                    AddValidationItem(new ValidationItem("IRILO", sectionProblem, messageProblem, true));
                }
            }

            // Diabetes
            //if (ICD9Code == (int)SCFastTrackTypes.Diabetes)
            if (IsICDCodeOfType(lookupDao, "Diabetes"))
            {
                if ((FastingBloodSugar == null) || (FastingBloodSugar == 0))
                {
                    canForward = false;
                    sectionProblem = "Fasting Blood Sugar";
                    messageProblem = "Can Not Forward Case.  Must Select a Fasting Blood Sugar concentration";
                    AddValidationItem(new ValidationItem("IRILO", sectionProblem, messageProblem, true));
                }
                if ((HgbA1C == null) || (HgbA1C == 0))
                {
                    canForward = false;
                    sectionProblem = "HgbA1C";
                    messageProblem = "Can Not Forward Case.  Must Select a HgbA1C level";
                    AddValidationItem(new ValidationItem("IRILO", sectionProblem, messageProblem, true));
                }
                if ((CurrentOptometryExam == null))
                {
                    canForward = false;
                    sectionProblem = "Current Optometry Exam";
                    messageProblem = "Can Not Forward Case.  Must Pick Whether a Current Optometry Exam has been attached";
                    AddValidationItem(new ValidationItem("IRILO", sectionProblem, messageProblem, true));
                }
                if (HasOtherSignificantConditions == null)
                {
                    canForward = false;
                    sectionProblem = "Other Significant Medical Conditions";
                    messageProblem = "Can Not Forward Case.  Must Pick whether Other Significant Medical Conditions exist";
                    AddValidationItem(new ValidationItem("IRILO", sectionProblem, messageProblem, true));
                }
                if (HasOtherSignificantConditions == 1 && OtherSignificantConditionsList == null)
                {
                    canForward = false;
                    sectionProblem = "Other Significant Medical Conditions (details)";
                    messageProblem = "Can Not Forward Case.  Must Provide Other Significant Medical Condition details";
                    AddValidationItem(new ValidationItem("IRILO", sectionProblem, messageProblem, true));
                }
                if (ControlledWithOralAgents == null)
                {
                    canForward = false;
                    sectionProblem = "Controlled with Oral Agents";
                    messageProblem = "Can Not Forward Case.  Must Pick whether Oral Agents Control the Diabetes";
                    AddValidationItem(new ValidationItem("IRILO", sectionProblem, messageProblem, true));
                }
                if (ControlledWithOralAgents == 1 && OralAgentsList == null)
                {
                    canForward = false;
                    sectionProblem = "Controlled with Oral Agents (list)";
                    messageProblem = "Can Not Forward Case.  Must Provide Oral Agents details";
                    AddValidationItem(new ValidationItem("IRILO", sectionProblem, messageProblem, true));
                }
                if (RequiresInsulin == null)
                {
                    canForward = false;
                    sectionProblem = "Requires Insulin";
                    messageProblem = "Can Not Forward Case.  Must Pick whether Insulin is Required";
                    AddValidationItem(new ValidationItem("IRILO", sectionProblem, messageProblem, true));
                }
                if (RequiresInsulin == 1 && InsulinDosageRegime == null)
                {
                    canForward = false;
                    sectionProblem = "Requires Insulin (Dosage Regime)";
                    messageProblem = "Can Not Forward Case.  Must Provide Insulin Dosage Regime details";
                    AddValidationItem(new ValidationItem("IRILO", sectionProblem, messageProblem, true));
                }
                if (RequiresNonInsulinMed == null)
                {
                    canForward = false;
                    sectionProblem = "Requires Non-Insulin Med";
                    messageProblem = "Can Not Forward Case.  Must Pick whether Non-Insulin Medication is Required";
                    AddValidationItem(new ValidationItem("IRILO", sectionProblem, messageProblem, true));
                }
            }

            // Asthma
            //if (ICD9Code == (int)SCFastTrackTypes.Asthma)
            if (IsICDCodeOfType(lookupDao, "Asthma"))
            {
                if (PulmonaryFunctionTest == null)
                {
                    canForward = false;
                    sectionProblem = "Pulmonary Function Test";
                    messageProblem = "Can Not Forward Case.  Must Pick whether PFT is consistent with Asthma";
                    AddValidationItem(new ValidationItem("IRILO", sectionProblem, messageProblem, true));
                }
                if (MethacholineChallenge == null)
                {
                    canForward = false;
                    sectionProblem = "Methacholine Challenge";
                    messageProblem = "Can Not Forward Case.  Must Select a Methacholine Challenge result";
                    AddValidationItem(new ValidationItem("IRILO", sectionProblem, messageProblem, true));
                }
                if (RequiresDailySteroids == null)
                {
                    canForward = false;
                    sectionProblem = "Requires Daily Steroids";
                    messageProblem = "Can Not Forward Case.  Must Pick whether Daily Steroids are required";
                    AddValidationItem(new ValidationItem("IRILO", sectionProblem, messageProblem, true));
                }
                if (RequiresDailySteroids == 1 && DailySteroidDosage == null)
                {
                    canForward = false;
                    sectionProblem = "Requires Daily Steroids (Dosage)";
                    messageProblem = "Can Not Forward Case.  Must Provide Daily Steroids Dosage details";
                    AddValidationItem(new ValidationItem("IRILO", sectionProblem, messageProblem, true));
                }
                if ((RescueInhalerUsageFrequency == null) || (RescueInhalerUsageFrequency == 0))
                {
                    canForward = false;
                    sectionProblem = "Rescue Inhaler Usage Frequency";
                    messageProblem = "Can Not Forward Case.  Must Select a Rescue Inhaler Usage Frequency";
                    AddValidationItem(new ValidationItem("IRILO", sectionProblem, messageProblem, true));
                }
                if (SymptomsExacerbatedByColdExercise == null)
                {
                    canForward = false;
                    sectionProblem = "Symptoms Exacerbated by Cold or Exercise";
                    messageProblem = "Can Not Forward Case.  Must Pick whether Symptoms are Exacerbated by Cold or Exercise";
                    AddValidationItem(new ValidationItem("IRILO", sectionProblem, messageProblem, true));
                }
                if (SymptomsExacerbatedByColdExercise == 1 && ExerciseColdSymptomDescription == null)
                {
                    canForward = false;
                    sectionProblem = "Symptoms Exacerbated by Cold or Exercise (details)";
                    messageProblem = "Can Not Forward Case.  Must Provide Symptom Exacerbation details";
                    AddValidationItem(new ValidationItem("IRILO", sectionProblem, messageProblem, true));
                }
                if (ExacerbatedSymptomsRequireOralSteroids == null)
                {
                    canForward = false;
                    sectionProblem = "Exacerbated Symptoms Require Oral Steroids";
                    messageProblem = "Can Not Forward Case.  Must Pick whether Exacerbated Symptoms require Oral Steroids";
                    AddValidationItem(new ValidationItem("IRILO", sectionProblem, messageProblem, true));
                }
                if (ExacerbatedSymptomsRequireOralSteroids == 1 && ExacerbatedSymptomsOralSteroidsDosage == null)
                {
                    canForward = false;
                    sectionProblem = "Exacerbated Symptoms Require Oral Steroids (Dosage)";
                    messageProblem = "Can Not Forward Case.  Must Provide Exacerbated Symptoms - Oral Steroids Dosage details";
                    AddValidationItem(new ValidationItem("IRILO", sectionProblem, messageProblem, true));
                }
                if (NormalPFTwithTreatment == null)
                {
                    canForward = false;
                    sectionProblem = "Normal PFTs with Treatment";
                    messageProblem = "Can Not Forward Case.  Must Pick Treatment results in Normal PFTs";
                    AddValidationItem(new ValidationItem("IRILO", sectionProblem, messageProblem, true));
                }
                if (HOIntubation == null)
                {
                    canForward = false;
                    sectionProblem = "HO Intubation";
                    messageProblem = "Can Not Forward Case.  Must Pick whether HO Intubation is necessary";
                    AddValidationItem(new ValidationItem("IRILO", sectionProblem, messageProblem, true));
                }
            }

            return canForward;
        }

        private bool ValidateCanForwardCaseRuleForSeniorMedicalReviewer()
        {
            bool canForward = true;

            if (string.IsNullOrEmpty(SeniorMedicalReviewerConcur))
            {
                canForward = false;
                AddValidationItem(new ValidationItem("Senior Medical Reviewer", "Decision", "Must provide a Concur/Non-Concur Decision for the IRILO case."));
            }
            else
            {
                if (SeniorMedicalReviewerConcur.Equals(DECISION_NONCONCUR))
                {
                    if (!ICD9Code.HasValue || ICD9Code.Value == 0)
                    {
                        canForward = false;
                        AddValidationItem(new ValidationItem("Senior Medical Reviewer", "Med Off ICD 10 Code", "Must select an ICD 10 Code for the IRILO case."));
                    }

                    if (!SeniorMedicalReviewerApproved.HasValue)
                    {
                        canForward = false;
                        AddValidationItem(new ValidationItem("Senior Medical Reviewer", "Determination", "Must provide a Decision for the IRILO case."));
                    }
                    else
                    {
                        if ((!AlternateProcessAs.HasValue || AlternateProcessAs.Value == 0) && (SeniorMedicalReviewerApproved.Value == (int)Finding.Disqualify || SeniorMedicalReviewerApproved.Value == (int)Finding.Admin_Disqualified))
                        {
                            canForward = false;
                            AddValidationItem(new ValidationItem("IRILO", "Med Off Process", "Please select a Process As option."));
                        }

                        if (!AlternateReturnToDutyDate.HasValue && (SeniorMedicalReviewerApproved.Value == (int)Finding.Qualify_RTD || SeniorMedicalReviewerApproved.Value == (int)Finding.Admin_Qualified))
                        {
                            canForward = false;
                            AddValidationItem(new ValidationItem("Senior Medical Reviewer", "Return To Duty Date", "Must choose a Return To Duty Date for Qualified IRILO Case."));
                        }

                        if (string.IsNullOrEmpty(AlternateDQParagraph) && (SeniorMedicalReviewerApproved.Value == (int)Finding.Disqualify || SeniorMedicalReviewerApproved.Value == (int)Finding.Admin_Disqualified))
                        {
                            canForward = false;
                            AddValidationItem(new ValidationItem("Senior Medical Reviewer", "DQ Paragraph", "Must enter a DQ paragraph if you choose Disqualify for the IRILO Case."));
                        }

                        if ((SeniorMedicalReviewerApproved.Value == (int)Finding.Disqualify || SeniorMedicalReviewerApproved.Value == (int)Finding.Admin_Disqualified) && !AlternateMemoTemplateID.HasValue)
                        {
                            canForward = false;
                            AddValidationItem(new ValidationItem("Memos", "Med Off Select Memo", "Must select a Memo/Letter for disqualify"));
                        }

                        if ((SeniorMedicalReviewerApproved.Value == (int)Finding.Qualify_RTD || SeniorMedicalReviewerApproved.Value == (int)Finding.Admin_Qualified) && !AlternateMemoTemplateID.HasValue)
                        {
                            canForward = false;
                            AddValidationItem(new ValidationItem("Memos", "Med Off Select Memo", "Must select a Memo/Letter corresponding to Return to Duty decision."));
                        }

                        if (SeniorMedicalReviewerApproved.Value == (int)Finding.Admin_LOD && (!AlternateMemoTemplateID.HasValue || AlternateMemoTemplateID.Value != (int)MemoType.IRILO_Admin_LOD_AFPC))
                        {
                            canForward = false;
                            AddValidationItem(new ValidationItem("Memos", "Admin LOD Memo", "Please select IRILO Admin LOD AFPC Letter.", true));
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(SeniorMedicalReviewerComment))
            {
                AddValidationItem(new ValidationItem("Senior Medical Reviewer", "Decision Explanation", "Must provide a Decision Explanation for the IRILO case."));
                canForward = false;
            }

            return canForward;
        }

        private bool ValidateQualifyMemosRule(IMemoDao2 memoDao)
        {
            bool canForward = true;
            int? activeMemoTemplateId = GetActiveMemoTemplateId();
            int? activeBoardMedicalFinding = GetActiveBoardMedicalFinding();

            if (Status == (int)SpecCaseFTWorkStatus.FinalReview || Status == (int)SpecCaseFTWorkStatus.AdminLOD || Status == (int)SpecCaseFTWorkStatus.IMRPHolding)
            {
                IList<Memorandum2> memolist = (from m in memoDao.GetByRefnModule(Id, (int)ModuleType.SpecCaseFT)
                                               where m.Deleted == false && m.Template.Id == Convert.ToByte(activeMemoTemplateId)
                                               select m).ToList<Memorandum2>();

                if (memolist.Count < 1 && activeMemoTemplateId.HasValue)
                {
                    canForward = false;
                    string memoName = (from m in memoDao.GetAllTemplates() where m.Id == Convert.ToByte(activeMemoTemplateId) select m.Title).Single();
                    AddValidationItem(new ValidationItem("Memos", "Memo", memoName + "  Memo  not found.", true));
                }
                else
                {
                    if ((activeBoardMedicalFinding == (int)Finding.Qualify_RTD || activeBoardMedicalFinding == (int)Finding.Admin_Qualified) && !activeMemoTemplateId.HasValue)
                    {
                        canForward = false;
                        AddValidationItem(new ValidationItem("Memos", "Med Off Select Memo", "Must select a Memo/Letter corresponding to Return to Duty decision."));
                    }
                    else if (activeBoardMedicalFinding == (int)Finding.Admin_LOD && activeMemoTemplateId != (int)MemoType.IRILO_Admin_LOD_AFPC)
                    {
                        canForward = false;
                        AddValidationItem(new ValidationItem("Memos", "Admin LOD Memo", "Please select IRILO Admin LOD AFPC Letter.", true));
                    }
                }

                // check for MEB Disqul Letter from Board Medical step
                if (activeBoardMedicalFinding == (int)Finding.Admin_LOD)
                {
                    IList<Memorandum2> memolist2 = (from m in memoDao.GetByRefnModule(Id, (int)ModuleType.SpecCaseFT)
                                                    where m.Deleted == false && m.Template.Id == Convert.ToByte(MemoType.IRILO_MEB_Disqualification_Letter)
                                                    select m).ToList<Memorandum2>();

                    if (memolist2.Count < 1)
                    {
                        canForward = false;
                        string memoName2 = (from m in memoDao.GetAllTemplates() where m.Id == Convert.ToByte(MemoType.IRILO_MEB_Disqualification_Letter) select m.Title).Single();
                        AddValidationItem(new ValidationItem("Memos", "IRILO Memo", memoName2 + "  Memo  not found.", true));
                    }
                    else
                    {
                        canForward = true;
                    }
                }
            }

            if (Status == (int)SpecCaseFTWorkStatus.MedicalReview)
            {
                if (activeBoardMedicalFinding == (int)Finding.Admin_LOD && activeMemoTemplateId != (int)MemoType.IRILO_Admin_LOD_AFPC)
                {
                    canForward = false;
                    AddValidationItem(new ValidationItem("Memos", "Admin LOD Memo", "Please select IRILO Admin LOD AFPC Letter.", true));
                }
            }

            return canForward;
        }
    }
}