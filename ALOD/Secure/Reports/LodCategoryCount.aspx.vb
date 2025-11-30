Imports ALOD.Core.Domain.Reports
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Data.Services
Imports ALOD.Web.UserControls
Imports ALODWebUtility.Common

Namespace Web.Reports

    Partial Class Secure_Reports_LodCategoryCount
        Inherits System.Web.UI.Page

#Region "fields/property"

        Protected Const MSG_NO_SUB_UNITS As String = "No sub units found"
        Protected _reportsDao As IReportsDao
        Private _chain As New Dictionary(Of Integer, String)
        Private _deathCount As Integer
        Private _diseaseCount As Integer
        Private _injuryCount As Integer
        Private endDate As Date = Nothing

        Public Property DeathCount() As Integer
            Get
                Return _deathCount
            End Get
            Set(ByVal value As Integer)
                _deathCount = value
            End Set
        End Property

        Public Property DiseaseCount() As Integer
            Get
                Return _diseaseCount
            End Get
            Set(ByVal value As Integer)
                _diseaseCount = value
            End Set
        End Property

        Public Property InjuryCount() As Integer
            Get
                Return _injuryCount
            End Get
            Set(ByVal value As Integer)
                _injuryCount = value
            End Set
        End Property

        Public ReadOnly Property LastCSID() As Integer
            Get
                Dim lastUnit As KeyValuePair(Of Integer, String) = CType(ViewState("drill"), Dictionary(Of Integer, String)).Last
                Return lastUnit.Key
            End Get

        End Property

        Public ReadOnly Property LastUnitName() As String
            Get
                Dim lastUnit As KeyValuePair(Of Integer, String) = CType(ViewState("drill"), Dictionary(Of Integer, String)).Last
                Return lastUnit.Value
            End Get

        End Property

        Public Property SelectedCSID() As Integer
            Get
                If ViewState(KEY_SELECTED_CS_ID) IsNot Nothing Then
                    Return CInt(ViewState(KEY_SELECTED_CS_ID))
                Else
                    Return Nothing
                End If
            End Get

            Set(ByVal value As Integer)
                ViewState(KEY_SELECTED_CS_ID) = value
            End Set

        End Property

        Protected Property FeedBackMessage() As String
            Get
                If (ViewState(KEY_NO_SUB_UNITS_MESSAGE) Is Nothing) Then
                    Return String.Empty
                End If
                Return CStr(ViewState(KEY_NO_SUB_UNITS_MESSAGE))
            End Get
            Set(ByVal value As String)
                ViewState(KEY_NO_SUB_UNITS_MESSAGE) = value
            End Set
        End Property

        Protected ReadOnly Property ReportsDao As IReportsDao
            Get
                If (_reportsDao Is Nothing) Then
                    _reportsDao = New NHibernateDaoFactory().GetReportsDao()
                End If

                Return _reportsDao
            End Get
        End Property

#End Region

#Region "Sort"

        Private Const ASCENDING As String = "Asc"
        Private Const DEFAULT_SORT_COLUMN As String = "Total"
        Private Const DESCENDING As String = "Desc"
        Private Const DET_SORT_COLUMN_KEY As String = "_DetSortExp_"
        Private Const DET_SORT_DIR_KEY As String = "_DetSortDirection_"
        Private Const SORT_COLUMN_KEY As String = "_SortExp_"
        Private Const SORT_DIR_KEY As String = "_SortDirection_"

        Private Property DetailSortColumn() As String
            Get
                If (ViewState(DET_SORT_COLUMN_KEY) Is Nothing) Then
                    ViewState(DET_SORT_COLUMN_KEY) = "case_Id"
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

        Private Property SortColumn() As String
            Get
                If (ViewState(SORT_COLUMN_KEY) Is Nothing) Then
                    ViewState(SORT_COLUMN_KEY) = "Total"
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
                    ViewState(SORT_DIR_KEY) = DESCENDING
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

#Region "Load"

        Protected Function ConstructReportArguments(ByVal cs_id As Integer, ByVal viewType As Integer) As LODCategoryCountReportArgs
            Dim args As LODCategoryCountReportArgs = New LODCategoryCountReportArgs()

            args.UnitId = cs_id
            args.ReportView = viewType
            args.BeginDate = rptTemplate.BeginDate
            args.EndDate = rptTemplate.EndDate
            args.IsComplete = rptTemplate.IsComplete

            Return args
        End Function

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If (Not Page.IsPostBack) Then
                _chain.Add(rptTemplate.Unit, rptTemplate.UnitText)
                ViewState("drill") = _chain
                SetRepeater()
                rptTemplate.ControlVisibility(Secure_Shared_UserControls_ReportNav.CtrlList.subordinate, False)
                rptTemplate.ControlVisibility(Secure_Shared_UserControls_ReportNav.CtrlList.SSN, False)
                rptTemplate.ControlVisibility(Secure_Shared_UserControls_ReportNav.CtrlList.SortOrder, False)

            End If
            AddHandler rptTemplate.RptClicked, AddressOf RunReport

        End Sub

        Private Sub RunReport(ByVal sender As Object, ByVal e As EventArgs)
            If (Page.IsValid) Then
                SortGridView(rptTemplate.Unit, CInt(Session("ReportView")), rptTemplate.UnitText)
                _chain.Clear()
                AddChain(CInt(Session("UnitId")), LookupService.GetUnitText(CInt(Session("unitId"))))
                AddChain(rptTemplate.Unit, rptTemplate.UnitText)
                ViewState("drill") = Nothing
                ViewState("drill") = _chain
                SetRepeater()
            End If
        End Sub

#End Region

#Region "Navigation"

        Protected Sub rptNav_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles rptNav.ItemCommand
            Dim args() As String
            If (e.CommandName <> String.Empty) Then
                args = e.CommandName.ToString.Split(";")
                Dim inChain As Boolean = True
                For Each item As KeyValuePair(Of Integer, String) In CType(ViewState("drill"), Dictionary(Of Integer, String))
                    If (inChain And Not _chain.Keys.Contains(item.Key)) Then
                        AddChain(item.Key, item.Value)
                    End If
                    If (item.Key = CInt(args(0))) Then
                        inChain = False
                    End If
                Next
                SortGridView(CInt(args(0)), CInt(Session("ReportView")), args(1))
                ViewState("drill") = _chain
                SetRepeater()
                rptTemplate.FillUnitLookup(CInt(args(0)), CInt(Session("ReportView")))

            End If
        End Sub

        Protected Sub rptNav_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptNav.ItemDataBound
            If (e.Item.ItemType <> ListItemType.Item AndAlso e.Item.ItemType <> ListItemType.AlternatingItem) Then
                Exit Sub
            End If
            Dim item As KeyValuePair(Of Integer, String) = CType(e.Item.DataItem, KeyValuePair(Of Integer, String))
            Dim lnk As LinkButton = CType(e.Item.FindControl("lnkButton"), LinkButton)
            lnk.Text = item.Value
            lnk.CommandName = item.Key.ToString() + ";" + item.Value
        End Sub

        Private Sub AddChain(ByVal cs_id As Integer, ByVal text As String)
            If (Not _chain.Keys.Contains(cs_id)) Then
                _chain.Add(cs_id, text)
            End If
        End Sub

        Private Sub SetRepeater()
            rptNav.DataSource = _chain
            rptNav.DataBind()
        End Sub

#End Region

#Region "ReportView"

        Protected Sub ReportGridView_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles ReportGridView.PageIndexChanging
            ReportGridView.PageIndex = e.NewPageIndex
            SortGridView(LastCSID, CInt(Session("ReportView")), LastUnitName)
        End Sub

        Protected Sub ReportGridView_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles ReportGridView.RowCommand
            If e.CommandName = "drill" Then
                Dim args() As String = e.CommandArgument.ToString.Split(";")
                Dim cs_id As Integer = CType(args(0), Integer)
                Dim unitName = args(1)

                SortGridView(cs_id, CInt(Session("ReportView")), unitName)
                _chain = ViewState("drill")
                AddChain(cs_id, CType(e.CommandSource, LinkButton).Text)
                ViewState("drill") = _chain
                SetRepeater()
                rptTemplate.FillUnitLookup(cs_id, CInt(Session("ReportView")))
            End If
        End Sub

        Protected Sub ReportGridView_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles ReportGridView.RowDataBound
            Dim grid As GridView = CType(sender, GridView)

            If e.Row.RowType = DataControlRowType.DataRow Then
                Dim data As System.Data.DataRowView = e.Row.DataItem
                If CInt(data("cs_id")) = SelectedCSID Then
                    e.Row.Visible = False
                End If
                _deathCount += data("death")
                _injuryCount += data("injury")
                _diseaseCount += data("disease")
            End If

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

        Protected Sub ReportGridView_Sorting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewSortEventArgs) Handles ReportGridView.Sorting
            ReportGridView.PageIndex = 0
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
            SortGridView(LastCSID, CInt(Session("ReportView")), LastUnitName)
        End Sub

        Private Sub FillGrid(ByVal cs_id As Integer, ByVal viewType As Integer, ByVal unitName As String)
            SelectedCSID = cs_id
            Dim ds As DataSet = ReportsDao.ExecuteLODCategoryCountReport(ConstructReportArguments(cs_id, viewType))

            ReportGridView.DataSource = ds
            ReportGridView.DataBind()

            rowDetail.Visible = False

            fillDetailGrid(cs_id)
            If DetailGridView.Rows.Count > 0 Then
                UnitNameLbl.Text = unitName
                rowDetail.Visible = True
            End If
            'The selected unit will always be part of the result set but hidden
            'If only the selected unit is part of the result set the Result grid will only show the header so
            'visibility on ReportGridView is removed
            If ReportGridView.Rows.Count > 1 Then
                ReportGridView.Visible = True
                FeedBackMessage = String.Empty
            Else
                FeedBackMessage = MSG_NO_SUB_UNITS
                ReportGridView.Visible = False
            End If
        End Sub

        Private Sub SortGridView(ByVal csId As Integer, ByVal View As Integer, ByVal unitName As String)
            FillGrid(csId, View, unitName)
            Dim dt As System.Data.DataTable = CType(ReportGridView.DataSource, System.Data.DataSet).Tables(0)

            Dim dv As New System.Data.DataView(dt)

            If (SortColumn.Length > 0) Then
                dv.Sort = SortExpression
            End If

            ReportGridView.DataSource = dv
            ReportGridView.DataBind()
        End Sub

#End Region

#Region "DetailView"

        Protected Sub DetailGridView_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles DetailGridView.PageIndexChanging
            DetailGridView.PageIndex = e.NewPageIndex
            SortDetailGridView()
        End Sub

        Protected Sub DetailGridView_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles DetailGridView.RowDataBound

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

        Protected Sub DetailGridView_Sorting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewSortEventArgs) Handles DetailGridView.Sorting

            DetailGridView.PageIndex = 0
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

        Private Sub fillDetailGrid(ByVal cs_id As Integer)
            Dim ds As DataSet = ReportsDao.ExecuteLODCategoryCountDetailsReport(ConstructReportArguments(cs_id, 0))
            DetailGridView.DataSource = ds
            DetailGridView.DataBind()
        End Sub

        Private Sub SortDetailGridView()
            fillDetailGrid(LastCSID)
            Dim dt As System.Data.DataTable = CType(DetailGridView.DataSource, System.Data.DataSet).Tables(0)

            Dim dv As New System.Data.DataView(dt)

            If (DetailSortColumn.Length > 0) Then
                dv.Sort = DetailSortExpression
            End If
            DetailGridView.DataSource = dv
            DetailGridView.DataBind()

        End Sub

#End Region

        Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

            FeedbackPanel.Visible = FeedBackMessage.Length > 0
            FeedbackMessageLabel.Text = FeedBackMessage

        End Sub

    End Class

End Namespace