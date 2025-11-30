Imports ALOD.Core.Domain.Documents
Imports ALOD.Core.Domain.WelcomePageBanner
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Logging
Imports ALODWebUtility.Common

Namespace Web.Admin

    Partial Class Secure_Shared_Admin_MsgAdmin
        Inherits System.Web.UI.Page

        Const COMMAND_DELETE As String = "DeleteMessage"
        Const COMMAND_UPDATE As String = "UpdatePopup"
        Private _docDao As IDocumentDao
        Private _documents As IList(Of Document)
        Private _hyperLinkDao As IHyperLinkDao
        Private _hyperLinkTypeDao As IHyperLinkTypeDao
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
                    _hyperLinkDao = New NHibernateDaoFactory().GetHyperLinkDao()
                End If

                Return _hyperLinkDao
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

        Protected Sub btnAddMessage_Click(sender As Object, e As EventArgs) Handles btnAddMessage.Click
            ucCreateMessage.Show()
            ucCreateMessage.Visible = True
            UcEditMessage.Visible = False
        End Sub

        Protected Sub dataMessages_Selecting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.ObjectDataSourceSelectingEventArgs) Handles dataMessages.Selecting
            e.InputParameters("compo") = SESSION_COMPO
            e.InputParameters("isAdmin") = UserHasPermission(PERMISSION_SYSTEM_ADMIN)
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

        Protected Sub gvMessages_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles gvMessages.RowCommand
            Dim Index As Integer = CType(e.CommandSource.Parent.Parent, GridViewRow).RowIndex
            Dim messageId As Integer = 0

            Try
                messageId = Integer.Parse(gvMessages.DataKeys(Index).Value)
            Catch ex As Exception
                Exit Sub
            End Try

            If (messageId = 0) Then
                Exit Sub
            End If

            Select Case e.CommandName
                Case COMMAND_UPDATE
                    UcEditMessage.Show(messageId)
                    UcEditMessage.Visible = True
                    ucCreateMessage.Visible = False
                Case COMMAND_DELETE
                    Dim message As New Message
                    message.DeleteMessage(gvMessages.DataKeys(Index).Value)
                    gvMessages.DataBind()

                    LogManager.LogAction(ModuleType.System, UserAction.MessageDelete,
                                         messageId, "Deleted Message")

            End Select

        End Sub

        Protected Sub ucCreateMessage_MessageCanceled() Handles ucCreateMessage.MessageCancelled

            ucCreateMessage.Visible = False
        End Sub

        Protected Sub ucCreateMessage_MessageSaved() Handles ucCreateMessage.MessageSaved
            gvMessages.DataBind()
            ucCreateMessage.Visible = False
        End Sub

        Protected Sub ucEditMessage_MessageCanceled() Handles UcEditMessage.MessageCancelled

            UcEditMessage.Visible = False
        End Sub

        Protected Sub ucEditMessage_MessageSaved() Handles UcEditMessage.MessageSaved
            gvMessages.DataBind()
            UcEditMessage.Visible = False
        End Sub

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Dim message As New Message
            message.Assigned = True

            If (Not IsPostBack) Then

                ucCreateMessage.Visible = False
                UcEditMessage.Visible = False
                PopulateHyperLinksGridView()

            End If

        End Sub

        Private Sub PopulateHyperLinksGridView()
            gdvHyperLinks.DataSource = HyperLinkDao.GetAll().ToList()
            gdvHyperLinks.DataBind()
        End Sub

    End Class

End Namespace