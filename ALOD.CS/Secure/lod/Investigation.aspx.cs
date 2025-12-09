using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ALOD.Core.Domain.Common;
using ALOD.Core.Domain.Lookup;
using ALOD.Core.Domain.Modules.Lod;
using ALOD.Core.Domain.Users;
using ALOD.Core.Domain.Workflow;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Core.Utils;
using ALOD.Data;
using ALOD.Data.Services;
using ALOD.Logging;
using ALOD.Web.UserControls;
using static ALODWebUtility.Common.Utility;

namespace ALOD.Web.LOD
{
    public partial class Secure_lod_Investigation : Page
    {
        protected LineOfDuty _lod;

        public TabNavigator Navigator => ((LodMaster)Page.Master).Navigator;

        public int refId => int.Parse(Request.QueryString["refId"]);

        public TabControls TabControl => ((LodMaster)Page.Master).TabControl;

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

        protected string CalendarImage => ResolveClientUrl("~/App_Themes/" + Page.Theme + "/Images/Calendar.gif");

        protected LineOfDuty lod
        {
            get
            {
                if (_lod == null)
                {
                    _lod = LodService.GetById((int)Request.QueryString["refId"]);
                }
                return _lod;
            }
        }

        public void LoadFindings(LineOfDutyInvestigation investigation)
        {
            LineOfDutyFindings cFinding;

            // IO Findings
            cFinding = lod.FindByType(PersonnelTypes.IO);
            investigation.IOFinding = cFinding;

            rblFindings.DataSource = GetIOFindings();
            rblFindings.DataValueField = "Id";
            rblFindings.DataTextField = "Description";
            rblFindings.DataBind();

            if (cFinding != null)
            {
                if (cFinding.Finding != null)
                {
                    rblFindings.SelectedValue = cFinding.Finding.ToString();
                }
            }

            if (!UserCanEdit)
            {
                rblFindings.Visible = false;
                if (cFinding != null)
                {
                    if (cFinding.Finding.HasValue)
                    {
                        lblFindings.Text = cFinding.Description;
                    }
                }
            }

            // Save Name
            if (investigation.IoUserId.HasValue)
            {
                AppUser ioUser = UserService.GetById(investigation.IoUserId);
                lblIOName.Text = ioUser.FullName;
                lblIOGrade.Text = ioUser.Rank.Grade;
                lblIOUnit.Text = ioUser.CurrentUnit.Name + "   (" + ioUser.CurrentUnit.PasCode + ")";
            }
        }

        public void SaveFindings()
        {
            LineOfDutyFindings cFinding;
            // Board
            cFinding = CreateFinding(lod.Id);
            cFinding.PType = PersonnelTypes.IO;
            if (rblFindings.SelectedValue != "")
            {
                cFinding.Finding = byte.Parse(rblFindings.SelectedValue);
            }
            lod.SetFindingByType(cFinding);
        }

        protected void DisplayReadWrite(LineOfDutyInvestigation investigation)
        {
            LoadFindings(investigation);
            if (investigation.ReportDate.HasValue)
            {
                txtDateReport.Text = Server.HtmlDecode(investigation.ReportDate.Value.ToString(DATE_FORMAT));
            }

            SetRadioList(rblInvestigationOf, investigation.InvestigationOf);

            // Member Status
            if (investigation.Status.HasValue)
            {
                int status = investigation.Status.Value;

                switch (status)
                {
                    case 1:
                        rbRegularOrEad.Checked = true;
                        break;
                    case 2:
                        rbAdMore.Checked = true;
                        break;
                    case 3:
                        rbAdLess.Checked = true;
                        break;
                    case 4:
                        rbInactive.Checked = true;
                        break;
                    case 5:
                        rbShortTour.Checked = true;
                        break;
                }

                txtInactiveDutyTraining.Text = Server.HtmlDecode(investigation.InactiveDutyTraining);

                if (status == 4 || status == 5)
                {
                    if (investigation.DurationStart.HasValue)
                    {
                        txtDateStart.Text = Server.HtmlDecode(investigation.DurationStart.Value.ToString(DATE_FORMAT));
                        txtHrStart.Text = Server.HtmlDecode(investigation.DurationStart.Value.ToString(HOUR_FORMAT));
                    }

                    if (investigation.DurationEnd.HasValue)
                    {
                        txtDateFinish.Text = Server.HtmlDecode(investigation.DurationEnd.Value.ToString(DATE_FORMAT));
                        txtHrFinish.Text = Server.HtmlDecode(investigation.DurationEnd.Value.ToString(HOUR_FORMAT));
                    }
                }
            }

            // Other Personnel
            if (investigation.OtherPersonnel != null)
            {
                if (investigation.OtherPersonnel.Count >= 1)
                {
                    PersonnelData per = investigation.OtherPersonnel[0];
                    txtOtherName1.Text = Server.HtmlDecode(per.Name);
                    SetDropdownByValue(ddlGrade1, per.Grade);
                    chkOtherInvestMade1.Checked = per.InvestigationMade;
                }

                if (investigation.OtherPersonnel.Count >= 2)
                {
                    PersonnelData per = investigation.OtherPersonnel[1];
                    txtOtherName2.Text = Server.HtmlDecode(per.Name);
                    SetDropdownByValue(ddlGrade2, per.Grade);
                    chkOtherInvestMade2.Checked = per.InvestigationMade;
                }

                if (investigation.OtherPersonnel.Count >= 3)
                {
                    PersonnelData per = investigation.OtherPersonnel[2];
                    txtOtherName3.Text = Server.HtmlDecode(per.Name);
                    SetDropdownByValue(ddlGrade3, per.Grade);
                    chkOtherInvestMade3.Checked = per.InvestigationMade;
                }
            }

            // Basis for findings
            if (investigation.FindingsDate.HasValue)
            {
                txtDateCircumstance.Text = Server.HtmlDecode(investigation.FindingsDate.Value.ToString(DATE_FORMAT));
                txtHrCircumstance.Text = Server.HtmlDecode(investigation.FindingsDate.Value.ToString(HOUR_FORMAT));
            }

            txtCircumstancePlace.Text = Server.HtmlDecode(investigation.Place);
            txtCircumstanceSustained.Text = Server.HtmlDecode(investigation.HowSustained);
            txtDiagnosis.Text = Server.HtmlDecode(investigation.MedicalDiagnosis);
            SetRadioList(rblPresentForDuty, investigation.PresentForDuty);
            SetRadioList(rblAbsentWithAuthority, investigation.AbsentWithAuthority);
            SetRadioList(rblIntentionalMisconduct, investigation.IntentionalMisconduct);
            SetRadioList(rblMentallySound, investigation.MentallySound);
            txtRemarks.Text = Server.HtmlDecode(investigation.Remarks);
        }

        protected void InitControls()
        {
            if (UserCanEdit)
            {
                txtDateReport.CssClass = "datePicker";
                txtDateStart.CssClass = "datePicker";
                txtDateFinish.CssClass = "datePickerAll";
                txtDateCircumstance.CssClass = "datePicker";
            }

            TabControl.Item(NavigatorButtonType.Save).Enabled = UserCanEdit;

            SetMaxLength(txtCircumstanceSustained);
            SetMaxLength(txtDiagnosis);
            SetMaxLength(txtRemarks);

            rbAdLess.Attributes.Add("onclick", "toggleDutyDates(false);");
            rbAdMore.Attributes.Add("onclick", "toggleDutyDates(false);");
            rbRegularOrEad.Attributes.Add("onclick", "toggleDutyDates(false);");
            rbInactive.Attributes.Add("onclick", "toggleDutyDates(true);");
            rbShortTour.Attributes.Add("onclick", "toggleDutyDates(true);");
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            ((LodMaster)Page.Master).TabClick += TabButtonClicked;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SigCheck.VerifySignature(refId);

                UserCanEdit = GetAccessLOD(Navigator.PageAccess, true, lod);

                SetInputFormatRestriction(Page, txtDateReport, FormatRestriction.Numeric, "/");
                SetInputFormatRestriction(Page, txtInactiveDutyTraining, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT);
                SetInputFormatRestriction(Page, txtDateStart, FormatRestriction.Numeric, "/");
                SetInputFormatRestriction(Page, txtDateFinish, FormatRestriction.Numeric, "/");
                SetInputFormatRestriction(Page, txtHrStart, FormatRestriction.Numeric);
                SetInputFormatRestriction(Page, txtHrFinish, FormatRestriction.Numeric);
                SetInputFormatRestriction(Page, txtOtherName1, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT);
                SetInputFormatRestriction(Page, txtOtherName2, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT);
                SetInputFormatRestriction(Page, txtOtherName3, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT);
                SetInputFormatRestriction(Page, txtDateCircumstance, FormatRestriction.Numeric, "/");
                SetInputFormatRestriction(Page, txtHrCircumstance, FormatRestriction.Numeric);
                SetInputFormatRestriction(Page, txtCircumstancePlace, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT);
                SetInputFormatRestriction(Page, txtCircumstanceSustained, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT);
                SetInputFormatRestriction(Page, txtDiagnosis, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT);
                SetInputFormatRestriction(Page, txtRemarks, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT);

                LoadData();

                if (UserCanEdit)
                {
                    InitControls();
                    page_readOnly.Value = "";
                    rblPresentForDuty.Attributes.Add("onclick", "CheckPresentForDuty();");
                    rbShortTour.Attributes.Add("onclick", "CheckAll();");
                    rbInactive.Attributes.Add("onclick", "CheckAll();");
                    rbAdLess.Attributes.Add("onclick", "CheckAll();");
                    rbAdMore.Attributes.Add("onclick", "CheckAll();");
                    rbRegularOrEad.Attributes.Add("onclick", "CheckAll();");
                }
                else
                {
                    page_readOnly.Value = "Y";
                }

                LogManager.LogAction(ModuleType.LOD, UserAction.ViewPage, refId, "Viewed Page: Investigation");
            }
        }

        protected void Save_Click(ref object sender, ref TabNavigationEventArgs e)
        {
            SaveData();
        }

        private bool CheckTime(TextBox val)
        {
            if (val.Text.Trim().Length == 4 && val.Text.All(char.IsDigit) &&
                int.Parse(val.Text.Substring(0, 2)) < 24 && int.Parse(val.Text.Substring(2, 2)) < 60)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void DisplayReadOnly(LineOfDutyInvestigation investigation)
        {
            txtDateReport.Visible = false;
            rblInvestigationOf.Visible = false;
            rbRegularOrEad.Visible = false;
            rbAdLess.Visible = false;
            rbAdMore.Visible = false;
            rbInactive.Visible = false;
            rbShortTour.Visible = false;

            txtDateStart.Visible = false;
            txtDateFinish.Visible = false;
            txtHrStart.Visible = false;
            txtHrFinish.Visible = false;
            txtInactiveDutyTraining.Visible = false;

            txtOtherName1.Visible = false;
            txtOtherName2.Visible = false;
            txtOtherName3.Visible = false;
            ddlGrade1.Visible = false;
            ddlGrade2.Visible = false;
            ddlGrade3.Visible = false;
            chkOtherInvestMade1.Visible = false;
            chkOtherInvestMade2.Visible = false;
            chkOtherInvestMade3.Visible = false;

            txtDateCircumstance.Visible = false;
            txtHrCircumstance.Visible = false;

            txtCircumstancePlace.Visible = false;
            txtCircumstanceSustained.Visible = false;
            txtDiagnosis.Visible = false;
            rblPresentForDuty.Visible = false;
            rblAbsentWithAuthority.Visible = false;
            rblIntentionalMisconduct.Visible = false;
            rblMentallySound.Visible = false;

            txtRemarks.Visible = false;
            LoadFindings(investigation);
            if (!string.IsNullOrEmpty(investigation.Remarks))
            {
                lblRemarks.Text = investigation.Remarks.Replace("\r\n", "<br />") + "<br /><br /><br />";
            }

            if (investigation.ReportDate.HasValue)
            {
                lblDateReport.Text = investigation.ReportDate.Value.ToString(DATE_FORMAT);
            }

            if (investigation.InvestigationOf.HasValue)
            {
                lblInvestigationOf.Text = investigation.InvestigationDescription;
            }

            if (investigation.Status.HasValue)
            {
                switch (investigation.Status)
                {
                    case 1:
                        StatusRegularImage.Visible = true;
                        break;
                    case 2:
                        StatusADImage.Visible = true;
                        AdDurationLabel.Text = "&nbsp;More than 30 days";
                        break;
                    case 3:
                        StatusADImage.Visible = true;
                        AdDurationLabel.Text = "&nbsp;Less than 30 days";
                        break;
                    case 4:
                        StatusInactiveImage.Visible = true;
                        if (!string.IsNullOrEmpty(investigation.InactiveDutyTraining))
                        {
                            lblInactiveDutyTraining.Text = investigation.InactiveDutyTraining;
                        }
                        break;
                    case 5:
                        StatusShortImage.Visible = true;
                        break;
                }

                if (investigation.Status == 4 || investigation.Status == 5)
                {
                    if (investigation.DurationStart.HasValue)
                    {
                        lblDateStart.Text = investigation.DurationStart.Value.ToString(DATE_HOUR_FORMAT);
                    }

                    if (investigation.DurationEnd.HasValue)
                    {
                        lblDateFinish.Text = investigation.DurationEnd.Value.ToString(DATE_HOUR_FORMAT);
                    }
                }
            }

            // Other Personnel
            if (investigation.OtherPersonnel != null)
            {
                if (investigation.OtherPersonnel.Count >= 1)
                {
                    PersonnelData per = investigation.OtherPersonnel[0];
                    lblOtherName1.Text = per.Name;
                    lblOtherGrade1.Text = per.Grade;
                    Invest1Image.Visible = per.InvestigationMade;
                }

                if (investigation.OtherPersonnel.Count >= 2)
                {
                    PersonnelData per = investigation.OtherPersonnel[1];
                    lblOtherName2.Text = per.Name;
                    lblOtherGrade2.Text = per.Grade;
                    Invest2Image.Visible = per.InvestigationMade;
                }

                if (investigation.OtherPersonnel.Count >= 3)
                {
                    PersonnelData per = investigation.OtherPersonnel[2];
                    lblOtherName3.Text = per.Name;
                    lblOtherGrade3.Text = per.Grade;
                    Invest3Image.Visible = per.InvestigationMade;
                }
            }

            // Basis for findings
            if (investigation.FindingsDate.HasValue)
            {
                lblDateCircumstance.Text = investigation.FindingsDate.Value.ToString(DATE_FORMAT);
                lblHourCircumstance.Text = investigation.FindingsDate.Value.ToString(HOUR_FORMAT);
            }

            lblCircumstancePlace.Text = investigation.Place;

            if (!string.IsNullOrEmpty(investigation.HowSustained))
            {
                lblCircumstanceSustained.Text = investigation.HowSustained.Replace("\r\n", "<br />") + "<br /><br /><br />";
            }

            if (!string.IsNullOrEmpty(investigation.MedicalDiagnosis))
            {
                lblDiagnosis.Text = investigation.MedicalDiagnosis.Replace("\r\n", "<br />") + "<br /><br /><br />";
            }

            if (investigation.PresentForDuty.HasValue)
            {
                lblPresentForDuty.Text = investigation.PresentForDuty.Value ? "Yes" : "No";
            }

            if (investigation.AbsentWithAuthority.HasValue)
            {
                lblAbsentWithAuthority.Text = investigation.AbsentWithAuthority.Value ? "Yes" : "No";
            }

            if (investigation.IntentionalMisconduct.HasValue)
            {
                lblIntentionalMisconduct.Text = investigation.IntentionalMisconduct.Value ? "Yes" : "No";
            }

            if (investigation.MentallySound.HasValue)
            {
                lblMentallySound.Text = investigation.MentallySound.Value ? "Yes" : "No";
            }
        }

        private IEnumerable<FindingsLookUp> GetIOFindings()
        {
            return (from p in new LookupDao().GetWorkflowFindings(lod.Workflow, UserGroups.InvestigatingOfficer)
                    where !p.FindingType.Equals(InvestigationDecision.FORMAL_INVESTIGATION) &&
                          !p.FindingType.Equals(InvestigationDecision.APPROVE) &&
                          !p.FindingType.Equals(InvestigationDecision.DISAPPROVE)
                    select p);
        }

        private void LoadData()
        {
            ILineOfDutyInvestigationDao dao = new NHibernateDaoFactory().GetLineOfDutyInvestigationDao();
            LineOfDutyInvestigation investigation = dao.FindById(refId);

            if (investigation == null)
            {
                investigation = new LineOfDutyInvestigation(refId);
                dao.Save(investigation);
                return;
            }

            if (UserCanEdit)
            {
                DisplayReadWrite(investigation);
            }
            else
            {
                DisplayReadOnly(investigation);
            }
        }

        private void SaveData()
        {
            if (!UserCanEdit)
            {
                return;
            }

            ILineOfDutyInvestigationDao dao = new NHibernateDaoFactory().GetLineOfDutyInvestigationDao();
            LineOfDutyInvestigation investigation = dao.GetById(refId);

            if (investigation == null)
            {
                return;
            }

            if (txtDateReport.Text.Trim().Length > 0)
            {
                try
                {
                    investigation.ReportDate = DateTime.Parse(Server.HtmlEncode(txtDateReport.Text));
                }
                catch (Exception)
                {
                    // Swallow exception
                }
            }

            // Investigation Of
            if (rblInvestigationOf.SelectedValue != "")
            {
                investigation.InvestigationOf = byte.Parse(rblInvestigationOf.SelectedValue);
            }

            // Member Status
            if (rbRegularOrEad.Checked)
            {
                investigation.Status = 1;
                investigation.InactiveDutyTraining = string.Empty;
            }
            else if (rbAdMore.Checked)
            {
                investigation.Status = 2;
                investigation.InactiveDutyTraining = string.Empty;
            }
            else if (rbAdLess.Checked)
            {
                investigation.Status = 3;
                investigation.InactiveDutyTraining = string.Empty;
            }
            else if (rbInactive.Checked)
            {
                investigation.Status = 4;
                investigation.InactiveDutyTraining = Server.HtmlEncode(txtInactiveDutyTraining.Text.Trim());
            }
            else if (rbShortTour.Checked)
            {
                investigation.Status = 5;
                investigation.InactiveDutyTraining = string.Empty;
            }

            if (investigation.Status == 4 || investigation.Status == 5)
            {
                // Start date
                try
                {
                    if (txtDateStart.Text.Trim().Length > 0)
                    {
                        if (txtHrStart.Text.Trim().Length > 0)
                        {
                            investigation.DurationStart = ParseDateAndTime(Server.HtmlEncode(txtDateStart.Text.Trim() + " " + txtHrStart.Text.Trim()));
                        }
                        else
                        {
                            investigation.DurationStart = DateTime.Parse(Server.HtmlEncode(txtDateStart.Text.Trim()));
                        }
                    }
                }
                catch (Exception)
                {
                    investigation.DurationStart = null;
                }

                // End date
                try
                {
                    if (txtDateFinish.Text.Trim().Length > 0)
                    {
                        if (txtHrFinish.Text.Trim().Length > 0)
                        {
                            investigation.DurationEnd = ParseDateAndTime(Server.HtmlEncode(txtDateFinish.Text.Trim() + " " + txtHrFinish.Text.Trim()));
                        }
                        else
                        {
                            investigation.DurationEnd = DateTime.Parse(Server.HtmlEncode(txtDateFinish.Text.Trim()));
                        }
                    }
                }
                catch (Exception)
                {
                    investigation.DurationEnd = null;
                }
            }
            else
            {
                // These don't apply
                investigation.DurationStart = null;
                investigation.DurationEnd = null;
            }

            // Other Personnel
            investigation.OtherPersonnel = new List<PersonnelData>();
            investigation.OtherPersonnel.Clear();

            if (!string.IsNullOrEmpty(txtOtherName1.Text.Trim()))
            {
                PersonnelData per = new PersonnelData();
                per.Name = Server.HtmlEncode(txtOtherName1.Text.Trim());
                per.Grade = ddlGrade1.SelectedValue;
                per.InvestigationMade = chkOtherInvestMade1.Checked;
                investigation.OtherPersonnel.Add(per);
            }

            if (!string.IsNullOrEmpty(txtOtherName2.Text.Trim()))
            {
                PersonnelData per = new PersonnelData();
                per.Name = Server.HtmlEncode(txtOtherName2.Text.Trim());
                per.Grade = ddlGrade2.SelectedValue;
                per.InvestigationMade = chkOtherInvestMade2.Checked;
                investigation.OtherPersonnel.Add(per);
            }

            if (!string.IsNullOrEmpty(txtOtherName3.Text.Trim()))
            {
                PersonnelData per = new PersonnelData();
                per.Name = Server.HtmlEncode(txtOtherName3.Text.Trim());
                per.Grade = ddlGrade3.SelectedValue;
                per.InvestigationMade = chkOtherInvestMade3.Checked;
                investigation.OtherPersonnel.Add(per);
            }

            // Basis for findings - All Dates
            if (txtDateCircumstance.Text.Trim() != "" && CheckDate(txtDateCircumstance))
            {
                if (CheckTime(txtHrCircumstance))
                {
                    investigation.FindingsDate = ParseDateAndTime(Server.HtmlEncode(txtDateCircumstance.Text.Trim() + " " + txtHrCircumstance.Text.Trim()));
                }
                else
                {
                    investigation.FindingsDate = DateTime.Parse(Server.HtmlEncode(txtDateCircumstance.Text.Trim()));
                }
            }
            else
            {
                investigation.FindingsDate = null;
            }

            investigation.Place = Server.HtmlEncode(txtCircumstancePlace.Text.Trim());
            investigation.HowSustained = Server.HtmlEncode(txtCircumstanceSustained.Text.Trim());
            investigation.MedicalDiagnosis = Server.HtmlEncode(txtDiagnosis.Text.Trim());

            if (rblPresentForDuty.SelectedValue.Length > 0)
            {
                investigation.PresentForDuty = bool.Parse(rblPresentForDuty.SelectedValue);
            }

            if (rblPresentForDuty.SelectedValue == "False")
            {
                if (rblAbsentWithAuthority.SelectedValue.Length > 0)
                {
                    investigation.AbsentWithAuthority = bool.Parse(rblAbsentWithAuthority.SelectedValue);
                }
            }
            else
            {
                investigation.AbsentWithAuthority = null;
            }

            if (rblIntentionalMisconduct.SelectedValue.Length > 0)
            {
                investigation.IntentionalMisconduct = bool.Parse(rblIntentionalMisconduct.SelectedValue);
            }

            if (rblMentallySound.SelectedValue.Length > 0)
            {
                investigation.MentallySound = bool.Parse(rblMentallySound.SelectedValue);
            }

            investigation.Remarks = Server.HtmlEncode(txtRemarks.Text.Trim());

            investigation.ModifiedBy = (int)Session["UserId"];
            investigation.ModifiedDate = DateTime.Now;
            SaveFindings();
            dao.SaveOrUpdate(investigation);
        }

        #region Validation

        protected void ShowValidationErrors(IList<ValidationItem> lstValidation)
        {
            string inValidFields;

            foreach (ValidationItem item in lstValidation)
            {
                inValidFields = (string)GetLocalResourceObject(item.Field());
                if (inValidFields != null)
                {
                    string[] strFields = inValidFields.Trim().Split(',');
                    for (int i = 0; i < strFields.Length; i++)
                    {
                        WebControl ctl = FindOnTab(strFields[i], this);
                        HighlightInvalidField(ctl, false);
                    }
                }
            }
        }

        #endregion

        private void TabButtonClicked(ref object sender, ref TabNavigationEventArgs e)
        {
            if (Navigator.CurrentStep.IsReadOnly)
            {
                return;
            }

            if (e.ButtonType == NavigatorButtonType.Save || e.ButtonType == NavigatorButtonType.NavigatedAway ||
                e.ButtonType == NavigatorButtonType.NextStep || e.ButtonType == NavigatorButtonType.PreviousStep)
            {
                SaveData();
            }
        }
    }
}
