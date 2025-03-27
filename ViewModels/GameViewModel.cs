using System;
using System.Windows;

using Game2048.Commands;
using Game2048.Data;
using Game2048.Models;
using Game2048.ViewModels.Base;

namespace Game2048.ViewModels
{
    // ViewModel для экрана игры. Управляет всей логикой игрового процесса.
    public class GameViewModel : ViewModel
    {
        private readonly GameBoard gameBoard; // Объект, представляющий игровое поле
        private readonly Random random;       // Генератор случайных чисел для генерации новых плиток

        // Свойство для доступа к полю из UI (матрица 4x4)
        public int[,] Board { get => gameBoard.board; private set => Set(ref gameBoard.board, value); }

        // Свойство для доступа к очкам из UI
        public int Score { get => gameBoard.score; private set => Set(ref gameBoard.score, value); }

        // Конструктор: инициализация поля, команд и запуск новой игры
        public GameViewModel()
        {
            gameBoard = new GameBoard();
            random = new Random();

            // Команды, которые привязываются к кнопкам на экране
            ShiftLeftCommand = new RelayCommand(ShiftLeft);
            ShiftRightCommand = new RelayCommand(ShiftRight);
            ShiftDownCommand = new RelayCommand(ShiftDown);
            ShiftUpCommand = new RelayCommand(ShiftUp);
            ResetCommand = new RelayCommand(Reset);

            Reset(); // Запуск новой игры
        }

        // Команда для перехода на страницу меню
        public static NavigationCommand NavigateToMenuPage =>
            new(NavigateToPage, new Uri("Views/Pages/MenuPage.xaml", UriKind.RelativeOrAbsolute));

        // Команды для движения плиток и перезапуска игры
        public RelayCommand ShiftLeftCommand { get; init; }
        public RelayCommand ShiftRightCommand { get; init; }
        public RelayCommand ShiftDownCommand { get; init; }
        public RelayCommand ShiftUpCommand { get; init; }
        public RelayCommand ResetCommand { get; init; }


        // Начинает новую игру: сбрасывает поле и очки, создаёт 2 плитки
        private void Reset()
        {
            Board = new int[gameBoard.boardSize, gameBoard.boardSize];
            Score = 0;
            GenerateRandomNumber();
            GenerateRandomNumber();
            Update();
        }

        // Создаёт новую плитку 2 или 4 в случайной пустой ячейке
        private void GenerateRandomNumber()
        {
            int row, col;

            do
            {
                row = random.Next(gameBoard.boardSize);
                col = random.Next(gameBoard.boardSize);
            } while (gameBoard.board[row, col] != 0); // Пока не найдём пустую

            // 90% вероятность — 2, 10% — 4
            gameBoard.board[row, col] = random.Next(100) < 90 ? 2 : 4;
        }

        // Обновляет отображаемое поле и счёт
        private void Update()
        {
            Board = gameBoard.Board;
            Score = gameBoard.Score;
        }



        // Проверка на выигрыш/проигрыш после каждого хода
        private void CheckGameState()
        {
            Update();

            if (IsGameOver())
            {
                MessageBoxResult result = MessageBox.Show(
                    "Вы проиграли! Желаете занести себя в список?",
                    "Конец",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Information
                );

                if (result == MessageBoxResult.Yes)
                    AddToStatistics();

                Reset(); // Начать заново
            }
            else if (IsGameWin())
            {
                MessageBoxResult result = MessageBox.Show(
                    "Вы выиграли! Желаете занести себя в список?",
                    "Конец",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Information
                );

                if (result == MessageBoxResult.Yes)
                    AddToStatistics();

                Reset(); // Начать заново
            }
        }

        // Победа — если есть плитка со значением 2048
        public bool IsGameWin()
        {
            for (int row = 0; row < gameBoard.boardSize; row++)
                for (int col = 0; col < gameBoard.boardSize; col++)
                    if (gameBoard.board[row, col] == gameBoard.WinValue)
                        return true;

            return false;
        }

        // Проигрыш — если нет пустых ячеек и нет ходов
        public bool IsGameOver()
        {
            for (int row = 0; row < gameBoard.boardSize; row++)
                for (int col = 0; col < gameBoard.boardSize; col++)
                    if (gameBoard.board[row, col] == 0)
                        return false; // Ещё есть пустые

            // Проверяем соседей: есть ли одинаковые рядом
            for (int row = 0; row < gameBoard.boardSize; row++)
            {
                for (int col = 0; col < gameBoard.boardSize; col++)
                {
                    int value = gameBoard.board[row, col];

                    if (row > 0 && gameBoard.board[row - 1, col] == value) return false;
                    if (row < gameBoard.boardSize - 1 && gameBoard.board[row + 1, col] == value) return false;
                    if (col > 0 && gameBoard.board[row, col - 1] == value) return false;
                    if (col < gameBoard.boardSize - 1 && gameBoard.board[row, col + 1] == value) return false;
                }
            }

            return true; // Нет ни пустых, ни ходов
        }



        // Запрос имени игрока и добавление в список результатов
        private void AddToStatistics()
        {
            string name;
            do
            {
                name = Microsoft.VisualBasic.Interaction.InputBox("Введите ваше имя: ", "Ввод имени", "");

                if (string.IsNullOrEmpty(name))
                    MessageBox.Show("Имя не может быть пустым. Пожалуйста, введите ваше имя!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

            } while (string.IsNullOrEmpty(name));

            Statistics.Add(name, Score.ToString()); // Добавляем в JSON
        }



        // Движение влево
        public void ShiftLeft()
        {
            bool shifted = false;

            for (int i = 0; i < gameBoard.board.GetLength(0); i++)
            {
                int index = 0;
                for (int j = 0; j < gameBoard.board.GetLength(1); j++)
                {
                    if (gameBoard.board[i, j] != 0)
                    {
                        if (index > 0 && gameBoard.board[i, index - 1] == gameBoard.board[i, j])
                        {
                            gameBoard.board[i, index - 1] *= 2;
                            gameBoard.board[i, j] = 0;
                            shifted = true;
                            gameBoard.score += gameBoard.board[i, index - 1];
                        }
                        else
                        {
                            if (j != index)
                            {
                                gameBoard.board[i, index] = gameBoard.board[i, j];
                                gameBoard.board[i, j] = 0;
                                shifted = true;
                            }
                            index++;
                        }
                    }
                }
            }

            if (shifted)
            {
                GenerateRandomNumber();
                CheckGameState();
            }
        }

        // Движение вправо
        public void ShiftRight()
        {
            bool shifted = false;

            for (int i = 0; i < gameBoard.board.GetLength(0); i++)
            {
                int index = gameBoard.board.GetLength(1) - 1;
                for (int j = gameBoard.board.GetLength(1) - 1; j >= 0; j--)
                {
                    if (gameBoard.board[i, j] != 0)
                    {
                        if (index < gameBoard.board.GetLength(1) - 1 && gameBoard.board[i, index + 1] == gameBoard.board[i, j])
                        {
                            gameBoard.board[i, index + 1] *= 2;
                            gameBoard.board[i, j] = 0;
                            shifted = true;
                            gameBoard.score += gameBoard.board[i, index + 1];
                        }
                        else
                        {
                            if (j != index)
                            {
                                gameBoard.board[i, index] = gameBoard.board[i, j];
                                gameBoard.board[i, j] = 0;
                                shifted = true;
                            }
                            index--;
                        }
                    }
                }
            }

            if (shifted)
            {
                GenerateRandomNumber();
                CheckGameState();
            }
        }

        // Движение вниз
        public void ShiftDown()
        {
            bool shifted = false;

            for (int j = 0; j < gameBoard.board.GetLength(1); j++)
            {
                int index = gameBoard.board.GetLength(0) - 1;
                for (int i = gameBoard.board.GetLength(0) - 1; i >= 0; i--)
                {
                    if (gameBoard.board[i, j] != 0)
                    {
                        if (index < gameBoard.board.GetLength(0) - 1 && gameBoard.board[index + 1, j] == gameBoard.board[i, j])
                        {
                            gameBoard.board[index + 1, j] *= 2;
                            gameBoard.board[i, j] = 0;
                            shifted = true;
                            gameBoard.score += gameBoard.board[index + 1, j];
                        }
                        else
                        {
                            if (i != index)
                            {
                                gameBoard.board[index, j] = gameBoard.board[i, j];
                                gameBoard.board[i, j] = 0;
                                shifted = true;
                            }
                            index--;
                        }
                    }
                }
            }

            if (shifted)
            {
                GenerateRandomNumber();
                CheckGameState();
            }
        }

        // Движение вверх
        public void ShiftUp()
        {
            bool shifted = false;

            for (int j = 0; j < gameBoard.board.GetLength(1); j++)
            {
                int index = 0;
                for (int i = 0; i < gameBoard.board.GetLength(0); i++)
                {
                    if (gameBoard.board[i, j] != 0)
                    {
                        if (index > 0 && gameBoard.board[index - 1, j] == gameBoard.board[i, j])
                        {
                            gameBoard.board[index - 1, j] *= 2;
                            gameBoard.board[i, j] = 0;
                            shifted = true;
                            gameBoard.score += gameBoard.board[index - 1, j];
                        }
                        else
                        {
                            if (i != index)
                            {
                                gameBoard.board[index, j] = gameBoard.board[i, j];
                                gameBoard.board[i, j] = 0;
                                shifted = true;
                            }
                            index++;
                        }
                    }
                }
            }

            if (shifted)
            {
                GenerateRandomNumber();
                CheckGameState();
            }
        }

    }
}
