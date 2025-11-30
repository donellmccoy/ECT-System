Imports ALOD.Core.Domain.Documents
Imports ALOD.Core.Domain.WelcomePageBanner
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALODWebUtility.Common

Namespace Web.Admin

    Public Class WelcomePageBanner
        Inherits System.Web.UI.Page

        Private _banner As ALOD.Core.Domain.WelcomePageBanner.WelcomePageBanner
        Private _docCatViewDao As IDocCategoryViewDao
        Private _docDao As IDocumentDao
        Private _documents As IList(Of Document)
        Private _docViewDao As IDocumentViewDao
        Private _hyperLinkDao As IHyperLinkDao
        Private _hyperLinkTypeDao As IHyperLinkTypeDao
        Private _keyValDao As IKeyValDao
        Private _permissionDao As IALODPermissionDao
        Private _viewHelpDocPerm As ALODPermission

        ReadOnly Property Documents() As IList(Of Document)
            Get
                If (_documents Is Nothing) Then
                    _documents = DocumentDao.GetDocumentsByGroupId(DocumentGroupId)
                End If

                Return _documents
            End Get
        End Property

        Protected ReadOnly Property Banner As ALOD.Core.Domain.WelcomePageBanner.WelcomePageBanner
            Get
                If (_banner Is Nothing) Then
                    _banner = New ALOD.Core.Domain.WelcomePageBanner.WelcomePageBanner(KeyValDao, HyperLinkDao, DocumentDao, DocumentGroupId)
                End If

                Return _banner
            End Get
        End Property

        Protected ReadOnly Property DocCatViewDao() As IDocCategoryViewDao
            Get
                If (_docCatViewDao Is Nothing) Then
                    _docCatViewDao = New NHibernateDaoFactory().GetDocCategoryViewDao()
                End If

                Return _docCatViewDao
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

        Protected ReadOnly Property DocumentViewDao() As IDocumentViewDao
            Get
                If (_docViewDao Is Nothing) Then
                    _docViewDao = New NHibernateDaoFactory().GetDocumentViewDao()
                End If

                Return _docViewDao
            End Get
        End Property

        Protected ReadOnly Property DocumentViewId() As Integer
            Get
                Return DocumentViewDao.GetIdByDescription("Help Documents")
            End Get
        End Property

        Protected ReadOnly Property HyperLinkDao As IHyperLinkDao
            Get
                If (_hyperLinkDao Is Nothing) Then
                    _hyperLinkDao = New NHibernateDaoFactory().GetHyperLinkDao()
                End If

                Return _hyperLinkDao
            End Get
        End Property

        Protected ReadOnly Property HyperLinkTypeDao As IHyperLinkTypeDao
            Get
                If (_hyperLinkTypeDao Is Nothing) Then
                    _hyperLinkTypeDao = New NHibernateDaoFactory().GetHyperLinkTypeDao()
                End If

                Return _hyperLinkTypeDao
            End Get
        End Property

        Protected ReadOnly Property KeyValDao As IKeyValDao
            Get
                If (_keyValDao Is Nothing) Then
                    _keyValDao = New NHibernateDaoFactory().GetKeyValDao()
                End If

                Return _keyValDao
            End Get
        End Property

        Protected ReadOnly Property PermissionDao() As IALODPermissionDao
            Get
                If (_permissionDao Is Nothing) Then
                    _permissionDao = New NHibernateDaoFactory().GetPermissionDao()
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

        Protected Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click
            If (chkBannerEnabled.Checked) Then
                Banner.Enabled = True
            Else
                Banner.Enabled = False
            End If

            If (Not String.IsNullOrEmpty(txtBannerContent.Text)) Then
                Banner.RawBannerText = Server.HtmlEncode(txtBannerContent.Text)
            Else
                Banner.RawBannerText = String.Empty
            End If

            Banner.Save()
        End Sub

        Protected Sub gdvHyperLinks_PageIndexChanging(sender As Object, e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles gdvHyperLinks.PageIndexChanging
            gdvHyperLinks.PageIndex = e.NewPageIndex
            PopulateHyperLinksGridView()
        End Sub

        Protected Sub gdvHyperLinks_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gdvHyperLinks.RowDataBound
            If (e.Row.RowType <> DataControlRowType.DataRow) Then
                Exit Sub
            End If

            Dim type As String = e.Row.Cells(1).Text
            Dim link As HyperLink = HyperLinkDao.GetById(gdvHyperLinks.DataKeys(e.Row.RowIndex)("Id"))

            ' Check if this row is being edited or not
            If (e.Row.RowIndex <> gdvHyperLinks.EditIndex AndAlso link IsNot Nothing) Then
                If (type.Equals("Document")) Then
                    Dim document As Document = Documents.SingleOrDefault(Function(x) x.Id = Long.Parse(link.Value))

                    If (document IsNot Nothing) Then
                        CType(e.Row.FindControl("lblLinkValue"), Label).Text = document.Description & "." & document.Extension
                    Else
                        CType(e.Row.FindControl("lblLinkValue"), Label).Text = "DOCUMENT NOT FOUND"
                    End If
                Else
                    CType(e.Row.FindControl("lblLinkValue"), Label).Text = link.Value
                End If
            End If
        End Sub

        Protected Sub gdvPHFormFields_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles gdvHyperLinks.RowCommand
            If (e.CommandName.Equals("Page")) Then
                Exit Sub
            End If

            Dim parts() As String = e.CommandArgument.ToString().Split("|")

            If (parts.Count <> 1) Then
                Exit Sub
            End If

            Dim rowIndex As Int16 = CInt(parts(0)) Mod gdvHyperLinks.PageSize
            Dim link As HyperLink = HyperLinkDao.GetById(gdvHyperLinks.DataKeys(rowIndex)("Id"))

            If (link Is Nothing) Then
                Exit Sub
            End If

            If (e.CommandName = "EditLink") Then
                gdvHyperLinks.SelectedIndex = rowIndex
            ElseIf (e.CommandName = "DeleteLink") Then
                HyperLinkDao.Delete(link)
                HyperLinkDao.CommitChanges()
                PopulateHyperLinksGridView()
            End If
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If (Not IsPostBack) Then
                SetMaxLength(txtBannerContent)

                chkBannerEnabled.Checked = Banner.Enabled
                txtBannerContent.Text = Server.HtmlDecode(Banner.RawBannerText)
                PopulateHyperLinksGridView()
            End If
        End Sub

        Private Sub PopulateHelpDocumentsDDL(ByVal ddl As DropDownList)
            If (ddl Is Nothing) Then
                Exit Sub
            End If

            If (ddl.Items IsNot Nothing AndAlso ddl.Items.Count > 0) Then
                Exit Sub
            End If

            ddl.DataSource = DocumentDao.GetDocumentsByGroupId(DocumentGroupId).OrderBy(Function(x) x.Description).ToList()
            ddl.DataValueField = "Id"
            ddl.DataTextField = "Description"
            ddl.DataBind()

            InsertDropDownListZeroValue(ddl, "-- Select a Document --")
            ddl.SelectedIndex = 0
        End Sub

        Private Sub PopulateHyperLinksGridView()
            gdvHyperLinks.DataSource = HyperLinkDao.GetAll().ToList()
            gdvHyperLinks.DataBind()
        End Sub

        Private Sub PopulateHyperLinkTypeDDL(ByVal ddl As DropDownList)
            If (ddl Is Nothing) Then
                Exit Sub
            End If

            If (ddl.Items IsNot Nothing AndAlso ddl.Items.Count > 1) Then
                Exit Sub
            End If

            ddl.DataSource = HyperLinkTypeDao.GetAll()
            ddl.DataValueField = "Id"
            ddl.DataTextField = "Name"
            ddl.DataBind()

            InsertDropDownListZeroValue(ddl, "-- Select Link Type --")
            ddl.SelectedIndex = 0
        End Sub

    End Class

End Namespace