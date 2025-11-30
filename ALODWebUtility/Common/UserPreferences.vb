Imports System.Web
Imports ALOD.Core.Utils

Namespace Common

    Public Class UserPreferences

        Public Shared Sub DeleteSetting(ByVal key As String)

            Dim request As HttpRequest = HttpContext.Current.Request

            Dim cookie As HttpCookie

            If (request.Cookies("userPrefs") Is Nothing) Then
                cookie = New HttpCookie("userPrefs")
            Else
                cookie = request.Cookies("userPrefs")
            End If

            cookie(key) = ""

            If (AppMode = DeployMode.Production) Then
                cookie.Secure = True
            End If

            HttpContext.Current.Response.Cookies.Add(cookie)

        End Sub

        Public Shared Function GetSetting(ByVal key As String, Optional ByVal defaultValue As String = "") As String

            Dim request As HttpRequest = HttpContext.Current.Request

            If (request.Cookies("userPrefs") IsNot Nothing) Then
                If (request.Cookies("userPrefs")(key) IsNot Nothing) Then
                    Return request.Cookies("userPrefs")(key)
                End If
            End If

            Return defaultValue

        End Function

        Public Shared Sub SaveSetting(ByVal key As String, ByVal value As String)

            Dim request As HttpRequest = HttpContext.Current.Request

            Dim cookie As HttpCookie

            If (request.Cookies("userPrefs") Is Nothing) Then
                cookie = New HttpCookie("userPrefs")
            Else
                cookie = request.Cookies("userPrefs")
            End If

            cookie.HttpOnly = True
            cookie.Expires = DateTime.Now.AddYears(1)

            If (AppMode = DeployMode.Production) Then
                cookie.Secure = True
            End If

            cookie(key) = value

            HttpContext.Current.Response.Cookies.Add(cookie)

        End Sub

    End Class

End Namespace