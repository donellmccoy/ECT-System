namespace ALOD.Core.Domain.Documents
{
    public enum DocumentClass
    {
        Document = 1,
        Memo = 2
    };

    public enum DocumentStatus
    {
        Pending = 1,
        Approved = 2,
        Deleted = 3,
        Test = 4,
        Batch = 5
    };

    //These match with DocCatId in the DcoumentCategory2 Table
    public enum DocumentType
    {
        LODSupportingDocument = 1,
        BatchImport = 2,
        MilitaryMedicalDocumentation = 3,
        CivilianMedicalDocumentation = 4,
        Labs = 5,
        RadiologyAndImaging = 6,
        Studies = 7,
        SpecialtyConsults = 8,
        ProofOfMilitaryStatus = 9,
        MembersStatement = 10,
        Maps = 11,
        AccidentReport = 12,
        AutopsyReportDeathCertificate = 13,
        Miscellaneous = 14,
        MemberReinvestigationRequest = 29,
        ReinvestigationSupportingDocs = 30,
        AFForm469 = 31,
        Form1971 = 32,
        Orders = 33,
        IncapSupportingDocs = 34,
        MemberHistory = 35,
        PhysicalEvaluationForm = 36,
        AHLTANotes = 37,
        Form2807 = 38,
        Form2808 = 39,
        ApprovedForm2808 = 40,
        BMTSupportingMedicalDocs = 41,
        Disposition = 42,
        BMTSupportingDocs = 43,
        CongressionalComplaint = 44,
        SupportingDocs = 45,
        CongressInquiryPacket = 46,
        SignedNarrativeDocument = 47,
        WWDCoverLetter = 48,
        NarrativeSummary = 49,
        MemberUtilizationQuestionnaire = 50,
        MedicalEvaluationFactSheet = 51,
        PS3811 = 52,
        CMASInformation = 53,
        SleepStudy = 54,
        OptometryExam = 55,
        RTDLetter = 56,

        //ALCLetter1 = 57,
        AssignmentLimitationLetter = 57,

        ALCLetter2 = 58,
        ALCLetter3 = 59,
        PFTResults = 60,
        BCMRRequest = 61,
        IPEBElection = 62,
        UnitCmdrMemo = 63,
        LetterToMemberRequestingDocumentation = 64,
        PrivatePhysicianDocs = 65,
        RILOChecklist = 66,
        CongressionalReponse = 67,
        BCMRResponse = 68,
        WWD_Disposition = 69,
        AdminLOD = 70,
        Memorandum = 71,
        PALDocument = 72,
        AFPCDisposition = 73,
        FinalForm348 = 74,
        FinalForm261 = 75,
        AF422AF469 = 76,
        MedicalSpecialInstructions = 77,
        Checklist = 78,
        MAJCOMDisposition = 79,
        DWSupportingMedicalDocs = 80,
        NonEmergentSurgeryRequestMemorandum = 81,
        CurrentOrders = 82,
        CurrentRTDMemorandum = 83,
        MEPS = 84,
        GeneralDocumentation = 85,
        RequirementsTraceabilityMatrix = 86,
        ReleaseNotes = 87,
        Waiver = 88,
        Attachments = 89,
        RSDocuments = 90,
        OriginalDocuments = 91,
        PHForm = 92,
        UntimelySubmissionOfIncidentReport = 93,
        AppealDecision = 94,
        SupportingAppealDocuments = 95,
        SignedNotificationMemo = 96,
        Form348R = 97,
        RedactedSupportingDocuments = 98,
        UnredactedSupportingDocumnets = 99,
        MemberAppealRequest = 100,
        SupportingSARCAppealDocuments = 101,
        ALcCoverLetter = 102,
        AmendedDocuments = 104,

        //added for task 136
        PertinentMedicalRecords = 105,

        StatusVerificationDocuments = 106,

        //INCAP Doc
        InterimOrFinalLODAndDDForm_261 = 107,

        INCAPPayPM_1971 = 108,
        Medical_1971 = 109,
        UnitCC_1971 = 110,
        Legal_1971 = 111,
        Finance_1971 = 112,
        WingCC_1971 = 113,
        MemberSignedPersonnelBriefing = 114,
        MemberSignedFinancialBriefing = 115,
        AFForm_1768_StaffSummarySheet = 116,
        CommandersLetterOfDelayedSubmission = 117,
        CurrentPayStatement_LossOfIncome = 118,
        EmployerEmployeeReleaseStatement_LossOfIncomeAndEmployed = 119,
        StatementFromCivilianEmployment_LossOfIncomeAndEmployed = 120,
        SelfEmploymentUnemployedStatement_LossOfIncomeAndSelfEmployed = 121,
        SelfEmployedIncomeProtectionStatement_LossOfIncomeAndSelfEmployed = 122,
        MedicalEvaluation_Medical = 123,
        MedicalTreatmentPlan_Medical = 124,
        Signed287_Medical = 125,
        MedicalEntitlements_Medical = 126,
        INCAPPayCalculationWorksheet = 127,
        PCARS = 131
    };

    //These match with documentId in the Dcoument Category Table
    public enum DocumentTypeOld
    {
        LODSupportingDocument = 1,
        BatchImport = 2,
        MilitaryMedicalDocumentation = 3,
        CivilianMedicalDocumentation = 4,
        Labs = 5,
        RadiologyAndImaging = 6,
        Studies = 7,
        SpecialtyConsults = 8,
        ProofOfMilitaryStatus = 9,
        MembersStatement = 10,
        Maps = 11,
        AccidentReport = 12,
        AutopsyReportDeathCertificate = 13,
        Miscellaneous = 14,
        MemberReinvestigationRequest = 29,
        ReinvestigationSupportingDocs = 30,
        PertinentMedicalRecords = 105,
        StatusVerificationDocuments = 106
    };

    // Matches DocumentView table
    public enum DocumentViewType
    {
        LineOfDuty = 1,
        HIPAA = 2,
        ReinvestigationRequest = 3,
        Incap = 4,
        BMT = 5,
        Congress = 6,
        WWD = 7,
        PWaivers = 8,
        MEB = 9,
        BCMR = 10,
        FastTrack = 11,
        CMAS = 12,
        MEPS = 13,
        MMSO = 14,
        MH = 16,
        NE = 17,
        DW = 18,
        MO = 19,
        PEPP = 20,
        HelpDocuments = 21,
        RS = 22,
        PH = 23,
        AppealRequest = 24,
        LineOfDuty_v2 = 25,
        SARC = 26,
        Appeal_SARC = 27,
        RW = 28,
        AGRCert = 29,
        PSCD = 30
    };

    //These match with Ids in the core_MemoTemplates  Table
    public enum MemoType
    {
        LodFindingsILOD = 2,
        LodFindingsNLOD = 3,
        LodFindingsNLODDeath = 4,
        LodAppointIo = 5,
        ReinvestigationRequestApproved = 12,
        ReinvestigationRequestDenied = 14,
        PWaiverApproved = 15,
        PWaiverDenied = 16,
        WWDDisqualified = 18,      // REMOVED
        WWDQualified = 19,         // REMOVED
        ALCLetterC1 = 20,          // REMOVED
        ALCLetterC2 = 21,          // REMOVED
        ALCLetterC3 = 23,          // REMOVED
        ReturnToDutyLetter = 24,
        MEB_4_Non_ALC_C_SAF = 25,
        MEB_4_Non_ALC_C_SG = 26,
        MEB_DQ_AGR = 27,
        MEB_DQ = 28,
        MEB_RTD_AGR_SAF = 29,
        MEB_RTD_AGR_SG = 30,
        MEB_RTD_HIV_AGR_SAF = 31,
        MEB_RTD_HIV_AGR_SG = 32,
        MEB_RTD_HIV_SAF = 33,
        MEB_RTD_HIV_SG = 34,
        MEB_RTD_SAF = 35,
        MEB_RTD_SG = 36,
        WWD_4_Non_ALC_C = 37,
        WWD_4_2_RTD = 38,

        //  WWD_4_2_Non_ALC_C_OSA = 39,         // REMOVED
        WWD_Admin_LOD_AFPC = 40,

        MEB_Disqualification_Letter = 41,  // MEB_Disqualification_Letter for WWD
        WWD_Disqualification_Letter = 42,  // REMOVED
        WWD_HIV_Renewal_C2 = 43,

        //    WWD_RTD_ALC_C_OSA = 44,            // REMOVED
        WWD_RTD_SAF = 45,

        WWD_RTD_SG = 46,

        //   WWD_4_Non_ALC_C_OSA = 47,         // REMOVED
        IRILO_4_Non_ALC_C = 48,

        IRILO_4_2_RTD = 49,

        //   IRILO_4_2_Non_ALC_C_OSA = 50,     // REMOVED
        IRILO_Admin_LOD_AFPC = 51,

        IRILO_MEB_Disqualification_Letter = 52, // MEB_Disqualification_Letter for IRILO
        IRILO_Disqualification_Letter = 53, // REMOVED
        IRILO_HIV_Renewal_C2 = 54,

        //    IRILO_RTD_ALC_C_OSA = 55,        // REMOVED
        IRILO_RTD_SAF = 56,

        IRILO_RTD_SG = 57,

        //    IRILO_4_Non_ALC_C_OSA = 58,     // REMOVED
        Return_Without_Action = 59,

        WWD_Unit_Disqualification_Letter = 60,
        MEB_4_Non_ALC_C_Exp_SAF = 61,
        MEB_4_Non_ALC_C_Exp_SG = 62,
        LodFindingsILODDeath = 63,
        PalaceChaseMSD = 64,
        PalaceChaseAFI = 65,
        PalaceChaseQualified = 66,
        RSALCMemo = 67,
        RSNonALCMemo = 68,
        DisapprovalAppeal = 69,
        ApprovalAppeal = 70,
        PalaceChaseMSDandAFI = 71,
        SARC_Determination_ILOD = 72,
        SARC_Determination_NILOD = 73,
        IRILO_DQ_MEB = 74,
        IRILO_DQ_WWD = 75,
        SARC_APPEAL_DISAPPROVAL = 76,
        SARC_APPEAL_APPROVED = 77,
        IRILO_DQ_PENDING = 78,
        RW_4_Non_RW_C = 79,
        RW_4_2_RTD = 80,
        RW_Admin_LOD_AFPC = 81,
        RW_HIV = 82,
        RW_RTD_SAF = 83,
        RW_RTD_SG = 84,
        RW_DQ_MEB = 85,
        RW_DQ_WD = 86,
        RW_DQ_Pending_LOD = 87,
        AGR_Approved_WING_FON = 196,
        AGR_Approved_HQ_FON = 197,
        AGR_Approved_WING_INIT = 198,
        AGR_Approved_HQ_INIT = 199,
        AGR_Certiication_Denied = 200,

        /* --- ANG ------ */
        ANGLodFindingsILOD = 102,
        ANGLodFindingsNLOD = 103,
        ANGLodFindingsNLODDeath = 104,
        ANGLodAppointIo = 105,
        ANGReinvestigationRequestApproved = 112,
        ANGReinvestigationRequestDenied = 114,
        ANGPWaiverApproved = 115,
        ANGPWaiverDenied = 116,
        ANGWWDDisqualified = 118,      // REMOVED
        ANGWWDQualified = 119,         // REMOVED
        ANGALCLetterC1 = 120,          // REMOVED
        ANGALCLetterC2 = 121,          // REMOVED
        ANGALCLetterC3 = 123,          // REMOVED
        ANGReturnToDutyLetter = 124,
        ANGMEB_4_Non_ALC_C_SAF = 125,
        ANGMEB_4_Non_ALC_C_SG = 126,
        ANGMEB_DQ_AGR = 127,
        ANGMEB_DQ = 128,
        ANGMEB_RTD_AGR_SAF = 129,
        ANGMEB_RTD_AGR_SG = 130,
        ANGMEB_RTD_HIV_AGR_SAF = 131,
        ANGMEB_RTD_HIV_AGR_SG = 132,
        ANGMEB_RTD_HIV_SAF = 133,
        ANGMEB_RTD_HIV_SG = 134,
        ANGMEB_RTD_SAF = 135,
        ANGMEB_RTD_SG = 136,
        ANGWWD_4_Non_ALC_C = 137,
        ANGWWD_4_2_RTD = 138,

        //  ANGWWD_4_2_Non_ALC_C_OSA = 39,         // REMOVED
        ANGWWD_Admin_LOD_AFPC = 140,

        ANGMEB_Disqualification_Letter = 141,  // MEB_Disqualification_Letter for WWD
        ANGWWD_Disqualification_Letter = 142,  // REMOVED
        ANGWWD_HIV_Renewal_C2 = 143,

        //    ANGWWD_RTD_ALC_C_OSA = 44,            // REMOVED
        ANGWWD_RTD_SAF = 145,

        ANGWWD_RTD_SG = 146,

        //   ANGWWD_4_Non_ALC_C_OSA = 47,         // REMOVED
        ANGIRILO_4_Non_ALC_C = 148,

        ANGIRILO_4_2_RTD = 149,

        //   ANGIRILO_4_2_Non_ALC_C_OSA = 50,     // REMOVED
        ANGIRILO_Admin_LOD_AFPC = 151,

        ANGIRILO_MEB_Disqualification_Letter = 152, // MEB_Disqualification_Letter for IRILO
        ANGIRILO_Disqualification_Letter = 153, // REMOVED
        ANGIRILO_HIV_Renewal_C2 = 154,

        //    ANGIRILO_RTD_ALC_C_OSA = 55,        // REMOVED
        ANGIRILO_RTD_SAF = 156,

        ANGIRILO_RTD_SG = 157,

        //    ANGIRILO_4_Non_ALC_C_OSA = 58,     // REMOVED
        ANGReturn_Without_Action = 159,

        ANGWWD_Unit_Disqualification_Letter = 160,
        ANGMEB_4_Non_ALC_C_Exp_SAF = 161,
        ANGMEB_4_Non_ALC_C_Exp_SG = 162,
        ANGLodFindingsILODDeath = 163,
        ANGPalaceChaseMSD = 164,
        ANGPalaceChaseAFI = 165,
        ANGPalaceChaseQualified = 166,
        ANGRSALCMemo = 167,
        ANGRSNonALCMemo = 168,
        ANGDisapprovalAppeal = 169,
        ANGApprovalAppeal = 170,
        ANGPalaceChaseMSDandAFI = 171,
        ANGSARC_Determination_ILOD = 172,
        ANGSARC_Determination_NILOD = 173,
        ANGIRILO_DQ_MEB = 174,
        ANGIRILO_DQ_WWD = 175,
        ANGSARC_APPEAL_DISAPPROVAL = 176,
        ANGSARC_APPEAL_APPROVED = 177,
        ANGIRILO_DQ_PENDING = 178,
        ANGRW_4_Non_RW_C = 179,
        ANGRW_4_2_RTD = 180,
        ANGRW_Admin_LOD_AFPC = 181,
        ANGRW_HIV = 182,
        ANGRW_RTD_SAF = 183,
        ANGRW_RTD_SG = 184,
        ANGRW_DQ_MEB = 185,
        ANGRW_DQ_WD = 186,
        ANGRW_DQ_Pending_LOD = 187,
        NILOD_Informal_LOD_V3 = 212,
        NILOD_Formal_LOD_V3 = 213,
        NILOD_Informal_Death_LOD_V3 = 214,
        NILOD_Formal_Death_LOD_V3 = 207,
        ANG_IRILO_DQ_WWD_Pending = 215,
    };

    public enum PrintDocuments
    {
        FormARFC348 = 1,
        FormDD261 = 2,
        FormARFC348_v2 = 3,
        FormAFRC348R = 4,
        GeneralIRILO = 5
    }
}