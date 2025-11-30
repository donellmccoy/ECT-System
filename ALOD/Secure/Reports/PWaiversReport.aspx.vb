Imports ALOD.Core.Domain.Reports
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Core.Utils
Imports ALOD.Data
Imports ALOD.Data.Services
Imports ALODWebUtility.Common
Imports ALODWebUtility.Permission.Search

Namespace Web.Reports

    Partial Class Secure_Reports_PWaiversReport
        Inherits System.Web.UI.Page

        Private Const ASCENDING As String = "Asc"
        Private Const DESCENDING As String = "Desc"
        Private Const SORT_COLUMN_KEY As String = "_SortExp_"
        Private Const SORT_DIR_KEY As String = "_SortDirection_"
        Private Const subtractDays As Double = -30

        Private _reportsDao As IReportsDao

        Protected ReadOnly Property BeginDate As Nullable(Of Date)
            Get
                If (txtBeginDate.Text.Trim() = "") Then
                    Return Nothing
                Else
                    Return Server.HtmlEncode(CDate(txtBeginDate.Text))
                End If
            End Get
        End Property

        Protected ReadOnly Property CalendarImage() As String
            Get
                Return Me.ResolveClientUrl("~/App_Themes/" + Page.Theme + "/Images/Calendar.gif")
            End Get
        End Property

        Protected ReadOnly Property EndDate As Nullable(Of Date)
            Get
                If (txtEndDate.Text.Trim() = "") Then
                    Return Nothing
                Else
                    Return Server.HtmlEncode(CDate(txtEndDate.Text))
                End If
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

        Public Sub FillUnitLookup(ByVal cs_id As Integer, ByVal viewType As Integer)
            Dim units = From u In LookupService.GetChildUnits(cs_id, viewType)
                        Select u
                        Order By u.Name

            ddlUnit.DataSource = units
            ddlUnit.DataBind()

            SetDropdownByValue(ddlUnit, cs_id.ToString())
        End Sub

        Protected Function ConstructReportArgs(ByVal ssn As String) As PWaiversReportArgs
            Dim args As PWaiversReportArgs = New PWaiversReportArgs()

            args.UnitId = ddlUnit.SelectedValue
            args.SSN = Server.HtmlEncode(ssn)
            args.BeginDate = BeginDate
            args.EndDate = EndDate
            args.Interval = IntervalDropDownList.SelectedValue
            args.UserGroupId = Session("GroupId")
            args.IncludeSubordinateUnits = chkSubordinateUnit.Checked

            Return args
        End Function

        Protected Function GetServiceMembersByName() As DataTable
            Return LookupService.GetServerMembersByName(txtMemberLastName.Text, txtMemberFirstName.Text, txtMemberMiddleName.Text)
        End Function

        Protected Sub MemberSelected(ByVal sender As Object, ByVal e As MemberSelectedEventArgs)
            Dim resultsTable As DataTable = GetServiceMembersByName()
            Dim ssn As String = String.Empty

            Session("rowIndex") = e.SelectedRowIndex
            ssn = resultsTable.Rows(e.SelectedRowIndex).Field(Of String)("SSN")

            FillGrid(ssn)
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            AddHandler ucMemberSelectionGrid.MemberSelected, AddressOf MemberSelected

            If (Not Page.IsPostBack) Then
                SetControlInputRestrictions()

                FillUnitLookup(CInt(Session("UnitId")), CInt(Session("ReportView")))
                SetDropdownByValue(ddlUnit, CInt(Session("UnitId")))

                trSSN.Visible = True
                trName.Visible = False
            End If
        End Sub

        Protected Sub ReportGrid_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles ReportGrid.PageIndexChanging
            ReportGrid.PageIndex = e.NewPageIndex
            FillGrid(GetSelectedMemberSSN())
        End Sub

        Protected Sub ReportGrid_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles ReportGrid.RowCommand
            If (e.CommandName = "view") Then
                Dim parts() As String = e.CommandArgument.ToString().Split(";")
                Dim strQuery As New StringBuilder()
                Dim args As New ItemSelectedEventArgs
                args.RefId = CInt(parts(0))

                strQuery.Append("refId=" + CType(args.RefId, String))

                args.Url = "~/Secure/SC_PWaivers/init.aspx?" + strQuery.ToString()

                Response.Redirect(args.Url)
            End If
        End Sub

        Protected Sub ReportGrid_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles ReportGrid.RowDataBound
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

        Protected Sub ReportGrid_Sorting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewSortEventArgs) Handles ReportGrid.Sorting
            ReportGrid.PageIndex = 0

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

            If (rdbSSN.Checked) Then
                FillGrid(txtSSN.Text)
            ElseIf (rdbName.Checked) Then

                lblInvalidMemberName.Visible = False
                lblMemberNotFound.Visible = False

                If (String.IsNullOrEmpty(txtMemberLastName.Text) = True And String.IsNullOrEmpty(txtMemberFirstName.Text) = True And String.IsNullOrEmpty(txtMemberMiddleName.Text) = True) Then
                    lblInvalidMemberName.Visible = True
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
                    FillGrid(resultsTable.Rows(0).Field(Of String)("SSN"))
                Else
                    lblMemberNotFound.Visible = True
                    Exit Sub
                End If
            End If
        End Sub

        Protected Sub SearchSelectionRadioButton_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rdbSSN.CheckedChanged, rdbName.CheckedChanged
            If (rdbSSN.Checked) Then
                trSSN.Visible = True
                trName.Visible = False
            Else
                trSSN.Visible = False
                trName.Visible = True
            End If
        End Sub

        Protected Sub SetControlInputRestrictions()
            SetInputFormatRestriction(Page, txtBeginDate, FormatRestriction.AlphaNumeric, "/")
            SetInputFormatRestriction(Page, txtEndDate, FormatRestriction.AlphaNumeric, "/")
            SetInputFormatRestriction(Page, txtSSN, FormatRestriction.Numeric, "-")
        End Sub

        Private Sub FillGrid(ByVal ssn As String)
            Dim Valid As Boolean = True

            GridViewErrorLabel.Text = String.Empty

            If (ssn.Length > 0) Then
                If (Not IsNumeric(ssn)) Then
                    Valid = False
                End If
            End If

            If (Valid) Then
                Dim ds As DataSet = ReportsDao.ExecutePWaiversReport(ConstructReportArgs(ssn))
                Dim view As DataView = New DataView(ds.Tables(0))

                If (SortColumn.Length > 0) Then
                    view.Sort = SortExpression
                End If

                ReportGrid.DataSource = view
                ReportGrid.DataBind()

                ResultsPanel.Visible = True
                MemberSelectionPanel.Visible = False
            Else
                GridViewErrorLabel.Text = "SSN format is incorrect."
                ResultsPanel.Visible = False
            End If
        End Sub

        Private Function GetSelectedMemberSSN() As String
            If (rdbSSN.Checked) Then
                Return txtSSN.Text
            ElseIf (rdbName.Checked) Then
                If (String.IsNullOrEmpty(txtMemberLastName.Text) = True And String.IsNullOrEmpty(txtMemberFirstName.Text) = True And String.IsNullOrEmpty(txtMemberMiddleName.Text) = True) Then
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

        Private Sub ReportButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ReportButton.Click
            If (txtEndDate.Text.Length = 0) Then
                txtEndDate.Text = Now.ToShortDateString()
            End If

            ReportGrid.Sort(SortOrderDDL.SelectedValue, WebControls.SortDirection.Ascending)
        End Sub

    End Class

End Namespace