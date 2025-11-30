Imports ALOD.Core.Domain.Documents
Imports ALOD.Core.Domain.Modules.SpecialCases
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Web.UserControls

Namespace Web.Special_Case.CMAS

    Partial Class Secure_sc_cm_Documents
        Inherits System.Web.UI.Page

#Region "Fields"

        Private _DocumentDao As IDocumentDao
        Private _factory As IDaoFactory
        Private _special As SpecialCase
        Private _SpecialDao As ISpecialCaseDAO

#End Region

#Region "Properties"

        ReadOnly Property DaoFactory() As IDaoFactory
            Get
                If (_factory Is Nothing) Then
                    _factory = New NHibernateDaoFactory()
                End If

                Return _factory
            End Get
        End Property

        ReadOnly Property DocumentDao() As IDocumentDao
            Get
                If (_DocumentDao Is Nothing) Then
                    _DocumentDao = New SRXDocumentStore(CStr(HttpContext.Current.Session("UserName")))
                End If

                Return _DocumentDao
            End Get
        End Property

        ReadOnly Property SCDao() As ISpecialCaseDAO
            Get
                If (_SpecialDao Is Nothing) Then
                    _SpecialDao = DaoFactory.GetSpecialCaseDAO()
                End If

                Return _SpecialDao
            End Get
        End Property

        Public ReadOnly Property Navigator() As TabNavigator
            Get
                Return Master.Navigator
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

#End Region

#Region "Page Methods"

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If (Not IsPostBack) Then

                'make sure we have a groupId
                If (SpecCase.DocumentGroupId Is Nothing OrElse SpecCase.DocumentGroupId = 0) Then
                    SpecCase.CreateDocumentGroup(DocumentDao)
                    SCDao.SaveOrUpdate(SpecCase)  'Save the new Document Group ID with the Special Case
                    SCDao.CommitChanges()
                End If

                SpecCase.ProcessDocuments(DaoFactory)
                Documents.Initialize(Me, Navigator, New WorkflowDocument(SpecCase, 1, DaoFactory))

            End If

        End Sub

#End Region

    End Class

End Namespace