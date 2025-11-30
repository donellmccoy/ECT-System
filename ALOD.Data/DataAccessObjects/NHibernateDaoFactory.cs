using ALOD.Core.Domain.Documents;
using ALOD.Core.Domain.Lookup;
using ALOD.Core.Interfaces;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Core.Utils;
using System.Web;

namespace ALOD.Data
{
    /// <summary>
    /// Concrete factory implementation for creating NHibernate-based Data Access Objects (DAOs).
    /// </summary>
    /// <remarks>
    /// This factory follows the Factory pattern to provide centralized DAO instantiation.
    /// All Get*Dao methods create and return new instances of their respective DAO implementations.
    /// This factory is used throughout the application to abstract DAO creation from business logic.
    /// </remarks>
    public class NHibernateDaoFactory : IDaoFactory
    {
        /// <summary>
        /// Creates and returns a new instance of the ActionTypes DAO.
        /// </summary>
        /// <returns>An IActionTypesDao instance for managing action type entities.</returns>
        public IActionTypesDao GetActionTypesDao()
        {
            return new ActionTypesDao();
        }

        /// <summary>
        /// Creates and returns a new instance of the AppealPostProcessing DAO.
        /// </summary>
        /// <returns>An IAppealPostProcessingDAO instance for managing appeal post-processing operations.</returns>
        public IAppealPostProcessingDAO GetAppealPostProcessingDao()
        {
            return new AppealPostProcessingDao();
        }

        /// <summary>
        /// Creates and returns a new instance of the ApplicationWarmupProcess DAO.
        /// </summary>
        /// <returns>An IApplicationWarmupProcessDao instance for managing application warmup process entities.</returns>
        public IApplicationWarmupProcessDao GetApplicationWarmupProcessDao()
        {
            return new ApplicationWarmupProcessDao();
        }

        /// <summary>
        /// Creates and returns a new instance of the ARCNet DAO.
        /// </summary>
        /// <returns>An IARCNetDao instance for managing ARCNet entities.</returns>
        public IARCNetDao GetARCNetDao()
        {
            return new ARCNetDao();
        }

        /// <summary>
        /// Creates and returns a new instance of the AssociatedCase DAO.
        /// </summary>
        /// <returns>An IAssociatedCaseDao instance for managing associated case entities.</returns>
        public IAssociatedCaseDao GetAssociatedCaseDao()
        {
            return new AssociatedCaseDao();
        }

        /// <summary>
        /// Creates and returns a new instance of the CaseComments DAO.
        /// </summary>
        /// <returns>An ICaseCommentsDao instance for managing case comment entities.</returns>
        public ICaseCommentsDao GetCaseCommentsDao()
        {
            return new CaseCommentsDao();
        }

        /// <summary>
        /// Creates and returns a new instance of the CaseLock DAO.
        /// </summary>
        /// <returns>An ICaseLockDao instance for managing case lock entities.</returns>
        public ICaseLockDao GetCaseLockDao()
        {
            return new CaseLockDao();
        }

        /// <summary>
        /// Creates and returns a new instance of the CaseType DAO.
        /// </summary>
        /// <returns>An ICaseTypeDao instance for managing case type entities.</returns>
        public ICaseTypeDao GetCaseTypeDao()
        {
            return new CaseTypeDao();
        }

        /// <summary>
        /// Creates and returns a new instance of the CertificationStamp DAO.
        /// </summary>
        /// <returns>An ICertificationStampDao instance for managing certification stamp entities.</returns>
        public ICertificationStampDao GetCertificationStampDao()
        {
            return new CertificationStampDao();
        }

        /// <summary>
        /// Creates and returns a new instance of the CompletedByGroup DAO.
        /// </summary>
        /// <returns>An ICompletedByGroupDao instance for managing completed-by-group entities.</returns>
        public ICompletedByGroupDao GetCompletedByGroupDao()
        {
            return new CompletedByGroupDao();
        }

        /// <summary>
        /// Creates and returns a new instance of the DocCategoryView DAO.
        /// </summary>
        /// <returns>An IDocCategoryViewDao instance for managing document category view entities.</returns>
        public IDocCategoryViewDao GetDocCategoryViewDao()
        {
            return new DocCategoryViewDao();
        }

        /// <summary>
        /// Creates and returns a new instance of the Document DAO.
        /// </summary>
        /// <returns>An IDocumentDao instance for managing document entities. Uses session username if available.</returns>
        /// <remarks>
        /// This method creates a SRXDocumentStore instance. If a username is available in the current session,
        /// it initializes the store with that username for audit tracking purposes.
        /// </remarks>
        public IDocumentDao GetDocumentDao()
        {
            if (HttpContext.Current.Session["UserName"] != null)
            {
                return new SRXDocumentStore((string)HttpContext.Current.Session["UserName"]);
            }
            else
            {
                return new SRXDocumentStore();
            }
        }

        /// <summary>
        /// Creates and returns a new instance of the DocumentView DAO.
        /// </summary>
        /// <returns>An IDocumentViewDao instance for managing document view entities.</returns>
        public IDocumentViewDao GetDocumentViewDao()
        {
            return new DocumentViewDao();
        }

        /// <summary>
        /// Creates and returns a new instance of the EmailTemplate DAO.
        /// </summary>
        /// <returns>An IEmailTemplateDao instance for managing email template entities.</returns>
        public IEmailTemplateDao GetEmailTemplateDao()
        {
            return new EmailTemplateDao();
        }

        /// <summary>
        /// Creates and returns a new instance of the HyperLink DAO.
        /// </summary>
        /// <returns>An IHyperLinkDao instance for managing hyperlink entities.</returns>
        public IHyperLinkDao GetHyperLinkDao()
        {
            return new HyperLinkDao();
        }

        /// <summary>
        /// Creates and returns a new instance of the HyperLinkType DAO.
        /// </summary>
        /// <returns>An IHyperLinkTypeDao instance for managing hyperlink type entities.</returns>
        public IHyperLinkTypeDao GetHyperLinkTypeDao()
        {
            return new HyperLinkTypeDao();
        }

        /// <summary>
        /// Creates and returns a new instance of the ICD9Code DAO.
        /// </summary>
        /// <returns>An IICD9CodeDao instance for managing ICD-9 medical diagnosis code entities.</returns>
        public IICD9CodeDao GetICD9CodeDao()
        {
            return new ICD9CodeDao();
        }

        /// <summary>
        /// Creates and returns a new instance of the KeyVal DAO.
        /// </summary>
        /// <returns>An IKeyValDao instance for managing key-value pair entities.</returns>
        public IKeyValDao GetKeyValDao()
        {
            return new KeyValDao();
        }

        /// <summary>
        /// Creates and returns a new instance of the LineOfDutyAudit DAO.
        /// </summary>
        /// <returns>An ILineOfDutyAuditDao instance for managing Line of Duty audit trail entities.</returns>
        public ILineOfDutyAuditDao GetLineOfDutyAuditDao()
        {
            return new LineOfDutyAuditDao();
        }

        /// <summary>
        /// Creates and returns a new instance of the LineOfDuty DAO.
        /// </summary>
        /// <returns>An ILineOfDutyDao instance for managing Line of Duty investigation entities.</returns>
        public ILineOfDutyDao GetLineOfDutyDao()
        {
            return new LineOfDutyDao();
        }

        /// <summary>
        /// Creates and returns a new instance of the LineOfDutyFindings DAO.
        /// </summary>
        /// <returns>An ILineOfDutyFindingsDao instance for managing LOD investigation findings entities.</returns>
        public ILineOfDutyFindingsDao GetLineOfDutyFindingsDao()
        {
            return new LineOfDutyFindingsDao();
        }

        /// <summary>
        /// Creates and returns a new instance of the LineOfDutyInvestigation DAO.
        /// </summary>
        /// <returns>An ILineOfDutyInvestigationDao instance for managing LOD investigation detail entities.</returns>
        public ILineOfDutyInvestigationDao GetLineOfDutyInvestigationDao()
        {
            return new LineOfDutyInvestigationDao();
        }

        /// <summary>
        /// Creates and returns a new instance of the LineOfDutyMedical DAO.
        /// </summary>
        /// <returns>An ILineOfDutyMedicalDao instance for managing LOD medical information entities.</returns>
        public ILineOfDutyMedicalDao GetLineOfDutyMedicalDao()
        {
            return new LineOfDutyMedicalDao();
        }

        /// <summary>
        /// Creates and returns a new instance of the LineOfDutyPostProcessing DAO.
        /// </summary>
        /// <returns>An ILineOfDutyPostProcessingDao instance for managing LOD post-processing operations.</returns>
        public ILineOfDutyPostProcessingDao GetLineOfDutyPostProcessingDao()
        {
            return new LineOfDutyPostProcessingDao();
        }

        /// <summary>
        /// Creates and returns a new instance of the LineOfDutyUnit DAO.
        /// </summary>
        /// <returns>An ILineOfDutyUnitDao instance for managing LOD unit information entities.</returns>
        public ILineOfDutyUnitDao GetLineOfDutyUnitDao()
        {
            return new LineOfDutyUnitDao();
        }

        /// <summary>
        /// Creates and returns a new instance of the LODAppeal DAO.
        /// </summary>
        /// <returns>An ILODAppealDAO instance for managing Line of Duty appeal entities.</returns>
        public ILODAppealDAO GetLODAppealDao()
        {
            return new LODAppealDao();
        }

        /// <summary>
        /// Creates and returns a new instance of the LODComments DAO.
        /// </summary>
        /// <returns>An ILODCommentsDao instance for managing LOD comment entities.</returns>
        public ILODCommentsDao GetLODCommentsDao()
        {
            return new LODCommentsDao();
        }

        /// <summary>
        /// Creates and returns a new instance of the LODReinvestigation DAO.
        /// </summary>
        /// <returns>An ILODReinvestigateDAO instance for managing LOD reinvestigation entities.</returns>
        public ILODReinvestigateDAO GetLODReinvestigationDao()
        {
            return new LODReinvestigationDao();
        }

        /// <summary>
        /// Creates and returns a new instance of the LodSearch DAO.
        /// </summary>
        /// <returns>An ILodSearchDao instance for managing LOD search operations.</returns>
        public ILodSearchDao GetLodSearchDao()
        {
            return new LodSearchDao();
        }

        /// <summary>
        /// Creates and returns a new instance of the LookupCancelReasons DAO.
        /// </summary>
        /// <returns>An ILookupCancelReasonsDao instance for managing cancellation reason lookup entities.</returns>
        public ILookupCancelReasonsDao GetLookupCancelReasonsDao()
        {
            return new LookupCancelReasonsDao();
        }

        /// <summary>
        /// Creates and returns a new instance of the Lookup DAO.
        /// </summary>
        /// <returns>An ILookupDao instance for managing general lookup table entities.</returns>
        public ILookupDao GetLookupDao()
        {
            return new LookupDao();
        }

        /// <summary>
        /// Creates and returns a new instance of the LookupDisposition DAO.
        /// </summary>
        /// <returns>An ILookupDispositionDao instance for managing disposition lookup entities.</returns>
        public ILookupDispositionDao GetLookupDispositionDao()
        {
            return new LookupDispositionDao();
        }

        /// <summary>
        /// Creates and returns a new instance of the Member DAO.
        /// </summary>
        /// <returns>An IMemberDao instance for managing service member entities.</returns>
        public IMemberDao GetMemberDao()
        {
            return new MemberDao();
        }

        /// <summary>
        /// Creates and returns a new instance of the Memo DAO.
        /// </summary>
        /// <returns>An IMemoDao instance for managing memo entities.</returns>
        public IMemoDao GetMemoDao()
        {
            return new MemoDao();
        }

        /// <summary>
        /// Creates and returns a new instance of the Memo DAO (version 2).
        /// </summary>
        /// <returns>An IMemoDao2 instance for managing memo entities using an alternate implementation.</returns>
        public IMemoDao2 GetMemoDao2()
        {
            return new MemoDao2();
        }

        /// <summary>
        /// Creates and returns a new instance of the MemoSignature DAO.
        /// </summary>
        /// <returns>An IMemoSignatureDao instance for managing memo signature entities.</returns>
        public IMemoSignatureDao GetMemoSignatureDao()
        {
            return new MemoSignatureDao();
        }

        /// <summary>
        /// Creates and returns a new instance of the Message DAO.
        /// </summary>
        /// <returns>An IMessageDao instance for managing system message entities.</returns>
        public IMessageDao GetMessageDao()
        {
            return new MessageDao();
        }

        /// <summary>
        /// Creates and returns a new instance of the WorkflowOptionActions DAO.
        /// </summary>
        /// <returns>An IWorkflowOptionActionsDao instance for managing workflow option action entities.</returns>
        public IWorkflowOptionActionsDao GetOptionActionsDao()
        {
            return new WorkflowOptionActionsDao();
        }

        /// <summary>
        /// Creates and returns a new instance of the WorkflowOptionRules DAO.
        /// </summary>
        /// <returns>An IWorkflowOptionRulesDao instance for managing workflow option rule entities.</returns>
        public IWorkflowOptionRulesDao GetOptionRulesDao()
        {
            return new WorkflowOptionRulesDao();
        }

        /// <summary>
        /// Creates and returns a new instance of the PageAccess DAO.
        /// </summary>
        /// <returns>An IPageAccessDao instance for managing page access permission entities.</returns>
        public IPageAccessDao GetPageAccessDao()
        {
            return new PageAccessDao();
        }

        /// <summary>
        /// Creates and returns a new instance of the PALData DAO.
        /// </summary>
        /// <returns>An IPALDataDao instance for managing PAL (Personnel Accountability Locator) data entities.</returns>
        public IPALDataDao GetPALDataDao()
        {
            return new PALDataDao();
        }

        /// <summary>
        /// Creates and returns a new instance of the ALODPermission DAO.
        /// </summary>
        /// <returns>An IALODPermissionDao instance for managing application permission entities.</returns>
        public IALODPermissionDao GetPermissionDao()
        {
            return new ALODPermissionDao();
        }

        /// <summary>
        /// Creates and returns a new instance of the PHQuery DAO.
        /// </summary>
        /// <returns>An IPHQueryDao instance for managing psychological health query entities.</returns>
        public IPHQueryDao GetPHQueryDao()
        {
            return new PHQueryDao();
        }

        /// <summary>
        /// Creates and returns a new instance of the PsychologicalHealth DAO.
        /// </summary>
        /// <returns>An IPsychologicalHealthDao instance for managing psychological health assessment entities.</returns>
        public IPsychologicalHealthDao GetPsychologicalHealthDao()
        {
            return new PsychologicalHealthDao();
        }

        /// <summary>
        /// Creates and returns a new instance of the Query DAO.
        /// </summary>
        /// <returns>An IQueryDao instance for managing dynamic query entities and execution.</returns>
        public IQueryDao GetQueryDao()
        {
            return new QueryDao();
        }

        /// <summary>
        /// Creates and returns a new instance of the ReminderEmail DAO.
        /// </summary>
        /// <returns>An IReminderEmailDao instance for managing email reminder entities.</returns>
        public IReminderEmailDao GetReminderEmailDao()
        {
            return new ReminderEmailsDao();
        }

        /// <summary>
        /// Creates and returns a new instance of the Reports DAO.
        /// </summary>
        /// <returns>An IReportsDao instance for managing report generation and execution.</returns>
        public IReportsDao GetReportsDao()
        {
            return new ReportsDao();
        }

        /// <summary>
        /// Creates and returns a new instance of the RestrictedSARCOtherICDCode DAO.
        /// </summary>
        /// <returns>An IRestrictedSARCOtherICDCodeDAO instance for managing restricted SARC ICD code entities.</returns>
        public IRestrictedSARCOtherICDCodeDAO GetRestrictedSARCOtherICDCodeDao()
        {
            return new RestrictedSARCOtherICDCodeDAO();
        }

        /// <summary>
        /// Creates and returns a new instance of the Return DAO.
        /// </summary>
        /// <returns>An IReturnDao instance for managing return workflow entities.</returns>
        public IReturnDao GetReturnDao()
        {
            return new ReturnDao();
        }

        /// <summary>
        /// Creates and returns a new instance of the RuleType DAO.
        /// </summary>
        /// <returns>An IRuleTypeDao instance for managing workflow rule type entities.</returns>
        public IRuleTypeDao GetRuleTypeDao()
        {
            return new RulesTypeDao();
        }

        /// <summary>
        /// Creates and returns a new instance of the RWOA DAO.
        /// </summary>
        /// <returns>An IRwoaDao instance for managing RWOA (Reserve While On Active Duty) entities.</returns>
        public IRwoaDao GetRwoaDao()
        {
            return new RwoaDao();
        }

        /// <summary>
        /// Creates and returns a new instance of the RWOAReason DAO.
        /// </summary>
        /// <returns>An IRwoaReasonDao instance for managing RWOA reason entities.</returns>
        public IRwoaReasonDao GetRwoaReasonDao()
        {
            return new RwoaReasonDao();
        }

        /// <summary>
        /// Creates and returns a new instance of the SARCAppeal DAO.
        /// </summary>
        /// <returns>An ISARCAppealDAO instance for managing SARC (Sexual Assault Response Coordinator) appeal entities.</returns>
        public ISARCAppealDAO GetSARCAppealDao()
        {
            return new SARCAppealDao();
        }

        /// <summary>
        /// Creates and returns a new instance of the SARCAppealPostProcessing DAO.
        /// </summary>
        /// <returns>An ISARCAppealPostProcessingDAO instance for managing SARC appeal post-processing operations.</returns>
        public ISARCAppealPostProcessingDAO GetSARCAppealPostProcessingDao()
        {
            return new SARCAppealPostProcessingDao();
        }

        /// <summary>
        /// Creates and returns a new instance of the SARC DAO.
        /// </summary>
        /// <returns>An ISARCDAO instance for managing restricted SARC case entities.</returns>
        public ISARCDAO GetSARCDao()
        {
            return new RestrictedSARCDao();
        }

        /// <summary>
        /// Creates and returns a new instance of the SARCPostProcessing DAO.
        /// </summary>
        /// <returns>An ISARCPostProcessingDao instance for managing SARC post-processing operations.</returns>
        public ISARCPostProcessingDao GetSARCPostProcessingDao()
        {
            return new RestrictedSARCPostProcessingDao();
        }

        /// <summary>
        /// Creates and returns a new instance of the SignatureMetaData DAO.
        /// </summary>
        /// <returns>An ISignatueMetaDateDao instance for managing signature metadata entities.</returns>
        public ISignatueMetaDateDao GetSigMetaDataDao()
        {
            return new SignatureMetaDataDao();
        }

        /// <summary>
        /// Creates and returns a new instance of the SpecialCase DAO.
        /// </summary>
        /// <returns>An ISpecialCaseDAO instance for managing special case investigation entities.</returns>
        public ISpecialCaseDAO GetSpecialCaseDAO()
        {
            return new SpecialCaseDao();
        }

        /// <summary>
        /// Creates and returns a new instance of the StatusCode DAO.
        /// </summary>
        /// <returns>An IStatusCodeDao instance for managing status code lookup entities.</returns>
        public IStatusCodeDao GetStatusCodeDao()
        {
            return new StatusCodeDao();
        }

        /// <summary>
        /// Creates and returns a new instance of the SuicideMethod DAO.
        /// </summary>
        /// <returns>An ISuicideMethodDao instance for managing suicide method classification entities.</returns>
        public ISuicideMethodDao GetSuicideMethodDao()
        {
            return new SuicideMethodDao();
        }

        /// <summary>
        /// Creates and returns a new instance of the Tracking DAO.
        /// </summary>
        /// <returns>An ITrackingDao instance for managing case tracking entities.</returns>
        public ITrackingDao GetTrackingDao()
        {
            return new TrackingDao();
        }

        /// <summary>
        /// Creates and returns a new instance of the UnitChainEntry DAO.
        /// </summary>
        /// <returns>An IUnitChainEntryDao instance for managing unit chain of command entry entities.</returns>
        public IUnitChainEntryDao GetUnitChainEntryDao()
        {
            return new UnitChainEntryDao();
        }

        /// <summary>
        /// Creates and returns a new instance of the UnitChainType DAO.
        /// </summary>
        /// <returns>An IUnitChainTypeDao instance for managing unit chain type entities.</returns>
        public IUnitChainTypeDao GetUnitChainTypeDao()
        {
            return new UnitChainTypeDao();
        }

        /// <summary>
        /// Creates and returns a new instance of the Unit DAO.
        /// </summary>
        /// <returns>An IUnitDao instance for managing military unit entities.</returns>
        public IUnitDao GetUnitDao()
        {
            return new UnitDao();
        }

        /// <summary>
        /// Creates and returns a new instance of the User DAO.
        /// </summary>
        /// <returns>An IUserDao instance for managing user account entities.</returns>
        public IUserDao GetUserDao()
        {
            return new UserDao();
        }

        /// <summary>
        /// Creates and returns a new instance of the UserGroup DAO.
        /// </summary>
        /// <returns>An IUserGroupDao instance for managing user group entities.</returns>
        public IUserGroupDao GetUserGroupDao()
        {
            return new UserGroupDao();
        }

        /// <summary>
        /// Creates and returns a new instance of the UserGroupLevel DAO.
        /// </summary>
        /// <returns>An IUserGroupLevelDao instance for managing user group level entities.</returns>
        public IUserGroupLevelDao GetUserGroupLevelDao()
        {
            return new UserGroupLevelDao();
        }

        /// <summary>
        /// Creates and returns a new instance of the UserRole DAO.
        /// </summary>
        /// <returns>An IUserRoleDao instance for managing user role assignment entities.</returns>
        public IUserRoleDao GetUserRoleDao()
        {
            return new UserRoleDao();
        }

        /// <summary>
        /// Creates and returns a new instance of the UserRoleRequest DAO.
        /// </summary>
        /// <returns>An IUserRoleRequestDao instance for managing user role request entities.</returns>
        public IUserRoleRequestDao GetUserRoleRequestDao()
        {
            return new UseRoleRequestDao();
        }

        /// <summary>
        /// Creates and returns a new instance of the Utility DAO.
        /// </summary>
        /// <returns>An IUtilityDao instance for managing utility and helper operations.</returns>
        public IUtilityDao GetUtilityDao()
        {
            return new UtilityDao();
        }

        /// <summary>
        /// Creates and returns a new instance of the Workflow DAO.
        /// </summary>
        /// <returns>An IWorkflowDao instance for managing workflow configuration entities.</returns>
        public IWorkflowDao GetWorkflowDao()
        {
            return new WorkflowDao();
        }

        /// <inheritdoc/>
        public IWorkflowOptionDao GetWorkflowOptionDao()
        {
            return new WorkflowOptionsDao();
        }

        /// <inheritdoc/>
        public IWorkStatusDao GetWorkStatusDao()
        {
            return new WorkStatusDao();
        }

        /// <inheritdoc/>
        public IWorkStatusTrackingDao GetWorkStatusTrackingDao()
        {
            return new WorkStatusTrackingDao();
        }
    }
}