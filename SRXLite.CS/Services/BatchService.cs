using System;
using System.Collections.Generic;
using System.Web.Services;
using System.Web.Services.Protocols;
using SRXLite.Classes;
using SRXLite.DataTypes;
using static SRXLite.Modules.ExceptionHandling;

namespace SRXLite.Web.Services
{
    [WebService(Namespace = "http://tempuri.org/srxlite/batchservice")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class BatchService : System.Web.Services.WebService
    {
        public ServiceLogin _login;
        private ServiceUser _user;
        private Batch _batch;
        private Entity _entity;

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public BatchService()
        {
            _login = new ServiceLogin();
            _user = new ServiceUser();
        }

        #endregion

        #region GetBatchUploadUrl

        [WebMethod]
        [SoapHeader("_login")]
        public IAsyncResult BeginGetBatchUploadUrl(
            BatchType batchType,
            string location,
            string entityName,
            int docTypeID,
            string stylesheetUrl,
            string entityDisplayText,
            AsyncCallback callback,
            object stateObject)
        {
            try
            {
                _user.Authenticate(_login);
                _batch = new Batch(_user);
                return _batch.BeginGetUploadUrl(batchType, location, entityName, docTypeID, stylesheetUrl, entityDisplayText, callback, stateObject);
            }
            catch (UnauthorizedAccessException accessEx)
            {
                LogError(accessEx.ToString());
                throw accessEx;
            }
            catch (Exception ex)
            {
                SoapException soapEx = CreateSoapException(ex.Message, ex.ToString(), _user, "BeginGetBatchUploadUrl", true);
                throw soapEx;
            }
        }

        [WebMethod]
        [SoapHeader("_login")]
        public string EndGetBatchUploadUrl(IAsyncResult result)
        {
            try
            {
                _user.Authenticate(_login);
                return _batch.EndGetUploadUrl(result);
            }
            catch (UnauthorizedAccessException accessEx)
            {
                LogError(accessEx.ToString());
                throw accessEx;
            }
            catch (Exception ex)
            {
                SoapException soapEx = CreateSoapException(ex.Message, ex.ToString(), _user, "EndGetBatchUploadUrl", true);
                throw soapEx;
            }
        }

        #endregion

        #region GetEntityBatchList

        [WebMethod]
        [SoapHeader("_login")]
        public IAsyncResult BeginGetEntityBatchList(
            string entityName,
            AsyncCallback callback,
            object stateObject)
        {
            try
            {
                _user.Authenticate(_login);
                _entity = new Entity(_user, entityName);
                return _entity.BeginGetBatchList(callback, stateObject);
            }
            catch (UnauthorizedAccessException accessEx)
            {
                LogError(accessEx.ToString());
                throw accessEx;
            }
            catch (Exception ex)
            {
                SoapException soapEx = CreateSoapException(ex.Message, ex.ToString(), _user, "BeginGetEntityBatchList", true);
                throw soapEx;
            }
        }

        [WebMethod]
        [SoapHeader("_login")]
        public List<BatchData> EndGetEntityBatchList(IAsyncResult result)
        {
            try
            {
                _user.Authenticate(_login);
                return _entity.EndGetBatchList(result);
            }
            catch (UnauthorizedAccessException accessEx)
            {
                LogError(accessEx.ToString());
                throw accessEx;
            }
            catch (Exception ex)
            {
                SoapException soapEx = CreateSoapException(ex.Message, ex.ToString(), _user, "EndGetEntityBatchList", true);
                throw soapEx;
            }
        }

        #endregion

        #region Dispose

        protected override void Dispose(bool disposing)
        {
            if (_batch != null) _batch.Dispose();
            if (_entity != null) _entity.Dispose();
            base.Dispose(disposing);
        }

        #endregion
    }
}
