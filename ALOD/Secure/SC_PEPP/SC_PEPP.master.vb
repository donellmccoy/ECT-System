Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Modules.SpecialCases
Imports ALOD.Core.Domain.Users
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Web.UserControls
Imports ALODWebUtility
Imports ALODWebUtility.Common

Namespace Web.Special_Case.PEPP

    Partial Class SC_PEPPMaster
        Inherits System.Web.UI.MasterPage

        Private _dao As ISpecialCaseDAO
        Private _sc As SC_PEPP = Nothing
        Private _scId As Integer = 0

        Public Delegate Sub TabClickEventHandler(ByRef sender As Object, ByRef e As TabNavigationEventArgs)

        Public Event TabClick As TabClickEventHandler

        ReadOnly Property SCDao() As ISpecialCaseDAO
            Get
                If (_dao Is Nothing) Then
                    _dao = New NHibernateDaoFactory().GetSpecialCaseDAO()
                End If

                Return _dao
            End Get
        End Property

        Public ReadOnly Property GroupId As Byte
            Get
                Return CByte(Session("GroupId"))
            End Get
        End Property

        Public ReadOnly Property ModHeader() As ModuleHeader
            Get
                Return ModuleHeader1
            End Get
        End Property

        Public ReadOnly Property Navigator() As TabNavigator
            Get
                Return TabNavigator1
            End Get
        End Property

        Public ReadOnly Property NestedHolder() As ContentPlaceHolder
            Get
                Return CType(Me.Master.FindControl("ContentMain").FindControl("ContentNested"), ContentPlaceHolder)
            End Get
        End Property

        Public ReadOnly Property TabControl() As TabControls
            Get
                Return TabControls1
            End Get
        End Property

        Protected ReadOnly Property SpecCase() As SC_PEPP
            Get
                If (_sc Is Nothing) Then
                    _sc = SCDao.GetById(Request.QueryString("RefId"))
                End If

                Return _sc
            End Get
        End Property

        Public Function ConstructSeniorMedicalReviewerTabVisibilityArgs() As SeniorMedicalReviewerTabVisibilityArgs
            Dim args As SeniorMedicalReviewerTabVisibilityArgs

            Dim workStatuses As List(Of Integer) = New List(Of Integer)
            workStatuses.Add(SpecCasePEPPWorkStatus.SeniorMedicalReview)

            args.RefId = SpecCase.Id
            args.ModuleId = SpecCase.moduleId
            args.WorkStatusIds = workStatuses
            args.TabTitle = "PEPP Senior Med"
            args.Steps = Navigator.Steps

            Return args
        End Function

        Public Function GetStartPageTitle(ByVal workStatus As SpecCasePEPPWorkStatus) As String
            Dim startPage As String = "PEPP Member"

            Select Case workStatus
                Case SpecCasePEPPWorkStatus.InitiateCase
                    If (GroupId = UserGroups.AFRCHQTechnician) Then
                        startPage = "PEPP HQ AFRC Tech"
                    End If
                Case SpecCasePEPPWorkStatus.MedicalReview
                    If (GroupId = UserGroups.BoardMedical) Then
                        startPage = "PEPP Board Med"
                    End If
                Case Else
                    startPage = "PEPP Member"
            End Select

            If (Not Navigator(startPage).Visible) Then
                startPage = "PEPP Member"
            End If

            Return startPage
        End Function

        Public Sub InitPageAccess(ByVal specCase As SC_PEPP)
            Dim accessDao As IPageAccessDao = New NHibernateDaoFactory().GetPageAccessDao()

            Navigator.ClearSession()
            Navigator.InitControl()
            Navigator.SetPageAccess(accessDao.GetByWorkflowGroupAndStatus(specCase.Workflow, GroupId, specCase.Status))

            Utility.InitSeniorMedicalReviewerTabVisibility(ConstructSeniorMedicalReviewerTabVisibilityArgs())
        End Sub

        Public Function LoadCase(ByVal refId As Integer) As SC_PEPP
            Dim scPEPP As SC_PEPP = Nothing

            Try
                If _scId <> refId Then
                    _sc = Nothing  'Ensure we grab based on refId
                    _scId = refId
                End If
                scPEPP = SpecCase
            Catch ex As Exception
                'we failed to load the case, nothing we can do
                ALOD.Logging.LogManager.LogError(ex.ToString() + " \ refId: " + refId.ToString())
                SetErrorMessage(Resources.Messages.ERROR_LOADING_CASE)
                Response.Redirect(Resources._Global.StartPage, True)
            End Try

            If (scPEPP Is Nothing) Then
                'we failed to load the case, nothing we can do
                ALOD.Logging.LogManager.LogError("NULL Physical Examination Processing Program Error loading Physical Examination Processing Program with requestId: " + refId.ToString())
                SetErrorMessage(Resources.Messages.ERROR_LOADING_CASE)
                Response.Redirect(Resources._Global.StartPage, True)
            End If

            Return scPEPP
        End Function

        Public Sub SetSubWorkflowType(ByVal scPEPP As SC_PEPP)
            If scPEPP.SubWorkflowType = ALOD.Data.Services.LookupService.GetSCSubWorkflowTypeId("Physical Examination Processing Program (PE)") Then
                Session("SubWorkflowType") = "(PEPP) "
            End If
            If scPEPP.SubWorkflowType = ALOD.Data.Services.LookupService.GetSCSubWorkflowTypeId("AIMWITS (AW)") Then
                Session("SubWorkflowType") = "(AIMWITS) "
            End If
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Dim refId As Integer = 0
            Integer.TryParse(Request.QueryString("RefId"), refId)

            If (refId = 0) Then
                Response.Redirect(Resources._Global.StartPage, True)
            End If

            Dim sessionId As Integer = 0
            Integer.TryParse(Session("RefId"), sessionId)

            If (sessionId = 0) Then
                Response.Redirect(Resources._Global.StartPage, True)
            End If

            If (refId <> sessionId) Then
                InitNavigator(refId)
            End If

            ModHeader.ModuleHeader = "PE"
        End Sub

        Private Sub InitNavigator(ByVal refId As Integer)
            Dim factory As New NHibernateDaoFactory()
            Dim userAccess As PageAccess.AccessLevel = PageAccess.AccessLevel.None
            Dim scPEPP As New SC_PEPP

            Session("refId") = refId

            userAccess = SCDao.GetUserAccess(SESSION_USER_ID, refId, ModuleType.SpecCasePEPP)

            Utility.VerifyUserAccess(userAccess, Resources.Messages.ERROR_NO_ACCESS, Resources._Global.StartPage)

            scPEPP = LoadCase(refId)

            Utility.UpdateCaseLock(userAccess, refId, ModuleType.SpecCasePEPP)
            InitPageAccess(scPEPP)

            Navigator.Commit()

            SetSubWorkflowType(scPEPP)

            SCDao.Evict(scPEPP)
        End Sub

        Private Sub TabButtonClicked(ByRef sender As Object, ByRef e As TabNavigationEventArgs) Handles TabNavigator1.ButtonClicked

            RaiseEvent TabClick(Me, e)

        End Sub

    End Class

End Namespace