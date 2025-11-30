Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Modules.SpecialCases
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Core.Utils
Imports ALOD.Data
Imports ALOD.Logging
Imports ALOD.Web.UserControls
Imports ALODWebUtility.Common

Namespace Web.Special_Case.MEB

    Partial Class Secure_sc_meb_MedTech
        Inherits System.Web.UI.Page

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

        Protected ReadOnly Property CalendarImage() As String
            Get
                Return Me.ResolveClientUrl("~/App_Themes/" + Page.Theme + "/Images/Calendar.gif")
            End Get
        End Property

#Region "MEBProperty"

        Private _scAccess As Dictionary(Of String, ALOD.Core.Domain.Workflow.PageAccessType)
        Dim dao As ISpecialCaseDAO

        Private sc As SC_MEB = Nothing
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

        Protected ReadOnly Property SpecCase() As SC_MEB
            Get
                If (sc Is Nothing) Then
                    sc = SCDao.GetById(ReferenceId)
                End If

                Return sc
            End Get
        End Property

#End Region

        Protected ReadOnly Property MasterPage() As SC_MEBMaster
            Get
                Dim master As SC_MEBMaster = CType(Page.Master, SC_MEBMaster)
                Return master
            End Get
        End Property

        Protected ReadOnly Property SectionList() As Dictionary(Of String, ALOD.Core.Domain.Workflow.PageAccessType)
            Get
                If (_scAccess Is Nothing) Then
                    _scAccess = SpecCase.ReadSectionList(CInt(Session("GroupId")))
                End If
                Return _scAccess
            End Get
        End Property

        Private ReadOnly Property RefId() As Integer
            Get
                Return CInt(Request.QueryString("refId"))
            End Get
        End Property

        Protected Sub DisplayReadOnly(ByVal ShowStoppers As Integer)

            'Date
            MEBNotificationDateTextBox.Visible = False
            If (sc.MemberNotifiedDate.HasValue) Then

                MEBNotificationDateLabel.Text = String.Format(": {0}", sc.MemberNotifiedDate.Value.ToString(DATE_FORMAT))

            End If

        End Sub

        Protected Sub DisplayReadWrite()

            'Date
            MEBNotificationDateTextBox.CssClass = "datePicker"

            If (SpecCase.MemberNotifiedDate.HasValue) Then
                MEBNotificationDateTextBox.Text = Server.HtmlDecode(sc.MemberNotifiedDate.Value.ToString(DATE_FORMAT))
            End If

        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If (Not IsPostBack) Then
                UserCanEdit = GetAccess(Navigator.PageAccess, True)
                InitControls()

                'Validation
                If (UserCanEdit) Then
                    sc.Validate()
                    If (sc.Validations.Count > 0) Then
                        ShowPageValidationErrors(sc.Validations, Me)
                    End If
                End If

                LogManager.LogAction(ModuleType.SpecCaseMEB, UserAction.ViewPage, RefId, "Viewed Page: Med Tech")

            End If

        End Sub

        Private Sub InitControls()

            SetInputFormatRestriction(Page, MEBNotificationDateTextBox, FormatRestriction.Numeric, "/")

            DisplayReadWrite()

            If Not UserCanEdit Then
                DisplayReadOnly(0)
            End If
        End Sub

        Private Sub SaveFindings()

            If (Not UserCanEdit) Then
                Exit Sub
            End If

            If (SpecCase.Status = SpecCaseMEBWorkStatus.MedTechInput) Then

                'Date | Member Notification Date
                Dim dateValue As DateTime
                If (DateTime.TryParse(MEBNotificationDateTextBox.Text.Trim, dateValue)) Then
                    sc.MemberNotifiedDate = Server.HtmlEncode(dateValue)
                End If

                SCDao.SaveOrUpdate(SpecCase)
                SCDao.CommitChanges()

            End If

        End Sub

#Region "TabEvent"

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            AddHandler Me.Master.TabClick, AddressOf TabButtonClicked
        End Sub

        Protected Sub Save_Click(ByRef sender As Object, ByRef e As TabNavigationEventArgs)

            If (Navigator.CurrentStep.IsReadOnly) Then
                Exit Sub
            End If

            If (e.ButtonType = NavigatorButtonType.Save OrElse e.ButtonType = NavigatorButtonType.NavigatedAway _
                OrElse e.ButtonType = NavigatorButtonType.NextStep OrElse e.ButtonType = NavigatorButtonType.PreviousStep) Then
                SaveFindings()
            End If

        End Sub

        Private Sub TabButtonClicked(ByRef sender As Object, ByRef e As TabNavigationEventArgs)

            If (e.ButtonType = NavigatorButtonType.Save OrElse
                e.ButtonType = NavigatorButtonType.NavigatedAway OrElse
                e.ButtonType = NavigatorButtonType.NextStep OrElse
                e.ButtonType = NavigatorButtonType.PreviousStep) Then
                SaveFindings()
            End If

        End Sub

#End Region

    End Class

End Namespace