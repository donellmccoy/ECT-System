Imports ALOD.Core.Domain.Reports
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Web.UserControls

Namespace Web.Reports

    Partial Class Secure_Reports_Rowa
        Inherits System.Web.UI.Page

        Private Const ASCENDING As String = "Asc"
        Private Const DESCENDING As String = "Desc"
        Private Const DET_SORT_COLUMN_KEY As String = "_DetSortExp_"
        Private Const DET_SORT_DIR_KEY As String = "_DetSortDirection_"
        Private _countRow As Integer = 0
        Private _reportsDao As IReportsDao

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

        Protected Function ConstructReportsArgs(ByVal rwoaId As Integer) As LODRWOAReportArgs
            Dim args As LODRWOAReportArgs = New LODRWOAReportArgs()

            args.RWOAId = rwoaId
            args.BeginDate = rptNav.BeginDate
            args.EndDate = rptNav.EndDate
            args.Compo = rptNav.GetCompo

            Return args
        End Function

        Protected Sub DetailGrid_Sorting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewSortEventArgs) Handles GridView2.Sorting
            GridView2.PageIndex = 0
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

            SortGridView()
        End Sub

        Protected Sub GridView1_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles GridView1.RowCommand
            If (e.CommandName.Equals("getDetail")) Then
                Dim args() As String = e.CommandArgument.ToString.Split(";")
                ViewState("csid") = CType(args(0), Integer)
                rowaDetail.Visible = True
                DetailsReasonLabel.Text = args(1)
                FillDetailGrid(CType(args(0), Integer))
            End If
        End Sub

        Protected Sub GridView1_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridView1.RowDataBound
            If (e.Row.RowType = DataControlRowType.DataRow) Then
                Dim data As System.Data.DataRowView = e.Row.DataItem
                _countRow += data("TotalCount")
            ElseIf (e.Row.RowType = DataControlRowType.Footer) Then
                e.Row.Cells(0).Text = "Total:"
                e.Row.Cells(1).Text = _countRow
            End If
        End Sub

        Protected Sub GridView2_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles GridView2.PageIndexChanging
            GridView2.PageIndex = e.NewPageIndex
            SortGridView()
        End Sub

        Protected Sub GridView2_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridView2.RowDataBound
            Dim grid As GridView = CType(sender, GridView)

            If (e.Row.RowType = DataControlRowType.Header) Then
                Dim cellIndex As Integer = -1

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
        End Sub

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

        Private Sub FillDetailGrid(ByVal rwoaId As Integer)
            GridView2.DataSource = ReportsDao.ExecuteLODRWOADetailsReport(ConstructReportsArgs(rwoaId))
            GridView2.DataBind()
        End Sub

        Private Sub FillGrid()
            Dim ds As DataSet = ReportsDao.ExecuteLODRWOACountReport(ConstructReportsArgs(0))
            GridView1.DataSource = ds
            GridView1.DataBind()
        End Sub

        Private Sub RunReport(ByVal sender As Object, ByVal e As EventArgs)
            If (Page.IsValid) Then
                rowaCount.Visible = True
                rowaDetail.Visible = False
                FillGrid()
            End If
        End Sub

        Private Sub SortGridView()
            FillDetailGrid(CType(ViewState("csid"), Integer))
            Dim dt As System.Data.DataTable = CType(GridView2.DataSource, System.Data.DataSet).Tables(0)

            Dim dv As New System.Data.DataView(dt)

            If (DetailSortColumn.Length > 0) Then
                dv.Sort = DetailSortExpression
            End If

            GridView2.DataSource = dv
            GridView2.DataBind()
        End Sub

    End Class

End Namespace