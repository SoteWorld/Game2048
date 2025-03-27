using System;
using Game2048.Commands.Base;

namespace Game2048.Commands
{
    // Команда, которой можно передать произвольное действие без параметров
    public class RelayCommand : BaseCommand
    {
        private readonly Action execute; // Делегат действия, которое нужно выполнить

        // Конструктор получает действие, которое будет выполняться при вызове команды
        public RelayCommand(Action execute)
        {
            this.execute = execute;
        }

        // Метод, который вызывается при активации команды (например, нажатии кнопки)
        public override void Execute(object parameter)
        {
            execute.Invoke(); // Просто вызывает переданное действие
        }
    }
}
