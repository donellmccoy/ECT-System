Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.Modules.SpecialCases
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Logging
Imports ALOD.Web.UserControls
Imports ALODWebUtility.Common

Namespace Web.Special_Case.MMSO

    Partial Class Secure_sc_mmso_SMData
        Inherits System.Web.UI.Page

        Protected Const KEY_REFID As String = "refId"
        Protected Const KEY_REFSTATUS As String = "refStatus"

        Public Delegate Function GetSpecialCasesDelegate(ByVal param As StringDictionary) As DataSet

        Public Delegate Function GetUnDeletedLodsDelegate(ByVal param As StringDictionary) As DataSet

#Region "LODProperty"

        Private _scAccess As Dictionary(Of String, ALOD.Core.Domain.Workflow.PageAccessType)
        Dim dao As ISpecialCaseDAO

        Private sc As SC_MMSO = Nothing
        Private scId As Integer = 0

        ReadOnly Property SCDao() As ISpecialCaseDAO
            Get
                If (dao Is Nothing) Then
                    dao = New NHibernateDaoFactory().GetSpecialCaseDAO()
                End If

                Return dao
            End Get
        End Property

        Public ReadOnly Property ReferenceId() As Integer
            Get
                Return Integer.Parse(Request.QueryString("refId"))
            End Get
        End Property

        Protected ReadOnly Property SpecCase() As SC_MMSO
            Get
                If (sc Is Nothing) Then
                    sc = SCDao.GetById(ReferenceId)
                End If

                Return sc
            End Get
        End Property

#End Region

        Public ReadOnly Property Navigator() As TabNavigator
            Get
                Return MasterPage.Navigator
            End Get
        End Property

        Public ReadOnly Property TabControl() As TabControls
            Get
                Return Master.TabControl
            End Get
        End Property

        Public Property UserCanEdit() As Boolean
            Get
                If (ViewState("CanEdit") Is Nothing) Then
                    ViewState("CanEdit") = False
                End If
                Return CBool(ViewState("CanEdit"))
            End Get
            Set(ByVal value As Boolean)
                ViewState("CanEdit") = value
            End Set
        End Property

        Protected ReadOnly Property MasterPage() As SC_MMSOMaster
            Get
                Dim master As SC_MMSOMaster = CType(Page.Master, SC_MMSOMaster)
                Return master
            End Get
        End Property

        Private ReadOnly Property RefId() As Integer
            Get
                Return CInt(Request.QueryString("refId"))
            End Get
        End Property

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            AddHandler Me.Master.TabClick, AddressOf TabButtonClicked
        End Sub

        '''<summary>
        '''<mod date="2011-02-28">The member case history grid is being filled by using asynchronlus delegate .
        '''</mod>
        '''</summary>
        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If (Not IsPostBack) Then

                SigCheck.VerifySignature(ReferenceId)
                Session("requestId") = ReferenceId

                'when this page loads we always want to clear out any previous navigator and start fresh
                Navigator.ClearSession()  'In case we came from a different module
                'and initialize our control
                Navigator.InitControl()

                'now set our page access for this session
                Dim access As IList(Of PageAccess)
                Dim groupId As Byte = CByte(Session("GroupId"))
                Dim accessDao As IPageAccessDao = New NHibernateDaoFactory().GetPageAccessDao()
                access = accessDao.GetByWorkflowGroupAndStatus(SpecCase.Workflow, groupId, SpecCase.Status)
                Navigator.SetPageAccess(access)

                SetPageAccess()
                GetData()

                TabControl.Item(NavigatorButtonType.Save).Visible = False

                If (UserHasPermission("sysAdmin")) Then
                    ChangeUnitButton.Attributes.Add("onclick", "showSearcher('Select New Unit'); return false;")
                Else
                    ChangeUnitButton.Visible = False
                End If

                LogManager.LogAction(ModuleType.SpecCaseMMSO, UserAction.ViewPage, ReferenceId, "Viewed Page: Member")

                CaseHistory.Initialize(Me, SpecCase.MemberSSN, SpecCase.CaseId, False)

            End If

        End Sub

        Protected Sub SetPageAccess()

            ''if we don't have a case lock aquired, this will be read-only regardless of pageacess
            If (Not SESSION_LOCK_AQUIRED) Then
                UserCanEdit = False
                Exit Sub
            End If

            'otherwise, we have a case lock, so check page access
            Dim access = (From t In Navigator.PageAccess Where t.PageTitle = "MB Member" Select t).SingleOrDefault()

            If (access Is Nothing) Then
                UserCanEdit = False
            Else
                If (access.Access = ALOD.Core.Domain.Workflow.PageAccess.AccessLevel.ReadWrite) Then
                    UserCanEdit = True
                Else
                    UserCanEdit = False
                End If
            End If

        End Sub

#Region "TabEvent"

        Private Sub TabButtonClicked(ByRef sender As Object, ByRef e As TabNavigationEventArgs)

            If (e.ButtonType = NavigatorButtonType.Save OrElse
                e.ButtonType = NavigatorButtonType.NavigatedAway OrElse
                e.ButtonType = NavigatorButtonType.NextStep OrElse
                e.ButtonType = NavigatorButtonType.PreviousStep) Then
            End If

        End Sub

#End Region

        Private Sub GetData()

            lblName.Text = Server.HtmlEncode(SpecCase.MemberName)
            If SpecCase.MemberRank IsNot Nothing Then
                lblRank.Text = Server.HtmlEncode(SpecCase.MemberRank.Title)
            End If
            lblCompo.Text = Server.HtmlEncode(Utility.GetCompoString(SpecCase.MemberCompo))
            UnitTextBox.Text = Server.HtmlEncode(SpecCase.MemberUnit)
            If (SpecCase.MemberDOB.HasValue) Then
                lbldob.Text = Server.HtmlEncode(SpecCase.MemberDOB.Value.ToString(DATE_FORMAT))
            End If

        End Sub

    End Class

End Namespace