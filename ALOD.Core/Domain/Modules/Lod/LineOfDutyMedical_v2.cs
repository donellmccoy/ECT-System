using ALOD.Core.Domain.Common;
using ALOD.Core.Domain.Users;
using System;

namespace ALOD.Core.Domain.Modules.Lod
{
    [Serializable]
    public class LineOfDutyMedical_v2 : LineOfDutyMedical
    {
        public LineOfDutyMedical_v2() : base()
        {
        }

        public LineOfDutyMedical_v2(int id) : base(id)
        {
        }

        public virtual string AlcoholTestDone { get; set; }
        public virtual string BoardFinalization { get; set; }
        public virtual Boolean? ConditionEPTS { get; set; }
        public virtual string DeployedLocation { get; set; }
        public virtual string DrugTestDone { get; set; }
        public virtual int? Influence { get; set; }
        public virtual int? MemberCategory { get; set; }
        public virtual int? MemberComponent { get; set; }
        public virtual string MemberCondition { get; set; }
        public virtual int? MemberFrom { get; set; }
        public virtual string MemberResponsible { get; set; }
        public virtual string MobilityStandards { get; set; }
        public virtual string OtherTest { get; set; }
        public virtual DateTime? OtherTestDate { get; set; }
        public virtual DateTime? PsychiatricDate { get; set; }
        public virtual string PsychiatricEval { get; set; }
        public virtual string RelevantCondition { get; set; }
        public virtual Boolean? ServiceAggravated { get; set; }

        #region IValidatable Members

        /// <inheritdoc/>
        public override bool Validate(int userid)
        {
            string section = "Medical";
            bool isValid = true;
            validations.Clear();

            if (MemberCategory == 0 && userid != (int)UserGroups.ANGMedicalTechnician)
            {
                if (MemberCategory == 0 && userid != (int)UserGroups.WingSarc)
                {
                    AddValidationItem(new ValidationItem(section, "CategorySelect_v2", "Member category selection is required"));
                    isValid = false;
                }
            }
            if ((String.IsNullOrEmpty(MemberStatus)))
            {
                AddValidationItem(new ValidationItem(section, "MemberStatusSelect_v2", "Member status selection is required"));
                isValid = false;
            }
            if (MemberFrom == 0)
            {
                AddValidationItem(new ValidationItem(section, "FromSelect_v2", "Member from selection is required"));
                isValid = false;
            }
            if (!(ICD9Id.HasValue) && userid != (int)UserGroups.ANGMedicalTechnician)
            {
                if (!(ICD9Id.HasValue) && userid != (int)UserGroups.WingSarc)
                {
                    AddValidationItem(new ValidationItem(section, "ddlICDChapter,ddlICDSection,ddlICDDiagnosisLevel1,ddlICDDiagnosisLevel2,ddlICDDiagnosisLevel3,ddlICDDiagnosisLevel4", "ICD code  is required"));
                    isValid = false;
                }
            }
            if (String.IsNullOrEmpty(NatureOfEvent) && userid != (int)UserGroups.ANGMedicalTechnician)
            {
                if (String.IsNullOrEmpty(NatureOfEvent) && userid != (int)UserGroups.WingSarc)
                {
                    AddValidationItem(new ValidationItem(section, "IncidentTypeSelect_v2", "Nature of Incident is required"));
                    isValid = false;
                }
            }
            if ((String.IsNullOrEmpty(DiagnosisText)) && userid != (int)UserGroups.ANGMedicalTechnician)
            {
                if (String.IsNullOrEmpty(DiagnosisText) && userid != (int)UserGroups.WingSarc)
                {
                    AddValidationItem(new ValidationItem(section, "DiagnosisTextBox_v2", "Diagnosis Text is required"));
                    isValid = false;
                }
            }
            if (String.IsNullOrEmpty(MedicalFacilityType))
            {
                AddValidationItem(new ValidationItem(section, "HospitalTypeSelect_v2", "Type of Medical Facility is required"));
                isValid = false;
            }
            if (String.IsNullOrEmpty(MedicalFacility))
            {
                AddValidationItem(new ValidationItem(section, "HospitalNameTextBox_v2", "Name of Medical Facility is required"));
                isValid = false;
            }
            if ((TreatmentDate != null && TreatmentDate.HasValue))
            {
                if (TreatmentDate.Value > DateTime.Now)
                {
                    AddValidationItem(new ValidationItem(section, "TreatmentDateBox_v2, TreatmentHourBox_v2", "Treatment date can not be future date"));
                    isValid = false;
                }
            }
            else
            {
                AddValidationItem(new ValidationItem(section, "TreatmentDateBox_v2, TreatmentHourBox_v2", "Treatment date required"));
                isValid = false;
            }
            if (String.IsNullOrEmpty(EventDetails))
            {
                AddValidationItem(new ValidationItem(section, "EventDetailsBox_v2", "Details of Accident is required"));
                isValid = false;
            }

            if ((userid == (int)UserGroups.MedicalOfficer) || (userid == (int)UserGroups.MedOfficer_Pilot))
            {
                if (String.IsNullOrEmpty(DeathInvolved))
                {
                    AddValidationItem(new ValidationItem(section, "DeathCaseChoice_v2", "Death Involved selection is required"));
                    isValid = false;
                }

                //****Changes made  for task 1 changes 8/18/2023 Diamante Lawson***

                if (MvaInvolved.ToString().Equals("Yes") &&

                String.IsNullOrEmpty(LODInitiation.ToString()))
                {
                    // AddValidationItem(new ValidationItem(section, "rblLODInitiation", "Did member sign the Member LOD"));
                    // isValid = false;
                }

                if (String.IsNullOrEmpty(WrittenDiagnosis.ToString()))
                {
                    // AddValidationItem(new ValidationItem(section, "rblWrittenDiagnosis", "Is there a written diagnosis from a medical provider"));
                    // isValid = false;
                }

                if (String.IsNullOrEmpty(MemberRequest.ToString()))
                {
                    // AddValidationItem(new ValidationItem(section, "rblMemberRequest", "Did the member request the LOD within 180 days"));
                    // isValid = false;
                }

                if (String.IsNullOrEmpty(PriorToDutytatus.ToString()))
                {
                    AddValidationItem(new ValidationItem(section, "rblPriorToDutyStatus", "Is there medical evidence of the illness or disease"));
                    isValid = false;
                }

                //if (String.IsNullOrEmpty(StatusWorsened.ToString()))
                //{
                //    AddValidationItem(new ValidationItem(section, "rblStatusWorsened", "Is there any medical evidence during qualified duty status"));
                //    isValid = false;
                //}

                if (String.IsNullOrEmpty(MvaInvolved))
                {
                    AddValidationItem(new ValidationItem(section, "MvaInvolvedChoice_v2", "Motor Vehicle Accident selection is required"));
                    isValid = false;
                }

                if (String.IsNullOrEmpty(MemberCondition))
                {
                    AddValidationItem(new ValidationItem(section, "MemberConditionSelect_v2", "Member Condition selection is required"));
                    isValid = false;
                }
                else if (Influence == 0 & MemberCondition.Equals("was"))
                {
                    AddValidationItem(new ValidationItem(section, "InfluenceSelect_v2", "Influence selection is required"));
                    isValid = false;
                }

                if (String.IsNullOrEmpty(AlcoholTestDone) & (Influence == 1 || Influence == 3))
                {
                    AddValidationItem(new ValidationItem(section, "AlcoholTest_v2", "Alcohol Test Done selection is required"));
                    isValid = false;
                }

                if (String.IsNullOrEmpty(DrugTestDone) & (Influence == 2 || Influence == 3))
                {
                    AddValidationItem(new ValidationItem(section, "DrugTest_v2", "Drug Test Done selection is required"));
                    isValid = false;
                }

                if (String.IsNullOrEmpty(MemberResponsible))
                {
                    AddValidationItem(new ValidationItem(section, "MentallyResponsiblerbl_v2", "Member mentally responsible selection is required"));
                    isValid = false;
                }

                if (String.IsNullOrEmpty(PsychiatricEval))
                {
                    AddValidationItem(new ValidationItem(section, "PsychiatricEval_v2", "Psychiatric Evaluation Completed selection is required"));
                    isValid = false;
                }

                if (!String.IsNullOrEmpty(PsychiatricEval))
                {
                    if (PsychiatricEval.Equals("Yes"))
                    {
                        if ((PsychiatricDate != null && PsychiatricDate.HasValue))
                        {
                            if (PsychiatricDate.Value > DateTime.Now)
                            {
                                AddValidationItem(new ValidationItem(section, "PsychiatricDatePicker_v2", "Psychiatric date can not be future date"));
                                isValid = false;
                            }
                        }
                        else
                        {
                            AddValidationItem(new ValidationItem(section, "PsychiatricDatePicker_v2", "Psychiatric date required"));
                            isValid = false;
                        }
                    }
                }

                if (String.IsNullOrEmpty(OtherTest))
                {
                    AddValidationItem(new ValidationItem(section, "OtherTest_v2", "Other Tests done selection is required"));
                    isValid = false;
                }
                if (!String.IsNullOrEmpty(OtherTest))
                {
                    if (OtherTest.Equals("Yes"))
                    {
                        if ((OtherTestDate != null && OtherTestDate.HasValue))
                        {
                            if (OtherTestDate.Value > DateTime.Now)
                            {
                                AddValidationItem(new ValidationItem(section, "OtherTestDatePicker_v2", "Other Test date can not be future date"));
                                isValid = false;
                            }
                        }
                        else
                        {
                            AddValidationItem(new ValidationItem(section, "OtherTestDatePicker_v2", "Other test date required"));
                            isValid = false;
                        }
                    }
                }

                if (String.IsNullOrEmpty(DeployedLocation))
                {
                    AddValidationItem(new ValidationItem(section, "rblDeoployedLocation", "Deployed Location selection is required"));
                    isValid = false;
                }

                if (!String.IsNullOrEmpty(DeployedLocation))
                {
                    if (DeployedLocation.Equals("No"))
                    {
                        if (Epts == null)
                        {
                            AddValidationItem(new ValidationItem(section, "rblEPTS_v2", "EPTS selection is required"));
                            isValid = false;
                        }

                        if (String.IsNullOrEmpty(MobilityStandards))
                        {
                            AddValidationItem(new ValidationItem(section, "MobilityStandards_v2", "Mobility Standard selection is required"));
                            isValid = false;
                        }
                    }
                }
            }
            return isValid;
        }

        #endregion IValidatable Members
    }
}