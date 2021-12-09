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
            var secret = new ImportantSecrets()
            {
                Secret1 = "Hello",
                Secret2 = "World",
                Secret3 = "!!!!"
            };
            Console.WriteLine($"Записываем=>{JsonSerializer.Serialize(secret)}");
            var result = SecretManaged.Set(secret);
            Console.WriteLine($"Результат операции записи:{result}");
            
            var returnSecret = SecretManaged.Get<ImportantSecrets>();
            Console.WriteLine($"Возвращаемое значение=>{JsonSerializer.Serialize(returnSecret)}");
            
            Console.WriteLine($"Результат удаления секрета:{SecretManaged.DeleteSecret()}");

            var emptySecret = SecretManaged.Get<ImportantSecrets>();
            Console.WriteLine($"Считывание серета если файла нет (значение по умолчанию)=>{JsonSerializer.Serialize(emptySecret)}");
            Console.WriteLine("~Конец~");
        }
    }
}
