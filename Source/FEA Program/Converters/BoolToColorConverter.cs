using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace FEA_Program.Converters
{
    /// <summary>
    /// Converts a boolean value to a color
    /// </summary>
    internal class BoolToColorConverter : IValueConverter
    {
        public Color TrueColor { get; set; } = Colors.Black;
        public Color FalseColor { get; set; } = Colors.Red;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool bValue = (bool)value;

            Color colorToUse = bValue ? TrueColor : FalseColor;
            return new SolidColorBrush(colorToUse);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException(); // One-way only
        }
    }
}
