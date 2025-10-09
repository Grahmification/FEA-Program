using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace FEA_Program.Converters
{
    /// <summary>
    /// Returns Collapsed if an object is null, and vise-versa
    /// </summary>
    internal class NullToVisibilityConverter : IValueConverter
    {
        public bool Invert { get; set; } = false; // Optional toggle to reverse behavior

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isNull = value == null;
            if (Invert)
                isNull = !isNull;

            return isNull ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException(); // One-way only
        }
    }
}
