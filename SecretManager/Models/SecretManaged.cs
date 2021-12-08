using System;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;

namespace SecretManager.Models
{
    public class SecretManaged
    {
        private readonly ILogger<SecretManaged> _logger;
        private readonly SecretReader _reader;
        private readonly SecretWriter _writer;
        private readonly string _key;
        private readonly string _fileName;
        private readonly string _directoryName;
        private object lockObject = new object();


        public SecretManaged(string key, string directoryName = null, string secretFileName = null, ILogger<SecretManaged> logger = null)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(key);
            _key = key.Trim();
            _directoryName = string.IsNullOrEmpty(directoryName) ? "Secrets" : secretFileName;
            _fileName = string.IsNullOrEmpty(secretFileName) ? "secret" : secretFileName;
            _reader = new SecretReader();
            _writer = new SecretWriter();
            _logger = logger;
        }


        public T Get<T>() where T : class, new()
        {
            try
            {
                return _reader.Get<T>(GetFullPath());
            }
            catch (Exception e)
            {
                _logger?.LogError(e, $"Не удалось прочитать данные. Причина: {e.Message}");
                throw;
            }
        }

        public bool Set<T>(T data) where T : class, new()
        {
            try
            {
                var path = GetFullPath();
                _writer.Set(path, _key,data);
                return true;
            }
            catch (Exception e)
            {
                _logger?.LogError(e, $"Не удалось записать данные. Причина: {e.Message}");
                return false;
            }
        }
        

        private string GetFullPath() => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), _directoryName, $"{_fileName}.txt");
    }
}