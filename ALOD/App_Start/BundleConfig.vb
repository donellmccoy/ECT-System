Imports System.Web.Optimization

Namespace Web
    ''' <summary>
    ''' Configures script and style bundles for the ALOD application.
    ''' Bundles improve performance by reducing HTTP requests and minifying resources.
    ''' </summary>
    Public Class BundleConfig
    ''' <summary>
    ''' Registers all script and style bundles for the application.
    ''' Call this method from Application_Start in Global.asax.
    ''' </summary>
    Public Shared Sub RegisterBundles(bundles As BundleCollection)
        ' Core jQuery libraries bundle
        bundles.Add(New ScriptBundle("~/bundles/jquery").Include(
            "~/Script/jquery-3.6.0.min.js",
            "~/Script/jquery-migrate-3.4.1.min.js"))

        ' jQuery UI bundle (version 1.13.0)
        bundles.Add(New ScriptBundle("~/bundles/jqueryui").Include(
            "~/Script/jquery-ui-1.13.0.min.js",
            "~/Script/jquery.blockUI.min.js"))

        ' Common application scripts
        bundles.Add(New ScriptBundle("~/bundles/common").Include(
            "~/Script/jqModal.js",
            "~/Script/common.js"))

        ' Application-specific scripts
        bundles.Add(New ScriptBundle("~/bundles/app").Include(
            "~/Script/TimeoutDialog.js",
            "~/Script/tracking.js"))

        ' Core CSS bundle - COMMENTED OUT: Now using ASP.NET Theme (styleSheetTheme="DefaultBlue" in web.config)
        ' bundles.Add(New StyleBundle("~/Content/css").Include(
        '     "~/App_Themes/DefaultBlue/styles.css",
        '     "~/App_Themes/DefaultBlue/Menus.css",
        '     "~/App_Themes/DefaultBlue/Navigator.css",
        '     "~/App_Themes/DefaultBlue/Calendar.css",
        '     "~/App_Themes/DefaultBlue/Print.css"))

        ' jQuery UI CSS bundle - COMMENTED OUT: Now using ASP.NET Theme
        ' bundles.Add(New StyleBundle("~/Content/jqueryui").Include(
        '     "~/App_Themes/DefaultBlue/jquery-ui-1.11.3.custom.css",
        '     "~/App_Themes/DefaultBlue/jquery-ui.structure.css",
        '     "~/App_Themes/DefaultBlue/jquery-ui.theme.css",
        '     "~/App_Themes/DefaultBlue/jqModal.css"))

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
