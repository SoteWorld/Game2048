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
    }
}
