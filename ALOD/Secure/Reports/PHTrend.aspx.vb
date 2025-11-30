Namespace Web.Reports

    Public Class PHTrend
        Inherits System.Web.UI.Page

        '  Protected Const MSG_NO_SUB_UNITS As String = "No sub units found"

        Private _chain As New Dictionary(Of Integer, String)

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If (Not Page.IsPostBack) Then

                Dim date1, date2 As String

                date1 = (Now.Day + 1) - Now.Day

                date2 = Now.Month & "/" & date1 & "/" & Now.Year

                lblDate.Text = date2

                '    ReportNavPH1.IncludeSubordinate.

            End If

        End Sub

    End Class

End Namespace