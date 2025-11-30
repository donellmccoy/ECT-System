Imports ALOD.Core.Domain.Common.KeyVal
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Core.Utils
Imports ALOD.Data

Namespace Web

    Public Class SecureAccessDenied
        Inherits System.Web.UI.Page

        Private _keyValDao As IKeyValDao

        ReadOnly Property KeyValDao() As IKeyValDao
            Get
                If (_keyValDao Is Nothing) Then
                    _keyValDao = New NHibernateDaoFactory().GetKeyValDao()
                End If

                Return _keyValDao
            End Get
        End Property

        Protected Property AdditionalComments() As String
            Get
                Return lblAdditionalComments.Text
            End Get
            Set(value As String)
                lblAdditionalComments.Text = value
            End Set
        End Property

        Protected Property DeniedSubject() As String
            Get
                Return lblDeniedSubject.Text
            End Get
            Set(value As String)
                lblDeniedSubject.Text = value
            End Set
        End Property

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If (Not IsPostBack) Then
                Dim deniedType As Integer = 0
                Integer.TryParse(Request.QueryString("deniedType"), deniedType)

                InitData(deniedType)
            End If
        End Sub

        Private Sub InitData(ByVal deniedType As Integer)
            Select Case deniedType
                Case AccessDeniedType.CaseDetails
                    Dim values As List(Of KeyValValue) = KeyValDao.GetKeyValuesByKeyDesciption("Access Denied - Case Details")
                    DeniedSubject = values(0).Value
                    AdditionalComments = values(1).Value
                Case Else
                    DeniedSubject = "You do not have access to this page for unknown reasons."
                    AdditionalComments = "Please contact the help desk for further assistance."
            End Select
        End Sub

    End Class

End Namespace