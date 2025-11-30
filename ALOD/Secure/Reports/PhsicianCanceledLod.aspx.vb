Imports ALOD.Core.Domain.Reports
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Data.Services
Imports ALOD.Web.UserControls
Imports ALODWebUtility.Common

Namespace Web.Reports

    Partial Class Secure_Reports_PhsicianCanceledLod
        Inherits System.Web.UI.Page

        Private Const ASCENDING As String = "Asc"
        Private Const DESCENDING As String = "Desc"
        Private Const SORT_COLUMN_KEY As String = "_SortExp_"
        Private Const SORT_DIR_KEY As String = "_SortDirection_"
        Private _reportsDao As IReportsDao

        Public ReadOnly Property SelectedOrder() As String
            Get
                Select Case rptNav.SortOrder
                    Case "SSN"
                        Return "ssn"
                    Case "UNIT"
                        Return "member_unit"
                    Case "LASTNAME"
                        Return "member_name"
                    Case Else
                        Return ""
                End Select
            End Get
        End Property

        Protected ReadOnly Property ReportsDao As IReportsDao
            Get
                If (_reportsDao Is Nothing) Then
                    _reportsDao = New NHibernateDaoFactory().GetReportsDao()
                End If

                Return _reportsDao
            End Get
        End Property

        Private Property SortColumn() As String
            Get
                If (ViewState(SORT_COLUMN_KEY) Is Nothing) Then
                    ViewState(SORT_COLUMN_KEY) = "member_name"
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

        Protected Function ConstructReportArgs(ByVal ssn As String) As LODPhysicianCancelledReportArgs
            Dim args As LODPhysicianCancelledReportArgs = New LODPhysicianCancelledReportArgs()

            args.UnitId = rptNav.Unit
            args.ReportView = SESSION_REPORT_VIEW
            args.SSN = ssn
            args.BeginDate = rptNav.BeginDate
            args.EndDate = rptNav.EndDate
            args.IncludeSubordinateUnits = rptNav.IncludeSubordinate

            Return args
        End Function

        Protected Function GetServiceMembersByName() As DataTable
            Return LookupService.GetServerMembersByName(rptNav.MemberLastName, rptNav.MemberFirstName, rptNav.MemberMiddleName)
        End Function

        Protected Sub grvCanceledLOD_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles grvCanceledLOD.PageIndexChanging
            grvCanceledLOD.PageIndex = e.NewPageIndex
            SortGridView(GetSelectedMemberSSN())
        End Sub

        Protected Sub grvCanceledLOD_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles grvCanceledLOD.RowDataBound
            Dim grid As GridView = CType(sender, GridView)

            If (e.Row.RowType = DataControlRowType.Header) Then
                Dim cellIndex As Integer = -1

                For Each field As DataControlField In grid.Columns
                    If (field.SortExpression = SortColumn) Then
                        cellIndex = grid.Columns.IndexOf(field)
                    End If
                Next

                If (cellIndex > -1) Then
                    If (SortDirection = ASCENDING) Then
                        e.Row.Cells(cellIndex).CssClass = "gridViewHeader sort-asc"
                    Else
                        e.Row.Cells(cellIndex).CssClass = "gridViewHeader sort-desc"
                    End If
                End If
            End If
        End Sub

        Protected Sub grvCanceledLOD_Sorting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewSortEventArgs) Handles grvCanceledLOD.Sorting
            grvCanceledLOD.PageIndex = 0
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

            If (rptNav.SSNRadioButtonChecked) Then
                SortGridView(rptNav.SSN)
            ElseIf (rptNav.MemberNameRadioButtonChecked) Then
                rptNav.InvalidMemberNameErrorLabelVisibility = False
                rptNav.MemberNotFoundErrorLabelVisibility = False

                If (rptNav.IsMemberNameInvalid) Then
                    rptNav.InvalidMemberNameErrorLabelVisibility = True
                    MemberSelectionPanel.Visible = False
                    ResultsPanel.Visible = False
                    Exit Sub
                End If

                Dim resultsTable As DataTable = GetServiceMembersByName()

                If (resultsTable.Rows.Count > 1) Then
                    ucMemberSelectionGrid.Initialize(resultsTable)
                    MemberSelectionPanel.Visible = True
                    ResultsPanel.Visible = False
                    Exit Sub
                ElseIf (resultsTable.Rows.Count = 1) Then
                    Session("rowIndex") = 0
                    SortGridView(resultsTable.Rows(0).Field(Of String)("SSN"))
                Else
                    rptNav.MemberNotFoundErrorLabelVisibility = True
                    Exit Sub
                End If
            End If
        End Sub

        Protected Sub InitEventHandlers()
            AddHandler rptNav.RptClicked, AddressOf RunReport
            AddHandler ucMemberSelectionGrid.MemberSelected, AddressOf MemberSelected
        End Sub

        Protected Sub InitReportNavControl()
            rptNav.ControlVisibility(Secure_Shared_UserControls_ReportNav.CtrlList.LODStatus, False)
        End Sub

        Protected Sub MemberSelected(ByVal sender As Object, ByVal e As MemberSelectedEventArgs)
            Dim resultsTable As DataTable = GetServiceMembersByName()
            Dim ssn As String = String.Empty

            Session("rowIndex") = e.SelectedRowIndex
            ssn = resultsTable.Rows(e.SelectedRowIndex).Field(Of String)("SSN")

            SortGridView(ssn)
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            InitEventHandlers()

            If (Not Page.IsPostBack) Then
                InitReportNavControl()
            End If
        End Sub

        Private Function GetSelectedMemberSSN() As String
            If (rptNav.SSNRadioButtonChecked) Then
                Return rptNav.SSN
            ElseIf (rptNav.MemberNameRadioButtonChecked) Then
                If (rptNav.IsMemberNameInvalid) Then
                    Return String.Empty
                End If

                If (Session("rowIndex") Is Nothing) Then
                    Return String.Empty
                End If

                Dim resultsTable As DataTable = GetServiceMembersByName()

                If (resultsTable.Rows.Count > 0) Then
                    Return resultsTable.Rows(Session("rowIndex")).Field(Of String)("SSN")
                Else
                    Return String.Empty
                End If
            End If

            Return String.Empty
        End Function

        Private Sub RunReport(ByVal sender As Object, ByVal e As EventArgs)
            If (Page.IsValid) Then
                grvCanceledLOD.Sort(SelectedOrder, WebControls.SortDirection.Ascending)
            End If
        End Sub

        Private Sub SortGridView(ByVal ssn As String)
            Dim ds As DataSet = ReportsDao.ExecuteLODPhysicianCancelledReport(ConstructReportArgs(ssn))
            Dim dv As New DataView(ds.Tables(0))

            If (SortColumn.Length > 0) Then
                dv.Sort = SortExpression
            End If

            grvCanceledLOD.DataSource = dv
            grvCanceledLOD.DataBind()

            ResultsPanel.Visible = True
            MemberSelectionPanel.Visible = False
        End Sub

    End Class

End Namespace