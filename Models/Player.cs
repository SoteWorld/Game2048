namespace Game2048.Models
{
    // Класс Player представляет игрока в игре 2048.
    public class Player
    {
        // Имя игрока (устанавливается только при создании, не меняется)
        public string Name { get; init; }

        // Результат (очки), можно обновлять, если игрок набрал больше
        public string Score { get; set; }

        // Конструктор, принимающий имя и очки
        public Player(string name, string score)
        {
            Name = name;
            Score = score;
        }
    }
}
