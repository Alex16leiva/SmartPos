using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SmartPos.Comunes.Converters
{
    // Convierte False -> Visible / True -> Collapsed (Para textos de ayuda en edición)
    public class InverseBoolToVisConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value is bool b && b) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    // Convierte True -> "Nuevo" / False -> "Editar" (Para el título del panel)
    public class NuevoEditarConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value is bool b && b) ? "NUEVO REGISTRO" : "EDITAR REGISTRO";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}