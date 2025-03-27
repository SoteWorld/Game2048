using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;

namespace Game2048.Converters
{
    // Конвертер, который преобразует двумерный массив int[,] (игровое поле)
    // в ObservableCollection<ObservableCollection<string>> — формат, понятный XAML-привязке
    public class IntArrayToObservableCollectionConverter : IValueConverter
    {
        // Преобразует значение из ViewModel для отображения в UI
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not int[,] intArray)
                return null;

            // Главная коллекция (строки)
            ObservableCollection<ObservableCollection<string>> observableCollection = new();

            for (int i = 0; i < intArray.GetLength(0); i++)
            {
                // Вложенная коллекция (ячейки строки)
                ObservableCollection<string> innerCollection = new();
                for (int j = 0; j < intArray.GetLength(1); j++)
                {
                    // Преобразуем 0 в "" (пустая строка), остальные значения — в текст
                    innerCollection.Add(intArray[i, j] != 0 ? intArray[i, j].ToString() : "");
                }
                observableCollection.Add(innerCollection);
            }

            return observableCollection;
        }

        // Обратное преобразование не реализовано (и не нужно)
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
