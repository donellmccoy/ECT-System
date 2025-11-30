Option Strict On

Imports SRXLite.DataAccess
Imports SRXLite.Modules

Namespace Web

    Partial Class ErrorLog
        Inherits System.Web.UI.Page

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If Not IsPostBack Then
                BindGrid(25)
            End If
        End Sub

        Private Sub BindGrid(ByVal RowCount As Short)
            gvErrorLog.DataSource = DB.ExecuteDataset("dsp_ErrorLog_Select " & RowCount)
            gvErrorLog.DataBind()
        End Sub

        Protected Sub gvErrorLog_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvErrorLog.RowDataBound
            If e.Row.RowType = DataControlRowType.DataRow Then
                Dim errorMsg As String = e.Row.Cells(1).Text
                Dim errorText As String = Left(errorMsg, 200)
                If errorMsg.Length > 200 Then
                    Dim spanID As String = "E" & e.Row.RowIndex
                    errorText &= "...<span id='" & spanID & "' style='display:none'>" & errorMsg.Substring(200) & "</span>"
                    e.Row.Cells(1).Attributes.Add("onclick", "expand('" & spanID & "');")
                    e.Row.Cells(1).Style.Add("cursor", "pointer")
                End If
                e.Row.Cells(1).Text = errorText.Replace(vbCrLf, "<br />")
            End If
        End Sub

        Protected Sub ddlTop_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlTop.SelectedIndexChanged
            BindGrid(ShortCheck(ddlTop.SelectedValue))
        End Sub

    End Class

End Namespace