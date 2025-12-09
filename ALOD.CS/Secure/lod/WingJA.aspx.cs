using System;
using System.Collections.Generic;
using System.Web.UI;
using ALOD.Core.Domain.Common;
using ALOD.Core.Domain.Lookup;
using ALOD.Core.Domain.Modules.Lod;
using ALOD.Core.Domain.Users;
using ALOD.Core.Domain.Workflow;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Core.Utils;
using ALOD.Data;
using ALOD.Web.UserControls;
using static ALODWebUtility.Common.Utility;

namespace ALOD.Web.LOD
{
    public partial class Secure_lod_WingJA : Page
    {
        protected const string optionalFindings = InvestigationDecision.APPROVE + "," + InvestigationDecision.DISAPPROVE;
        protected const string optionalInformalFindings = InvestigationDecision.APPROVE + "," + InvestigationDecision.DISAPPROVE + "," + InvestigationDecision.NOT_LOD_MISCONDUCT;

        private LineOfDuty _lod;
        private ILineOfDutyDao _lodDao;
        private Dictionary<string, PageAccessType> _scAccess;

        public TabNavigator Navigator => MasterPage.Navigator;

        public TabControls TabControl => ((LodMaster)Master).TabControl;

        public bool UserCanEdit
        {
            get
            {
                if (ViewState["UserCanEdit"] == null)
                {
                    ViewState["UserCanEdit"] = false;
                }
                return (bool)ViewState["UserCanEdit"];
            }
            set => ViewState["UserCanEdit"] = value;
        }

        protected LineOfDuty lod
        {
            get
            {
                if (_lod == null)
                {
                    _lod = LodDao.GetById(RefId);
                }
                return _lod;
            }
        }

        protected LodMaster MasterPage => (LodMaster)Page.Master;

        protected Dictionary<string, PageAccessType> SectionList
        {
            get
            {
                if (_scAccess == null)
                {
                    _scAccess = lod.ReadSectionList((int)Session["GroupId"]);
                }
                return _scAccess;
            }
        }

        private ILineOfDutyDao LodDao
        {
            get
            {
                if (_lodDao == null)
                {
                    _lodDao = new NHibernateDaoFactory().GetLineOfDutyDao();
                }
                return _lodDao;
            }
        }

        private int RefId => int.Parse(Request.QueryString["refId"]);

        protected void Page_Init(object sender, EventArgs e)
        {
            ((LodMaster)Master).TabClick += TabButtonClicked;
            FormalFindings_v2.DoDecisionAutoPostBack = true;
            FormalFindings_v2.DeselectFinding();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                UserCanEdit = GetAccessLOD(Navigator.PageAccess, true, lod);
                InitControls();
            }
        }

        private void InitControls()
        {
            bool formal = lod.Formal;

            if (lod.Workflow == 1)
            {
                OriginalLOD.Visible = true;
                FormalPanel.Visible = formal;
                LoadFindings(formal);
            }
            else
            {
                LOD_v2.Visible = true;
                if (SESSION_WS_ID(RefId) == 213)
                {
                    FormalPanel_v2.Visible = true;
                    FormalPanel_v2.Enabled = true;
                    FormalFindings_v2.Visible = true;
                }
                else
                {
                    FormalPanel_v2.Visible = formal;
                }
                LoadFindings_v2(formal);
            }
        }

        #region LOD

        protected void LoadFindings(bool formal)
        {
            LineOfDutyFindings userFinding;
            userFinding = lod.FindByType(PersonnelTypes.WING_JA);

            // Load the formal findings
            FormalFindings.LoadFindingOptionsByWorkflow(lod.Workflow, UserGroups.WingJudgeAdvocate);

            userFinding = lod.FindByType(PersonnelTypes.FORMAL_WING_JA);

            if (userFinding != null)
            {
                // Set the wing JA findings
                if (userFinding.DecisionYN != string.Empty)
                {
                    FormalFindings.Decision = userFinding.DecisionYN;
                }

                if (userFinding.Finding != null)
                {
                    FormalFindings.Findings = userFinding.Finding.Value;
                }

                FormalFindings.Remarks = userFinding.Explanation;
            }

            FormalFindings.PrevFindingsLableText = "IO Findings:";
            FormalFindings.PrevFindingsText = "Not found";
            LineOfDutyFindings ioFinding = lod.FindByType(PersonnelTypes.IO);
            if (ioFinding != null)
            {
                if (ioFinding.Finding != null)
                {
                    FormalFindings.PrevFindingsText = ioFinding.Description;
                }
            }

            if (lod.CurrentStatusCode == LodStatusCode.FormalActionByWingJA && UserCanEdit)
            {
                FormalFindings.SetReadOnly = false;
                SigCheckFormal.Visible = false;
            }
            else
            {
                FormalFindings.SetReadOnly = true;
                SigCheckFormal.VerifySignature(RefId, PersonnelTypes.FORMAL_WING_JA);
            }
        }

        private void SaveFindings()
        {
            if (!UserCanEdit)
            {
                return;
            }

            LineOfDutyFindings finding;
            finding = CreateFinding(RefId);

            PageAccessType pageAccess;
            pageAccess = SectionList[SectionNames.WING_JA_REV.ToString()];

            if (lod.CurrentStatusCode == LodStatusCode.FormalActionByWingJA)
            {
                finding.PType = PersonnelTypes.FORMAL_WING_JA;
                finding.DecisionYN = FormalFindings.Decision;
                if (FormalFindings.Decision != "Y" && FormalFindings.Findings != 0)
                {
                    finding.Finding = FormalFindings.Findings;
                }
                finding.Explanation = FormalFindings.Remarks;
                lod.SetFindingByType(finding);
            }
        }

        #endregion

        #region LOD_v2

        protected void LoadFindings_v2(bool formal)
        {
            LineOfDutyFindings userFinding;
            userFinding = lod.FindByType(PersonnelTypes.WING_JA);

            // Load the informal findings
            InformalFindings_v2.LoadFindingOptionsByWorkflow(lod.Workflow, UserGroups.WingJudgeAdvocate, optionalInformalFindings, false);

            if (userFinding != null)
            {
                // Set the wing JA findings
                if (userFinding.DecisionYN != string.Empty)
                {
                    InformalFindings_v2.Decision = userFinding.DecisionYN;
                }

                if (userFinding.Finding != null)
                {
                    InformalFindings_v2.Findings = userFinding.Finding.Value;
                }

                InformalFindings_v2.Remarks = userFinding.Explanation;
            }

            InformalFindings_v2.PrevFindingsLableText = "Unit Commander Findings:";
            InformalFindings_v2.PrevFindingsText = "Not found";
            LineOfDutyFindings cmdrFinding = lod.FindByType(PersonnelTypes.UNIT_CMDR);
            if (cmdrFinding != null)
            {
                if (cmdrFinding.Finding != null)
                {
                    InformalFindings_v2.PrevFindingsText = cmdrFinding.Description;
                }
            }

            if (lod.Status == LodWorkStatus_v3.WingJA_LODV3 && UserCanEdit)
            {
                InformalFindings_v2.SetReadOnly = false;
                SigCheckInformal_v2.Visible = false;
            }
            else
            {
                InformalFindings_v2.SetReadOnly = true;
                SigCheckInformal_v2.VerifySignature(RefId, PersonnelTypes.WING_JA);
            }

            // Load the formal findings
            try
            {
                FormalFindings_v2.LoadFindingOptionsByWorkflow(lod.Workflow, UserGroups.WingJudgeAdvocate, optionalFindings, false);
                userFinding = lod.FindByType(PersonnelTypes.FORMAL_WING_JA);

                if (userFinding != null)
                {
                    // Set the wing JA findings
                    if (userFinding.DecisionYN != string.Empty)
                    {
                        FormalFindings_v2.Decision = userFinding.DecisionYN;
                    }

                    if (userFinding.Finding != null)
                    {
                        FormalFindings_v2.Findings = userFinding.Finding.Value;
                    }

                    FormalFindings_v2.Remarks = userFinding.Explanation;
                }

                FormalFindings_v2.PrevFindingsLableText = "IO Findings:";
                FormalFindings_v2.PrevFindingsText = "Not found";
                LineOfDutyFindings ioFinding = lod.FindByType(PersonnelTypes.IO);
                if (ioFinding != null)
                {
                    if (ioFinding.Finding != null)
                    {
                        foreach (FindingsLookUp f in new LookupDao().GetWorkflowFindings(lod.Workflow, UserGroups.InvestigatingOfficer))
                        {
                            if (f.Id == ioFinding.Finding.Value)
                            {
                                FormalFindings_v2.PrevFindingsText = f.Description;
                            }
                        }
                    }
                }

                if (lod.Status >= LodWorkStatus_v2.FormalActionByWingJA &&
                    lod.Status < LodWorkStatus_v3.MedicalTechReview_LODV3 && UserCanEdit)
                {
                    FormalFindings_v2.SetReadOnly = false;
                    SigCheckFormal_v2.Visible = false;
                }
                else
                {
                    FormalFindings_v2.SetReadOnly = true;
                    SigCheckFormal_v2.VerifySignature(RefId, PersonnelTypes.FORMAL_WING_JA);
                }

                // Disable Findings if "Concur with the action of Unit Commander" is selected
                if (FormalFindings_v2.Decision == "Y")
                {
                    FormalFindings_v2.FindingsEnabled = false;
                }
            }
            catch
            {
                // Swallow exception
            }
        }

        private void SaveFindings_v2()
        {
            if (!UserCanEdit)
            {
                return;
            }

            LineOfDutyFindings finding;
            finding = CreateFinding(RefId);

            PageAccessType pageAccess;
            pageAccess = SectionList[SectionNames.WING_JA_REV.ToString()];

            if (lod.Status == LodWorkStatus_v3.WingJA_LODV3)
            {
                try
                {
                    finding.PType = PersonnelTypes.WING_JA;
                    finding.DecisionYN = InformalFindings_v2.Decision;
                    if (InformalFindings_v2.Decision != "Y" && InformalFindings_v2.Findings != 0)
                    {
                        finding.Finding = InformalFindings_v2.Findings;
                    }
                    finding.Explanation = InformalFindings_v2.Remarks;
                    lod.SetFindingByType(finding);
                }
                catch
                {
                    // Swallow exception
                }
            }
            else if (lod.Status == LodWorkStatus_v2.FormalActionByWingJA)
            {
                try
                {
                    finding.PType = PersonnelTypes.FORMAL_WING_JA;
                    finding.DecisionYN = FormalFindings_v2.Decision;
                    if (FormalFindings_v2.Decision != "Y" && FormalFindings_v2.Findings != 0)
                    {
                        finding.Finding = FormalFindings_v2.Findings;
                    }
                    finding.Explanation = FormalFindings_v2.Remarks;
                    lod.SetFindingByType(finding);
                }
                catch
                {
                    // Swallow exception
                }
            }
        }

        #endregion

        #region TabEvent

        protected void Save_Click(ref object sender, ref TabNavigationEventArgs e)
        {
            if (Navigator.CurrentStep.IsReadOnly)
            {
                return;
            }

            if (e.ButtonType == NavigatorButtonType.Save || e.ButtonType == NavigatorButtonType.NavigatedAway ||
                e.ButtonType == NavigatorButtonType.NextStep || e.ButtonType == NavigatorButtonType.PreviousStep)
            {
                if (lod.Workflow == 1)
                {
                    SaveFindings();
                }
                else
                {
                    SaveFindings_v2();
                }
            }
        }

        private void TabButtonClicked(ref object sender, ref TabNavigationEventArgs e)
        {
            if (e.ButtonType == NavigatorButtonType.Save ||
                e.ButtonType == NavigatorButtonType.NavigatedAway ||
                e.ButtonType == NavigatorButtonType.NextStep ||
                e.ButtonType == NavigatorButtonType.PreviousStep)
            {
                if (lod.Workflow == 1)
                {
                    SaveFindings();
                }
                else
                {
                    SaveFindings_v2();
                }
            }
        }

        #endregion
    }
}
