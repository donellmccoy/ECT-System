Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.Modules.Appeals
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data

Namespace Web.AP

    Partial Class Secure_ap_Tracking
        Inherits System.Web.UI.Page

        Private _appeal As LODAppeal
        Private _dao As ILODAppealDAO
        Private _daoFactory As IDaoFactory
        Private _details As IQueryable(Of TrackingItem)

        ReadOnly Property APDao() As ILODAppealDAO
            Get
                If (_dao Is Nothing) Then
                    _dao = DaoFactory.GetLODAppealDao()
                End If

                Return _dao
            End Get
        End Property

        Public ReadOnly Property ModuleType() As ModuleType
            Get
                Return ModuleType.AppealRequest
            End Get

        End Property

        Protected ReadOnly Property AP() As LODAppeal
            Get
                If (_appeal Is Nothing) Then
                    _appeal = APDao.GetById(RequestId, False)

                End If
                Return _appeal
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

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If (Not IsPostBack) Then

                CaseTracking.Initialize(Me, ModuleType, RequestId, AP.CaseId, AP.Status, GetUnitId())

            End If
        End Sub

        Private Function GetUnitId() As Integer

            Dim LOD = Services.LodService.GetById(AP.InitialLodId)

            If (LOD.isAttachPas) Then
                Return LOD.MemberAttachedUnitId
            Else
                Return LOD.MemberUnitId
            End If

        End Function

    End Class

End Namespace