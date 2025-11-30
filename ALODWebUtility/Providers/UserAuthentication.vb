Imports System.Security.Principal
Imports System.Text
Imports System.Web.Security
Imports ALODWebUtility.Permission

Namespace Providers

    Public Class UserAuthentication
        Inherits MembershipUser
        Implements IIdentity

#Region "Members/Properties"

        Protected _isAuthenticated As Boolean = False
        Protected _perms As List(Of String) = New List(Of String)
        Protected _userName As String = String.Empty
        Const _delimiter As String = ","

        Public ReadOnly Property Permissions() As List(Of String)
            Get
                Return _perms
            End Get
        End Property

        Public Property Roles() As String
            Get
                Dim buffer As New StringBuilder

                For Each item As String In _perms
                    buffer.Append(item + _delimiter)
                Next

                If (buffer.Length > 0) Then
                    buffer.Remove(buffer.Length - 1, 1)
                End If

                Return buffer.ToString()

            End Get

            Set(ByVal value As String)

                For Each item As String In value.Split(New String() {_delimiter}, StringSplitOptions.RemoveEmptyEntries)
                    _perms.Add(item)
                Next

            End Set
        End Property

        Public Overrides ReadOnly Property UserName() As String
            Get
                Return _userName
            End Get
        End Property

#End Region

#Region "Constructors"

        Public Sub New(ByVal userName As String, ByVal roles As String, ByVal isAuthed As Boolean)
            _userName = userName
            Me.Roles = roles
            _isAuthenticated = isAuthed
        End Sub

        Public Sub New(ByVal userName As String)
            _userName = userName
            LoadPermissions()
        End Sub

        Protected Sub New()

        End Sub

#End Region

#Region "Loading"

        Protected Sub LoadPermissions()

            Dim perms As New PermissionList()
            perms.GetByUserId(_userName)

            For Each perm As Permission.Permission In perms
                Permissions.Add(perm.Name)
            Next

            _isAuthenticated = True

        End Sub

#End Region

#Region "IIdentity"

        Public ReadOnly Property AuthenticationType() As String Implements System.Security.Principal.IIdentity.AuthenticationType
            Get
                Return "LodAuth"
            End Get
        End Property

        Public ReadOnly Property IsAuthenticated() As Boolean Implements System.Security.Principal.IIdentity.IsAuthenticated
            Get
                Return _isAuthenticated
            End Get
        End Property

        Public ReadOnly Property Name() As String Implements System.Security.Principal.IIdentity.Name
            Get
                Return _userName
            End Get
        End Property

#End Region

    End Class

End Namespace