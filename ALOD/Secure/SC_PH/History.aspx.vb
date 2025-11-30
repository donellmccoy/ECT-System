Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.Modules.SpecialCases
Imports ALOD.Core.Domain.Users
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Logging
Imports ALOD.Web.UserControls
Imports ALODWebUtility.Common
Imports ALODWebUtility.Permission.Search

Namespace Web.Special_Case.PH

    Public Class History
        Inherits System.Web.UI.Page

        Private Const ASCENDING As String = "Asc"
        Private Const DESCENDING As String = "Desc"
        Private Const SORT_COLUMN_KEY As String = "_SortExp_"
        Private Const SORT_DIR_KEY As String = "_SortDirection_"
        Private _dphUser As AppUser
        Private _phDao As IPsychologicalHealthDao
        Private _scAccess As Dictionary(Of String, ALOD.Core.Domain.Workflow.PageAccessType)
        Private _userDao As IUserDao
        Private _workflowDao As IWorkflowDao
        Private dao As ISpecialCaseDAO
        Private sc As SC_PH = Nothing
        Private scId As Integer = 0

        ReadOnly Property SCDao() As ISpecialCaseDAO
            Get
                If (dao Is Nothing) Then
                    dao = New NHibernateDaoFactory().GetSpecialCaseDAO()
                End If

                Return dao
            End Get
        End Property

        Public ReadOnly Property Navigator() As TabNavigator
            Get
                Return MasterPage.Navigator
            End Get
        End Property

        Public ReadOnly Property PHDao As IPsychologicalHealthDao
            Get
                If (_phDao Is Nothing) Then
                    _phDao = New NHibernateDaoFactory().GetPsychologicalHealthDao()
                End If

                Return _phDao
            End Get
        End Property

        Public ReadOnly Property ReferenceId() As Integer
            Get
                Return Integer.Parse(Request.QueryString("refId"))
            End Get
        End Property

        Public ReadOnly Property TabControl() As TabControls
            Get
                Return Master.TabControl
            End Get
        End Property

        Protected ReadOnly Property MasterPage() As SC_PHMaster
            Get
                Dim master As SC_PHMaster = CType(Page.Master, SC_PHMaster)
                Return master
            End Get
        End Property

        Protected ReadOnly Property SpecCase() As SC_PH
            Get
                If (sc Is Nothing) Then
                    sc = SCDao.GetById(ReferenceId)
                End If

                Return sc
            End Get
        End Property

        Protected ReadOnly Property UserDao() As IUserDao
            Get
                If (_userDao Is Nothing) Then
                    _userDao = New NHibernateDaoFactory().GetUserDao()
                End If

                Return _userDao
            End Get
        End Property

        Protected ReadOnly Property WorkflowDao As IWorkflowDao
            Get
                If (_workflowDao Is Nothing) Then
                    _workflowDao = New NHibernateDaoFactory().GetWorkflowDao()
                End If

                Return _workflowDao
            End Get
        End Property

        Private ReadOnly Property DPHUser As AppUser
            Get
                Return SpecCase.DPHUser
            End Get
        End Property

        Private ReadOnly Property RefId() As Integer
            Get
                Return CInt(Request.QueryString("refId"))
            End Get
        End Property

#Region "Sort Properties..."

        Private Property SortColumn() As String
            Get
                If (ViewState(SORT_COLUMN_KEY) Is Nothing) Then
                    ViewState(SORT_COLUMN_KEY) = "Case_Id"
                End If

                Return ViewState(SORT_COLUMN_KEY)
            End Get
            Set(ByVal value As String)
                ViewState(SORT_COLUMN_KEY) = value
            End Set
        End Property

        Private Property SortDirection() As String
            Get
                If (ViewState(SORT_DIR_KEY) Is Nothing) Then
                    ViewState(SORT_DIR_KEY) = ASCENDING
                End If

                Return ViewState(SORT_DIR_KEY)
            End Get
            Set(ByVal value As String)
                ViewState(SORT_DIR_KEY) = value
            End Set
        End Property

        Private ReadOnly Property SortExpression() As String
            Get
                Return SortColumn + " " + SortDirection
            End Get
        End Property

#End Region

        Protected Sub gdvPHHistory_PageIndexChanging(sender As Object, e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles gdvPHHistory.PageIndexChanging
            gdvPHHistory.PageIndex = e.NewPageIndex
            PopulateWingPHHistoryGridView(DPHUser.CurrentUnitId)
        End Sub

        Protected Sub gdvPHHistory_Sorting(sender As Object, e As System.Web.UI.WebControls.GridViewSortEventArgs) Handles gdvPHHistory.Sorting
            gdvPHHistory.PageIndex = 0

            If (SortColumn <> "") Then
                If (SortColumn = e.SortExpression) Then
                    If SortDirection = ASCENDING Then
                        SortDirection = DESCENDING
                    Else
                        SortDirection = ASCENDING
                    End If
                Else
                    SortDirection = ASCENDING
                End If
            End If

            SortColumn = e.SortExpression

            PopulateWingPHHistoryGridView(DPHUser.CurrentUnitId)
        End Sub

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            AddHandler Me.Master.TabClick, AddressOf TabButtonClicked
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If (Not IsPostBack) Then
                TabControl.Item(NavigatorButtonType.Save).Visible = False

                InitControls()

                LogManager.LogAction(ModuleType.SpecCasePH, UserAction.ViewPage, RefId, "Viewed Page: PH History")
            End If
        End Sub

        Private Sub gdvPHHistory_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs) Handles gdvPHHistory.RowCommand
            If (e.CommandName = "view") Then
                Dim parts() As String = e.CommandArgument.ToString().Split(";")
                Dim strQuery As New StringBuilder()
                Dim args As New ItemSelectedEventArgs()
                args.RefId = parts(0)

                strQuery.Append("refId=" + CType(args.RefId, String))
                args.Type = parts(1)

                args.Url = GetWorkflowInitPageURL(args.Type, args.RefId)

                Session("RefId") = args.RefId
                Response.Redirect(args.Url)
            End If
        End Sub

        Private Sub InitControls()
            If (DPHUser Is Nothing) Then
                pnlWingPHHistory.Visible = False
                Exit Sub
            End If

            lblHistoryPanelTitle.Text = "History for the " & DPHUser.CurrentUnitName
            PopulateWingPHHistoryGridView(DPHUser.CurrentUnitId)
        End Sub

        Private Sub PopulateWingPHHistoryGridView(ByVal wingId As Integer)
            If (wingId <= 0) Then
                gdvPHHistory.Visible = False
                lblNoHistory.Visible = True
                Exit Sub
            End If

            Dim dSet As DataSet = PHDao.GetCasesByCaseUnit(wingId)

            If (dSet Is Nothing OrElse dSet.Tables.Count = 0) Then
                gdvPHHistory.Visible = False
                lblNoHistory.Visible = True
                Exit Sub
            End If

            gdvPHHistory.Visible = True
            lblNoHistory.Visible = False

            Dim dvSort As DataView = dSet.Tables(0).DefaultView
            dvSort.Sort = SortExpression

            gdvPHHistory.DataSource = dvSort
            gdvPHHistory.DataBind()
        End Sub

#Region "TabEvent"

        Protected Sub Save_Click(ByRef sender As Object, ByRef e As TabNavigationEventArgs)
            If (Navigator.CurrentStep.IsReadOnly) Then
                Exit Sub
            End If

            If (e.ButtonType = NavigatorButtonType.Save OrElse
                e.ButtonType = NavigatorButtonType.NavigatedAway OrElse
                e.ButtonType = NavigatorButtonType.NextStep OrElse
                e.ButtonType = NavigatorButtonType.PreviousStep) Then
            End If
        End Sub

        Private Sub TabButtonClicked(ByRef sender As Object, ByRef e As TabNavigationEventArgs)
            If (e.ButtonType = NavigatorButtonType.Save OrElse
                e.ButtonType = NavigatorButtonType.NavigatedAway OrElse
                e.ButtonType = NavigatorButtonType.NextStep OrElse
                e.ButtonType = NavigatorButtonType.PreviousStep) Then
            End If
        End Sub

#End Region

    End Class

End Namespace