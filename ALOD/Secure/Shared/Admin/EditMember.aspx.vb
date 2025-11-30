Imports ALOD.Core.Domain.Lookup
Imports ALOD.Core.Domain.ServiceMembers
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Data.Services
Imports ALOD.Logging
Imports ALODWebUtility.Common

Namespace Web.Admin

    Partial Class Secure_Shared_Admin_EditMember
        Inherits System.Web.UI.Page

        Protected Const ACTION As String = "onclick"
        Protected Const KEY_ERROR_MESSAGE As String = "ERROR_MESSAGE"
        Protected Const KEY_FEEDBACK_MESSAGE As String = "FEEDBACK_MESSAGE"
        Protected Const MSG_ERROR_MEMBER_EDITED As String = "Error updating member. "
        Protected Const MSG_MEMBER_EDITED As String = "Member data saved."
        Protected Const PAGE_MEMBER_SEARCH As String = "~/Secure/Shared/Admin/MemberData.aspx"
        Protected Const SCRIPT_FUNCTION As String = "showSearcher('Select New Unit'); return false;"
        Protected Const SECTION_RANK As String = "Rank"
        Protected Const SECTION_UNIT As String = "Unit"
        Protected Const SECTION_USER_DATA As String = "Member Data"
        Protected editMember As ServiceMember

        Private _daoFactory As IDaoFactory
        Private _lookupDao As ILookupDao
        Private _memberDao As IMemberDao

        Protected ReadOnly Property DaoFactory As IDaoFactory
            Get
                If (_daoFactory Is Nothing) Then
                    _daoFactory = New NHibernateDaoFactory()
                End If

                Return _daoFactory
            End Get
        End Property

        Protected Property ErrorMessage() As String
            Get
                If (ViewState(KEY_ERROR_MESSAGE) Is Nothing) Then
                    Return String.Empty
                End If
                Return CStr(ViewState(KEY_ERROR_MESSAGE))
            End Get
            Set(ByVal value As String)
                ViewState(KEY_ERROR_MESSAGE) = value
            End Set
        End Property

        Protected Property FeedbackMessage() As String
            Get
                If (ViewState(KEY_FEEDBACK_MESSAGE) Is Nothing) Then
                    Return String.Empty
                End If
                Return CStr(ViewState(KEY_FEEDBACK_MESSAGE))
            End Get
            Set(ByVal value As String)
                ViewState(KEY_FEEDBACK_MESSAGE) = value
            End Set
        End Property

        Protected ReadOnly Property LookupDao() As ILookupDao
            Get
                If (_lookupDao Is Nothing) Then
                    _lookupDao = DaoFactory.GetLookupDao
                End If

                Return _lookupDao
            End Get
        End Property

        Protected ReadOnly Property Member() As ServiceMember
            Get
                If (editMember Is Nothing) Then
                    If Not (Session(SESSIONKEY_EDIT_MEMBER_SSAN) Is Nothing) Then
                        editMember = MemberDao.FindById(Session(SESSIONKEY_EDIT_MEMBER_SSAN))
                    Else
                        editMember = Nothing
                    End If

                End If

                Return editMember
            End Get
        End Property

        Protected ReadOnly Property MemberDao() As IMemberDao
            Get
                If (_memberDao Is Nothing) Then
                    _memberDao = DaoFactory.GetMemberDao()
                End If

                Return _memberDao
            End Get
        End Property

        Protected Sub gdvMILPDSChangeHistory_PageIndexChanging(sender As Object, e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles gdvMILPDSChangeHistory.PageIndexChanging
            gdvMILPDSChangeHistory.PageIndex = e.NewPageIndex
            LoadMILPDSChangeHistory()
        End Sub

        Protected Sub gdvSystemAdminChangeHistory_PageIndexChanging(sender As Object, e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles gdvSystemAdminChangeHistory.PageIndexChanging
            gdvSystemAdminChangeHistory.PageIndex = e.NewPageIndex
            LoadSystemAdminChangeHistory()
        End Sub

        Protected Sub InitChangeUnitButton()
            If (UserHasPermission(PERMISSION_SYSTEM_ADMIN)) Then
                ChangeUnitButton.Visible = True
                ChangeUnitButton.Attributes.Add(ACTION, SCRIPT_FUNCTION)
            Else
                ChangeUnitButton.Visible = False
            End If
        End Sub

        Protected Sub InitControls()
            InitRankSelectDropDownList()
            LoadMemeber()
            InitChangeUnitButton()
            LoadMILPDSChangeHistory()
            LoadSystemAdminChangeHistory()
        End Sub

        Protected Sub InitRankSelectDropDownList()
            RankSelect.DataSource = LookupDao.GetRanks()
            RankSelect.DataTextField = "Name"
            RankSelect.DataValueField = "Value"
            RankSelect.DataBind()
        End Sub

        Protected Sub lnkManageMembers_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkManageMembers.Click
            Session(SESSIONKEY_EDIT_MEMBER_SSAN) = Nothing
            Response.Redirect(PAGE_MEMBER_SEARCH)
        End Sub

        Protected Sub LoadMemeber()
            Try
                NameLbl.Text = Server.HtmlEncode(Member.FullName)
                If Member.Rank IsNot Nothing Then
                    SetDropdownByValue(RankSelect, Member.Rank.Id)
                End If
                lblCompo.Text = Server.HtmlEncode(Utility.GetCompoString(Member.Component))
                UnitTextBox.Text = Server.HtmlEncode(Member.Unit.Name)
                If (Member.DateOfBirth.HasValue) Then
                    DobLbl.Text = Server.HtmlEncode(Member.DateOfBirth.Value.ToString(DATE_FORMAT))
                End If

                If Member.MailingAddress IsNot Nothing Then
                    AddressLbl.Text = Member.MailingAddress.FullAddress()
                End If
                DutyPhoneLbl.Text = Member.DutyPhone
            Catch ex As Exception
                LogManager.LogError(ex)
            End Try
        End Sub

        Protected Sub LoadMILPDSChangeHistory()
            If (Member Is Nothing) Then
                Exit Sub
            End If

            pnlMILPDSChangeHistory.Visible = True
            gdvMILPDSChangeHistory.DataSource = MemberDao.GetMILPDSChangeHistoryBySSN(Member.SSN)
            gdvMILPDSChangeHistory.DataBind()
        End Sub

        Protected Sub LoadSystemAdminChangeHistory()
            If (Member Is Nothing) Then
                Exit Sub
            End If

            pnlSystemAdminChangeHistory.Visible = True
            gdvSystemAdminChangeHistory.DataSource = MemberDao.GetSystemAdminChangeHistoryBySSN(Member.SSN)
            gdvSystemAdminChangeHistory.DataBind()
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If (Session(SESSIONKEY_EDIT_MEMBER_SSAN) Is Nothing) Then
                Response.Redirect(Resources._Global.StartPage, True)
            End If

            If (Not IsPostBack) Then
                InitControls()
            End If
        End Sub

        Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            ErrorPanel.Visible = ErrorMessage.Length > 0
            ErrorMessageLabel.Text = ErrorMessage
            FeedbackMessageLabel.Text = FeedbackMessage
            FeedbackPanel.Visible = FeedbackMessage.Length > 0
        End Sub

        Protected Sub SaveMemberButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles SaveMemberButton.Click
            ErrorMessage = String.Empty
            FeedbackMessage = String.Empty
            Dim changes As New ChangeSet()

            Dim rankId As Integer = 0

            If (Not UserHasPermission(PERMISSION_SYSTEM_ADMIN)) Then
                Exit Sub
            End If

            Integer.TryParse(RankSelect.SelectedValue, rankId)
            Dim rank As UserRank = LookupService.GetRank(rankId)

            Dim newUnit As ALOD.Core.Domain.Users.Unit
            Dim newUnitId As Integer = -1
            If (Not Integer.TryParse(newUnitIDLabel.Text.Trim, newUnitId)) Then
                newUnit = Nothing
            Else
                newUnit = LookupService.GetUnitById(newUnitId)
            End If

            If (newUnit Is Nothing) AndAlso (rank Is Nothing) Then
                Exit Sub
            End If

            If (rankId <> Member.Rank.Id) Then
                changes.Add(SECTION_USER_DATA, SECTION_RANK, Member.Rank.Title, rank.Title)
                Member.Rank = rank
            End If

            If (newUnit IsNot Nothing) AndAlso (newUnitId <> Member.Unit.Id) Then
                changes.Add(SECTION_USER_DATA, SECTION_UNIT, Member.Unit.Name, newUnit.Name)
                Member.Unit = newUnit
                UnitTextBox.Text = newUnit.Name
            End If

            If changes.Count > 0 Then
                With _memberDao
                    Try
                        .Save(Member)
                        .CommitChanges()
                        Dim logId As Integer = LogManager.LogAction(ModuleType.System, UserAction.ModifyMemberData, SESSION_USER_ID, "Changed member data for  " + Member.FullName + "  SSAN " + Member.SSN)
                        changes.Save(logId)
                        FeedbackMessage = MSG_MEMBER_EDITED
                    Catch ex As Exception
                        ErrorMessage = MSG_ERROR_MEMBER_EDITED
                    End Try
                End With
            End If
        End Sub

    End Class

End Namespace