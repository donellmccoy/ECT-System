using System;
using System.Reflection;
using System.Web.UI;
using ALODWebUtility.Common;
using static ALODWebUtility.Common.Utility;

namespace ALOD.Web
{
    public partial class Public_Public : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Register javascripts
            WriteHostName(Page);

            // Set version number in footer
            SetVersionLabel();
        }

        private void SetVersionLabel()
        {
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                Version version = assembly.GetName().Version;
                VersionLabel.Text = string.Format("Version {0}.{1}.{2}", version.Major, version.Minor, version.Build);
            }
            catch (Exception)
            {
                // If version cannot be retrieved, leave label empty or set a default
                VersionLabel.Text = string.Empty;
            }
        }
    }
}
