namespace ALOD.Core.Domain.Workflow
{
    public enum AFRCWorkflows : int
    {
        LOD = 1,
        ReinvestigationRequest = 5,
        SpecCaseIncap = 6,
        SpecCaseCongress = 7,
        SpecCaseBMT = 8,
        SpecCaseWWD = 11,
        SpecCasePW = 12,
        SpecCaseMEB = 13,
        SpecCaseBCMR = 14,
        SpecCaseFT = 15,
        SpecCaseCMAS = 16,
        SpecCaseMEPS = 17,
        SpecCaseMMSO = 18,
        SpecCaseMH = 19,
        SpecCaseNE = 20,
        SpecCaseDW = 21,
        SpecCaseMO = 22,
        SpecCasePEPP = 23,
        SpecCaseRS = 24,
        SpecCasePH = 25,
        AppealRequest = 26,
        LOD_v2 = 27,
        SARCRestricted = 28,
        SARCRestrictedAppeal = 29,
        SpecCaseRW = 30,
        SpecCaseAGR = 31,
        SpecCasePSCD = 32,

        /* --- ANG ------ */
        ANGLOD = 101,
        ANGReinvestigationRequest = 105,
        ANGSpecCaseIncap = 106,
        ANGSpecCaseCongress = 107,
        ANGSpecCaseBMT = 108,
        ANGSpecCaseWWD = 111,
        ANGSpecCasePW = 112,
        ANGSpecCaseMEB = 113,
        ANGSpecCaseBCMR = 114,
        ANGSpecCaseFT = 115,
        ANGSpecCaseCMAS = 116,
        ANGSpecCaseMEPS = 117,
        ANGSpecCaseMMSO = 118,
        ANGSpecCaseMH = 119,
        ANGSpecCaseNE = 120,
        ANGSpecCaseDW = 121,
        ANGSpecCaseMO = 122,
        ANGSpecCasePEPP = 123,
        ANGSpecCaseRS = 124,
        ANGSpecCasePH = 125,
        ANGAppealRequest = 126,
        ANGLOD_v2 = 127,
        ANGSARCRestricted = 128,
        ANGSARCRestrictedAppeal = 129,
        ANGSpecCaseRW = 130,

        /* INCAP */
        INInitiate = 41,
        INAppeal = 42,
        INComplete = 43,
        INExtension = 44,
        INCancelled = 89,
        INHolding_RMU = 255,
        INMedicalReview_WG = 311,
        INHolding_PM = 312,
        INFinanceReview = 313,
        INDisposition = 314,
        INARCSMEConsult = 315,
        INImmediateCommanderReview = 319,
        INWingJAReview = 320,
        INWingCCAction = 321,
        INApproved = 322,
        INDisapproved = 323,
        INMedicalReview_WG_Ext = 355,
        INImmediateCommanderReview_Ext = 356,
        INFinanceReview_Ext = 357,
        INWingJAReview_Ext = 358,
        INWingCommanderRecommendation_Ext = 359,
        INOPR_Ext_HR_Review = 360,
        INOCR_Ext_HR_Review = 361,
        INDirectorOfStaffReview = 362,
        INCommandChiefReview = 363,
        INViceCommanderReview = 364,
        INDirectorOfPersonnelReview = 365,
        INCAFRAction = 366,
        INWingCCRecommendation_Appeal = 367,
        INOPR_Appeal_HR_Review = 368,
        INOCR_Appeal_HR_Review = 369,
        INDirectorOfStaffReview_Appeal = 370,
        INCommandChiefReview_Appeal = 371,
        INViceCommanderReview_Appeal = 372,
        INDirectorOfPersonnelReview_Appeal = 373,
        INCAFR_Action_Appeal = 374
    };

    public enum ModuleType
    {
        SpecialCasesALL = 0,
        System = 1,
        LOD = 2,
        ReinvestigationRequest = 5,
        SpecCaseIncap = 6,
        SpecCaseCongress = 7,
        SpecCaseBMT = 8,
        SpecCaseWWD = 9,
        SpecCasePW = 10,
        SpecCaseMEB = 11,
        SpecCaseBCMR = 12,
        SpecCaseFT = 13,
        SpecCaseCMAS = 14,
        SpecCaseMEPS = 15,
        SpecCaseMMSO = 16,
        SpecCaseMH = 17,
        SpecCaseNE = 18,
        SpecCaseDW = 19,
        SpecCaseMO = 20,
        SpecCasePEPP = 21,
        SpecCaseRS = 22,
        SpecCasePH = 23,
        AppealRequest = 24,
        SARC = 25,
        SARCAppeal = 26,
        SpecCaseRW = 27,
        SpecCaseAGR = 28,
        SpecCasePSCD = 30, // task 136

        /* --- ANG ------ */
        ANGSpecialCasesALL = 100,
        ANGSystem = 101,
        ANGLOD = 102,
        ANGReinvestigationRequest = 105,
        ANGSpecCaseIncap = 106,
        ANGSpecCaseCongress = 107,
        ANGSpecCaseBMT = 108,
        ANGSpecCaseWWD = 109,
        ANGSpecCasePW = 110,
        ANGSpecCaseMEB = 111,
        ANGSpecCaseBCMR = 112,
        ANGSpecCaseFT = 113,
        ANGSpecCaseCMAS = 114,
        ANGSpecCaseMEPS = 115,
        ANGSpecCaseMMSO = 116,
        ANGSpecCaseMH = 117,
        ANGSpecCaseNE = 118,
        ANGSpecCaseDW = 119,
        ANGSpecCaseMO = 120,
        ANGSpecCasePEPP = 121,
        ANGSpecCaseRS = 122,
        ANGSpecCasePH = 123,
        ANGAppealRequest = 124,
        ANGSARC = 125,
        ANGSARCAppeal = 126,
        ANGSpecCaseRW = 127
    }

    public enum OptionRules
    {
        Formal = 1,
        ValidatePreviousStatus = 2,
        ValidatePostStatus = 3,
        Document = 4,
        Memo = 5,
        LastStatusWas = 13,
        LastStatusWasNot = 14,
        PrevStatusWas = 17,
        PrevStatusWasNot = 18,
        MemoApproveRmuInitial = 123,
        MemoApproveRmufon = 124,
        MemoApproveHqInitial = 125,
        MemoApproveHqfon = 126,
        MemoDenied = 127,
        MemoValidateRmuInitial = 128,
        MemoValidateRmufon = 129,
        MemoValidateHqInitial = 130,
        MemoValidateHqfon = 131,
        MemoValidateDenied = 132
    }

    public enum PageAccessType
    {
        None = 0,
        ReadOnly = 1,
        ReadWrite = 2
    };

    public enum ReportingView
    {
        Total_View = 1,
        Non_Medical_Reporting_View = 2,
        Medical_Reporting_View = 3,
        RMU_View_Physical_Responsibility = 4,
        JA_View = 5,
        MPF_View = 6,
        System_Administration_View = 7,
        Old_RMU_type_view = 8
    };

    public enum WorkflowActionType
    {
        SendEmail = 1,
        SignMemo = 2,
        RemoveSignature = 3,
        LockDocuments = 4,
        SendDocumentsToStorage = 5,
        AddSignature = 9,
        ChangeToFormal = 11,
        SaveRWOA = 12,
        SaveFinalDecision = 13,
        AddApprovalAuthoritySignature = 14,
        SetApprovalDate = 15,
        SendLessonsLearnedEmail = 16,
        SetSAFUploadDate = 17,
        ChangeToInformal = 18,
        RecommendCancelFormal = 19,
        ReturnRWOA = 21,
        ReturnTo = 22,
        ReturnBack = 23
    }

    public class LODMOduleType
    {
        public const string Investigation = "investigation";
        public const string Medical = "medical";
        public const string Unit = "unit";
    }
}