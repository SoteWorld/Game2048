using System;
using System.Windows.Controls;
using Game2048.Commands.Base;
using Game2048.ViewModels.Base;

namespace Game2048.Commands
{
    // Команда для перехода на другую страницу
    public class NavigationCommand : BaseCommand
    {
        private readonly Action<Page, Uri> execute; // Делегат навигации
        private readonly Uri uri;                  // Куда нужно перейти

        // Конструктор получает делегат и URI страницы
        public NavigationCommand(Action<Page, Uri> execute, Uri uri)
        {
            this.execute = execute;
            this.uri = uri;
        }

        // Выполняет переход: вызывает переданную функцию, передаёт текущую страницу и нужный URI
        public override void Execute(object parameter)
        {
            execute.Invoke((Page)parameter, uri);
        }
    }
}
