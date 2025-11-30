Imports ALOD.Core.Domain.Reports
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Web.UserControls

Namespace Web.Reports

    Partial Class Secure_Reports_LodStatistics
        Inherits System.Web.UI.Page

        Private Const AA_DECISION_HDR As String = "AA Decision"
        Private Const ASCENDING As String = "Asc"
        Private Const CONFLICT_RLBAA As String = "conflictRLBAA"
        Private Const CONFLICT_WCAA As String = "conflictWCAA"
        Private Const CONFLICT_WCRLB As String = "conflictWCRLB"
        Private Const DATE_GRID_COL As Integer = 3
        Private Const DECISION_LABEL_A As String = "DecisionaALabel"
        Private Const DECISION_LABEL_B As String = "DecisionaBLabel"
        Private Const DECISIONA_GRID_COL As Integer = 4
        Private Const DECISIONB_GRID_COL As Integer = 5
        Private Const DESCENDING As String = "Desc"
        Private Const DET_SORT_COLUMN_KEY As String = "_DetSortExp_"
        Private Const DET_SORT_DIR_KEY As String = "_DetSortDirection_"
        Private Const FINDING_COL_A As String = "findingA"
        Private Const FINDING_COL_B As String = "findingB"
        Private Const HDR_TEXT_LOD_CLOSED As String = "Date Closed"
        Private Const HDR_TEXT_LOD_INITIATED As String = "Date Initiated"
        Private Const KEY_CATEGORY As String = "category"
        Private Const LOD_INITIATED As String = "lodCount"
        Private Const RLB_DECISION_HDR As String = "RLB Decision"
        Private Const WC_DECISION_HDR As String = "WC Decision"
        Private _initiatedCount As Integer = 0
        Private _reportsDao As IReportsDao
        Private _rlbaaCount As Integer = 0
        Private _rlbClosedCount As Integer = 0
        Private _wcaaCount As Integer = 0
        Private _wcrlbCount As Integer = 0
        Private _wingClosedCount As Integer = 0

        Protected ReadOnly Property CalendarImage() As String
            Get
                Return Me.ResolveClientUrl("~/App_Themes/" + Page.Theme + "/Images/Calendar.gif")
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

        Private Property DetailSortColumn() As String
            Get
                If (ViewState(DET_SORT_COLUMN_KEY) Is Nothing) Then
                    ViewState(DET_SORT_COLUMN_KEY) = "member_name"
                End If

                Return ViewState(DET_SORT_COLUMN_KEY)
            End Get
            Set(ByVal value As String)
                ViewState(DET_SORT_COLUMN_KEY) = value
            End Set
        End Property

        Private Property DetailSortDirection() As String
            Get
                If (ViewState(DET_SORT_DIR_KEY) Is Nothing) Then
                    ViewState(DET_SORT_DIR_KEY) = ASCENDING
                End If

                Return ViewState(DET_SORT_DIR_KEY)
            End Get
            Set(ByVal value As String)
                ViewState(DET_SORT_DIR_KEY) = value
            End Set
        End Property

        Private ReadOnly Property DetailSortExpression() As String
            Get
                Return DetailSortColumn + " " + DetailSortDirection
            End Get
        End Property

        Private ReadOnly Property ShowDecision() As Boolean
            Get
                If (GetIsConflictCategory()) Then
                    Return True
                Else
                    Return False
                End If
            End Get
        End Property

        Protected Function ConstructDetailsReportArgs() As LODStatisticsReportArgs
            Dim args As LODStatisticsReportArgs = New LODStatisticsReportArgs()

            args.UserId = Session("UserId")
            args.BeginDate = ViewState("startDate")
            args.EndDate = ViewState("endDate")
            args.Category = ViewState("category")

            Return args
        End Function

        Protected Function ConstructReportArgs() As LODStatisticsReportArgs
            Dim args As LODStatisticsReportArgs = New LODStatisticsReportArgs()

            args.UserId = Session("UserId")
            args.BeginDate = rptNav.BeginDate
            args.EndDate = rptNav.EndDate

            Return args
        End Function

        Protected Sub DetailGrid_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles DetailGrid.PageIndexChanging
            DetailGrid.PageIndex = e.NewPageIndex
            SortDetailGridView()
        End Sub

        Protected Sub DetailGrid_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles DetailGrid.RowDataBound
            Dim grid As GridView = CType(sender, GridView)

            If (e.Row.RowType = DataControlRowType.Header) Then
                Dim cellIndex As Integer = -1
                Dim headerText1 As String = String.Empty
                Dim headerText2 As String = String.Empty

                If ViewState(KEY_CATEGORY) = CONFLICT_RLBAA Then
                    headerText1 = RLB_DECISION_HDR
                    headerText2 = AA_DECISION_HDR
                End If

                If ViewState(KEY_CATEGORY) = CONFLICT_WCAA Then
                    headerText1 = WC_DECISION_HDR
                    headerText2 = AA_DECISION_HDR
                End If

                If ViewState(KEY_CATEGORY) = CONFLICT_WCRLB Then
                    headerText1 = WC_DECISION_HDR
                    headerText2 = RLB_DECISION_HDR
                End If

                Dim decisionAfld As DataControlField
                Dim decisionBfld As DataControlField
                Dim datefld As DataControlField

                decisionAfld = grid.Columns(DECISIONA_GRID_COL)
                decisionBfld = grid.Columns(DECISIONB_GRID_COL)
                datefld = grid.Columns(DATE_GRID_COL)

                If ShowDecision Then
                    decisionAfld.Visible = True
                    decisionBfld.Visible = True
                    decisionAfld.SortExpression = FINDING_COL_A
                    decisionAfld.HeaderText = headerText1
                    decisionBfld.SortExpression = FINDING_COL_B
                    decisionBfld.HeaderText = headerText2
                Else
                    decisionAfld.Visible = False
                    decisionBfld.Visible = False
                End If

                If (ViewState(KEY_CATEGORY) = LOD_INITIATED) Then
                    datefld.HeaderText = HDR_TEXT_LOD_INITIATED
                Else
                    datefld.HeaderText = HDR_TEXT_LOD_CLOSED
                End If

                'Sorting
                For Each field As DataControlField In grid.Columns
                    If (field.SortExpression = DetailSortColumn) Then
                        cellIndex = grid.Columns.IndexOf(field)
                    End If
                Next
                If (cellIndex > -1) Then
                    If (DetailSortDirection = ASCENDING) Then
                        e.Row.Cells(cellIndex).CssClass = "gridViewHeader sort-asc"
                    Else
                        e.Row.Cells(cellIndex).CssClass = "gridViewHeader sort-desc"
                    End If
                End If
            End If

            'If the category is conflict then only the data will be  be extracted from the database
            If (ShowDecision) Then
                If e.Row.RowType = DataControlRowType.DataRow Then
                    Dim data As System.Data.DataRowView = e.Row.DataItem
                    Dim DecisionLableA As Label = CType(e.Row.FindControl(DECISION_LABEL_A), Label)
                    Dim DecisionLableB As Label = CType(e.Row.FindControl(DECISION_LABEL_B), Label)
                    DecisionLableA.Text = data(FINDING_COL_A)
                    DecisionLableB.Text = data(FINDING_COL_B)
                End If
            End If
        End Sub

        Protected Sub DetailGrid_Sorting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewSortEventArgs) Handles DetailGrid.Sorting
            DetailGrid.PageIndex = 0
            If (DetailSortColumn <> "") Then
                If (DetailSortColumn = e.SortExpression) Then
                    If DetailSortDirection = ASCENDING Then
                        DetailSortDirection = DESCENDING
                    Else
                        DetailSortDirection = ASCENDING
                    End If
                Else
                    DetailSortDirection = ASCENDING
                End If
            End If

            DetailSortColumn = e.SortExpression

            SortDetailGridView()
        End Sub

        Protected Function GetIsConflictCategory() As Boolean
            If (ViewState(KEY_CATEGORY) = CONFLICT_RLBAA OrElse ViewState(KEY_CATEGORY) = CONFLICT_WCRLB OrElse ViewState(KEY_CATEGORY) = CONFLICT_WCAA) Then
                Return True
            Else
                Return False
            End If
        End Function

        Protected Sub InitReportNavControl()
            rptNav.ControlVisibility(Secure_Shared_UserControls_ReportNav.CtrlList.LODStatus, False)
            rptNav.ControlVisibility(Secure_Shared_UserControls_ReportNav.CtrlList.subordinate, False)
            rptNav.ControlVisibility(Secure_Shared_UserControls_ReportNav.CtrlList.SSN, False)
            rptNav.ControlVisibility(Secure_Shared_UserControls_ReportNav.CtrlList.Unit, False)
            rptNav.ControlVisibility(Secure_Shared_UserControls_ReportNav.CtrlList.SortOrder, False)
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            AddHandler rptNav.RptClicked, AddressOf RunReport

            If (Not Page.IsPostBack) Then
                InitReportNavControl()
            End If
        End Sub

        Protected Sub StatGrid_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles StatGrid.RowCommand
            Dim gvRow As GridViewRow = CType((CType(e.CommandSource, Control).NamingContainer), GridViewRow)

            If (e.CommandName = "getDetail") Then
                Dim row() As String = e.CommandArgument.ToString.Split(";")
                DetailDiv.Visible = True
                ViewState("startDate") = row(0)
                ViewState("endDate") = row(1)
                ViewState("category") = row(2)
                DetailsMonthLabel.Text = gvRow.Cells(0).Text
                DetailsCatLabel.Text = row(3)
                SortDetailGridView()
            End If
        End Sub

        Protected Sub StatGrid_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles StatGrid.RowDataBound
            If (e.Row.RowType = DataControlRowType.DataRow) Then
                Dim data As System.Data.DataRowView = e.Row.DataItem

                _initiatedCount += data("lodInitiated")
                _wingClosedCount += data("wingClosed")
                _rlbClosedCount += data("rlpClosed")
                _wcrlbCount += data("wcrlb")
                _wcaaCount += data("wcaa")
                _rlbaaCount += data("rlbaa")
            ElseIf (e.Row.RowType = DataControlRowType.Footer) Then
                e.Row.Cells(0).Text = "Total:"
                e.Row.Cells(1).Text = _initiatedCount
                e.Row.Cells(2).Text = _wingClosedCount
                e.Row.Cells(3).Text = _rlbClosedCount
                e.Row.Cells(4).Text = _wcrlbCount
                e.Row.Cells(5).Text = _wcaaCount
                e.Row.Cells(6).Text = _rlbaaCount
            End If
        End Sub

        Private Sub FillDetailGrid(ByVal startDate As Date, ByVal endDate As Date, ByVal category As String)
            Dim ds As DataSet = ReportsDao.ExecuteLODStatisticsDetailsReport(ConstructDetailsReportArgs())

            DetailGrid.DataSource = ds
            DetailGrid.DataBind()
        End Sub

        Private Sub FillGrid()
            Dim ds As DataSet = ReportsDao.ExecuteLODStatisticsReport(ConstructReportArgs())

            StatGrid.DataSource = ds
            StatGrid.DataBind()
        End Sub

        Private Sub RunReport(ByVal sender As Object, ByVal e As EventArgs)
            If (Page.IsValid) Then
                DetailDiv.Visible = False
                FillGrid()
            End If
        End Sub

        Private Sub SortDetailGridView()
            FillDetailGrid(ViewState("startDate"), ViewState("endDate"), ViewState("category"))

            Dim dt As System.Data.DataTable = CType(DetailGrid.DataSource, System.Data.DataSet).Tables(0)
            Dim dv As New System.Data.DataView(dt)

            If (DetailSortColumn.Length > 0) Then
                If Not dt.Columns.Contains(DetailSortColumn) Then
                    ViewState(DET_SORT_COLUMN_KEY) = Nothing
                End If
            End If

            dv.Sort = DetailSortExpression
            DetailGrid.DataSource = dv
            DetailGrid.DataBind()
        End Sub

    End Class

End Namespace