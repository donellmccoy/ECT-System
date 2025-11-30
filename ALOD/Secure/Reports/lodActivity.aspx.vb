Imports ALOD.Core.Domain.Documents
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Reports
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Data.Services
Imports ALOD.Web.UserControls

Namespace Web.Reports

    Partial Class Secure_Reports_lodActivity
        Inherits System.Web.UI.Page

        Protected _reportsDao As IReportsDao
        Private _chain As New Dictionary(Of String, String)

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
        Private Const SORT_COLUMN_KEY As String = "_SortExp_"
        Private Const SORT_DIR_KEY As String = "_SortDirection_"

        Private Property DetailSortColumn() As String
            Get
                If (ViewState(DET_SORT_COLUMN_KEY) Is Nothing) Then
                    ViewState(DET_SORT_COLUMN_KEY) = "Name"
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
                    ViewState(SORT_COLUMN_KEY) = "Name"
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

#Region "Properties"

        Protected ReadOnly Property CalendarImage() As String
            Get
                Return Me.ResolveClientUrl("~/App_Themes/" + Page.Theme + "/Images/Calendar.gif")
            End Get
        End Property

#End Region

#Region "Properites to set Print button to view Final 348/216 snapshot "

        Private _dao As IDocumentDao

        ' Added by skennedy 7/16/2013
        Private _documents As IList(Of ALOD.Core.Domain.Documents.Document)

        Private instance As LineOfDuty

        ReadOnly Property DocumentDao() As IDocumentDao
            Get
                If (_dao Is Nothing) Then
                    _dao = New SRXDocumentStore(CStr(HttpContext.Current.Session("UserName")))
                End If

                Return _dao
            End Get
        End Property

#End Region

#Region "Navigation"

        Protected Sub rptNav_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles rptNav.ItemCommand
            Dim args() As String
            If (e.CommandName <> String.Empty) Then
                args = e.CommandName.ToString.Split(";")
                Dim inChain As Boolean = True
                For Each item As KeyValuePair(Of String, String) In CType(ViewState("drill"), Dictionary(Of String, String))
                    If (inChain And Not _chain.Keys.Contains(item.Key)) Then
                        AddChain(item.Key, item.Value)
                    End If
                    If (item.Key = CInt(args(0))) Then
                        inChain = False
                    End If
                Next
                FillReportGrid(CInt(args(0)), args(1))
                ViewState("drill") = _chain
                SetRepeater()
                rptTemplate.Unit = CInt(args(0))
                '    rptTemplate.FillUnitLookup(CInt(args(0)), CInt(Session("ReportView")))
            End If
        End Sub

        Protected Sub rptNav_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptNav.ItemDataBound

            If (e.Item.ItemType <> ListItemType.Item AndAlso e.Item.ItemType <> ListItemType.AlternatingItem) Then
                Exit Sub
            End If
            Dim item As KeyValuePair(Of String, String) = CType(e.Item.DataItem, KeyValuePair(Of String, String))
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

#Region "Load"

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If (Not Page.IsPostBack) Then
                '  _chain.Add(CInt(rptTemplate.Unit), rptTemplate.UnitText)
                '  ViewState("drill") = _chain
                '  SetRepeater()
                rptTemplate.ControlVisibility(Secure_Shared_UserControls_ReportNav.CtrlList.SSN, False)
                rptTemplate.ControlVisibility(Secure_Shared_UserControls_ReportNav.CtrlList.SortOrder, False)
            End If
            AddHandler rptTemplate.RptClicked, AddressOf RunReport

        End Sub

#End Region

#Region "ReportGrid"

        Public Sub UpdateChain(ByVal cs_id As Integer, ByVal Name As String)
            _chain = ViewState("drill")
            AddChain(cs_id, Name)
            ViewState("drill") = _chain
            SetRepeater()
        End Sub

        Protected Function ConstructReportArguments(ByVal cs_id As Integer) As LODActivitiesMetricsReportArgs
            Dim args As LODActivitiesMetricsReportArgs = New LODActivitiesMetricsReportArgs()

            args.UnitId = cs_id
            args.BeginDate = rptTemplate.BeginDate
            args.EndDate = rptTemplate.EndDate
            args.ReportView = CInt(Session("ReportView"))
            args.IsComplete = rptTemplate.IsComplete
            args.IncludeSubordinateUnits = rptTemplate.IncludeSubordinate

            Return args
        End Function

        Protected Sub ReportGrid_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles ReportGrid.RowCommand
            If e.CommandName = "drill" Then
                Dim args() As String = e.CommandArgument.ToString.Split(";")
                Dim cs_id As Integer = CType(args(0), Integer)
                Dim unitName = args(1)
                FillReportGrid(cs_id, unitName)
                rptTemplate.Unit = cs_id
                UpdateChain(cs_id, unitName)

            End If
        End Sub

        Private Sub FillReportGrid(ByVal cs_id As Integer, ByVal Name As String)

            '  Dim ds As System.Data.DataSet = ALodRptList.GetLodActivitiesCount(cs_id, CInt(Session("ReportView")), rptTemplate.BeginDate, rptTemplate.EndDate, rptTemplate.IsComplete, rptTemplate.IncludeSubordinate)
            '  Dim ds As System.Data.DataSet = ReportsDao.ExecuteLODMetricsActivitiesCountReport(ConstructReportArguments(cs_id))
            ' ReportGrid.DataSource = ds
            ' ReportGrid.DataBind()
            'DetailDiv.Visible = False
            'If (ReportGrid.Rows.Count = 1) Then
            FillDetailGrid()
            '    DetailsNameLabel.Text = Name
            DetailDiv.Visible = True

            'End If
        End Sub

        Private Sub RunReport(ByVal sender As Object, ByVal e As EventArgs)

            If (Page.IsValid) Then
                FillReportGrid(rptTemplate.Unit, rptTemplate.UnitText)
                _chain.Clear()
                AddChain(CInt(Session("UnitId")), LookupService.GetUnitText(CInt(Session("unitId"))))
                AddChain(rptTemplate.Unit, rptTemplate.UnitText)
                ViewState("drill") = Nothing
                ViewState("drill") = _chain
                '  DetailDiv.Visible = False
                SetRepeater()

            End If

        End Sub

#End Region

#Region "DetailView"

        Protected Sub grvLODActivities_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles grvLODActivities.PageIndexChanging
            grvLODActivities.PageIndex = e.NewPageIndex
            FillDetailGrid()
        End Sub

        Protected Sub grvLODActivities_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles grvLODActivities.RowDataBound

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

            If (e.Row.RowType = DataControlRowType.DataRow) Then

                Dim data As DataRowView = CType(e.Row.DataItem, DataRowView)
                Dim refID As Integer = data("lodid")
                ViewFinal(refID, data, e)

            End If

        End Sub

        Protected Sub grvLODActivities_Sorting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewSortEventArgs) Handles grvLODActivities.Sorting

            grvLODActivities.PageIndex = 0
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

            FillDetailGrid()

        End Sub

        ' Handles Printing 384/261 forms:
        ' If IsFinal then get pdf from database, else build pdf on the fly
        Protected Sub ViewFinal(ByVal refID As String, ByVal data As DataRowView, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs)

            Dim form348ID As String = 0
            Dim form261ID As String = 0
            instance = LodService.GetById(CInt(data("lodid")))

            ' ckeck for GroupID; some cases were cancled
            If (instance.DocumentGroupId) Then
                _documents = DocumentDao.GetDocumentsByGroupId(instance.DocumentGroupId)
            End If

            If (data("description").ToString() = "Complete") Then

                ' fileSubString is used to get the correct 348 document if multiple 348s are associated with the LOD's group Id.
                ' This happens when a case is overridden and recompleted or if a case is reinvestigated.
                Dim fileSubString As String = instance.CaseId & "-Generated"

                Dim isDoc As Boolean = False

                If (_documents IsNot Nothing) Then
                    For Each docItem In _documents
                        If (docItem.DocType.ToString() = "FinalForm348" AndAlso docItem.OriginalFileName.Contains(fileSubString)) Then
                            form348ID = docItem.Id.ToString()
                            isDoc = True

                        End If

                        'If (docItem.DocType.ToString() = "FinalForm261") Then
                        '    form261ID = docItem.Id.ToString()

                        'End If

                    Next
                End If

                If (isDoc) Then

                    Dim url348 As String = Me.ResolveClientUrl("~/Secure/Shared/DocumentViewer.aspx?docID=" & form348ID & "&modId=" & ModuleType.LOD)
                    ' Dim url261 As String = Me.ResolveClientUrl("~/Secure/Shared/DocumentViewer.aspx?docID=" & form261ID)
                    ' CType(e.Row.FindControl("PrintImage"), ImageButton).OnClientClick = "viewDocs('" & url348 & "', '" & url261 & "'); return false;"
                    CType(e.Row.FindControl("PrintImage"), ImageButton).OnClientClick = "viewDoc('" & url348 & "'); return false;"
                    CType(e.Row.FindControl("PrintImage"), ImageButton).AlternateText = "Print Final Forms"
                Else
                    CType(e.Row.FindControl("PrintImage"), ImageButton).OnClientClick = "printForms('" & refID & "', 'lod'); return false;"
                    CType(e.Row.FindControl("PrintImage"), ImageButton).AlternateText = "Print Final Forms"

                End If
            Else
                CType(e.Row.FindControl("PrintImage"), ImageButton).OnClientClick = "printForms('" & refID & "', 'lod'); return false;"
                CType(e.Row.FindControl("PrintImage"), ImageButton).AlternateText = "Print Draft Forms"

            End If

        End Sub

        Private Sub FillDetailGrid()
            FillDetailGrid(rptTemplate.Unit)
            Dim dt As System.Data.DataTable = CType(grvLODActivities.DataSource, System.Data.DataSet).Tables(0)

            Dim dv As New System.Data.DataView(dt)

            If (DetailSortColumn.Length > 0) Then
                dv.Sort = DetailSortExpression
            End If

            grvLODActivities.DataSource = dv
            grvLODActivities.DataBind()

        End Sub

        Private Sub FillDetailGrid(ByVal cs_id As Integer)
            'Dim ds As DataSet = ALodRptList.GetLodActivities(cs_id, CInt(Session("ReportView")), rptTemplate.BeginDate, rptTemplate.EndDate, rptTemplate.IsComplete, rptTemplate.IncludeSubordinate)
            Dim ds As DataSet = ReportsDao.ExecuteLODMetricsActivitiesDetailsReport(ConstructReportArguments(cs_id))
            grvLODActivities.DataSource = ds
            grvLODActivities.DataBind()
        End Sub

#End Region

    End Class

End Namespace