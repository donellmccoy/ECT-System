using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using SRXLite.DataAccess;
using static SRXLite.Modules.Util;
using static SRXLite.Modules.ExceptionHandling;
using static SRXLite.DataAccess.DB;

namespace SRXLite.Classes
{
    /// <summary>
    /// Document page class for managing individual document pages
    /// </summary>
    public class DocumentPage : IDisposable
    {
        private Bitmap _bitmap;
        private bool _browserViewable;
        private string _contentType;
        private AsyncDB _db;
        private long _docPageID;
        private string _errorMessage;
        private byte[] _fileBytes;
        private string _fileExt;
        private AsyncCallback _getBitmapUserCallback;
        private bool _hasErrors = false;
        private ImageSettings _imageSettings;
        private int _subuserID;
        private AsyncCallback _userCallback;
        private short _userID;
        private object _userStateObject;
        private RotateFlipType _rotateFlipType;
        private AsyncCallback _rotateFlipUserCallback;

        #region Constructors

        /// <summary>
        /// Constructor with user
        /// </summary>
        /// <param name="user"></param>
        public DocumentPage(User user)
        {
            _db = new AsyncDB(HandleError);
            _userID = user.UserID;
            _subuserID = user.SubuserID;
        }

        /// <summary>
        /// Constructor with user and page ID
        /// </summary>
        /// <param name="user"></param>
        /// <param name="docPageID"></param>
        public DocumentPage(User user, long docPageID)
        {
            _db = new AsyncDB(HandleError);
            _userID = user.UserID;
            _subuserID = user.SubuserID;
            _docPageID = docPageID;
        }

        /// <summary>
        /// Constructor with GUID data
        /// </summary>
        /// <param name="data"></param>
        public DocumentPage(DocumentPageGuid.GuidData data)
        {
            _db = new AsyncDB(HandleError);
            _userID = data.UserID;
            _subuserID = data.SubuserID;
            _docPageID = data.DocPageID;
        }

        #endregion

        #region Properties

        public string ContentType
        {
            get { return _contentType; }
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
        }

        public string FileExtension
        {
            get { return _fileExt; }
        }

        public bool HasErrors
        {
            get { return _hasErrors; }
        }

        public bool IsBrowserViewable
        {
            get { return _browserViewable; }
        }

        public bool IsImage
        {
            get { return _contentType != null && _contentType.StartsWith("image/"); }
        }

        #endregion

        #region ImageSettings

        public struct ImageSettings
        {
            /// <summary>Height of the bitmap.</summary>
            public int Height { get; set; }

            /// <summary>RotateFlip setting to apply to the bitmap.</summary>
            public RotateFlipType RotateType { get; set; }

            /// <summary>Scale the height of the bitmap to the specified width.</summary>
            public bool ScaleHeight { get; set; }

            /// <summary>Scale the width of the bitmap to the specified height.</summary>
            public bool ScaleWidth { get; set; }

            /// <summary>Width of the bitmap.</summary>
            public int Width { get; set; }
        }

        #endregion

        #region Delete

        /// <summary>
        /// Starts an asynchronous operation for deleting a page.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="stateObject"></param>
        /// <returns></returns>
        public IAsyncResult BeginDelete(AsyncCallback callback, object stateObject)
        {
            if (_docPageID == 0) throw new Exception("DocPageID is missing");

            // Delete DB records, return file paths
            SqlCommand command = new SqlCommand();
            command.CommandText = "dsp_DocumentPage_Delete";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(GetSqlParameter("@DocPageID", _docPageID));
            command.Parameters.Add(GetSqlParameter("@UserID", _userID));
            command.Parameters.Add(GetSqlParameter("@SubuserID", _subuserID));
            command.Parameters.Add(GetSqlParameter("@ClientIP", GetClientIP()));

            return _db.BeginExecuteNonQuery(command, callback, stateObject);
        }

        /// <summary>
        /// Ends the asynchronous operation for deleting a page.
        /// </summary>
        /// <param name="result"></param>
        public void EndDelete(IAsyncResult result)
        {
            _db.EndExecuteNonQuery(result);
        }

        #endregion

        #region GetBytes

        /// <summary>
        /// Starts an asynchronous operation for retrieving the file.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="stateObject"></param>
        /// <returns></returns>
        public IAsyncResult BeginGetBytes(AsyncCallback callback, object stateObject)
        {
            SqlCommand command = new SqlCommand();
            command.CommandText = "dsp_DocumentPage_GetFile";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(GetSqlParameter("@DocPageID", _docPageID));

            return _db.BeginExecuteReader(command, callback, stateObject);
        }

        /// <summary>
        /// Ends the asynchronous operation for retrieving the file.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public byte[] EndGetBytes(IAsyncResult result)
        {
            using (SqlDataReader reader = _db.EndExecuteReader(result))
            {
                while (reader.Read())
                {
                    _contentType = NullCheck(reader["ContentType"]);
                    _browserViewable = BoolCheck(reader["BrowserViewable"]);
                    _fileExt = NullCheck(reader["FileExt"]);
                    _fileBytes = (byte[])reader["FileData"];
                }
            }

            return _fileBytes;
        }

        /// <summary>
        /// Synchronous method to get file bytes
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes()
        {
            IAsyncResult result = BeginGetBytes(null, null);
            return EndGetBytes(result);
        }

        #endregion

        #region GetBitmap

        /// <summary>
        /// Starts an asynchronous operation for retrieving an image as a bitmap.
        /// </summary>
        /// <param name="imageSettings"></param>
        /// <param name="callback"></param>
        /// <param name="stateObject"></param>
        /// <returns></returns>
        public IAsyncResult BeginGetBitmap(ImageSettings imageSettings, AsyncCallback callback, object stateObject)
        {
            _getBitmapUserCallback = callback;
            _imageSettings = imageSettings;
            return BeginGetBytes(BeginGetBitmapCallback, stateObject);
        }

        /// <summary>
        /// Ends the asynchronous operation and returns the bitmap object.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public Bitmap EndGetBitmap(IAsyncResult result)
        {
            return _bitmap;
        }

        /// <summary>
        /// Callback for bitmap retrieval
        /// </summary>
        private void BeginGetBitmapCallback(IAsyncResult result)
        {
            EndGetBytes(result); // Sets _fileBytes
            ProcessFileAsBitmap(_imageSettings);
            _getBitmapUserCallback.BeginInvoke(result, null, null);
        }

        #endregion

        #region ProcessFileAsBitmap

        /// <summary>
        /// Returns the file as a bitmap with the specified image settings applied.
        /// </summary>
        /// <param name="imageSettings"></param>
        /// <returns></returns>
        public Bitmap ProcessFileAsBitmap(ImageSettings imageSettings)
        {
            if (_fileBytes == null) throw new NotImplementedException("File data has not been initialized.");
            return ProcessFileAsBitmap(_fileBytes, imageSettings);
        }

        /// <summary>
        /// Returns the file as a bitmap with the specified image settings applied.
        /// </summary>
        /// <param name="fileBytes"></param>
        /// <param name="imageSettings"></param>
        /// <returns></returns>
        public Bitmap ProcessFileAsBitmap(byte[] fileBytes, ImageSettings imageSettings)
        {
            double aspectRatio;
            int height, width;

            using (MemoryStream ms = new MemoryStream(fileBytes, 0, fileBytes.Length))
            {
                using (Image img = Image.FromStream(ms))
                {
                    aspectRatio = img.Width / (double)img.Height;

                    // Limit resolution to that of the actual image
                    height = Math.Min(imageSettings.Height, img.Height);
                    width = Math.Min(imageSettings.Width, img.Width);

                    if (height == 0) height = img.Height;
                    if (width == 0) width = img.Width;

                    // Scale the height or width
                    if (imageSettings.ScaleHeight)
                    {
                        height = (int)Math.Round(width / aspectRatio);
                    }
                    else if (imageSettings.ScaleWidth)
                    {
                        width = (int)Math.Round(height * aspectRatio);
                    }

                    _bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                    using (Graphics g = Graphics.FromImage(_bitmap))
                    {
                        g.FillRectangle(new SolidBrush(Color.White), 0, 0, width, height);
                        g.DrawImage(img, 0, 0, width, height);
                        _bitmap.RotateFlip(imageSettings.RotateType);
                    }

                    return _bitmap; // _bitmap disposed in Dispose method
                }
            }
        }

        #endregion

        #region RotateFlip

        /// <summary>
        /// Starts an asynchronous operation for saving RotateFlip settings to an image.
        /// </summary>
        /// <param name="rotateFlipType"></param>
        /// <param name="callback"></param>
        /// <param name="stateObject"></param>
        /// <returns></returns>
        public IAsyncResult BeginRotateFlip(RotateFlipType rotateFlipType, AsyncCallback callback, object stateObject)
        {
            _rotateFlipUserCallback = callback;
            _rotateFlipType = rotateFlipType;
            return BeginGetBytes(BeginRotateFlipCallback, stateObject);
        }

        /// <summary>
        /// Ends the asynchronous operation.
        /// </summary>
        /// <param name="result"></param>
        public void EndRotateFlip(IAsyncResult result)
        {
            EndUpdate(result);
        }

        /// <summary>
        /// Callback for rotate flip operation
        /// </summary>
        private void BeginRotateFlipCallback(IAsyncResult result)
        {
            byte[] fileBytes = EndGetBytes(result);

            using (MemoryStream ms = new MemoryStream(fileBytes, 0, fileBytes.Length))
            {
                using (Bitmap bmp = new Bitmap(ms))
                {
                    try
                    {
                        bmp.RotateFlip(_rotateFlipType);

                        Encoder enc = Encoder.SaveFlag;
                        EncoderParameters encParams = new EncoderParameters(1);
                        EncoderParameter encParam = new EncoderParameter(enc, (long)EncoderValue.CompressionNone);
                        encParams.Param[0] = encParam;

                        using (MemoryStream ms2 = new MemoryStream())
                        {
                            bmp.Save(ms2, GetEncoderInfo(_contentType), encParams);
                            BeginUpdate(ms2.ToArray(), _rotateFlipUserCallback, result.AsyncState);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogError(ex.ToString(), _userID, _subuserID);
                        HandleError(result, ex.ToString(), _rotateFlipUserCallback);
                    }
                }
            }
        }

        #endregion

        #region Update

        public IAsyncResult BeginUpdate(byte[] fileData, AsyncCallback callback, object stateObject)
        {
            SqlCommand command = new SqlCommand();
            command.CommandText = "dsp_DocumentPage_UpdateFile";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(GetSqlParameter("@DocPageID", _docPageID));
            command.Parameters.Add(GetSqlParameter("@FileData", fileData));

            return _db.BeginExecuteNonQuery(command, callback, stateObject);
        }

        /// <summary>
        /// Ends the asynchronous operation for retrieving the file.
        /// </summary>
        /// <param name="result"></param>
        public void EndUpdate(IAsyncResult result)
        {
            _db.EndExecuteNonQuery(result);
        }

        #endregion

        #region HandleError

        public void HandleError(IAsyncResult result, string errorMessage)
        {
            HandleError(result, errorMessage, _userCallback);
        }

        public void HandleError(IAsyncResult result, string errorMessage, AsyncCallback callback)
        {
            _hasErrors = true;
            _errorMessage = errorMessage;
            if (callback != null)
            {
                callback.Invoke(result);
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
                }

                // Free unmanaged resources
                if (_db != null) _db.Dispose();
                if (_bitmap != null) _bitmap.Dispose();

                disposedValue = true;
            }
        }

        #endregion
    }
}
