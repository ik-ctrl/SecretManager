using System.IO;
using System.Text;
using System.Text.Json;

namespace SecretManager.Models
{
    /// <summary>
    /// Записыватель секретов
    /// </summary>
    internal static class SecretWriter
    {
        /// <summary>
        /// Сохранить секрет в файл
        /// </summary>
        /// <typeparam name="T">Сохраняемый тип</typeparam>
        /// <param name="fullPath">Путь сохранения секрета</param>
        /// <param name="cipherKey">Ключ шифрования</param>
        /// <param name="data">Данные для записи</param>
        public static void Set<T>(string fullPath, string cipherKey, T data) where T : class, new()
        {
            var lockObject = new object();
            var text = JsonSerializer.Serialize(data);
            var encryptText=Crypto.Encrypt(text,cipherKey);
            lock (lockObject)
            {
                if (!Directory.Exists(Path.GetDirectoryName(fullPath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
                
                using (var writer = new StreamWriter(fullPath, false, Encoding.Default))
                {
                    writer.WriteLine(encryptText);
                }
            }
        }
    }
}