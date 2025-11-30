using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using SRXLite.DataAccess;
using SRXLite.DataTypes;
using static SRXLite.Modules.Util;
using static SRXLite.Modules.ExceptionHandling;
using static SRXLite.DataAccess.DB;

namespace SRXLite.Classes
{
    /// <summary>
    /// Document class for managing document operations
    /// </summary>
    public class Document : IDisposable
    {
        private bool _appendable;
        private bool _browserViewable;
        private string _contentType;
        private HttpContext _context;
        private int _currentFrame = 0;
        private AsyncDB _db;
        private DocumentGuid _docGuid;
        private long _docID;
        private string _errorMessage;
        private byte[] _fileBytes;
        private string _fileExt;

        // Multiframe images
        private List<byte[]> _frameFileBytes;

        private Group _group;
        private long _groupID;
        private Guid _guid;
        private bool _hasErrors = false;
        private bool _multiframe = false;
        private bool _multiframeComplete = false;
        private int _subuserID;
        private UploadKeys _uploadKeys;
        private AsyncCallback _userCallback;
        private short _userID;
        private object _userStateObject;
        private string _entityDisplayText;
        private string _stylesheetUrl;

        #region Properties

        public string ContentType
        {
            get { return _contentType; }
        }

        public long DocID
        {
            get { return _docID; }
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
        }

        public string FileExt
        {
            get { return _fileExt; }
        }

        public bool HasErrors
        {
            get { return _hasErrors; }
        }

        public bool IsAppendable
        {
            get { return _appendable; }
        }

        public bool IsBrowserViewable
        {
            get { return _browserViewable; }
        }

        public bool IsImage
        {
            get { return _contentType != null && _contentType.StartsWith("image/"); }
        }

        public bool IsMultiframe
        {
            get { return _multiframe; }
        }

        public UploadKeys UploadKeys
        {
            get { return _uploadKeys; }
            set { _uploadKeys = value; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor with user
        /// </summary>
        /// <param name="user">User object</param>
        public Document(User user)
        {
            _db = new AsyncDB(HandleError);
            _userID = user.UserID;
            _subuserID = user.SubuserID;
            _context = HttpContext.Current;
        }

        /// <summary>
        /// Constructor with user and document ID
        /// </summary>
        /// <param name="user">User object</param>
        /// <param name="docID">Document ID</param>
        public Document(User user, long docID)
        {
            _db = new AsyncDB(HandleError);
            _userID = user.UserID;
            _subuserID = user.SubuserID;
            _docID = docID;
            _context = HttpContext.Current;
        }

        /// <summary>
        /// Constructor with user, document ID, and group ID
        /// </summary>
        /// <param name="user">User requesting the document</param>
        /// <param name="docID">ID of the document</param>
        /// <param name="groupID">Group ID of the document</param>
        public Document(User user, long docID, long groupID)
        {
            _db = new AsyncDB(HandleError);
            _userID = user.UserID;
            _subuserID = user.SubuserID;
            _docID = docID;
            _groupID = groupID;
            _context = HttpContext.Current;
        }

        /// <summary>
        /// Constructor with GUID data
        /// </summary>
        /// <param name="data">Document data associated with a GUID</param>
        /// <param name="context">HTTP context (optional)</param>
        public Document(DocumentGuid.GuidData data, HttpContext context = null)
        {
            _db = new AsyncDB(HandleError);
            _userID = data.UserID;
            _subuserID = data.SubuserID;
            _docID = data.DocID;
            _groupID = data.GroupID;
            _guid = data.DocGuid;
            _context = context ?? HttpContext.Current;
        }

        #endregion

        #region AddPage

        /// <summary>
        /// Starts an asynchronous operation for adding a page to the document.
        /// If the page is a multiframe image, each frame will be added to the document as a new page.
        /// </summary>
        /// <param name="fileBytes"></param>
        /// <param name="callback"></param>
        /// <param name="stateObject"></param>
        /// <returns></returns>
        public IAsyncResult BeginAddPage(byte[] fileBytes, AsyncCallback callback, object stateObject)
        {
            if (fileBytes == null || _docID <= 0)
            {
                _hasErrors = true;
                callback.BeginInvoke(null, null, null);
                LogError("BeginAddPage: Invalid data");
                return null;
            }

            _fileBytes = fileBytes;
            _userCallback = callback;

            if (_multiframe)
            {
                try
                {
                    using (MemoryStream ms = new MemoryStream(_fileBytes.Length))
                    {
                        ms.Write(_fileBytes, 0, _fileBytes.Length);

                        using (Image img = Image.FromStream(ms))
                        {
                            Guid fGuid = img.FrameDimensionsList[0];
                            FrameDimension fDimension = new FrameDimension(fGuid);
                            int frameCount = img.GetFrameCount(fDimension);

                            if (frameCount > 1)
                            {
                                _frameFileBytes = new List<byte[]>();

                                for (int i = 0; i < frameCount; i++)
                                {
                                    using (MemoryStream msPage = new MemoryStream())
                                    {
                                        img.SelectActiveFrame(fDimension, i);
                                        img.Save(msPage, ImageFormat.Tiff);
                                        _frameFileBytes.Add(msPage.ToArray());
                                    }
                                }

                                _currentFrame = 0;
                                _multiframeComplete = false;

                                return BeginAddPageMultiframe(EndAddPageMultiframe, stateObject);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _hasErrors = true;
                    callback.BeginInvoke(null, null, null);
                    LogError("BeginAddPage: " + ex.Message);
                    return null;
                }
            }

            return BeginAddPage(callback, stateObject);
        }

        /// <summary>
        /// Ends the add page operation
        /// </summary>
        /// <param name="result"></param>
        public void EndAddPage(IAsyncResult result)
        {
            if (_multiframeComplete) return;
            _db.EndExecuteNonQuery(result);
        }

        /// <summary>
        /// Internal method for adding a page
        /// </summary>
        private IAsyncResult BeginAddPage(AsyncCallback callback, object stateObject)
        {
            // Insert DocumentPage record
            SqlCommand command = new SqlCommand();
            command.CommandText = "dsp_DocumentPage_Insert";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(GetSqlParameter("@UserID", _userID));
            command.Parameters.Add(GetSqlParameter("@SubuserID", _subuserID));
            command.Parameters.Add(GetSqlParameter("@DocID", _docID));
            command.Parameters.Add(GetSqlParameter("@ClientIP", GetClientIP(_context)));
            command.Parameters.Add(GetSqlParameter("@DocPageID", null, SqlDbType.BigInt, ParameterDirection.Output));
            command.Parameters.Add(GetSqlParameter("@FileData", _fileBytes, SqlDbType.VarBinary));

            return _db.BeginExecuteNonQuery(command, callback, stateObject);
        }

        /// <summary>
        /// Begin adding multiframe page
        /// </summary>
        private IAsyncResult BeginAddPageMultiframe(AsyncCallback callback, object stateObject)
        {
            _fileBytes = _frameFileBytes[_currentFrame];
            return BeginAddPage(callback, stateObject);
        }

        /// <summary>
        /// End adding multiframe page
        /// </summary>
        private void EndAddPageMultiframe(IAsyncResult result)
        {
            EndAddPage(result);
            _currentFrame++;

            if (_currentFrame < _frameFileBytes.Count)
            {
                BeginAddPageMultiframe(EndAddPageMultiframe, null);
            }
            else
            {
                _multiframeComplete = true;
                _userCallback.Invoke(result);
            }
        }

        #endregion

        #region Create

        /// <summary>
        /// Starts an asynchronous operation for creating a new document without any pages.
        /// If a group ID was specified in the constructor, the document will be added to that group.
        /// </summary>
        /// <param name="docUploadKeys"></param>
        /// <param name="callback"></param>
        /// <param name="stateObject"></param>
        /// <returns></returns>
        public IAsyncResult BeginCreate(UploadKeys docUploadKeys, AsyncCallback callback, object stateObject)
        {
            // Save to DB, get docID
            SqlCommand command = new SqlCommand();
            command.CommandText = "dsp_Document_Insert";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(GetSqlParameter("@DocDate", docUploadKeys.DocDate, SqlDbType.SmallDateTime));
            command.Parameters.Add(GetSqlParameter("@DocDescription", docUploadKeys.DocDescription));
            command.Parameters.Add(GetSqlParameter("@DocStatusID", (byte)docUploadKeys.DocStatus));
            command.Parameters.Add(GetSqlParameter("@DocTypeID", docUploadKeys.DocTypeID));
            command.Parameters.Add(GetSqlParameter("@EntityName", docUploadKeys.EntityName));
            command.Parameters.Add(GetSqlParameter("@FileExt", FormatFileExt(docUploadKeys.FileName)));
            command.Parameters.Add(GetSqlParameter("@OriginalFileName", FormatFileName(docUploadKeys.FileName)));
            command.Parameters.Add(GetSqlParameter("@InputTypeID", (byte)docUploadKeys.InputType));
            command.Parameters.Add(GetSqlParameter("@OwnerUserID", _userID));
            command.Parameters.Add(GetSqlParameter("@OwnerSubuserID", _subuserID));
            command.Parameters.Add(GetSqlParameter("@DocGuid", _guid));
            command.Parameters.Add(GetSqlParameter("@ClientIP", GetClientIP(_context)));
            command.Parameters.Add(GetSqlParameter("@DocID", null, SqlDbType.BigInt, ParameterDirection.Output));
            command.Parameters.Add(GetSqlParameter("@Multiframe", null, SqlDbType.Bit, ParameterDirection.Output));

            // Insert a new document record, output the docID
            return _db.BeginExecuteNonQuery(command, callback, stateObject);
        }

        /// <summary>
        /// Finishes the asynchronous operation for creating a new document without any pages.
        /// </summary>
        /// <param name="result"></param>
        public void EndCreate(IAsyncResult result)
        {
            Dictionary<string, object> outputParams = new Dictionary<string, object>();
            outputParams.Add("@DocID", null);
            outputParams.Add("@Multiframe", null);

            _db.EndExecuteNonQuery(result, outputParams);

            _docID = LngCheck(outputParams["@DocID"]);
            _multiframe = BoolCheck(outputParams["@Multiframe"]);

            // Add the document to the group
            if (_groupID > 0)
            {
                _group = new Group(_userID, _subuserID, _groupID);
                _group.BeginAddDocument(_docID, EndGroupAddDocument, null);
            }
        }

        /// <summary>
        /// Callback for group add document
        /// </summary>
        private void EndGroupAddDocument(IAsyncResult result)
        {
            _group.EndAddDocument(result);
        }

        #endregion

        #region Delete

        /// <summary>
        /// Starts an asynchronous operation for permanently deleting a document.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="stateObject"></param>
        /// <returns></returns>
        public IAsyncResult BeginDelete(AsyncCallback callback, object stateObject)
        {
            SqlCommand command = new SqlCommand();
            command.CommandText = "dsp_Document_Delete";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(GetSqlParameter("@DocID", _docID));
            command.Parameters.Add(GetSqlParameter("@UserID", _userID));
            command.Parameters.Add(GetSqlParameter("@SubuserID", _subuserID));
            command.Parameters.Add(GetSqlParameter("@ClientIP", GetClientIP(_context)));

            return _db.BeginExecuteNonQuery(command, callback, stateObject);
        }

        /// <summary>
        /// Ends the asynchronous operation for permanently deleting a document.
        /// </summary>
        /// <param name="result"></param>
        public void EndDelete(IAsyncResult result)
        {
            _db.EndExecuteNonQuery(result);
        }

        #endregion

        #region GeneratePDF

        /// <summary>
        /// Generates a PDF from the document
        /// </summary>
        /// <returns></returns>
        public byte[] GeneratePDF()
        {
            byte[] fileBytes = null;
            string fileExt = "";
            string contentType = "";
            string docTypeName = "";
            DateTime docDate = DateTime.MinValue;
            int pageNum = 0;
            string lastDocTypeName = "";
            string lastPagePath = "";
            string pagePath;
            long pageID;
            User user = new User(_userID, _subuserID);

            SqlCommand command = new SqlCommand();
            command.CommandText = "dsp_Document_GetPageIDs";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@DocID", _docID);

            using (PDF pdfDoc = new PDF())
            {
                using (SqlDataReader reader = DB.ExecuteReader(command))
                {
                    if (!reader.HasRows)
                    {
                        // Check if documents were found
                        return pdfDoc.GetBytes();
                    }

                    while (reader.Read())
                    {
                        fileExt = NullCheck(reader["FileExt"]).ToLower();
                        contentType = NullCheck(reader["ContentType"]);
                        docTypeName = NullCheck(reader["DocTypeName"]);
                        docDate = DateCheck(reader["DocDate"], DateTime.MinValue);
                    }

                    reader.NextResult();

                    while (reader.Read())
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            pageNum++;
                            pageID = LngCheck(reader["DocPageID"]);

                            using (DocumentPage docPage = new DocumentPage(user, pageID))
                            {
                                fileBytes = docPage.GetBytes();
                            }

                            pagePath = docTypeName + "\\" + docDate;

                            // Add document and bookmark to PDF file
                            pdfDoc.AddPage(fileBytes, PDF.PageType.Image, docTypeName, docDate, pageNum);

                            if (docTypeName != lastDocTypeName)
                            {
                                // Link the top-level bookmark to the first page of each doc type
                                pdfDoc.AddBookmark(docTypeName, false);
                            }

                            if (pageNum == 1 && fileExt != "pdf" && pagePath != lastPagePath)
                            {
                                // Link the DocDate bookmark to the first page of the document
                                pdfDoc.AddBookmark(pagePath, false);
                            }

                            lastDocTypeName = docTypeName;
                            lastPagePath = pagePath;
                        }
                    }
                }

                // Log usage
                UsageLog usageLog = new UsageLog(_context);
                UsageLog.UsageData usageData = new UsageLog.UsageData();
                usageData.UserID = _userID;
                usageData.SubuserID = _subuserID;
                usageData.ActionType = ActionType.GenerateDocumentPDF;
                usageData.DocID = _docID;
                usageLog.Insert(usageData);

                return pdfDoc.GetBytes();
            }
        }

        #endregion

        #region GetPageList

        /// <summary>
        /// Starts an asynchronous operation for retrieving data for all pages in the document.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="stateObject"></param>
        /// <returns></returns>
        public IAsyncResult BeginGetPageList(AsyncCallback callback, object stateObject)
        {
            SqlCommand command = new SqlCommand();
            command.CommandText = "dsp_Document_GetPageList";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(GetSqlParameter("@UserID", _userID));
            command.Parameters.Add(GetSqlParameter("@SubuserID", _subuserID));
            command.Parameters.Add(GetSqlParameter("@DocID", _docID));

            return _db.BeginExecuteReader(command, callback, stateObject);
        }

        /// <summary>
        /// Finishes the asynchronous operation and returns a list of data about each page.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public List<DocumentPageData> EndGetPageList(IAsyncResult result)
        {
            List<DocumentPageData> list = new List<DocumentPageData>();
            DocumentPageData docPageData;
            string appRootUrl = GetURL(_context);
            CryptoManager crypto = new CryptoManager(_context);

            using (SqlDataReader reader = _db.EndExecuteReader(result))
            {
                while (reader.Read()) // First resultset
                {
                    docPageData = new DocumentPageData();
                    docPageData.PageUrl = appRootUrl + "/Handlers/GetPage.ashx?id=" + crypto.EncryptForUrl(NullCheck(reader["DocPageGuid"]));
                    docPageData.DocPageID = LngCheck(reader["DocPageID"]);
                    docPageData.PageNumber = ShortCheck(reader["PageNumber"]);

                    list.Add(docPageData);
                }

                reader.NextResult();
                while (reader.Read()) // Second resultset
                {
                    _contentType = NullCheck(reader["ContentType"]);
                    _fileExt = NullCheck(reader["FileExt"]);
                    _appendable = BoolCheck(reader["Appendable"]);
                    _multiframe = BoolCheck(reader["Multiframe"]);
                    _browserViewable = BoolCheck(reader["BrowserViewable"]);
                }
            }

            return list;
        }

        #endregion

        #region GetUploadUrl

        /// <summary>
        /// Starts an asynchronous operation for generating a URL to upload a document.
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="docTypeID"></param>
        /// <param name="groupID"></param>
        /// <param name="stylesheetUrl"></param>
        /// <param name="entityDisplayText"></param>
        /// <param name="callback"></param>
        /// <param name="stateObject"></param>
        /// <returns></returns>
        public IAsyncResult BeginGetUploadUrl(
            string entityName,
            int docTypeID,
            long groupID,
            string stylesheetUrl,
            string entityDisplayText,
            AsyncCallback callback,
            object stateObject)
        {
            _stylesheetUrl = stylesheetUrl;
            _entityDisplayText = entityDisplayText;
            _docGuid = new DocumentGuid();
            DocumentGuid.GuidData data = new DocumentGuid.GuidData();
            data.UserID = _userID;
            data.SubuserID = _subuserID;
            data.EntityName = entityName;
            data.DocTypeID = docTypeID;
            data.GroupID = groupID;

            return _docGuid.BeginCreate(data, callback, stateObject);
        }

        /// <summary>
        /// Finishes the asynchronous operation and returns a URL to upload a document.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public string EndGetUploadUrl(IAsyncResult result)
        {
            _guid = _docGuid.EndCreate(result);
            CryptoManager crypto = new CryptoManager(_context);
            return GetURL(_context, "/Tools/DocumentUpload.aspx", 
                "id=" + crypto.EncryptForUrl(_guid.ToString()) + 
                "&styleurl=" + _context.Server.UrlEncode(_stylesheetUrl) + 
                "&e=" + _context.Server.UrlEncode(_entityDisplayText));
        }

        #endregion

        #region GetViewerUrl

        /// <summary>
        /// Starts an asynchronous operation for generating a URL to view the document.
        /// </summary>
        /// <param name="docID"></param>
        /// <param name="isReadOnly"></param>
        /// <param name="callback"></param>
        /// <param name="stateObject"></param>
        /// <returns></returns>
        public IAsyncResult BeginGetViewerUrl(
            long docID,
            bool isReadOnly,
            AsyncCallback callback,
            object stateObject)
        {
            _docGuid = new DocumentGuid();
            DocumentGuid.GuidData data = new DocumentGuid.GuidData();
            data.DocID = docID;
            data.UserID = _userID;
            data.SubuserID = _subuserID;
            data.IsReadOnly = isReadOnly;
            data.EntityName = "";

            return _docGuid.BeginCreate(data, callback, stateObject);
        }

        /// <summary>
        /// Finishes the asynchronous operation and returns a URL to view the document.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public string EndGetViewerUrl(IAsyncResult result)
        {
            _guid = _docGuid.EndCreate(result);
            CryptoManager crypto = new CryptoManager(_context);
            return GetURL(_context, "/Tools/DocumentViewer.aspx", "id=" + crypto.EncryptForUrl(_guid.ToString()));
        }

        #endregion

        #region UpdateStatus

        /// <summary>
        /// Starts an asynchronous operation for updating the status of the document.
        /// </summary>
        /// <param name="docStatus"></param>
        /// <param name="callback"></param>
        /// <param name="stateObject"></param>
        /// <returns></returns>
        public IAsyncResult BeginUpdateStatus(DocumentStatus docStatus, AsyncCallback callback, object stateObject)
        {
            SqlCommand command = new SqlCommand();
            command.CommandText = "dsp_Document_UpdateStatus";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(GetSqlParameter("@UserID", _userID));
            command.Parameters.Add(GetSqlParameter("@SubuserID", _subuserID));
            command.Parameters.Add(GetSqlParameter("@DocID", _docID));
            command.Parameters.Add(GetSqlParameter("@DocStatusID", (byte)docStatus));
            command.Parameters.Add(GetSqlParameter("@ClientIP", GetClientIP(_context)));

            return _db.BeginExecuteNonQuery(command, callback, stateObject);
        }

        /// <summary>
        /// Finishes the asynchronous operation for updating the status of the document.
        /// </summary>
        /// <param name="result"></param>
        public void EndUpdateStatus(IAsyncResult result)
        {
            _db.EndExecuteNonQuery(result);
        }

        #endregion

        #region UpdateKeys

        /// <summary>
        /// Starts an asynchronous operation for updating the keys of the document.
        /// </summary>
        /// <param name="docKeys"></param>
        /// <param name="callback"></param>
        /// <param name="stateObject"></param>
        /// <returns></returns>
        public IAsyncResult BeginUpdateKeys(DocumentKeys docKeys, AsyncCallback callback, object stateObject)
        {
            SqlCommand command = new SqlCommand();
            command.CommandText = "dsp_Document_UpdateKeys";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(GetSqlParameter("@UserID", _userID));
            command.Parameters.Add(GetSqlParameter("@SubuserID", _subuserID));
            command.Parameters.Add(GetSqlParameter("@DocID", _docID));
            command.Parameters.Add(GetSqlParameter("@DocDate", docKeys.DocDate));
            command.Parameters.Add(GetSqlParameter("@DocDescription", docKeys.DocDescription));
            command.Parameters.Add(GetSqlParameter("@DocStatusID", (byte)docKeys.DocStatus));
            command.Parameters.Add(GetSqlParameter("@DocTypeID", docKeys.DocTypeID));
            command.Parameters.Add(GetSqlParameter("@EntityName", docKeys.EntityName));
            command.Parameters.Add(GetSqlParameter("@ClientIP", GetClientIP(_context)));

            return _db.BeginExecuteNonQuery(command, callback, stateObject);
        }

        /// <summary>
        /// Finishes the asynchronous operation for updating the keys of the document.
        /// </summary>
        /// <param name="result"></param>
        public void EndUpdateKeys(IAsyncResult result)
        {
            _db.EndExecuteNonQuery(result);
        }

        #endregion

        #region Upload

        /// <summary>
        /// Starts an asynchronous operation for uploading a new document.
        /// </summary>
        /// <param name="fileBytes"></param>
        /// <param name="docUploadKeys"></param>
        /// <param name="callback"></param>
        /// <param name="stateObject"></param>
        /// <returns></returns>
        public IAsyncResult BeginUpload(byte[] fileBytes, UploadKeys docUploadKeys, AsyncCallback callback, object stateObject)
        {
            _fileBytes = fileBytes;
            _userCallback = callback;
            _userStateObject = stateObject;
            return BeginCreate(docUploadKeys, BeginUploadCallback, stateObject);
        }

        /// <summary>
        /// Finishes the asynchronous operation for uploading a new document.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public long EndUpload(IAsyncResult result)
        {
            EndAddPage(result);
            return _docID;
        }

        /// <summary>
        /// Callback for upload create
        /// </summary>
        private void BeginUploadCallback(IAsyncResult result)
        {
            EndCreate(result);
            BeginAddPage(_fileBytes, _userCallback, _userStateObject);
        }

        #endregion

        #region HandleError

        public void HandleError(IAsyncResult result, string errorMessage)
        {
            _hasErrors = true;
            _errorMessage = errorMessage;
            if (_userCallback != null)
            {
                _userCallback.Invoke(result);
                System.Threading.Thread.CurrentThread.Abort();
            }
        }

        #endregion

        #region IDisposable Support

        private bool disposedValue = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // Free managed resources
                    _fileBytes = null;
                    if (_frameFileBytes != null) _frameFileBytes.Clear();
                }

                // Free unmanaged resources
                if (_db != null) _db.Dispose();
                if (_group != null) _group.Dispose();
                if (_docGuid != null) _docGuid.Dispose();

                disposedValue = true;
            }
        }

        #endregion
    }
}
