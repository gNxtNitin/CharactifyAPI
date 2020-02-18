using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Charactify.API.Shared
{
    public class Manager
    {
        public static string Encrypt(string plainText)
        {

            // This constant string is used as a "salt" value for the PasswordDeriveBytes function calls.
            // This size of the IV (in bytes) must = (keysize / 8).  Default keysize is 256, so the IV must be
            // 32 bytes long.  Using a 16 character string here gives us 32 bytes when converted to a byte array.
            byte[] initVectorBytes = Encoding.ASCII.GetBytes(ResponseCodeEnum.PASSWORD_KEY);

            // This constant is used to determine the keysize of the encryption algorithm.
            int keysize = 256;

            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            using (PasswordDeriveBytes password = new PasswordDeriveBytes(ResponseCodeEnum.PASS_PHRASE, null))
            {
                byte[] keyBytes = password.GetBytes(keysize / 8);
                using (RijndaelManaged symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.Mode = CipherMode.CBC;
                    using (ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes))
                    {
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                cryptoStream.FlushFinalBlock();
                                byte[] cipherTextBytes = memoryStream.ToArray();
                                return Convert.ToBase64String(cipherTextBytes);
                            }
                        }
                    }
                }
            }
        }

        public static string Decrypt(string cipherText)
        {
            if (cipherText != null && cipherText != "")
            {
                // This constant string is used as a "salt" value for the PasswordDeriveBytes function calls.
                // This size of the IV (in bytes) must = (keysize / 8).  Default keysize is 256, so the IV must be
                // 32 bytes long.  Using a 16 character string here gives us 32 bytes when converted to a byte array.
                byte[] initVectorBytes = Encoding.ASCII.GetBytes(ResponseCodeEnum.PASSWORD_KEY);

                // This constant is used to determine the keysize of the encryption algorithm.
                int keysize = 256;

                byte[] cipherTextBytes = Convert.FromBase64String(cipherText);
                using (PasswordDeriveBytes password = new PasswordDeriveBytes(ResponseCodeEnum.PASS_PHRASE, null))
                {
                    byte[] keyBytes = password.GetBytes(keysize / 8);
                    using (RijndaelManaged symmetricKey = new RijndaelManaged())
                    {
                        symmetricKey.Mode = CipherMode.CBC;
                        using (ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes))
                        {
                            using (MemoryStream memoryStream = new MemoryStream(cipherTextBytes))
                            {
                                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                                {
                                    byte[] plainTextBytes = new byte[cipherTextBytes.Length];
                                    int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                                    return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                return string.Empty;
            }
        }

        public string GetMD5Encryption(string val)
        {
            StringBuilder sb = new StringBuilder();
            MD5CryptoServiceProvider md5obj = new MD5CryptoServiceProvider();
            md5obj.ComputeHash(ASCIIEncoding.ASCII.GetBytes(val));
            byte[] result = md5obj.Hash;
            for (int i = 0; i < val.Length; i++)
            {
                sb.Append(result[i].ToString("x2"));
            }
            return sb.ToString();
        }


        public void LogError(string ex)
        {
            string message = string.Format("Time: {0}", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
            message += Environment.NewLine;
            message += "-----------------------------------------------------------";
            message += Environment.NewLine;
            message += string.Format("Message: {0}", ex);
            message += Environment.NewLine;
            //message += string.Format("StackTrace: {0}", ex.StackTrace);
            //message += Environment.NewLine;
            //message += string.Format("Source: {0}", ex.Source);
            //message += Environment.NewLine;
            //message += string.Format("TargetSite: {0}", ex.TargetSite.ToString());
            //message += Environment.NewLine;
            message += "-----------------------------------------------------------";
            message += Environment.NewLine;
            string Basepath = Directory.GetCurrentDirectory();
            // string rootPath = TPResources.env.ContentRootPath;
            String logFileName = "log_" + DateTime.Today.ToString("yyyyMMdd") + "." + "txt";
            string path = Basepath + "\\Logs\\Exception\\" + logFileName + "";

            //string path = HttpContext.Current.Server.MapPath("~/Logs/Exception/ " + logFileName + "");
            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.WriteLine(message);
                writer.Close();
            }
        }

        public void Request(string ex)
        {
            string message = string.Format("Time: {0}", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
            message += Environment.NewLine;
            message += "-----------------------------------------------------------";
            message += Environment.NewLine;
            message += string.Format("Message: {0}", ex);
            message += Environment.NewLine;
            //message += string.Format("StackTrace: {0}", ex.StackTrace);
            //message += Environment.NewLine;
            //message += string.Format("Source: {0}", ex.Source);
            //message += Environment.NewLine;
            //message += string.Format("TargetSite: {0}", ex.TargetSite.ToString());
            //message += Environment.NewLine;
            message += "-----------------------------------------------------------";
            message += Environment.NewLine;
            string Basepath = Directory.GetCurrentDirectory();
            // string rootPath = TPResources.env.ContentRootPath;
            String logFileName = "Request_" + DateTime.Today.ToString("yyyyMMdd") + "." + "txt";
            string path = Basepath + "\\Logs\\Exception\\" + logFileName + "";

            //string path = HttpContext.Current.Server.MapPath("~/Logs/Exception/ " + logFileName + "");
            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.WriteLine(message);
                writer.Close();
            }
        }
        public void Response(string ex)
        {
            string message = string.Format("Time: {0}", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
            message += Environment.NewLine;
            message += "-----------------------------------------------------------";
            message += Environment.NewLine;
            message += string.Format("Message: {0}", ex);
            message += Environment.NewLine;
            //message += string.Format("StackTrace: {0}", ex.StackTrace);
            //message += Environment.NewLine;
            //message += string.Format("Source: {0}", ex.Source);
            //message += Environment.NewLine;
            //message += string.Format("TargetSite: {0}", ex.TargetSite.ToString());
            //message += Environment.NewLine;
            message += "-----------------------------------------------------------";
            message += Environment.NewLine;
            string Basepath = Directory.GetCurrentDirectory();
            // string rootPath = TPResources.env.ContentRootPath;
            String logFileName = "Response_" + DateTime.Today.ToString("yyyyMMdd") + "." + "txt";
            string path = Basepath + "\\Logs\\Exception\\" + logFileName + "";

            //string path = HttpContext.Current.Server.MapPath("~/Logs/Exception/ " + logFileName + "");
            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.WriteLine(message);
                writer.Close();
            }
        }


        private string CreateErrorMessage(Exception ex)
        {
            throw new NotImplementedException();
        }
    }
}
