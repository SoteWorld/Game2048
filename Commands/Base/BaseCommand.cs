using System;
using System.Windows.Input;

namespace Game2048.Commands.Base
{
    // Абстрактный базовый класс для всех команд.
    // Реализует интерфейс ICommand, который нужен для работы с кнопками в WPF.
    public abstract class BaseCommand : ICommand
    {
        // Событие, уведомляющее UI о том, можно ли выполнить команду (обычно не используется здесь)
        public event EventHandler CanExecuteChanged;

        // Можно ли выполнить команду — всегда true (можно переопределить)
        public virtual bool CanExecute(object parameter) => true;

        // Метод, который должен быть реализован в наследниках — выполняет саму команду
        public abstract void Execute(object parameter);
    }
}
