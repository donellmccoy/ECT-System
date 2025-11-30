Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.Modules.SpecialCases
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data

Namespace Web.Special_Case.DW

    Partial Class Secure_SC_DW_Tracking
        Inherits System.Web.UI.Page

        Private _dao As ISpecialCaseDAO
        Private _daoFactory As IDaoFactory
        Private _details As IQueryable(Of TrackingItem)
        Private _special As SpecialCase

        ReadOnly Property SCDao() As ISpecialCaseDAO
            Get
                If (_dao Is Nothing) Then
                    _dao = DaoFactory.GetSpecialCaseDAO()
                End If

                Return _dao
            End Get
        End Property

        Public ReadOnly Property ModuleType() As ModuleType
            Get
                Return ModuleType.SpecCaseDW
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
                Return CInt(Request.QueryString("refId"))
            End Get
        End Property

        Protected ReadOnly Property SpecCase() As SpecialCase
            Get
                If (_special Is Nothing) Then
                    _special = SCDao.GetById(RequestId, False)

                End If
                Return _special
            End Get

        End Property

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If (Not IsPostBack) Then

                CaseTracking.Initialize(Me, ModuleType, RequestId, SpecCase.CaseId, SpecCase.Status, SpecCase.MemberUnitId)

            End If
        End Sub

    End Class

End Namespace