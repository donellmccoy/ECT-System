Imports ALODWebUtility.Common

Namespace Web

    Partial Class Secure_Popup
        Inherits System.Web.UI.MasterPage

        Public Sub SetHeaderTitle(ByVal title As String)
            lblPageTitle.Text = Server.HtmlEncode(title)
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            'ensure our session is still valid
            If (Session("UserId") Is Nothing) OrElse (CInt(Session("UserId") = 0)) Then
                Response.Redirect("~/Public/Logout.aspx", True)
            End If

            'register javascripts
            WriteHostName(Page)

            ' Scripts now loaded via bundles in master page head - no need for code-behind registration
            'Dim strAppPAth As String = Request.ApplicationPath
            'If Request.ApplicationPath = "" OrElse Request.ApplicationPath = "/" Then
            '    strAppPAth = Request.Url.Scheme + Uri.SchemeDelimiter + Request.Url.Host + ":" + Request.Url.Port.ToString()
            'End If
            'Page.ClientScript.RegisterClientScriptInclude("JQueryScript", strAppPAth + "/Script/jquery-3.6.0.min.js")
            'Page.ClientScript.RegisterClientScriptInclude("MigrateScript", strAppPAth + "/Script/jquery-migrate-3.4.1.min.js")
            'Page.ClientScript.RegisterClientScriptInclude("JqueryBlock", strAppPAth + "/Script/jquery.blockUI.min.js")
            'Page.ClientScript.RegisterClientScriptInclude("JqueryDom", strAppPAth + "/Script/jquery-dom.js")
            'Page.ClientScript.RegisterClientScriptInclude("JqueryUI", strAppPAth + "/Script/jquery-ui-1.13.0.min.js")
            'Page.ClientScript.RegisterClientScriptInclude("JqueryDimensions", strAppPAth + "/Script/jquery.dimensions.js")
            'Page.ClientScript.RegisterClientScriptInclude("JQueryModal", strAppPAth + "/Script/jqModal.js")
            'Page.ClientScript.RegisterClientScriptInclude("CommonScript", strAppPAth + "/Script/common.js")

            If (Not IsPostBack) Then
                SetPageTitle()
            End If

            'Utility.UpdateUserOnline(SESSION_USER_ID)
            If (Not UpdateUserOnline(CInt(Session("UserId")))) Then
                Exit Sub
            End If

        End Sub

        Protected Sub SetPageTitle()

            'set our page title
            Dim title As String = GetCurrentPageTitle(SiteMap.CurrentNode, SiteMap.CurrentNode.Title)
            If (title.Length > 0) Then
                Page.Title = "ECT :: " + title
            Else
                Page.Title = "ECT"
            End If

            If (title.Length > 0) Then
                Dim last As String = title.Substring(title.LastIndexOf(":") + 1)
                Page.Title = "ECT :: " + last
                lblPageTitle.Text = Server.HtmlEncode(last)
            Else
                Page.Title = "ECT"
            End If

        End Sub

        Private Function GetCurrentPageTitle(ByVal node As SiteMapNode, ByVal title As String) As String

            If (node.ParentNode IsNot Nothing) AndAlso (node.ParentNode.Title.Length > 0) Then
                title = node.ParentNode.Title + "." + title
                Return GetCurrentPageTitle(node.ParentNode, title)
            ElseIf (node.Title = title) Then
                Return title
            Else
                Dim parts As String() = title.Split(".")
                Dim buffer As New StringBuilder()
                'Dim nodes As Byte = 2

                For i As Short = 0 To parts.Length - 1
                    buffer.Append(parts(i))
                    buffer.Append(" :: ")
                Next

                buffer.Replace(" :: ", String.Empty, buffer.Length - 4, 4)
                Return buffer.ToString()

            End If

        End Function

    End Class

End Namespace