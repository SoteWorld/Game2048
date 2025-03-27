using System.Collections.ObjectModel;
using System.Linq;
using Game2048.Models;

namespace Game2048.Data
{
    // Класс Statistics управляет списком игроков и их рекордами.
    public class Statistics
    {
        // Путь к JSON-файлу, где хранится статистика
        private const string jsonPath = "Statistics.json";

        // Свойство Players читает игроков из файла
        public static ObservableCollection<Player> Players =>
            JsonFileManager.ReadListFromJsonFile<Player>(jsonPath);

        // Метод добавления или обновления игрока в статистике
        public static void Add(string name, string score)
        {
            Player player = new(name, score);

            // Читаем текущий список игроков
            ObservableCollection<Player> players = JsonFileManager.ReadListFromJsonFile<Player>(jsonPath);

            // Если игрок с таким именем уже есть
            if (players.Any(p => p.Name == player.Name))
            {
                // Ищем его
                Player writedPlayer = players.FirstOrDefault(p => p.Name == player.Name);

                // Обновляем очки, если новые выше
                if (int.Parse(writedPlayer.Score) < int.Parse(player.Score))
                    writedPlayer.Score = player.Score;
            }
            else
            {
                // Если игрок новый — добавляем в список
                players.Add(player);
            }

            // Сохраняем обновлённую статистику обратно в JSON
            JsonFileManager.WriteToJsonFile(jsonPath, players);
        }
    }
}
