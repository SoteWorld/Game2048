using System.Windows;
using System.Windows.Input;
using Game2048.Commands;
using Game2048.Models;
using Game2048.Repositories;
using Game2048.ViewModels.Base;
using Game2048.View.Pages;
using System.Windows.Controls;
using Game2048.Views.Windows;  // Добавляем пространство имён с MenuPage


namespace Game2048.ViewModels
{
    // ViewModel для страницы регистрации (RegistrationPage)
    public class RegistrationViewModel : ViewModel
    {
        private string _username;
        public string Username
        {
            get => _username;
            set => Set(ref _username, value);
        }

        private string _password;
        public string Password
        {
            get => _password;
            set => Set(ref _password, value);
        }

        private string _confirmPassword;
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => Set(ref _confirmPassword, value);
        }

        // Команда для регистрации
        public ICommand RegisterCommand { get; }

        public RegistrationViewModel()
        {
            RegisterCommand = new RelayCommand(Register);
        }

        private void Register()
        {
            // Проверяем, что поля не пустые

            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                MessageBox.Show("Username and Password cannot be empty.");
                return;
            }

            // Проверяем, совпадают ли пароли
            if (Password != ConfirmPassword)
            {
                MessageBox.Show("Passwords do not match");
                return;
            }

            var repo = new UserRepository();
            // Проверяем, существует ли уже пользователь с таким именем
            var existingUser = repo.GetUserByUsername(Username);
            if (existingUser != null)
            {
                MessageBox.Show("User already exists");
                return;
            }

            // Создаём нового пользователя
            UserProfile newUser = new UserProfile()
            {
                Username = Username,
                PasswordHash = Password, // В реальном приложении здесь нужно использовать хэширование
                GamesPlayed = 0,
                MaxScore = 0,
                TotalPlayTimeInSeconds = 0
            };

            repo.CreateUser(newUser);
            MessageBox.Show("Registration successful. You can now log in.");

            // Сохраняем текущего пользователя в сессии

            UserSession.CurrentUser = newUser;

            // После успешной регистрации переходим на страницу меню
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.MainFrame.Navigate(new MenuPage());
            }

        }
    }
}
