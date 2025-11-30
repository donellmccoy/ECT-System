Imports ALOD.Core.Domain.Users
Imports ALOD.Core.Interfaces
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Data.Services

Namespace Web.DBSign

    Public Class receivesignature
        Inherits System.Web.UI.Page

        Private _daoFactory As IDaoFactory
        Private _memoSignature As IMemoSignatureDao

        ReadOnly Property MemoSignatureDao() As IMemoSignatureDao
            Get
                If (_memoSignature Is Nothing) Then
                    _memoSignature = DaoFactory.GetMemoSignatureDao()
                End If

                Return _memoSignature
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

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Dim sigdn As String = Request.Form("DBS_SIGNER_DN")
            Dim sig As String = ""

            If (sigdn.Length > 0) Then
                Dim Index = sigdn.IndexOf("cn=")
                Dim hold = sigdn.IndexOf(",ou=")

                If (Index > -1 AndAlso hold > -1) Then

                    sig = sigdn.Substring(Index, hold).Replace("cn=", "")

                End If
            End If

            Dim signDate As String = Request.Form("DBS_SIGN_DATE")

            'Dim signatureString As String = Request.Form("DBS_SIGNER_CERT")
            Dim refId As Integer = CInt(Request.QueryString("refId"))
            Dim workflow As Integer = CInt(Request.QueryString("workflow"))
            Dim ptype As Integer = CInt(Request.QueryString("ptype"))
            Dim user As AppUser = UserService.CurrentUser()

            If user Is Nothing Then
                Throw New Exception("User not found.")
            End If

            MemoSignatureDao.InsertSignature(refId, workflow, sig, signDate, user.Id, ptype)

        End Sub

    End Class

End Namespace