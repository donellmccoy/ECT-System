Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.Modules.SARC
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data

Namespace Web.SARC

    Partial Class Tracking
        Inherits System.Web.UI.Page

        Private _dao As ISARCDAO
        Private _daoFactory As IDaoFactory
        Private _details As IQueryable(Of TrackingItem)
        Private _sarc As RestrictedSARC

        ReadOnly Property SADao() As ISARCDAO
            Get
                If (_dao Is Nothing) Then
                    _dao = DaoFactory.GetSARCDao()
                End If

                Return _dao
            End Get
        End Property

        Public ReadOnly Property ModuleType() As ModuleType
            Get
                Return ModuleType.SARC
            End Get

        End Property

        Public ReadOnly Property RequestId() As Integer
            Get
                Return CInt(Request.QueryString("refId"))
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

                CaseTracking.Initialize(Me, ModuleType, RequestId, SA.CaseId, SA.Status, SA.MemberUnitId)

            End If
        End Sub

    End Class

End Namespace