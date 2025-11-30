Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Modules.SpecialCases
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Core.Utils
Imports ALOD.Data
Imports ALOD.Web.UserControls
Imports ALODWebUtility.Common

Namespace Web.Special_Case.IRILO

    Partial Class Secure_sc_FastTrack_HQTech
        Inherits System.Web.UI.Page

#Region "FastTrackProperty"

        Private _scAccess As Dictionary(Of String, ALOD.Core.Domain.Workflow.PageAccessType)
        Dim dao As ISpecialCaseDAO

        Private sc As SC_FastTrack = Nothing
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

        Protected ReadOnly Property SpecCase() As SC_FastTrack
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

        Protected ReadOnly Property MasterPage() As SC_FastTrackMaster
            Get
                Dim master As SC_FastTrackMaster = CType(Page.Master, SC_FastTrackMaster)
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

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If (Not IsPostBack) Then
                UserCanEdit = GetAccess(Navigator.PageAccess, True)
                InitControls()
                SetMaxLength(DecisionComment)
            End If

            If Session(SESSIONKEY_COMPO) = "5" Then
                lblHeader.Text = "ANGRC Technician Decision"
            End If

        End Sub

        Private Sub InitControls()

            SetInputFormatRestriction(Page, DecisionComment, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)

            'Load Findings
            If SpecCase.hqt_approval1 = 1 Then
                DecisionY.Checked = True
            End If
            If SpecCase.hqt_approval1 = 0 Then
                DecisionN.Checked = True
            End If

            If Not IsNothing(SpecCase.hqt_approval1) Then
                DecisionComment.Text = Server.HtmlDecode(SpecCase.hqt_approval1_comment.ToString())
            End If

            If UserCanEdit Then
                DecisionY.Enabled = True
                DecisionN.Enabled = True
                DecisionComment.Enabled = True
            Else
                DecisionY.Enabled = False
                DecisionN.Enabled = False
                DecisionComment.Enabled = False
            End If
        End Sub

        Private Sub SaveFindings()

            If (Not UserCanEdit) Then
                Exit Sub
            End If

            Dim PageAccess As ALOD.Core.Domain.Workflow.PageAccessType
            PageAccess = SectionList(FTSectionNames.FT_HQT_INIT.ToString())

            If SpecCase.Status = SpecCaseFTWorkStatus.InitiateCase Then
                If DecisionN.Checked Then
                    SpecCase.hqt_approval1 = 0
                End If
                If DecisionY.Checked Then
                    SpecCase.hqt_approval1 = 1
                End If

                SpecCase.hqt_approval1_comment = Server.HtmlEncode(DecisionComment.Text)

            End If

            SCDao.SaveOrUpdate(SpecCase)
            SCDao.CommitChanges()
        End Sub

#Region "TabEvent"

        Public Function ValidBoxLength() As Boolean
            Dim IsValid As Boolean = True
            If Not CheckTextLength(DecisionComment) Then
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