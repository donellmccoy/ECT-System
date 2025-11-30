Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.Modules.SARC
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data

Namespace Web.APSA

    Partial Class Secure_apsa_Tracking
        Inherits System.Web.UI.Page

        Private _appeal As SARCAppeal
        Private _dao As ISARCAppealDAO
        Private _daoFactory As IDaoFactory
        Private _details As IQueryable(Of TrackingItem)
        Private _sarc As RestrictedSARC
        Private _sarcdao As ISARCDAO

        ReadOnly Property APSADao() As ISARCAppealDAO
            Get
                If (_dao Is Nothing) Then
                    _dao = DaoFactory.GetSARCAppealDao()
                End If

                Return _dao
            End Get
        End Property

        ReadOnly Property SADao() As ISARCDAO
            Get
                If (_sarcdao Is Nothing) Then
                    _sarcdao = DaoFactory.GetSARCDao()
                End If

                Return _sarcdao
            End Get
        End Property

        Public ReadOnly Property ModuleType() As ModuleType
            Get
                Return ModuleType.SARCAppeal
            End Get

        End Property

        Protected ReadOnly Property APSA() As SARCAppeal
            Get
                If (_appeal Is Nothing) Then
                    _appeal = APSADao.GetById(RequestId, False)

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

        Protected ReadOnly Property SA() As RestrictedSARC
            Get
                If (_sarc Is Nothing) Then
                    _sarc = SADao.GetById(RequestId, False)

                End If
                Return _sarc
            End Get
        End Property

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If (Not IsPostBack) Then

                CaseTracking.Initialize(Me, ModuleType, RequestId, APSA.CaseId, APSA.Status, GetUnitId())

            End If
        End Sub

        Private Function GetUnitId() As Integer

            If (APSA.InitialWorkflow = AFRCWorkflows.SARCRestricted) Then
                Return SA.MemberUnitId
            Else

                Dim LOD = Services.LodService.GetById(APSA.InitialId)

                If (LOD.isAttachPas) Then
                    Return LOD.MemberAttachedUnitId
                Else
                    Return LOD.MemberUnitId
                End If

            End If

        End Function

    End Class

End Namespace