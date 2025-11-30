Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Web.UserControls

Namespace Web.Reports

    Partial Class Secure_Reports_RFAByGroup
        Inherits System.Web.UI.Page

        Private Const ASCENDING As String = "Asc"
        Private Const DESCENDING As String = "Desc"
        Private Const SORT_COLUMN_KEY As String = "_SortExp_"
        Private Const SORT_DIR_KEY As String = "_SortDirection_"
        Private _reportsDao As IReportsDao

        Protected ReadOnly Property BeginDate() As Nullable(Of Date)
            Get
                Return rptNav.BeginDate
            End Get
        End Property

        Protected ReadOnly Property EndDate() As Nullable(Of Date)
            Get
                Return rptNav.EndDate
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
                    ViewState(SORT_COLUMN_KEY) = "Unit"
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

        Protected Sub rfaByGrpRptGrid_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles rfaByGrpRptGrid.PageIndexChanging
            rfaByGrpRptGrid.PageIndex = e.NewPageIndex
            FillGrid()
        End Sub

        Protected Sub rfaByGrpRptGrid_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles rfaByGrpRptGrid.RowDataBound
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

        Protected Sub rfaByGrpRptGrid_Sorting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewSortEventArgs) Handles rfaByGrpRptGrid.Sorting
            rfaByGrpRptGrid.PageIndex = 0
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
            FillGrid()
        End Sub

        Private Sub FillGrid()
            Dim ds As DataSet = ReportsDao.ExecuteRFAByGroupReport(BeginDate, EndDate)
            Dim view As DataView = New DataView(ds.Tables(0))

            If (SortColumn.Length > 0) Then
                view.Sort = SortExpression
            End If

            rfaByGrpRptGrid.DataSource = view
            rfaByGrpRptGrid.DataBind()
        End Sub

        Private Sub RunReport(ByVal sender As Object, ByVal e As EventArgs)
            If (Page.IsValid) Then
                FillGrid()
            End If
        End Sub

    End Class

End Namespace