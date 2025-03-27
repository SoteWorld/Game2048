using System;
using System.Collections.ObjectModel;

using Game2048.Commands;
using Game2048.Data;
using Game2048.Models;
using Game2048.ViewModels.Base;

namespace Game2048.ViewModels
{
    // ViewModel для страницы статистики
    public class StatisticsViewModel : ViewModel
    {
        // Команда перехода назад в меню
        public static NavigationCommand NavigateToMenuPage =>
            new(NavigateToPage, new Uri("Views/Pages/MenuPage.xaml", UriKind.RelativeOrAbsolute));

        // Коллекция игроков и их результатов
        public static ObservableCollection<Player> StatisticsCollection =>
            Statistics.Players;
    }
}
