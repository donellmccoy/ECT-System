Namespace Web.DBSign

    Partial Class Successful
        Inherits System.Web.UI.Page

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            'make sure the caller is a real caller
            Dim querySig As String = Request.Params("sgn").ToString()

            'if this is a signonly it will be doubled
            If (querySig.Contains(",")) Then
                querySig = querySig.Split(",")(0)
            End If

            Dim number As Long = CType(querySig, Long)
            Dim verify As Long = CType(Session("SigRan"), Long)

            'the random number is a one time use only, so we remove it on load
            Session.Remove("SigRan")

            If (number <> verify) Then
                'the numbers don't match, something is wrong
                'this is a possible replay attack, so redirect to the failure page
                Session.Remove("SignResult")
                Response.Redirect("~/Secure/Shared/DBSign/Failure.aspx?&error=wrongkey", True)
            End If

            'now make sure it's not too old
            Dim ts As Date = Date.FromFileTime(number)

            If (Now.Subtract(ts).TotalMinutes > 20) Then

            End If

            'indicate that the signature was successful
            Session("SignResult") = "1"
            Session("SignMode") = "DBSign"

            Response.Cache.SetCacheability(HttpCacheability.NoCache)

        End Sub

    End Class

End Namespace