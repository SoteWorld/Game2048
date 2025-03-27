using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace Game2048.Data
{
    // Класс для работы с JSON-файлами: чтение и запись статистики игроков.
    public class JsonFileManager
    {
        // Метод для записи коллекции объектов в JSON-файл
        public static void WriteToJsonFile<T>(string filePath, ObservableCollection<T> players)
        {
            JsonSerializerOptions options = new()
            {
                WriteIndented = true // форматирование JSON для удобочитаемости
            };

            string jsonString = JsonSerializer.Serialize(players, options);
            File.WriteAllText(filePath, jsonString); // запись строки в файл
        }

        // Метод для чтения коллекции объектов из JSON-файла
        public static ObservableCollection<T> ReadListFromJsonFile<T>(string filePath)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true // нечувствительность к регистру имён свойств
            };

            if (File.Exists(filePath))
            {
                return JsonDeserialize<T>(filePath, options);
            }
            else
            {
                // Если файла нет, создаём пустой и возвращаем пустую коллекцию
                File.Create(filePath).Close();
                return new ObservableCollection<T> { };
            }
        }

        // Приватный метод десериализации JSON с обработкой ошибок
        private static ObservableCollection<T> JsonDeserialize<T>(string filePath, JsonSerializerOptions options)
        {
            string jsonString = File.ReadAllText(filePath);
            try
            {
                return JsonSerializer.Deserialize<ObservableCollection<T>>(jsonString, options);
            }
            catch (JsonException)
            {
                // Если в файле некорректные данные, показываем ошибку и очищаем файл
                if (jsonString != "")
                {
                    ShowReadErrorMessage();
                    File.WriteAllText(filePath, "");
                }
                return new ObservableCollection<T> { };
            }
        }

        // Показ сообщения об ошибке при чтении
        private static void ShowReadErrorMessage()
        {
            MessageBox.Show("Ошибка при чтении статистики из файла!\nБудет выполнен сброс!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
