using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace Game2048.View.Pages
{
    public partial class RegistrationPage : Page
    {
        public RegistrationPage()
        {
            InitializeComponent();
        }

        // Обработчик для перехода на страницу входа
        private void OnLoginClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new LoginPage());
        }

    }
}
