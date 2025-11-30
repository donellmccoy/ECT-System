using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using ALOD.Logging;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;

namespace ALODWebUtility.Common
{
    public class SecureQueryString : IDictionary
    {
        #region Members/Properties

        protected string _hash = string.Empty;
        protected bool _isValid = false;
        protected char[] _querySeperators = { '?', '&' };
        protected string _queryVar = "data";
        protected string _rawQuery = string.Empty;
        protected char _sep = '|';
        protected bool _validHash = false;
        private static string _password = "f1dr32eg@@#WWEa123D&*";
        private static string _salt = "saltyDoggy";
        private string _hashMark = "|_h=";
        private Hashtable _values;

        /// <summary>
        /// Returns the entire query string encrypted as a block
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string EncryptedString
        {
            get
            {
                //we have to UrlEncode this twice in order to avoid the random encoding problems that
                //can occur with single encoding
                //if a single encoded string randomly hits the sequence *On*= (or in regex terms: [^a-zA-Z]on[a-zA-Z]*\s*= )
                //then the CrossSiteScriptingValidation will flag it as potentially dangerous
                //and throw and error, so we encode twice which will get around that
                return HttpContext.Current.Server.UrlEncode(
                    HttpContext.Current.Server.UrlEncode(Encrypt(this.RawString)));
            }
            set
            {
                Parse(Decrypt(value));
            }
        }

        /// <summary>
        /// Indicates if the decrypted string matches its hash
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks>If this value is FALSE the data cannot be trusted</remarks>
        public bool IsValid
        {
            get
            {
                return (_isValid && _validHash);
            }
        }

        public string QueryStringItem
        {
            get
            {
                return _queryVar;
            }
            set
            {
                _queryVar = value;
            }
        }

        /// <summary>
        /// Returns the unencrypted query string
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public string RawString
        {

            get
            {
                return this.ToString();
            }
            set
            {
                Parse(value);
                _rawQuery = this.ToString();
            }

        }

        /// <summary>
        /// Returns the key/value pairs stored in the query string as plain text
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string ToString()
        {
            StringBuilder buffer = new StringBuilder();

            //convert our key value pairs to a string
            foreach (DictionaryEntry item in _values)
            {

                if (buffer.Length > 0)
                {
                    buffer.Append(_sep);
                }

                buffer.Append(item.Key.ToString() + "=" + item.Value.ToString());
            }

            return buffer.ToString();
        }

        #endregion

        #region Getters

        public bool GetBoolean(string key)
        {

            if (this[key].ToString().Length > 0)
            {

                try
                {
                    return bool.Parse(this[key].ToString());
                }
                catch (ArgumentNullException argNull)
                {
                    LogManager.LogError(argNull);
                    bool rethrow = ExceptionPolicy.HandleException(argNull, "General");
                    if (rethrow) throw;
                }
                catch (FormatException format)
                {
                    LogManager.LogError(format);
                    bool rethrow = ExceptionPolicy.HandleException(format, "General");
                    if (rethrow) throw;
                }
            }

            return false; // Default value or exception handled
        }

        public byte GetByte(string key)
        {

            //if they key is not found it returns an empty string
            //either way, isnumeric will fail if it's not a number or not found
            // IsNumeric is VB specific. In C# we can try parsing.
            double num;
            if (double.TryParse(this[key].ToString(), out num))
            {
                try
                {
                    return byte.Parse(this[key].ToString());
                }
                catch (ArgumentNullException argNull)
                {
                    LogManager.LogError(argNull, "SecureQueryString.GetByte");
                    bool rethrow = ExceptionPolicy.HandleException(argNull, "General");
                    if (rethrow) throw;
                }
                catch (FormatException format)
                {
                    LogManager.LogError(format, "SecureQueryString.GetByte");
                    bool rethrow = ExceptionPolicy.HandleException(format, "General");
                    if (rethrow) throw;
                }
            }
            return 0; // Default value
        }

        public DateTime GetDateTime(string key)
        {

            if (_values.ContainsKey(key.ToLower()))
            {

                try
                {
                    return DateTime.Parse(this[key].ToString());
                }
                catch (ArgumentNullException argNull)
                {
                    LogManager.LogError(argNull, "SecureQueryString.GetDateTime");
                    bool rethrow = ExceptionPolicy.HandleException(argNull, "General");
                    if (rethrow) throw;
                }
                catch (FormatException format)
                {
                    LogManager.LogError(format, "SecureQueryString.GetDateTime");
                    bool rethrow = ExceptionPolicy.HandleException(format, "General");
                    if (rethrow) throw;
                }

            }
            return DateTime.MinValue; // Default value
        }

        public int GetDouble(string key) // The VB code returns Integer but name is GetDouble. I'll follow VB return type Integer.
        {

            //if they key is not found it returns an empty string
            //either way, isnumeric will fail if it's not a number or not found
            double num;
            if (double.TryParse(this[key].ToString(), out num))
            {
                try
                {
                    return (int)double.Parse(this[key].ToString()); // VB code parses Double but returns Integer?
                    // VB: Return Double.Parse(Me(key))
                    // The return type of function is Integer.
                    // So it casts Double to Integer.
                }
                catch (ArgumentNullException argNull)
                {
                    LogManager.LogError(argNull, "SecureQueryString.GetDouble");
                    bool rethrow = ExceptionPolicy.HandleException(argNull, "General");
                    if (rethrow) throw;
                }
                catch (FormatException format)
                {
                    LogManager.LogError(format, "SecureQueryString.GetDouble");
                    bool rethrow = ExceptionPolicy.HandleException(format, "General");
                    if (rethrow) throw;
                }
            }
            return 0;
        }

        public short GetInt16(string key)
        {

            //if they key is not found it returns an empty string
            //either way, isnumeric will fail if it's not a number or not found
            double num;
            if (double.TryParse(this[key].ToString(), out num))
            {
                try
                {
                    return short.Parse(this[key].ToString());
                }
                catch (ArgumentNullException argNull)
                {
                    LogManager.LogError(argNull, "SecureQueryString.GetInt16");
                    bool rethrow = ExceptionPolicy.HandleException(argNull, "General");
                    if (rethrow) throw;
                }
                catch (FormatException format)
                {
                    LogManager.LogError(format, "SecureQueryString.GetInt16");
                    bool rethrow = ExceptionPolicy.HandleException(format, "General");
                    if (rethrow) throw;
                }
            }
            return 0;
        }

        public int GetInt32(string key)
        {

            //if they key is not found it returns an empty string
            //either way, isnumeric will fail if it's not a number or not found
            double num;
            if (double.TryParse(this[key].ToString(), out num))
            {
                try
                {
                    return int.Parse(this[key].ToString());
                }
                catch (ArgumentNullException argNull)
                {
                    LogManager.LogError(argNull, "SecureQueryString.GetInt32");
                    bool rethrow = ExceptionPolicy.HandleException(argNull, "General");
                    if (rethrow) throw;
                }
                catch (FormatException format)
                {
                    LogManager.LogError(format, "SecureQueryString.GetInt32");
                    bool rethrow = ExceptionPolicy.HandleException(format, "General");
                    if (rethrow) throw;
                }
            }

            return 0;

        }

        public long GetInt64(string key)
        {

            //if they key is not found it returns an empty string
            //either way, isnumeric will fail if it's not a number or not found
            double num;
            if (double.TryParse(this[key].ToString(), out num))
            {
                try
                {
                    return long.Parse(this[key].ToString());
                }
                catch (ArgumentNullException argNull)
                {
                    LogManager.LogError(argNull, "SecureQueryString.GetInt64");
                    bool rethrow = ExceptionPolicy.HandleException(argNull, "General");
                    if (rethrow) throw;
                }
                catch (FormatException format)
                {
                    LogManager.LogError(format, "SecureQueryString.GetInt64");
                    bool rethrow = ExceptionPolicy.HandleException(format, "General");
                    if (rethrow) throw;
                }
            }
            return 0;
        }

        public int GetInteger(string key)
        {
            return GetInt32(key);
        }

        public string GetString(string key)
        {
            return this[key].ToString();
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates an empty query string
        /// </summary>
        /// <remarks></remarks>
        public SecureQueryString()
        {
            _values = new Hashtable();
        }

        /// <summary>
        /// Used to decrypt an encrypted query string
        /// </summary>
        /// <param name="input">Request.RawUrl</param>
        /// <remarks>The raw url data from the request must be parsed for this to function properly</remarks>
        public SecureQueryString(string input)
        {

            string[] parts = input.Split(_querySeperators);
            string dataPart = string.Empty;

            foreach (string key in parts)
            {
                if (key.IndexOf(_queryVar) == 0)
                {
                    int index = key.IndexOf("=");
                    dataPart = key.Substring(index + 1);
                    break;
                }
            }

            //if we didn't find our data= block then we can assume we were handed a raw data block
            //rather then a query string
            if (dataPart.Length == 0)
            {
                dataPart = input;
            }

            //make sure we are dealing with an unencoded string
            //if the string has already been decoded calling decode again won't hurt anything
            dataPart = HttpContext.Current.Server.UrlDecode(HttpContext.Current.Server.UrlDecode(dataPart));

            //decode has a bad habit of replacing + with a space, so undo that
            //since spaces are not valid in an encrypted string, we can safely do this
            dataPart = dataPart.Replace(" ", "+");

            _values = new Hashtable();

            if (dataPart.Length > 0)
            {
                //make sure we have something to decrypt
                Parse(Decrypt(dataPart));
            }

        }

        public SecureQueryString(string input, bool IsRawQuery)
        {

            string[] parts = input.Split(_querySeperators);
            string dataPart = string.Empty;

            foreach (string key in parts)
            {
                if (key.IndexOf(_queryVar) == 0)
                {
                    int index = key.IndexOf("=");
                    dataPart = key.Substring(index + 1);
                    break;
                }
            }

            //if we didn't find our data= block then we can assume we were handed a raw data block
            //rather then a query string
            if (dataPart.Length == 0)
            {
                dataPart = input;
            }

            //make sure we are dealing with an unencoded string
            //if the string has already been decoded calling decode again won't hurt anything
            dataPart = HttpContext.Current.Server.UrlDecode(HttpContext.Current.Server.UrlDecode(dataPart));

            //decode has a bad habit of replacing + with a space, so undo that
            //since spaces are not valid in an encrypted string, we can safely do this
            dataPart = dataPart.Replace(" ", "+");

            _values = new Hashtable();

            if (dataPart.Length > 0)
            {
                //make sure we have something to decrypt
                if (!IsRawQuery)
                {
                    Parse(Decrypt(dataPart));
                }
                else
                {
                    ParseRaw(dataPart);
                }

            }

        }

        #endregion

        #region Parsers

        protected void Parse(string input)
        {

            //strip off the leading seperator if there is one (there shouldn't be)
            if (input.IndexOf(_sep) == 0)
            {
                input = input.Substring(1);
            }

            //split our string into key/value pairs
            string[] parts = input.Split(_sep);
            string[] pair;

            foreach (string value in parts)
            {

                //now split each key/value pair
                pair = value.Split(new char[] { '=' }, 2, StringSplitOptions.RemoveEmptyEntries);

                //if they are both valid add them to the hashtable
                if (pair.Length == 2)
                {
                    _values.Add(pair[0].ToLower(), pair[1]);
                }

            }

            _isValid = true;

        }

        protected void ParseRaw(string input)
        {

            char[] _qSep = { ';', '&' };

            //strip off the leading seperator if there is one (there shouldn't be)
            if (input.IndexOf(_sep) == 0)
            {
                input = input.Substring(1);
            }

            //split our string into key/value pairs
            string[] parts = input.Split(_qSep);
            string[] pair;

            foreach (string value in parts)
            {

                //now split each key/value pair
                pair = value.Split(new char[] { '=' }, 2, StringSplitOptions.RemoveEmptyEntries);

                //if they are both valid add them to the hashtable
                if (pair.Length == 2)
                {
                    _values.Add(pair[0].ToLower(), pair[1]);
                }

            }

            _isValid = true;

        }

        #endregion

        #region Encrypt/Decrypt

        protected static byte[] DecryptData(byte[] data, PaddingMode paddingMode)
        {

            if ((data == null) || (data.Length == 0))
            {
                throw new ArgumentNullException("data");
            }

            if (_password.Length == 0)
            {
                throw new ArgumentNullException("password");
            }

            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(_password, Encoding.UTF8.GetBytes(_salt));
            RijndaelManaged rm = new RijndaelManaged();
            rm.Padding = paddingMode;
            ICryptoTransform decryptor = rm.CreateDecryptor(pdb.GetBytes(16), pdb.GetBytes(16));
            MemoryStream msDecrypt = new MemoryStream(data);
            CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            // Decrypted bytes will always be less then encrypted bytes, so len of encrypted data will be big enouph for buffer.
            byte[] fromEncrypt = new byte[data.Length];

            // Read as many bytes as possible.
            int read = csDecrypt.Read(fromEncrypt, 0, fromEncrypt.Length);

            if (read < fromEncrypt.Length)
            {
                // Return a byte array of proper size.
                byte[] clearBytes = new byte[read];
                Buffer.BlockCopy(fromEncrypt, 0, clearBytes, 0, read);
                return clearBytes;
            }

            return fromEncrypt;

        }

        protected static byte[] EncryptData(byte[] data, PaddingMode paddingMode)
        {

            if ((data == null) || (data.Length == 0))
            {
                throw new ArgumentNullException("data");
            }

            if (_password.Length == 0)
            {
                throw new ArgumentNullException("password");
            }

            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(_password, Encoding.UTF8.GetBytes(_salt));
            RijndaelManaged rm = new RijndaelManaged();
            rm.Padding = paddingMode;
            ICryptoTransform encryptor = rm.CreateEncryptor(pdb.GetBytes(16), pdb.GetBytes(16));

            MemoryStream msEncrypt = new MemoryStream();
            CryptoStream encStream = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
            encStream.Write(data, 0, data.Length);
            encStream.FlushFinalBlock();
            return msEncrypt.ToArray();

        }

        protected string Decrypt(string data)
        {

            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            if (_password.Length == 0)
            {
                throw new ArgumentNullException("password");
            }

            try
            {

                byte[] encBytes = Convert.FromBase64String(data);

                if (encBytes.Length > 0)
                {
                    //only decrypt it if we have a valid string to work with
                    byte[] decBytes = DecryptData(encBytes, PaddingMode.PKCS7);
                    _rawQuery = Encoding.UTF8.GetString(decBytes);
                }
            }
            catch (FormatException)
            {
                //we got some bad data, might have been tampered with, might just be an error
                //either way, we can't decrypt it
                //ErrorLog.LogError(ex, "Decrypt")
            }

            //chop off the trailing null if it's there
            //on multi-part strings an extra null gets appended, so strip it off
            if ((_rawQuery.Length > 0) && (_rawQuery[_rawQuery.Length - 1] == '\0'))
            {
                _rawQuery = _rawQuery.Substring(0, _rawQuery.Length - 1);
            }

            //before we return our string, pull our hash off the end and make sure it's valid
            int index = _rawQuery.IndexOf(_hashMark);

            if (index != -1)
            {
                //we have a hash, parse it out
                string hash = _rawQuery.Substring(index + _hashMark.Length);
                _rawQuery = _rawQuery.Substring(0, index);
                //now get the hash from what's left  and compare the two
                string queryHash = GetHash(_rawQuery);
                _validHash = (queryHash == hash);
            }

            if (!_validHash)
            {
                //this is a bad string, clear out our values and mark it as invalid
                _validHash = false;
                _rawQuery = string.Empty;
            }

            return _rawQuery;

        }

        protected string Encrypt(string data)
        {

            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            if (_password.Length == 0)
            {
                throw new ArgumentNullException("password");
            }

            //first, get the hash of our string
            string hash = GetHash(data);

            //now append our hash to our string
            data += _hashMark + hash;

            //now encrypt the whole thing
            byte[] encBytes = EncryptData(Encoding.UTF8.GetBytes(data), PaddingMode.PKCS7);
            return Convert.ToBase64String(encBytes);

        }

        protected string GetHash(string data)
        {

            byte[] rawBytes = Encoding.UTF8.GetBytes(data);
            byte[] hash = new MD5CryptoServiceProvider().ComputeHash(rawBytes);

            StringBuilder buffer = new StringBuilder(hash.Length);

            //convert our hash to a hex string
            for (int i = 0; i < hash.Length; i++)
            {
                buffer.Append(hash[i].ToString("X2"));
            }

            return buffer.ToString();

        }

        #endregion

        #region IDictionary Methods

        public int Count
        {
            get
            {
                return _values.Count;
            }
        }

        public bool IsFixedSize
        {
            get
            {
                return _values.IsFixedSize;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return _values.IsReadOnly;
            }
        }

        public bool IsSynchronized
        {
            get
            {
                return _values.IsSynchronized;
            }
        }

        public ICollection Keys
        {
            get
            {
                return _values.Keys;
            }
        }

        public object SyncRoot
        {
            get
            {
                return _values.SyncRoot;
            }
        }

        public ICollection Values
        {
            get
            {
                return _values.Values;
            }
        }

        public object this[object key]
        {
            get
            {
                string skey = key.ToString().ToLower();

                if (_values.ContainsKey(skey))
                {

                    object o = _values[skey];
                    if (o is string)
                    {
                        return ((string)o).Trim();
                    }

                    return o;
                }

                return string.Empty;

            }
            set
            {

                string skey = key.ToString().ToLower();

                if (_values.ContainsKey(skey))
                {
                    _values[skey] = value;
                }
                else
                {
                    //this is a new one, so add it first
                    _values.Add(skey, value);
                }

            }
        }

        public void Add(object key, object value)
        {
            _values.Add(key.ToString().ToLower(), value);
        }

        public void Clear()
        {
            _values.Clear();
        }

        public bool Contains(object key)
        {
            return _values.Contains(key.ToString().ToLower());
        }

        public void CopyTo(Array array, int index)
        {
            _values.CopyTo(array, index);
        }

        public IDictionaryEnumerator GetEnumerator()
        {
            return _values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _values.GetEnumerator();
        }

        public void Remove(object key)
        {
            _values.Remove(key.ToString().ToLower());
        }

        #endregion
    }
}
