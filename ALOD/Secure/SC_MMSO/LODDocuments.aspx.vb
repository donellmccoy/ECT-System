Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Modules.SpecialCases
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Data.Services
Imports ALOD.Web.UserControls

Namespace Web.Special_Case.MMSO

    Partial Class Secure_sc_mmso_lod_Documents
        Inherits System.Web.UI.Page

        Private _assocaiated As IAssociatedCaseDao
        Private _factory As IDaoFactory
        Private _lod As LineOfDuty = Nothing
        Dim dao As ISpecialCaseDAO
        Private sc As SC_MMSO = Nothing
        Private scId As Integer = 0

        ReadOnly Property associated() As IAssociatedCaseDao
            Get
                If (_assocaiated Is Nothing) Then
                    _assocaiated = factory.GetAssociatedCaseDao()
                End If

                Return _assocaiated
            End Get
        End Property

        ReadOnly Property factory() As IDaoFactory
            Get
                If (_factory Is Nothing) Then
                    _factory = New NHibernateDaoFactory()
                End If

                Return _factory
            End Get
        End Property

        ReadOnly Property instance() As LineOfDuty
            Get
                If (_lod Is Nothing) Then
                    _lod = LodService.GetById(associated.GetAssociatedCasesLOD(SpecCase.Id, SpecCase.Workflow).First.associated_RefId)
                End If
                Return _lod
            End Get
        End Property

        ReadOnly Property SCDao() As ISpecialCaseDAO
            Get
                If (dao Is Nothing) Then
                    dao = factory.GetSpecialCaseDAO()
                End If

                Return dao
            End Get
        End Property

        Public ReadOnly Property Navigator() As TabNavigator
            Get
                Return MasterPage.Navigator
            End Get
        End Property

        Protected ReadOnly Property MasterPage() As SC_MMSOMaster
            Get
                Dim master As SC_MMSOMaster = CType(Page.Master, SC_MMSOMaster)
                Return master
            End Get
        End Property

        Protected ReadOnly Property SpecCase() As SC_MMSO
            Get
                If (sc Is Nothing) Then
                    sc = SCDao.GetById(Request.QueryString("refId"))
                End If

                Return sc
            End Get
        End Property

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Dim HasAdminLod As Boolean

            If (SpecCase.HasAdminLOD Is Nothing) Then
                HasAdminLod = False
            Else
                HasAdminLod = SpecCase.HasAdminLOD
            End If

            If (associated.GetAssociatedCasesLOD(SpecCase.Id, SpecCase.Workflow).Count = 0) Then
                ucLODDocuments.Initialize(Me, 0, HasAdminLod, Navigator)
            Else

                ucLODDocuments.Initialize(Me, instance.Id, HasAdminLod, Navigator)
            End If

        End Sub

    End Class

End Namespace