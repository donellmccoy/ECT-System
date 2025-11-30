Imports System.Configuration
Imports System.Text
Imports System.Web
Imports ALOD.Core.Utils
Imports ALODWebUtility.Common

Namespace Providers

    Public Class AuthProvider

        Const _delimiter As String = "|"
        Const _paramCount As Short = 3
        Const AUTHCOOKIE_KEY As String = "authCookieName"
        Const LOGINURL_KEY As String = "loginUrl"

        ''' <summary>
        ''' Given an encrypted string from the authentication cookie returns a User object
        ''' </summary>
        ''' <param name="encrypted">The encrypted cookie value</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function Decrypt(ByVal encrypted As String) As UserAuthentication

            Dim user As UserAuthentication = Nothing

            Try
                Dim decryptedString As String = LodCrypto.Decrypt(encrypted)
                Dim parts As String() = decryptedString.Split(New String() {_delimiter}, StringSplitOptions.None)

                If (parts.Length = _paramCount) Then

                    Dim authed As Boolean = False
                    Boolean.TryParse(parts(2), authed)

                    user = New UserAuthentication(parts(0), parts(1), authed)

                End If
            Catch ex As Exception
                Throw ex
            End Try

            Return user

        End Function

        ''' <summary>
        ''' Encrypts a User object to a string for storing in a cookie
        ''' </summary>
        ''' <param name="user">The User object to store</param>
        ''' <returns>The encrypted string</returns>
        ''' <remarks>This stores the user name and permissions for the current user</remarks>
        Public Shared Function Encrypt(ByVal user As UserAuthentication) As String

            Dim encryptedString As String = String.Empty

            Try
                Dim buffer As New StringBuilder

                buffer.Append(user.UserName + _delimiter)
                buffer.Append(user.Roles + _delimiter)
                buffer.Append(user.IsAuthenticated.ToString())

                encryptedString = LodCrypto.Encrypt(buffer.ToString())
            Catch ex As Exception

            End Try

            Return encryptedString

        End Function

        Public Shared Sub SetAuthCookie(ByVal identity As UserAuthentication)

            Dim encrypted As String = Encrypt(identity)
            Dim cookieName As String = ConfigurationManager.AppSettings(AUTHCOOKIE_KEY)
            Dim cookie As New HttpCookie(cookieName, encrypted)

            If (AppMode = DeployMode.Production) Then
                cookie.Secure = True
            End If

            HttpContext.Current.Response.Cookies.Add(cookie)

        End Sub

    End Class

End Namespace