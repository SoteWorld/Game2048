using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;
using Game2048.Commands;
using Game2048.Models;
using Game2048.Repositories;
using Game2048.ViewModels.Base;

namespace Game2048.ViewModels
{
    // ViewModel для экрана игры. Управляет игровой логикой, ходами, счётом и таймером.
    public class GameViewModel : ViewModel
    {
        private readonly GameBoard gameBoard; // Объект, представляющий игровое поле
        private readonly Random random;       // Генератор случайных чисел для создания новых плиток
        private DispatcherTimer gameTimer;    // Таймер для отслеживания времени игры
        private TimeSpan elapsedTime;         // Поле для хранения прошедшего времени
        private DateTime gameStartTime;
        private Tile[,] tileBoard;               // матрица ссылок на Tile для отслеживания позиций
        public ObservableCollection<Tile> Tiles { get; } = new();


        // Свойство, предоставляющее игровое поле для UI
        public int[,] Board { get => gameBoard.board; private set => Set(ref gameBoard.board, value); }
        // Свойство, предоставляющее текущий счёт для UI
        public int Score { get => gameBoard.score; private set => Set(ref gameBoard.score, value); }
        // Новое свойство для отображения времени игры
        public TimeSpan ElapsedTime { get => elapsedTime; set => Set(ref elapsedTime, value); }

        public GameViewModel()
        {
            gameBoard = new GameBoard();
            random = new Random();
            tileBoard = new Tile[gameBoard.boardSize, gameBoard.boardSize];


            // Инициализация таймера: обновляем каждую секунду
            gameTimer = new DispatcherTimer();
            gameTimer.Interval = TimeSpan.FromSeconds(1);
            gameTimer.Tick += (s, e) =>
            {
                ElapsedTime = ElapsedTime.Add(TimeSpan.FromSeconds(1));
            };

            // Инициализация команд для движения и перезапуска игры
            ShiftLeftCommand = new RelayCommand(ShiftLeft);
            ShiftRightCommand = new RelayCommand(ShiftRight);
            ShiftDownCommand = new RelayCommand(ShiftDown);
            ShiftUpCommand = new RelayCommand(ShiftUp);
            ResetCommand = new RelayCommand(Reset);

            // Запуск новой игры
            Reset();
        }

        #region Команды
        // Команда для навигации к меню
        public static NavigationCommand NavigateToMenuPage { get => new(NavigateToPage, new Uri("Views/Pages/MenuPage.xaml", UriKind.RelativeOrAbsolute)); }

        public RelayCommand ShiftLeftCommand { get; init; }
        public RelayCommand ShiftRightCommand { get; init; }
        public RelayCommand ShiftDownCommand { get; init; }
        public RelayCommand ShiftUpCommand { get; init; }
        public RelayCommand ResetCommand { get; init; }
        #endregion

        #region Основные операции
        // Начало новой игры: сброс игрового поля, счёта и таймера
        private void Reset()
        {
            Board = new int[gameBoard.boardSize, gameBoard.boardSize];
            tileBoard = new Tile[gameBoard.boardSize, gameBoard.boardSize];
            Tiles.Clear();
            Score = 0;
            ElapsedTime = TimeSpan.Zero; // сбрасываем таймер
            gameStartTime = DateTime.Now; // фиксируем время начала игры
            gameTimer.Start();           // запускаем таймер
            GenerateRandomNumber();
            GenerateRandomNumber();
            Update();
        }

        // Создаёт новую плитку (значение 2 или 4) в случайной пустой ячейке
        private void GenerateRandomNumber()
        {
            int row, col;
            do
            {
                row = random.Next(gameBoard.boardSize);
                col = random.Next(gameBoard.boardSize);
            } while (gameBoard.board[row, col] != 0);

            int newValue = random.Next(100) < 90 ? 2 : 4;
            gameBoard.board[row, col] = newValue;
            // Создаем модель новой плитки и добавляем в коллекцию
            var newTile = new Tile { Row = row, Col = col, Value = newValue, IsNew = true, IsMerged = false };
            tileBoard[row, col] = newTile;
            Tiles.Add(newTile);
        }

        // Обновляет отображаемые данные: поле и счёт
        private void Update()
        {
            Board = gameBoard.Board;
            Score = gameBoard.Score;
        }
        #endregion

        #region Проверка состояния игры
        private void CheckGameState()
        {
            Update();
            if (IsGameOver())
            {
                MessageBoxResult result = MessageBox.Show("Вы проиграли! Желаете сохранить статистику?", "Конец", MessageBoxButton.YesNo, MessageBoxImage.Information);
                if (result == MessageBoxResult.Yes)
                {
                    UpdateStatistics();
                }
                Reset();
            }
            else if (IsGameWin())
            {
                MessageBoxResult result = MessageBox.Show("Вы выиграли! Желаете сохранить статистику?", "Конец", MessageBoxButton.YesNo, MessageBoxImage.Information);
                if (result == MessageBoxResult.Yes)
                {
                    UpdateStatistics();
                }
                Reset();
            }
        }

        public bool IsGameWin()
        {
            for (int row = 0; row < gameBoard.boardSize; row++)
            {
                for (int col = 0; col < gameBoard.boardSize; col++)
                {
                    if (gameBoard.board[row, col] == gameBoard.WinValue)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool IsGameOver()
        {
            for (int row = 0; row < gameBoard.boardSize; row++)
            {
                for (int col = 0; col < gameBoard.boardSize; col++)
                {
                    if (gameBoard.board[row, col] == 0)
                    {
                        return false;
                    }
                }
            }
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
            return true;
        }
        #endregion

        #region Статистика
        // Метод UpdateStatistics обновляет статистику текущего пользователя в базе данных с помощью MongoDB
        private void UpdateStatistics()
        {
            if (UserSession.CurrentUser != null)
            {
                // Вычисляем продолжительность игры в секундах
                long gameDurationInSeconds = (long)(DateTime.Now - gameStartTime).TotalSeconds;
                int currentScore = Score; // текущий счет игры

                var repo = new UserRepository();
                repo.IncrementUserStats(UserSession.CurrentUser.Id, currentScore, gameDurationInSeconds);

                // Обновляем объект сессии для отображения (если требуется)
                UserSession.CurrentUser.GamesPlayed += 1;
                UserSession.CurrentUser.TotalPlayTimeInSeconds += gameDurationInSeconds;
                if (currentScore > UserSession.CurrentUser.MaxScore)
                {
                    UserSession.CurrentUser.MaxScore = currentScore;
                }
            }
        }
        #endregion

        #region Движения плиток
        public void ShiftLeft()
        {
            bool shifted = false;
            // Сброс флагов анимации перед ходом
            foreach (var t in Tiles)
            {
                t.IsNew = false;
                t.IsMerged = false;
            }

            for (int i = 0; i < gameBoard.board.GetLength(0); i++)
            {
                int index = 0;
                for (int j = 0; j < gameBoard.board.GetLength(1); j++)
                {
                    if (gameBoard.board[i, j] != 0)
                    {
                        if (index > 0 && gameBoard.board[i, index - 1] == gameBoard.board[i, j])
                        {
                            // Объединение плиток
                            gameBoard.board[i, index - 1] *= 2;
                            gameBoard.board[i, j] = 0;
                            shifted = true;
                            gameBoard.score += gameBoard.board[i, index - 1];
                            // Обновляем модели: удаляем исходную плитку, удваиваем значение целевой
                            Tile tileToRemove = tileBoard[i, j];
                            Tile tileMergedInto = tileBoard[i, index - 1];
                            Tiles.Remove(tileToRemove);
                            tileBoard[i, j] = null;
                            tileMergedInto.Value = gameBoard.board[i, index - 1];
                            tileMergedInto.IsMerged = true;
                        }
                        else
                        {
                            if (j != index)
                            {
                                // Перемещение плитки
                                gameBoard.board[i, index] = gameBoard.board[i, j];
                                gameBoard.board[i, j] = 0;
                                tileBoard[i, index] = tileBoard[i, j];
                                tileBoard[i, j] = null;
                                // Обновляем координаты перемещенной плитки (для анимации)
                                if (tileBoard[i, index] != null)
                                {
                                    tileBoard[i, index].Row = i;
                                    tileBoard[i, index].Col = index;
                                }
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

        public void ShiftRight()
        {
            bool shifted = false;
            foreach (var t in Tiles) { t.IsNew = false; t.IsMerged = false; }

            for (int i = 0; i < gameBoard.board.GetLength(0); i++)
            {
                int index = gameBoard.board.GetLength(1) - 1;
                for (int j = gameBoard.board.GetLength(1) - 1; j >= 0; j--)
                {
                    if (gameBoard.board[i, j] != 0)
                    {
                        if (index < gameBoard.board.GetLength(1) - 1 && gameBoard.board[i, index + 1] == gameBoard.board[i, j])
                        {
                            // Объединение вправо
                            gameBoard.board[i, index + 1] *= 2;
                            gameBoard.board[i, j] = 0;
                            shifted = true;
                            gameBoard.score += gameBoard.board[i, index + 1];
                            Tile tileToRemove = tileBoard[i, j];
                            Tile tileMergedInto = tileBoard[i, index + 1];
                            Tiles.Remove(tileToRemove);
                            tileBoard[i, j] = null;
                            tileMergedInto.Value = gameBoard.board[i, index + 1];
                            tileMergedInto.IsMerged = true;
                        }
                        else
                        {
                            if (j != index)
                            {
                                // Перемещение вправо
                                gameBoard.board[i, index] = gameBoard.board[i, j];
                                gameBoard.board[i, j] = 0;
                                tileBoard[i, index] = tileBoard[i, j];
                                tileBoard[i, j] = null;
                                if (tileBoard[i, index] != null)
                                {
                                    tileBoard[i, index].Row = i;
                                    tileBoard[i, index].Col = index;
                                }
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

        public void ShiftDown()
        {
            bool shifted = false;
            foreach (var t in Tiles) { t.IsNew = false; t.IsMerged = false; }

            for (int j = 0; j < gameBoard.board.GetLength(1); j++)
            {
                int index = gameBoard.board.GetLength(0) - 1;
                for (int i = gameBoard.board.GetLength(0) - 1; i >= 0; i--)
                {
                    if (gameBoard.board[i, j] != 0)
                    {
                        if (index < gameBoard.board.GetLength(0) - 1 && gameBoard.board[index + 1, j] == gameBoard.board[i, j])
                        {
                            // Объединение вниз
                            gameBoard.board[index + 1, j] *= 2;
                            gameBoard.board[i, j] = 0;
                            shifted = true;
                            gameBoard.score += gameBoard.board[index + 1, j];
                            Tile tileToRemove = tileBoard[i, j];
                            Tile tileMergedInto = tileBoard[index + 1, j];
                            Tiles.Remove(tileToRemove);
                            tileBoard[i, j] = null;
                            tileMergedInto.Value = gameBoard.board[index + 1, j];
                            tileMergedInto.IsMerged = true;
                        }
                        else
                        {
                            if (i != index)
                            {
                                // Перемещение вниз
                                gameBoard.board[index, j] = gameBoard.board[i, j];
                                gameBoard.board[i, j] = 0;
                                tileBoard[index, j] = tileBoard[i, j];
                                tileBoard[i, j] = null;
                                if (tileBoard[index, j] != null)
                                {
                                    tileBoard[index, j].Row = index;
                                    tileBoard[index, j].Col = j;
                                }
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

        public void ShiftUp()
        {
            bool shifted = false;
            foreach (var t in Tiles) { t.IsNew = false; t.IsMerged = false; }

            for (int j = 0; j < gameBoard.board.GetLength(1); j++)
            {
                int index = 0;
                for (int i = 0; i < gameBoard.board.GetLength(0); i++)
                {
                    if (gameBoard.board[i, j] != 0)
                    {
                        if (index > 0 && gameBoard.board[index - 1, j] == gameBoard.board[i, j])
                        {
                            // Объединение вверх
                            gameBoard.board[index - 1, j] *= 2;
                            gameBoard.board[i, j] = 0;
                            shifted = true;
                            gameBoard.score += gameBoard.board[index - 1, j];
                            Tile tileToRemove = tileBoard[i, j];
                            Tile tileMergedInto = tileBoard[index - 1, j];
                            Tiles.Remove(tileToRemove);
                            tileBoard[i, j] = null;
                            tileMergedInto.Value = gameBoard.board[index - 1, j];
                            tileMergedInto.IsMerged = true;
                        }
                        else
                        {
                            if (i != index)
                            {
                                // Перемещение вверх
                                gameBoard.board[index, j] = gameBoard.board[i, j];
                                gameBoard.board[i, j] = 0;
                                tileBoard[index, j] = tileBoard[i, j];
                                tileBoard[i, j] = null;
                                if (tileBoard[index, j] != null)
                                {
                                    tileBoard[index, j].Row = index;
                                    tileBoard[index, j].Col = j;
                                }
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
        #endregion
    }
}
