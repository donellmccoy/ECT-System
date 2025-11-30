Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.Modules.Reinvestigations
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data

Namespace Web.RR

    Partial Class Secure_rr_Tracking
        Inherits System.Web.UI.Page

        Private _dao As ILODReinvestigateDAO
        Private _daoFactory As IDaoFactory
        Private _details As IQueryable(Of TrackingItem)
        Private _request As LODReinvestigation

        ReadOnly Property RRDao() As ILODReinvestigateDAO
            Get
                If (_dao Is Nothing) Then
                    _dao = DaoFactory.GetLODReinvestigationDao()
                End If

                Return _dao
            End Get
        End Property

        Public ReadOnly Property ModuleType() As ModuleType
            Get
                Return ModuleType.ReinvestigationRequest
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

        Protected ReadOnly Property RequestId() As Integer
            Get
                Return CInt(Request.QueryString("requestId"))
            End Get
        End Property

        Protected ReadOnly Property RR() As LODReinvestigation
            Get
                If (_request Is Nothing) Then
                    _request = RRDao.GetById(RequestId, False)

                End If
                Return _request
            End Get
        End Property

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If (Not IsPostBack) Then

                CaseTracking.Initialize(Me, ModuleType, RequestId, RR.CaseId, RR.Status, GetUnitId())

            End If
        End Sub

        Private Function GetUnitId() As Integer

            Dim LOD = Services.LodService.GetById(RR.InitialLodId)

            If (LOD.isAttachPas) Then
                Return LOD.MemberAttachedUnitId
            Else
                Return LOD.MemberUnitId
            End If

        End Function

    End Class

End Namespace