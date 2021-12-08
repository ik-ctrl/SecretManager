using System;
using System.IO;
using System.Text;
using System.Text.Json;

namespace SecretManager.Models
{
    internal class SecretWriter
    {
        public void Set<T>(string path,string cipherKey, T data) where T : class, new()
        {
            var lockObject = new object();
            var fullPath = path;
            var text = JsonSerializer.Serialize(data);
            var encryptText=Crypto.Encrypt(text,cipherKey);
            // var plainTextBytes = Encoding.UTF8.GetBytes(text);
            // var save = Convert.ToBase64String(plainTextBytes);
            lock (lockObject)
            {
                if (!Directory.Exists(Path.GetDirectoryName(fullPath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
                
                using (var writer = new StreamWriter(fullPath, false, Encoding.Default))
                {
                    //writer.WriteLine(save);
                    writer.WriteLine(encryptText);
                }
            }
        }
    }
}