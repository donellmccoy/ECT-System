namespace ALOD.Core.Interfaces.DAOInterfaces
{
    using ALOD.Core.Domain.Documents;
    using ALOD.Core.Domain.Lookup;
    using ALOD.Core.Utils;

    /// <summary>
    /// Provides an interface for retrieving DAO objects.
    /// </summary>
    public interface IDaoFactory
    {
        IActionTypesDao GetActionTypesDao();

        IAppealPostProcessingDAO GetAppealPostProcessingDao();

        IApplicationWarmupProcessDao GetApplicationWarmupProcessDao();

        IARCNetDao GetARCNetDao();

        IAssociatedCaseDao GetAssociatedCaseDao();

        ICaseCommentsDao GetCaseCommentsDao();

        ICaseLockDao GetCaseLockDao();

        ICaseTypeDao GetCaseTypeDao();

        ICertificationStampDao GetCertificationStampDao();

        IDocCategoryViewDao GetDocCategoryViewDao();

        IDocumentDao GetDocumentDao();

        IDocumentViewDao GetDocumentViewDao();

        IEmailTemplateDao GetEmailTemplateDao();

        IHyperLinkDao GetHyperLinkDao();

        IHyperLinkTypeDao GetHyperLinkTypeDao();

        IICD9CodeDao GetICD9CodeDao();

        IKeyValDao GetKeyValDao();

        ILineOfDutyAuditDao GetLineOfDutyAuditDao();

        ILineOfDutyDao GetLineOfDutyDao();

        ILineOfDutyFindingsDao GetLineOfDutyFindingsDao();

        ILineOfDutyInvestigationDao GetLineOfDutyInvestigationDao();

        ILineOfDutyMedicalDao GetLineOfDutyMedicalDao();

        ILineOfDutyPostProcessingDao GetLineOfDutyPostProcessingDao();

        ILineOfDutyUnitDao GetLineOfDutyUnitDao();

        ILODAppealDAO GetLODAppealDao();

        ILODCommentsDao GetLODCommentsDao();

        ILODReinvestigateDAO GetLODReinvestigationDao();

        ILodSearchDao GetLodSearchDao();

        ILookupCancelReasonsDao GetLookupCancelReasonsDao();

        ILookupDao GetLookupDao();

        ILookupDispositionDao GetLookupDispositionDao();

        IMemberDao GetMemberDao();

        IMemoDao GetMemoDao();

        IMemoDao2 GetMemoDao2();

        IMemoSignatureDao GetMemoSignatureDao();

        IMessageDao GetMessageDao();

        IWorkflowOptionActionsDao GetOptionActionsDao();

        IWorkflowOptionRulesDao GetOptionRulesDao();

        IPageAccessDao GetPageAccessDao();

        IPALDataDao GetPALDataDao();

        IALODPermissionDao GetPermissionDao();

        IPHQueryDao GetPHQueryDao();

        IPsychologicalHealthDao GetPsychologicalHealthDao();

        IQueryDao GetQueryDao();

        IReminderEmailDao GetReminderEmailDao();

        IReportsDao GetReportsDao();

        IRestrictedSARCOtherICDCodeDAO GetRestrictedSARCOtherICDCodeDao();

        IReturnDao GetReturnDao();

        IRuleTypeDao GetRuleTypeDao();

        IRwoaDao GetRwoaDao();

        IRwoaReasonDao GetRwoaReasonDao();

        ISARCAppealDAO GetSARCAppealDao();

        ISARCAppealPostProcessingDAO GetSARCAppealPostProcessingDao();

        ISARCDAO GetSARCDao();

        ISARCPostProcessingDao GetSARCPostProcessingDao();

        ISignatueMetaDateDao GetSigMetaDataDao();

        ISpecialCaseDAO GetSpecialCaseDAO();

        IStatusCodeDao GetStatusCodeDao();

        ISuicideMethodDao GetSuicideMethodDao();

        ITrackingDao GetTrackingDao();

        IUnitChainEntryDao GetUnitChainEntryDao();

        IUnitChainTypeDao GetUnitChainTypeDao();

        IUnitDao GetUnitDao();

        IUserDao GetUserDao();

        IUserGroupDao GetUserGroupDao();

        IUserGroupLevelDao GetUserGroupLevelDao();

        IUserRoleDao GetUserRoleDao();

        IUtilityDao GetUtilityDao();

        IWorkflowDao GetWorkflowDao();

        IWorkflowOptionDao GetWorkflowOptionDao();

        IWorkStatusDao GetWorkStatusDao();

        IWorkStatusTrackingDao GetWorkStatusTrackingDao();
    }
}