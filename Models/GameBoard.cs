using System;
using Game2048.ViewModels.Base;

namespace Game2048.Models
{
    // Класс GameBoard представляет игровое поле и его состояние.
    // Наследуется от ViewModel, чтобы поддерживать обновления UI через привязку данных.
    public class GameBoard : ViewModel
    {
        // Размер игрового поля (4x4)
        public readonly int boardSize = 4;

        // Значение, при достижении которого игрок выигрывает
        public readonly int WinValue = 2048;

        // Двумерный массив, представляющий игровое поле
        public int[,] board;

        // Очки игрока
        public int score;

        // Свойство Board с уведомлением UI об изменении
        public int[,] Board { get => board; set => Set(ref board, value); }

        // Свойство Score с уведомлением UI об изменении
        public int Score { get => score; set => Set(ref score, value); }
    }
}
