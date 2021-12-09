using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;

namespace SecretManager.Models
{
    /// <summary>
    ///Менеджер для работы с сохранеными секретами
    /// </summary>
    public static class SecretManaged
    {
        /// <summary>
        /// Запрос сохраненого секрета 
        /// </summary>
        /// <typeparam name="T">DTO для маппига</typeparam>
        /// <returns>Запрос сохраненого</returns>
        public static T Get<T>(ILogger logger = null) where T : class, new()
        {
            try
            {
                var callingAssemblyLocation = Assembly.GetCallingAssembly().Location;
                var path = GetFullPath(callingAssemblyLocation);
                var key = GetKey(callingAssemblyLocation);
                
                if(!SecretFileExists(path))
                {
                    var data = new T();
                    SecretWriter.Set(path, key, data);
                }
                return SecretReader.Get<T>(path, key);
            }
            catch (Exception e)
            {
                logger?.LogError(e, $"Не удалось прочитать данные. Причина: {e.Message}");
                return null;
            }
        }
        
        /// <summary>
        /// Запись секрета
        /// </summary>
        /// <typeparam name="T">DTO записываемого секрета</typeparam>
        /// <param name="data">Данные для записи</param>
        /// <param name="logger">Журнал логгирования операции</param>
        /// <returns>Результат операции</returns>
        public static bool Set<T>(T data, ILogger logger = null) where T : class, new()
        {
            try
            {
                if (data == null)
                    throw new ArgumentNullException(nameof(data));
                var callingAssemblyLocation = Assembly.GetCallingAssembly().Location;
                var path = GetFullPath(callingAssemblyLocation);
                var key = GetKey(callingAssemblyLocation);
                SecretWriter.Set(path, key, data);
                return true;
            }
            catch (Exception e)
            {
                logger?.LogError(e, $"Не удалось записать данные. Причина: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// Удаление файла с секретом
        /// </summary>
        /// <param name="logger">Журнал логгирования операции</param>
        /// <returns>Результат операции</returns>
        public static bool DeleteSecret(ILogger logger = null)
        {
            try
            {
                var calledAssemblyLocation = Assembly.GetCallingAssembly().Location;
                var location = GetFullPath(calledAssemblyLocation);
                if (File.Exists(location))
                {
                    File.Delete(location);
                }
                return true;
            }
            catch (Exception e)
            {
                logger?.LogError(e, $"Не удалить секрет. Причина: {e.Message}");
                return false;
            }
        } 
        
        /// <summary>
        /// Проверка наличия файла с  конфиденциальной информацией
        /// </summary>
        /// <param name="path">Путь к проверяемому файлу</param>
        /// <param name="logger">Журнал логирования сообщения </param>
        /// <returns>Результат проверки</returns>
        private static bool SecretFileExists(string path, ILogger logger=null)
        {
            try
            {
                return File.Exists(path);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, $"Не удалось проверить наличия файла c конфиденциальной информацией. Причина: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Формирования полнрго пути к файлу
        /// </summary>
        /// <returns>Полный до файла</returns>
        private static string GetFullPath(string callingAssemblyLocation)
        {
            if (string.IsNullOrEmpty(callingAssemblyLocation))
                throw new ArgumentNullException(callingAssemblyLocation, "Не удалось установить имя вызывающей сборки");

            var directoryName = $"[{ComputeName(callingAssemblyLocation, 7)}]";
            var fileName = $"{ComputeName(Path.GetFileName(callingAssemblyLocation), 5, "")}";
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), directoryName, $"{fileName}.txt");
        }

        /// <summary>
        /// Вычисление имени
        /// </summary>
        /// <param name="path">Локация вызывающего приложения</param>
        /// <param name="byteCount">Необходимое количество байтов в имени</param>
        /// <param name="separator">Разделитель между байтами</param>
        /// <returns>Зашифрованное валидное имя</returns>
        private static string ComputeName(string path, int byteCount, string separator = "-")
        {
            if (byteCount <= 0)
                throw new ArgumentException("Не корректно задано значение необходимых байтов (byteCount <= 0)");

            byte[] bytes = Encoding.Unicode.GetBytes(path);

            var hasher = new SHA256Managed();
            var hash = hasher.ComputeHash(bytes);
            var name = string.Empty;
            var hashLength = hash.Length;
            if (hashLength < byteCount)
                byteCount = hashLength;

            var sb = new StringBuilder();
            for (var i = 0; i < byteCount; i++)
            {
                sb.Append(hash[i]);
                if (i != byteCount - 1)
                    sb.Append(separator);
            }
            return sb.ToString();
        }
        
        /// <summary>
        ///  Запрос ключа шифрования для вызова пути
        /// </summary>
        /// <param name="location">Путь сборки вызвавшая метод</param>
        /// <returns>Ключ шифрования</returns>
        private static string GetKey(string location) => Path.GetFileName(location);
    }
}