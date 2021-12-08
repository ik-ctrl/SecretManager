using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace SecretManager.Models
{
    internal static class Crypto
    {
 
        /// <summary>
        /// Шифрование текста по протокоул AES
        /// </summary>
        /// <param name="plainText">Текст</param>
        /// <param name="cipherKey">Ключ шифрования</param>
        /// <returns></returns>
        public static string Encrypt(string plainText, string cipherKey)
        {
            var key = cipherKey.PadLeft(32);
            byte[] iv = new byte[16];
            byte[] array;
            var cryptoKey = Encoding.UTF8.GetBytes(key).Take(32);
            
            using (Aes aes = Aes.Create())
            {
                aes.Key = cryptoKey.ToArray();
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }
                        array = memoryStream.ToArray();
                    }
                }
            }
            return Convert.ToBase64String(array);
        }


        /// <summary>
        /// Расшифрока зашифрованной строки по протоколу AES
        /// </summary>
        /// <param name="cipherText">Зашифрованный текст</param>
        /// <param name="cipherKey">Ключ шифрования</param>
        /// <returns></returns>
        public static string Decrypt(string cipherText,string cipherKey)
        {
            var key = cipherKey.PadLeft(32);
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);
            var cryptoKey = Encoding.UTF8.GetBytes(key).Take(32);

            using (Aes aes = Aes.Create())
            {
                aes.Key = cryptoKey.ToArray();
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}