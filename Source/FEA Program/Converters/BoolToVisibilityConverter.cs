using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace FEA_Program.Converters
{
    /// <summary>
    /// Returns Hidden if an object is false, and vise-versa
    /// </summary>
    internal class BoolToVisibilityConverter : IValueConverter
    {
        public bool Invert { get; set; } = false; // Optional toggle to reverse behavior
        public Visibility HiddenState { get; set; } = Visibility.Hidden;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool bValue = (bool)value;
            if (Invert)
                bValue = !bValue;

            return bValue ? Visibility.Visible : HiddenState;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException(); // One-way only
        }
    }
}
