using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SmartPos.Comunes.Converters
{
    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Si el valor es nulo, mostramos el mensaje (Visible)
            // Si tiene contenido, lo ocultamos (Collapsed)
            return value == null ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}