Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Modules.SpecialCases
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data

Namespace Web.Special_Case.PSCD

    Partial Class Secure_PSCD_Tracking
        Inherits System.Web.UI.Page

        Private _dao As ILineOfDutyDao
        Private _daoFactory As IDaoFactory
        Private _details As IQueryable(Of TrackingItem)
        Private _lod As LineOfDuty

        Private _special As SpecialCase
        Private _Specialdao As ISpecialCaseDAO

        ReadOnly Property SCDao() As ISpecialCaseDAO
            Get
                If (_Specialdao Is Nothing) Then
                    _Specialdao = DaoFactory.GetSpecialCaseDAO()
                End If

                Return _Specialdao
            End Get
        End Property

        Public ReadOnly Property ModuleType() As ModuleType
            Get
                Return ModuleType.SpecCasePSCD
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

                CaseTracking.Initialize(Me, ModuleType, RequestId, SpecCase.CaseId, SpecCase.Status, GetUnitId())

            End If
        End Sub

        Private Function GetUnitId() As Integer
            'CodeCleanUp
            'If (LOD.isAttachPas) Then
            '    Return LOD.MemberAttachedUnitId
            'Else
            '    Return LOD.MemberUnitId
            'End If

            Return SpecCase.MemberUnitId
        End Function

    End Class

End Namespace