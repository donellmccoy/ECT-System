namespace ALOD.Core.Utils
{
    public enum AccessDeniedType
    {
        CaseDetails = 1
    }

    public enum AdHocReportType
    {
        Standard = 1,
        PH_Totals = 2
    }

    public enum DataType
    {
        DT_Integer = 1,
        DT_Numeric = 2,
        DT_String = 3,
        DT_DateTime = 4,
        DT_Option = 5,
        DT_MultiOption = 6,
        DT_CSV = 9
    }

    public enum DecisionType
    {
        ApproveDisapprove = 1,
        ConcurNotConcur = 2
    }

    public enum DeployMode
    {
        Development,
        Test,
        Demo,
        Training,
        Production
    }

    public enum Finding
    {
        In_Line_Of_Duty = 1,
        Epts_Lod_Not_Applicable = 2,
        Epts_Service_Aggravated = 3,
        Nlod_Due_To_Own_Misconduct = 4,
        Nlod_Not_Due_To_OwnMisconduct = 5,
        Recommend_Formal_Investigation = 6,
        Request_Consultation = 7,
        Approve = 8,
        Disapprove = 9,
        Qualify_RTD = 10,
        Disqualify = 11,
        Admin_LOD = 12,
        Admin_Qualified = 13,
        Admin_Disqualified = 14,
        PSCD_Applicable = 15,
        PSCD_Not_Applicable = 16
    }

    public enum FormatRestriction
    {
        Numeric,
        Alpha,
        AlphaNumeric,
        Money,
        UIC
    }

    public enum FromLocation
    {
        MTF = 1,
        RMU = 2,
        GMU = 3,
        DeployedLocation = 4
    }

    public enum InfoSource
    {
        Member = 1,
        CivilianPolice = 2,
        MilitaryPolice = 3,
        OSI = 4,
        Witness = 5,
        Other = 6
    }

    public enum JoinType
    {
        None,
        Inner,
        left,
        Outer,
        Full
    }

    public enum LODTemplate : int
    {
        LODAwaitingAction = 1,
        LODComplete = 2,
        UserRegistered = 6,
        AccountChangeRequest = 7,
        LODCancelled = 10,
        AccountModified = 24
    }

    public enum MemberComponent
    {
        AFR = 1,
        RegAF = 2,
        ANG = 3,
        USAFACadet = 4,
        AFROTCCadet = 5
    }

    public enum MemberInfluence
    {
        Alcohol = 1,
        Drugs = 2,
        AlcoholDrugs = 3
    }

    public enum NILODSubFindings
    {
        AbsentwithoutAuthority = 1,
        EPTS_NSA = 2
    }

    public enum Occurrence
    {
        Present = 1,
        AbsentWithAuthority = 2,
        AbsentWithoutAuthority = 3,
        InactiveDutyTraining = 4,
        DutyOrTraining = 5
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
        PrevStatusWasNot = 18
    }

    public enum PhyCancelReason
    {
        Duplicate_LOD = 1,
        LOD_In_Error = 2,
        Annotation_Made = 3,
        Other = 4
    }

    public enum ProximateCause
    {
        Misconduct = 1,
        RequiredDuties = 2,
        OffDuty = 3,
        SponseredEvent = 4,
        PTtest = 5,
        VehicleAccident = 6,
        HighRiskActivity = 7,
        CombatRelated = 8,
        AircraftPassenger = 9,
        SexualAssault = 10,
        InfectiousDisease = 11,
        HazardousExposure = 12,
        FreeText = 13
    }

    public enum RuleKind
    {
        Visibility = 1,
        Validation = 2
    }

    public enum RuleType
    {
        Visibility = 1,
        Validation = 2
    }

    public class DutyStatus
    {
        public const string Active_Duty_Status = "active";
        public const string AFTP = "aftp";
        public const string Other = "other";
        public const string RMP = "rmp";
        public const string Saturday_night_rule = "snr";
        public const string Travel_to_from_duty = "travel";
        public const string Unit_sponsored_event = "use";
        public const string UTA = "uta";
    }

    public class InvestigationDecision
    {
        public const string APPROVE = "APPROVE";
        public const string DISAPPROVE = "DISAPPROVE";
        public const string EPTS_LOD_NOT_APPLICABLE = "EPTS_NOT_LOD";
        public const string EPTS_SERVICE_AGGRAVATE = "EPTS_SERVICE";
        public const string FORMAL_INVESTIGATION = "FORMAL_INV";
        public const string LINE_OF_DUTY = "LINE_OF_DUTY";
        public const string NOT_LOD_MISCONDUCT = "NOT_LOD_MISCONDUCT";
        public const string NOT_LOD_NOT_MISCONDUCT = "NOT_LOD_NOT_MISCONDUCT";
    }
}