namespace ALOD.Core.Domain.Modules.Lod
{
    public enum AGRSectionNames
    {
        AGR_MED_TECH_INIT = 1,
        AGR_HQT_REV = 2,
        AGR_BOARD_SG_REV = 3,
        AGR_HQT_FINAL_REV = 4,
        AGR_APPROVED = 5,
        AGR_DENIED = 6,
        AGR_SENIOR_MED_REV = 7,
        AGR_RLB = 22
    };

    public enum AppealStatusCode : int
    {
        AppealInitiation = 188,
        AppealBoardTechReview = 189,
        AppealBoardMedicalReview = 190,
        AppealBoardLegalReview = 191,
        AppealBoardAdminReview = 192,
        AppealApprovingAuthorityReview = 193,
        AppealAppellateAuthorityReview = 194,
        AppealApproved = 195,
        AppealDenied = 196,
        AppealCanceled = 197,
        AppealSeniorMedicalReview = 247,
        UnKnown = 0
    };

    public enum AppealWorkStatus : int
    {
        AppealInitiation = 190,
        AppealBoardTechReview = 191,
        AppealBoardMedicalReview = 192,
        AppealBoardLegalReview = 193,
        AppealBoardAdminReview = 194,
        AppealApprovingAuthorityReview = 195,
        AppealAppellateAuthorityReview = 196,
        AppealApproved = 197,
        AppealDenied = 198,
        AppealCanceled = 199,
        AppealSeniorMedicalReview = 272,
        UnKnown = 0
    };

    public enum APSASectionNames
    {
        APSA_INIT = 1,
        APSA_SARC_ADMIN = 2,
        APSA_BOARD_MED_REV = 3,
        APSA_BOARD_LEGAL_REV = 4,
        APSA_BOARD_ADMIN_REV = 5,
        APSA_APPELLATE_AUTH_REV = 6,
        APSA_APPROVED = 7,
        APSA_DENIED = 8,
        APSA_CANCELED = 9,
        APSA_SENIOR_MED_REV = 10,
        APSA_RLB = 22
    }

    public enum APSectionNames
    {
        //'These are just the page namess created to get the access
        //'of groups at each status (These are not status codes even though they look alike)
        AP_INIT = 1,

        AP_BOARD_TECH_REV = 2,
        AP_BOARD_MED_REV = 3,
        AP_BOARD_LEGAL_REV = 4,
        AP_BOARD_ADMIN_REV = 5,
        AP_APPROVING_AUTH_REV = 6,
        AP_APPEALLATE_AUTH_REV = 7,
        AP_APPROVED = 8,
        AP_DENIED = 9,
        AP_CANCLED = 10,
        AP_SENIOR_MED_REV = 11,
        AP_BOARD_SENIOR_MED_REV = 12,
        AP_RLB = 22
    };

    public enum BCSectionNames
    {
        //'These are just the page namess created to get the access
        //'of groups at each status (These are not status codes even though they look alike)
        BC_HQT_INIT = 1,

        BC_BOARD_SG_REV = 2,
        BC_HQT_FINAL_REV = 3,
        BC_COMPLETE = 4,
        BC_SENIOR_MED_REV = 5,
        BC_RLB = 22
    };

    public enum BTSectionNames
    {
        //'These are just the page namess created to get the access
        //'of groups at each status (These are not status codes even though they look alike)
        BT_HQT_INIT = 1,

        BT_BOARD_SG_REV = 2,
        BT_HQT_FINAL_REV = 3,

        //BT_COMPLETE = 4,
        BT_APPROVED = 4,

        BT_DENIED = 5,
        BT_SENIOR_MED_REV = 6,
        BT_RLB = 22
    };

    public enum CISectionNames
    {
        //'These are just the page namess created to get the access
        //'of groups at each status (These are not status codes even though they look alike)
        CI_HQT_INIT = 1,

        CI_BOARD_SG_REV = 2,
        CI_HQT_FINAL_REV = 3,
        CI_COMPLETE = 4,
        CI_SENIOR_MED_REV = 5,
        CI_RLB = 22
    };

    public enum CMSectionNames
    {
        //'These are just the page namess created to get the access
        //'of groups at each status (These are not status codes even though they look alike)
        CM_HQT_INIT = 1,

        CM_COMPLETE = 4,
        CM_RLB = 22
    };

    public enum CommentsTypes : int
    {
        UnitComments = 2,
        BoardComments = 3,
        SysAdminComments = 4,
        LessonsLearned = 8,
        CaseDialogue = 9
    };

    public enum DWSectionNames
    {
        DW_MED_TECH_INIT = 1,
        DW_HQT_REV = 2,
        DW_BOARD_SG_REV = 3,
        DW_HQT_FINAL_REV = 4,
        DW_APPROVED = 5,
        DW_DENIED = 6,
        DW_SENIOR_MED_REV = 7,
        DW_RLB = 22
    };

    public enum EMailTemplates : int
    {
        PWaiverApproved = 66,
        PWaiverDenied = 44,
        PWaiverApprovedFromMedOff = 71,
        PWaiverDeniedFromMedOff = 72,
        PWaiverReturned = 73,
        IncapReturned = 70,
        MMSOApproved = 74,
        MMSODenied = 75,
        MMSOAwaitingAction = 76
    };

    public enum FTSectionNames
    {
        //'These are just the page namess created to get the access
        //'of groups at each status (These are not status codes even though they look alike)
        FT_HQT_INIT = 1,

        FT_RTD = 4,
        FT_DISQUALIFIED = 5,
        FT_SENIOR_MED_REV = 6,
        FT_RLB = 22
    };

    public enum INSectionNames
    {
        //'These are just the page namess created to get the access
        //'of groups at each status (These are not status codes even though they look alike)
        IN_HQT_INIT = 1,

        IN_BOARD_SG_REV = 2,
        IN_HQT_FINAL_REV = 3,
        IN_COMPLETE = 4,
        IN_SENIOR_MED_REV = 5,
        IN_RLB = 22
    };

    public enum LodFinding
    {
        InLineOfDuty = 1,
        EptsLodNotApplicable = 2,
        EptsServiceAgravated = 3,
        NotInLineOfDutyDueToOwnMisconduct = 4,
        NotInLineOfDutyNotDueToOwnMisconduct = 5,
        RecommendFormalInvestigation = 6,
    };

    public enum LodRwoaReasons : byte
    {
        CANCEL_LOD = 1,
        MULTIPLE_DIAGNOSES = 2,
        NO_ORDERS = 3,
        WRONG_ORDERS_PROVIDED = 4,
        WRONG_DIAGNOSIS = 5,
        ORDERS_DO_NOT_COVER_ACTIVE_DUTY_SERVICE_IN_QUESTION = 6,
        POLICE_REPORT_NOT_INCLUDED_WITH_MVAS = 7,
        NO_MEDICAL_DOCUMENTATION = 8,
        INSUFFICIENT_MEDICAL_DOCUMENTATION = 9,
        SUPPORTING_DOCUMENTATION_PERTAINS_TO_DIFFERENT_INDIVIDUAL = 10,
        SUPPORTING_DOCUMENTATION_IS_DISTORTED_UNREADABLE = 11,
    };

    public enum LodStatusCode : int
    {
        MedTechReview = 1,
        MedicalOfficerReview = 3,
        UnitCommanderReview = 4,
        WingJAReview = 5,
        AppointingAutorityReview = 6,
        NotifyFormalInvestigator = 7,  //This goes to mpf/LOD-PM
        FormalInvestigation = 8,
        FormalActionByWingJA = 9,
        FormalActionByAppointingAuthority = 10,
        BoardReview = 11,
        BoardMedicalReview = 12,
        BoardLegalReview = 13,
        ApprovingAuthorityAction = 15,
        FormalBoardReview = 20,
        FormalBoardMedicalReview = 21,
        FormalBoardLegalReview = 22,
        FormalApprovingAuthorityAction = 24,
        FormalInvestigationDirected = 25,  //This goes to board
        Complete = 16,
        Cancelled = 29,
        WingSarcInput = 28, // Missing from previous Code, not sure if we are handling this step at all. On the Wotkflow Steps, there is no option to "Return" to this step.
        UnKnown = 0,
        AppointingAuthority_Pilot = 122,
        BoardPersonnelReview = 169,
        FormalBoardPersonnelReview = 170,
        SeniorMedicalReview = 229,
        FormalSeniorMedicalReview = 249,
        LODBoardPersonnelReview = 266,
        FormalLODBoardPersonnelReview = 269,
        MedicalTech_LODV3 = 292,
        MedicalOfficer_LODV3 = 293,
        UnitCC_LODV3 = 294,
        WingJA_LODV3 = 295,
        AppointingAutority_LODV3 = 296,
        Complete_LODV3 = 297,
        LODAudit_SG = 300,
        LODAudit_JA = 301,
        LODAudit_A1 = 302,
        ANGMedTechReview = 1001,
        ANGMedicalOfficerReview = 1003,
        ANGUnitCommanderReview = 1004,
        ANGWingJAReview = 1005,
        ANGAppointingAutorityReview = 1006,
        ANGNotifyFormalInvestigator = 1007,  //This goes to mpf/LOD-PM
        ANGFormalInvestigation = 1008,
        ANGFormalActionByWingJA = 1009,
        ANGFormalActionByAppointingAuthority = 1010,
        ANGBoardReview = 1011,
        ANGBoardMedicalReview = 1012,
        ANGBoardLegalReview = 1013,
        ANGApprovingAuthorityAction = 1015,
        ANGFormalBoardReview = 1020,
        ANGFormalBoardMedicalReview = 1021,
        ANGFormalBoardLegalReview = 1022,
        ANGFormalApprovingAuthorityAction = 1024,
        ANGFormalInvestigationDirected = 1025,  //This goes to board
        ANGComplete = 1016,
        ANGCancelled = 1029,
        ANGWingSarcInput = 1028, // Missing from previous Code, not sure if we are handling this step at all. On the Wotkflow Steps, there is no option to "Return" to this step.
        ANGBoardPersonnelReview = 1169,
        ANGFormalBoardPersonnelReview = 1170,
        ANGSeniorMedicalReview = 1229,
        ANGFormalSeniorMedicalReview = 1249
    };

    public enum LodWorkStatus : int
    {
        UnKnown = 0,
        MedTechReview = 1,
        MedicalOfficerReview = 2,
        UnitCommanderReview = 3,
        WingJAReview = 4,
        AppointingAutorityReview = 5,
        NotifyFormalInvestigator = 7,  //This goes to mpf/LOD-PM
        BoardReview = 8,
        BoardSeniorReview = 9,
        BoardMedicalReview = 10,
        FormalInvestigation = 11,
        Complete = 13,
        FormalActionByWingJA = 14,
        FormalActionByAppointingAuthority = 15,
        BoardLegalReview = 16,
        ApprovingAuthorityAction = 17,
        FormalBoardReview = 18,
        FormalBoardMedicalReview = 19,
        FormalBoardLegalReview = 20,
        FormalBoardSeniorReview = 21,
        FormalApprovingAuthorityAction = 22,
        FormalInvestigationDirected = 23,  //This goes to board
        WingSarcInput = 25,
        Cancelled = 26, // Missing from previous Code, not sure if we are handling this step at all. On the Wotkflow Steps, there is no option to "Return" to this step.
        BoardPersonnelReview = 171,
        FormalBoardPersonnelReview = 172
    };

    // Should be reorganized!!!
    // ***************************** IMPORTANT *****************************
    // For all the Statuses, use the column ws_id, view [vw_WorkStatus]
    public enum LodWorkStatus_v2 : int
    {
        UnKnown = 0,
        MedTechReview = 200,
        MedicalOfficerReview = 201,
        UnitCommanderReview = 202,
        WingJAReview = 203, // Deprecated, Informal Wing JA Review v2, Use v3
        AppointingAutorityReview = 204,
        NotifyFormalInvestigator = 205,  //This goes to mpf/LOD-PM
        BoardReview = 208,
        BoardMedicalReview = 210,
        FormalInvestigation = 206,
        Complete = 220,
        FormalActionByWingJA = 213, // Formal Wing JA Review
        FormalActionByAppointingAuthority = 214, // Formal Wing CC Review
        BoardLegalReview = 209,
        ApprovingAuthorityAction = 212,
        FormalBoardReview = 215,
        FormalBoardMedicalReview = 216,
        FormalBoardLegalReview = 217,
        FormalApprovingAuthorityAction = 219,
        WingSarcInput = 207,
        Cancelled = 221, // Missing from previous Code, not sure if we are handling this step at all. On the Wotkflow Steps, there is no option to "Return" to this step.
        BoardPersonnelReview = 211,
        FormalBoardPersonnelReview = 218,
        FormalSeniorMedicalReview = 274,
        SeniorMedicalReview = 254,

        /*  ANG  below */
        ANGMedTechReview = 1200,
        ANGMedicalOfficerReview = 1201,
        ANGUnitCommanderReview = 1202,
        ANGWingJAReview = 1203,
        ANGAppointingAutorityReview = 1204,
        ANGNotifyFormalInvestigator = 1205,  //This goes to mpf/LOD-PM
        ANGBoardReview = 1208,
        ANGBoardMedicalReview = 1210,
        ANGFormalInvestigation = 1206,
        ANGComplete = 1220,
        ANGFormalActionByWingJA = 1213,
        ANGFormalActionByAppointingAuthority = 1214,
        ANGBoardLegalReview = 1209,
        ANGApprovingAuthorityAction = 1212,
        ANGFormalBoardReview = 1215,
        ANGFormalBoardMedicalReview = 1216,
        ANGFormalBoardLegalReview = 1217,
        ANGFormalApprovingAuthorityAction = 1219,
        ANGWingSarcInput = 1207,
        ANGCancelled = 1221, // Missing from previous Code, not sure if we are handling this step at all. On the Wotkflow Steps, there is no option to "Return" to this step.
        ANGBoardPersonnelReview = 1211,
        ANGFormalBoardPersonnelReview = 1218,
        ANGFormalSeniorMedicalReview = 1274,
        ANGSeniorMedicalReview = 1254,
    };

    // For all the Statuses, use the column ws_id, view [vw_WorkStatus]
    public enum LodWorkStatus_v3 : int
    {
        MedicalTechReview_LODV3 = 325, // Informal Med Tech Review (or Pilot ?)
        MedicalOfficerReview_LODV3 = 326, // Informal Med Officer Review
        UnitCommanderReview_LODV3 = 327, // Informal Unit CC Review
        WingJA_LODV3 = 328, // Informal Wing JA Review
        AppointingAutorityReview_LODV3 = 329, // Informal Wing CC Review
        Complete_LODV3 = 330
    }

    public enum MBSectionNames
    {
        //'These are just the page namess created to get the access
        //'of groups at each status (These are not status codes even though they look alike)
        MB_HQT_FINAL_REV = 4,

        //MB_COMPLETE = 5,
        MB_RTD = 5,

        MB_DISQUALIFIED = 6,
        MB_SENIOR_MED_REV = 7,
        MB_RLB = 22
    };

    public enum MHSectionNames
    {
        //'These are just the page namess created to get the access
        //'of groups at each status (These are not status codes even though they look alike)
        MH_HQT_INIT = 1,

        MH_BOARD_SG = 2,
        MH_HQT_FINAL_REV = 3,

        //MH_COMPLETE = 4,
        MH_APPROVED = 4,

        MH_DISAPPROVED = 5,
        MH_SENIOR_MED_REV = 6,
        MH_RLB = 22
    };

    public enum MMSectionNames
    {
        MM_INPUT = 1,
        MM_UCINPUT = 2,
        MM_REVIEW = 3,
        MM_COMPLETE = 4
    };

    public enum MOSectionNames
    {
        MO_MED_TECH_INIT = 1,
        MO_HQT_INIT = 2,
        MO_HQT_REV = 3,
        MO_BOARD_SG_REV = 4,
        MO_HQT_FINAL_REV = 5,
        MO_APPROVED = 6,
        MO_DENIED = 7,
        MO_SENIOR_MED_REV = 8,
        MO_RLB = 22
    };

    public enum MPSectionNames
    {
        //'These are just the page namess created to get the access
        //'of groups at each status (These are not status codes even though they look alike)
        MP_HQT_INIT = 1,

        MP_COMPLETE = 4,
        MP_RLB = 22
    };

    public enum NESectionNames
    {
        //'These are just the page namess created to get the access
        //'of groups at each status (These are not status codes even though they look alike)
        NE_MED_TECH_INIT = 1,

        NE_HQT_REV = 2,
        NE_BOARD_SG_REV = 3,
        NE_HQT_FINAL_REV = 4,
        NE_APPROVED = 5,
        NE_DENIED = 6,
        NE_SENIOR_MED_REV = 7,
        NE_RLB = 22
    };

    public enum PersonnelTypes : short
    {
        MED_TECH = 1,
        MED_OFF = 2,
        UNIT_CMDR = 3,
        WING_JA = 4,
        APPOINT_AUTH = 5, //Wing CC
        BOARD = 6, //Approving Authorty
        BOARD_JA = 7, //Board Legal
        BOARD_SG = 8, //Board Med
        BOARD_SR = 9,
        BOARD_AA = 10,
        MPF = 11,
        FORMAL_WING_JA = 12,
        FORMAL_APP_AUTH = 13,
        FORMAL_BOARD_RA = 14,
        FORMAL_BOARD_JA = 15,
        FORMAL_BOARD_SG = 16,
        FORMAL_BOARD_SR = 17,
        FORMAL_BOARD_AA = 18,
        IO = 19,
        LOD_MFP = 20,
        LOD_PM = 21,
        BOARD_A1 = 22,
        FORMAL_BOARD_A1 = 23,
        APPELLATE_AUTH = 24,
        WING_SARC_RSL = 25,
        SARC_ADMIN = 26,
        SENIOR_MEDICAL_REVIEWER = 27,
        FORMAL_SENIOR_MEDICAL_REVIEWER = 28
    };

    public enum PESectionNames
    {
        PE_HQT_INIT = 1,
        PE_RWOA_HOLD = 2,
        PE_COMPLETE = 3,
        PE_BOARD_SG_REV = 4,
        PE_HQT_FINAL_REV = 5,
        PE_SENIOR_MED_REV = 6,
        PE_RLB = 22
    };

    public enum PHSectionNames
    {
        PH_UNIT_INIT = 1,
        PH_HQDPH_REV = 2,
        PH_COMPLETE = 3,
        PH_RLB = 22
    };

    //PSCD is PSCD
    public enum PSCDSectionNames
    {
        PSCD_MEDTECH_INIT = 1,
        PSCD_MEDOFF = 2,
        PSCD_HQMED = 3,
        PSCD_BOARDMED = 4,
        PSCD_SENIORMED = 5,
        PSCD_BOARDLEGAL = 6,
        PSCD_APPROVINGAUTHORITY = 7,
        PSCD_BOARDADMIN = 8,
        PSCD_RLB = 22
    };

    public enum PWSectionNames
    {
        //'These are just the page namess created to get the access
        //'of groups at each status (These are not status codes even though they look alike)
        PW_MED_TECH_INIT = 1,

        PW_HQT_REV = 2,
        PW_BOARD_SG_REV = 3,
        PW_HQT_FINAL_REV = 4,
        PW_APPROVED = 5,
        PW_DENIED = 6,
        PW_SENIOR_MED_REV = 7,
        PW_RLB = 22
    };

    public enum ReinvestigationRequestStatusCode : int
    {
        InitiateRequest = 30,
        WingJARequestReview = 31,
        WingCCRequestReview = 32,
        BoardTechRequestReview = 33,
        BoardMedicalRequestReview = 34,
        BoardLegalRequestReview = 35,
        ApprovingAuthorityRequestAction = 36,
        BoardTechRequestFinalReview = 39,
        RequestApproved = 38,
        RequestDenied = 40,
        Cancelled = 85,
        UnKnown = 0,
        BoardA1RequestReview = 171,
        SeniorMedicalReview = 246
    };

    public enum ReinvestigationRequestWorkStatus : int
    {
        InitiateRequest = 27,
        WingJARequestReview = 28,
        WingCCRequestReview = 29,
        BoardTechRequestReview = 30,
        BoardMedicalRequestReview = 31,
        BoardLegalRequestReview = 32,
        ApprovingAuthorityRequestAction = 33,
        BoardTechRequestFinalReview = 38,
        RequestApproved = 36,
        RequestDenied = 39,
        Cancelled = 88,
        UnKnown = 0,
        BoardA1RequestReview = 173,
        SeniorMedicalReview = 271
    };

    public enum RRSectionNames
    {
        //'These are just the page namess created to get the access
        //'of groups at each status (These are not status codes even though they look alike)
        RR_INIT = 1,

        RR_WING_JA_REV = 2,
        RR_WING_CC_REV = 3,
        RR_BOARD_TECH_REV = 4,
        RR_BOARD_MED_REV = 5,
        RR_BOARD_LEGAL_REV = 6,
        RR_BOARD_APPROVING_AUTH_REV = 7,
        RR_BOARD_TECH_FINAL = 8,
        RR_APPROVED = 9,
        RR_DENIED = 10,
        RR_RLB = 22,
        RR_BOARD_A1_REV = 23,
        RR_SENIOR_MED_REV = 24
    };

    public enum RSSectionNames
    {
        RS_HQT_INIT = 1,
        RS_BOARD_SG_REV = 2,
        RS_HQT_FINAL_REV = 3,
        RS_RECRUITER_COMMENTS = 4,
        RS_QUALIFIED = 5,
        RS_DISQUALIFIED = 6,
        RS_SENIOR_MED_REV = 7,
        RS_RLB = 22
    };

    public enum RWSectionNames
    {
        //'These are just the page namess created to get the access
        //'of groups at each status (These are not status codes even though they look alike)
        RW_MED_TECH_INIT = 1,

        RW_HQ_TECH_INIT = 2,
        RW_HQT_REV = 3,
        RW_BOARD_SG_REV = 4,
        RW_HQT_FINAL_REV = 5,
        RW_RTD = 6,
        RW_DISQUALIFIED = 7,
        RW_SENIOR_MED_REV = 8,
        RW_RLB = 22
    };

    public enum SARCAppealStatusCode : int
    {
        Initiate = 206,
        SARCAdminReview = 207,
        BoardMedicalReview = 208,
        BoardLegalReview = 209,
        AppellateAuthorityReview = 210,
        BoardAdminReview = 211,
        Approved = 212,
        Denied = 213,
        Canceled = 214,
        SeniorMedicalReview = 248,
        Unkown = 0
    }

    public enum SARCAppealWorkStatus : int
    {
        Initiate = 231,
        SARCAdminReview = 232,
        BoardMedicalReview = 233,
        BoardLegalReview = 234,
        AppellateAuthorityReview = 235,
        BoardAdminReview = 236,
        Approved = 237,
        Denied = 238,
        Canceled = 239,
        SeniorMedicalReview = 273,
        Unkown = 0
    }

    public enum SARCRestrictedSectionNames
    {
        SARC_WING_INIT = 1,
        SARC_ADMIN_REV = 2,
        SARC_BOARD_MEDICAL_REV = 3,
        SARC_BOARD_JA_REV = 4,
        SARC_APPROV_REV = 5,
        SARC_WING_COMPLETE = 6,
        SARC_SENIOR_MED_REV = 7,
        SARC_RLB = 22
    };

    public enum SARCRestrictedStatusCode : int
    {
        SARCInitiate = 198,
        SARCAdminReview = 199,
        SARCBoardMedicalReview = 200,
        SARCBoardJAReview = 201,
        SARCApprovingAuthorityReview = 202,
        SARCBoardAdminReview = 203,
        SARCComplete = 204,
        SARCCancelled = 205,
        SARCSeniorMedicalReview = 244
    };

    public enum SARCRestrictedWorkStatus : int
    {
        SARCInitiate = 223,
        SARCAdminReview = 224,
        SARCBoardMedicalReview = 225,
        SARCBoardJAReview = 226,
        SARCApprovingAuthorityReview = 227,
        SARCBoardAdminReview = 228,
        SARCComplete = 229,
        SARCCancelled = 230,
        SARCSeniorMedicalReview = 269,
        UnKnown = 0
    };

    public enum SARCSectionNames
    {
        SARC_INIT = 1,
        SARC_ADMIN = 2,
        SARC_BOARD_MED_REV = 3,
        SARC_BOARD_JA_REV = 4,
        SARC_BOARD_APPROVING_AUTH_REV = 5,
        SARC_BOARD_ADMIN_REV = 6,
        SARC_COMPELTED = 7,
        SARC_CANCELLED = 8,
        SARC_SENIOR_MED_REV = 9,
        SARC_RLB = 22
    };

    public enum SCFastTrackTypes : int
    {
        // New "Types" based on ICD 9 Code
        Sleep = 1045, // 327

        Diabetes = 251, // 250
        Asthma = 498 // 493
    }

    public enum SectionNames
    {
        //'These are just the page namess created to get the access
        //'of groups at each status (These are not status codes even though they look alike)
        MED_TECH_REV = 1,

        MED_OFF_REV = 2,
        UNIT_CMD_REV = 3,
        WING_JA_REV = 4,
        APP_AUTH_REV = 5,
        NOTIFY_IO = 6,
        IO = 7,
        FORMAL_ACTION_WING_JA = 8,
        FORMAL_ACTION_APP_AUTH = 9,
        BOARD_REV = 10,
        BOARD_MED_REV = 11,
        BOARD_LEGAL_REV = 12,
        BOARD_SENIOR_REV = 13,
        BOARD_APPROVING_AUTH_REV = 14,

        FORMAL_BOARD_REV = 15,
        FORMAL_BOARD_MED_REV = 16,
        FORMAL_BOARD_LEGAL_REV = 17,
        FORMAL_BOARD_SENIOR_REV = 18,
        FORMAL_BOARD_APPROVING_AUTH_REV = 19,
        COMPLETED = 20,
        APPOINT_IO = 21,
        RLB = 22,

        BOARD_PERSONNEL_REV = 23,
        FORMAL_BOARD_PERSONNEL_REV = 24,

        SENIOR_MED_REV = 25,
        FORMAL_SENIOR_MED_REV = 26
    };

    public enum SpecCaseAGRStatusCode : int
    {
        InitiateCase = 270,
        PackageReview = 271,
        MedicalReview = 272,
        FinalReview = 274,
        Approved = 275,
        HqTechInitiate = 278,
        Denied = 273,
        Cancel = 276,
        SeniorMedicalReview = 277,
        /* Wing */
        MedicalOfficerReview = 279,
        LocalFinalReview = 280,
        Unknown = 0
    };

    public enum SpecCaseAGRWorkStatus : int
    {
        InitiateCase = 300,
        PackageReview = 301,
        MedicalReview = 302,
        FinalReview = 304,
        Approved = 305,
        Denied = 303,
        HqInitiateCase = 308,
        Cancel = 306,
        SeniorMedicalReview = 307,
        /* Wing */
        MedicalOfficerReview = 309,
        LocalFinalReview = 310,
        Unknown = 0
    };

    public enum SpecCaseBCStatusCode : int
    {
        InitiateCase = 67,
        MedicalReview = 120,
        FinalReview = 121,
        Complete = 68,
        Cancelled = 93,
        SeniorMedicalReview = 236,
        UnKnown = 0
    };

    public enum SpecCaseBCWorkStatus : int
    {
        InitiateCase = 68,
        MedicalReview = 122,
        FinalReview = 123,
        Complete = 69,
        Cancelled = 96,
        SeniorMedicalReview = 261,
        UnKnown = 0
    };

    public enum SpecCaseBMTStatusCode : int
    {
        InitiateCase = 61,
        MedicalReview = 62,
        FinalReview = 63,
        Cancelled = 88,
        Approved = 141,
        Denied = 142,
        SeniorMedicalReview = 232,
        UnKnown = 0
    };

    public enum SpecCaseBMTWorkStatus : int
    {
        InitiateCase = 47,
        MedicalReview = 48,
        FinalReview = 49,
        Cancelled = 91,
        Approved = 143,
        Denied = 144,
        SeniorMedicalReview = 257,
        UnKnown = 0
    };

    public enum SpecCaseCMASStatusCode : int
    {
        InitiateCase = 70,
        Complete = 73,
        Cancelled = 94,
        UnKnown = 0
    };

    public enum SpecCaseCMASWorkStatus : int
    {
        InitiateCase = 71,
        Complete = 74,
        Cancelled = 97,
        UnKnown = 0
    };

    public enum SpecCaseCongressStatusCode : int
    {
        InitiateCase = 65,
        MedicalReview = 118,
        FinalReview = 119,
        Complete = 66,
        Cancelled = 87,
        SeniorMedicalReview = 231,
        UnKnown = 0
    };

    public enum SpecCaseCongressWorkStatus : int
    {
        InitiateCase = 45,
        MedicalReview = 120,
        FinalReview = 121,
        Complete = 46,
        Cancelled = 90,
        SeniorMedicalReview = 256,
        UnKnown = 0
    };

    public enum SpecCaseDWStatusCode : int
    {
        InitiateCase = 122,
        PackageReview = 123,
        MedicalReview = 124,
        FinalReview = 125,
        FinalReviewPending = 136,
        Complete = 126,
        Cancel = 127,
        SeniorMedicalReview = 240,
        Unknown = 0
    };

    public enum SpecCaseDWWorkStatus : int
    {
        InitiateCase = 124,
        PackageReview = 125,
        MedicalReview = 126,
        FinalReview = 127,
        FinalReviewPending = 138,
        Complete = 128,
        Cancel = 129,
        SeniorMedicalReview = 265,
        Unknown = 0
    };

    public enum SpecCaseFTStatusCode : int
    {
        InitiateCase = 69,
        PackageReview = 81,
        MedicalReview = 82,
        FinalReview = 83,
        RTD = 143,
        Disqualified = 144,
        Cancelled = 84,
        AdminLOD = 110,
        IMRPHolding = 216,
        SeniorMedicalReview = 237,
        UnKnown = 0
    };

    public enum SpecCaseFTWorkStatus : int
    {
        InitiateCase = 70,
        PackageReview = 84,
        MedicalReview = 85,
        FinalReview = 86,
        RTD = 145,
        Disqualified = 146,
        Cancelled = 87,
        AdminLOD = 112,
        IMRPHolding = 241,
        HQTechInitiate = 244,
        SeniorMedicalReview = 262,
        UnKnown = 0
    };

    public enum SpecCaseIncapStatusCode : int
    {
        IN_Initiate = 57,
        IN_Appeal = 58,
        IN_Complete = 59,
        IN_Extension = 60,
        IN_Cancelled = 86,
        IN_Holding_RMU = 230,
        IN_MedicalReview_WG = 281,
        IN_Holding_PM = 282,
        IN_FinanceReview = 283,

        //IN_MedicalReview_WG = 284,
        IN_ARCSME_Consult = 285,

        IN_ImmediateCommanderReview = 286,
        IN_WingJAReview = 287,
        IN_WingCCAction = 288,
        IN_Approved = 289,
        IN_Disapproved = 290,
        IN_MedicalReview_WG_Extension = 320,
        IN_ImmediateCommanderReview_Extension = 321,
        IN_FinanceReview_Extension = 322,
        IN_WingJAReview_Extension = 323,
        IN_WingCommanderRecommendation_Extension = 324,
        IN_OPRExtension_HRReview = 325,
        IN_OCRExtension_HRReview = 326,
        IN_DirectorOfStaffReview = 327,
        IN_CommandChiefReview = 328,
        IN_ViceCommanderReview = 329,
        IN_DirectorOfPersonnelReview = 330,
        IN_CAFRAction = 331,
        IN_WingCommanderRecommendation_Appeal = 332,
        IN_OPRAppeal_HRReview = 333,
        IN_OCRAppeal_HRReview = 334,
        IN_DirectorOfStaffReview_Appeal = 335,
        IN_CommandChiefReview_Appeal = 336,
        IN_ViceCommanderReview_Appeal = 337,
        IN_DirectorOfPersonnelReview_Appeal = 338,
        IN_CAFRAction_Appeal = 339,
        UnKnown = 0
    };

    public enum SpecCaseIncapWorkStatus : int
    {
        INInitiate = 41,
        INAppeal = 42,
        INComplete = 43,
        INExtension = 44,
        INCancelled = 89,
        INHolding_RMU = 255,
        INMedicalReview_WG = 311,
        INHolding_PM = 312,
        INFinanceReview = 313,

        //INMedicalReview_WG = 314,
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

    public enum SpecCaseMEBStatusCode : int
    {
        InitiateCase = 54,
        RWOAAction = 91,
        MedTechInput = 75,
        MedicalReview = 76,
        FinalReview = 80,
        AwaitingAFPC = 92,
        RTD = 139,
        Disqualified = 140,
        Cancelled = 77,
        MedicalReviewAFPC = 151,
        FinalReviewAFPC = 152,
        IPEBDisposition = 156,
        FPEBDisposition = 157,
        TransitionPhase = 158,
        MedicalReviewIPEB = 163,
        FinalReviewIPEB = 164,
        MedicalReviewFPEB = 165,
        FinalReviewFPEB = 166,
        MedicalReviewTransitionPhase = 167,
        FinalReviewTransitionPhase = 168,
        SeniorMedicalReview = 235,
        SeniorMedicalReviewAFPC = 250,
        SeniorMedicalReviewIPEB = 251,
        SeniorMedicalReviewFPEB = 252,
        SeniorMedicalReviewTransitionPhase = 253,
        UnKnown = 0
    };

    public enum SpecCaseMEBWorkStatus : int
    {
        InitiateCase = 65,
        RWOAAction = 94,
        MedTechInput = 78,
        MedicalReview = 79,
        FinalReview = 83,
        AwaitingAFPC = 95,
        RTD = 141,
        Disqualified = 142,
        Cancelled = 80,
        MedicalReviewAFPC = 153,
        FinalReviewAFPC = 154,
        IPEBDisposition = 158,
        FPEBDisposition = 159,
        TransitionPhase = 160,
        MedicalReviewIPEB = 165,
        FinalReviewIPEB = 166,
        MedicalReviewFPEB = 167,
        FinalReviewFPEB = 168,
        MedicalReviewTransitionPhase = 169,
        FinalReviewTransitionPhase = 170,
        SeniorMedicalReview = 260,
        SeniorMedicalReviewAFPC = 275,
        SeniorMedicalReviewIPEB = 276,
        SeniorMedicalReviewFPEB = 277,
        SeniorMedicalReviewTransitionPhase = 278,
        UnKnown = 0
    };

    public enum SpecCaseMEPSWorkStatus : int
    {
        InitiateCase = 76,
        Complete = 77,
        Cancelled = 98,
        UnKnown = 0
    };

    public enum SpecCaseMHStatusCode : int
    {
        InitiateCase = 104,
        MedicalReview = 105,
        FinalReview = 106,
        Cancelled = 108,
        Override = 109,
        Approved = 137,
        Disapproved = 138,
        Holding = 215,
        SeniorMedicalReview = 238,
        UnKnown = 0
    };

    public enum SpecCaseMHWorkStatus : int
    {
        InitiateCase = 106,
        MedicalReview = 107,
        FinalReview = 108,
        Cancelled = 110,
        Override = 111,
        Approved = 139,
        Disapproved = 140,
        Holding = 240,
        SeniorMedicalReview = 263,
        UnKnown = 0
    };

    public enum SpecCaseMMSOWorkStatus : int
    {
        InitiateCase = 99,
        SMRInput = 100,
        ContractRepReview = 101,
        NurseReview = 102,
        SPOCReview = 103,
        Approved = 104,
        Denied = 105,
        Cancelled = 150,
        Unknown = 0
    };

    public enum SpecCaseMOStatusCode : int
    {
        MedTechInitiateCase = 128,
        HQAFRCTechInitiateCase = 129,
        PackageReview = 130,
        MedicalReview = 131,
        FinalReview = 132,
        Cancelled = 133,
        Approved = 134,
        Denied = 135,
        SeniorMedicalReview = 241,
        Unknown = 0
    };

    public enum SpecCaseMOWorkStatus : int
    {
        MedTechInitiateCase = 130,
        HQAFRCTechInitiateCase = 131,
        PackageReview = 132,
        MedicalReview = 133,
        FinalReview = 134,
        Cancelled = 135,
        Approved = 136,
        Denied = 137,
        SeniorMedicalReview = 266,
        Unknown = 0
    };

    public enum SpecCaseNEStatusCode : int
    {
        InitiateCase = 111,
        PackageReview = 112,
        MedicalReview = 113,
        FinalReview = 114,
        Cancelled = 115,
        Approved = 116,
        Denied = 117,
        HQInitiate = 217,
        Holding = 218,
        SeniorMedicalReview = 239,
        Unknown = 0
    };

    public enum SpecCaseNEWorkStatus : int
    {
        InitiateCase = 113,
        PackageReview = 114,
        MedicalReview = 115,
        FinalReview = 116,
        Cancelled = 117,
        Approved = 118,
        Denied = 119,
        HQInitiate = 242,
        Holding = 243,
        SeniorMedicalReview = 264,
        Unknown = 0
    };

    public enum SpecCasePEPPStatusCode : int
    {
        InitiateCase = 145,
        RWOAHold = 146,
        Complete = 147,
        Cancelled = 153,
        MedicalReview = 172,
        FinalReview = 173,
        ExtAgencyDisposition = 174,
        SeniorMedicalReview = 242,
        Unknown = 0
    };

    public enum SpecCasePEPPWorkStatus : int
    {
        InitiateCase = 147,
        RWOAHold = 148,
        Complete = 149,
        Cancelled = 155,
        MedicalReview = 174,
        FinalReview = 175,
        ExtAgencyDisposition = 176,
        SeniorMedicalReview = 267,
        Unknown = 0
    };

    public enum SpecCasePHStatusCode : int
    {
        InitiateCase = 182,
        AFRCReview = 183,
        Delinquent = 184,
        DelinquentAFRCReview = 185,
        Complete = 186,
        Cancelled = 187,
        Unknown = 0
    };

    public enum SpecCasePHWorkStatus : int
    {
        InitiateCase = 184,
        AFRCReview = 185,
        Delinquent = 186,
        DelinquentAFRCReview = 187,
        Complete = 188,
        Cancelled = 189,
        Unknown = 0
    };

    public enum SpecCasePSCDStatusCode : int
    {
        //StatusId
        MedTechInitiate = 298, // 302,

        MedOffReview = 299,  //303,
        HQMedTech = 304,  //300,
        BoardMedicalOfficerHQ = 305, // 301,
        BoardMedicalOfficerHQSMR = 306, // 302,
        AFRCJA = 307, // 303,
        ApprovingAuthority = 308, // 304,
        BoardTechComplete = 309, // 305,
        Cancelled = 310, // 306,
        BoardPersonnel = 313, //
        Unknown = 0
    }

    public enum SpecCasePSCDWorkStatus : int
    {
        //this is the WS_ID
        MedTechInit = 335,

        MedOffRev = 336,
        HQMedTech = 337,
        BoardMedOffHQ = 338,
        BoardMedOffHQSMR = 339,
        AFRCJA = 340,  //AKA Board Legal
        ApprovAuth = 341,
        HQMedTech_FinalReview = 342,
        Cancelled = 343,
        BoardAdmin = 346,
        Unknown = 0
    }

    public enum SpecCasePWStatusCode : int
    {
        InitiateCase = 47,
        PackageReview = 48,
        MedicalReview = 49,
        Denied = 50,
        FinalReview = 51,
        Approved = 52,
        Cancelled = 90,
        SeniorMedicalReview = 234,
        UnKnown = 0
    };

    public enum SpecCasePWWorkStatus : int
    {
        InitiateCase = 58,
        PackageReview = 59,
        MedicalReview = 60,
        Denied = 61,
        FinalReview = 62,
        Approved = 63,
        Cancelled = 93,
        SeniorMedicalReview = 259,
        UnKnown = 0
    };

    public enum SpecCaseRSStatusCode : int
    {
        InitiateCase = 175,
        MedicalReview = 176,
        FinalReview = 177,
        RecruiterComments = 178,
        Qualified = 179,
        Disqualified = 180,
        Cancelled = 181,
        SeniorMedicalReview = 243,
        Unknown = 0
    };

    public enum SpecCaseRSWorkStatus : int
    {
        InitiateCase = 177,
        MedicalReview = 178,
        FinalReview = 179,
        RecruiterComments = 180,
        Qualified = 181,
        Disqualified = 182,
        Cancelled = 183,
        SeniorMedicalReview = 268,
        Unknown = 0
    };

    public enum SpecCaseRWStatusCode : int
    {
        MedTechInitiateCase = 220,
        HQAFRCTechInitiateCase = 221,
        PackageReview = 222,
        MedicalReview = 223,
        FinalReview = 224,
        RTD = 225,
        Disqualified = 226,
        Cancelled = 227,
        AdminLOD = 228,
        SeniorMedicalReview = 245,
        Unknown = 0
    };

    public enum SpecCaseRWWorkStatus : int
    {
        MedTechInitiateCase = 245,
        HQAFRCTechInitiateCase = 246,
        PackageReview = 247,
        MedicalReview = 248,
        FinalReview = 249,
        RTD = 250,
        Disqualified = 251,
        Cancelled = 252,
        AdminLOD = 253,
        SeniorMedicalReview = 270,
        Unknown = 0
    };

    public enum SpecCaseWWDStatusCode : int
    {
        InitiateCase = 41,
        PackageReview = 42,
        MedicalReview = 43,
        Denied = 44,
        FinalReview = 45,
        Approved = 46,
        Cancelled = 78,
        AdminLOD = 79,
        AwaitingSAF = 89,
        MedicalReviewSAF = 149,
        FinalReviewSAF = 150,
        IPEBDisposition = 154,
        FPEBDisposition = 155,
        MedicalReviewIPEB = 159,
        FinalReviewIPEB = 160,
        MedicalReviewFPEB = 161,
        FinalReviewFPEB = 162,
        SeniorMedicalReview = 233,
        SeniorMedicalReviewSAF = 254,
        SeniorMedicalReviewIPEB = 255,
        SeniorMedicalReviewFPEB = 256,
        UnKnown = 0
    };

    public enum SpecCaseWWDWorkStatus : int
    {
        InitiateCase = 52,
        PackageReview = 53,
        MedicalReview = 54,
        Denied = 55,
        FinalReview = 56,
        Approved = 57,
        Cancelled = 81,
        AdminLOD = 82,
        AwaitingSAF = 92,
        MedicalReviewSAF = 151,
        FinalReviewSAF = 152,
        IPEBDisposition = 156,
        FPEBDisposition = 157,
        MedicalReviewIPEB = 161,
        FinalReviewIPEB = 162,
        MedicalReviewFPEB = 163,
        FinalReviewFPEB = 164,
        SeniorMedicalReview = 258,
        SeniorMedicalReviewSAF = 279,
        SeniorMedicalReviewIPEB = 280,
        SeniorMedicalReviewFPEB = 281,
        UnKnown = 0
    };

    public enum WDSectionNames
    {
        //'These are just the page namess created to get the access
        //'of groups at each status (These are not status codes even though they look alike)
        WD_MED_TECH_INIT = 1,

        WD_HQT_REV = 2,
        WD_BOARD_SG_REV = 3,
        WD_HQT_FINAL_REV = 4,
        WD_APPROVED = 5,
        WD_DENIED = 6,
        WD_SENIOR_MED_REV = 7,
        WD_RLB = 22
    };
}