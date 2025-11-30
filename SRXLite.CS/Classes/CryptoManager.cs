using System;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Caching;
using static SRXLite.Modules.AppSettings;

namespace SRXLite.Classes
{
    public class CryptoManager
    {
        private HttpContext _context;
        private byte[] _iv = new byte[16];
        private byte[] _key = new byte[24];

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public CryptoManager()
        {
            _context = HttpContext.Current;
            GenerateKey(EncryptionKey());
        }

        /// <summary>
        /// Constructor with context
        /// </summary>
        /// <param name="context"></param>
        public CryptoManager(HttpContext context)
        {
            _context = context;
            GenerateKey(EncryptionKey());
        }

        /// <summary>
        /// Constructor with passphrase
        /// </summary>
        /// <param name="passPhrase"></param>
        public CryptoManager(string passPhrase)
        {
            _context = HttpContext.Current;
            GenerateKey(passPhrase);
        }

        #endregion

        #region GenerateKey

        /// <summary>
        /// Generates the encryption key and initialization vector
        /// </summary>
        /// <param name="passPhrase"></param>
        private void GenerateKey(string passPhrase)
        {
            if (_context.Cache["Encryption_Key"] == null || _context.Cache["Encryption_IV"] == null)
            {
                byte[] phraseAsBytes = ASCIIEncoding.ASCII.GetBytes(passPhrase);
                SHA384CryptoServiceProvider sha384 = new SHA384CryptoServiceProvider();
                sha384.ComputeHash(phraseAsBytes);
                byte[] result = sha384.Hash;
                sha384.Clear();

                for (int i = 0; i <= 23; i++) _key[i] = result[i]; // 192-bit key
                for (int i = 24; i <= 39; i++) _iv[i - 24] = result[i]; // 128-bit IV

                // Reuse these values since they are accessed frequently by all users
                _context.Cache.Insert("Encryption_Key", _key, null, Cache.NoAbsoluteExpiration, TimeSpan.Zero);
                _context.Cache.Insert("Encryption_IV", _iv, null, Cache.NoAbsoluteExpiration, TimeSpan.Zero);
            }
            else
            {
                // Get values from cache
                _key = (byte[])_context.Cache["Encryption_Key"];
                _iv = (byte[])_context.Cache["Encryption_IV"];
            }
        }

        #endregion

        #region EncryptToBase64String

        /// <summary>
        /// Encrypts a string to Base64
        /// </summary>
        /// <param name="stringToEncrypt"></param>
        /// <returns></returns>
        private string EncryptToBase64String(string stringToEncrypt)
        {
            AesCryptoServiceProvider rijndael = new AesCryptoServiceProvider();
            byte[] inputAsBytes = ASCIIEncoding.ASCII.GetBytes(stringToEncrypt);
            string encryptedString;

            try
            {
                rijndael.KeySize = 192;
                rijndael.Key = _key;
                rijndael.IV = _iv;

                using (ICryptoTransform rijndaelTransform = rijndael.CreateEncryptor())
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, rijndaelTransform, CryptoStreamMode.Write))
                        {
                            cs.Write(inputAsBytes, 0, inputAsBytes.Length);
                            cs.FlushFinalBlock();
                            encryptedString = Convert.ToBase64String(ms.ToArray());
                        }
                    }
                }

                // Make string safe for URL
                encryptedString = encryptedString.Replace("+", "!");
                encryptedString = encryptedString.Replace("/", "-");
                encryptedString = encryptedString.Replace("=", "_");

                return encryptedString;
            }
            finally
            {
                if (rijndael != null) rijndael.Clear();
                if (inputAsBytes != null) Array.Clear(inputAsBytes, 0, inputAsBytes.Length);
            }
        }

        #endregion

        #region DecryptFromBase64String

        /// <summary>
        /// Decrypts a Base64 string
        /// </summary>
        /// <param name="stringToDecrypt"></param>
        /// <returns></returns>
        private string DecryptFromBase64String(string stringToDecrypt)
        {
            if (stringToDecrypt.Length == 0) return "";

            // Replace original characters
            stringToDecrypt = stringToDecrypt.Replace("!", "+");
            stringToDecrypt = stringToDecrypt.Replace("-", "/");
            stringToDecrypt = stringToDecrypt.Replace("_", "=");

            AesCryptoServiceProvider rijndael = new AesCryptoServiceProvider();
            byte[] inputAsBytes = Convert.FromBase64String(stringToDecrypt);

            try
            {
                rijndael.KeySize = 192;
                rijndael.Key = _key;
                rijndael.IV = _iv;

                using (ICryptoTransform rijndaelTransform = rijndael.CreateDecryptor())
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, rijndaelTransform, CryptoStreamMode.Write))
                        {
                            cs.Write(inputAsBytes, 0, inputAsBytes.Length);
                            cs.FlushFinalBlock();
                            return Encoding.UTF8.GetString(ms.ToArray());
                        }
                    }
                }
            }
            finally
            {
                if (rijndael != null) rijndael.Clear();
                if (inputAsBytes != null) Array.Clear(inputAsBytes, 0, inputAsBytes.Length);
            }
        }

        #endregion

        #region Encrypt

        /// <summary>
        /// Encrypts a string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string Encrypt(string value)
        {
            if (!IsEncryptionEnabled()) return value;
            return EncryptToBase64String(value);
        }

        #endregion

        #region Decrypt

        /// <summary>
        /// Decrypts a string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string Decrypt(string value)
        {
            if (!IsEncryptionEnabled()) return value;
            return DecryptFromBase64String(value);
        }

        #endregion

        #region EncryptForUrl

        /// <summary>
        /// Encrypts a string for URL use
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string EncryptForUrl(string value)
        {
            if (!IsEncryptionEnabled()) return _context.Server.UrlEncode(value);
            return EncryptToBase64String(value);
        }

        #endregion

        #region DecryptNameValuePairs

        /// <summary>
        /// Decrypts name-value pairs
        /// </summary>
        /// <param name="text"></param>
        /// <param name="itemDelimeter"></param>
        /// <param name="valueDelimeter"></param>
        /// <returns></returns>
        public Hashtable DecryptNameValuePairs(string text, char itemDelimeter = '&', char valueDelimeter = '=')
        {
            Hashtable data = new Hashtable();
            string[] items = Decrypt(text).Split(itemDelimeter);
            string[] nameValue;
            for (int i = 0; i < items.Length; i++)
            {
                nameValue = items[i].Split(valueDelimeter);
                if (nameValue.Length > 1)
                {
                    data.Add(nameValue[0], nameValue[1]);
                }
            }

            return data;
        }

        #endregion
    }
}
