using System;
using System.Collections.Generic;
using System.Web.Services;
using System.Web.Services.Protocols;
using SRXLite.Classes;
using SRXLite.DataTypes;
using static SRXLite.Modules.Util;
using static SRXLite.Modules.ExceptionHandling;

namespace SRXLite.Web.Services
{
    [WebService(Namespace = "http://tempuri.org/srxlite/documentservice")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class DocumentService : System.Web.Services.WebService
    {
        public ServiceLogin _login;
        private ServiceUser _user;
        private Document _doc;
        private Group _group;
        private Entity _entity;

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public DocumentService()
        {
            _login = new ServiceLogin();
            _user = new ServiceUser();
        }

        #endregion

        #region CreateGroup

        [WebMethod]
        [SoapHeader("_login")]
        public IAsyncResult BeginCreateGroup(string groupName, AsyncCallback callback, object stateObject)
        {
            try
            {
                _user.Authenticate(_login);
                _group = new Group(_user);
                return _group.BeginCreate(groupName, callback, stateObject);
            }
            catch (UnauthorizedAccessException accessEx)
            {
                LogError(accessEx.ToString());
                throw accessEx;
            }
            catch (Exception ex)
            {
                SoapException soapEx = CreateSoapException(ex.Message, ex.ToString(), _user, "BeginCreateGroup", true);
                throw soapEx;
            }
        }

        [WebMethod]
        [SoapHeader("_login")]
        public long EndCreateGroup(IAsyncResult result)
        {
            try
            {
                _user.Authenticate(_login);
                return _group.EndCreate(result);
            }
            catch (UnauthorizedAccessException accessEx)
            {
                LogError(accessEx.ToString());
                throw accessEx;
            }
            catch (Exception ex)
            {
                SoapException soapEx = CreateSoapException(ex.Message, ex.ToString(), _user, "EndCreateGroup", true);
                throw soapEx;
            }
        }

        #endregion

        #region DeleteDocument

        [WebMethod]
        [SoapHeader("_login")]
        public IAsyncResult BeginDeleteDocument(long docID, AsyncCallback callback, object stateObject)
        {
            try
            {
                _user.Authenticate(_login);
                _doc = new Document(_user, docID);
                return _doc.BeginDelete(callback, stateObject);
            }
            catch (UnauthorizedAccessException accessEx)
            {
                LogError(accessEx.ToString());
                throw accessEx;
            }
            catch (Exception ex)
            {
                SoapException soapEx = CreateSoapException(ex.Message, ex.ToString(), _user, "BeginDeleteDocument", true);
                throw soapEx;
            }
        }

        [WebMethod]
        [SoapHeader("_login")]
        public void EndDeleteDocument(IAsyncResult result)
        {
            try
            {
                _user.Authenticate(_login);
                _doc.EndDelete(result);
            }
            catch (UnauthorizedAccessException accessEx)
            {
                LogError(accessEx.ToString());
                throw accessEx;
            }
            catch (Exception ex)
            {
                SoapException soapEx = CreateSoapException(ex.Message, ex.ToString(), _user, "EndDeleteDocument", true);
                throw soapEx;
            }
        }

        #endregion

        #region GetEntityDocumentList

        [WebMethod]
        [SoapHeader("_login")]
        public IAsyncResult BeginGetEntityDocumentList(string entityID, AsyncCallback callback, object stateObject)
        {
            try
            {
                _user.Authenticate(_login);
                _entity = new Entity(_user, entityID);
                return _entity.BeginGetDocumentList(callback, stateObject);
            }
            catch (UnauthorizedAccessException accessEx)
            {
                LogError(accessEx.ToString());
                throw accessEx;
            }
            catch (Exception ex)
            {
                SoapException soapEx = CreateSoapException(ex.Message, ex.ToString(), _user, "BeginGetEntityDocumentList", true);
                throw soapEx;
            }
        }

        [WebMethod]
        [SoapHeader("_login")]
        public List<DocumentData> EndGetEntityDocumentList(IAsyncResult result)
        {
            try
            {
                _user.Authenticate(_login);
                return _entity.EndGetDocumentList(result);
            }
            catch (UnauthorizedAccessException accessEx)
            {
                LogError(accessEx.ToString());
                throw accessEx;
            }
            catch (Exception ex)
            {
                SoapException soapEx = CreateSoapException(ex.Message, ex.ToString(), _user, "EndGetEntityDocumentList", true);
                throw soapEx;
            }
        }

        #endregion

        #region GetGroupDocumentList

        [WebMethod]
        [SoapHeader("_login")]
        public IAsyncResult BeginGetGroupDocumentList(long groupID, AsyncCallback callback, object stateObject)
        {
            try
            {
                _user.Authenticate(_login);
                _group = new Group(_user, groupID);
                return _group.BeginGetDocumentList(callback, stateObject);
            }
            catch (UnauthorizedAccessException accessEx)
            {
                LogError(accessEx.ToString());
                throw accessEx;
            }
            catch (Exception ex)
            {
                SoapException soapEx = CreateSoapException(ex.Message, ex.ToString(), _user, "BeginGetGroupDocumentList", true);
                throw soapEx;
            }
        }

        [WebMethod]
        [SoapHeader("_login")]
        public List<DocumentData> EndGetGroupDocumentList(IAsyncResult result)
        {
            try
            {
                _user.Authenticate(_login);
                return _group.EndGetDocumentList(result);
            }
            catch (UnauthorizedAccessException accessEx)
            {
                LogError(accessEx.ToString());
                throw accessEx;
            }
            catch (Exception ex)
            {
                SoapException soapEx = CreateSoapException(ex.Message, ex.ToString(), _user, "EndGetGroupDocumentList", true);
                throw soapEx;
            }
        }

        #endregion

        #region GetDocumentUploadUrl

        [WebMethod]
        [SoapHeader("_login")]
        public IAsyncResult BeginGetDocumentUploadUrl(
            string entityName,
            int docTypeID,
            long groupID,
            string stylesheetUrl,
            string entityDisplayText,
            AsyncCallback callback,
            object stateObject)
        {
            try
            {
                _user.Authenticate(_login);
                _doc = new Document(_user);
                return _doc.BeginGetUploadUrl(entityName, docTypeID, groupID, stylesheetUrl, entityDisplayText, callback, stateObject);
            }
            catch (UnauthorizedAccessException accessEx)
            {
                LogError(accessEx.ToString());
                throw accessEx;
            }
            catch (Exception ex)
            {
                SoapException soapEx = CreateSoapException(ex.Message, ex.ToString(), _user, "BeginGetDocumentUploadUrl", true);
                throw soapEx;
            }
        }

        [WebMethod]
        [SoapHeader("_login")]
        public string EndGetDocumentUploadUrl(IAsyncResult result)
        {
            try
            {
                _user.Authenticate(_login);
                return _doc.EndGetUploadUrl(result);
            }
            catch (UnauthorizedAccessException accessEx)
            {
                LogError(accessEx.ToString());
                throw accessEx;
            }
            catch (Exception ex)
            {
                SoapException soapEx = CreateSoapException(ex.Message, ex.ToString(), _user, "EndGetDocumentUploadUrl", true);
                throw soapEx;
            }
        }

        #endregion

        #region GetDocumentViewerUrl

        [WebMethod]
        [SoapHeader("_login")]
        public IAsyncResult BeginGetDocumentViewerUrl(
            long docID,
            bool isReadOnly,
            AsyncCallback callback,
            object stateObject)
        {
            try
            {
                _user.Authenticate(_login);
                _doc = new Document(_user, docID);
                return _doc.BeginGetViewerUrl(docID, isReadOnly, callback, stateObject);
            }
            catch (UnauthorizedAccessException accessEx)
            {
                LogError(accessEx.ToString());
                throw accessEx;
            }
            catch (Exception ex)
            {
                SoapException soapEx = CreateSoapException(ex.Message, ex.ToString(), _user, "BeginGetDocumentViewerUrl", true);
                throw soapEx;
            }
        }

        [WebMethod]
        [SoapHeader("_login")]
        public string EndGetDocumentViewerUrl(IAsyncResult result)
        {
            try
            {
                _user.Authenticate(_login);
                return _doc.EndGetViewerUrl(result);
            }
            catch (UnauthorizedAccessException accessEx)
            {
                LogError(accessEx.ToString());
                throw accessEx;
            }
            catch (Exception ex)
            {
                SoapException soapEx = CreateSoapException(ex.Message, ex.ToString(), _user, "EndGetDocumentViewerUrl", true);
                throw soapEx;
            }
        }

        #endregion

        #region MoveGroupDocument

        [WebMethod]
        [SoapHeader("_login")]
        public IAsyncResult BeginMoveGroupDocument(
            long docID,
            long sourceGroupID,
            long targetGroupID,
            AsyncCallback callback,
            object stateObject)
        {
            try
            {
                _user.Authenticate(_login);
                _group = new Group(_user, sourceGroupID);
                return _group.BeginMoveDocument(docID, targetGroupID, callback, stateObject);
            }
            catch (UnauthorizedAccessException accessEx)
            {
                LogError(accessEx.ToString());
                throw accessEx;
            }
            catch (Exception ex)
            {
                SoapException soapEx = CreateSoapException(ex.Message, ex.ToString(), _user, "BeginMoveGroupDocument", true);
                throw soapEx;
            }
        }

        [WebMethod]
        [SoapHeader("_login")]
        public void EndMoveGroupDocument(IAsyncResult result)
        {
            try
            {
                _user.Authenticate(_login);
                _group.EndMoveDocument(result);
            }
            catch (UnauthorizedAccessException accessEx)
            {
                LogError(accessEx.ToString());
                throw accessEx;
            }
            catch (Exception ex)
            {
                SoapException soapEx = CreateSoapException(ex.Message, ex.ToString(), _user, "EndMoveGroupDocument", true);
                throw soapEx;
            }
        }

        #endregion

        #region UpdateDocumentStatus

        [WebMethod]
        [SoapHeader("_login")]
        public IAsyncResult BeginUpdateDocumentStatus(
            long docID,
            DocumentStatus docStatus,
            AsyncCallback callback,
            object stateObject)
        {
            try
            {
                _user.Authenticate(_login);
                _doc = new Document(_user, docID);
                return _doc.BeginUpdateStatus(docStatus, callback, stateObject);
            }
            catch (UnauthorizedAccessException accessEx)
            {
                LogError(accessEx.ToString());
                throw accessEx;
            }
            catch (Exception ex)
            {
                SoapException soapEx = CreateSoapException(ex.Message, ex.ToString(), _user, "BeginUpdateDocumentStatus", true);
                throw soapEx;
            }
        }

        [WebMethod]
        [SoapHeader("_login")]
        public void EndUpdateDocumentStatus(IAsyncResult result)
        {
            try
            {
                _user.Authenticate(_login);
                _doc.EndUpdateStatus(result);
            }
            catch (UnauthorizedAccessException accessEx)
            {
                LogError(accessEx.ToString());
                throw accessEx;
            }
            catch (Exception ex)
            {
                SoapException soapEx = CreateSoapException(ex.Message, ex.ToString(), _user, "EndUpdateDocumentStatus", true);
                throw soapEx;
            }
        }

        #endregion

        #region UpdateDocumentKeys

        [WebMethod]
        [SoapHeader("_login")]
        public IAsyncResult BeginUpdateDocumentKeys(
            long docID,
            DocumentKeys docKeys,
            AsyncCallback callback,
            object stateObject)
        {
            try
            {
                _user.Authenticate(_login);
                _doc = new Document(_user, docID);
                return _doc.BeginUpdateKeys(docKeys, callback, stateObject);
            }
            catch (UnauthorizedAccessException accessEx)
            {
                LogError(accessEx.ToString());
                throw accessEx;
            }
            catch (Exception ex)
            {
                SoapException soapEx = CreateSoapException(ex.Message, ex.ToString(), _user, "BeginUpdateDocumentKeys", true);
                throw soapEx;
            }
        }

        [WebMethod]
        [SoapHeader("_login")]
        public void EndUpdateDocumentKeys(IAsyncResult result)
        {
            try
            {
                _user.Authenticate(_login);
                _doc.EndUpdateKeys(result);
            }
            catch (UnauthorizedAccessException accessEx)
            {
                LogError(accessEx.ToString());
                throw accessEx;
            }
            catch (Exception ex)
            {
                SoapException soapEx = CreateSoapException(ex.Message, ex.ToString(), _user, "EndUpdateDocumentKeys", true);
                throw soapEx;
            }
        }

        #endregion

        #region UploadDocument

        [WebMethod]
        [SoapHeader("_login")]
        public IAsyncResult BeginUploadDocument(
            byte[] fileBytes,
            UploadKeys uploadKeys,
            long groupID,
            AsyncCallback callback,
            object stateObject)
        {
            try
            {
                _user.Authenticate(_login);

                // Validation
                if (!IsFileExtValid(uploadKeys.FileName))
                    throw new Exception("File type not supported.");
                if (!IsFileSizeValid(fileBytes.Length))
                    throw new Exception("File size exceeded the maximum limit of " + GetFileSizeUploadLimitMB() + " MB.");

                _doc = new Document(_user, 0, groupID);
                uploadKeys.InputType = InputType.WebServiceUpload;

                return _doc.BeginUpload(fileBytes, uploadKeys, callback, stateObject);
            }
            catch (UnauthorizedAccessException accessEx)
            {
                LogError(accessEx.ToString());
                throw accessEx;
            }
            catch (Exception ex)
            {
                SoapException soapEx = CreateSoapException(ex.Message, ex.ToString(), _user, "BeginUploadDocument", true);
                throw soapEx;
            }
        }

        [WebMethod(Description = "")]
        [SoapHeader("_login")]
        public long EndUploadDocument(IAsyncResult result)
        {
            try
            {
                if (_doc.HasErrors)
                {
                    throw new Exception(_doc.ErrorMessage);
                }

                _user.Authenticate(_login);
                _doc.EndUpload(result);
                return _doc.DocID;
            }
            catch (UnauthorizedAccessException accessEx)
            {
                LogError(accessEx.ToString());
                throw accessEx;
            }
            catch (Exception ex)
            {
                SoapException soapEx = CreateSoapException(ex.Message, ex.ToString(), _user, "EndUploadDocument", true);
                throw soapEx;
            }
        }

        #endregion

        #region CopyGroupDocuments

        [WebMethod]
        [SoapHeader("_login")]
        public IAsyncResult BeginCopyGroupDocuments(
            long oldGroupId,
            long newGroupId,
            long oldDocTypeId,
            long newDocTypeId,
            AsyncCallback callback,
            object stateObject)
        {
            try
            {
                _user.Authenticate(_login);
                _group = new Group(_user);
                return _group.BeginCopyGroupDocuments(oldGroupId, newGroupId, oldDocTypeId, newDocTypeId, callback, stateObject);
            }
            catch (UnauthorizedAccessException accessEx)
            {
                LogError(accessEx.ToString());
                throw accessEx;
            }
            catch (Exception ex)
            {
                SoapException soapEx = CreateSoapException(ex.Message, ex.ToString(), _user, "BeginCopyGroupDocuments", true);
                throw soapEx;
            }
        }

        [WebMethod]
        [SoapHeader("_login")]
        public void EndCopyGroupDocuments(IAsyncResult result)
        {
            try
            {
                _user.Authenticate(_login);
                _group.EndCopyGroupDocuments(result);
            }
            catch (UnauthorizedAccessException accessEx)
            {
                LogError(accessEx.ToString());
                throw accessEx;
            }
            catch (Exception ex)
            {
                SoapException soapEx = CreateSoapException(ex.Message, ex.ToString(), _user, "EndCopyGroupDocuments", true);
                throw soapEx;
            }
        }

        #endregion

        #region Dispose

        protected override void Dispose(bool disposing)
        {
            if (_doc != null) _doc.Dispose();
            if (_group != null) _group.Dispose();
            if (_entity != null) _entity.Dispose();
            base.Dispose(disposing);
        }

        #endregion
    }
}
