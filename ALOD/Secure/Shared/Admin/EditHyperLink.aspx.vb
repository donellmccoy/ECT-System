Imports ALOD.Core.Domain.Documents
Imports ALOD.Core.Domain.WelcomePageBanner
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Core.Utils
Imports ALOD.Data
Imports ALODWebUtility.Common

Namespace Web.Admin

    Public Class EditHyperLink
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

        Protected Sub btnAddHyperLink_Click(sender As Object, e As EventArgs) Handles btnAddHyperLink.Click
            InitAddLinkControls()
        End Sub

        Protected Sub btnAddLink_Click(sender As Object, e As EventArgs) Handles btnAddLink.Click
            trAddValidationErrors.Visible = False
            bltAddErrors.Items.Clear()

            ' VALIDATE INPUT
            If (String.IsNullOrEmpty(txtAddLinkName.Text)) Then
                bltAddErrors.Items.Add("Must provide a link Reference Name")
            End If

            If (txtAddLinkName.Text.Length > txtAddLinkName.MaxLength) Then
                bltAddErrors.Items.Add("The Reference Name CANNOT be greater than " + txtAddLinkName.MaxLength + " characters")
            End If

            If (HyperLinkDao.GetAll().Where(Function(x) x.Name.ToUpper().Equals(txtAddLinkName.Text.ToUpper())).Count > 0) Then
                bltAddErrors.Items.Add("A banner link with a reference name of " + txtAddLinkName.Text + " already exists. Please pick a different reference name.")
            End If

            If (ddlAddHyperLinkType.SelectedIndex < 0) Then
                bltAddErrors.Items.Add("Must select a link Type")
            End If

            If (String.IsNullOrEmpty(txtAddDisplayText.Text)) Then
                bltAddErrors.Items.Add("Must provide link Display Text")
            End If

            If (txtAddDisplayText.Text.Length > txtAddDisplayText.MaxLength) Then
                bltAddErrors.Items.Add("The Display Text CANNOT be greater than " + txtAddDisplayText.MaxLength + " characters")
            End If

            If (ddlAddHyperLinkType.SelectedItem.Text.Equals("Document") AndAlso ddlAddHelpDocuments.SelectedIndex < 0) Then
                bltAddErrors.Items.Add("Must select a link Value")
            End If

            If (ddlAddHyperLinkType.SelectedItem.Text.Equals("Website") AndAlso String.IsNullOrEmpty(txtAddWebsiteLink.Text)) Then
                bltAddErrors.Items.Add("Must provide a link Value")
            End If

            If (ddlAddHyperLinkType.SelectedItem.Text.Equals("Website") AndAlso txtAddWebsiteLink.Text.Length > txtAddWebsiteLink.MaxLength) Then
                bltAddErrors.Items.Add("The Website value CANNOT be greater than " + txtAddWebsiteLink.MaxLength + " characters")
            End If

            ' If any errors were found then don't create new link...
            If (bltAddErrors.Items.Count > 0) Then
                trAddValidationErrors.Visible = True
                Exit Sub
            End If

            ' Create a new Banner Link and save it to the database...
            Dim newLink As HyperLink = New HyperLink()

            newLink.Name = Server.HtmlEncode(txtAddLinkName.Text)
            newLink.Type = HyperLinkTypeDao.GetById(Convert.ToInt32(ddlAddHyperLinkType.SelectedValue))
            newLink.DisplayText = Server.HtmlEncode(txtAddDisplayText.Text)

            If (ddlAddHyperLinkType.SelectedItem.Text.Equals("Document")) Then
                newLink.Value = ddlAddHelpDocuments.SelectedValue
            Else
                newLink.Value = Server.HtmlEncode(txtAddWebsiteLink.Text)
            End If

            HyperLinkDao.SaveOrUpdate(newLink)
            HyperLinkDao.CommitChanges()
            HyperLinkDao.Evict(newLink)

            PopulateHyperLinksGridView()
            pnlAddHyperLink.Visible = False
        End Sub

        Protected Sub btnEditLink_Click(sender As Object, e As EventArgs) Handles btnEditLink.Click
            trEditValidationErrors.Visible = False
            bltEditErrors.Items.Clear()

            ' VALIDATE INPUT
            If (String.IsNullOrEmpty(txtEditLinkName.Text)) Then
                bltEditErrors.Items.Add("Must provide a link Reference Name")
            End If

            If (txtEditLinkName.Text.Length > txtEditLinkName.MaxLength) Then
                bltEditErrors.Items.Add("The Reference Name CANNOT be greater than " + txtEditLinkName.MaxLength + " characters")
            End If

            If (HyperLinkDao.GetAll().Where(Function(x) x.Name.ToString().Equals(txtEditLinkName.Text.ToUpper())).Count > 0) Then
                bltAddErrors.Items.Add("A banner link with a reference name of " + txtEditLinkName.Text + " already exists. Please pick a different reference name.")
            End If

            If (ddlEditHyperLinkType.SelectedIndex < 0) Then
                bltEditErrors.Items.Add("Must select a link Type")
            End If

            If (String.IsNullOrEmpty(txtEditDisplayText.Text)) Then
                bltEditErrors.Items.Add("Must provide link Display Text")
            End If

            If (txtEditDisplayText.Text.Length > txtEditDisplayText.MaxLength) Then
                bltEditErrors.Items.Add("The Display Text CANNOT be greater than " + txtEditDisplayText.MaxLength + " characters")
            End If

            If (ddlEditHyperLinkType.SelectedItem.Text.Equals("Document") AndAlso ddlEditHelpDocuments.SelectedIndex <= 0) Then
                bltEditErrors.Items.Add("Must select a link Value")
            End If

            If (ddlEditHyperLinkType.SelectedItem.Text.Equals("Website") AndAlso String.IsNullOrEmpty(txtEditWebsiteLink.Text)) Then
                bltEditErrors.Items.Add("Must provide a link Value")
            End If

            If (ddlEditHyperLinkType.SelectedItem.Text.Equals("Website") AndAlso txtEditWebsiteLink.Text.Length > txtEditWebsiteLink.MaxLength) Then
                bltEditErrors.Items.Add("The Website value CANNOT be greater than " + txtEditWebsiteLink.MaxLength + " characters")
            End If

            ' If any errors were found then don't create new link...
            If (bltEditErrors.Items.Count > 0) Then
                trEditValidationErrors.Visible = True
                Exit Sub
            End If

            ' Modify the existing Banner Link and save the changes to the database...
            Dim editLink As HyperLink = HyperLinkDao.GetById(CInt(ViewState("EditId")))

            If (editLink Is Nothing) Then
                bltEditErrors.Items.Add("Edit Failed: Could not find the selected link")
                trEditValidationErrors.Visible = True
                Exit Sub
            End If

            editLink.Name = Server.HtmlEncode(txtEditLinkName.Text)
            editLink.Type = HyperLinkTypeDao.GetById(Convert.ToInt32(ddlEditHyperLinkType.SelectedValue))
            editLink.DisplayText = Server.HtmlEncode(txtEditDisplayText.Text)

            If (ddlEditHyperLinkType.SelectedItem.Text.Equals("Document")) Then
                editLink.Value = ddlEditHelpDocuments.SelectedValue
            Else
                editLink.Value = Server.HtmlEncode(txtEditWebsiteLink.Text)
            End If

            HyperLinkDao.SaveOrUpdate(editLink)
            HyperLinkDao.CommitChanges()
            HyperLinkDao.Evict(editLink)

            PopulateHyperLinksGridView()
            pnlEditHyperLink.Visible = False
        End Sub

        Protected Sub CancelLinkAddOrEdit(sender As Object, e As EventArgs) Handles btnCancelAddLink.Click, btnCancelEditLink.Click
            pnlAddHyperLink.Visible = False
            pnlEditHyperLink.Visible = False
        End Sub

        Protected Sub ddlAddHyperLinkType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlAddHyperLinkType.SelectedIndexChanged
            If (ddlAddHyperLinkType.SelectedItem.Text.Equals("Document")) Then
                txtAddWebsiteLink.Visible = False
                ddlAddHelpDocuments.Visible = True
            Else
                txtAddWebsiteLink.Visible = True
                ddlAddHelpDocuments.Visible = False
            End If
        End Sub

        Protected Sub ddlEditHyperLinkType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlEditHyperLinkType.SelectedIndexChanged
            If (ddlEditHyperLinkType.SelectedItem.Text.Equals("Document")) Then
                txtEditWebsiteLink.Visible = False
                ddlEditHelpDocuments.Visible = True
            Else
                txtEditWebsiteLink.Visible = True
                ddlEditHelpDocuments.Visible = False
            End If
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
                InitEditLinkControls(link)
            ElseIf (e.CommandName = "DeleteLink") Then
                HyperLinkDao.Delete(link)
                HyperLinkDao.CommitChanges()
                PopulateHyperLinksGridView()
            End If
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If (Not IsPostBack) Then

                SetInputFormatRestriction(Page, txtAddLinkName, FormatRestriction.AlphaNumeric)
                SetInputFormatRestriction(Page, txtAddDisplayText, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
                SetInputFormatRestriction(Page, txtAddWebsiteLink, FormatRestriction.AlphaNumeric, WEBSITE_SPECIAL_CHAR_INPUT)
                SetInputFormatRestriction(Page, txtEditLinkName, FormatRestriction.AlphaNumeric)
                SetInputFormatRestriction(Page, txtEditDisplayText, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
                SetInputFormatRestriction(Page, txtEditWebsiteLink, FormatRestriction.AlphaNumeric, WEBSITE_SPECIAL_CHAR_INPUT)

                PopulateHyperLinksGridView()
            End If
        End Sub

        Private Sub InitAddLinkControls()
            ResetAddLinkControls()
            pnlAddHyperLink.Visible = True
            PopulateHyperLinkTypeDDL(ddlAddHyperLinkType)
            PopulateHelpDocumentsDDL(ddlAddHelpDocuments)
        End Sub

        Private Sub InitEditLinkControls(ByVal link As HyperLink)
            ResetEditLinkControls()

            If (link Is Nothing) Then
                Exit Sub
            End If

            ViewState("EditId") = link.Id

            pnlEditHyperLink.Visible = True
            PopulateHyperLinkTypeDDL(ddlEditHyperLinkType)
            PopulateHelpDocumentsDDL(ddlEditHelpDocuments)

            txtEditLinkName.Text = link.Name
            txtEditDisplayText.Text = link.DisplayText

            ddlEditHyperLinkType.SelectedValue = link.Type.Id

            If (ddlEditHyperLinkType.SelectedItem.Text.Equals("Document")) Then
                ddlEditHelpDocuments.Visible = True
                txtEditWebsiteLink.Visible = False

                If (ddlEditHelpDocuments.Items.FindByValue(link.Value) IsNot Nothing) Then
                    ddlEditHelpDocuments.SelectedValue = link.Value
                End If
            Else
                ddlEditHelpDocuments.Visible = False
                txtEditWebsiteLink.Visible = True
                txtEditWebsiteLink.Text = link.Value
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

        Private Sub ResetAddLinkControls()
            pnlAddHyperLink.Visible = False
            txtAddLinkName.Text = String.Empty
            txtAddDisplayText.Text = String.Empty
            txtAddWebsiteLink.Text = String.Empty
            ddlAddHyperLinkType.SelectedIndex = -1
            ddlAddHelpDocuments.SelectedIndex = -1

            trAddValidationErrors.Visible = False
            bltAddErrors.Items.Clear()
        End Sub

        Private Sub ResetEditLinkControls()
            pnlEditHyperLink.Visible = False
            txtEditLinkName.Text = String.Empty
            txtEditDisplayText.Text = String.Empty
            txtEditWebsiteLink.Text = String.Empty
            ddlEditHyperLinkType.SelectedIndex = -1
            ddlEditHelpDocuments.SelectedIndex = -1

            trEditValidationErrors.Visible = False
            bltEditErrors.Items.Clear()

            gdvHyperLinks.SelectedIndex = -1
            gdvHyperLinks.EditIndex = -1

            ViewState("EditId") = 0
        End Sub

    End Class

End Namespace