Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data

Namespace Web.Sys

    Public Class CancelReasons
        Inherits System.Web.UI.Page

        Private _CancelReasonsDao As ILookupCancelReasonsDao

        Public ReadOnly Property CancelReasonsDao As ILookupCancelReasonsDao
            Get
                If (_CancelReasonsDao Is Nothing) Then
                    _CancelReasonsDao = New NHibernateDaoFactory().GetLookupCancelReasonsDao()
                End If

                Return _CancelReasonsDao
            End Get
        End Property

        Protected Sub gdv_CancelReasons_RowUpdating(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewUpdateEventArgs) Handles gdvCancelReasons.RowUpdating

            Dim CancelReasonsId As Integer = CInt(gdvCancelReasons.DataKeys(e.RowIndex).Value)
            Dim CancelReasonsDesc As String = CType(gdvCancelReasons.Rows(e.RowIndex).FindControl("txtDescription"), TextBox).Text
            Dim CancelReasonsOrder As Integer = Integer.Parse(CType(gdvCancelReasons.Rows(e.RowIndex).FindControl("txtDisplay"), TextBox).Text)

            CancelReasonsDao.UpdateCancelReasons(CancelReasonsId, CancelReasonsDesc, CancelReasonsOrder)

            gdvCancelReasons.EditIndex = -1

            UpdateCancelReasonsGridView()
        End Sub

        Protected Sub gdvCancelReasons_RowCancelingEdit(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCancelEditEventArgs) Handles gdvCancelReasons.RowCancelingEdit
            gdvCancelReasons.EditIndex = -1
            UpdateCancelReasonsGridView()
        End Sub

        Protected Sub gdvCancelReasons_RowEditing(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewEditEventArgs) Handles gdvCancelReasons.RowEditing
            gdvCancelReasons.EditIndex = e.NewEditIndex
            UpdateCancelReasonsGridView()
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If (Not IsPostBack) Then
                UpdateCancelReasonsGridView()
            End If

        End Sub

        Private Sub UpdateCancelReasonsGridView()
            gdvCancelReasons.DataSource = CancelReasonsDao.GetAll()
            gdvCancelReasons.DataBind()
        End Sub

    End Class

End Namespace