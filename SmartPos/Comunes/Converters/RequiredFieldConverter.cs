using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace SmartPos.Comunes.Converters
{
    public class RequiredFieldConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrWhiteSpace(value?.ToString())
                ? new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 230, 230)) // Rojo suave
                : System.Windows.Media.Brushes.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
