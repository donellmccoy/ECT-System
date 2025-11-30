using System.Web.Optimization;

namespace ALOD
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            // jQuery bundles
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Script/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Script/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/common").Include(
                        "~/Script/common.js",
                        "~/Script/jqModal.js",
                        "~/Script/jquery.color.js",
                        "~/Script/jquery.dimensions.js",
                        "~/Script/TimeoutDialog.js"));

            // CSS bundles
            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/App_Themes/DefaultBlue/jquery-ui-1.11.3.custom.css",
                      "~/App_Themes/DefaultBlue/jqModal.css",
                      "~/App_Themes/DefaultBlue/Menus.css",
                      "~/App_Themes/DefaultBlue/Navigator.css",
                      "~/App_Themes/DefaultBlue/Calendar.css"));

            // Enable optimizations
            #if DEBUG
                BundleTable.EnableOptimizations = false;
            #else
                BundleTable.EnableOptimizations = true;
            #endif
        }
    }
}
