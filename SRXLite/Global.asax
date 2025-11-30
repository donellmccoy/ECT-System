<%@ Application Language="VB" %>
<%@ Import Namespace="System.Web.Optimization" %>
<%@ Import Namespace="SRXLite.Modules" %>

<script runat="server">

    Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' Code that runs on application startup
        
        ' Register script and style bundles for optimization
        BundleConfig.RegisterBundles(BundleTable.Bundles)
    End Sub
    
    Sub Application_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Code that runs on application shutdown
    End Sub
        
    Sub Application_Error(ByVal sender As Object, ByVal e As EventArgs)
		' Code that runs when an unhandled error occurs
		Dim errorID As Integer = LogError(FormatLastError())
		
		'Error will be cleared on the redirect page
		'Redirect set in web.config customErrors section
		HttpContext.Current.Server.GetLastError.Data.Add("ErrorID", errorID)
	End Sub

    Sub Session_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' Code that runs when a new session is started
    End Sub
       
</script>