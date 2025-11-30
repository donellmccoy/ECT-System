Imports System.Security.Principal
Imports System.Text

Namespace Providers

    <CLSCompliant(False)>
    Public Class LodPrinciple
        Implements IPrincipal

        Protected _identity As IIdentity
        Protected _roles As List(Of String)

        Public ReadOnly Property Permissions() As String
            Get
                Dim buffer As New StringBuilder

                For Each perm As String In _roles
                    buffer.Append(perm + ",")
                Next

                If (buffer.Length > 0) Then
                    buffer = buffer.Remove(buffer.Length - 1, 1)
                End If

                Return buffer.ToString()
            End Get
        End Property

#Region "IPrinciple"

        Public ReadOnly Property Identity() As System.Security.Principal.IIdentity Implements System.Security.Principal.IPrincipal.Identity
            Get
                Return Me._identity
            End Get
        End Property

        Public Function IsInRole(ByVal role As String) As Boolean Implements System.Security.Principal.IPrincipal.IsInRole
            Return _roles.Contains(role)
        End Function

#End Region

#Region "Constructors"

        Public Sub New(ByVal identity As IIdentity)
            _identity = identity
            _roles = New List(Of String)
        End Sub

        Public Sub New(ByVal identity As IIdentity, ByVal roles As List(Of String))
            _identity = identity
            _roles = roles
        End Sub

        Public Sub New(ByVal identity As IIdentity, ByVal roles As String())
            _identity = identity
            _roles = New List(Of String)

            For Each item As String In roles
                _roles.Add(item)
            Next
        End Sub

        Public Sub New(ByVal identity As IIdentity, ByVal roles As String, ByVal seperator As Char)
            _identity = identity
            _roles = New List(Of String)

            Dim parts As String() = roles.Split(New Char() {seperator})

            For Each item As String In parts
                _roles.Add(item)
            Next

        End Sub

#End Region

    End Class

End Namespace