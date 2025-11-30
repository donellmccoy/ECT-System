namespace ALOD.Core.Domain.DBSign
{
    public enum DBSignAction
    {
        Sign,
        Verify
    };

    public enum DBSignResult
    {
        NoSignature,
        SignatureValid,
        SignatureInvalid,
        ConnectionError,
        Unknown
    };

    public enum DBSignSupportedBrowsers
    {
        InternetExplorer = 1,
        Firefox = 2
    };

    public enum DBSignTemplateId
    {
        SignOnly = 1,
        Form348 = 2,
        Form348Medical = 3,
        Form348Unit = 4,
        Form348Findings = 5,
        Form261 = 6,
        WingCC = 7,
        Form348RRAA = 8,
        WingCCRR = 9,
        MMSO_Unit = 10,
        Form348SARC = 11,      // Restricted SARC
        Form348SARCWing = 12,
        Form348SARCFindings = 13,
        Form348AP = 14,
        Form348APFindings = 15,
        Form348APSARC = 16,
        Form348APSARCFindings = 17,
        Form348RR = 18,
        Form348RRFindings = 19,
        Form348PostProcessing = 20,
        Form348AppealPostProcessing = 21,
        Form348SARCPostProcessing = 22,
        Form348SARCAppealPostProcessing = 23
    };
}