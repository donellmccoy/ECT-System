Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Users
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Core.Utils
Imports ALOD.Data
Imports ALODWebUtility.Common

Namespace Web.UserControls

    Partial Class Secure_Shared_UserControls_CaseComments
        Inherits System.Web.UI.UserControl

        Private Const COOKIE_COMMENT_SORT_ORDER As String = "commentOrder"
        Private _casecomments As ICaseCommentsDao
        Private _daoFactory As IDaoFactory
        Private _userGroupDao As IUserGroupDao

#Region "Properties"

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

        Public Sub Initialize(ByVal hostPage As Page, ByVal modId As Integer, ByVal refId As Integer, ByVal navigator As TabNavigator, ByVal BoardComments As Boolean)
            ModuleId = modId
            RequestId = refId
            LessonsLearned = BoardComments

            If (ModuleId = ModuleType.LOD) Then
                UserCanEdit = GetAccessLOD(navigator.PageAccess, True, Services.LodService.GetById(refId))
            Else
                UserCanEdit = GetAccess(navigator.PageAccess, True)
            End If
            Dim wsId As Int16 = SESSION_WS_ID(refId)

            If (wsId = 332 And (SESSION_GROUP_ID = 9 Or SESSION_GROUP_ID = 8)) Then ' 9 = Board Medical/ 8 = Board Legal/ 332 = ARC SME Consult
                AddUnitCommentButton.Visible = True
            Else
                AddUnitCommentButton.Visible = UserCanEdit
            End If

            ScriptManager.GetCurrent(hostPage).RegisterAsyncPostBackControl(Me)
        End Sub

        Protected Sub LoadBoardRepeater()
            BoardcommentRepeater.DataSource = CaseCommentsDao.GetByCase(RequestId, ModuleId, 3, SortCheckBox.Checked)
            BoardcommentRepeater.DataBind()
            If (BoardcommentRepeater.Items.Count < 1) Then
                divBoardRepeater.Visible = False
            Else
                divBoardRepeater.Visible = True
            End If
        End Sub

        Protected Sub LoadData(ByVal commentType As Integer)
            If (commentType = CommentsTypes.UnitComments OrElse commentType = CommentsTypes.BoardComments OrElse commentType = CommentsTypes.SysAdminComments) Then
                LoadSARepeater()
                LoadUnitRepeater()
            End If

            If (commentType = CommentsTypes.BoardComments OrElse commentType = CommentsTypes.SysAdminComments) Then
                LoadBoardRepeater()
            End If

            If (commentType = CommentsTypes.LessonsLearned) Then
                LoadLessonsRepeater()
            End If
        End Sub

        Protected Sub LoadLessonsRepeater()
            LessonsLearnedRepeater.DataSource = CaseCommentsDao.GetByCase(RequestId, ModuleId, 8, SortCheckBox.Checked)
            LessonsLearnedRepeater.DataBind()
            If (LessonsLearnedRepeater.Items.Count < 1) Then
                divLessonsRepeater.Visible = False
            Else
                divLessonsRepeater.Visible = True
            End If
        End Sub

        Protected Sub LoadSARepeater()
            SAcommentRepeater.DataSource = CaseCommentsDao.GetByCase(RequestId, ModuleId, 4, SortCheckBox.Checked)
            SAcommentRepeater.DataBind()
            If (SAcommentRepeater.Items.Count < 1) Then
                divsaRepeater.Visible = False
            Else
                divsaRepeater.Visible = True
            End If
        End Sub

        Protected Sub LoadUnitRepeater()
            UnitcommentRepeater.DataSource = CaseCommentsDao.GetByCase(RequestId, ModuleId, 2, SortCheckBox.Checked)
            UnitcommentRepeater.DataBind()
            If (UnitcommentRepeater.Items.Count < 1) Then
                divUnitRepeater.Visible = False
            Else
                divUnitRepeater.Visible = True
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

            If (Not LessonsLearned) Then
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

        Protected Sub CommentRepeater_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.RepeaterCommandEventArgs) Handles UnitcommentRepeater.ItemCommand, BoardcommentRepeater.ItemCommand, SAcommentRepeater.ItemCommand, LessonsLearnedRepeater.ItemCommand
            Dim id As Integer
            Integer.TryParse(e.CommandArgument(), id)
            Dim data As CaseComments = CaseCommentsDao.GetById(id)

            If (e.CommandName = "Delete") Then
                If (data.Id > 0 AndAlso e.Item.ItemIndex > -1) Then
                    CaseCommentsDao.SaveOrUpdate(data.Id, data.caseId, data.module, data.comment, data.createdBy.Id, data.createdDate, 1, data.commentType)
                    LoadData(CommentsTypeIdentifier)
                End If
            End If

            If (e.CommandName = "Edit") Then
                If (data.Id > 0 AndAlso e.Item.ItemIndex > -1) Then
                    CaseCommentsDao.SaveOrUpdate(data.Id, data.caseId, data.module, Server.HtmlEncode(CommentsBox.Text), data.createdBy.Id, DateTime.Now, 0, data.commentType)
                    LoadData(CommentsTypeIdentifier)
                End If
            End If
        End Sub

        Protected Sub CommentRepeater_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles SAcommentRepeater.ItemDataBound, BoardcommentRepeater.ItemDataBound, UnitcommentRepeater.ItemDataBound, LessonsLearnedRepeater.ItemDataBound
            Dim user As AppUser
            Dim data As CaseComments

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
                    commentLabel.Text = "DELETED: " + commentLabel.Text
                End If

                commentLabel.Text = ReplaceNewlinesWithHtmlBrTag(commentLabel.Text)

                If (SESSION_USER_ID = user.Id AndAlso UserCanEdit) Then
                    e.Item.FindControl("pnlDelete").Visible = True
                End If
            End If
        End Sub

        Protected Sub SaveButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles SaveButton.Click
            CaseCommentsDao.SaveOrUpdate(0, RequestId, ModuleId, Server.HtmlEncode(CommentsBox.Text), SESSION_USER_ID, DateTime.Now, 0, CommentsTypeIdentifier)
            LoadData(CommentsTypeIdentifier)
            CommentsBox.Text = String.Empty
        End Sub

        Protected Sub SortCheckBox_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles SortCheckBox.CheckedChanged
            UserPreferences.SaveSetting(COOKIE_COMMENT_SORT_ORDER, SortCheckBox.Checked)
            LoadData(CommentsTypeIdentifier)
        End Sub

        Private Function CanUserViewDeletedComment(comment As CaseComments) As Boolean
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