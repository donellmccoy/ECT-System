Namespace Providers

    Public Class LodSiteMapProvider
        Inherits System.Web.XmlSiteMapProvider

        Public Overrides Function IsAccessibleToUser(ByVal context As System.Web.HttpContext, ByVal node As System.Web.SiteMapNode) As Boolean

            If (node Is Nothing) Then
                Throw New System.ArgumentNullException("node")
            End If

            If (context Is Nothing) Then
                Throw New System.ArgumentNullException("context")
            End If

            If (Not Me.SecurityTrimmingEnabled) Then
                Return True
            End If

            Dim nodeRoles As IList = node.Roles
            Dim user As System.Security.Principal.IPrincipal = context.User

            If (user Is Nothing) OrElse (nodeRoles Is Nothing) OrElse (nodeRoles.Count = 0) Then
                Return True
            End If

            If (nodeRoles.Contains("*")) Then
                Return True
            End If

            For Each role As String In nodeRoles
                If (user.IsInRole(role)) Then
                    Return True
                End If
            Next

            Return False

        End Function

    End Class

End Namespace