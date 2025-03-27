using System;
using Game2048.Commands;
using Game2048.ViewModels.Base;

namespace Game2048.ViewModels
{
    // ViewModel для главного меню.
    public class MenuViewModel : ViewModel
    {
        // Команда для перехода на игровую страницу
        public static NavigationCommand NavigateToGamePage =>
            new(NavigateToPage, new Uri("Views/Pages/GamePage.xaml", UriKind.RelativeOrAbsolute));

        // Команда для перехода на страницу статистики
        public static NavigationCommand NavigateToStatisticsPage =>
            new(NavigateToPage, new Uri("Views/Pages/StatisticsPage.xaml", UriKind.RelativeOrAbsolute));

        // Команда для выхода из приложения
        public static RelayCommand QuitApp =>
            new(Quit);
    }
}
