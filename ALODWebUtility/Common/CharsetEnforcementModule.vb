Imports System.Web

Namespace Common
    ''' <summary>
    ''' HTTP Module to enforce UTF-8 charset on JavaScript resources served by ASP.NET handlers
    ''' </summary>
    Public Class CharsetEnforcementModule
        Implements IHttpModule

        Public Sub Init(context As HttpApplication) Implements IHttpModule.Init
            AddHandler context.PreSendRequestHeaders, AddressOf OnPreSendRequestHeaders
        End Sub

        Private Sub OnPreSendRequestHeaders(sender As Object, e As EventArgs)
            Dim app As HttpApplication = TryCast(sender, HttpApplication)
            If app IsNot Nothing AndAlso app.Context IsNot Nothing Then
                Dim response = app.Context.Response
                Dim request = app.Context.Request

                ' Check if this is a WebResource.axd or ScriptResource.axd request
                If request.Path.EndsWith("WebResource.axd", StringComparison.OrdinalIgnoreCase) OrElse
                   request.Path.EndsWith("ScriptResource.axd", StringComparison.OrdinalIgnoreCase) Then

                    ' Get the current Content-Type
                    Dim contentType = response.ContentType

                    ' If it's JavaScript and doesn't have charset, add it
                    If Not String.IsNullOrEmpty(contentType) Then
                        If (contentType.Contains("javascript") OrElse contentType.Contains("ecmascript")) AndAlso
                           Not contentType.Contains("charset") Then
                            response.ContentType = contentType & "; charset=utf-8"
                        End If
                    End If
                End If
            End If
        End Sub

        Public Sub Dispose() Implements IHttpModule.Dispose
            ' Nothing to dispose
        End Sub
    End Class
End Namespace
