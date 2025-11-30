Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.Lookup
Imports ALOD.Core.Domain.Modules.SpecialCases
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Core.Utils
Imports ALOD.Data
Imports ALOD.Logging
Imports ALOD.Web.UserControls
Imports ALODWebUtility.Common

Namespace Web.Special_Case.MH

    Partial Class Secure_sc_mh_HQTech
        Inherits System.Web.UI.Page

#Region "MHProperty"

        Private _scAccess As Dictionary(Of String, ALOD.Core.Domain.Workflow.PageAccessType)
        Dim dao As ISpecialCaseDAO

        Private sc As SC_MH = Nothing
        Private scId As Integer = 0

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

        Protected ReadOnly Property CalendarImage() As String
            Get
                Return Me.ResolveClientUrl("~/App_Themes/" + Page.Theme + "/Images/Calendar.gif")
            End Get
        End Property

        Protected ReadOnly Property SpecCase() As SC_MH
            Get
                If (sc Is Nothing) Then
                    sc = SCDao.GetById(ReferenceId)
                End If

                Return sc
            End Get
        End Property

#End Region

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

        Protected ReadOnly Property MasterPage() As SC_MHMaster
            Get
                Dim master As SC_MHMaster = CType(Page.Master, SC_MHMaster)
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

        Protected Sub DisplayReadOnly(ByVal ShowStoppers As Integer)

            ucICDCodeControl.DisplayReadOnly(True)
            ucICD7thCharacterControl.DisplayReadOnly()
            footnote.Visible = False
            DiagnosisTextBox.ReadOnly = True
            txtHTD.Enabled = False

        End Sub

        Protected Sub DisplayReadWrite()

            ucICDCodeControl.DisplayReadWrite(False)
            ucICD7thCharacterControl.DisplayReadWrite()
            txtHTD.CssClass = "datePickerAll"

        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            ucICDCodeControl.Initialilze(Me)
            ucICD7thCharacterControl.Initialize(ucICDCodeControl)

            If (Not IsPostBack) Then
                UserCanEdit = GetAccess(Navigator.PageAccess, True)
                InitControls()
                SetMaxLength(DiagnosisTextBox)
            End If

            LogManager.LogAction(ModuleType.SpecCaseMH, UserAction.ViewPage, RefId, "Viewed Page: MH HQ AFRC Tech")

            If Session(SESSIONKEY_COMPO) = "5" Then
                lblHeader.Text = "ANGRC Technician - MH"
            End If

        End Sub

        Private Sub InitControls()

            Dim lkupDAO As ILookupDao
            lkupDAO = New NHibernateDaoFactory().GetLookupDao()

            SetInputFormatRestriction(Page, txtHTD, FormatRestriction.Numeric, "/")
            SetInputFormatRestriction(Page, DiagnosisTextBox, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)

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

            If Not IsNothing(SpecCase.HighTenureDate) Then
                txtHTD.Text = Server.HtmlDecode(SpecCase.HighTenureDate)
            End If

            DisplayReadWrite()

            If Not UserCanEdit Then
                DisplayReadOnly(0)
            End If

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

            If Not String.IsNullOrEmpty(txtHTD.Text) Then
                SpecCase.HighTenureDate = Server.HtmlEncode(txtHTD.Text)
            End If

            SCDao.SaveOrUpdate(SpecCase)
            SCDao.CommitChanges()

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