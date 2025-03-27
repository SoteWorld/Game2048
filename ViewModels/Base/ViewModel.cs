using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Game2048.ViewModels.Base
{
    // Абстрактный базовый класс ViewModel.
    // Реализует интерфейс INotifyPropertyChanged для связи с UI.
    public abstract class ViewModel : INotifyPropertyChanged
    {
        // Событие, которое уведомляет UI об изменении свойства
        public event PropertyChangedEventHandler PropertyChanged;

        // Метод вызова события изменения свойства
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Устанавливает значение поля и вызывает обновление UI
        protected virtual bool Set<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        // Навигация к новой странице (используется в NavigationCommand)
        protected static void NavigateToPage(Page page, Uri uri)
        {
            NavigationService.GetNavigationService(page).Navigate(uri);
        }

        // Завершает работу приложения
        protected static void Quit()
        {
            Application.Current.Shutdown();
        }
    }
}
