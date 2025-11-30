Imports ALOD.Core.Domain.Documents
Imports ALOD.Core.Domain.Modules.Appeals
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Web.UserControls

Namespace Web.AP

    Partial Class Secure_ap_lod_Documents
        Inherits System.Web.UI.Page

        Private _appealId As Integer = 0
        Private _DocumentDao As IDocumentDao
        Private _factory As IDaoFactory
        Private _LODAppealDAO As ILODAppealDAO
        Private ap As LODAppeal = Nothing

        ReadOnly Property APDao() As ILODAppealDAO
            Get
                If (_LODAppealDAO Is Nothing) Then
                    _LODAppealDAO = DaoFactory.GetLODAppealDao()
                End If

                Return _LODAppealDAO
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

        ReadOnly Property LODAppeal() As LODAppeal
            Get

                If (ap Is Nothing) Then
                    _appealId = RequestId

                    If _appealId <> 0 Then
                        ap = APDao.GetById(_appealId)
                    Else
                        ap = Nothing
                    End If
                End If
                Return ap
            End Get
        End Property

        ReadOnly Property RequestId() As Integer
            Get
                Return CInt(Request.QueryString("requestId"))
            End Get
        End Property

        Public ReadOnly Property Navigator() As TabNavigator
            Get
                Return MasterPage.Navigator
            End Get
        End Property

        Protected ReadOnly Property DaoFactory As IDaoFactory
            Get
                If (_factory Is Nothing) Then
                    _factory = New NHibernateDaoFactory()
                End If

                Return _factory
            End Get
        End Property

        Protected ReadOnly Property MasterPage() As AppealRequestMaster
            Get
                Dim master As AppealRequestMaster = CType(Page.Master, AppealRequestMaster)
                Return master
            End Get
        End Property

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If (Not LODAppeal.DocumentGroupId.HasValue OrElse LODAppeal.DocumentGroupId.Value = 0) Then
                LODAppeal.CreateDocumentGroup(DocumentDao)
                APDao.SaveOrUpdate(LODAppeal)
                APDao.CommitChanges()
            End If

            ucLODDocuments.Initialize(Me, LODAppeal.InitialLodId, False, Navigator)
        End Sub

    End Class

End Namespace