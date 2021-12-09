using System.IO;
using System.Text;
using System.Text.Json;

namespace SecretManager.Models
{
    /// <summary>
    /// Считыватель секретов
    /// </summary>
    internal static class SecretReader
    {
        /// <summary>
        /// Запрос сохраненных секретов
        /// </summary>
        /// <typeparam name="T">Возвращаемый тип</typeparam>
        /// <param name="fullPath">Путь до файла хранения секретов</param>
        /// <param name="cipherKey">ключ шифрования</param>
        /// <returns>Сохраненный секрет</returns>
        public static T Get<T>(string fullPath,string cipherKey) where T : class, new()
        {
            var lockObject = new object();
            string crypto;
            lock (lockObject)
            {
                if (!Directory.Exists(Path.GetDirectoryName(fullPath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

                using (var reader = new StreamReader(fullPath, Encoding.Default))
                {
                    crypto = reader.ReadToEnd();
                }
            }
            var encodeString = Crypto.Decrypt(crypto, cipherKey);
            return JsonSerializer.Deserialize<T>(encodeString);
        }
    }
}