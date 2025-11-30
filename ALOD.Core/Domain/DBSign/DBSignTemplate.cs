using ALOD.Core.Interfaces;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Core.Utils;
using ALOD.Logging;
using System;
using System.Collections.Generic;
using System.Data;

namespace ALOD.Core.Domain.DBSign
{
    public class DBSignTemplate : IExtractedEntity
    {
        public DBSignTemplate()
        {
            SetPropertiesToDefaultValues();
        }

        private DBSignTemplate(DataRow row)
        { }

        public virtual bool HasSecondaryKey
        {
            get
            {
                return !string.IsNullOrEmpty(SecondaryKeyName);
            }
        }

        public virtual int Id { get; protected set; }
        public virtual string PrimaryKeyName { get; protected set; }
        public virtual string SecondaryKeyName { get; protected set; }
        public virtual string TableName { get; protected set; }
        public virtual string Title { get; protected set; }

        /// <inheritdoc/>
        public virtual bool ExtractFromDataRow(DataRow row)
        {
            try
            {
                this.Id = DataHelpers.GetIntFromDataRow("t_id", row);
                this.Title = DataHelpers.GetStringFromDataRow("title", row);
                this.PrimaryKeyName = DataHelpers.GetStringFromDataRow("primary_key", row);
                this.SecondaryKeyName = DataHelpers.GetStringFromDataRow("secondary_key", row);
                this.TableName = DataHelpers.GetStringFromDataRow("template_table_name", row);

                return true;
            }
            catch (Exception e)
            {
                SetPropertiesToDefaultValues();
                LogManager.LogError(e);
                return false;
            }
        }

        /// <inheritdoc/>
        public bool ExtractFromDataRow(DataRow row, IDaoFactory daoFactory)
        {
            try
            {
                return ExtractFromDataRow(row);
            }
            catch (Exception e)
            {
                SetPropertiesToDefaultValues();
                LogManager.LogError(e);
                return false;
            }
        }

        public virtual string GetKeyListParameter(DBSignSupportedBrowsers browser)
        {
            switch (browser)
            {
                case DBSignSupportedBrowsers.InternetExplorer:
                    return GetInternetExplorerKeyListParameter();

                case DBSignSupportedBrowsers.Firefox:
                    return GetFireFoxKeyListParameter();

                default:
                    return GetInternetExplorerKeyListParameter();
            }
        }

        public virtual string GetPrimaryKeyParameter(DBSignSupportedBrowsers browser, string keyValue)
        {
            switch (browser)
            {
                case DBSignSupportedBrowsers.InternetExplorer:
                    return GetInternetExplorerPrimaryKeyParameter(keyValue);

                case DBSignSupportedBrowsers.Firefox:
                    return GetFirefoxPrimaryKeyParameter(keyValue);

                default:
                    return GetInternetExplorerPrimaryKeyParameter(keyValue);
            }
        }

        public virtual Dictionary<string, string> GetPrimaryKeys(string PKValue, string SKValue)
        {
            Dictionary<string, string> templatePrimaryKeys = new Dictionary<string, string>();

            templatePrimaryKeys.Add(PrimaryKeyName, PKValue);

            if (HasSecondaryKey)
                templatePrimaryKeys.Add(SecondaryKeyName, SKValue);

            return templatePrimaryKeys;
        }

        public virtual string GetSecondaryKeyParameter(DBSignSupportedBrowsers browser, string keyValue)
        {
            switch (browser)
            {
                case DBSignSupportedBrowsers.InternetExplorer:
                    return GetInternetExplorerSecondaryKeyParameter(keyValue);

                case DBSignSupportedBrowsers.Firefox:
                    return GetFirefoxSecondaryKeyParameter(keyValue);

                default:
                    return GetInternetExplorerSecondaryKeyParameter(keyValue);
            }
        }

        public virtual string GetVerificationQueryString(string appPool, string primaryKeyValue, string secondaryKeyValue = null)
        {
            string nonKeyInfo = string.Empty;
            string keyList = string.Empty;
            string keyValues = string.Empty;

            nonKeyInfo = @"?CONTENT_TYPE=VERIFY&DB_POOL_NAME=" + appPool + "&TEMPLATE_NAME=" + TableName;

            keyList = "&PK_LIST=:" + PrimaryKeyName;
            keyValues = "&:" + PrimaryKeyName + "=" + primaryKeyValue;

            if (HasSecondaryKey)
            {
                keyList += (",:" + SecondaryKeyName);
                keyValues += ("&:" + SecondaryKeyName + "=" + secondaryKeyValue);
            }

            return (nonKeyInfo + keyList + keyValues);
        }

        protected virtual string GetFireFoxKeyListParameter()
        {
            string keyList = PrimaryKeyName;

            if (HasSecondaryKey)
            {
                keyList += (",:" + SecondaryKeyName);
            }

            return ("PK_LIST=:" + keyList + "' />" + Environment.NewLine);
        }

        protected virtual string GetFirefoxPrimaryKeyParameter(string keyValue)
        {
            return (":" + PrimaryKeyName + "=" + keyValue + Environment.NewLine);
        }

        protected virtual string GetFirefoxSecondaryKeyParameter(string keyValue)
        {
            if (!HasSecondaryKey)
                return string.Empty;

            return (":" + SecondaryKeyName + "=" + keyValue + Environment.NewLine);
        }

        protected virtual string GetInternetExplorerKeyListParameter()
        {
            string keyList = PrimaryKeyName;

            if (HasSecondaryKey)
            {
                keyList += (",:" + SecondaryKeyName);
            }

            return ("<param name='PK_LIST' value=':" + keyList + "' />" + Environment.NewLine);
        }

        protected virtual string GetInternetExplorerPrimaryKeyParameter(string keyValue)
        {
            return ("<param name=':" + PrimaryKeyName + "' value='" + keyValue + "' />" + Environment.NewLine);
        }

        protected virtual string GetInternetExplorerSecondaryKeyParameter(string keyValue)
        {
            if (!HasSecondaryKey)
                return string.Empty;

            return ("<param name=':" + SecondaryKeyName + "' value='" + keyValue + "' />" + Environment.NewLine);
        }

        protected void SetPropertiesToDefaultValues()
        {
            this.Id = 0;
            this.Title = string.Empty;
            this.PrimaryKeyName = string.Empty;
            this.SecondaryKeyName = string.Empty;
        }
    }
}