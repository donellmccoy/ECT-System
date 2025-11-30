using ALOD.Core.Domain.DBSign;
using ALOD.Core.Interfaces;
using ALOD.Core.Interfaces.DAOInterfaces;
using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace ALOD.Data.Services
{
    public class DBSignService : IDigitalSignatureService
    {
        private IDBSignTemplateDao _dbSignTemplateDao = null;

        private SqlDataStore _sqlStore = null;

        public DBSignService(DBSignTemplateId template, int primaryKey, int secondaryKey)
        {
            Text = "";
            Result = DBSignResult.Unknown;
            TemplateId = template;
            PrimaryId = primaryKey;
            SecondaryId = secondaryKey;
            Template = DBSignTemplateDao.GetById((int)TemplateId);
        }

        /// <summary>
        /// Gets or sets the primary key identifier for the entity being digitally signed.
        /// </summary>
        public int PrimaryId { get; set; }

        /// <summary>
        /// Gets the result of the last digital signature verification operation.
        /// </summary>
        public DBSignResult Result { get; private set; }

        /// <summary>
        /// Gets or sets the secondary key identifier for the entity being digitally signed.
        /// </summary>
        public int SecondaryId { get; set; }

        /// <summary>
        /// Gets the DBSign template configuration for the digital signature.
        /// </summary>
        public DBSignTemplate Template { get; private set; }

        /// <summary>
        /// Gets the unique identifier for the DBSign template.
        /// </summary>
        public DBSignTemplateId TemplateId { get; private set; }

        /// <summary>
        /// Gets the name of the database table associated with the template.
        /// </summary>
        public string TemplateTableName
        {
            get
            {
                return Template.TableName;
            }
        }

        /// <summary>
        /// Gets the error message text from the last signature verification operation.
        /// </summary>
        public string Text { get; private set; }

        protected IDBSignTemplateDao DBSignTemplateDao
        {
            get
            {
                if (_dbSignTemplateDao == null)
                    _dbSignTemplateDao = new DBSignTemplateDao();

                return _dbSignTemplateDao;
            }
        }

        private string AppPool
        {
            get { return ConfigurationManager.AppSettings["DbSignDatabase"]; }
        }

        private string BaseUrl
        {
            get { return ConfigurationManager.AppSettings["DbSignUrl"]; }
        }

        private SqlDataStore SqlStore
        {
            get
            {
                if (_sqlStore == null)
                {
                    _sqlStore = new SqlDataStore();
                }

                return _sqlStore;
            }
        }

        /// <summary>
        /// Retrieves information about the person who digitally signed the document.
        /// </summary>
        /// <returns>A DigitalSignatureInfo object containing the signer's name, signature, and date signed.</returns>
        public DigitalSignatureInfo GetSignerInfo()
        {
            DigitalSignatureInfo info = new DigitalSignatureInfo();

            string sql = "Select top 1 subject_dn, DBS_SIGN_DATE " +
                 "FROM dbsign.DBS_CERTS " +
                 "INNER JOIN dbsign.SIG_" + TemplateTableName + " " +
                 "ON CERT_ID=DBS_CERT_ID " +
                 "WHERE TARGET_KEY = @refId ";

            sql = sql.Replace("TARGET_KEY", Template.PrimaryKeyName);

            if (Template.HasSecondaryKey)
            {
                sql += (" AND " + Template.SecondaryKeyName + "=@ptype");
            }

            sql += " ORDER BY DBS_SIGN_DATE DESC";

            DbCommand cmd = SqlStore.GetSqlStringCommand(sql);
            SqlStore.AddInParameter(cmd, "@refId", System.Data.DbType.Int32, PrimaryId);

            if (Template.HasSecondaryKey)
            {
                SqlStore.AddInParameter(cmd, "@ptype", System.Data.DbType.Int32, SecondaryId);
            }

            SqlDataStore.RowReader del = (SqlDataStore source, IDataReader reader) =>
            {
                info.Signature = source.GetString(reader, 0);
                info.DateSigned = source.GetDateTime(reader, 1);
            };

            SqlStore.ExecuteReader(del, cmd);

            if (info.Signature.Length > 0)
            {
                int index = info.Signature.IndexOf("cn=");
                int stop = info.Signature.IndexOf(",ou=");

                if (index != -1 && stop != -1)
                {
                    info.Signature = info.Signature.Substring(index, stop).Replace("cn=", "");

                    //we get the display name from the parts of the signature
                    StringBuilder buffer = new StringBuilder();

                    foreach (string part in info.Signature.Split('.'))
                    {
                        if (!char.IsNumber(part[0]))
                        {
                            buffer.Append(part + " ");
                        }
                    }

                    info.Name = buffer.ToString();
                }
            }

            return info;
        }

        /// <summary>
        /// Constructs the URL for performing digital signature actions.
        /// </summary>
        /// <param name="action">The DBSign action to perform (Sign or Verify).</param>
        /// <returns>The full URL to the DBSign service endpoint.</returns>
        public string GetUrl(DBSignAction action)
        {
            string url = BaseUrl;

            if (action == DBSignAction.Verify)
            {
                url += Template.GetVerificationQueryString(AppPool, PrimaryId.ToString(), SecondaryId.ToString());
            }

            return url;
        }

        /// <summary>
        /// Verifies the digital signature for the document by calling the DBSign service.
        /// </summary>
        /// <returns>A DBSignResult indicating the verification status (SignatureValid, SignatureInvalid, NoSignature, ConnectionError, or Unknown).</returns>
        public DBSignResult VerifySignature()
        {
            int code = -1;
            string content = string.Empty;
            string result = string.Empty;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(GetUrl(DBSignAction.Verify));
                request.KeepAlive = false;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());

                content = HttpUtility.UrlDecode(reader.ReadToEnd());

                Text = ExtractResponseValue(content, "DBS_ERROR_MSG");
                result = ExtractResponseValue(content, "DBS_ERROR_VAL");
            }
            catch (Exception)
            {
                code = -1;
            }

            try
            {
                code = int.Parse(result);

                switch (code)
                {
                    case 0:
                        Result = DBSignResult.SignatureValid;
                        break;

                    case 135:
                        Result = DBSignResult.SignatureInvalid;
                        break;

                    case 133:
                        Result = DBSignResult.NoSignature;
                        break;

                    case 132:
                        Result = DBSignResult.ConnectionError;
                        break;

                    default:
                        Result = DBSignResult.Unknown;
                        break;
                }
                ;
            }
            catch
            {
                Result = DBSignResult.Unknown;
            }

            return Result;
        }

        private string ExtractResponseValue(string content, string key)
        {
            foreach (string pair in content.Split('&'))
            {
                string[] parts = pair.Split('=');

                if (parts.Length != 2)
                    continue;

                if (parts[0] == key)
                {
                    return parts[1];
                }
            }

            return "";
        }
    }
}