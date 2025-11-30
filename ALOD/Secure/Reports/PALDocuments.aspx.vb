Imports ALOD.Core.Domain.Documents
Imports ALOD.Core.Domain.Users
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Core.Utils
Imports ALOD.Data
Imports ALODWebUtility.Common

Namespace Web.Reports

    Partial Class Secure_Reports_PALDocsReport
        Inherits System.Web.UI.Page

        Private Const ASCENDING As String = "Asc"
        Private Const DESCENDING As String = "Desc"
        Private Const SORT_COLUMN_KEY As String = "_SortExp_"
        Private Const SORT_DIR_KEY As String = "_SortDirection_"
        Private Const subtractDays As Double = -30

        Private _documentDAO As IDocumentDao
        Private _reportsDao As IReportsDao

        Protected ReadOnly Property CalendarImage() As String
            Get
                Return Me.ResolveClientUrl("~/App_Themes/" + Page.Theme + "/Images/Calendar.gif")
            End Get
        End Property

        Protected ReadOnly Property DocumentDao() As IDocumentDao
            Get
                If (_documentDAO Is Nothing) Then
                    _documentDAO = New SRXDocumentStore(CStr(HttpContext.Current.Session("UserName")))
                End If

                Return _documentDAO
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

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If (IsUserBelongsToTheBoard(SESSION_GROUP_ID, True, True) Or SESSION_GROUP_ID = UserGroups.SeniorMedicalReviewer) Then
                If (Not Page.IsPostBack) Then
                    SetControlInputRestrictions()
                End If
            Else
                Response.Redirect(ConfigurationManager.AppSettings("AccessDeniedUrl"))
            End If
        End Sub

        Protected Sub ReportGrid_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles ReportGrid.PageIndexChanging
            ReportGrid.PageIndex = e.NewPageIndex
            FillGrid()
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
            FillGrid()
        End Sub

        Protected Sub SetControlInputRestrictions()
            SetInputFormatRestriction(Page, txtSSN, FormatRestriction.Numeric, "-")
            SetInputFormatRestriction(Page, txtLastName, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
        End Sub

        Private Sub FillGrid()
            Dim Valid As Boolean = True

            GridViewErrorLabel.Text = String.Empty

            'SSN
            If (txtSSN.Text.Length > 0) Then
                If (Not IsNumeric(txtSSN.Text)) Then
                    Valid = False
                End If
            End If

            If (Valid) Then
                Dim ds As DataSet = ReportsDao.ExecutePALDocumentsReport(Server.HtmlEncode(txtLastName.Text), Server.HtmlEncode(txtSSN.Text))
                Dim view As DataView = New DataView(ds.Tables(0))

                ReportGrid.DataSource = view
                ReportGrid.DataBind()
            Else
                GridViewErrorLabel.Text = "SSN format is incorrect."
            End If
        End Sub

        Private Sub ReportButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ReportButton.Click
            Dim StrError = ""

            If Len(txtSSN.Text) <> 4 Then
                StrError = StrError & "Last 4 SSN must be the last 4 numbers of the member SSN.  "
            Else
                If Not IsNumeric(txtSSN.Text) Then
                    StrError = StrError & "Last 4 SSN must be the last 4 numbers of the member SSN.  "
                End If
            End If

            If Len(txtLastName.Text) < 2 Then
                StrError = StrError & "First two letters of the member Last Name must be entered.  "
            End If

            If Len(StrError) < 1 Then
                errorLbl.Text = StrError
                errorLbl.ForeColor = Drawing.Color.Black
                FillGrid()
            Else
                errorLbl.Text = StrError
                errorLbl.ForeColor = Drawing.Color.Red
                ReportGrid.DataSource = Nothing
                ReportGrid.DataBind()
            End If
        End Sub

    End Class

End Namespace