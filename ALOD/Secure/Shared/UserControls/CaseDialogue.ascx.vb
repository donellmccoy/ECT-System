Imports System.Data.SqlClient
Imports System.Runtime.CompilerServices
Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Users
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Core.Utils
Imports ALOD.Data
Imports ALOD.Data.Services
Imports ALOD.Logging
Imports ALOD.Web.UserControls
Imports ALODWebUtility.Common

Namespace Secure.Shared.UserControls
    Module PageControlExtensions

        <Extension()>
        Function FindChildControlById(ByVal controlToStartWith As Control, ByVal controlIdToFind As String) As Control
            If controlToStartWith Is Nothing Then Return Nothing
            If controlToStartWith.ID = controlIdToFind Then Return controlToStartWith
            For Each childControl As Control In controlToStartWith.Controls
                Dim resCtrl As Control = FindChildControlById(childControl, controlIdToFind)
                If resCtrl IsNot Nothing Then Return resCtrl
            Next childControl
            Return Nothing
        End Function ' Function FindChildControlById(ByVal controlToStartWith As Control, ByVal controlIdToFind As String) As Control

    End Module

    Partial Class CaseDialogue
        Inherits System.Web.UI.UserControl

        Private Const COOKIE_COMMENT_SORT_ORDER As String = "commentOrder"
        Shared _roleDao As IUserRoleDao
        Shared _userdao As IUserDao
        Private _casecomments As ICaseCommentsDao
        Private _dao As ILineOfDutyDao
        Private _daoFactory As IDaoFactory
        Private _emailDao As IEmailDao
        Private _lod As LineOfDuty
        Private _userGroupDao As IUserGroupDao
        Private _workstatusDao As IWorkStatusDao

#Region "Properties"

        ReadOnly Property LODDao() As ILineOfDutyDao
            Get
                If (_dao Is Nothing) Then
                    _dao = DaoFactory.GetLineOfDutyDao()
                End If

                Return _dao
            End Get
        End Property

        ReadOnly Property WorkstatusDao() As IWorkStatusDao
            Get
                If (_workstatusDao Is Nothing) Then
                    _workstatusDao = DaoFactory.GetWorkStatusDao()
                End If

                Return _workstatusDao
            End Get
        End Property

        Public Shared ReadOnly Property Dao() As IUserDao
            Get
                If _userdao Is Nothing Then
                    _userdao = New NHibernateDaoFactory().GetUserDao()
                End If

                Return _userdao
            End Get
        End Property

        Public Shared ReadOnly Property roleDao() As IUserRoleDao
            Get
                If _roleDao Is Nothing Then
                    _roleDao = New NHibernateDaoFactory().GetUserRoleDao()
                End If

                Return _roleDao
            End Get
        End Property

        Public Property CommentsTypeIdentifier() As Integer
            Get
                Dim key As String = "commentType"
                If (ViewState(key) Is Nothing) Then
                    ViewState(key) = 0
                End If
                Return CInt(ViewState(key))
            End Get
            Set(value As Integer)
                ViewState("commentType") = value
            End Set
        End Property

        Public Property IsDialogue() As Boolean
            Get
                Dim key As String = "IsDialogue"
                If (ViewState(key) Is Nothing) Then
                    ViewState(key) = False
                End If
                Return CInt(ViewState(key))
            End Get
            Set(value As Boolean)
                ViewState("IsDialogue") = value
            End Set
        End Property

        Public Property LessonsLearned() As Boolean
            Get
                Dim key As String = "LessonsLearned"
                If (ViewState(key) Is Nothing) Then
                    ViewState(key) = False
                End If
                Return CInt(ViewState(key))
            End Get
            Set(value As Boolean)
                ViewState("LessonsLearned") = value
            End Set
        End Property

        Public Property ModuleId() As Integer
            Get
                Dim key As String = "moduleId"
                If (ViewState(key) Is Nothing) Then
                    ViewState(key) = 0
                End If
                Return CInt(ViewState(key))
            End Get
            Set(value As Integer)
                ViewState("moduleId") = value
            End Set
        End Property

        Public ReadOnly Property refId() As Integer
            Get
                Return Integer.Parse(Request.QueryString("refId"))
            End Get
        End Property

        Public Property RequestId() As Integer
            Get
                Dim key As String = "requestId"
                If (ViewState(key) Is Nothing) Then
                    ViewState(key) = 0
                End If
                Return CInt(ViewState(key))
            End Get
            Set(value As Integer)
                ViewState("requestId") = value
            End Set
        End Property

        Public Property UserCanEdit() As Boolean
            Get
                If (ViewState("UserCanEdit") Is Nothing) Then
                    ViewState("UserCanEdit") = False
                End If
                Return CBool(ViewState("UserCanEdit"))
            End Get
            Set(value As Boolean)
                ViewState("UserCanEdit") = value
            End Set
        End Property

        Protected ReadOnly Property CaseCommentsDao() As ICaseCommentsDao
            Get
                If (_casecomments Is Nothing) Then
                    _casecomments = DaoFactory.GetCaseCommentsDao()
                End If
                Return _casecomments
            End Get
        End Property

        Protected ReadOnly Property DaoFactory As IDaoFactory
            Get
                If (_daoFactory Is Nothing) Then
                    _daoFactory = New NHibernateDaoFactory()
                End If

                Return _daoFactory
            End Get
        End Property

        Protected ReadOnly Property EmailDao() As IEmailDao
            Get
                If (_emailDao Is Nothing) Then
                    _emailDao = New EmailService()
                End If
                Return _emailDao
            End Get
        End Property

        Protected ReadOnly Property LOD() As LineOfDuty
            Get
                If (_lod Is Nothing) Then
                    _lod = LODDao.GetById(refId, False)

                End If
                Return _lod
            End Get
        End Property

        Protected ReadOnly Property PersonImage() As String
            Get
                Return Me.ResolveClientUrl("~/Images/circle-round-person.png")
            End Get
        End Property

        Protected ReadOnly Property UserGroupDao As IUserGroupDao
            Get
                If (_userGroupDao Is Nothing) Then
                    _userGroupDao = DaoFactory.GetUserGroupDao()
                End If

                Return _userGroupDao
            End Get
        End Property

#End Region

#Region "Load"

        Public Sub Initialize(ByVal hostPage As Page, ByVal modId As Integer, ByVal refId As Integer, ByVal navigator As TabNavigator, ByVal BoardComments As Boolean, ByVal CaseDialogue As Boolean)
            ModuleId = modId
            RequestId = refId
            LessonsLearned = BoardComments
            IsDialogue = CaseDialogue

            If (ModuleId = ModuleType.LOD) Then
                UserCanEdit = GetAccessLOD(navigator.PageAccess, True, Services.LodService.GetById(refId))
            Else
                UserCanEdit = GetAccess(navigator.PageAccess, True)
            End If

            AddUnitCommentButton.Visible = True

            ScriptManager.GetCurrent(hostPage).RegisterAsyncPostBackControl(Me)
        End Sub

        Protected Sub LoadCaseDialogueRepeater()
            CaseDialogueRepeater.DataSource = CaseCommentsDao.GetByCaseDialogue(RequestId, ModuleId, 9, SortCheckBox.Checked)
            CaseDialogueRepeater.DataBind()
            If (CaseDialogueRepeater.Items.Count < 1) Then
                divCaseDialogueRepeater.Visible = False
            Else
                divCaseDialogueRepeater.Visible = True
            End If
        End Sub

        Protected Sub LoadData(ByVal commentType As Integer)
            If (commentType = CommentsTypes.UnitComments OrElse commentType = CommentsTypes.BoardComments OrElse commentType = CommentsTypes.SysAdminComments OrElse commentType = CommentsTypes.CaseDialogue) Then
                LoadCaseDialogueRepeater()
            End If

        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If (Not IsPostBack) Then
                SetInputFormatRestriction(Page, CommentsBox, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
                DetermineCommentsType()
                InitCommentsControls()
            End If
        End Sub

        Private Sub DetermineCommentsType()
            Dim userGroup As UserGroup = UserGroupDao.GetById(SESSION_GROUP_ID)

            If (IsDialogue) Then
                CommentsTypeIdentifier = CommentsTypes.CaseDialogue
                lblCommentType.Text = "Case Dialogue Comments"
            ElseIf (Not LessonsLearned) Then
                CommentsTypeIdentifier = userGroup.GroupLevel.GetAssociatedCommentsType()
                lblCommentType.Text = GetCommnetTypeLabel(userGroup.GroupLevel.GetAssociatedCommentsType())
            Else
                CommentsTypeIdentifier = CommentsTypes.LessonsLearned
                lblCommentType.Text = "Lessons Learned Comments"
            End If
        End Sub

        Private Function GetCommnetTypeLabel(commentsType As CommentsTypes) As String
            Select Case commentsType
                Case CommentsTypes.UnitComments
                    Return "Unit Comments"

                Case CommentsTypes.BoardComments
                    Return "Board Comments"

                Case CommentsTypes.SysAdminComments
                    Return "Sys Admin Comments"

                Case CommentsTypes.CaseDialogue
                    Return "Case Dialogue Comments"
            End Select
            '6/21/19
            Return "Unknown"
        End Function

        Private Sub InitCommentsControls()
            Dim sortNewest As Boolean = True
            Boolean.TryParse(UserPreferences.GetSetting(COOKIE_COMMENT_SORT_ORDER), sortNewest)
            SortCheckBox.Checked = sortNewest
            LoadData(CommentsTypeIdentifier)
        End Sub

#End Region

#Region "UserAction"

        Public Shared Sub SendAccountModifiedEmail(adminId As Integer, userId As Integer, Applink As String, Activity As String)
            Try
                Dim mailService As New EmailService()
                Dim manager As New MailManager(mailService)
                Dim user As AppUser = Dao.GetById(userId)
                Dim otherRoles As String = user.AllRolesString()
                Dim toList As StringCollection = mailService.GetEmailListForUser(userId)
                '24--Status Changed Email
                manager.AddTemplate(CType(LODTemplate.LODAwaitingAction, Integer), "", toList)
                manager.SetField("NAME", user.SignatureName)
                manager.SetField("ROLE", user.CurrentRoleName)
                'manager.SetField("STATUS", user.StatusDescription)
                'manager.SetField("OTHER_ROLES", otherRoles)
                manager.SetField("APP_LINK", Applink)
                manager.SetField("ACTIVITY", Activity)

                manager.SendAll()
            Catch ex As Exception
                ' Show the exception.
                LogManager.LogError(ex, "SendAccountModifiedEmail failed: " & ex.Message)
            End Try
        End Sub

        Protected Sub btnReplyParent_Click(sender As Object, e As EventArgs)
            Try
                Dim row As RepeaterItem = TryCast(TryCast(sender, Button).NamingContainer, RepeaterItem)
                Dim lblchildCommentid As TextBox = CType(row.FindControl("lb1COmmenId"), TextBox)
                Dim txtCommentParent As TextBox = CType(row.FindControl("textCommentReplyParent"), TextBox)
                If (txtCommentParent.Text IsNot String.Empty) Then
                    Dim ChildCommentText As String = txtCommentParent.Text.Trim()

                    If (row.ItemIndex > -1) Then
                        Dim id As Integer
                        Integer.TryParse(lblchildCommentid.Text, id)
                        Dim data As CaseDialogueComments = CaseCommentsDao.GetDialogueById(id)
                        Dim role As String = HttpContext.Current.Session("Group")

                        Dim cs As String = ConfigurationManager.ConnectionStrings("LOD").ConnectionString
                        Dim con As SqlConnection = New SqlConnection(cs)
                        Dim cmd As SqlCommand = New SqlCommand("core_ChildCaseComments_SaveOrUpdateComment", con)
                        cmd.CommandType = CommandType.StoredProcedure
                        cmd.Parameters.AddWithValue("@id", 0)
                        cmd.Parameters.AddWithValue("@refId", data.caseId)
                        cmd.Parameters.AddWithValue("@module", data.module)
                        cmd.Parameters.AddWithValue("@comment", Server.HtmlEncode(ChildCommentText))
                        cmd.Parameters.AddWithValue("@userId", SESSION_USER_ID)
                        cmd.Parameters.AddWithValue("@createdDate", DateTime.Now)
                        cmd.Parameters.AddWithValue("@commentType", data.commentType)
                        cmd.Parameters.AddWithValue("@commentId", id)
                        cmd.Parameters.AddWithValue("@role", role)
                        con.Open()
                        cmd.ExecuteNonQuery()
                        con.Close()

                        ' Send Enail to Group
                        Dim user As AppUser
                        user = data.createdBy
                        SendAccountModifiedEmail(0, user.Id, GetHostName(), "Case Dialogue")

                        LoadData(CommentsTypeIdentifier)
                    End If

                End If
            Catch ex As Exception
                ' Show the exception.
                LogManager.LogError(ex, "btnReplyParent_Click failed: " & ex.Message)
            End Try

        End Sub

        Protected Sub btnUpdateChildComment_Click(sender As Object, e As EventArgs)
            Try
                Dim row As RepeaterItem = TryCast(TryCast(sender, Button).NamingContainer, RepeaterItem)
                ' Inner Repeater Control
                Dim lblchildCommentid As TextBox = CType(row.FindControl("lb1ChildCOmmenId"), TextBox)
                Dim txtCommentChild As TextBox = CType(row.FindControl("textUpdateChildComment"), TextBox)
                If (txtCommentChild.Text IsNot String.Empty) Then
                    Dim ChildCommentText As String = txtCommentChild.Text.Trim()

                    If (row.ItemIndex > -1) Then
                        Dim id As Integer
                        Integer.TryParse(lblchildCommentid.Text, id)
                        Dim data As CaseDialogueComments = CaseCommentsDao.GetDialogueById(id)
                        Dim role As String = HttpContext.Current.Session("Group")

                        Dim cs As String = ConfigurationManager.ConnectionStrings("LOD").ConnectionString
                        Dim con As SqlConnection = New SqlConnection(cs)
                        Dim cmd As SqlCommand = New SqlCommand("core_ChildCaseComments_UpdateComment", con)
                        cmd.CommandType = CommandType.StoredProcedure
                        cmd.Parameters.AddWithValue("@id", id)
                        cmd.Parameters.AddWithValue("@comment", Server.HtmlEncode(ChildCommentText))
                        cmd.Parameters.AddWithValue("@createdDate", DateTime.Now)
                        con.Open()
                        cmd.ExecuteNonQuery()
                        con.Close()

                        ' Send Enail to Group
                        Dim user As AppUser
                        user = data.createdBy
                        SendAccountModifiedEmail(0, user.Id, GetHostName(), "Case Dialogue")

                        LoadData(CommentsTypeIdentifier)
                    End If
                End If
            Catch ex As Exception
                ' Show the exception.
                LogManager.LogError(ex, "btnUpdateChildComment_Click failed: " & ex.Message)
            End Try
        End Sub

        Protected Sub commentRepeater_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles CaseDialogueRepeater.ItemCommand
            Dim id As Integer
            Integer.TryParse(e.CommandArgument(), id)
            Dim data As CaseDialogueComments = CaseCommentsDao.GetDialogueById(id)
            Dim role As String = HttpContext.Current.Session("Group")

            If (e.CommandName = "Delete") Then
                If (data.Id > 0 AndAlso e.Item.ItemIndex > -1) Then
                    CaseCommentsDao.SaveOrUpdateDialogue(data.Id, data.caseId, data.module, data.comment, data.createdBy.Id, data.createdDate, 1, data.commentType, role)
                    LoadData(CommentsTypeIdentifier)
                End If
            End If

            If (e.CommandName = "Edit") Then
                If (data.Id > 0 AndAlso e.Item.ItemIndex > -1) Then
                    CaseCommentsDao.SaveOrUpdateDialogue(data.Id, data.caseId, data.module, Server.HtmlEncode(CommentsBox.Text), data.createdBy.Id, DateTime.Now, 0, data.commentType, role)
                    LoadData(CommentsTypeIdentifier)
                End If
            End If

        End Sub

        Protected Sub commentRepeater_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles CaseDialogueRepeater.ItemDataBound
            Dim user As AppUser
            Dim data As CaseDialogueComments
            Try
                If (e.Item.ItemIndex > -1) Then
                    data = e.Item.DataItem
                    Dim id As Integer
                    Integer.TryParse(data.Id, id)

                    If (e.Item.ItemIndex > -1) Then
                        data = e.Item.DataItem

                        If (Not CanUserViewDeletedComment(data)) Then
                            e.Item.FindControl("mainDiv").Visible = False
                            Exit Sub
                        End If

                        Dim dateLabel As Label = e.Item.FindControl("dateLabel")
                        dateLabel.Text = data.createdDate

                        user = data.createdBy
                        Dim userLabel As Label = e.Item.FindControl("createdByLabel")
                        userLabel.Text = user.CommentName

                        Dim commentLabel As Label = e.Item.FindControl("commentLabel")
                        commentLabel.Text = data.comment

                        If (data.deleted) Then
                            userLabel.CssClass = "labelRequired CreatedByLabel"
                            dateLabel.CssClass = "labelRequired"
                            commentLabel.CssClass = "labelRequired"
                            commentLabel.Text = "DELETED: " + data.comment
                        End If

                        Dim RoleLabel As Label = e.Item.FindControl("RoleLabel")
                        RoleLabel.Text = IIf(data.role IsNot Nothing, data.role + "|", "")

                        commentLabel.Text = ReplaceNewlinesWithHtmlBrTag(commentLabel.Text)

                        'Dim e1 As Control = e.Item
                        'Dim e2 As Control = e.Item

                        'If (UserCanEdit And Not SESSION_USER_ID = user.Id) Then
                        '    'e.Item.FindControl("lnkReplyParent").Visible = True
                        '    'e.Item.FindControl("lnkCancel").Visible = True
                        '    e1.Page.FindChildControlById("lnkReplyParent").Visible = True
                        '    e2.Page.FindChildControlById("lnkCancel").Visible = True
                        'End If

                        ' Hide if DELETED
                        If (data.deleted) Then
                            e.Item.FindControl("pnlDelete").Visible = False
                        End If

                        ' Inner Repeater Control
                        Dim reptrCaseDialogue As Repeater = e.Item.FindControl("CaseDialogueRepeater2")

                        reptrCaseDialogue.DataSource = CaseCommentsDao.GetChildByCase(data.caseId, data.module, 9, data.Id)
                        reptrCaseDialogue.DataBind()

                        ' Hide or Show Child elements
                        For Each outerItem As RepeaterItem In CaseDialogueRepeater.Items
                            If Not (outerItem Is Nothing) Then
                                For Each childItem As RepeaterItem In reptrCaseDialogue.Items
                                    If (SESSION_USER_ID = user.Id) Then
                                        childItem.FindControl("pnlDelete2").Visible = True
                                        childItem.FindControl("lnkReplyParent2").Visible = True
                                        childItem.FindControl("lnkCancel2").Visible = True
                                    Else
                                        childItem.FindControl("pnlDelete2").Visible = False
                                        'childItem.FindControl("lnkReplyParent").Visible = False
                                        'childItem.FindControl("lnkCancel").Visible = False
                                    End If
                                Next
                            End If
                        Next

                    End If
                End If
            Catch ex As Exception
                ' Show the exception.
                LogManager.LogError(ex, "commentRepeater_ItemDataBound failed: " & ex.Message)
            End Try
        End Sub

        Protected Sub SaveButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles SaveButton.Click
            Dim role As String = HttpContext.Current.Session("Group")
            CaseCommentsDao.SaveOrUpdateDialogue(0, RequestId, ModuleId, Server.HtmlEncode(CommentsBox.Text), SESSION_USER_ID, DateTime.Now, 0, CommentsTypeIdentifier, role)
            LoadData(CommentsTypeIdentifier)
            CommentsBox.Text = String.Empty
        End Sub

        Protected Sub SortCheckBox_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles SortCheckBox.CheckedChanged
            UserPreferences.SaveSetting(COOKIE_COMMENT_SORT_ORDER, SortCheckBox.Checked)
            LoadData(CommentsTypeIdentifier)
        End Sub

        Private Function CanUserViewDeletedComment(comment As CaseDialogueComments) As Boolean
            If (Not comment.deleted) Then
                Return True
            End If

            If (SESSION_GROUP_ID = UserGroups.SystemAdministrator) Then
                Return True
            End If

            If (SESSION_USER_ID = comment.createdBy.Id) Then
                Return True
            End If

            Return False
        End Function

        Private Function ReplaceNewlinesWithHtmlBrTag(text As String) As String
            Return text.Replace(Environment.NewLine, "<br>").Replace(vbLf, "<br>")
        End Function

#End Region

    End Class

End Namespace