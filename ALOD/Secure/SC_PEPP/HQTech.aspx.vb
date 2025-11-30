Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.Lookup
Imports ALOD.Core.Domain.Modules.SpecialCases
Imports ALOD.Core.Domain.Users
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Core.Utils
Imports ALOD.Data
Imports ALOD.Logging
Imports ALOD.Web.UserControls
Imports ALODWebUtility.Common

Namespace Web.Special_Case.PEPP

    Partial Class Secure_sc_pepp_HQTech
        Inherits System.Web.UI.Page

        Private _specCaseDao As ISpecialCaseDAO

        Public ReadOnly Property Navigator() As TabNavigator
            Get
                Return MasterPage.Navigator
            End Get
        End Property

        Public ReadOnly Property TabControl() As TabControls
            Get
                Return Master.TabControl
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

#Region "LODProperty"

        Private _dispositionDao As ILookupDispositionDao
        Dim _lookupDao As ILookupDao
        Private _scAccess As Dictionary(Of String, ALOD.Core.Domain.Workflow.PageAccessType)
        Dim dao As ISpecialCaseDAO

        'Private sc As SpecialCase = Nothing
        Private sc As SC_PEPP = Nothing

        Private scId As Integer = 0

        ReadOnly Property LookupDao() As ILookupDao
            Get
                If (_lookupDao Is Nothing) Then
                    _lookupDao = New NHibernateDaoFactory().GetLookupDao()
                End If

                Return _lookupDao
            End Get
        End Property

        ReadOnly Property SCDao() As ISpecialCaseDAO
            Get
                If (dao Is Nothing) Then
                    dao = New NHibernateDaoFactory().GetSpecialCaseDAO()
                End If

                Return dao
            End Get
        End Property

        Public ReadOnly Property ReferenceId() As Integer
            Get
                Return Integer.Parse(Request.QueryString("refId"))
            End Get
        End Property

        Protected ReadOnly Property SpecCase() As SC_PEPP
            Get
                If (sc Is Nothing) Then
                    sc = SCDao.GetById(ReferenceId)
                End If

                Return sc
            End Get
        End Property

        Private ReadOnly Property DispositionDao() As ILookupDispositionDao
            Get
                If (_dispositionDao Is Nothing) Then
                    _dispositionDao = New NHibernateDaoFactory().GetLookupDispositionDao()
                End If

                Return _dispositionDao
            End Get
        End Property

#End Region

        Protected ReadOnly Property MasterPage() As SC_PEPPMaster
            Get
                Dim master As SC_PEPPMaster = CType(Page.Master, SC_PEPPMaster)
                Return master
            End Get
        End Property

        Protected ReadOnly Property SectionList() As Dictionary(Of String, ALOD.Core.Domain.Workflow.PageAccessType)
            Get
                If (_scAccess Is Nothing) Then
                    _scAccess = SpecCase.ReadSectionList(CInt(Session("GroupId")))
                End If
                Return _scAccess
            End Get
        End Property

        Private ReadOnly Property RefId() As Integer
            Get
                Return CInt(Request.QueryString("refId"))
            End Get
        End Property

        Private ReadOnly Property SpecCaseDao() As ISpecialCaseDAO
            Get
                If (_specCaseDao Is Nothing) Then
                    _specCaseDao = New NHibernateDaoFactory().GetSpecialCaseDAO()
                End If
                Return _specCaseDao
            End Get
        End Property

        Protected Sub ddlRating_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles ddlRating.SelectedIndexChanged
            If ddlRating.SelectedItem.Text.ToLower() = "other" Then
                OtherRatingRow.Visible = True
            Else
                OtherRatingRow.Visible = False
            End If
        End Sub

        Protected Sub ddlType_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles chklType.SelectedIndexChanged
            'If chklType.SelectedItem.Text.ToLower() = "other" Then
            '    OtherTypeRow.Visible = True
            'Else
            '    OtherTypeRow.Visible = False
            'End If
            OtherTypeRow.Visible = GetIsTypeOtherChecked()
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            ucICDCodeControl.Initialilze(Me)
            ucICD7thCharacterControl.Initialize(ucICDCodeControl)

            If (Not IsPostBack) Then
                UserCanEdit = GetAccess(Navigator.PageAccess, True)
                InitControls()
                SetMaxLength(txtEnterType)
                SetMaxLength(txtEnterRating)
                SetMaxLength(DiagnosisTextBox)

                'Validation
                If (UserCanEdit) Then
                    sc.Validate()
                    If (sc.Validations.Count > 0) Then
                        ShowPageValidationErrors(sc.Validations, Me)
                    End If
                End If

                LogManager.LogAction(ModuleType.SpecCasePEPP, UserAction.ViewPage, RefId, "Viewed Page: Physical Examinationn Processing Program HQ AFRC Technician")

            End If

        End Sub

        Protected Sub rdbWaiverRequiredYes_CheckedChanged(sender As Object, e As EventArgs) Handles rdbWaiverRequiredYes.CheckedChanged, rdbWaiverRequiredNo.CheckedChanged
            If (rdbWaiverRequiredYes.Checked) Then
                ToggleWaiverControls(True)
            Else
                ToggleWaiverControls(False)
            End If
        End Sub

        Private Function GetIsTypeOtherChecked() As Boolean
            For Each item As ListItem In chklType.Items
                If (item.Text.Trim.ToLower() = "other" AndAlso item.Selected) Then
                    Return True
                End If
            Next

            Return False
        End Function

        Private Function GetSelectedTypes() As List(Of Integer)
            Dim selectedTypes As List(Of Integer) = New List(Of Integer)()

            For Each item As ListItem In chklType.Items
                If (item.Selected) Then
                    selectedTypes.Add(item.Value)
                End If
            Next

            Return selectedTypes
        End Function

        ' Returns the abbreviated form a special case name. The names are meant to come from the ModuleType enum in App_Code/Core/Common/Enums.vb
        Private Function GetSpecCaseAbbreviation(ByVal name As String) As String
            If (String.IsNullOrEmpty(name) Or Not name.Contains("SpecCase")) Then
                Return String.Empty
            End If

            Dim abbreviation As String = name
            Dim subStr As String = "SpecCase"

            'Parse out the part of the name that exists after 'SpecCase'
            abbreviation = name.Substring(subStr.Length).ToUpper()

            Return abbreviation
        End Function

        Private Sub InitControls()

            SetInputFormatRestriction(Page, txtDateReceived, FormatRestriction.Numeric, "/")
            SetInputFormatRestriction(Page, txtExpireDate, FormatRestriction.Numeric, "/")
            SetInputFormatRestriction(Page, txtEnterType, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, txtEnterRating, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, DiagnosisTextBox, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)

            'Load Findings

            If Not IsNothing(SpecCase.ICD9Code) Then
                ucICDCodeControl.InitializeHierarchy(SpecCase.ICD9Code)

                If (ucICDCodeControl.IsValidICDCode(SpecCase.ICD9Code)) Then
                    ucICDCodeControl.UpdateICDCodeDiagnosisLabel(SpecCase.ICD9Code, UserCanEdit)
                    DiagnosisTextBox.Text = Server.HtmlDecode(SpecCase.ICD9Diagnosis)
                End If

                If (Not String.IsNullOrEmpty(SpecCase.ICD7thCharacter)) Then
                    ucICD7thCharacterControl.InitializeCharacters(SpecCase.ICD9Code, SpecCase.ICD7thCharacter)
                    ucICD7thCharacterControl.Update7thCharacterLabel(SpecCase.ICD9Code, SpecCase.ICD7thCharacter)
                Else
                    ucICD7thCharacterControl.InitializeCharacters(SpecCase.ICD9Code, String.Empty)
                End If
            Else
                ucICD7thCharacterControl.InitializeCharacters(0, String.Empty)
            End If

            PopulateDropDowns()

            ' Get Type
            Dim types As List(Of LookUpItem) = SpecCaseDao.GetSpecialCasePEPPTypes(SpecCase.Id)

            If (types IsNot Nothing AndAlso types.Count > 0) Then
                For Each t As LookUpItem In types
                    Dim item As ListItem = chklType.Items.FindByValue(t.Value)

                    If (item IsNot Nothing) Then
                        item.Selected = True
                    End If
                Next
            End If

            ' Get Other Type Name
            If Not IsNothing(SpecCase.TypeName) Then
                txtEnterType.Text = Server.HtmlDecode(SpecCase.TypeName.ToString())
            End If

            ' Display the type text box if necessary
            OtherTypeRow.Visible = GetIsTypeOtherChecked()
            'If ddlType.SelectedItem.Text.ToLower() = "other" Then
            '    OtherTypeRow.Visible = True
            'Else
            '    OtherTypeRow.Visible = False
            'End If

            ' Get Rating
            If SpecCase.Rating.HasValue Then
                ' Check if the selected Rating is an Active or Inactive Rating
                If (Not (ddlRating.Items.FindByValue(SpecCase.Rating) Is Nothing)) Then
                    ddlRating.SelectedValue = SpecCase.Rating
                Else
                    ' Inactive; therefore add the inactive rating to the bottom of the dropdown.
                    ' This is needed for older cases that might have been created using ratings that are now inactive.
                    Dim item = New ListItem()

                    item.Text = LookupDao.GetPEPPRatings(SpecCase.Rating, 0).Tables(0).Rows(0)("Name").ToString()
                    item.Value = SpecCase.Rating
                    ddlRating.Items.Insert(ddlRating.Items.Count, item)
                    ddlRating.SelectedValue = SpecCase.Rating
                End If
            End If

            ' Get Other Rating Name
            If Not IsNothing(SpecCase.RatingName) Then
                txtEnterRating.Text = Server.HtmlDecode(SpecCase.RatingName.ToString())
            End If

            ' Display the rating text box if necessary
            If ddlRating.SelectedItem.Text.ToLower() = "other" Then
                OtherRatingRow.Visible = True
            Else
                OtherRatingRow.Visible = False
            End If

            ' Get Disposition
            If SpecCase.Disposition.HasValue Then
                ' Check if the selected Disposition is an Active or Inactive Disposition
                If (Not (ddlDisposition.Items.FindByValue(SpecCase.Disposition) Is Nothing)) Then
                    ddlDisposition.SelectedValue = SpecCase.Disposition
                Else
                    ' Inactive; therefore add the inactive Disposition to the bottom of the dropdown.
                    ' This is needed for older cases that might have been created using Dispositions that are now inactive.
                    Dim item = New ListItem()

                    'item.Text = LookupDao.GetPEPPDispositions(SpecCase.Disposition, 0).Tables(0).Rows(0)("Name").ToString()
                    item.Text = DispositionDao.GetById(SpecCase.Disposition).Name
                    item.Value = SpecCase.Disposition
                    ddlDisposition.Items.Insert(ddlDisposition.Items.Count, item)
                    ddlDisposition.SelectedValue = SpecCase.Disposition
                End If
            End If

            ' Get Renewal
            If SpecCase.Renewal = 1 Then
                rdbRenewalYes.Checked = True
            End If

            If SpecCase.Renewal = 0 Then
                rdbRenewalNo.Checked = True
            End If

            ' Get Base Assign
            If SpecCase.BaseAssign.HasValue Then
                ddlBaseAssign.SelectedValue = SpecCase.BaseAssign
            End If

            ' Get Date Received
            If SpecCase.DateReceived.HasValue Then
                txtDateReceived.Text = Server.HtmlDecode(SpecCase.DateReceived.Value.ToString(DATE_FORMAT))
            End If

            ' Get Waiver Required
            If (SpecCase.IsWaiverRequired.HasValue) Then
                If (SpecCase.IsWaiverRequired.Value = True) Then
                    rdbWaiverRequiredYes.Checked = True
                Else
                    rdbWaiverRequiredNo.Checked = True
                End If
            End If

            ' Get Waiver Expiration Date
            If SpecCase.WaiverExpirationDate.HasValue Then
                txtExpireDate.Text = Server.HtmlDecode(SpecCase.WaiverExpirationDate.Value.ToString(DATE_FORMAT))
            End If

            chbIndefinite.Attributes.Add("onclick", "SetIndefinite(this)")

            If txtExpireDate.Text = "12/31/2100" Then
                txtExpireDate.CssClass = ""
                txtExpireDate.Enabled = False
                chbIndefinite.Checked = True
            Else
                txtExpireDate.CssClass = "datePickerFuture"
                txtExpireDate.Enabled = True
                chbIndefinite.Checked = False

            End If

            ' Get ALC
            If SpecCase.ALCLetterType.HasValue Then
                ddlALC.SelectedValue = SpecCase.ALCLetterType
            End If

            If UserCanEdit Then
                'Read/Write

                If (rdbWaiverRequiredYes.Checked = True) Then
                    ToggleWaiverControls(True)
                Else
                    ToggleWaiverControls(False)
                End If

                DiagnosisTextBox.ReadOnly = False

                chklType.Enabled = True
                ddlRating.Enabled = True
                ddlDisposition.Enabled = True
                ddlBaseAssign.Enabled = True
                ddlALC.Enabled = True

                rdbRenewalNo.Enabled = True
                rdbRenewalYes.Enabled = True

                rdbWaiverRequiredNo.Enabled = True
                rdbWaiverRequiredYes.Enabled = True

                txtDateReceived.CssClass = "datePickerPast"
                txtDateReceived.Enabled = True
            Else
                'Read Only

                ucICDCodeControl.DisplayReadOnly(True)
                ucICD7thCharacterControl.DisplayReadOnly()
                DiagnosisTextBox.Enabled = True
                DiagnosisTextBox.ReadOnly = True

                chklType.Enabled = False
                ddlRating.Enabled = False
                ddlDisposition.Enabled = False
                ddlBaseAssign.Enabled = False
                ddlALC.Enabled = False

                rdbRenewalNo.Enabled = False
                rdbRenewalYes.Enabled = False

                rdbWaiverRequiredNo.Enabled = False
                rdbWaiverRequiredYes.Enabled = False

                txtDateReceived.Enabled = False
                txtExpireDate.Enabled = False
                txtExpireDate.CssClass = String.Empty

                txtEnterType.Enabled = False
                txtEnterType.ReadOnly = True

                txtEnterRating.Enabled = False
                txtEnterRating.ReadOnly = True

                chbIndefinite.Enabled = False

            End If

        End Sub

        Private Sub PopulateDropDowns()

            Dim lkupDAO As ILookupDao
            lkupDAO = New NHibernateDaoFactory().GetLookupDao()

            ' Populate the Type dropdown
            chklType.DataSource = lkupDAO.GetPEPPTypes(0, 1)
            chklType.DataTextField = "Name"
            chklType.DataValueField = "Value"
            chklType.DataBind()

            chklType.RepeatColumns = chklType.Items.Count / 2
            'listTop.Text = "--- Select a Type ---"
            'listTop.Value = 0
            'ddlType.Items.Insert(0, listTop)

            ' Populate the Rating dropdown
            ddlRating.DataSource = lkupDAO.GetPEPPRatings(0, 1)
            ddlRating.DataTextField = "Name"
            ddlRating.DataValueField = "Value"
            ddlRating.DataBind()

            Dim listTop = New ListItem()
            listTop = New ListItem()
            listTop.Text = "--- Select a Rating ---"
            listTop.Value = 0
            ddlRating.Items.Insert(0, listTop)

            ' Populate the Disposition dropdown
            ddlDisposition.DataSource = DispositionDao.GetWorkflowDispositions(AFRCWorkflows.SpecCasePEPP)
            ddlDisposition.DataTextField = "Name"
            ddlDisposition.DataValueField = "Id"
            ddlDisposition.DataBind()

            listTop = New ListItem()
            listTop.Text = "--- Select a Disposition ---"
            listTop.Value = 0
            ddlDisposition.Items.Insert(0, listTop)

            ' Populate the Base Assign dropdown
            ddlBaseAssign.DataSource = From n In lkupDAO.GetRMUNames("") Select n
            ddlBaseAssign.DataTextField = "Name"
            ddlBaseAssign.DataValueField = "Value"
            ddlBaseAssign.DataBind()

            listTop = New ListItem()
            listTop.Text = "--- Select One ---"
            listTop.Value = 0
            ddlBaseAssign.Items.Insert(0, listTop)

        End Sub

        Private Sub SaveFindings()

            If (Not UserCanEdit) Then
                Exit Sub
            End If

            If (ucICDCodeControl.IsICDCodeSelected()) Then
                SpecCase.ICD9Code = ucICDCodeControl.SelectedICDCodeID
                SpecCase.ICD9Description = ucICDCodeControl.SelectedICDCodeText
            Else
                'SpecCase.ICD9Code = Nothing
                'SpecCase.ICD9Description = Nothing
            End If

            SpecCase.ICD9Diagnosis = Server.HtmlEncode(DiagnosisTextBox.Text.Trim())

            If (ucICD7thCharacterControl.Is7thCharacterSelected()) Then
                SpecCase.ICD7thCharacter = ucICD7thCharacterControl.Selected7thCharacter
            Else
                SpecCase.ICD7thCharacter = Nothing
            End If

            If (Not IsNothing(SpecCase.ICD9Code)) Then
                ucICDCodeControl.UpdateICDCodeDiagnosisLabel(SpecCase.ICD9Code, True)
            End If

            ' Save Types
            Dim selectedTypes As List(Of Integer) = GetSelectedTypes()

            If (selectedTypes IsNot Nothing) Then
                SpecCaseDao.UpdateSpecialCasePEPPTypes(SpecCase.Id, selectedTypes)
            End If

            ' Save Other Type Name
            If (GetIsTypeOtherChecked()) Then
                SpecCase.TypeName = Server.HtmlEncode(txtEnterType.Text)
            Else
                SpecCase.TypeName = Server.HtmlEncode(String.Empty)
            End If

            ' Save Rating
            If Not String.IsNullOrEmpty(ddlRating.SelectedValue) AndAlso ddlRating.SelectedValue > 0 Then
                SpecCase.Rating = ddlRating.SelectedValue
            End If

            ' Save Other Rating Name
            If ddlRating.SelectedItem.Text.ToLower() = "other" Then
                SpecCase.RatingName = Server.HtmlEncode(txtEnterRating.Text)
            Else
                SpecCase.RatingName = Server.HtmlEncode(String.Empty)
            End If

            ' Save Disposition
            If Not String.IsNullOrEmpty(ddlDisposition.SelectedValue) AndAlso ddlDisposition.SelectedValue > 0 Then
                SpecCase.Disposition = ddlDisposition.SelectedValue
            End If

            ' Save Renewal
            If rdbRenewalNo.Checked Then
                SpecCase.Renewal = 0
            End If

            If rdbRenewalYes.Checked Then
                SpecCase.Renewal = 1
            End If

            ' Save Base Assign
            If Not String.IsNullOrEmpty(ddlBaseAssign.SelectedValue) AndAlso ddlBaseAssign.SelectedValue > 0 Then
                SpecCase.BaseAssign = ddlBaseAssign.SelectedValue
            End If

            ' Save Date Recevied
            If Not String.IsNullOrEmpty(txtDateReceived.Text) Then
                SpecCase.DateReceived = Server.HtmlEncode(txtDateReceived.Text)
            End If

            ' Save Waiver Required
            If (rdbWaiverRequiredNo.Checked) Then
                SpecCase.IsWaiverRequired = False
            End If

            If (rdbWaiverRequiredYes.Checked) Then
                SpecCase.IsWaiverRequired = True
            End If

            ' Save Expire Date
            If Not String.IsNullOrEmpty(txtExpireDate.Text) Then
                SpecCase.WaiverExpirationDate = Server.HtmlEncode(txtExpireDate.Text)
            End If

            ' Save ALC
            If Not String.IsNullOrEmpty(ddlALC.SelectedValue) AndAlso ddlALC.SelectedValue > 0 Then
                SpecCase.ALCLetterType = ddlALC.SelectedValue
            End If

            SCDao.SaveOrUpdate(SpecCase)
            SCDao.CommitChanges()

        End Sub

        Private Sub ToggleWaiverControls(ByVal enable As Boolean)
            If (enable) Then
                txtExpireDate.Enabled = True
                chbIndefinite.Enabled = True
                txtExpireDate.CssClass = "datePickerFuture"
                ucICDCodeControl.DisplayReadWrite(False)
                ucICD7thCharacterControl.DisplayReadWrite()
                DiagnosisTextBox.Enabled = True
                lblExpireDateRowText.CssClass = "labelRequired"
                lblExpireDateRowText.Text = "*Waiver Expiration Date:"

                lblICDRowText.CssClass = "labelRequired"
                lblICDRowText.Text = "*Diagnosis:"
                lblDiagnosisRowText.CssClass = "labelRequired"
                lblDiagnosisRowText.Text = "*Diagnosis Text:"
            Else
                txtExpireDate.Enabled = False
                chbIndefinite.Enabled = False
                txtExpireDate.CssClass = String.Empty
                ucICDCodeControl.DisplayReadOnly(False)
                ucICD7thCharacterControl.DisplayReadOnly()
                DiagnosisTextBox.Enabled = False
                lblExpireDateRowText.CssClass = String.Empty
                lblExpireDateRowText.Text = "Waiver Expiration Date:"
                lblICDRowText.CssClass = String.Empty
                lblICDRowText.Text = "Diagnosis:"
                lblDiagnosisRowText.CssClass = String.Empty
                lblDiagnosisRowText.Text = "Diagnosis Text:"
            End If

        End Sub

#Region "TabEvent"

        Public Function ValidBoxLength() As Boolean
            Dim IsValid As Boolean = True
            If Not CheckTextLength(DiagnosisTextBox) Then
                IsValid = False
            End If

            Return IsValid
        End Function

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            AddHandler Me.Master.TabClick, AddressOf TabButtonClicked
        End Sub

        Protected Sub Save_Click(ByRef sender As Object, ByRef e As TabNavigationEventArgs)

            If (Navigator.CurrentStep.IsReadOnly) Then
                Exit Sub
            End If

            If (e.ButtonType = NavigatorButtonType.Save OrElse e.ButtonType = NavigatorButtonType.NavigatedAway _
                OrElse e.ButtonType = NavigatorButtonType.NextStep OrElse e.ButtonType = NavigatorButtonType.PreviousStep) Then
                SaveFindings()
            End If

            If Not (ValidBoxLength()) Then
                e.Cancel = True
                Exit Sub
            End If

        End Sub

        Private Sub TabButtonClicked(ByRef sender As Object, ByRef e As TabNavigationEventArgs)

            If (e.ButtonType = NavigatorButtonType.Save OrElse
                e.ButtonType = NavigatorButtonType.NavigatedAway OrElse
                e.ButtonType = NavigatorButtonType.NextStep OrElse
                e.ButtonType = NavigatorButtonType.PreviousStep) Then
                SaveFindings()
            End If

            If Not (ValidBoxLength()) Then
                e.Cancel = True
                Exit Sub
            End If

        End Sub

#End Region

    End Class

End Namespace