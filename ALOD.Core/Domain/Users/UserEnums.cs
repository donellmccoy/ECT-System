namespace ALOD.Core.Domain.Users
{
    public enum AccessScope
    {
        Unit = 1,
        Compo = 2,
    };

    public enum AccessStatus
    {
        None = 1,
        Pending = 2,
        Approved = 3,
        Disabled = 4,
    };

    public enum UserGroups
    {
        SystemAdministrator = 1,
        UnitCommander = 2,
        MedicalTechnician = 3,
        MedicalOfficer = 4,
        WingCommander = 5,
        WingJudgeAdvocate = 6,
        BoardTechnician = 7, //formaley Board Administrator
        BoardLegal = 8,
        BoardMedical = 9,

        //BoardSeniorReviewer = 10,
        BoardApprovalAuthority = 11,

        WingAdmin = 12,
        MPF = 13, //not sure what MPF is so I made INCAP_PM enum as well
        INCAP_PM = 13,
        InvestigatingOfficer = 14,
        WingSarc = 25,
        NafAdmin = 38,
        MedTechSARC = 79,
        MedOffSARC = 80,
        MPF_ReadOnly = 83,
        FMTech = 83,
        BoardApprovalAuthority_ReadOnly = 85,
        WingJudgeAdvocate_ReadOnly = 86,
        auditor_ReadOnly = 87,
        AFRCHQTechnician = 88,
        MMSOContractRepresentative = 89,
        MMSONurse = 90,
        MMSOServicePOS = 91,
        SeniorMedicalReviewer = 92,
        RMU = 93,
        MedicalTechnician_ReadOnly = 94,
        LOD_MFP = 95,
        LOD_PM = 96,
        BoardAdministrator = 97, // formaley Board A1
        UnitPH = 98,
        HQAFRCDPH = 99,
        AppellateAuthority = 100,
        UnitCommander_ReadOnly = 101,
        RSL = 102,
        SARCAdmin = 103,

        ANGMedicalTechnician = 110, // formally 109
        MedTech_Pilot = 118,
        MedOfficer_Pilot = 119,
        UnitCC_LODV3 = 120,
        WingJA_Pilot = 121,
        AppointingAuthority_Pilot = 122,
        HumanResoures_OPR = 124,
        HumanResoures_OCR = 125,
        AFR_DirectorOfStaff = 126,
        AFR_CommandChief = 127,
        AFR_ViceCommander = 128,
        AFR_DirectorOfPersonnel = 129,
        ChiefAirForceReserve = 130
    };
}