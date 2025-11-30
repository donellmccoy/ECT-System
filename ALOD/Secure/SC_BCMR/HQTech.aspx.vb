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

Namespace Web.Special_Case.BCMR

    Partial Class Secure_sc_bc_HQTech
        Inherits System.Web.UI.Page

#Region "LODProperty"

        Private _scAccess As Dictionary(Of String, ALOD.Core.Domain.Workflow.PageAccessType)
        Dim dao As ISpecialCaseDAO

        Private sc As SC_BCMR = Nothing
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

        Protected ReadOnly Property CalendarImage() As String
            Get
                Return Me.ResolveClientUrl("~/App_Themes/" + Page.Theme + "/Images/Calendar.gif")
            End Get
        End Property

        Protected ReadOnly Property SpecCase() As SC_BCMR
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

        Protected ReadOnly Property MasterPage() As SC_BCMRMaster
            Get
                Dim master As SC_BCMRMaster = CType(Page.Master, SC_BCMRMaster)
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

            If (IsPostBack) Then
                'no need to populate these more than once
                Exit Sub
            End If

            'Disable the edit controls
            SuspenseDate.Enabled = False
            SuspenseDate.CssClass = ""
            TMTNumber.Enabled = False
            TMTRecDate.Enabled = False
            TMTRecDate.CssClass = ""

        End Sub

        Protected Sub DisplayReadWrite()

            If (SpecCase.SuspenseDate.HasValue) Then
                SuspenseDate.Text = Server.HtmlDecode(SpecCase.SuspenseDate.Value.ToString(DATE_FORMAT))
            End If
            If (SpecCase.TMTReceiveDate.HasValue) Then
                TMTRecDate.Text = Server.HtmlDecode(SpecCase.TMTReceiveDate.Value.ToString(DATE_FORMAT))
            End If
            TMTNumber.Text = Server.HtmlDecode(SpecCase.TMTNumber)

            ValidBoxLength()
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

            End If

            TabControl.Item(NavigatorButtonType.Save).Visible = UserCanEdit
            LogManager.LogAction(ModuleType.SpecCaseBCMR, UserAction.ViewPage, RefId, "Viewed Page: BC HQ AFRC Tech")

            If Session(SESSIONKEY_COMPO) = "5" Then
                lblTMTNumber.Text = "CMTARS"
                lblAFRC.Text = "BC"
                lblTMTRecDate.Text = "CMTARS Receive Date"
                lblHeader.Text = "ANGRC Technician - BCMR"
            End If

        End Sub

        Private Sub InitControls()

            SetInputFormatRestriction(Page, TMTNumber, FormatRestriction.AlphaNumeric, PERMITTED_SPECIAL_CHAR_INPUT)
            SetInputFormatRestriction(Page, TMTRecDate, FormatRestriction.Numeric, "/")
            SetInputFormatRestriction(Page, SuspenseDate, FormatRestriction.Numeric, "/")

            DisplayReadWrite()

            If UserCanEdit Then
                TMTRecDate.CssClass = "datePicker"
                SuspenseDate.CssClass = "datePickerFuture"
            Else
                DisplayReadOnly(0)
            End If
        End Sub

        Private Sub SaveFindings()

            If (Not UserCanEdit) Then
                Exit Sub
            End If

            Dim PageAccess As ALOD.Core.Domain.Workflow.PageAccessType
            PageAccess = SectionList(BCSectionNames.BC_HQT_INIT.ToString())

            If SpecCase.Status = SpecCaseBCWorkStatus.InitiateCase Then
                SpecCase.TMTNumber = Server.HtmlEncode(TMTNumber.Text)

                'Date Field
                Try
                    If (CheckDate(SuspenseDate)) Then
                        If (SuspenseDate.Text.Trim.Length > 0) Then
                            SpecCase.SuspenseDate = Server.HtmlEncode(DateTime.Parse(SuspenseDate.Text.Trim))
                        End If
                    Else
                        SpecCase.SuspenseDate = Nothing
                    End If
                Catch ex As Exception
                    SpecCase.SuspenseDate = Nothing
                End Try

                'Date Field
                Try
                    If (CheckDate(TMTRecDate)) Then
                        If (TMTRecDate.Text.Trim.Length > 0) Then
                            SpecCase.TMTReceiveDate = Server.HtmlEncode(DateTime.Parse(TMTRecDate.Text.Trim))
                        End If
                    Else
                        SpecCase.TMTReceiveDate = Nothing
                    End If
                Catch ex As Exception
                    SpecCase.TMTReceiveDate = Nothing
                End Try

                SCDao.SaveOrUpdate(SpecCase)
                SCDao.CommitChanges()
            End If
        End Sub

#Region "TabEvent"

        Public Function ValidBoxLength() As Boolean

            Dim IsValid As Boolean = True

            If Not CheckTextLength(SuspenseDate) Then
                IsValid = False
            End If
            If Not CheckTextLength(TMTRecDate) Then
                IsValid = False
            End If

            Return IsValid

        End Function

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