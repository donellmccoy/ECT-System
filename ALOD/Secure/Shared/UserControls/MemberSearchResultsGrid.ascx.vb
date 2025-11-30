Imports ALODWebUtility.Common

Namespace Web.UserControls

    Public Class MemberSearchResultsGrid
        Inherits System.Web.UI.UserControl

        Public Event MemberSelected(ByVal sender As Object, ByVal e As MemberSelectedEventArgs)

        Public ReadOnly Property GridViewUniqueId() As String
            Get
                Return grdMemberSelection.ClientID
            End Get
        End Property

        Public Sub Initialize(ByRef dataSource As DataTable)

            grdMemberSelection.DataSource = dataSource
            grdMemberSelection.DataBind()

        End Sub

        Protected Sub grdMemberSelection_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs) Handles grdMemberSelection.RowCommand

            If (e.CommandName = "MemberSelected") Then
                Dim evtArgs As New MemberSelectedEventArgs

                evtArgs.SelectedRowIndex = Convert.ToInt32(e.CommandArgument)

                RaiseEvent MemberSelected(Me, evtArgs)
            End If

        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        End Sub

    End Class

End Namespace