using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using ALOD.Core.Domain.Modules.Appeals;
using ALOD.Core.Domain.Modules.Lod;
using ALOD.Core.Domain.Modules.Reinvestigations;
using ALOD.Core.Domain.Modules.SARC;
using ALOD.Core.Domain.Workflow;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Core.Utils;
using ALOD.Data;
using ALOD.Data.Services;
using ALOD.Logging;
using static ALODWebUtility.Common.Utility;

namespace ALOD.Web.LOD
{
    public partial class Secure_lod_Override : Page
    {
        public const string NO_CASE_OR_OVERRIDE_PERMISSION = "Case does not exist or you do not have override permissions";

        private LODAppeal _appeal = null;
        private NHibernateDaoFactory _daoFactory;
        private LineOfDuty _lineOfDuty = null;
        private LODReinvestigation _reinvestigationRequest = null;
        private RestrictedSARC _sarc = null;
        private SARCAppeal _sarcAppeal = null;

        public string CaseId => Server.HtmlEncode(CaseIdbox.Text.Trim());

        protected LODAppeal AP
        {
            get
            {
                if (_appeal == null)
                {
                    _appeal = AppealDao.GetByCaseId(CaseId);
                }
                return _appeal;
            }
            set => _appeal = value;
        }

        protected ILODAppealDAO AppealDao => DaoFactory.GetLODAppealDao;

        protected NHibernateDaoFactory DaoFactory
        {
            get
            {
                if (_daoFactory == null)
                {
                    _daoFactory = new NHibernateDaoFactory();
                }
                return _daoFactory;
            }
        }

        protected ILineOfDutyDao LineOfDutyDao => DaoFactory.GetLineOfDutyDao();

        protected LineOfDuty LOD
        {
            get
            {
                if (_lineOfDuty == null)
                {
                    _lineOfDuty = DaoFactory.GetLodSearchDao.GetByCaseId(CaseId);
                }
                return _lineOfDuty;
            }
            set => _lineOfDuty = value;
        }

        protected ILODReinvestigateDAO ReinvestigationRequestDao => DaoFactory.GetLODReinvestigationDao();

        protected LODReinvestigation RR
        {
            get
            {
                if (_reinvestigationRequest == null)
                {
                    _reinvestigationRequest = ReinvestigationRequestDao.GetByCaseId(CaseId);
                }
                return _reinvestigationRequest;
            }
            set => _reinvestigationRequest = value;
        }

        protected RestrictedSARC SARC
        {
            get
            {
                if (_sarc == null)
                {
                    _sarc = SARCDao.GetByCaseId(CaseId);
                }
                return _sarc;
            }
            set => _sarc = value;
        }

        protected SARCAppeal SARCAppeal
        {
            get
            {
                if (_sarcAppeal == null)
                {
                    _sarcAppeal = SARCAppealDao.GetByCaseId(CaseId);
                }
                return _sarcAppeal;
            }
            set => _sarcAppeal = value;
        }

        protected ISARCAppealDAO SARCAppealDao => DaoFactory.GetSARCAppealDao();

        protected ISARCDAO SARCDao => DaoFactory.GetSARCDao();

        protected ModuleType SelectedModule => (ModuleType)int.Parse(ddlModules.SelectedValue);

        public void ChangeStatus()
        {
            IWorkStatusDao workStatusDao = DaoFactory.GetWorkStatusDao();
            ReminderEmailsDao reminderEmailDao = new ReminderEmailsDao();

            int refId;
            int oldStatusId;
            int newStatusId;

            if (ddlWorkStatus.SelectedValue != "")
            {
                newStatusId = int.Parse(ddlWorkStatus.SelectedValue);

                switch (SelectedModule)
                {
                    case ModuleType.LOD:
                        refId = LOD.Id;
                        oldStatusId = LOD.Status;
                        LOD.PerformOverride(DaoFactory, newStatusId, oldStatusId);
                        LineOfDutyDao.SaveOrUpdate(LOD);
                        break;

                    case ModuleType.ReinvestigationRequest:
                        refId = RR.Id;
                        oldStatusId = RR.Status;
                        RR.PerformOverride(DaoFactory, newStatusId, oldStatusId);
                        ReinvestigationRequestDao.SaveOrUpdate(RR);
                        break;

                    case ModuleType.AppealRequest:
                        refId = AP.Id;
                        oldStatusId = AP.Status;
                        AP.PerformOverride(DaoFactory, newStatusId, oldStatusId);
                        AppealDao.SaveOrUpdate(AP);
                        break;

                    case ModuleType.SARC:
                        refId = SARC.Id;
                        oldStatusId = SARC.Status;
                        SARC.PerformOverride(DaoFactory, newStatusId, oldStatusId);
                        SARCDao.SaveOrUpdate(SARC);
                        break;

                    case ModuleType.SARCAppeal:
                        refId = SARCAppeal.Id;
                        oldStatusId = SARCAppeal.Status;
                        SARCAppeal.PerformOverride(DaoFactory, newStatusId, oldStatusId);
                        SARCAppealDao.SaveOrUpdate(SARCAppeal);
                        break;

                    default:
                        return;
                }

                LogManager.LogAction(SelectedModule, UserAction.Override, refId, "Override Status Action", oldStatusId, newStatusId);
            }
        }

        public void ResetCaseControls()
        {
            errlbl.Text = "";
            Namelbl.Text = "";
            Ranklbl.Text = "";
            Unitlbl.Text = "";
            Statuslbl.Text = "";
            lblCaseId.Text = "";
            pnlCaseInfo.Visible = false;
            trErrors.Visible = false;
        }

        public void Search()
        {
            ResetCaseControls();

            if ((int)SelectedModule == 0)
            {
                errlbl.Text = "Please select a workflow";
                trErrors.Visible = true;
                return;
            }

            if (string.IsNullOrEmpty(CaseId))
            {
                errlbl.Text = "Please enter a caseId";
                trErrors.Visible = true;
                return;
            }

            if (CaseId.Length < GetCaseIdMinimumLengthForModule(SelectedModule))
            {
                errlbl.Text = "Invalid case Id";
                trErrors.Visible = true;
                return;
            }

            switch (SelectedModule)
            {
                case ModuleType.LOD:
                    if (LOD == null)
                    {
                        SetCaseLoadingError();
                        return;
                    }
                    PopulateLODData();
                    break;

                case ModuleType.ReinvestigationRequest:
                    if (RR == null)
                    {
                        SetCaseLoadingError();
                        return;
                    }
                    PopulateRRData();
                    break;

                case ModuleType.AppealRequest:
                    if (AP == null)
                    {
                        SetCaseLoadingError();
                        return;
                    }
                    PopulateAppealData();
                    break;

                case ModuleType.SARC:
                    if (SARC == null)
                    {
                        SetCaseLoadingError();
                        return;
                    }
                    PopulateSARCData();
                    break;

                case ModuleType.SARCAppeal:
                    if (SARCAppeal == null)
                    {
                        SetCaseLoadingError();
                        return;
                    }
                    PopulateSARCAppealData();
                    break;
            }
        }

        protected int GetCaseIdMinimumLengthForModule(ModuleType modType)
        {
            switch (modType)
            {
                case ModuleType.LOD:
                    return 12;
                case ModuleType.ReinvestigationRequest:
                    return 15;
                case ModuleType.SARC:
                    return 15;
                case ModuleType.AppealRequest:
                    return 19;
                case ModuleType.SARCAppeal:
                    return 21;
                default:
                    return 12;
            }
        }

        protected List<ALOD.Core.Domain.Workflow.Module> GetOverrideableModules()
        {
            List<ALOD.Core.Domain.Workflow.Module> modules = DataHelpers.ExtractObjectsFromDataSet<ALOD.Core.Domain.Workflow.Module>(LookupService.GetAllModules());
            return modules.Where(x => x.IsSpecialCase == false && !x.Name.Equals("System")).ToList();
        }

        protected void InitControls()
        {
            PopulateWorklowDDL();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SetInputFormatRestriction(Page, CaseIdbox, FormatRestriction.AlphaNumeric, "-");
                InitControls();
            }
        }

        protected void PopulateAppealData()
        {
            if (AP != null)
            {
                Namelbl.Text = AP.MemberName;
                Ranklbl.Text = AP.MemberRank.Title;
                Unitlbl.Text = AP.MemberUnit;
                lblCaseId.Text = "for CASE ID: " + CaseId;
                Statuslbl.Text = AP.WorkflowStatus.Description;
                PopulateWorkStatusDDL(AP.Workflow);
                ddlWorkStatus.SelectedValue = AP.WorkflowStatus.Id.ToString();
                pnlCaseInfo.Visible = true;
            }
        }

        protected void PopulateLODData()
        {
            if (LOD != null)
            {
                Namelbl.Text = LOD.MemberName;
                Ranklbl.Text = LOD.MemberRank.Title;
                Unitlbl.Text = LOD.MemberUnit;
                lblCaseId.Text = "for CASE ID: " + CaseId;
                Statuslbl.Text = LOD.WorkflowStatus.Description;
                PopulateWorkStatusDDL(LOD.Workflow);
                ddlWorkStatus.SelectedValue = LOD.WorkflowStatus.Id.ToString();
                pnlCaseInfo.Visible = true;
            }
        }

        protected void PopulateRRData()
        {
            if (RR != null)
            {
                Namelbl.Text = RR.MemberName;
                Ranklbl.Text = RR.MemberRank.Title;
                Unitlbl.Text = RR.MemberUnit;
                lblCaseId.Text = "for CASE ID: " + CaseId;
                Statuslbl.Text = RR.WorkflowStatus.Description;
                PopulateWorkStatusDDL(RR.Workflow);
                ddlWorkStatus.SelectedValue = RR.WorkflowStatus.Id.ToString();
                pnlCaseInfo.Visible = true;
            }
        }

        protected void PopulateSARCAppealData()
        {
            if (SARCAppeal != null)
            {
                Namelbl.Text = SARCAppeal.MemberName;
                Ranklbl.Text = SARCAppeal.MemberRank.Title;
                Unitlbl.Text = SARCAppeal.MemberUnit;
                lblCaseId.Text = "for CASE ID: " + CaseId;
                Statuslbl.Text = SARCAppeal.WorkflowStatus.Description;
                PopulateWorkStatusDDL(SARCAppeal.Workflow);
                ddlWorkStatus.SelectedValue = SARCAppeal.WorkflowStatus.Id.ToString();
                pnlCaseInfo.Visible = true;
            }
        }

        protected void PopulateSARCData()
        {
            if (SARC != null)
            {
                Namelbl.Text = SARC.MemberName;
                Ranklbl.Text = SARC.MemberRank.Title;
                Unitlbl.Text = SARC.MemberUnit;
                lblCaseId.Text = "for CASE ID: " + CaseId;
                Statuslbl.Text = SARC.WorkflowStatus.Description;
                PopulateWorkStatusDDL(SARC.Workflow);
                ddlWorkStatus.SelectedValue = SARC.WorkflowStatus.Id.ToString();
                pnlCaseInfo.Visible = true;
            }
        }

        protected void PopulateWorklowDDL()
        {
            ddlModules.DataSource = GetOverrideableModules();
            ddlModules.DataValueField = "Id";
            ddlModules.DataTextField = "Name";
            ddlModules.DataBind();

            foreach (ListItem item in ddlModules.Items)
            {
                if (item.Text.Equals("LOD"))
                {
                    item.Text = "Line of Duty";
                }
            }

            InsertDropDownListZeroValue(ddlModules, "-- Select Workflow --");
        }

        protected void PopulateWorkStatusDDL(int workflowId)
        {
            IWorkStatusDao workStatusDao = DaoFactory.GetWorkStatusDao();

            ddlWorkStatus.DataSource = workStatusDao.GetByWorkflow(workflowId).OrderBy(x => x.Description);
            ddlWorkStatus.DataValueField = "Id";
            ddlWorkStatus.DataTextField = "Description";
            ddlWorkStatus.DataBind();
        }

        protected void SearchButton_Click(object sender, EventArgs e)
        {
            Search();
        }

        protected void SetCaseLoadingError()
        {
            errlbl.Text = NO_CASE_OR_OVERRIDE_PERMISSION;
            trErrors.Visible = true;
        }

        protected void StatusChangeBtn_Click(object sender, EventArgs e)
        {
            if (SelectedModule == ModuleType.LOD && LOD != null)
            {
                ChangeStatus();
                LOD = null;
                PopulateLODData();
            }
            else if (SelectedModule == ModuleType.ReinvestigationRequest && RR != null)
            {
                ChangeStatus();
                RR = null;
                PopulateRRData();
            }
            else if (SelectedModule == ModuleType.AppealRequest && AP != null)
            {
                ChangeStatus();
                AP = null;
                PopulateAppealData();
            }
            else if (SelectedModule == ModuleType.SARC && SARC != null)
            {
                ChangeStatus();
                SARC = null;
                PopulateSARCData();
            }
            else if (SelectedModule == ModuleType.SARCAppeal && SARCAppeal != null)
            {
                ChangeStatus();
                SARCAppeal = null;
                PopulateSARCAppealData();
            }
        }
    }
}
