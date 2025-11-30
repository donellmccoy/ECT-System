Imports ALODWebUtility.Common
Imports System.Reflection

Namespace Web

    Partial Class Public_Public
        Inherits System.Web.UI.MasterPage

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            'register javascripts
            WriteHostName(Page)

            'Page.ClientScript.RegisterClientScriptInclude("JQueryScript", ResolveUrl("~/Script/jquery-3.6.0.min.js"))
            'Page.ClientScript.RegisterClientScriptInclude("MigrateScript", ResolveUrl("~/Script/jquery-migrate-3.4.1.min.js"))

            'Page.ClientScript.RegisterClientScriptInclude("JqueryBlock", ResolveUrl("~/Script/jquery.blockUI.min.js"))
            'Page.ClientScript.RegisterClientScriptInclude("JqueryUI", ResolveUrl("~/Script/jquery-ui-1.13.0.min.js"))
            'Page.ClientScript.RegisterClientScriptInclude("JQueryModal", ResolveUrl("~/Script/jqModal.js"))
            'Page.ClientScript.RegisterClientScriptInclude("CommonScript", ResolveUrl("~/Script/common.js"))

            ' Set version number in footer
            SetVersionLabel()

        End Sub

        Private Sub SetVersionLabel()
            Try
                Dim assembly As Assembly = Assembly.GetExecutingAssembly()
                Dim version As Version = assembly.GetName().Version
                VersionLabel.Text = String.Format("Version {0}.{1}.{2}", version.Major, version.Minor, version.Build)
            Catch ex As Exception
                ' If version cannot be retrieved, leave label empty or set a default
                VersionLabel.Text = String.Empty
            End Try
        End Sub

    End Class

End Namespace