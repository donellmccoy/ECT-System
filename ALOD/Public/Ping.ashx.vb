Public Class Ping
    Implements System.Web.IHttpHandler

    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

    Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest

        context.Response.ContentType = "text/plain"
        context.Response.Write(Now.Ticks)

    End Sub

End Class