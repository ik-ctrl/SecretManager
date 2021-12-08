using System;
using System.Text.Json;
using SecretManager.Models;

namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("~Тестовый образец~");



            var secret = new ImportantSecrets();
            var manager = new SecretManaged("Ключ работает, но берет первые 32 бита для шифрования","TestDirectory","TestSecretFile",null);

            Console.WriteLine($"Проверки наличия файла:{manager.SecretFileExists()}");

            var json1 = JsonSerializer.Serialize(secret);
            Console.WriteLine("Загружаемое значение:");
            Console.WriteLine(json1);
            manager.Set(secret);

            var returnSecret = manager.Get<ImportantSecrets>();
            var json2 = JsonSerializer.Serialize(returnSecret);
            Console.WriteLine("Полученное значение");
            Console.WriteLine(json2);

            Console.WriteLine("~Конец~");
        }
    }
}
