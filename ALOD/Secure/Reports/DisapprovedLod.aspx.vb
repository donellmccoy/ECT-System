Imports ALOD.Core.Domain.Reports
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Web.UserControls

Namespace Web.Reports

    Partial Class Secure_Reports_DisapprovedLod
        Inherits System.Web.UI.Page

#Region "fields/property"

        Protected _reportsDao As IReportsDao

        Private _eptsTotal As Integer = 0
        Private _nIlodNotTotal As Integer = 0
        Private _nIlodTotal As Integer = 0
        Private _RowTotal As Integer = 0

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

#Region "Sort"

        Private Const ASCENDING As String = "Asc"
        Private Const DESCENDING As String = "Desc"
        Private Const DET_SORT_COLUMN_KEY As String = "_DetSortExp_"
        Private Const DET_SORT_DIR_KEY As String = "_DetSortDirection_"

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

#End Region

#End Region

        Protected Function ConstructReportArguments(ByVal pType As Integer, ByVal finding As Integer) As LODDisapprovedReportArgs
            Dim args As LODDisapprovedReportArgs = New LODDisapprovedReportArgs()

            args.UserId = Integer.Parse(Session("UserId"))
            args.BeginDate = rptNav.BeginDate
            args.EndDate = rptNav.EndDate
            args.PType = pType
            args.Finding = finding

            Return args
        End Function

        Protected Sub DisapprovedGridView_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles DisapprovedGridView.RowCommand
            If (e.CommandName = "getDetail") Then
                rowDetail.Visible = True
                Dim row() As String = e.CommandArgument.ToString.Split(",")
                ViewState("pTypeId") = row(0)
                ViewState("finding") = row(1)

                DetailsGroupLabel.Text = row(2)
                DetailsReasonLabel.Text = row(3)
                'Since already set the pTypeId and Finding
                SortDetailGridView()
            End If
        End Sub

        Protected Sub DisapprovedGridView_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles DisapprovedGridView.RowDataBound
            If (e.Row.RowType = DataControlRowType.DataRow) Then
                Dim data As System.Data.DataRowView = e.Row.DataItem
                _RowTotal = data("eptCount") + data("nILodCount") + data("nIlodNotCount")
                _eptsTotal += data("eptCount")
                _nIlodTotal += data("nILodCount")
                _nIlodNotTotal += data("nIlodNotCount")
                Dim totalDisplay As Label = CType(e.Row.FindControl("lblTotal"), Label)
                totalDisplay.Text = _RowTotal

            ElseIf (e.Row.RowType = DataControlRowType.Footer) Then
                e.Row.Cells(0).Text = "Total:"
                e.Row.Cells(1).Text = _eptsTotal
                e.Row.Cells(2).Text = _nIlodTotal
                e.Row.Cells(3).Text = _nIlodNotTotal
                e.Row.Cells(4).Text = _eptsTotal + _nIlodTotal + _nIlodNotTotal
            End If
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If (Not Page.IsPostBack) Then
                rptNav.ControlVisibility(Secure_Shared_UserControls_ReportNav.CtrlList.LODStatus, False)
                rptNav.ControlVisibility(Secure_Shared_UserControls_ReportNav.CtrlList.subordinate, False)
                rptNav.ControlVisibility(Secure_Shared_UserControls_ReportNav.CtrlList.SSN, False)
                rptNav.ControlVisibility(Secure_Shared_UserControls_ReportNav.CtrlList.Unit, False)
                rptNav.ControlVisibility(Secure_Shared_UserControls_ReportNav.CtrlList.SortOrder, False)

            End If
            AddHandler rptNav.RptClicked, AddressOf RunReport

        End Sub

        Private Sub RunReport(ByVal sender As Object, ByVal e As EventArgs)
            If (Page.IsValid) Then
                rowDetail.Visible = False

                Dim ds As DataSet = ReportsDao.ExecuteLODDisapprovedReport(ConstructReportArguments(0, 0))

                DisapprovedGridView.DataSource = ds
                DisapprovedGridView.DataBind()
            End If
        End Sub

#Region "DetailView"

        Protected Sub DetailGrid_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles DetailGrid.PageIndexChanging
            DetailGrid.PageIndex = e.NewPageIndex
            SortDetailGridView()
        End Sub

        Protected Sub DetailGrid_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles DetailGrid.RowDataBound

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

        Private Sub FillDetailGrid(ByVal pTypeId As Integer, ByVal finding As Integer)
            DetailGrid.DataSource = ReportsDao.ExecuteLODDisapprovedDetailsReport(ConstructReportArguments(pTypeId, finding))
            DetailGrid.DataBind()
        End Sub

        Private Sub SortDetailGridView()
            FillDetailGrid(ViewState("pTypeId"), ViewState("finding"))
            Dim dt As System.Data.DataTable = CType(DetailGrid.DataSource, System.Data.DataSet).Tables(0)

            Dim dv As New System.Data.DataView(dt)

            If (DetailSortColumn.Length > 0) Then
                dv.Sort = DetailSortExpression
            End If

            DetailGrid.DataSource = dv
            DetailGrid.DataBind()

        End Sub

#End Region

    End Class

End Namespace