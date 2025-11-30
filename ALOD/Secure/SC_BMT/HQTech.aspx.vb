Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Modules.SpecialCases
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Core.Utils
Imports ALOD.Data
Imports ALOD.Logging
Imports ALOD.Web.UserControls
Imports ALODWebUtility.Common

Namespace Web.Special_Case.BMT

    Partial Class Secure_sc_bmt_HQTech
        Inherits System.Web.UI.Page

#Region "BMTProperty"

        Private _scAccess As Dictionary(Of String, ALOD.Core.Domain.Workflow.PageAccessType)
        Dim dao As ISpecialCaseDAO

        Private sc As SC_BMT = Nothing
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

        Protected ReadOnly Property SpecCase() As SC_BMT
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

        Protected ReadOnly Property MasterPage() As SC_BMTMaster
            Get
                Dim master As SC_BMTMaster = CType(Page.Master, SC_BMTMaster)
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

            'Disable the edit controls
            CommentsTB.ReadOnly = True

        End Sub

        Protected Sub DisplayReadWrite()

            CommentsTB.ReadOnly = False
            CommentsTB.Text = Server.HtmlDecode(SpecCase.CaseComments)

        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If (Not IsPostBack) Then
                UserCanEdit = GetAccess(Navigator.PageAccess, True)
                InitControls()
                SetMaxLength(CommentsTB)
                SetInputFormatRestriction(Page, CommentsTB, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)

                LogManager.LogAction(ModuleType.SpecCaseBMT, UserAction.ViewPage, RefId, "Viewed Page: BT HQ AFRC Tech")

            End If

        End Sub

        Private Sub InitControls()
            If (IsPostBack) Then
                'no need to populate these more than once
                Exit Sub
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

            Dim PageAccess As ALOD.Core.Domain.Workflow.PageAccessType
            PageAccess = SectionList(BTSectionNames.BT_HQT_INIT.ToString())

            If SpecCase.Status = SpecCaseBMTWorkStatus.InitiateCase Then
                SpecCase.CaseComments = Server.HtmlEncode(CommentsTB.Text)

            End If

            SCDao.SaveOrUpdate(SpecCase)
            SCDao.CommitChanges()
        End Sub

#Region "TabEvent"

        Public Function ValidBoxLength() As Boolean
            Dim IsValid As Boolean = True
            If Not CheckTextLength(CommentsTB) Then
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