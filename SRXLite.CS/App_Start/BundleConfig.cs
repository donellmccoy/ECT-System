using System.Web.Optimization;

namespace SRXLite.App_Start
{
    /// <summary>
    /// Configures script and style bundles for the SRXLite application.
    /// Bundles improve performance by reducing HTTP requests and minifying resources.
    /// </summary>
    public class BundleConfig
    {
        /// <summary>
        /// Registers all script and style bundles for the application.
        /// Call this method from Application_Start in Global.asax.
        /// </summary>
        public static void RegisterBundles(BundleCollection bundles)
        {
            // Core jQuery library bundle (version 3.6.0)
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/jquery-3.6.0.min.js"));

            // Core CSS bundle (SRXLite uses a simpler theme structure)
            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/styles.css"));

            // Enable optimizations even in debug mode (for testing)
            // Set to False to use unminified versions during development
#if DEBUG
            BundleTable.EnableOptimizations = false;
#else
            BundleTable.EnableOptimizations = true;
#endif
        }
    }
}
