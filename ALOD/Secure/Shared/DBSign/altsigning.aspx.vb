Imports ALOD.Core.Utils
Imports ALOD.Logging
Imports ALODWebUtility.Common

Namespace Web.DBSign

    Partial Class Secure_Shared_DBSign_altsigning
        Inherits System.Web.UI.Page

        Protected Sub btnGo_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnGo.Click

            If (hValid.Value = "N") Then
                Exit Sub
            End If

            If txtSSN.Text.Trim <> CStr(HttpContext.Current.Session("SSN")) Then
                lblMsg.ForeColor = Drawing.Color.Red
                lblMsg.Text = "Invalid: Incorrect SSN"
                hValid.Value = "N"
                Exit Sub
            End If

            'indicate that the signature was successful
            Session("SignResult") = "1"
            Session("SignMode") = "SSN"
            BodyTag.Attributes.Add("onload", "next();")

        End Sub

        Protected Sub btnVerifySSN_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnVerifySSN.Click

            txtSSN.Enabled = True
            btnVerifySSN.Enabled = True

            If (txtSSN.Text.Trim = CStr(HttpContext.Current.Session("SSN"))) Then
                lblMsg.Visible = True
                lblMsg.ForeColor = Drawing.Color.Green
                lblMsg.Text = Server.HtmlEncode("Valid: " + CStr(HttpContext.Current.Session("UserName")))
                hValid.Value = "Y"
                BodyTag.Attributes.Add("onload", "enableGo();")
            Else
                txtSSN.Enabled = True
                btnVerifySSN.Enabled = True
                lblMsg.ForeColor = Drawing.Color.Red
                lblMsg.Text = "Invalid: Incorrect SSN"
                hValid.Value = "N"
                BodyTag.Attributes.Add("onload", "disableGo();")
            End If

        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            'make sure the caller is a real caller
            Dim querySig As String = Request.Params("sgn").ToString()
            Dim number As Long = CType(querySig, Long)
            Dim verify As Long = CType(Session("SigRan"), Long)

            If (number <> verify) Then
                'the numbers don't match, possible replay attack.  Either way, we don't sign
                Session.Remove("SignResult")
                LogManager.LogError("Signature failed.  Invalid SigRan")
                Response.Redirect("~/Secure/Shared/DBSign/Failure.aspx", True)
            End If

            If (Not IsPostBack) Then

                btnGo.Attributes.Add("onclick", "return processGo();")
                rbCAC.Attributes.Add("onclick", "rbCACJS()")
                rbSSN.Attributes.Add("onclick", "rbSSNJS()")
                txtSSN.Attributes.Add("onchange", "onChangeSSN();")

                SetDefaultButton(txtSSN, btnVerifySSN)

                If (AppMode <> DeployMode.Production) Then
                    rbSSN.Checked = True
                    txtSSN.Text = Server.HtmlEncode(CStr(HttpContext.Current.Session("SSN")))
                    btnVerifySSN_Click(sender, e)
                End If

            End If

            WriteHostName(Page)
            Response.Cache.SetCacheability(HttpCacheability.NoCache)

        End Sub

    End Class

End Namespace