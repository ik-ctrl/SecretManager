using System;
using System.IO;
using System.Text;
using System.Text.Json;

namespace SecretManager.Models
{
    internal class SecretReader
    {
        public T Get<T>(string fullPath) where T : class, new()
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
            var encodeString = Crypto.Decrypt(crypto);
            return JsonSerializer.Deserialize<T>(encodeString);
        }
    }
}