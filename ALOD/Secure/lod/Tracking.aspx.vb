Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data

Namespace Web.LOD

    Partial Class Secure_lod_Tracking
        Inherits System.Web.UI.Page

        Private _dao As ILineOfDutyDao
        Private _daoFactory As IDaoFactory
        Private _details As IQueryable(Of TrackingItem)
        Private _lod As LineOfDuty

        ReadOnly Property LODDao() As ILineOfDutyDao
            Get
                If (_dao Is Nothing) Then
                    _dao = DaoFactory.GetLineOfDutyDao()
                End If

                Return _dao
            End Get
        End Property

        Public ReadOnly Property ModuleType() As ModuleType
            Get
                Return ModuleType.LOD
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

        Protected ReadOnly Property LOD() As LineOfDuty
            Get
                If (_lod Is Nothing) Then
                    _lod = LODDao.GetById(RequestId, False)

                End If
                Return _lod
            End Get
        End Property

        Protected ReadOnly Property RequestId() As Integer
            Get
                Return CInt(Request.QueryString("refId"))
            End Get
        End Property

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If (Not IsPostBack) Then

                CaseTracking.Initialize(Me, ModuleType, RequestId, LOD.CaseId, LOD.Status, GetUnitId())

            End If
        End Sub

        Private Function GetUnitId() As Integer

            If (LOD.isAttachPas) Then
                Return LOD.MemberAttachedUnitId
            Else
                Return LOD.MemberUnitId
            End If

        End Function

    End Class

End Namespace