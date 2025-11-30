Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.Lookup
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Users
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Core.Utils
Imports ALOD.Data
Imports ALOD.Data.Services
Imports ALOD.Logging
Imports ALOD.Web.UserControls
Imports ALODWebUtility.Common

Namespace Web.LOD

    Partial Class Secure_lod_Investigation
        Inherits System.Web.UI.Page
        Protected _lod As LineOfDuty

        Public ReadOnly Property Navigator() As TabNavigator
            Get
                Return CType(Page.Master, LodMaster).Navigator
            End Get
        End Property

        Public ReadOnly Property refId() As Integer
            Get
                Return Integer.Parse(Request.QueryString("refId"))
            End Get
        End Property

        Public ReadOnly Property TabControl() As TabControls
            Get
                Return CType(Page.Master, LodMaster).TabControl
            End Get
        End Property

        Public Property UserCanEdit() As Boolean
            Get
                If (ViewState("UserCanEdit") Is Nothing) Then
                    ViewState("UserCanEdit") = False
                End If
                Return CBool(ViewState("UserCanEdit"))
            End Get
            Set(value As Boolean)
                ViewState("UserCanEdit") = value
            End Set
        End Property

        Protected ReadOnly Property CalendarImage() As String
            Get
                Return Me.ResolveClientUrl("~/App_Themes/" + Page.Theme + "/Images/Calendar.gif")
            End Get
        End Property

        Protected ReadOnly Property lod() As LineOfDuty
            Get
                If (_lod Is Nothing) Then
                    _lod = LodService.GetById(CType(Request.QueryString("refId"), Integer))
                End If
                Return _lod
            End Get
        End Property

        Public Sub LoadFindings(ByVal investigation As LineOfDutyInvestigation)

            Dim cFinding As LineOfDutyFindings

            'IO Findings
            cFinding = lod.FindByType(PersonnelTypes.IO)
            investigation.IOFinding = cFinding

            rblFindings.DataSource = GetIOFindings()
            rblFindings.DataValueField = "Id"
            rblFindings.DataTextField = "Description"
            rblFindings.DataBind()

            If Not (cFinding Is Nothing) Then
                If Not cFinding.Finding Is Nothing Then
                    rblFindings.SelectedValue = cFinding.Finding
                End If
            End If

            If Not (UserCanEdit) Then

                rblFindings.Visible = False
                If Not (cFinding Is Nothing) Then
                    If (cFinding.Finding.HasValue) Then
                        lblFindings.Text = cFinding.Description
                    End If
                End If
            End If

            'Save Name
            If (investigation.IoUserId.HasValue) Then
                Dim ioUser As ALOD.Core.Domain.Users.AppUser = UserService.GetById(investigation.IoUserId)
                lblIOName.Text = ioUser.FullName
                lblIOGrade.Text = ioUser.Rank.Grade
                lblIOUnit.Text = ioUser.CurrentUnit.Name + "   (" + ioUser.CurrentUnit.PasCode + ")"
            End If

        End Sub

        Public Sub SaveFindings()

            Dim cFinding As LineOfDutyFindings
            'Board
            cFinding = CreateFinding(lod.Id)
            cFinding.PType = PersonnelTypes.IO
            If rblFindings.SelectedValue <> "" Then
                cFinding.Finding = rblFindings.SelectedValue
            End If
            lod.SetFindingByType(cFinding)

        End Sub

        Protected Sub DisplayReadWrite(ByVal investigation As LineOfDutyInvestigation)
            LoadFindings(investigation)
            If (investigation.ReportDate.HasValue) Then
                txtDateReport.Text = Server.HtmlDecode(investigation.ReportDate.Value.ToString(DATE_FORMAT))
            End If

            SetRadioList(rblInvestigationOf, investigation.InvestigationOf)

            '--------------------------
            ' Member Status
            '--------------------------
            If (investigation.Status.HasValue) Then
                Dim Status As Integer = investigation.Status

                Select Case Status
                    Case 1
                        rbRegularOrEad.Checked = True
                    Case 2
                        rbAdMore.Checked = True
                    Case 3
                        rbAdLess.Checked = True
                    Case 4
                        rbInactive.Checked = True
                    Case 5
                        rbShortTour.Checked = True
                End Select

                txtInactiveDutyTraining.Text = Server.HtmlDecode(investigation.InactiveDutyTraining)

                If (Status = 4 OrElse Status = 5) Then
                    If (investigation.DurationStart.HasValue) Then
                        txtDateStart.Text = Server.HtmlDecode(investigation.DurationStart.Value.ToString(DATE_FORMAT))
                        txtHrStart.Text = Server.HtmlDecode(investigation.DurationStart.Value.ToString(HOUR_FORMAT))
                    End If

                    If (investigation.DurationEnd.HasValue) Then
                        txtDateFinish.Text = Server.HtmlDecode(investigation.DurationEnd.Value.ToString(DATE_FORMAT))
                        txtHrFinish.Text = Server.HtmlDecode(investigation.DurationEnd.Value.ToString(HOUR_FORMAT))
                    End If
                End If
            End If
            '--------------------------
            ' Other Personnel
            '--------------------------
            If (investigation.OtherPersonnel IsNot Nothing) Then
                If (investigation.OtherPersonnel.Count >= 1) Then
                    Dim per As PersonnelData = investigation.OtherPersonnel(0)
                    txtOtherName1.Text = Server.HtmlDecode(per.Name)
                    SetDropdownByValue(ddlGrade1, per.Grade)
                    chkOtherInvestMade1.Checked = per.InvestigationMade
                End If

                If (investigation.OtherPersonnel.Count >= 2) Then
                    Dim per As PersonnelData = investigation.OtherPersonnel(1)
                    txtOtherName2.Text = Server.HtmlDecode(per.Name)
                    SetDropdownByValue(ddlGrade2, per.Grade)
                    chkOtherInvestMade2.Checked = per.InvestigationMade
                End If

                If (investigation.OtherPersonnel.Count >= 3) Then
                    Dim per As PersonnelData = investigation.OtherPersonnel(2)
                    txtOtherName3.Text = Server.HtmlDecode(per.Name)
                    SetDropdownByValue(ddlGrade3, per.Grade)
                    chkOtherInvestMade3.Checked = per.InvestigationMade
                End If
            End If
            '--------------------------
            ' Basis for findings
            '--------------------------

            If (investigation.FindingsDate.HasValue) Then
                txtDateCircumstance.Text = Server.HtmlDecode(investigation.FindingsDate.Value.ToString(DATE_FORMAT))
                txtHrCircumstance.Text = Server.HtmlDecode(investigation.FindingsDate.Value.ToString(HOUR_FORMAT))
            End If

            txtCircumstancePlace.Text = Server.HtmlDecode(investigation.Place)
            txtCircumstanceSustained.Text = Server.HtmlDecode(investigation.HowSustained)
            txtDiagnosis.Text = Server.HtmlDecode(investigation.MedicalDiagnosis)
            SetRadioList(rblPresentForDuty, investigation.PresentForDuty)
            SetRadioList(rblAbsentWithAuthority, investigation.AbsentWithAuthority)
            SetRadioList(rblIntentionalMisconduct, investigation.IntentionalMisconduct)
            SetRadioList(rblMentallySound, investigation.MentallySound)
            txtRemarks.Text = Server.HtmlDecode(investigation.Remarks)
            'Before calling the validation on the investigation the io finding should be set

            '--------------------------
            'findings
            '--------------------------

            ' SetRadioList(rblFindings, investigation.Finding)

        End Sub

        Protected Sub InitControls()

            If (UserCanEdit) Then
                txtDateReport.CssClass = "datePicker"
                txtDateStart.CssClass = "datePicker"
                txtDateFinish.CssClass = "datePickerAll"
                txtDateCircumstance.CssClass = "datePicker"
            End If

            TabControl.Item(NavigatorButtonType.Save).Enabled = UserCanEdit

            SetMaxLength(txtCircumstanceSustained)
            SetMaxLength(txtDiagnosis)
            SetMaxLength(txtRemarks)

            rbAdLess.Attributes.Add("onclick", "toggleDutyDates(false);")
            rbAdMore.Attributes.Add("onclick", "toggleDutyDates(false);")
            rbRegularOrEad.Attributes.Add("onclick", "toggleDutyDates(false);")
            rbInactive.Attributes.Add("onclick", "toggleDutyDates(true);")
            rbShortTour.Attributes.Add("onclick", "toggleDutyDates(true);")
        End Sub

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            AddHandler CType(Page.Master, LodMaster).TabClick, AddressOf TabButtonClicked

        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If (Not IsPostBack) Then

                SigCheck.VerifySignature(refId)

                UserCanEdit = GetAccessLOD(Navigator.PageAccess, True, lod)

                SetInputFormatRestriction(Page, txtDateReport, FormatRestriction.Numeric, "/")
                SetInputFormatRestriction(Page, txtInactiveDutyTraining, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
                SetInputFormatRestriction(Page, txtDateStart, FormatRestriction.Numeric, "/")
                SetInputFormatRestriction(Page, txtDateFinish, FormatRestriction.Numeric, "/")
                SetInputFormatRestriction(Page, txtHrStart, FormatRestriction.Numeric)
                SetInputFormatRestriction(Page, txtHrFinish, FormatRestriction.Numeric)
                SetInputFormatRestriction(Page, txtOtherName1, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
                SetInputFormatRestriction(Page, txtOtherName2, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
                SetInputFormatRestriction(Page, txtOtherName3, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
                SetInputFormatRestriction(Page, txtDateCircumstance, FormatRestriction.Numeric, "/")
                SetInputFormatRestriction(Page, txtHrCircumstance, FormatRestriction.Numeric)
                SetInputFormatRestriction(Page, txtCircumstancePlace, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
                SetInputFormatRestriction(Page, txtCircumstanceSustained, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
                SetInputFormatRestriction(Page, txtDiagnosis, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
                SetInputFormatRestriction(Page, txtRemarks, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)

                LoadData()

                If (UserCanEdit) Then
                    InitControls()
                    page_readOnly.Value = ""
                    rblPresentForDuty.Attributes.Add("onclick", "CheckPresentForDuty();")
                    rbShortTour.Attributes.Add("onclick", "CheckAll();")
                    rbInactive.Attributes.Add("onclick", "CheckAll();")
                    rbAdLess.Attributes.Add("onclick", "CheckAll();")
                    rbAdMore.Attributes.Add("onclick", "CheckAll();")
                    rbRegularOrEad.Attributes.Add("onclick", "CheckAll();")
                Else
                    page_readOnly.Value = "Y"
                End If

                LogManager.LogAction(ModuleType.LOD, UserAction.ViewPage, refId, "Viewed Page: Investigation")
            End If

        End Sub

        Protected Sub Save_Click(ByRef sender As Object, ByRef e As TabNavigationEventArgs)

            SaveData()

        End Sub

        Private Function CheckTime(ByVal val As TextBox) As Boolean
            If (val.Text.Trim.Length = 4 AndAlso Char.IsDigit(val.Text) AndAlso val.Text.Substring(0, 2) < 24 AndAlso val.Text.Substring(2, 2) < 60) Then
                Return True
            Else
                Return False
            End If
        End Function

        Private Sub DisplayReadOnly(ByVal investigation As LineOfDutyInvestigation)

            txtDateReport.Visible = False
            rblInvestigationOf.Visible = False
            rbRegularOrEad.Visible = False
            rbAdLess.Visible = False
            rbAdMore.Visible = False
            rbInactive.Visible = False
            rbShortTour.Visible = False

            txtDateStart.Visible = False
            txtDateFinish.Visible = False
            txtHrStart.Visible = False
            txtHrFinish.Visible = False
            txtInactiveDutyTraining.Visible = False

            txtOtherName1.Visible = False
            txtOtherName2.Visible = False
            txtOtherName3.Visible = False
            ddlGrade1.Visible = False
            ddlGrade2.Visible = False
            ddlGrade3.Visible = False
            chkOtherInvestMade1.Visible = False
            chkOtherInvestMade2.Visible = False
            chkOtherInvestMade3.Visible = False

            txtDateCircumstance.Visible = False
            txtHrCircumstance.Visible = False

            txtCircumstancePlace.Visible = False
            txtCircumstanceSustained.Visible = False
            txtDiagnosis.Visible = False
            rblPresentForDuty.Visible = False
            rblAbsentWithAuthority.Visible = False
            rblIntentionalMisconduct.Visible = False
            rblMentallySound.Visible = False

            txtRemarks.Visible = False
            LoadFindings(investigation)
            If Not String.IsNullOrEmpty(investigation.Remarks) Then
                lblRemarks.Text = investigation.Remarks.Replace(vbCrLf, "<br />") + "<br /><br /><br />"
            End If

            If (investigation.ReportDate.HasValue) Then
                lblDateReport.Text = investigation.ReportDate.Value.ToString(DATE_FORMAT)
            End If

            If (investigation.InvestigationOf.HasValue) Then
                lblInvestigationOf.Text = investigation.InvestigationDescription
            End If

            If (investigation.Status.HasValue) Then
                Select Case investigation.Status
                    Case 1
                        StatusRegularImage.Visible = True
                    Case 2
                        StatusADImage.Visible = True
                        AdDurationLabel.Text = "&nbsp;More than 30 days"
                    Case 3
                        StatusADImage.Visible = True
                        AdDurationLabel.Text = "&nbsp;Less than 30 days"
                    Case 4
                        StatusInactiveImage.Visible = True

                        If (Not String.IsNullOrEmpty(investigation.InactiveDutyTraining)) Then
                            lblInactiveDutyTraining.Text = investigation.InactiveDutyTraining
                        End If
                    Case 5
                        StatusShortImage.Visible = True

                End Select

                If (investigation.Status = 4 Or investigation.Status = 5) Then

                    If (investigation.DurationStart.HasValue) Then
                        lblDateStart.Text = investigation.DurationStart.Value.ToString(DATE_HOUR_FORMAT)
                    End If

                    If (investigation.DurationEnd.HasValue) Then
                        lblDateFinish.Text = investigation.DurationEnd.Value.ToString(DATE_HOUR_FORMAT)
                    End If

                End If

            End If

            '--------------------------
            ' Other Personnel
            '--------------------------

            If (investigation.OtherPersonnel IsNot Nothing) Then

                If (investigation.OtherPersonnel.Count >= 1) Then
                    Dim per As PersonnelData = investigation.OtherPersonnel(0)
                    lblOtherName1.Text = per.Name
                    lblOtherGrade1.Text = per.Grade
                    Invest1Image.Visible = per.InvestigationMade
                End If

                If (investigation.OtherPersonnel.Count >= 2) Then
                    Dim per As PersonnelData = investigation.OtherPersonnel(1)
                    lblOtherName2.Text = per.Name
                    lblOtherGrade2.Text = per.Grade
                    Invest2Image.Visible = per.InvestigationMade
                End If

                If (investigation.OtherPersonnel.Count >= 3) Then
                    Dim per As PersonnelData = investigation.OtherPersonnel(2)
                    lblOtherName3.Text = per.Name
                    lblOtherGrade3.Text = per.Grade
                    Invest3Image.Visible = per.InvestigationMade
                End If

            End If

            '--------------------------
            ' Basis for findings
            '--------------------------

            If (investigation.FindingsDate.HasValue) Then
                lblDateCircumstance.Text = investigation.FindingsDate.Value.ToString(DATE_FORMAT)
                lblHourCircumstance.Text = investigation.FindingsDate.Value.ToString(HOUR_FORMAT)
            End If

            lblCircumstancePlace.Text = investigation.Place

            If (Not String.IsNullOrEmpty(investigation.HowSustained)) Then
                lblCircumstanceSustained.Text = investigation.HowSustained.Replace(vbCrLf, "<br />") + "<br /><br /><br />"
            End If

            If (Not String.IsNullOrEmpty(investigation.MedicalDiagnosis)) Then
                lblDiagnosis.Text = investigation.MedicalDiagnosis.Replace(vbCrLf, "<br />") + "<br /><br /><br />"
            End If
            'IIf statements are simple enough - no error conditions and very little overhead
            If (investigation.PresentForDuty.HasValue) Then
                lblPresentForDuty.Text = IIf(investigation.PresentForDuty, "Yes", "No")
            End If

            If (investigation.AbsentWithAuthority.HasValue) Then
                lblAbsentWithAuthority.Text = IIf(investigation.AbsentWithAuthority, "Yes", "No")
            End If

            If (investigation.IntentionalMisconduct.HasValue) Then
                lblIntentionalMisconduct.Text = IIf(investigation.IntentionalMisconduct, "Yes", "No")
            End If

            If (investigation.MentallySound.HasValue) Then
                lblMentallySound.Text = IIf(investigation.MentallySound, "Yes", "No")
            End If

        End Sub

        Private Function GetIOFindings() As IEnumerable(Of FindingsLookUp)
            Return (From p In New LookupDao().GetWorkflowFindings(lod.Workflow, UserGroups.InvestigatingOfficer)
                    Where Not p.FindingType.Equals(InvestigationDecision.FORMAL_INVESTIGATION) And
                          Not p.FindingType.Equals(InvestigationDecision.APPROVE) And
                          Not p.FindingType.Equals(InvestigationDecision.DISAPPROVE)
                    Select p)
        End Function

        Private Sub LoadData()

            Dim dao As ILineOfDutyInvestigationDao = New NHibernateDaoFactory().GetLineOfDutyInvestigationDao()
            Dim investigation As LineOfDutyInvestigation = dao.FindById(refId)

            If (investigation Is Nothing) Then
                investigation = New LineOfDutyInvestigation(refId)
                dao.Save(investigation)
                Exit Sub
            End If

            If (UserCanEdit) Then
                DisplayReadWrite(investigation)
            Else
                DisplayReadOnly(investigation)
            End If

        End Sub

        Private Sub SaveData()

            If (Not UserCanEdit) Then
                Exit Sub
            End If

            Dim dao As ILineOfDutyInvestigationDao = New NHibernateDaoFactory().GetLineOfDutyInvestigationDao()
            Dim investigation As LineOfDutyInvestigation = dao.GetById(refId)

            If (investigation Is Nothing) Then
                Exit Sub
            End If

            If (txtDateReport.Text.Trim.Length > 0) Then
                Try
                    investigation.ReportDate = Server.HtmlEncode(Date.Parse(txtDateReport.Text))
                Catch ex As Exception
                End Try
            End If

            '--------------------------
            ' Investigation Of
            '--------------------------
            If (rblInvestigationOf.SelectedValue <> "") Then
                investigation.InvestigationOf = CByte(rblInvestigationOf.SelectedValue)
            End If

            '--------------------------
            ' Member Status
            '--------------------------

            If (rbRegularOrEad.Checked) Then
                investigation.Status = 1
                investigation.InactiveDutyTraining = String.Empty
            ElseIf (rbAdMore.Checked) Then
                investigation.Status = 2
                investigation.InactiveDutyTraining = String.Empty
            ElseIf (rbAdLess.Checked) Then
                investigation.Status = 3
                investigation.InactiveDutyTraining = String.Empty
            ElseIf (rbInactive.Checked) Then
                investigation.Status = 4
                investigation.InactiveDutyTraining = Server.HtmlEncode(txtInactiveDutyTraining.Text.Trim)
            ElseIf (rbShortTour.Checked) Then
                investigation.Status = 5
                investigation.InactiveDutyTraining = String.Empty
            End If

            If (investigation.Status = 4 Or investigation.Status = 5) Then

                'start date
                Try
                    If (txtDateStart.Text.Trim.Length > 0) Then
                        If (txtHrStart.Text.Trim.Length > 0) Then
                            investigation.DurationStart = Server.HtmlEncode(ParseDateAndTime((txtDateStart.Text.Trim) + " " + Trim(Me.txtHrStart.Text)))
                        Else
                            investigation.DurationStart = Server.HtmlEncode(Date.Parse(txtDateStart.Text.Trim))
                        End If
                    End If
                Catch ex As Exception
                    investigation.DurationStart = Nothing
                End Try

                'end date
                Try
                    If (txtDateFinish.Text.Trim.Length > 0) Then
                        If (txtHrFinish.Text.Trim.Length > 0) Then
                            investigation.DurationEnd = Server.HtmlEncode(ParseDateAndTime((txtDateFinish.Text.Trim) + " " + Trim(Me.txtHrFinish.Text)))
                        Else
                            investigation.DurationEnd = Server.HtmlEncode(Date.Parse(txtDateFinish.Text.Trim))
                        End If
                    End If
                Catch ex As Exception
                    investigation.DurationEnd = Nothing
                End Try
            Else

                'these don't apply
                investigation.DurationStart = Nothing
                investigation.DurationEnd = Nothing

            End If

            '--------------------------
            ' Other Personnel
            '--------------------------
            investigation.OtherPersonnel = New List(Of ALOD.Core.Domain.Common.PersonnelData)
            investigation.OtherPersonnel.Clear()

            If (Not String.IsNullOrEmpty(txtOtherName1.Text.Trim)) Then
                Dim per As New PersonnelData()
                per.Name = Server.HtmlEncode(txtOtherName1.Text.Trim)
                per.Grade = ddlGrade1.SelectedValue
                per.InvestigationMade = chkOtherInvestMade1.Checked
                investigation.OtherPersonnel.Add(per)
            End If

            If (Not String.IsNullOrEmpty(txtOtherName2.Text.Trim)) Then
                Dim per As New PersonnelData()
                per.Name = Server.HtmlEncode(txtOtherName2.Text.Trim)
                per.Grade = ddlGrade2.SelectedValue
                per.InvestigationMade = chkOtherInvestMade2.Checked
                investigation.OtherPersonnel.Add(per)
            End If

            If (Not String.IsNullOrEmpty(txtOtherName3.Text.Trim)) Then
                Dim per As New PersonnelData()
                per.Name = Server.HtmlEncode(txtOtherName3.Text.Trim)
                per.Grade = ddlGrade3.SelectedValue
                per.InvestigationMade = chkOtherInvestMade3.Checked
                investigation.OtherPersonnel.Add(per)
            End If

            '--------------------------
            ' Basis for findings
            '--------------------------

            'All Dates
            If (txtDateCircumstance.Text.Trim <> "" AndAlso CheckDate(Me.txtDateCircumstance)) Then
                If (CheckTime(Me.txtHrCircumstance)) Then
                    investigation.FindingsDate = Server.HtmlEncode(ParseDateAndTime((txtDateCircumstance.Text.Trim) + " " + Trim(Me.txtHrCircumstance.Text)))
                Else
                    investigation.FindingsDate = Server.HtmlEncode(DateTime.Parse(txtDateCircumstance.Text.Trim))
                End If
            Else
                investigation.FindingsDate = Nothing
            End If

            investigation.Place = Server.HtmlEncode(txtCircumstancePlace.Text.Trim)
            investigation.HowSustained = Server.HtmlEncode(txtCircumstanceSustained.Text.Trim)
            investigation.MedicalDiagnosis = Server.HtmlEncode(txtDiagnosis.Text.Trim)

            If (rblPresentForDuty.SelectedValue.Length > 0) Then
                investigation.PresentForDuty = Boolean.Parse(rblPresentForDuty.SelectedValue)
            End If

            If (rblPresentForDuty.SelectedValue = "False") Then
                If (rblAbsentWithAuthority.SelectedValue.Length > 0) Then
                    investigation.AbsentWithAuthority = Boolean.Parse(rblAbsentWithAuthority.SelectedValue)
                End If
            Else
                investigation.AbsentWithAuthority = Nothing
            End If

            If (rblIntentionalMisconduct.SelectedValue.Length > 0) Then
                investigation.IntentionalMisconduct = Boolean.Parse(rblIntentionalMisconduct.SelectedValue)
            End If

            If (rblMentallySound.SelectedValue.Length > 0) Then
                investigation.MentallySound = Boolean.Parse(rblMentallySound.SelectedValue)
            End If

            investigation.Remarks = Server.HtmlEncode(txtRemarks.Text.Trim)

            '--------------------------
            'findings
            '--------------------------

            ' If rblFindings.SelectedValue <> "" Then
            'investigation.Finding = Byte.Parse(rblFindings.SelectedValue)
            'End If

            'If (investigation.Validate() = False) Then
            '    ShowValidationErrors(Lod.Validation)
            'End If

            investigation.ModifiedBy = CInt(Session("UserId"))
            investigation.ModifiedDate = DateTime.Now
            SaveFindings()
            dao.SaveOrUpdate(investigation)

        End Sub

#Region "Validation"

        Protected Sub ShowValidationErrors(ByVal lstValidation As IList(Of ValidationItem))
            Dim InValidFields As String

            '        vldErrorMsgs.DataSource = lstValidation

            For Each Item As ValidationItem In lstValidation

                InValidFields = CType(GetLocalResourceObject(Item.Field()), String)
                If Not (InValidFields Is Nothing) Then
                    Dim strFields() As String = InValidFields.Trim.Split(",")
                    Dim i As Integer
                    For i = 0 To strFields.Length - 1
                        Dim ctl As WebControl = FindOnTab(strFields(i), Me)
                        HighlightInvalidField(ctl, False)
                    Next
                End If
            Next

        End Sub

#End Region

        Private Sub TabButtonClicked(ByRef sender As Object, ByRef e As TabNavigationEventArgs)

            If (Navigator.CurrentStep.IsReadOnly) Then
                Exit Sub
            End If

            If (e.ButtonType = NavigatorButtonType.Save OrElse e.ButtonType = NavigatorButtonType.NavigatedAway _
                OrElse e.ButtonType = NavigatorButtonType.NextStep OrElse e.ButtonType = NavigatorButtonType.PreviousStep) Then
                SaveData()
            End If

        End Sub

    End Class

End Namespace