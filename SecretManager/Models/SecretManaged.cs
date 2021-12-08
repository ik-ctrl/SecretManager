using System;
using System.IO;
using Microsoft.Extensions.Logging;

namespace SecretManager.Models
{
    /// <summary>
    ///Менеджер для работы с сохранеными секретами
    /// </summary>
    public class SecretManaged
    {
        private readonly ILogger<SecretManaged> _logger;
        private readonly SecretReader _reader;
        private readonly SecretWriter _writer;
        private readonly string _key;
        private readonly string _fileName;
        private readonly string _directoryName;

        /// <summary>
        /// Инициализация
        /// </summary>
        /// <param name="key">Ключ шифрования</param>
        /// <param name="directoryName">Наименова директории хранения</param>
        /// <param name="secretFileName">Наименования файла хранения без расширения</param>
        /// <param name="logger">Журнла логгирования</param>
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

        /// <summary>
        /// Проверка наличия файла с  конфиденциальной информацией
        /// </summary>
        /// <returns>Результат проверки</returns>
        public bool SecretFileExists()
        {
            try
            {
                return File.Exists(GetFullPath());
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"Не удалось проверить наличия файла c конфиденциальной информацией. Причина: {ex.Message}");
                return false;
            }
        }


        /// <summary>
        /// Запрос сохраненого секрета 
        /// </summary>
        /// <typeparam name="T">DTO для маппига</typeparam>
        /// <returns>Запрос сохраненого</returns>
        public T Get<T>() where T : class, new()
        {
            try
            {
                return _reader.Get<T>(GetFullPath(), _key);
            }
            catch (Exception e)
            {
                _logger?.LogError(e, $"Не удалось прочитать данные. Причина: {e.Message}");
                throw;
            }
        }

        /// <summary>
        ///  Запись секрета
        /// </summary>
        /// <typeparam name="T">DTO записываемого секрета</typeparam>
        /// <param name="data">Данные для записи</param>
        /// <returns>Результат операции</returns>
        public bool Set<T>(T data) where T : class, new()
        {
            try
            {
                if (data == null)
                    throw new ArgumentNullException(nameof(data));

                var path = GetFullPath();
                _writer.Set(path, _key, data);
                return true;
            }
            catch (Exception e)
            {
                _logger?.LogError(e, $"Не удалось записать данные. Причина: {e.Message}");
                return false;
            }
        }


        /// <summary>
        /// Формирования полнрго пути к файлу
        /// </summary>
        /// <returns>Полный до файла</returns>
        private string GetFullPath() => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), _directoryName, $"{_fileName}.txt");
    }
}