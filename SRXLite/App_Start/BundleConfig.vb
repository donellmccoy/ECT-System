Imports System.Web.Optimization

Namespace Modules
    ''' <summary>
    ''' Configures script and style bundles for the SRXLite application.
    ''' Bundles improve performance by reducing HTTP requests and minifying resources.
    ''' </summary>
    Public Class BundleConfig
    ''' <summary>
    ''' Registers all script and style bundles for the application.
    ''' Call this method from Application_Start in Global.asax.
    ''' </summary>
    Public Shared Sub RegisterBundles(bundles As BundleCollection)
        ' Core jQuery library bundle (version 3.6.0)
        bundles.Add(New ScriptBundle("~/bundles/jquery").Include(
            "~/Scripts/jquery-3.6.0.min.js"))

        ' Core CSS bundle (SRXLite uses a simpler theme structure)
        bundles.Add(New StyleBundle("~/Content/css").Include(
            "~/styles.css"))

        ' Enable optimizations even in debug mode (for testing)
        ' Set to False to use unminified versions during development
#If DEBUG Then
        BundleTable.EnableOptimizations = False
#Else
        BundleTable.EnableOptimizations = True
#End If
    End Sub
    End Class
End Namespace
