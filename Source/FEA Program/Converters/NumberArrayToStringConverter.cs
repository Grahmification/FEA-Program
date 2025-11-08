using System.Globalization;
using System.Windows.Data;

namespace FEA_Program.Converters
{
    /// <summary>
    /// Converts an array of double[] or int[] to a string of the joined values
    /// </summary>
    public class NumberArrayToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double[] arr)
                return string.Join(", ", arr.Select(v => v.ToString("G3"))); // format as needed
            else if (value is int[] arr2)
                return string.Join(", ", arr2.Select(v => v.ToString()));
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException(); // One-way only
        }
    }
}
