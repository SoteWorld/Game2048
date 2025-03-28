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
    // ViewModel для страницы входа (LoginPage)
    public class LoginViewModel : ViewModel
    {
        private string _username;
        public string Username
        {
            get => _username;
            set => Set(ref _username, value);
        }

        private string _password;
        // Это свойство будет хранить введённый пароль
        public string Password
        {
            get => _password;
            set => Set(ref _password, value);
        }

        // Команда для входа
        public ICommand LoginCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(Login);
        }

        private void Login()
        {
            // Здесь можно использовать либо свойство Password, либо параметр команды.
            var repo = new UserRepository();
            UserProfile user = repo.GetUserByUsername(Username);

            if (user != null && user.VerifyPassword(Password))
            {
                // Сохраняем текущего пользователя в сессии
                UserSession.CurrentUser = user;

                // После успешной регистрации переходим на страницу меню
                var mainWindow = Application.Current.MainWindow as MainWindow;
                if (mainWindow != null)
                {
                    mainWindow.MainFrame.Navigate(new MenuPage());
                }
            }
            else
            {
                MessageBox.Show("Invalid username or password");
            }
        }
    }
}
