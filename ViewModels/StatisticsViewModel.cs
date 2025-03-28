using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using Game2048.Commands;
using Game2048.Models;
using Game2048.Repositories;
using Game2048.ViewModels.Base;

namespace Game2048.ViewModels
{
    // ViewModel для страницы статистики, отображающей данные пользователей из MongoDB
    public class StatisticsViewModel : ViewModel
    {
        // Команда перехода назад в меню
        public static NavigationCommand NavigateToMenuPage =>
            new(NavigateToPage, new Uri("Views/Pages/MenuPage.xaml", UriKind.RelativeOrAbsolute));

        private ObservableCollection<UserProfile> _users;
        public ObservableCollection<UserProfile> Users
        {
            get => _users;
            set => Set(ref _users, value);
        }

        public StatisticsViewModel()
        {
            Refresh();
        }

        // Метод для обновления статистики с сортировкой по убыванию по MaxScore
        public void Refresh()
        {
            var repo = new UserRepository();
            Users = repo.GetAllUsers();

            // Применяем сортировку: максимальный счёт по убыванию
            var view = CollectionViewSource.GetDefaultView(Users);
            view.SortDescriptions.Clear();
            view.SortDescriptions.Add(new SortDescription("MaxScore", ListSortDirection.Descending));
        }
    }
}
