using System.Windows;
using System.Windows.Controls;

namespace Game2048.Views.Windows
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // Экспонируем Frame как публичное свойство
        public Frame MainFrame => MainWindowFrame;

        public View.Pages.GamePage GamePage
        {
            get => default;
            set
            {
            }
        }

        public View.Pages.MenuPage MenuPage
        {
            get => default;
            set
            {
            }
        }

        public View.Pages.StatisticsPage StatisticsPage
        {
            get => default;
            set
            {
            }
        }

        public View.Pages.LoginPage LoginPage
        {
            get => default;
            set
            {
            }
        }

        public View.Pages.RegistrationPage RegistrationPage
        {
            get => default;
            set
            {
            }
        }
    }
}
