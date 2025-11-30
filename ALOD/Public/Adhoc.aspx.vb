Imports ALOD.Data

Public Class Adhoc
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim sql As New SqlDataStore()
        GridView1.DataSource = sql.ExecuteDataSet("sp_Adhoc_WorkflowTRacking")
        GridView1.DataBind()

    End Sub

End Class