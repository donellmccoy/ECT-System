Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.Documents
Imports ALOD.Core.Domain.Lookup
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Modules.SpecialCases
Imports ALOD.Core.Domain.Users
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Core.Utils
Imports ALOD.Data
Imports ALOD.Logging
Imports ALOD.Web.UserControls
Imports ALODWebUtility.Common

Namespace Web.Special_Case.RS

    Partial Class Secure_sc_rs_HQTech
        Inherits System.Web.UI.Page

        Private _caseTypeDao As ICaseTypeDao
        Private _completedByGroupDao As ICompletedByGroupDao
        Private _dispositionDao As ILookupDispositionDao
        Private _MedTechSig As SignatureMetaData
        Private _memoDao As IMemoDao2
        Private _sigDao As ISignatueMetaDateDao
        Private _specCaseDao As ISpecialCaseDAO
        Private _stampDao As ICertificationStampDao

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

        Protected ReadOnly Property MasterPage() As SC_RSMaster
            Get
                Dim master As SC_RSMaster = CType(Page.Master, SC_RSMaster)
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

        Private ReadOnly Property CaseTypeDao() As ICaseTypeDao
            Get
                If (_caseTypeDao Is Nothing) Then
                    _caseTypeDao = New NHibernateDaoFactory().GetCaseTypeDao()
                End If

                Return _caseTypeDao
            End Get
        End Property

        Private ReadOnly Property CertificationStampDao() As ICertificationStampDao
            Get
                If (_stampDao Is Nothing) Then
                    _stampDao = New NHibernateDaoFactory().GetCertificationStampDao()
                End If

                Return _stampDao
            End Get
        End Property

        Private ReadOnly Property CompletedByGroupDao() As ICompletedByGroupDao
            Get
                If (_completedByGroupDao Is Nothing) Then
                    _completedByGroupDao = New NHibernateDaoFactory().GetCompletedByGroupDao()
                End If

                Return _completedByGroupDao
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

        Private ReadOnly Property MedTechSig() As SignatureMetaData
            Get
                If (_MedTechSig Is Nothing) Then
                    _MedTechSig = SigDao.GetByWorkStatus(SpecCase.Id, SpecCase.Workflow, SpecCaseWWDWorkStatus.InitiateCase)
                End If

                Return _MedTechSig
            End Get
        End Property

        Private ReadOnly Property MemoDao() As IMemoDao2
            Get
                If (_memoDao Is Nothing) Then
                    _memoDao = New NHibernateDaoFactory().GetMemoDao2()
                End If

                Return _memoDao
            End Get
        End Property

        Private ReadOnly Property RefId() As Integer
            Get
                Return CInt(Request.QueryString("refId"))
            End Get
        End Property

        Private ReadOnly Property SigDao() As SignatureMetaDataDao
            Get
                If (_sigDao Is Nothing) Then
                    _sigDao = New NHibernateDaoFactory().GetSigMetaDataDao()
                End If

                Return _sigDao
            End Get
        End Property

#Region "Special Case Property"

        Dim _lookupDao As ILookupDao
        Private _scAccess As Dictionary(Of String, ALOD.Core.Domain.Workflow.PageAccessType)
        Dim dao As ISpecialCaseDAO

        'Private sc As SpecialCase = Nothing
        Private sc As SC_RS = Nothing

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

        Protected ReadOnly Property SpecCase() As SC_RS
            Get
                If (sc Is Nothing) Then
                    sc = SCDao.GetById(ReferenceId)
                End If

                Return sc
            End Get
        End Property

#End Region

        Private ReadOnly Property SpecCaseDao() As ISpecialCaseDAO
            Get
                If (_specCaseDao Is Nothing) Then
                    _specCaseDao = New NHibernateDaoFactory().GetSpecialCaseDAO()
                End If
                Return _specCaseDao
            End Get
        End Property

        Protected Sub ddlALC_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles ddlALC.SelectedIndexChanged
            If (Not SpecCase.GetActiveBoardMedicalFinding().HasValue) Then
                FillMemoTemplatesList()
            End If
        End Sub

        Protected Sub ddlCaseType_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles ddlCaseType.SelectedIndexChanged
            Dim selectedValue As Integer = Integer.Parse(ddlCaseType.SelectedValue)

            If (HasSubCaseTypes(selectedValue)) Then
                TurnSubCaseTypeRowOnOff(True)
            Else
                TurnSubCaseTypeRowOnOff(False)
            End If

            FillSubCaseTypeList(selectedValue)
            InitCertificationStampControls()

            utrOtherCaseType.Visible = GetIsCaseTypeOtherSelected()
            utrOtherSubCaseType.Visible = GetIsSubCaseTypeOtherSelected()
        End Sub

        Protected Sub ddlCompletedByUnit_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles ddlCompletedByUnit.SelectedIndexChanged
            OtherCompletedByUnitRow.Visible = GetIsCompletedByUnitOtherSelected()
        End Sub

        Protected Sub ddlDisposition_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles ddlDisposition.SelectedIndexChanged
            'If (Not SpecCase.GetActiveBoardMedicalFinding().HasValue) Then
            UpdateCertificationStampControls()
            FillMemoTemplatesList()
            'End If
        End Sub

        Protected Sub ddlRating_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles ddlRating.SelectedIndexChanged
            OtherRatingRow.Visible = GetIsRatingOtherSelected()
        End Sub

        Protected Sub ddlSubCaseType_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles ddlSubCaseType.SelectedIndexChanged
            utrOtherSubCaseType.Visible = GetIsSubCaseTypeOtherSelected()
        End Sub

        Protected Sub ddlType_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles ddlType.SelectedIndexChanged
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
                SetMaxLength(txtCompletedByUnitName)
                SetMaxLength(DiagnosisTextBox)
                SetMaxLength(txtFreeText)
                SetMaxLength(txtSecondaryFreeText)
                SetMaxLength(txtOtherCaseTypeName)
                SetMaxLength(txtOtherSubCaseTypeName)

                'Validation
                If (UserCanEdit) Then
                    sc.Validate()
                    If (sc.Validations.Count > 0) Then
                        ShowPageValidationErrors(sc.Validations, Me)
                    End If
                End If

                LogManager.LogAction(ModuleType.SpecCaseRS, UserAction.ViewPage, RefId, "Viewed Page: Recruiting Services HQ AFRC Technician")

            End If

        End Sub

        Private Sub DisableControls()
            ucICDCodeControl.DisplayReadOnly(True)
            ucICD7thCharacterControl.DisplayReadOnly()
            DiagnosisTextBox.Enabled = True
            DiagnosisTextBox.ReadOnly = True

            ddlType.Enabled = False
            ddlRating.Enabled = False
            ddlDisposition.Enabled = False
            ddlALC.Enabled = False
            ddlCertificationStamp.Enabled = False
            txtFreeText.Enabled = False
            ddlSecondaryCertificationStamp.Enabled = False
            txtSecondaryFreeText.Enabled = False

            ddlCompletedByUnit.Enabled = False
            txtCompletedByUnitName.Enabled = False
            txtCompletedByUnitName.ReadOnly = True

            txtDateReceived.Enabled = False

            txtEnterType.Enabled = False
            txtEnterType.ReadOnly = True

            txtEnterRating.Enabled = False
            txtEnterRating.ReadOnly = True

            pnlCaseInfo.Visible = True
            pnlCaseInfoReassessment.Visible = False
            upnlCaseInfo.Visible = False

            ddlMemoTemplates.Enabled = False

            txtPOCEmailLabel.Enabled = False
            txtPOCPhoneLabel.Enabled = False
            txtPOCNameLabel.Enabled = False
        End Sub

        Private Sub EnableControls(ByVal isReassessmentCase As Boolean)
            If (SpecCase.Status = SpecCaseRSWorkStatus.FinalReview) Then 'AndAlso SpecCase.AreDispositionsDifferent(DispositionDao) = True
                DisableControls()

                ddlCertificationStamp.Enabled = True
                txtFreeText.Enabled = True
                ddlSecondaryCertificationStamp.Enabled = True
                txtSecondaryFreeText.Enabled = True
                ddlMemoTemplates.Enabled = True
            Else
                DiagnosisTextBox.ReadOnly = False

                ddlType.Enabled = True
                ddlRating.Enabled = True
                ddlDisposition.Enabled = True
                ddlALC.Enabled = True
                ddlCompletedByUnit.Enabled = True
                ddlMemoTemplates.Enabled = True
                txtCompletedByUnitName.Enabled = True

                txtDateReceived.CssClass = "datePickerPast"
                txtDateReceived.Enabled = True

                If (isReassessmentCase) Then
                    pnlCaseInfo.Visible = False
                    pnlCaseInfoReassessment.Visible = True
                    upnlCaseInfo.Visible = True

                    If (ddlCaseType.SelectedValue > 0 AndAlso HasSubCaseTypes(ddlCaseType.SelectedValue)) Then
                        TurnSubCaseTypeRowOnOff(True)
                    Else
                        TurnSubCaseTypeRowOnOff(False)
                    End If

                    utrOtherCaseType.Visible = GetIsCaseTypeOtherSelected()
                    utrOtherSubCaseType.Visible = GetIsSubCaseTypeOtherSelected()
                Else
                    pnlCaseInfo.Visible = True
                    pnlCaseInfoReassessment.Visible = False
                    upnlCaseInfo.Visible = False
                End If
            End If
        End Sub

        Private Sub FillCaseTypeList()
            ddlCaseType.DataSource = CaseTypeDao.GetWorkflowCaseTypes(SpecCase.Workflow)
            ddlCaseType.DataTextField = "Name"
            ddlCaseType.DataValueField = "Id"
            ddlCaseType.DataBind()

            Dim listTop = New ListItem()
            listTop.Text = "--- Select Speciality Case Type ---"
            listTop.Value = 0
            ddlCaseType.Items.Insert(0, listTop)
        End Sub

        Private Sub FillMemoTemplatesList()
            Dim dValue As Integer = -1
            Dim alc As Integer = 0
            'Dim includeFirstItem As Boolean = True
            Dim includeBothItem As Boolean = False

            If (SpecCase.GetActiveBoardMedicalFinding().HasValue) Then
                dValue = SpecCase.GetActiveDispositionValue(DispositionDao)
            ElseIf (ddlDisposition.SelectedValue > 0) Then
                dValue = SpecCase.NormalizeHQTechDisposition(DispositionDao.GetById(ddlDisposition.SelectedValue))
            ElseIf (SpecCase.Disposition.HasValue AndAlso SpecCase.Disposition.Value > 0) Then
                dValue = SpecCase.NormalizeHQTechDisposition(DispositionDao)
            End If

            If (Not SpecCase.GetActiveBoardMedicalFinding().HasValue) Then
                alc = Integer.Parse(ddlALC.SelectedValue)
            Else
                alc = SpecCase.GetActiveALC()
            End If

            If (dValue = 1) Then
                If (alc > 0) Then
                    ddlMemoTemplates.DataSource = MemoDao.GetTemplatesByModule(ModuleType.SpecCaseRS).Where(Function(x) x.Id = MemoType.RSALCMemo)
                    ddlMemoTemplates.Enabled = False
                Else
                    ddlMemoTemplates.DataSource = MemoDao.GetTemplatesByModule(ModuleType.SpecCaseRS).Where(Function(x) x.Id = MemoType.PalaceChaseQualified Or x.Id = MemoType.RSNonALCMemo)
                    ddlMemoTemplates.Enabled = (True And UserCanEdit)
                End If

                'includeFirstItem = False
            ElseIf (dValue = 0) Then
                ddlMemoTemplates.DataSource = MemoDao.GetTemplatesByModule(ModuleType.SpecCaseRS).Where(Function(x) x.Id = MemoType.PalaceChaseAFI Or x.Id = MemoType.PalaceChaseMSD Or x.Id = MemoType.PalaceChaseMSDandAFI)
                ddlMemoTemplates.Enabled = (True And UserCanEdit)
                includeBothItem = True
            Else
                ddlMemoTemplates.DataSource = New List(Of MemoTemplate)()
                ddlMemoTemplates.Enabled = (True And UserCanEdit)
            End If

            ddlMemoTemplates.DataValueField = "Id"
            ddlMemoTemplates.DataTextField = "Title"
            ddlMemoTemplates.DataBind()

            'If (includeFirstItem) Then
            '    Dim listTop = New ListItem()
            '    listTop.Text = "--- Select Memo ---"
            '    listTop.Value = 0
            '    ddlMemoTemplates.Items.Insert(0, listTop)
            'End If

        End Sub

        Private Sub FillSubCaseTypeList(ByVal parentCaseTypeId As Integer)
            If (parentCaseTypeId < 1 OrElse Not HasSubCaseTypes(parentCaseTypeId)) Then
                ddlSubCaseType.DataSource = New List(Of CaseType)()
            Else
                Dim ct As CaseType = CaseTypeDao.GetById(parentCaseTypeId)

                If (ct Is Nothing) Then
                    ddlSubCaseType.DataSource = New List(Of CaseType)()
                Else
                    ddlSubCaseType.DataSource = ct.SubCaseTypes
                End If
            End If

            ddlSubCaseType.DataTextField = "Name"
            ddlSubCaseType.DataValueField = "Id"
            ddlSubCaseType.DataBind()

            Dim listTop = New ListItem()
            listTop.Text = "--- Select Case Type ---"
            listTop.Value = 0
            ddlSubCaseType.Items.Insert(0, listTop)
        End Sub

        Private Function GetIsCaseTypeOtherSelected() As Boolean
            If (ddlCaseType.Items IsNot Nothing AndAlso ddlCaseType.Items.Count > 0 AndAlso ddlCaseType.SelectedItem.Text.ToLower() = "other") Then
                Return True
            End If

            Return False
        End Function

        Private Function GetIsCompletedByUnitOtherSelected() As Boolean
            If ddlCompletedByUnit.SelectedItem.Text.ToLower() = "other" Then
                Return True
            End If

            Return False
        End Function

        Private Function GetIsMemoRowVisible() As Boolean
            If (Not SpecCase.CaseType.HasValue OrElse SpecCase.CaseType.Value = 0) Then
                Return False
            End If

            Dim ct As CaseType = CaseTypeDao.GetById(SpecCase.CaseType.Value)

            If (ct Is Nothing) Then
                Return False
            End If

            If (ct.Name.Equals(ALOD.Core.Domain.Lookup.CaseType.PalaceChaseName) AndAlso ct.Name.Equals(ALOD.Core.Domain.Lookup.CaseType.PalaceFrontName)) Then
                Return True
            End If

            Return False
        End Function

        Private Function GetIsRatingOtherSelected() As Boolean
            If ddlRating.SelectedItem.Text.ToLower() = "other" Then
                Return True
            End If

            Return False
        End Function

        Private Function GetIsSubCaseTypeOtherSelected() As Boolean
            If (ddlSubCaseType.Items IsNot Nothing AndAlso ddlSubCaseType.Items.Count > 0 AndAlso ddlSubCaseType.SelectedItem.Text.ToLower() = "other") Then
                Return True
            End If

            Return False
        End Function

        Private Function GetIsTypeOtherChecked() As Boolean
            If ddlType.SelectedItem.Text.ToLower() = "other" Then
                Return True
            End If

            Return False
        End Function

        Private Function GetSelectedTypes() As List(Of Integer)
            Dim selectedTypes As List(Of Integer) = New List(Of Integer)()

            If (Integer.Parse(ddlType.SelectedValue) > 0) Then
                selectedTypes.Add(Integer.Parse(ddlType.SelectedValue))
            End If

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

        Private Function HasSubCaseTypes(ByVal caseTypeId As Integer) As Boolean
            Dim ct As CaseType = CaseTypeDao.GetById(caseTypeId)

            If (ct Is Nothing) Then
                Return False
            End If

            If (ct.SubCaseTypes.Count > 0) Then
                Return True
            End If

            Return False
        End Function

        Private Sub InitCertificationStampControls()
            Dim ct As CaseType = Nothing
            Dim isReassessmentCase As Boolean = SCDao.GetIsReassessmentCase(SpecCase.Id)

            If (isReassessmentCase AndAlso ddlCaseType.SelectedValue > 0) Then
                ct = CaseTypeDao.GetById(ddlCaseType.SelectedValue)
            ElseIf (SpecCase.CaseType.HasValue AndAlso SpecCase.CaseType.Value > 0) Then
                ct = CaseTypeDao.GetById(SpecCase.CaseType.Value)
            End If

            If (ct IsNot Nothing AndAlso ((ct.Name.Equals(ALOD.Core.Domain.Lookup.CaseType.PalaceChaseName) Or ct.Name.Equals(ALOD.Core.Domain.Lookup.CaseType.PalaceFrontName)) OrElse ct.Name.Equals(ALOD.Core.Domain.Lookup.CaseType.MEPSRequestName))) Then
                lblCertificationStamp.CssClass = String.Empty
                lblCertificationStamp.Text = "Certification Stamp:"
            Else
                lblCertificationStamp.CssClass = "labelRequired"
                lblCertificationStamp.Text = "*Certification Stamp:"
            End If

        End Sub

        Private Sub InitControls()

            Dim isReassessmentCase As Boolean = SCDao.GetIsReassessmentCase(SpecCase.Id)

            SetInputFormatRestriction(Page, txtDateReceived, FormatRestriction.Numeric, "/")
            SetInputFormatRestriction(Page, txtEnterType, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, txtEnterRating, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, txtCompletedByUnitName, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, DiagnosisTextBox, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, txtFreeText, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, txtSecondaryFreeText, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, txtOtherCaseTypeName, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, txtOtherSubCaseTypeName, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)

            Dim uDao As UserDao = New NHibernateDaoFactory().GetUserDao()
            Dim currUser As AppUser = uDao.GetById(SESSION_USER_ID)

            'Load Findings

            PopulateDropDowns()

            ' Get start page data
            If (isReassessmentCase) Then
                If (SpecCase.CaseType.HasValue AndAlso SpecCase.CaseType.Value > 0) Then
                    ddlCaseType.SelectedValue = SpecCase.CaseType.Value
                    FillSubCaseTypeList(SpecCase.CaseType.Value)
                End If

                If (Not String.IsNullOrEmpty(SpecCase.CaseTypeName)) Then
                    txtOtherCaseTypeName.Text = SpecCase.CaseTypeName
                End If

                If (SpecCase.SubCaseType.HasValue AndAlso SpecCase.SubCaseType.Value > 0) Then
                    ddlSubCaseType.SelectedValue = SpecCase.SubCaseType.Value
                End If

                If (Not String.IsNullOrEmpty(SpecCase.SubCaseTypeName)) Then
                    txtOtherSubCaseTypeName.Text = SpecCase.SubCaseTypeName
                End If
            Else
                If (SpecCase.CaseType.HasValue AndAlso SpecCase.CaseType.Value > 0) Then
                    Dim ct As CaseType = CaseTypeDao.GetById(SpecCase.CaseType.Value)

                    If (ct IsNot Nothing) Then
                        lblCaseType.Text = ct.Name

                        If (ct.Name.ToLower().Equals("other")) Then
                            lblOtherCaseTypeName.Text = ": " & Server.HtmlDecode(SpecCase.CaseTypeName)
                            lblOtherCaseTypeName.Visible = True
                        End If
                    Else
                        lblCaseType.Text = "Unknown"
                    End If

                    If (ct.SubCaseTypes.Count > 0) Then
                        trSubCaseType.Visible = True

                        If (SpecCase.SubCaseType.HasValue AndAlso SpecCase.SubCaseType.Value > 0) Then
                            Dim subCt As CaseType = (From x In CaseTypeDao.GetAllSubCaseTypes()
                                                     Where x.Id = SpecCase.SubCaseType).First()

                            If (subCt IsNot Nothing) Then
                                lblSubCaseType.Text = subCt.Name

                                If (subCt.Name.ToLower().Equals("other")) Then
                                    lblOtherSubCaseTypeName.Text = ": " & Server.HtmlDecode(SpecCase.SubCaseTypeName)
                                    lblOtherSubCaseTypeName.Visible = True
                                End If
                            Else
                                lblSubCaseType.Text = "Unknown"
                            End If
                        Else
                            lblSubCaseType.Text = "Unknown"
                        End If
                    End If
                Else
                    lblCaseType.Text = "Unknown"
                End If
            End If

            ' Get ICD data
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

            'PopulateDropDowns()

            ' Get Type
            Dim types As List(Of LookUpItem) = SpecCaseDao.GetSpecialCasePEPPTypes(SpecCase.Id)

            If (types IsNot Nothing AndAlso types.Count > 0) Then
                For Each t As LookUpItem In types
                    ddlType.SelectedValue = t.Value
                Next
            Else
                ddlType.SelectedIndex = 0
            End If

            ' Get Other Type Name
            If Not IsNothing(SpecCase.TypeName) Then
                txtEnterType.Text = Server.HtmlDecode(SpecCase.TypeName.ToString())
            End If

            ' Display the type text box if necessary
            OtherTypeRow.Visible = GetIsTypeOtherChecked()

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
            OtherRatingRow.Visible = GetIsRatingOtherSelected()

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

            UpdateCertificationStampControls()

            ' Get CompletedByGroup
            If SpecCase.CompletedByUnit.HasValue Then
                ' Check if the selected CompletedByGroup is Active or Inactive
                If (Not (ddlCompletedByUnit.Items.FindByValue(SpecCase.CompletedByUnit) Is Nothing)) Then
                    ddlCompletedByUnit.SelectedValue = SpecCase.CompletedByUnit
                Else
                    ' Inactive; therefore add the inactive CompletedByGroup to the bottom of the dropdown.
                    ' This is needed for older cases that might have been created using CompletedByGroups that are now inactive.
                    Dim item = New ListItem()

                    item.Text = CompletedByGroupDao.GetById(SpecCase.CompletedByUnit).Name
                    item.Value = SpecCase.CompletedByUnit
                    ddlCompletedByUnit.Items.Insert(ddlCompletedByUnit.Items.Count, item)
                    ddlCompletedByUnit.SelectedValue = SpecCase.CompletedByUnit
                End If
            End If

            If (Not String.IsNullOrEmpty(SpecCase.CompletedByUnitName)) Then
                txtCompletedByUnitName.Text = SpecCase.CompletedByUnitName
            End If

            ' Display the completed by group name text box if necessary
            OtherCompletedByUnitRow.Visible = GetIsCompletedByUnitOtherSelected()

            ' Get Date Received
            If SpecCase.DateReceived.HasValue Then
                txtDateReceived.Text = Server.HtmlDecode(SpecCase.DateReceived.Value.ToString(DATE_FORMAT))
            End If

            ' Get ALC
            If SpecCase.ALCLetterType.HasValue Then
                ddlALC.SelectedValue = SpecCase.ALCLetterType
            End If

            If (Not String.IsNullOrEmpty(SpecCase.FreeText)) Then
                txtFreeText.Text = Server.HtmlDecode(SpecCase.FreeText)
            End If

            If (Not String.IsNullOrEmpty(SpecCase.SecondaryFreeText)) Then
                txtSecondaryFreeText.Text = Server.HtmlDecode(SpecCase.SecondaryFreeText)
            End If

            MemoTemplateRow.Visible = GetIsMemoRowVisible()

            InitCertificationStampControls()

            If UserCanEdit Then
                'Read/Write
                EnableControls(isReassessmentCase)
            Else
                'Read Only
                DisableControls()
            End If

            FillMemoTemplatesList()

            If (SpecCase.MemoTemplateID.HasValue) Then
                ddlMemoTemplates.SelectedValue = SpecCase.MemoTemplateID.Value
            End If

            If Not String.IsNullOrEmpty(SpecCase.PocEmail) Then
                txtPOCEmailLabel.Text = SpecCase.PocEmail
            End If
            If Not String.IsNullOrEmpty(SpecCase.PocPhoneDSN) Then
                txtPOCPhoneLabel.Text = SpecCase.PocPhoneDSN
            End If
            If Not String.IsNullOrEmpty(SpecCase.PocRankAndName) Then
                txtPOCNameLabel.Text = SpecCase.PocRankAndName
            End If

        End Sub

        Private Sub PopulateDropDowns()

            Dim listTop = New ListItem()
            Dim lkupDAO As ILookupDao
            lkupDAO = New NHibernateDaoFactory().GetLookupDao()

            ' Populate the Type dropdown
            ddlType.DataSource = lkupDAO.GetPEPPTypes(0, 1)
            ddlType.DataTextField = "Name"
            ddlType.DataValueField = "Value"
            ddlType.DataBind()

            listTop.Text = "--- Select a Type ---"
            listTop.Value = 0
            ddlType.Items.Insert(0, listTop)

            ' Populate the Rating dropdown
            ddlRating.DataSource = lkupDAO.GetPEPPRatings(0, 1)
            ddlRating.DataTextField = "Name"
            ddlRating.DataValueField = "Value"
            ddlRating.DataBind()

            listTop = New ListItem()
            listTop.Text = "--- Select a Rating ---"
            listTop.Value = 0
            ddlRating.Items.Insert(0, listTop)

            ' Populate the Disposition dropdown
            ddlDisposition.DataSource = DispositionDao.GetWorkflowDispositions(AFRCWorkflows.SpecCaseRS)
            ddlDisposition.DataTextField = "Name"
            ddlDisposition.DataValueField = "Id"
            ddlDisposition.DataBind()

            listTop = New ListItem()
            listTop.Text = "--- Select a Disposition ---"
            listTop.Value = 0
            ddlDisposition.Items.Insert(0, listTop)

            ' Populate the Completed By Unit dropdown
            ddlCompletedByUnit.DataSource = CompletedByGroupDao.GetWorkflowCompletedByGroups(AFRCWorkflows.SpecCaseRS)
            ddlCompletedByUnit.DataTextField = "Name"
            ddlCompletedByUnit.DataValueField = "Id"
            ddlCompletedByUnit.DataBind()

            listTop = New ListItem()
            listTop.Text = "--- Select One ---"
            listTop.Value = 0
            ddlCompletedByUnit.Items.Insert(0, listTop)

            FillCaseTypeList()
            'FillMemoTemplatesList()

        End Sub

        Private Sub SaveFindings()

            If (Not UserCanEdit) Then
                Exit Sub
            End If

            ' Save Start Page data for reassessment cases...
            If (SCDao.GetIsReassessmentCase(SpecCase.Id)) Then
                If (ddlCaseType.Items IsNot Nothing AndAlso ddlCaseType.Items.Count > 0 AndAlso ddlCaseType.SelectedValue > 0) Then
                    SpecCase.CaseType = ddlCaseType.SelectedValue
                End If

                If (ddlSubCaseType.Items IsNot Nothing AndAlso ddlSubCaseType.Items.Count > 0 AndAlso ddlSubCaseType.SelectedValue > 0) Then
                    SpecCase.SubCaseType = ddlSubCaseType.SelectedValue
                End If

                If (GetIsCaseTypeOtherSelected()) Then
                    SpecCase.CaseTypeName = Server.HtmlEncode(txtOtherCaseTypeName.Text)
                End If

                If (GetIsSubCaseTypeOtherSelected()) Then
                    SpecCase.SubCaseTypeName = Server.HtmlEncode(txtOtherSubCaseTypeName.Text)
                End If
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
            If (GetIsRatingOtherSelected()) Then
                SpecCase.RatingName = Server.HtmlEncode(txtEnterRating.Text)
            Else
                SpecCase.RatingName = Server.HtmlEncode(String.Empty)
            End If

            ' Save Disposition
            If Not String.IsNullOrEmpty(ddlDisposition.SelectedValue) AndAlso ddlDisposition.SelectedValue > 0 Then
                SpecCase.Disposition = ddlDisposition.SelectedValue
            End If

            ' Save Completed By Unit
            If Not String.IsNullOrEmpty(ddlCompletedByUnit.SelectedValue) AndAlso ddlCompletedByUnit.SelectedValue > 0 Then
                SpecCase.CompletedByUnit = ddlCompletedByUnit.SelectedValue
            End If

            ' Save Other Completed By Unit
            If (GetIsCompletedByUnitOtherSelected()) Then
                SpecCase.CompletedByUnitName = Server.HtmlEncode(txtCompletedByUnitName.Text)
            Else
                SpecCase.CompletedByUnitName = Server.HtmlEncode(String.Empty)
            End If

            ' Save Date Recevied
            If Not String.IsNullOrEmpty(txtDateReceived.Text) Then
                SpecCase.DateReceived = Server.HtmlEncode(txtDateReceived.Text)
            End If

            ' Save ALC
            If Not String.IsNullOrEmpty(ddlALC.SelectedValue) Then
                SpecCase.ALCLetterType = ddlALC.SelectedValue
            End If

            ' Save Certification Stamp
            If (Not String.IsNullOrEmpty(ddlCertificationStamp.SelectedValue) AndAlso ddlCertificationStamp.SelectedValue > 0) Then
                SpecCase.CertificationStamp = ddlCertificationStamp.SelectedValue
            End If

            ' Save Free Text
            If (Not String.IsNullOrEmpty(txtFreeText.Text)) Then
                SpecCase.FreeText = Server.HtmlEncode(txtFreeText.Text)
            Else
                SpecCase.FreeText = Nothing
            End If

            ' Save Secondary Certification Stamp
            If (Not String.IsNullOrEmpty(ddlSecondaryCertificationStamp.SelectedValue)) Then
                SpecCase.SecondaryCertificationStamp = ddlSecondaryCertificationStamp.SelectedValue
            End If

            ' Save Secondary Free Text
            If (Not String.IsNullOrEmpty(txtSecondaryFreeText.Text)) Then
                SpecCase.SecondaryFreeText = Server.HtmlEncode(txtSecondaryFreeText.Text)
            Else
                SpecCase.SecondaryFreeText = Nothing
            End If

            ' Save Memo Template
            If (Not String.IsNullOrEmpty(ddlMemoTemplates.SelectedValue) AndAlso ddlMemoTemplates.SelectedValue <> 0) Then
                SpecCase.MemoTemplateID = Integer.Parse(ddlMemoTemplates.SelectedValue)
            End If

            If Not String.IsNullOrEmpty(txtPOCNameLabel.Text) Then
                SpecCase.PocRankAndName = Server.HtmlEncode(txtPOCNameLabel.Text)
            End If

            If Not String.IsNullOrEmpty(txtPOCPhoneLabel.Text) Then
                SpecCase.PocPhoneDSN = Server.HtmlEncode(txtPOCPhoneLabel.Text)
            End If

            If Not String.IsNullOrEmpty(txtPOCEmailLabel.Text) Then
                SpecCase.PocEmail = Server.HtmlEncode(txtPOCEmailLabel.Text)
            End If

            SCDao.SaveOrUpdate(SpecCase)
            SCDao.CommitChanges()

        End Sub

        Private Sub TurnSubCaseTypeRowOnOff(ByVal isOn As Boolean)
            utrSubCaseType.Visible = isOn
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

        Private Sub UpdateCertificationStampControls()
            Dim listTop = New ListItem()
            Dim source As String = String.Empty

            ' Determine datasource...
            'If (SpecCase.GetActiveBoardMedicalFinding().HasValue AndAlso SpecCase.GetActiveBoardMedicalFinding().Value = 1) Then
            '    source = "Qualified"
            'ElseIf (SpecCase.GetActiveBoardMedicalFinding().HasValue AndAlso SpecCase.GetActiveBoardMedicalFinding().Value = 0) Then
            '    source = "Disqualified"
            'Else
            If (ddlDisposition.SelectedItem.Text = "Qualified") Then
                source = "Qualified"
            ElseIf (ddlDisposition.SelectedItem.Text = "Disqualified") Then
                source = "Disqualified"
            Else
                source = "Other"
            End If

            If (source = "Qualified") Then
                ddlCertificationStamp.DataSource = CertificationStampDao.GetWorkflowCertificationStampsByDisposition(AFRCWorkflows.SpecCaseRS, True).Where(Function(x) Not x.Name.ToLower().Contains("profile")).ToList()
                ddlCertificationStamp.Enabled = (True And UserCanEdit)
                txtFreeText.Enabled = (True And UserCanEdit)
                trSecondaryCertificationStamp.Visible = True
                trSecondaryFreeText.Visible = True
                ddlSecondaryCertificationStamp.DataSource = CertificationStampDao.GetWorkflowCertificationStampsByDisposition(AFRCWorkflows.SpecCaseRS, True).Where(Function(x) x.Name.ToLower().Contains("profile")).ToList()
                ddlSecondaryCertificationStamp.Enabled = (True And UserCanEdit)
                txtSecondaryFreeText.Enabled = (True And UserCanEdit)
                lblMemoTemplateRow.Text = "L"
            ElseIf (source = "Disqualified") Then
                ddlCertificationStamp.DataSource = CertificationStampDao.GetWorkflowCertificationStampsByDisposition(AFRCWorkflows.SpecCaseRS, False)
                ddlCertificationStamp.Enabled = (True And UserCanEdit)
                txtFreeText.Enabled = (True And UserCanEdit)
                trSecondaryCertificationStamp.Visible = False
                trSecondaryFreeText.Visible = False
                ddlSecondaryCertificationStamp.Enabled = False
                txtSecondaryFreeText.Enabled = False
                lblMemoTemplateRow.Text = "J"
            Else
                ddlCertificationStamp.DataSource = New List(Of CertificationStamp)()
                ddlCertificationStamp.Enabled = False
                txtFreeText.Enabled = False
                trSecondaryCertificationStamp.Visible = False
                trSecondaryFreeText.Visible = False
                ddlSecondaryCertificationStamp.Enabled = False
                txtSecondaryFreeText.Enabled = False
                lblMemoTemplateRow.Text = "J"
            End If

            ddlCertificationStamp.DataTextField = "Name"
            ddlCertificationStamp.DataValueField = "Id"
            ddlCertificationStamp.DataBind()

            listTop.Text = "--- Select a Certification Stamp ---"
            listTop.Value = 0
            ddlCertificationStamp.Items.Insert(0, listTop)

            ' Set selected value...
            If (SpecCase.CertificationStamp.HasValue AndAlso ddlCertificationStamp.Items.FindByValue(SpecCase.CertificationStamp.Value) IsNot Nothing) Then
                ddlCertificationStamp.SelectedValue = SpecCase.CertificationStamp.Value
            Else
                ddlCertificationStamp.SelectedValue = 0
            End If

            If (source = "Qualified") Then
                ddlSecondaryCertificationStamp.DataTextField = "Name"
                ddlSecondaryCertificationStamp.DataValueField = "Id"
                ddlSecondaryCertificationStamp.DataBind()

                listTop = New ListItem()
                listTop.Text = "--- Select a Certification Stamp ---"
                listTop.Value = 0
                ddlSecondaryCertificationStamp.Items.Insert(0, listTop)

                ' Set selected value...
                If (SpecCase.SecondaryCertificationStamp.HasValue AndAlso ddlSecondaryCertificationStamp.Items.FindByValue(SpecCase.SecondaryCertificationStamp.Value) IsNot Nothing) Then
                    ddlSecondaryCertificationStamp.SelectedValue = SpecCase.SecondaryCertificationStamp.Value
                Else
                    ddlSecondaryCertificationStamp.SelectedValue = 0
                End If
            End If
        End Sub

    End Class

End Namespace