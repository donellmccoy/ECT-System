using System;
using System.Security.Cryptography;
using System.Text;

namespace ALODWebUtility.Providers
{
    public class LodCrypto
    {
        private static string key = "SD23F@#^LSKDJsfwjdad1211197dKASJ2}{#H@#I";

        private static string filling
        {
            get
            {
                string ticks = DateTime.Now.Ticks.ToString();
                int len = ticks.Length;
                return ticks.ToString().Substring(len - 6, 5);
            }
        }

        /// <summary>
        /// Decrypt as string
        /// </summary>
        /// <param name="encrypted"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string Decrypt(string encrypted)
        {
            string decrypted = null;
            byte[] inputBytes = null;

            try
            {
                inputBytes = Convert.FromBase64String(encrypted);

                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                TripleDESCryptoServiceProvider tdesProvider = new TripleDESCryptoServiceProvider();
                tdesProvider.Key = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                tdesProvider.Mode = CipherMode.ECB;
                tdesProvider.Padding = PaddingMode.PKCS7;

                decrypted = ASCIIEncoding.ASCII.GetString(
                    tdesProvider.CreateDecryptor().TransformFinalBlock(inputBytes, 0, inputBytes.Length));
            }
            catch (Exception ex)
            {
            }

            string final = decrypted.Substring(0, decrypted.Length - filling.Length);
            return final;
        }

        /// <summary>
        /// Encrypt a string
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string Encrypt(string text)
        {
            string encrypted = null;

            try
            {
                byte[] inputBytes = UTF8Encoding.UTF8.GetBytes(text + filling);
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                TripleDESCryptoServiceProvider tdesProvider = new TripleDESCryptoServiceProvider();
                tdesProvider.Key = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                tdesProvider.Mode = CipherMode.ECB;
                tdesProvider.Padding = PaddingMode.PKCS7;

                encrypted = Convert.ToBase64String(
                    tdesProvider.CreateEncryptor().TransformFinalBlock(inputBytes, 0, inputBytes.Length));
            }
            catch (Exception ex)
            {
            }

            return encrypted;
        }
    }
}
