using System;
using System.Configuration;
using static SRXLite.Modules.Util;

namespace SRXLite.Modules
{
    public static class AppSettings
    {
        #region EncryptionKey

        public static string EncryptionKey()
        {
            return ConfigurationManager.AppSettings["EncryptionKey"];
        }

        #endregion

        #region Environment

        public static class Environment
        {
            public static bool IsDemo()
            {
                return (ConfigValue() == "DEMO");
            }

            public static bool IsDev()
            {
                return (ConfigValue() == "DEV");
            }

            public static bool IsProd()
            {
                return (ConfigValue() == "PROD");
            }

            private static string ConfigValue()
            {
                return ConfigurationManager.AppSettings["Environment"].ToUpper();
            }
        }

        #endregion

        #region FileSizeUploadLimit

        public static int FileSizeUploadLimit()
        {
            return IntCheck(ConfigurationManager.AppSettings["FileSizeUploadLimit"]);
        }

        public static double GetFileSizeUploadLimitMB()
        {
            return GetFileSizeMB(FileSizeUploadLimit());
        }

        public static double GetInitialFileSizeUploadLimitMB()
        {
            return GetFileSizeMB(InitialFileSizeUploadLimit());
        }

        public static int InitialFileSizeUploadLimit()
        {
            return IntCheck(ConfigurationManager.AppSettings["InitialFileSizeUploadLimit"]);
        }

        #endregion

        #region IsEncryptionEnabled

        public static bool IsEncryptionEnabled()
        {
            return BoolCheck(ConfigurationManager.AppSettings["EncryptionEnabled"], true);
        }

        #endregion
    }
}
