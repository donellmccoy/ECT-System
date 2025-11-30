Imports ALOD.Core.Domain.Documents
Imports ALOD.Core.Domain.WelcomePageBanner
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Core.Utils
Imports ALOD.Data
Imports ALOD.Logging
Imports ALODWebUtility.Common

Namespace Web

    Partial Class Secure
        Inherits System.Web.UI.MasterPage

        Protected _start As Date
        Private _daoFactory As IDaoFactory
        Private _docDao As IDocumentDao
        Private _hyperLinkDao As IHyperLinkDao
        Private _permissionDao As IALODPermissionDao
        Private _viewHelpDocPerm As ALODPermission

        Protected ReadOnly Property DaoFactory As IDaoFactory
            Get
                If (_daoFactory Is Nothing) Then
                    _daoFactory = New NHibernateDaoFactory()
                End If

                Return _daoFactory
            End Get
        End Property

        Protected ReadOnly Property DocumentDao() As IDocumentDao
            Get
                If (_docDao Is Nothing) Then
                    _docDao = New SRXDocumentStore(CStr(HttpContext.Current.Session("UserName")))
                End If

                Return _docDao
            End Get
        End Property

        Protected ReadOnly Property DocumentGroupId() As Integer
            Get
                Return PermissionDao.GetDocGroupIdByPermId(ViewHelpDocPermission.Id)
            End Get
        End Property

        Protected ReadOnly Property HyperLinkDao As IHyperLinkDao
            Get
                If (_hyperLinkDao Is Nothing) Then
                    _hyperLinkDao = DaoFactory.GetHyperLinkDao()
                End If

                Return _hyperLinkDao
            End Get
        End Property

        Protected ReadOnly Property PermissionDao() As IALODPermissionDao
            Get
                If (_permissionDao Is Nothing) Then
                    _permissionDao = DaoFactory.GetPermissionDao()
                End If

                Return _permissionDao
            End Get
        End Property

        Protected ReadOnly Property ViewHelpDocPermission() As ALODPermission
            Get
                If (_viewHelpDocPerm Is Nothing) Then
                    Dim lst As IList(Of ALODPermission) = PermissionDao.GetAll().ToList()
                    If (lst.Count > 0) Then
                        _viewHelpDocPerm = (From p In lst Where p.Name = "viewHelpDocs" Select p).First()
                    End If
                End If

                Return _viewHelpDocPerm
            End Get
        End Property

        Public Function BuildDocumentLink(l As HyperLink, documents As IList(Of Document))

            Dim logMessage = "Help Document with ID = " + l.Value
            Return "~/Secure/Shared/DocumentViewer.aspx?docId=" + l.Value + "&amp;modId=1&amp;doc=" + logMessage.Replace(" ", "+")
        End Function

        Public Sub GenerateQuestionMarkLink()

            Dim link = (From l In HyperLinkDao.GetAll().ToList() Where l.Name = "QUESTIONMARKHELP" Select l).FirstOrDefault()
            Dim documents As IList(Of Document) = DocumentDao.GetDocumentsByGroupId(DocumentGroupId)

            If (link IsNot Nothing) Then

                HelpLink.NavigateUrl = BuildDocumentLink(link, documents)

            End If

        End Sub

        Public Function GetCurrentPageTitle(ByVal node As SiteMapNode, ByVal title As String) As String

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

        Protected Sub ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.MenuEventArgs) Handles MainMenu.MenuItemDataBound

            Dim visible As String = CType(e.Item.DataItem, SiteMapNode)("visible")

            If (visible IsNot Nothing) AndAlso (visible.ToLower() = "false") Then
                If (e.Item.Parent IsNot Nothing) Then
                    e.Item.Parent.ChildItems.Remove(e.Item)
                Else
                    MainMenu.Items.Remove(e.Item)
                End If
            End If

            'does this option have an environment restriction?
            If (AppMode = DeployMode.Production) Then

                'some menu options are not available in production, remove them here
                Dim noProd As String = CType(e.Item.DataItem, SiteMapNode)("NoProd")

                If (noProd IsNot Nothing) AndAlso (noProd.ToLower() = "true") Then
                    If (e.Item.Parent IsNot Nothing) Then
                        e.Item.Parent.ChildItems.Remove(e.Item)
                    Else
                        MainMenu.Items.Remove(e.Item)
                    End If
                End If
            End If

            'does it have some other environment restriction?
            Dim modes As String = CType(e.Item.DataItem, SiteMapNode)("Modes")

            If (modes IsNot Nothing) AndAlso (modes.Length > 0) Then

                Dim hide As Boolean = True
                Dim parts() As String = modes.Split(",")

                For Each mode As String In parts
                    If (mode.CompareTo(AppMode.ToString()) = 0) Then
                        hide = False 'we have a matching environment, so show this one
                        Exit For
                    End If
                Next

                If (hide) Then
                    MainMenu.Items.Remove(e.Item)
                End If

            End If

            If Session(SESSIONKEY_COMPO) = "5" Then

                If e.Item.Text = "Worldwide Duty (WWD)" Then
                    e.Item.Text = "Non Duty Disability Evaluation System (NDDES)"
                ElseIf e.Item.Text = "My WWDs" Then
                    e.Item.Text = "My NDDES"
                ElseIf e.Item.Text = "Start New WWD" Then
                    e.Item.Text = "Start New NDDES"
                End If

                'Dim smn5 As SiteMapNode = CType(e.Item.DataItem, SiteMapNode)
                'smn5.ParentNode.ReadOnly = False
                'smn5.ReadOnly = False

                'If smn5.Title <> "" Then
                '    e.Item.DataItem("title") = smn5.Title.Replace("Worldwide Duty (WWD)", "Non Duty Disability Evaluation System (NDDES)")
                'End If

            End If

        End Sub

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            _start = Now
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            'ensure our session is still valid
            If (Session("UserId") Is Nothing) OrElse (CInt(Session("UserId") = 0)) Then
                Response.Redirect("~/Public/Logout.aspx", True)
            End If

            'register javascripts
            WriteHostName(Page)

            ' Scripts now loaded via bundles in master page head - no need for code-behind registration
            Dim scriptBasePath As String = Page.ResolveClientUrl("~/Script/")
            'Page.ClientScript.RegisterClientScriptInclude("JQueryScript", scriptBasePath + "jquery-3.6.0.min.js")
            'Page.ClientScript.RegisterClientScriptInclude("MigrateScript", scriptBasePath + "jquery-migrate-3.4.1.min.js")
            'Page.ClientScript.RegisterClientScriptInclude("JqueryBlock", scriptBasePath + "jquery.blockUI.min.js")
            'Page.ClientScript.RegisterClientScriptInclude("JqueryDom", scriptBasePath + "jquery-dom.js")
            'Page.ClientScript.RegisterClientScriptInclude("JqueryUI", scriptBasePath + "jquery-ui-1.13.0.min.js")
            ''Page.ClientScript.RegisterClientScriptInclude("JqueryDimensions", Request.ApplicationPath + "/Script/jquery.dimensions.js")
            'Page.ClientScript.RegisterClientScriptInclude("JQueryModal", scriptBasePath + "jqModal.js")
            Page.ClientScript.RegisterClientScriptInclude("CommonScript", scriptBasePath + "common.js")

            'check the PHI banner
            Dim hippa As String = SiteMap.CurrentNode("Hippa")

            If (hippa IsNot Nothing) AndAlso (hippa.ToLower() = "true") Then
                banner.Visible = True
            Else
                banner.Visible = False
            End If

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
                lblPageTitle.Text = "<H1>" + title.Replace("::", ">") + "</H1>"
            Else
                Page.Title = "ECT"
                lblPageTitle.Visible = False
            End If

            If Session(SESSIONKEY_COMPO) = "5" Then

                lblPageTitle.Text = lblPageTitle.Text.Replace("Worldwide Duty (WWD)", "Non Duty Disability Evaluation System (NDDES)")
                lblPageTitle.Text = lblPageTitle.Text.Replace("WWD", "NDDES")
                lblPageTitle.Text = lblPageTitle.Text.Replace("WD", "NDDES")

            End If

            If (AppMode = DeployMode.Development OrElse AppMode = DeployMode.Test) Then
                spnHIPPAPolicy.Visible = True
            End If

            'get our sessionviewer setup
            InitSessionViewer()

            'update the usersonline
            'Utility.UpdateUserOnline(CInt(Session("UserId")))
            If (Not UpdateUserOnline(CInt(Session("UserId")))) Then
                Exit Sub
            End If

            lblCurrentUser.Text = CStr(Session("DisplayName"))
            UserRoleLabel.Text = CStr(Session("Group"))

            If (SESSION_LAST_LOGIN.HasValue) Then
                LastLoginLabel.Text = "Last Login: " + SESSION_LAST_LOGIN.Value.ToString(DATE_HOUR_FORMAT)
            Else
                LastLoginLabel.TabIndex = "Last login: Not Found"
            End If

            GenerateQuestionMarkLink()

            ' Set version number in footer
            SetVersionLabel()

        End Sub

        Private Sub SetVersionLabel()
            Try
                Dim assembly As Reflection.Assembly = Reflection.Assembly.GetExecutingAssembly()
                Dim version As Version = assembly.GetName().Version
                VersionLabel.Text = String.Format("Version {0}.{1}.{2}", version.Major, version.Minor, version.Build)
            Catch ex As Exception
                ' If version cannot be retrieved, leave label empty or set a default
                VersionLabel.Text = String.Empty
            End Try
        End Sub

        Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

            HIPAAPolicy.NavigateUrl = ""

            If AppMode = DeployMode.Development OrElse AppMode = DeployMode.Test Then
                HIPAAPolicy.NavigateUrl = ""
                HIPAAPolicy.Text = "Generation Time: " + Now.Subtract(_start).TotalMilliseconds.ToString() + " ms"
            End If

            If AppMode = DeployMode.Production OrElse AppMode = DeployMode.Development Then
                Dim measuredTime As String = If(Now.Subtract(_start).TotalMilliseconds.ToString() IsNot Nothing, Now.Subtract(_start).TotalMilliseconds.ToString(), String.Empty)

                Dim referringPage As String = If(HttpContext.Current?.Request?.UrlReferrer?.AbsolutePath IsNot Nothing, HttpContext.Current.Request.UrlReferrer.AbsolutePath, String.Empty)

                Dim currentPage As String = If(Request?.Url?.AbsolutePath IsNot Nothing, Request.Url.AbsolutePath, String.Empty)

                Dim username As String = If(HttpContext.Current?.Session("UserName") IsNot Nothing, CStr(HttpContext.Current.Session("UserName")), String.Empty)

                Dim role As String = If(HttpContext.Current?.Session("Group") IsNot Nothing, CStr(HttpContext.Current.Session("Group")), String.Empty)

                Dim action_date As DateTime = DateTime.Now

                Try
                    'Send Generation Time Log
                    LogManager.LogGenerationTime(action_date, measuredTime, currentPage, referringPage, username, role)
                Catch ex As Exception
                    ' Show the exception.
                    LogManager.LogError(ex, "GenerationTimeLog failed: " & ex.Message)

                    ' Show the inner exception, if one is present.
                    If ex.InnerException IsNot Nothing Then
                        LogManager.LogError("Inner Exception: " & ex.InnerException.Message)
                    End If
                End Try
            End If
        End Sub

        Private Sub InitSessionViewer()

            If (AppMode <> DeployMode.Development) Then
                lnkSessionViewer.Visible = False
                lnkSessionViewer.Enabled = False
                SVSeparator.Visible = False
                Exit Sub
            End If

            Dim url As String = Page.ResolveClientUrl("~/Secure/Shared/SessionViewer.aspx")
            Dim features As String = "'width=700, height=600, scrollbars=1 resizable=1'"
            Dim buffer As New StringBuilder()

            buffer.Append("<SCRIPT language='javascript'>" + vbCrLf)
            buffer.Append("function OpenViewer(){" + vbCrLf)
            buffer.Append("window.open('" + url + "','', " + features + ");" + vbCrLf)
            buffer.Append("return false;" + vbCrLf)
            buffer.Append("}" + vbCrLf)
            buffer.Append("</SCRIPT>" + vbCrLf)

            Page.ClientScript.RegisterStartupScript(Page.GetType().BaseType, "OpenViewer", buffer.ToString())

            lnkSessionViewer.Visible = True
            lnkSessionViewer.Enabled = True
            lnkSessionViewer.Attributes.Add("onclick", "return OpenViewer();")
            SVSeparator.Visible = True

        End Sub

    End Class

End Namespace