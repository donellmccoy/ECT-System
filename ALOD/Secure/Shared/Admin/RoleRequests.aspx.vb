Imports ALOD.Data

Namespace Web.Admin

    Partial Class Secure_Shared_Admin_RoleRequests
        Inherits System.Web.UI.Page

        Private Const ASCENDING As String = "Asc"
        Private Const DESCENDING As String = "Desc"
        Private Const SORT_COLUMN_KEY As String = "_SortExp_"
        Private Const SORT_DIR_KEY As String = "_SortDirection_"

        Private Property SortColumn() As String
            Get
                If (ViewState(SORT_COLUMN_KEY) Is Nothing) Then
                    ViewState(SORT_COLUMN_KEY) = "LastName"
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

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If (Not IsPostBack) Then
                Session.Remove("editId")
                InitRequestGrid()
            End If

        End Sub

        Protected Sub RequestGrid_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles RequestGrid.RowCommand

            If (e.CommandName = "ProcessRequest") Then
                Session("editId") = CInt(e.CommandArgument)
                Response.Redirect("~/Secure/Shared/Admin/EditUser.aspx?caller=3", True)
            End If

        End Sub

        Protected Sub RequestGrid_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles RequestGrid.RowDataBound

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

        Protected Sub RequestGrid_Sorting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewSortEventArgs) Handles RequestGrid.Sorting

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
            InitRequestGrid()

        End Sub

        Private Sub InitRequestGrid()

            Dim data As DataSet = New NHibernateDaoFactory().GetUserRoleRequestDao().GetAllPendingRequests(CInt(Session("UserId")))
            Dim view As DataView = New DataView(data.Tables(0))

            If (SortColumn.Length > 0) Then
                view.Sort = SortExpression
            End If

            RequestGrid.DataSource = view
            RequestGrid.DataBind()

        End Sub

    End Class

End Namespace